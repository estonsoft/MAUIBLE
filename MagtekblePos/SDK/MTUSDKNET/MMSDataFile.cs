// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSDataFile


public class MMSDataFile
{
	protected Message mMessage;

	public MMSDataFile()
	{
	}

	public MMSDataFile(Message message)
	{
		mMessage = message;
	}

	public byte[] getCommandID()
	{
		return mMessage.getRequestID();
	}

	public byte[] getFileType()
	{
		byte[] array = new byte[4] { 0, 0, 0, 0 };
		TLVObject msgInfo = mMessage.getMsgInfo();
		if (msgInfo != null)
		{
			byte[] valueByteArray = msgInfo.getValueByteArray();
			if (valueByteArray != null && valueByteArray.Length >= 8)
			{
				array[0] = valueByteArray[4];
				array[1] = valueByteArray[5];
				array[2] = valueByteArray[6];
				array[3] = valueByteArray[7];
			}
		}
		return array;
	}

	public byte[] getPayload()
	{
		byte[] result = null;
		TLVObject tLVObject = null;
		if (mMessage != null)
		{
			tLVObject = mMessage.getPayload();
		}
		if (tLVObject != null)
		{
			result = tLVObject.getValueByteArray();
		}
		return result;
	}
}
