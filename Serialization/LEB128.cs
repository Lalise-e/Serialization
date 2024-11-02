using System.Diagnostics;

namespace Serialization
{
	public struct LEB128
	{
		private byte[] _number = [128];
		public LEB128(long value)
		{
			intToLEB128(value);
		}
		private void intToLEB128(long value)
		{
			if (value == long.MinValue)
				throw new ArgumentException($"Yeah sorry, I big dumb dumb, {long.MinValue} does not work with LEB128");
			ulong newValue = (ulong)value;
			int bytes = 0;
			//This just moves the signed bit to be the least significant bit instead of the most significant one
			//I will probably regret this.
			if (value<0)
			{
				//So, I was not actually aware of how negative intergers were stored prior to this,
				//I just assumed the MSB denoted wheather or not the number was negative and that everything
				//else was normal, but no, the negative number is the complement to its positive counterpart.
				newValue = (newValue << 1) + 1;
			}
			else
				newValue = newValue << 1;
			ulong valueCopy = newValue;
			while (true)
			{
				bytes += 1;
				valueCopy = valueCopy >> 7;
				if (valueCopy == 0)
					break;
			}
			_number = new byte[bytes];
			for (int i = 0; i < _number.Length; i++)
			{
				_number[i] = (byte)((newValue >> (7 * i)) & 127);
				if (i + 1 == _number.Length)
					_number[i] += 128;
			}
		}
		private long leb128ToInt64()
		{
			if (_number == null)
				_number = [128];
			ulong value = 0;
			for (int i = 0; i < _number.Length; i++)
			{
				value += (ulong)(_number[i] & 127) << (i * 7);
			}
			if (value % 2 == 0)
				return (long)(value >> 1);
			value = (value >> 1) + ((ulong)1 << 63);
			return (long)~value;
		}
		public byte[] GetBytes()
		{
			if (_number == null)
				return [128];
			return _number;
		}
		public static LEB128 FromBytes(byte[] bytes, out int length)
		{
			length = 0;
			while (true)
			{
				length++;
				if (length > 10)
					throw new FormatException($"{nameof(bytes)} is not a valid LEB128");
				if ((bytes[length - 1] & 128) == 128)
					break;
			}
			LEB128 result = new LEB128();
			result._number = bytes[..length];
			return result;
		}
		public static implicit operator LEB128(long value) {  return new LEB128(value); }
		public static implicit operator LEB128(int value) { return new LEB128(value); }
		public static implicit operator long(LEB128 value) { return value.leb128ToInt64(); }
		public static implicit operator int(LEB128 value) { return (int)value.leb128ToInt64(); }
		public static implicit operator byte[](LEB128 value) { return value.GetBytes(); }
	}
}
