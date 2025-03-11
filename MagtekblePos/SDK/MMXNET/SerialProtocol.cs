// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.SerialProtocol
using System;
using System.Collections.Generic;


internal class SerialProtocol
{
	public uint maxDataSize = uint.MaxValue;

	public event EventHandler<bool> Resend;

	public SerialProtocol(uint size = uint.MaxValue)
	{
		maxDataSize = size;
	}

	private byte[] buildOnePack(byte[] data, uint offset, uint Length)
	{
		byte[] array = new byte[data.Length + 5];
		array[0] = 0;
		array[1] = (byte)(Length >> 24);
		array[2] = (byte)(Length >> 16);
		array[3] = (byte)(Length >> 8);
		array[4] = (byte)Length;
		Array.Copy(data, offset, array, 5L, data.Length);
		return array;
	}

	public IEnumerable<byte[]> buildPackets(byte[] data)
	{
		uint offset = 0u;
		while (offset < data.Length)
		{
			uint packetsize = (uint)(data.Length - offset);
			if (packetsize > maxDataSize)
			{
				yield return buildOnePack(data, offset, maxDataSize);
				offset += maxDataSize;
			}
			else
			{
				yield return buildOnePack(data, offset, packetsize);
				offset += packetsize;
			}
		}
	}

	public byte[] processIncomingData(byte[] data)
	{
		try
		{
			if (data == null || data.Length <= 5)
			{
				return null;
			}
			if (data[0] == 2)
			{
				int num = (data[1] << 24) + (data[2] << 16) + (data[3] << 8) + data[4];
				byte[] array = new byte[num];
				Array.Copy(data, 5, array, 0, num);
				return array;
			}
			if (data[0] == 3 && data.Length == 9)
			{
				Debug.LogProtocol($"Adjust protocol buffer size to 0x{maxDataSize:X8}");
				maxDataSize = (uint)((data[5] << 24) + (data[6] << 16) + (data[7] << 8) + data[8]);
				this.Resend?.Invoke(this, e: true);
			}
		}
		catch
		{
		}
		return null;
	}
}
