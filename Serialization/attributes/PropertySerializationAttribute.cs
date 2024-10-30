using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization.attributes
{
	public class PropertySerializationAttribute : Attribute
	{
		public string PropertyName {  get; set; }
		public PropertySerializationAttribute(string propertyName)
		{
			if (propertyName[0] == '\0')
				throw new ArgumentException($"{nameof(propertyName)} cannot start with character \u0000");
			PropertyName = propertyName;
		}
	}
}
