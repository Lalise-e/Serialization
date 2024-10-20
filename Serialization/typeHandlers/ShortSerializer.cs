namespace Serialization.typeHandlers
{
	internal class ShortSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(short); } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return BitConverter.ToInt16(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return BitConverter.GetBytes((short)obj);
		}
	}
}
