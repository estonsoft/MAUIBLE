// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.TLVParser
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class TLVParser
{
	private const string hexDigits = "0123456789ABCDEF";

	private const byte MoreTagBytesFlag1 = 31;

	private const byte MoreTagBytesFlag2 = 128;

	private const byte ConstructedFlag = 32;

	private const byte MultiByteLengthFlag = 128;

	private const byte OneByteLengthMask = 127;

	public static string getHexString(byte data)
	{
		return getHexString(new byte[1] { data });
	}

	public static string getHexString(byte[] data)
	{
		string result = "";
		if (data != null && data.Length != 0)
		{
			StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
			try
			{
				foreach (byte b in data)
				{
					stringBuilder.AppendFormat("{0:X2}", b);
				}
			}
			catch (Exception)
			{
			}
			result = stringBuilder.ToString();
		}
		return result;
	}

	public static byte[] getByteArrayFromHexString(string str)
	{
		if (str.Length >> 1 < 1)
		{
			return null;
		}
		byte[] array = new byte[str.Length >> 1];
		try
		{
			for (int i = 0; i < str.Length; i += 2)
			{
				int num = "0123456789ABCDEF".IndexOf(char.ToUpperInvariant(str[i]));
				int num2 = "0123456789ABCDEF".IndexOf(char.ToUpperInvariant(str[i + 1]));
				if (num != -1)
				{
					_ = -1;
				}
				array[i >> 1] = (byte)((num << 4) | num2);
			}
		}
		catch (Exception)
		{
		}
		return array;
	}

	public static string getLengthHexString(int len)
	{
		return getHexString(getLengthByteArray(len));
	}

	public static byte[] getLengthByteArray(int len)
	{
		byte[] array;
		if (len < 128)
		{
			array = new byte[1] { (byte)len };
		}
		else
		{
			int i;
			for (i = 1; (double)len / Math.Pow(256.0, i) >= 1.0; i++)
			{
			}
			array = new byte[i + 1];
			array[0] = (byte)(128 + i);
			int num = i;
			for (int j = 0; j < i; j++)
			{
				num--;
				array[j + 1] = (byte)((len >> num * 8) & 0xFF);
			}
		}
		return array;
	}

	public static byte[] getTwoByteLengthArray(int len)
	{
		byte[] array = new byte[2];
		int num = 2;
		for (int i = 0; i < 2; i++)
		{
			num--;
			array[i] = (byte)((len >> num * 8) & 0xFF);
		}
		return array;
	}

	public static byte[] addTwoByteLength(byte[] data)
	{
		int len = 0;
		if (data != null)
		{
			len = data.Length;
		}
		byte[] twoByteLengthArray = getTwoByteLengthArray(len);
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(twoByteLengthArray);
		if (data != null)
		{
			binaryWriter.Write(data);
		}
		return memoryStream.ToArray();
	}

	public static byte[] getByteArrayFromASCIIString(string ASCII)
	{
		return Encoding.ASCII.GetBytes(ASCII);
	}

	public static bool isPrimitiveTagHexString(string tagHexString)
	{
		return isPrimitiveTagByteArray(getByteArrayFromHexString(tagHexString));
	}

	public static bool isPrimitiveTagByteArray(byte[] tagByteArray)
	{
		return !isConstructedTagByteArray(tagByteArray);
	}

	public static bool isConstructedTagHexString(string tagHexString)
	{
		return isConstructedTagByteArray(getByteArrayFromHexString(tagHexString));
	}

	public static bool isConstructedTagByteArray(byte[] tagByteArray)
	{
		bool result = false;
		if (tagByteArray != null && tagByteArray.Length != 0)
		{
			result = (tagByteArray[0] & 0x20) == 32;
		}
		return result;
	}

	public static TLVObject findFromListByTagHexString(List<TLVObject> tlvObjectList, string tlvTagHexString)
	{
		return findFromListByTagByteArray(tlvObjectList, getByteArrayFromHexString(tlvTagHexString));
	}

	public static TLVObject findFromListByTagByteArray(List<TLVObject> tlvObjectList, byte[] tlvTagByteArray)
	{
		TLVObject tLVObject = null;
		if (tlvObjectList != null)
		{
			foreach (TLVObject tlvObject in tlvObjectList)
			{
				if (tlvObject != null)
				{
					tLVObject = tlvObject.findByTagByteArray(tlvTagByteArray);
					if (tLVObject != null)
					{
						return tLVObject;
					}
				}
			}
		}
		return tLVObject;
	}

	public static List<TLVObject> parseTLVHexString(string tlvHexString)
	{
		return parseTLVByteArray(getByteArrayFromHexString(tlvHexString));
	}

	public static List<TLVObject> parseTLVByteArray(byte[] tlvByteArray)
	{
		List<TLVObject> list = new List<TLVObject>();
		if (tlvByteArray != null)
		{
			byte[] array = null;
			bool flag = true;
			int num = 0;
			while (num < tlvByteArray.Length)
			{
				byte b;
				if (flag)
				{
					int num2 = 0;
					bool flag2 = true;
					MemoryStream memoryStream = new MemoryStream();
					BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
					while (flag2 && num < tlvByteArray.Length)
					{
						b = tlvByteArray[num];
						binaryWriter.Write(b);
						num++;
						flag2 = ((num2 != 0) ? ((b & 0x80) == 128) : ((b & 0x1F) == 31));
						num2++;
					}
					array = memoryStream.ToArray();
					flag = false;
					continue;
				}
				b = tlvByteArray[num];
				int num4;
				if ((b & 0x80) == 128)
				{
					int num3 = b & 0x7F;
					num++;
					int i = 0;
					num4 = 0;
					for (; i < num3; i++)
					{
						if (num >= tlvByteArray.Length)
						{
							break;
						}
						b = tlvByteArray[num];
						num++;
						num4 = (num4 << 8) + (b & 0xFF);
					}
				}
				else
				{
					num4 = b & 0x7F;
					num++;
				}
				byte[] array2 = null;
				if (num4 > 0)
				{
					int num5 = num + num4;
					if (num5 > tlvByteArray.Length)
					{
						num5 = tlvByteArray.Length;
					}
					int num6 = num5 - num;
					if (num6 > 0)
					{
						array2 = new byte[num6];
						Array.Copy(tlvByteArray, num, array2, 0, num6);
					}
					num += num6;
				}
				if (isConstructedTagByteArray(array))
				{
					TLVObject tLVObject = new TLVObject(array);
					if (array2 != null)
					{
						List<TLVObject> list2 = parseTLVByteArray(array2);
						if (list2 != null)
						{
							foreach (TLVObject item in list2)
							{
								tLVObject.addTLVObject(item);
							}
						}
					}
					list.Add(tLVObject);
				}
				else
				{
					list.Add(new TLVObject(array, array2));
				}
				flag = true;
			}
		}
		return list;
	}
}
