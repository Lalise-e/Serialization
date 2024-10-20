namespace Serialization.typeHandlers
{
	internal class FloatSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(float); } }

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
