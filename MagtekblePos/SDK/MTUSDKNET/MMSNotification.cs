// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSNotification


public class MMSNotification
{
	protected Message mMessage;

	public MMSNotification()
	{
	}

	public MMSNotification(Message message)
	{
		mMessage = message;
	}

	public byte[] getNotificationID()
	{
		byte[] result = null;
		if (mMessage != null)
		{
			result = mMessage.getRequestID();
		}
		return result;
	}

	public byte[] getNotificationCode()
	{
		byte[] result = null;
		TLVObject tLVObject = null;
		if (mMessage != null)
		{
			tLVObject = mMessage.findByTagHexString("82");
		}
		if (tLVObject != null)
		{
			result = tLVObject.getValueByteArray();
		}
		return result;
	}

	public byte[] getAdditionalDetails()
	{
		byte[] result = null;
		TLVObject tLVObject = null;
		if (mMessage != null)
		{
			tLVObject = mMessage.findByTagHexString("83");
		}
		if (tLVObject != null)
		{
			result = tLVObject.getValueByteArray();
		}
		return result;
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

	public byte[] getCRC()
	{
		byte[] result = null;
		TLVObject tLVObject = null;
		if (mMessage != null)
		{
			tLVObject = mMessage.getCRC();
		}
		if (tLVObject != null)
		{
			result = tLVObject.getValueByteArray();
		}
		return result;
	}
}
