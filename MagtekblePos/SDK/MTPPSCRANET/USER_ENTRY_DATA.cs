// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.USER_ENTRY_DATA
public struct USER_ENTRY_DATA
{
	internal string mKSN;

	internal string mEDB;

	public byte OpStatus;

	public string MSRKsn
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

	public string EDB
	{
		get
		{
			return mEDB;
		}
		internal set
		{
			mEDB = value;
		}
	}

	public void clear()
	{
		EDB = "";
		MSRKsn = "";
	}

	public override string ToString()
	{
		return $"operation status = {OpStatus}\nKSN = {MSRKsn}\nEDB = {EDB}\n";
	}
}
