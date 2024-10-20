namespace Serialization.typeHandlers
{
	internal class CharSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(char); } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return (char)new UShortSerializer().Deserialize(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return new UShortSerializer().Serialize(obj);
		}
	}
}
