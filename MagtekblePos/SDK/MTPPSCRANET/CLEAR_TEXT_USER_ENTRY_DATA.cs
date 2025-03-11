// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.CLEAR_TEXT_USER_ENTRY_DATA
public struct CLEAR_TEXT_USER_ENTRY_DATA
{
	public byte OpStatus;

	public byte UserDataMode;

	public int DataLen;

	private string mData;

	public string Data
	{
		get
		{
			return mData;
		}
		internal set
		{
			mData = value;
			DataLen = mData.Length;
		}
	}

	public void clear()
	{
		DataLen = 0;
		Data = "";
	}

	public override string ToString()
	{
		return $"OpStatus={OpStatus}, UserDataMode={UserDataMode},Data={Data}";
	}
}
