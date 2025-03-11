// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.Message
using System.Collections.Generic;
using System.IO;


public class Message : TLVObject
{
	private static byte[] MESSAGE_TAG = new byte[2] { 170, 255 };

	public Message()
	{
		initialize(MESSAGE_TAG, null);
	}

	public Message(byte[] tag)
	{
		initialize(tag, null);
	}

	public Message(byte[] tag, byte[] value)
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

	public void addParam(byte[] tagBytes, byte[] valueBytes)
	{
		addTLVObject(new TLVObject(tagBytes, valueBytes));
	}

	public void addParam(string tagHexString, string valueHexString)
	{
		addTLVObject(new TLVObject(tagHexString, valueHexString));
	}

	public void addMessageInfoForCommand(byte[] commandBytes)
	{
		TLVObject tlvObject = MessageBuilder.BuildMessageInfoForCommand(commandBytes);
		addTLVObject(tlvObject);
	}

	public void addMessageInfoForCommand(string commandHexString)
	{
		addMessageInfoForCommand(TLVParser.getByteArrayFromHexString(commandHexString));
	}

	public void addMessageInfoForDataFile(byte[] commandIDBytes, byte[] fileIDBytes)
	{
		TLVObject tlvObject = MessageBuilder.BuildMessageInfoForDataFile(commandIDBytes, fileIDBytes);
		addTLVObject(tlvObject);
	}

	public void addMessageInfoForDataFile(string commandIDHexString, string fileIDHexString)
	{
		addMessageInfoForDataFile(TLVParser.getByteArrayFromHexString(commandIDHexString), TLVParser.getByteArrayFromHexString(fileIDHexString));
	}

	public void addPayload(byte[] valueBytes)
	{
		addParam(TLVParser.getByteArrayFromHexString("84"), valueBytes);
	}

	public void addPayload(string valueHexString)
	{
		addParam("84", valueHexString);
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

	public TLVObject getMsgInfo()
	{
		return findByTagHexString("81");
	}

	public byte getMsgInfoValue(int index)
	{
		byte result = 0;
		TLVObject msgInfo = getMsgInfo();
		if (msgInfo != null)
		{
			byte[] valueByteArray = msgInfo.getValueByteArray();
			if (valueByteArray != null && valueByteArray.Length > index)
			{
				result = valueByteArray[index];
			}
		}
		return result;
	}

	public byte getDirectionAndType()
	{
		return getMsgInfoValue(0);
	}

	public byte getMessageID()
	{
		return getMsgInfoValue(1);
	}

	public byte[] getRequestID()
	{
		byte[] array = new byte[2] { 0, 0 };
		TLVObject msgInfo = getMsgInfo();
		if (msgInfo != null)
		{
			byte[] valueByteArray = msgInfo.getValueByteArray();
			if (valueByteArray != null && valueByteArray.Length >= 4)
			{
				array[0] = valueByteArray[2];
				array[1] = valueByteArray[3];
			}
		}
		return array;
	}

	public byte getCRCType()
	{
		return getMsgInfoValue(4);
	}

	public bool isRequest()
	{
		return getDirectionAndType() == 1;
	}

	public bool isResponse()
	{
		return getDirectionAndType() == 130;
	}

	public bool isNotification()
	{
		return getDirectionAndType() == 131;
	}

	public bool isDataFile()
	{
		return getDirectionAndType() == 132;
	}

	public bool isFile()
	{
		return getDirectionAndType() == 132;
	}

	public TLVObject getPayload()
	{
		return findByTagHexString("84");
	}

	public TLVObject getCRC()
	{
		return findByTagHexString("9E");
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
