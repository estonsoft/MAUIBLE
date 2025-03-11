// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXMessage
public class MMXMessage
{
	protected int mTag;

	protected byte[] mData;

	public MMXMessage(int tag, byte[] data)
	{
		mTag = tag;
		mData = data;
	}

	public int getTag()
	{
		return mTag;
	}

	public byte[] getData()
	{
		return mData;
	}
}
