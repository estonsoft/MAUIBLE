// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTCMSMessage
using System;
using System.Collections.Generic;


public class MTCMSMessage
{
	protected int mMessageType;

	protected int mApplicationID;

	protected int mCommandID;

	protected int mResultCode;

	protected int mDataTag;

	protected byte[] mData;

	protected byte[] mMessageBytes;

	protected static byte[] TAG_C0 = new byte[1] { 192 };

	protected static byte[] TAG_C1 = new byte[1] { 193 };

	protected static byte[] TAG_C2 = new byte[1] { 194 };

	protected static byte[] TAG_C3 = new byte[1] { 195 };

	public MTCMSMessage(int messageType, int applicationID, int commandID, int resultCode, int dataTag, byte[] data)
	{
		mMessageType = messageType;
		mApplicationID = applicationID;
		mCommandID = commandID;
		mResultCode = resultCode;
		mDataTag = dataTag;
		mData = data;
		buildMessageBytes();
	}

	public MTCMSMessage(byte[] messageBytes)
	{
		if (messageBytes != null)
		{
			int num = messageBytes.Length;
			if (num > 0)
			{
				mMessageBytes = new byte[num];
				Array.Copy(messageBytes, mMessageBytes, num);
				parseMessageBytes();
			}
		}
	}

	~MTCMSMessage()
	{
	}

	public void setMessageType(int messageType)
	{
		mMessageType = messageType;
		buildMessageBytes();
	}

	public void setApplicationID(int applicationID)
	{
		mApplicationID = applicationID;
		buildMessageBytes();
	}

	public void setCommandID(int commandID)
	{
		mCommandID = commandID;
		buildMessageBytes();
	}

	public void setResultCode(int resultCode)
	{
		mResultCode = resultCode;
		buildMessageBytes();
	}

	public void setData(int dataTag, byte[] data)
	{
		mDataTag = dataTag;
		mData = data;
		buildMessageBytes();
	}

	public int getMessageType()
	{
		return mMessageType;
	}

	public int getApplicationID()
	{
		return mApplicationID;
	}

	public int getCommandID()
	{
		return mCommandID;
	}

	public int getResultCode()
	{
		return mResultCode;
	}

	public int getDataTag()
	{
		return mDataTag;
	}

	public byte[] getData()
	{
		return mData;
	}

	public byte[] getMessageBytes()
	{
		return mMessageBytes;
	}

	protected void buildMessageBytes()
	{
		MTTLVObject mTTLVObject = new MTTLVObject("FA");
		MTTLVObject tlvObject = new MTTLVObject(TAG_C0, new byte[1] { (byte)(mMessageType & 0xFF) });
		mTTLVObject.addTLVObject(tlvObject);
		MTTLVObject tlvObject2 = new MTTLVObject(TAG_C1, new byte[1] { (byte)(mApplicationID & 0xFF) });
		mTTLVObject.addTLVObject(tlvObject2);
		MTTLVObject tlvObject3 = new MTTLVObject(TAG_C2, new byte[1] { (byte)(mCommandID & 0xFF) });
		mTTLVObject.addTLVObject(tlvObject3);
		if (mMessageType == 2 || mMessageType == 3)
		{
			MTTLVObject tlvObject4 = new MTTLVObject(TAG_C3, new byte[1] { (byte)(mResultCode & 0xFF) });
			mTTLVObject.addTLVObject(tlvObject4);
		}
		if (mData != null && mData.Length != 0)
		{
			MTTLVObject tlvObject5 = new MTTLVObject(new byte[1] { (byte)(mDataTag & 0xFF) }, mData);
			mTTLVObject.addTLVObject(tlvObject5);
		}
		mMessageBytes = mTTLVObject.getValueByteArray();
	}

	protected int getFirstByteValue(byte[] byteArray)
	{
		int result = 0;
		if (byteArray != null && byteArray.Length != 0)
		{
			result = byteArray[0] & 0xFF;
		}
		return result;
	}

	protected void parseMessageBytes()
	{
		if (mMessageBytes == null)
		{
			return;
		}
		List<MTTLVObject> tlvObjectList = MTTLVParser.parseTLVByteArray(mMessageBytes);
		MTTLVObject mTTLVObject = MTTLVParser.findFromListByTagHexString(tlvObjectList, "C0");
		if (mTTLVObject != null)
		{
			mMessageType = getFirstByteValue(mTTLVObject.getValueByteArray());
		}
		MTTLVObject mTTLVObject2 = MTTLVParser.findFromListByTagHexString(tlvObjectList, "C1");
		if (mTTLVObject2 != null)
		{
			mApplicationID = getFirstByteValue(mTTLVObject2.getValueByteArray());
		}
		MTTLVObject mTTLVObject3 = MTTLVParser.findFromListByTagHexString(tlvObjectList, "C2");
		if (mTTLVObject3 != null)
		{
			mCommandID = getFirstByteValue(mTTLVObject3.getValueByteArray());
		}
		MTTLVObject mTTLVObject4 = MTTLVParser.findFromListByTagHexString(tlvObjectList, "C3");
		if (mTTLVObject4 != null)
		{
			mResultCode = getFirstByteValue(mTTLVObject4.getValueByteArray());
		}
		MTTLVObject mTTLVObject5 = MTTLVParser.findFromListByTagHexString(tlvObjectList, "C4");
		if (mTTLVObject5 != null)
		{
			mDataTag = 196;
			mData = mTTLVObject5.getValueByteArray();
			return;
		}
		MTTLVObject mTTLVObject6 = MTTLVParser.findFromListByTagHexString(tlvObjectList, "E0");
		if (mTTLVObject6 != null)
		{
			mDataTag = 224;
			mData = mTTLVObject6.getValueByteArray();
		}
	}
}
