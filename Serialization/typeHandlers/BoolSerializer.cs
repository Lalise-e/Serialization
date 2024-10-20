namespace Serialization.typeHandlers
{
	internal class BoolSerializer : ISerialization
	{
		public Type SerializationType { get {  return typeof(bool); } }

		public object? Deserialize(byte[] serializedBytes)
		{
			if (serializedBytes[0] == 1)
				return true;
			return false;
		}

		public byte[] Serialize(object? obj)
		{
			if ((bool)obj)
				return [1];
			return [0];
		}
	}
}
