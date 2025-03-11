// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.NotificationPayload
using System.Collections.Generic;


public class NotificationPayload : TLVObject
{
	public NotificationPayload(byte[] tag, byte[] value)
	{
		initializeNotificationPayload(tag, value);
	}

	public NotificationPayload(string tagHexString, string valueHexString)
	{
		initializeNotificationPayload(TLVParser.getByteArrayFromHexString(tagHexString), TLVParser.getByteArrayFromHexString(valueHexString));
	}

	public void initializeNotificationPayload(byte[] tag, byte[] value)
	{
		initialize(tag, value);
		if (value == null)
		{
			return;
		}
		List<TLVObject> list = TLVParser.parseTLVByteArray(value);
		if (list == null)
		{
			return;
		}
		foreach (TLVObject item in list)
		{
			addTLVObject(item);
		}
	}

	public byte[] getParamValue(string tagHexString)
	{
		byte[] result = null;
		TLVObject tLVObject = findByTagHexString(tagHexString);
		if (tLVObject != null)
		{
			result = tLVObject.getValueByteArray();
		}
		return result;
	}

	public TLVObject getPayload(string tag)
	{
		return findByTagHexString(tag);
	}

	public TLVObject getPayload()
	{
		return getPayload("84");
	}

	public byte[] getTagValue(string tag)
	{
		byte[] result = null;
		TLVObject payload = getPayload(tag);
		if (payload != null)
		{
			result = payload.getValueByteArray();
		}
		return result;
	}

	public byte[] getPayloadValue()
	{
		return getTagValue("84");
	}
}
