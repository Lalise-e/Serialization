namespace Serialization.typeHandlers
{
	internal class UShortSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(ushort); } }
		public bool IsStaticLength { get { return true; } }

		public int Length { get { return 2; } }

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
