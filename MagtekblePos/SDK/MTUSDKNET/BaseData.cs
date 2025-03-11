// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.BaseData


public class BaseData : IData
{
	private string mStringValue;

	private byte[] mByteArray;

	public string StringValue
	{
		get
		{
			if (mStringValue != null)
			{
				return mStringValue;
			}
			if (mByteArray != null)
			{
				return TLVParser.getHexString(mByteArray);
			}
			return "";
		}
		set
		{
			mStringValue = value;
		}
	}

	public byte[] ByteArray
	{
		get
		{
			if (mByteArray != null)
			{
				return mByteArray;
			}
			if (mStringValue != null)
			{
				return TLVParser.getByteArrayFromHexString(mStringValue);
			}
			return null;
		}
		set
		{
			mByteArray = value;
		}
	}

	public BaseData(string stringValue)
	{
		init(stringValue, null);
	}

	public BaseData(byte[] byteArray)
	{
		init(null, byteArray);
	}

	public BaseData(string stringValue, byte[] byteArray)
	{
		init(stringValue, byteArray);
	}

	protected void init(string stringValue, byte[] byteArray)
	{
		StringValue = null;
		ByteArray = null;
		if (stringValue != null)
		{
			StringValue = (string)stringValue.Clone();
		}
		if (byteArray != null)
		{
			ByteArray = (byte[])byteArray.Clone();
		}
	}

	public IData Clone()
	{
		return new BaseData(StringValue, ByteArray);
	}
}
