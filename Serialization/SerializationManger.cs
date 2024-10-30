using Serialization.attributes;
using System.Diagnostics;
using System.Reflection;

namespace Serialization
{
	//[LEB128, classID]						<-	header, is -1 if object is null
	//[LEB128, name length]					<-	serialized member, repeated for all marked members
	//[string, memberName]				<-	serialized member, repeated for all marked members
	//[LEB128, length of byte array]		<-	serialized member, repeated for all marked members
	//[byte[], serialized member]		<-	serialized member, repeated for all marked members
	public static class SerializationManager
	{
		private static readonly Dictionary<Type, ISerialization> _serializers = [];
		private static readonly Dictionary<Type, PropertyInfo[]> _propertyInfos = [];
		private static readonly Dictionary<int, Type> _idKey = [];
		private static readonly List<object> _instances = new();
		private static int _depth = 0;
		public static T? LoadObjectFromFile<T>(string path)
		{
			return (T)Deserialize(File.ReadAllBytes(path));
		}
		public static void SaveToFile(string path, object? obj)
		{
			File.WriteAllBytes(path, Serialize(obj));
		}
		public static byte[] Serialize(object obj)
		{
			_depth++;
			startLoad();
			if (obj == null)
				return ((LEB128)0).GetBytes();
			List<byte> result = [];
			if (!_propertyInfos.ContainsKey(obj.GetType()))
			{
				if (!_serializers.ContainsKey(obj.GetType()))
					throw new Exception($"class {obj.GetType()} is missing {nameof(ClassSerializationAttribute)}");
				result.AddRange(((LEB128)(-1)).GetBytes());
				result.AddRange(serializeObject(obj.GetType().AssemblyQualifiedName, true));
				result.AddRange(serializeObject(obj, true));
				_depth--;
				if(_depth == 0)
					_instances.Clear();
				return result.ToArray();
			}

			ClassSerializationAttribute? classAtt =
				obj.GetType().GetCustomAttribute<ClassSerializationAttribute>();
			if (classAtt == null)
				throw new Exception($"class {obj.GetType().Name} is missing {typeof(ClassSerializationAttribute)}");

			result.AddRange(((LEB128)classAtt.ClassID).GetBytes());
			foreach (PropertyInfo info in _propertyInfos[obj.GetType()])
			{
				string name = "";
				if (_instances.Contains(info.GetValue(obj, null)))
					name += "\u0000";
				name += info.GetCustomAttribute<PropertySerializationAttribute>().PropertyName;
				result.AddRange(serializeObject(name,true));
				result.AddRange(serializeObject(info.GetValue(obj), true));
			}
			_depth--;
			if (_depth == 0)
				_instances.Clear();
			return result.ToArray();
		}
		public static T? Deserialize<T>(byte[] bytes)
		{
			return (T)Deserialize(bytes);
		}
		public static object? Deserialize(byte[] obj)
		{
			//I am so terribly sorry to whoever has to read this.
			_depth++;
			startLoad();
			object? result;
			int length;
			int lebLength;
			int classID = LEB128.FromBytes(obj, out int readerHead);
			if (classID == 0)
				return null;
			if(classID == -1)
			{
				length = LEB128.FromBytes(obj[readerHead..], out lebLength);
				readerHead += lebLength;
				string fullName = deserializeObject<string>(obj[readerHead..(readerHead + length)]);
				readerHead += length;
				length = LEB128.FromBytes(obj[readerHead..], out lebLength);
				readerHead += lebLength;
				Type type = Type.GetType(fullName);
				_depth--;
				if (_depth == 0)
					_instances.Clear();
				return deserializeObject(obj[readerHead..(readerHead + length)], type);
			}
			if (!_idKey.ContainsKey(classID))
				throw new Exception($"No class with ID: {classID} exists.");
			Type objectType = _idKey[classID];
			result = objectType.GetConstructor([]).Invoke([]);
			PropertyInfo[] infos = _propertyInfos[objectType];
			PropertyInfo currentMember;
			string memberName = "";
			object? value;
			while (readerHead < obj.Length)
			{
				bool isInstanced = false;
				length = LEB128.FromBytes(obj[readerHead..], out lebLength);
				readerHead += lebLength;
				memberName = deserializeObject<string>(obj[readerHead..(readerHead + length)]);
				readerHead += length;
				if (memberName == null)
					continue;
				if (memberName[0] == '\0')
				{
					isInstanced = true;
					memberName = memberName[1..];
				}
				currentMember = infos.ToList().Find(
					p => p.GetCustomAttribute<PropertySerializationAttribute>().PropertyName == memberName);
				if (currentMember == null)
				{
					Debug.WriteLine($"Type {objectType.Name} is lacking a property with the name {memberName}");
					continue;
				}
				length = LEB128.FromBytes(obj[readerHead..], out lebLength);
				readerHead += lebLength;
				if(length == 0)
				{
					currentMember.SetValue(result, null);
					continue;
				}
				if (isInstanced)
					value = instanceDeserializer(obj[readerHead..(readerHead + length)]);
				else
				value = deserializeObject(obj[readerHead..(readerHead + length)], currentMember.PropertyType);
				readerHead += length;
				currentMember.SetValue(result, value);
			}
			_depth--;
			if (_depth == 0)
				_instances.Clear();
			return result;
		}
		private static byte[] serializeObject(object obj, bool includeLength = false)
		{
			List<byte> result = [];
			Func<object, byte[]> serializer = null;
			LEB128 length = 0;
			if (obj == null)
			{
				result.AddRange(length.GetBytes());
				return result.ToArray();
			}
			if (_instances.Contains(obj))
				serializer = instanceSerializer;
			if (!_serializers.ContainsKey(obj.GetType()))
			{
				ClassSerializationAttribute? att = obj.GetType().GetCustomAttribute<ClassSerializationAttribute>();
				if (att == null && !obj.GetType().IsArray)
					throw new Exception($"type {obj.GetType().Name} is missing a serializer.");
				else if ((att == null) && (serializer == null))
					serializer = arraySerializer;
				else if (serializer == null)
					serializer = Serialize;
			}
			else if (serializer == null)
				serializer = _serializers[obj.GetType()].Serialize;
			byte[] buffer = serializer(obj);
			if (includeLength)
				result.AddRange(((LEB128)buffer.Length).GetBytes());
			result.AddRange(buffer);
			if ((!_instances.Contains(obj)) && obj is not string)
				_instances.Add(obj);
			return result.ToArray();
		}
		private static object? deserializeObject(byte[] data, Type type)
		{
			object? result;
			if (data.Length == 0)
				return null;
			Func<byte[], object?> deserializer;
			if (!_serializers.ContainsKey(type))
			{
				ClassSerializationAttribute? att = type.GetCustomAttribute<ClassSerializationAttribute>();
				if (att == null && !type.IsArray)
					throw new Exception($"type {type.Name} is missing a serializer.");
				else if (att == null)
					deserializer = bytes => arrayDeserializer(bytes, type.GetElementType());
				else
					deserializer = Deserialize;
			}
			else
				deserializer = _serializers[type].Deserialize;
			result = deserializer(data);
			if (result is not string)
				_instances.Add(result);
			return result;
		}
		private static T? deserializeObject<T>(byte[] data)
		{
			return (T)deserializeObject(data, typeof(T));
		}
		private static byte[] arraySerializer(object obj)
		{
			if (!obj.GetType().IsArray)
				throw new ArgumentException("Object is not an array");
			Array array = (Array)obj;
			List<byte> result = [];
			int rank = obj.GetType().GetArrayRank();
			result.AddRange(((LEB128)rank).GetBytes());
			for (int i = 0; i < rank; i++)
			{
				result.AddRange(((LEB128)array.GetLength(i)).GetBytes());
			}
			foreach(object o in array)
			{
				result.AddRange(serializeObject(o, true));
			}
			return result.ToArray();
		}
		private static object arrayDeserializer(byte[] data, Type type)
		{
			int rank = LEB128.FromBytes(data, out int lebLength);
			int reader = lebLength;
			int[] lengths = new int[rank];
			for (int i = 0; i < rank; i++)
			{
				lengths[i] = LEB128.FromBytes(data[reader..], out lebLength);
				reader += lebLength;
			}
			Array array = Array.CreateInstance(type, lengths);
			int[]? index = new int[rank];
			while(index != null)
			{
				int length = LEB128.FromBytes(data[reader..], out lebLength);
				reader += lebLength;
				array.SetValue(deserializeObject(data[reader..(reader+length)],type), index);
				reader += length;
				index = incrementIndex(lengths, index);
			}
			Console.WriteLine();
			return array;
		}
		private static int[]? incrementIndex(int[] lengths, int[] index)
		{
			//I spent way too much time trying to figure out how to do this
			if (index.Length != lengths.Length)
				throw new ArgumentException("Missmatch in dimension count");
			int dimensions = index.Length;
			int currentDimension = dimensions - 1;
		IHateLabels:
			index[currentDimension]++;
			if (index[currentDimension] >= lengths[currentDimension])
			{
				index[currentDimension] = 0;
				currentDimension--;
				if (currentDimension < 0)
					return null;
				goto IHateLabels;
			}
			return index;
		}
		private static byte[] instanceSerializer(object obj)
		{
			if (!_instances.Contains(obj))
				throw new ArgumentException("object as not been cached");
			int index = _instances.IndexOf(obj);
			return ((LEB128)index).GetBytes();
		}
		private static object instanceDeserializer(byte[] data)
		{
			int index = LEB128.FromBytes(data, out int length);
			if (_instances.Count <= index)
				throw new ArgumentException("There is not an object associated with this index");
			return _instances[index];
		}
		private static bool _loaded = false;
		private static void startLoad()
		{
			if (_loaded)
				return;
			_loaded = true;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					checkType(type);
				}
			}
		}
		private static void checkType(Type type)
		{
			//Checks for ISerialization
			bool isSerializer = true;
			List<Type> interfaces = [.. type.GetInterfaces()];
			if (!interfaces.Contains(typeof(ISerialization)))
				isSerializer = false;
			if (type == typeof(ISerialization))
				isSerializer = false;
			if (isSerializer)
				loadInterface(type);

			ClassSerializationAttribute? att = type.GetCustomAttribute<ClassSerializationAttribute>();
			if (att == null)
				return;
			_idKey.Add(att.ClassID, type);
			loadProperties(type);
		}
		private static void loadProperties(Type type)
		{
			List<PropertyInfo> infos = [];
			PropertyInfo[] allProp = type.GetProperties(
				BindingFlags.Instance | BindingFlags.Public| BindingFlags.NonPublic);
			for (int i = 0; i < allProp.Length; i++)
			{
				PropertySerializationAttribute? att = allProp[i].GetCustomAttribute<PropertySerializationAttribute>();
				if (att == null)
					continue;
				infos.Add(allProp[i]);
			}
			_propertyInfos.Add(type, allProp.ToArray());
		}
		private static void loadInterface(Type type)
		{
			ConstructorInfo constructor = type.GetConstructor(Array.Empty<Type>()) ??
				throw new Exception($"{type.FullName} is missing a constructor with 0 arguments");
			ISerialization serializer = (ISerialization)constructor.Invoke(Array.Empty<object>());
			_serializers.Add(serializer.SerializationType, serializer);
			Debug.WriteLine("Serializer loaded for:\t" + serializer.SerializationType.Name);
		}
	}
}
