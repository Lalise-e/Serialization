namespace Serialization.typeHandlers
{
	public class DecimalSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(decimal); } }

		public bool IsStaticLength {  get { return true; } }

		public int Length { get { return 16; } }

		public unsafe object? Deserialize(byte[] serializedBytes)
		{
			fixed (byte* bPointer = &serializedBytes[0])
			{
				return *(decimal*)(long)bPointer;
			}
		}
		public unsafe byte[] Serialize(object? obj)
		{
			byte[] bytes = new byte[16];
			fixed (byte* bPointer = &bytes[0])
			{
				decimal* dPointer = (decimal*)(long)bPointer;
				*dPointer = (decimal)obj;
			}
			return bytes;
		}
	}
}
