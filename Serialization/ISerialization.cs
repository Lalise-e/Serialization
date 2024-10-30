namespace Serialization
{
	public interface ISerialization
	{
		public Type SerializationType { get; }
		public bool IsStaticLength {  get; }
		/// <summary>
		/// Bytes of serialized object, should be 0 if <see cref="IsStaticLength"/> is <see langword="false"/>.
		/// </summary>
		public int Length { get; }
		public byte[] Serialize(object? obj);
		public object? Deserialize(byte[] serializedBytes);
	}
}
