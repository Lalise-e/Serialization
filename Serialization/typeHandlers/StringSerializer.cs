using System.Text;

namespace Serialization.typeHandlers
{
	internal class StringSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(string); } }
		public bool IsStaticLength { get { return false; } }

		public int Length { get { return 0; } }

		public object? Deserialize(byte[] serializedBytes)
		{
			return Encoding.UTF8.GetString(serializedBytes);
		}

		public byte[] Serialize(object? obj)
		{
			return Encoding.UTF8.GetBytes((string)obj);
		}
	}
}
