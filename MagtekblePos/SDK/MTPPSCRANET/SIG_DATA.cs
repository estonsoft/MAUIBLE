// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.SIG_DATA
using System;

public struct SIG_DATA
{
	public byte[] Data;

	public int Sig_Length;

	public SIG_DATA(byte[] data, int size)
	{
		Sig_Length = size;
		Data = null;
	}

	public byte[] getData()
	{
		if (Data != null)
		{
			return Data;
		}
		return new byte[0];
	}

	public void release()
	{
	}

	public override string ToString()
	{
		if (Data == null)
		{
			return "Empty Signature\n";
		}
		return "Signature Data: " + BitConverter.ToString(getData());
	}
}
