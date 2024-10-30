namespace Serialization.typeHandlers
{
	internal class IntSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(int); } }

		public bool IsStaticLength { get { return true; } }

		public int Length { get { return 4; } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return BitConverter.ToInt32(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return BitConverter.GetBytes((int)obj);
		}
	}
}
