namespace Serialization.typeHandlers
{
	internal class UShortSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(ushort); } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return BitConverter.ToUInt16(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return BitConverter.GetBytes((ushort)obj);
		}
	}
}
