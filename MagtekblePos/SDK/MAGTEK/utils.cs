// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MagTek.utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal static class utils
{
	private static char[] NibleToHex;

	private static int[] HexToNible;

	static utils()
	{
		HexToNible = new int[256];
		NibleToHex = "0123456789ABCDEF".ToCharArray();
		for (int i = 0; i < 256; i++)
		{
			HexToNible[i] = -1;
		}
		HexToNible[Convert.ToInt32('0')] = 0;
		HexToNible[Convert.ToInt32('1')] = 1;
		HexToNible[Convert.ToInt32('2')] = 2;
		HexToNible[Convert.ToInt32('3')] = 3;
		HexToNible[Convert.ToInt32('4')] = 4;
		HexToNible[Convert.ToInt32('5')] = 5;
		HexToNible[Convert.ToInt32('6')] = 6;
		HexToNible[Convert.ToInt32('7')] = 7;
		HexToNible[Convert.ToInt32('8')] = 8;
		HexToNible[Convert.ToInt32('9')] = 9;
		HexToNible[Convert.ToInt32('A')] = 10;
		HexToNible[Convert.ToInt32('B')] = 11;
		HexToNible[Convert.ToInt32('C')] = 12;
		HexToNible[Convert.ToInt32('D')] = 13;
		HexToNible[Convert.ToInt32('E')] = 14;
		HexToNible[Convert.ToInt32('F')] = 15;
		HexToNible[Convert.ToInt32('a')] = 10;
		HexToNible[Convert.ToInt32('b')] = 11;
		HexToNible[Convert.ToInt32('c')] = 12;
		HexToNible[Convert.ToInt32('d')] = 13;
		HexToNible[Convert.ToInt32('e')] = 14;
		HexToNible[Convert.ToInt32('f')] = 15;
	}

	public static string ByteArrayToHexString(this byte[] data)
	{
		if (data != null && data.Length != 0)
		{
			char[] array = new char[data.Length * 2];
			for (int i = 0; i < data.Length; i++)
			{
				array[2 * i] = NibleToHex[data[i] >> 4];
				array[2 * i + 1] = NibleToHex[data[i] & 0xF];
			}
			return new string(array);
		}
		return "";
	}

	public static byte[] HexStringToByteArray(this string hex)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(hex);
		if (bytes.Length != hex.Length)
		{
			throw new InvalidCastException("Invalid Hex string");
		}
		if (bytes.Length % 2 != 0)
		{
			throw new InvalidCastException("Invalid Hex string length");
		}
		byte[] array = new byte[bytes.Length / 2];
		for (int i = 0; i < array.Length; i++)
		{
			int num = HexToNible[bytes[2 * i]];
			if (num < 0)
			{
				throw new InvalidCastException("Invalid Hex string");
			}
			int num2 = HexToNible[bytes[2 * i + 1]];
			if (num < 0)
			{
				throw new InvalidCastException("Invalid Hex string");
			}
			array[i] = (byte)((num << 4) + num2);
		}
		return array;
	}

	public static T[] SubArray<T>(T[] Source, int index, int length = -1)
	{
		if (Source != null && Source.Length > index)
		{
			if (length == -1)
			{
				length = Source.Length - index;
			}
			T[] array = new T[length];
			Array.Copy(Source, index, array, 0, length);
			return array;
		}
		return null;
	}

	public static ushort GetLittleEndianU8(this string source)
	{
		byte[] array = source.HexStringToByteArray();
		if (array == null)
		{
			return 0;
		}
		return array[0];
	}

	public static ushort GetLittleEndianU16(this string source)
	{
		byte[] array = source.HexStringToByteArray();
		if (array == null)
		{
			return 0;
		}
		if (array.Length > 1)
		{
			return (ushort)(array[1] * 256 + array[0]);
		}
		return array[0];
	}

	public static ulong GetLittleEndianU32(this string source)
	{
		byte[] array = source.HexStringToByteArray();
		if (array == null)
		{
			return 0uL;
		}
		if (array.Length > 3)
		{
			return (ulong)((long)(((ulong)array[3] << 24) + ((ulong)array[2] << 16)) + (long)array[1] * 256L + array[0]);
		}
		if (array.Length > 2)
		{
			return (ulong)((long)((ulong)array[2] << 16) + (long)array[1] * 256L + array[0]);
		}
		if (array.Length > 1)
		{
			return (ushort)(array[1] * 256 + array[0]);
		}
		return array[0];
	}

	public static string HexTLVLength(int Length)
	{
		if (Length >= 16777216)
		{
			return "84" + Length.ToString("X8");
		}
		if (Length >= 65536)
		{
			return "83" + Length.ToString("X6");
		}
		if (Length >= 256)
		{
			return "82" + Length.ToString("X4");
		}
		if (Length >= 128)
		{
			return "81" + Length.ToString("X2");
		}
		return Length.ToString("X2");
	}

	public static string HexStringSwapOrder(string Source)
	{
		return Source.HexStringToByteArray().Reverse().ToArray()
			.ByteArrayToHexString();
	}

	public static string ASCIIStringToHex(this string ASCII)
	{
		return Encoding.ASCII.GetBytes(ASCII).ByteArrayToHexString();
	}

	public static string HexToASCIIString(this string hexString)
	{
		try
		{
			string text = string.Empty;
			for (int i = 0; i < hexString.Length; i += 2)
			{
				_ = string.Empty;
				text += Convert.ToChar(Convert.ToUInt32(hexString.Substring(i, 2), 16));
			}
			return text;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return string.Empty;
	}

	public static bool CompareList<T>(IEnumerable<T> source, IEnumerable<T> target, out IEnumerable<T> match, out IEnumerable<T> unmatch) where T : IComparable<T>
	{
		List<T> intersect = source.Intersect(target, new Equalizer<T>()).ToList();
		source.Where((T x) => !intersect.Contains(x, new Equalizer<T>())).ToList();
		List<T> list = target.Where((T x) => !intersect.Contains(x, new Equalizer<T>())).ToList();
		match = intersect.ToList();
		unmatch = list;
		return list.Count() == 0;
	}

	public static bool CompareList<T>(IEnumerable<T> source, IEnumerable<T> target, out IEnumerable<T> match, out IEnumerable<T> unmatch, Comparer<T> c)
	{
		List<T> intersect = source.Intersect(target, c).ToList();
		source.Where((T x) => !intersect.Contains(x, c)).ToList();
		List<T> list = target.Where((T x) => !intersect.Contains(x, c)).ToList();
		match = intersect.ToList();
		unmatch = list;
		return list.Count() == 0;
	}
}
