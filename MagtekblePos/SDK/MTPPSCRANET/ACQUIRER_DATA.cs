// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.ACQUIRER_DATA
using System;

public struct ACQUIRER_DATA
{
	public byte OpStatus;

	internal byte[] Data;

	internal int Length;

	public byte[] getData()
	{
		if (Data != null)
		{
			return Data;
		}
		return new byte[0];
	}

	public ACQUIRER_DATA(int size)
	{
		OpStatus = 0;
		Length = size;
		Data = null;
	}

	public void release()
	{
	}

	public override string ToString()
	{
		if (Data == null)
		{
			return "Empty Acquirer Data\n";
		}
		return "Status = " + OpStatus + " Acquirer Data: " + BitConverter.ToString(getData());
	}
}
