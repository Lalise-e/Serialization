
namespace Serialization.typeHandlers
{
	internal class ByteArraySerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(byte[]); } }

		public bool IsStaticLength { get { return false; } }

		public int Length { get { return 0; } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return serializedBytes;
		}

		public byte[] Serialize(object? obj)
		{
			return (byte[])obj;
		}
	}
}
