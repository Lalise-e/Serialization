namespace Serialization.typeHandlers
{
	internal class DoubleSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(double); } }
		public object? Deserialize(byte[] serializedBytes)
		{
			return BitConverter.ToDouble(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return BitConverter.GetBytes((double)obj);
		}
	}
}
