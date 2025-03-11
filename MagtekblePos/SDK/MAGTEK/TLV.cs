// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MagTek.TLV
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

internal class TLV
{
	private const int MoreTagBytesFlag1 = 31;

	private const int MoreTagBytesFlag2 = 128;

	private const int ContructedFlag = 32;

	private const int MoreLengthFlag = 128;

	private const int OneByteLengthMask = 127;

	private const int MAX_TAG_LENGTH = 50;

	private string first = "";

	private string second = "";

	private int len;

	public string Tag => first;

	public string Value => second;

	public TLV(string tag, string value, int length = -1)
	{
		first = tag;
		second = value;
		if (length < 0)
		{
			len = (second.Length + 1) / 2;
		}
		else
		{
			len = length;
		}
	}

	private static void debug(string message)
	{
	}

	private static string ToHexString(byte[] data, int offset, int len)
	{
		try
		{
			if (data != null)
			{
				if (len <= 0)
				{
					len = data.Length;
				}
				StringBuilder stringBuilder = new StringBuilder(len * 2);
				for (int i = 0; i < len; i++)
				{
					stringBuilder.AppendFormat("{0:X2}", data[offset + i]);
				}
				return stringBuilder.ToString();
			}
		}
		catch (Exception ex)
		{
			debug(ex.Message);
		}
		return "";
	}

	private static byte[] ToByteArray(string hexString)
	{
		if (hexString.Length % 2 != 0)
		{
			return new byte[0];
		}
		byte[] array = new byte[hexString.Length / 2];
		for (int i = 0; i < array.Length; i++)
		{
			string s = hexString.Substring(i * 2, 2);
			array[i] = byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		}
		return array;
	}

	public static List<TLV> parseTLVData(byte[] data)
	{
		int num = data.Length;
		List<TLV> list = new List<TLV>();
		if (data != null)
		{
			int num2 = 0;
			bool flag = true;
			string text = null;
			byte[] array = new byte[50];
			while (num2 < num)
			{
				byte b = data[num2];
				if (flag)
				{
					int num3 = 0;
					bool flag2 = true;
					while (flag2 && num2 < num)
					{
						b = data[num2];
						num2++;
						array[num3] = b;
						flag2 = ((num3 != 0) ? ((b & 0x80) == 128) : ((b & 0x1F) == 31));
						num3++;
					}
					text = ToHexString(array, 0, num3);
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
						if (num2 >= num)
						{
							break;
						}
						b = data[num2];
						num2++;
						num4 = ((num4 & 0xFF) << 8) + b;
					}
				}
				else
				{
					num4 = b & 0x7F;
					num2++;
				}
				if (text != null)
				{
					if ((array[0] & 0x20) == 32)
					{
						list.Add(new TLV(text, "[Container]", num4));
					}
					else
					{
						int num6 = num2 + num4;
						if (num6 > num)
						{
							num6 = num;
						}
						int num7 = num6 - num2;
						string value = "";
						if (num7 > 0)
						{
							value = ToHexString(data, num2, num7);
						}
						list.Add(new TLV(text, value, num4));
						num2 += num4;
					}
				}
				flag = true;
			}
		}
		return list;
	}

	public static List<TLV> parseTLVData(string data)
	{
		return parseTLVData(ToByteArray(data));
	}

	public static string getTagValue(List<TLV> tlvList, string tagString)
	{
		string text = tagString.ToUpper();
		foreach (TLV tlv in tlvList)
		{
			if (text.Equals(tlv.first))
			{
				return tlv.second;
			}
		}
		return "";
	}

	public string getFormattedTagString()
	{
		return "[" + first + "] [" + len + "] " + second;
	}
}
