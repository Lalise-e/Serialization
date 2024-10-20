using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization.attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ClassSerializationAttribute : Attribute
	{
		public int ClassID { get; set; }
		public ClassSerializationAttribute(int classID)
		{
			ClassID = classID;
		}
	}
}
