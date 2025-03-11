// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSEncryptedBlock
using System.Collections.Generic;


public class MMSEncryptedBlock
{
	protected TLVObject mTLVObject;

	public MMSEncryptedBlock()
	{
	}

	public MMSEncryptedBlock(TLVObject tLVObject)
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

	public TLVObject getEncryptInfoObject()
	{
		return getObjectByTag("81");
	}

	public byte[] getEncryptInfoValue(string tag)
	{
		byte[] result = null;
		TLVObject encryptInfoObject = getEncryptInfoObject();
		if (encryptInfoObject != null)
		{
			TLVObject tLVObject = encryptInfoObject.findChildByTagHexString(tag);
			if (tLVObject != null)
			{
				result = tLVObject.getValueByteArray();
			}
		}
		return result;
	}

	public string getKSNString()
	{
		string result = "";
		byte[] encryptInfoValue = getEncryptInfoValue("83");
		if (encryptInfoValue != null)
		{
			result = TLVParser.getHexString(encryptInfoValue);
		}
		return result;
	}

	public List<MMSEncryptedElement> getEncryptedElements()
	{
		List<MMSEncryptedElement> list = new List<MMSEncryptedElement>();
		TLVObject objectByTag = getObjectByTag("84");
		if (objectByTag != null)
		{
			List<TLVObject> valueTLVObjectList = objectByTag.getValueTLVObjectList();
			if (valueTLVObjectList != null)
			{
				foreach (TLVObject item in valueTLVObjectList)
				{
					if (item != null)
					{
						list.Add(new MMSEncryptedElement(item));
					}
				}
			}
		}
		return list;
	}
}
