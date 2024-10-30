namespace Serialization.typeHandlers
{
	internal class FloatSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(float); } }

		public bool IsStaticLength { get { return true; } }

		public int Length { get { return 4; } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return BitConverter.ToSingle(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return BitConverter.GetBytes((float)obj);
		}
	}
}
