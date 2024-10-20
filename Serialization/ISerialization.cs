using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
	public interface ISerialization
	{
		public Type SerializationType { get; }
		public byte[] Serialize(object? obj);
		public object? Deserialize(byte[] serializedBytes);
	}
}
