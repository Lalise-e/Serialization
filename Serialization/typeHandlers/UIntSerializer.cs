namespace Serialization.typeHandlers
{
	internal class UIntSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(uint); } }
		public bool IsStaticLength { get { return true; } }

		public int Length { get { return 4; } }

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
