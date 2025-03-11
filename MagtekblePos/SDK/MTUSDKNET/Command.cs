// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.Command
using System.Collections.Generic;
using System.IO;


public class Command : TLVObject
{
	public Command(byte[] tag, byte[] value)
	{
		initializeCommand(tag, value);
	}

	public Command(string tagHexString, string valueHexString)
	{
		initializeCommand(TLVParser.getByteArrayFromHexString(tagHexString), TLVParser.getByteArrayFromHexString(valueHexString));
	}

	public void initializeCommand(byte[] tag, byte[] value)
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

	public void addParam(byte tagByte, byte valueByte)
	{
		addParam(new byte[1] { tagByte }, new byte[1] { valueByte });
	}

	public void addParam(byte tagByte, byte[] valueBytes)
	{
		addParam(new byte[1] { tagByte }, valueBytes);
	}

	public void addParam(byte[] tagBytes, byte[] valueBytes)
	{
		addTLVObject(new TLVObject(tagBytes, valueBytes));
	}

	public void addParam(string tagHexString, string valueHexString)
	{
		addTLVObject(new TLVObject(tagHexString, valueHexString));
	}

	public void addParam(TLVObject tlvObject)
	{
		addTLVObject(tlvObject);
	}

	public byte[] getByteArray()
	{
		byte[] valueByteArray = getValueByteArray();
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(mTag);
		if (valueByteArray != null)
		{
			binaryWriter.Write(valueByteArray);
		}
		return memoryStream.ToArray();
	}

	public TLVObject getPayload()
	{
		return findByTopLevelTagHexString("84");
	}

	public Command getCommand()
	{
		Command result = null;
		TLVObject payload = getPayload();
		if (payload != null)
		{
			result = MessageParser.GetCommand(payload.getValueByteArray());
		}
		return result;
	}
}
