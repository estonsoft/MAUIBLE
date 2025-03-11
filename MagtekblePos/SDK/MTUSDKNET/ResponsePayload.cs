// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.ResponsePayload
using System.Collections.Generic;


public class ResponsePayload : TLVObject
{
	public ResponsePayload(byte[] tag, byte[] value)
	{
		initializeResponsePayload(tag, value);
	}

	public ResponsePayload(string tagHexString, string valueHexString)
	{
		initializeResponsePayload(TLVParser.getByteArrayFromHexString(tagHexString), TLVParser.getByteArrayFromHexString(valueHexString));
	}

	public void initializeResponsePayload(byte[] tag, byte[] value)
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

	public TLVObject getParamObject(string tagHexString)
	{
		return findByTagHexString(tagHexString);
	}

	public TLVObject getPayload()
	{
		return findByTagHexString("82");
	}

	public byte[] getPayloadValue()
	{
		byte[] result = null;
		TLVObject payload = getPayload();
		if (payload != null)
		{
			result = payload.getValueByteArray();
		}
		return result;
	}
}
