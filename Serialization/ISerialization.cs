namespace Serialization
{
	public interface ISerialization
	{
		public Type SerializationType { get; }
		public byte[] Serialize(object? obj);
		public object? Deserialize(byte[] serializedBytes);
	}
}
