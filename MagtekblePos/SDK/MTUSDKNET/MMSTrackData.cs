// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSTrackData


public class MMSTrackData
{
	protected TLVObject mTLVObject;

	public MMSTrackData()
	{
	}

	public MMSTrackData(TLVObject tLVObject)
	{
		mTLVObject = tLVObject;
	}

	public TLVObject getObjectByTag(string tag)
	{
		TLVObject result = null;
		if (mTLVObject != null)
		{
			result = mTLVObject.findByTagHexString(tag);
		}
		return result;
	}

	public byte[] getValueByTag(string tag)
	{
		byte[] result = null;
		TLVObject objectByTag = getObjectByTag(tag);
		if (objectByTag != null)
		{
			result = objectByTag.getValueByteArray();
		}
		return result;
	}

	public byte[] getDataDescriptor()
	{
		return getValueByTag("81");
	}

	public byte[] getData()
	{
		return getValueByTag("84");
	}
}
