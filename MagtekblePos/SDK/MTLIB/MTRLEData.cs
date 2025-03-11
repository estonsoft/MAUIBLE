// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTRLEData
public class MTRLEData
{
	public static byte[] decodeRLEData(byte[] data)
	{
		int decodeRLEDataLength = getDecodeRLEDataLength(data);
		if (decodeRLEDataLength < 1)
		{
			return null;
		}
		byte[] array = new byte[decodeRLEDataLength];
		int num = 0;
		int num2 = 0;
		while (num < data.Length)
		{
			if (num + 1 < data.Length)
			{
				if (data[num] == data[num + 1])
				{
					if (num + 2 < data.Length)
					{
						int num3 = data[num + 2] & 0xFF;
						for (int i = 0; i < num3; i++)
						{
							array[num2++] = data[num];
						}
						num += 3;
					}
				}
				else
				{
					array[num2++] = data[num++];
				}
			}
			else
			{
				array[num2++] = data[num++];
			}
		}
		return array;
	}

	private static int getDecodeRLEDataLength(byte[] data)
	{
		int num = 0;
		int num2 = 0;
		while (num2 < data.Length)
		{
			if (num2 + 1 < data.Length)
			{
				if (data[num2] == data[num2 + 1])
				{
					if (num2 + 2 < data.Length)
					{
						num += data[num2 + 2] & 0xFF;
						num2 += 3;
					}
				}
				else
				{
					num++;
					num2++;
				}
			}
			else
			{
				num++;
				num2++;
			}
		}
		return num;
	}
}
