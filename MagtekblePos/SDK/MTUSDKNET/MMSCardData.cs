// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSCardData
using System.Collections.Generic;


public class MMSCardData
{
	protected TLVObject mTLVObject;

	public MMSCardData()
	{
	}

	public MMSCardData(TLVObject tLVObject)
	{
		mTLVObject = tLVObject;
	}

	public TLVObject getObjectByTag(string tag)
	{
		TLVObject result = null;
		if (mTLVObject != null)
		{
			result = mTLVObject.findChildByTagHexString(tag);
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

	public byte[] getCardDescriptor()
	{
		return getValueByTag("81");
	}

	public List<MMSTrackData> getTracks()
	{
		List<MMSTrackData> list = new List<MMSTrackData>();
		TLVObject objectByTag = getObjectByTag("A4");
		if (objectByTag != null)
		{
			List<TLVObject> valueTLVObjectList = objectByTag.getValueTLVObjectList();
			if (valueTLVObjectList != null)
			{
				foreach (TLVObject item in valueTLVObjectList)
				{
					if (item != null)
					{
						byte[] tagByteArray = item.getTagByteArray();
						if (tagByteArray != null && tagByteArray.Length != 0 && tagByteArray[0] == 160)
						{
							list.Add(new MMSTrackData(item));
						}
					}
				}
			}
		}
		return list;
	}

	public List<MMSEncryptedBlock> getEncryptedBlocks()
	{
		List<MMSEncryptedBlock> list = new List<MMSEncryptedBlock>();
		TLVObject objectByTag = getObjectByTag("A6");
		if (objectByTag != null)
		{
			List<TLVObject> valueTLVObjectList = objectByTag.getValueTLVObjectList();
			if (valueTLVObjectList != null)
			{
				foreach (TLVObject item in valueTLVObjectList)
				{
					if (item != null)
					{
						byte[] tagByteArray = item.getTagByteArray();
						if (tagByteArray != null && tagByteArray.Length != 0 && tagByteArray[0] == 128)
						{
							list.Add(new MMSEncryptedBlock(item));
						}
					}
				}
			}
		}
		return list;
	}
}
