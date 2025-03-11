// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTParser
using System;
using System.Collections.Generic;
using System.Text;

public class MTParser
{
	private const string hexDigits = "0123456789ABCDEF";

	public static List<Dictionary<string, string>> parseMTTLVData(byte[] data, bool hasSizeHeader)
	{
		List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
		if (data != null && data.Length >= 2)
		{
			int num = data.Length;
			byte[] array = data;
			if (hasSizeHeader)
			{
				num = ((data[0] & 0xFF) << 8) + (data[1] & 0xFF);
				array = new byte[num];
				Array.Copy(data, 2, array, 0, num);
			}
			if (array != null)
			{
				byte[] array2 = null;
				byte[] array3 = new byte[50];
				bool flag = true;
				int num2 = 0;
				while (num2 < array.Length)
				{
					byte b = array[num2];
					if (flag)
					{
						int num3 = 0;
						for (bool flag2 = true; flag2 && num2 < array.Length; flag2 = (b & 0x80) == 128)
						{
							b = array[num2];
							num2++;
							array3[num3] = b;
							num3++;
						}
						array2 = new byte[num3];
						Array.Copy(array3, array2, num3);
						flag = false;
						continue;
					}
					int num4 = 0;
					if ((b & 0x80) == 128)
					{
						int num5 = b & 0x7F;
						num2++;
						for (int i = 0; i < num5; i++)
						{
							if (num2 >= array.Length)
							{
								break;
							}
							b = array[num2];
							num2++;
							num4 = ((num4 & 0xFF) << 8) + (b & 0xFF);
						}
					}
					else
					{
						num4 = b & 0x7F;
						num2++;
					}
					if (array2 != null)
					{
						_ = array2.Length;
						byte num6 = array2[0];
						bool flag3 = (num6 & 0x20) == 32;
						bool flag4 = (num6 & 0xC0) == 192;
						if (flag3 || flag4)
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							dictionary.Add("tag", getHexString(array2));
							dictionary.Add("len", string.Concat(num4));
							dictionary.Add("value", "[Container]");
							list.Add(dictionary);
						}
						else
						{
							int num7 = num2 + num4;
							if (num7 > array.Length)
							{
								num7 = array.Length;
							}
							byte[] array4 = null;
							int num8 = num7 - num2;
							if (num8 > 0)
							{
								array4 = new byte[num8];
								Array.Copy(array, num2, array4, 0, num8);
							}
							Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
							dictionary2.Add("tag", getHexString(array2));
							dictionary2.Add("len", string.Concat(num4));
							if (array4 != null)
							{
								dictionary2.Add("value", getHexString(array4));
							}
							else
							{
								dictionary2.Add("value", "");
							}
							list.Add(dictionary2);
							num2 += num4;
						}
					}
					flag = true;
				}
			}
		}
		return list;
	}

	public static string getHexString(byte[] data)
	{
		StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
		try
		{
			foreach (byte b in data)
			{
				stringBuilder.AppendFormat("{0:X2}", new object[1] { b });
			}
		}
		catch (Exception)
		{
		}
		return stringBuilder.ToString();
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
				if (num == -1 || num2 == -1)
				{
					throw new ArgumentException("The string contains an invalid digit.", "s");
				}
				array[i >> 1] = (byte)((num << 4) | num2);
			}
		}
		catch (Exception)
		{
		}
		return array;
	}

	public static string getFourByteLengthString(int length)
	{
		return getHexString(new byte[4]
		{
			(byte)((length >> 24) & 0xFF),
			(byte)((length >> 16) & 0xFF),
			(byte)((length >> 8) & 0xFF),
			(byte)(length & 0xFF)
		});
	}

	public static string getTwoByteLengthString(int length)
	{
		return getHexString(new byte[2]
		{
			(byte)((length >> 8) & 0xFF),
			(byte)(length & 0xFF)
		});
	}

	public static string getOneByteLengthString(int length)
	{
		return getHexString(new byte[1] { (byte)(length & 0xFF) });
	}
}
