namespace Serialization.typeHandlers
{
	internal class SByteSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(sbyte); } }
		public bool IsStaticLength { get { return true; } }

		public int Length { get { return 1; } }

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
