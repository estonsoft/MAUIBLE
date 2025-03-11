// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSResponse


public class MMSResponse
{
	private Message mMessage;

	public MMSResponse()
	{
	}

	public MMSResponse(Message message)
	{
		mMessage = message;
	}

	public byte[] getRequestID()
	{
		byte[] result = null;
		if (mMessage != null)
		{
			result = mMessage.getRequestID();
		}
		return result;
	}

	public TLVObject getStatusCode()
	{
		TLVObject result = null;
		if (mMessage != null)
		{
			result = mMessage.findByTagHexString("82");
		}
		return result;
	}

	public byte[] getStatusCodeValue()
	{
		byte[] result = null;
		if (mMessage != null)
		{
			TLVObject tLVObject = mMessage.findByTagHexString("82");
			if (tLVObject != null)
			{
				result = tLVObject.getValueByteArray();
			}
		}
		return result;
	}

	public byte getOperationStatus()
	{
		byte result = 0;
		TLVObject statusCode = getStatusCode();
		if (statusCode != null)
		{
			byte[] valueByteArray = statusCode.getValueByteArray();
			if (valueByteArray != null && valueByteArray.Length != 0)
			{
				result = valueByteArray[0];
			}
		}
		return result;
	}

	public byte[] getResultCode()
	{
		byte[] array = new byte[2] { 0, 0 };
		TLVObject statusCode = getStatusCode();
		if (statusCode != null)
		{
			byte[] valueByteArray = statusCode.getValueByteArray();
			if (valueByteArray != null && valueByteArray.Length >= 3)
			{
				array[0] = valueByteArray[1];
				array[1] = valueByteArray[2];
			}
		}
		return array;
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
