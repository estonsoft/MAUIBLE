// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.PIN_DATA
public struct PIN_DATA
{
	public byte OpStatus;

	private string mKSN;

	private string mEPB;

	public string KSN
	{
		get
		{
			return mKSN;
		}
		internal set
		{
			mKSN = value;
		}
	}

	public string EPB
	{
		get
		{
			return mEPB;
		}
		internal set
		{
			mEPB = value;
		}
	}

	public void clear()
	{
		mKSN = "";
		mEPB = "";
	}

	public override string ToString()
	{
		return $"operation status = {OpStatus}\nKSN = {KSN}\nEPB = {EPB}\n";
	}

	public string ToSeparatedString(string separator)
	{
		return KSN + separator + EPB + separator + OpStatus;
	}
}
