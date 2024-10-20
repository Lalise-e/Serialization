namespace Serialization.typeHandlers
{
	internal class SByteSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(sbyte); } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return (sbyte)serializedBytes[0];
		}

		public byte[] Serialize(object? obj)
		{
			return [(byte)(sbyte)obj];
		}
	}
}
