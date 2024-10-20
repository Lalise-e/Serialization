
namespace Serialization.typeHandlers
{
	internal class LongSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(long); } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return BitConverter.ToInt64(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return BitConverter.GetBytes((long)obj);
		}
	}
}
