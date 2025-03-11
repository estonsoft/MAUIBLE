// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTCRC16
using System;

internal class MTCRC16
{
	private const ushort polynomial = 40961;

	private ushort[] table = new ushort[256];

	protected ushort CalculateChecksum(byte[] bytes)
	{
		ushort num = 0;
		for (int i = 0; i < bytes.Length; i++)
		{
			byte b = (byte)(num ^ bytes[i]);
			num = (ushort)((num >> 8) ^ table[b]);
		}
		return num;
	}

	public byte[] GetCRCBytes(byte[] bytes)
	{
		byte[] bytes2 = BitConverter.GetBytes(CalculateChecksum(bytes));
		if (bytes2 != null && bytes2.Length == 2)
		{
			return new byte[2]
			{
				bytes2[1],
				bytes2[0]
			};
		}
		return null;
	}

	public MTCRC16()
	{
		for (ushort num = 0; num < table.Length; num++)
		{
			ushort num2 = 0;
			ushort num3 = num;
			for (byte b = 0; b < 8; b++)
			{
				num2 = ((((num2 ^ num3) & 1) == 0) ? ((ushort)(num2 >> 1)) : ((ushort)((num2 >> 1) ^ 0xA001)));
				num3 >>= 1;
			}
			table[num] = num2;
		}
	}
}
