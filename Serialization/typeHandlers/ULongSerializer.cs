namespace Serialization.typeHandlers
{
	internal class ULongSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(ulong); } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return BitConverter.ToUInt64(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return BitConverter.GetBytes((ulong)obj);
		}
	}
}
