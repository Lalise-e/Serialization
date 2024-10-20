namespace Serialization.typeHandlers
{
	internal class IntSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(int); } }

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
