// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.EMV_DATA
using System;

public struct EMV_DATA
{
	public byte OpStatus;

	internal byte[] Data;

	public int Length;

	public byte[] getData()
	{
		if (Data != null)
		{
			return Data;
		}
		return new byte[0];
	}

	public EMV_DATA(int size)
	{
		OpStatus = 0;
		Length = size;
		Data = null;
	}

	public void release()
	{
		Data = null;
		Length = 0;
	}

	public override string ToString()
	{
		if (Data == null)
		{
			return "Empty EMV Data\n";
		}
		return "EMV Data: " + BitConverter.ToString(getData());
	}
}
