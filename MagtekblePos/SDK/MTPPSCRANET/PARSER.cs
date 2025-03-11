// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.PARSER
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

internal abstract class PARSER
{
	public int Status;

	public static string toHexString(byte[] data, int index, int len)
	{
		try
		{
			if (data != null)
			{
				int num = index + len;
				StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
				for (int i = index; i < num; i++)
				{
					stringBuilder.AppendFormat("{0:X2}", data[i]);
				}
				return stringBuilder.ToString();
			}
		}
		catch (Exception)
		{
		}
		return "";
	}

	public static byte[] toByteArray(string hexString)
	{
		if (hexString.Length % 2 != 0)
		{
			return new byte[0];
		}
		if (!Regex.IsMatch(hexString, "^[a-fA-F0-9]*$"))
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

	public abstract int parse(byte[] data, int dataLen);
}
