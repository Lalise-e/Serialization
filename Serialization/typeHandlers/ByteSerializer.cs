namespace Serialization.typeHandlers
{
	internal class ByteSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(byte); } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return serializedBytes[0];
		}
		public byte[] Serialize(object? obj)
		{
			return [(byte)obj];
		}
	}
}
