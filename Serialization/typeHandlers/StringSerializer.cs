
using System.Text;

namespace Serialization.typeHandlers
{
	internal class StringSerializer : ISerialization
	{
		public Type SerializationType { get { return typeof(string); } }

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
