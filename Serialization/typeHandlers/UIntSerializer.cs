namespace Serialization.typeHandlers
{
	internal class UIntSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(uint); } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return BitConverter.ToUInt32(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return BitConverter.GetBytes((uint)obj);
		}
	}
}
