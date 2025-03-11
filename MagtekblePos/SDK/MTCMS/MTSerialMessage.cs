// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTSerialMessage
using System;
using System.Text;


internal class MTSerialMessage
{
	private const byte SPECIAL_CHAR_NULL = 0;

	private const byte SPECIAL_CHAR_STX = 2;

	private const byte SPECIAL_CHAR_ETX = 3;

	private const byte SPECIAL_CHAR_LF = 10;

	private const byte SPECIAL_CHAR_CR = 13;

	private static MTCRC16 CRC16 = new MTCRC16();

	protected bool mHasStartingByte;

	protected bool mHasEndingByte;

	protected bool mCRCMode;

	protected byte mStartingByte;

	protected byte mEndingByte;

	protected byte[] mInputData;

	public event EventHandler<byte[]> OnInputMessage;

	public MTSerialMessage()
	{
		mHasStartingByte = false;
		mHasEndingByte = true;
		mCRCMode = false;
		mEndingByte = 10;
		reset();
	}

	~MTSerialMessage()
	{
	}

	public void setStartingByte(bool enabled, byte value)
	{
		mHasStartingByte = enabled;
		mStartingByte = value;
	}

	public void setEndingByte(bool enabled, byte value)
	{
		mHasEndingByte = enabled;
		mEndingByte = value;
	}

	public void setCRCMode(bool enabled)
	{
		mCRCMode = enabled;
	}

	public bool hasStartingByte()
	{
		return mHasStartingByte;
	}

	public bool hasEndingByte()
	{
		return mHasEndingByte;
	}

	public byte getStartingByte()
	{
		return mStartingByte;
	}

	public byte getEndingByte()
	{
		return mEndingByte;
	}

	public bool getCRCMode()
	{
		return mCRCMode;
	}

	public void reset()
	{
		mInputData = null;
	}

	public byte[] getCRC(byte[] data)
	{
		byte[] result = new byte[2];
		if (data != null)
		{
			result = CRC16.GetCRCBytes(data);
		}
		return result;
	}

	public byte[] getOutputData(byte[] data)
	{
		if (data != null)
		{
			_ = data.Length;
			if (data.Length != 0)
			{
				string hexString = MTParser.getHexString(data);
				if (hexString != null)
				{
					byte[] bytes = Encoding.UTF8.GetBytes(hexString);
					if (bytes != null)
					{
						int num = bytes.Length;
						if (num > 0)
						{
							int num2 = num;
							if (mHasStartingByte)
							{
								num2++;
							}
							if (mHasEndingByte)
							{
								num2++;
							}
							if (mCRCMode)
							{
								num2 += 4;
							}
							byte[] array = new byte[num2];
							if (array != null)
							{
								int num3 = 0;
								if (mHasStartingByte)
								{
									array[num3++] = mStartingByte;
								}
								Array.Copy(bytes, 0, array, num3, num);
								num3 += num;
								if (mCRCMode)
								{
									byte[] cRC = getCRC(data);
									if (cRC != null && cRC.Length == 2)
									{
										string hexString2 = MTParser.getHexString(cRC);
										if (hexString2 != null)
										{
											byte[] bytes2 = Encoding.UTF8.GetBytes(hexString2);
											if (bytes2 != null && bytes2.Length == 4)
											{
												Array.Copy(bytes2, 0, array, num3, 4);
											}
										}
									}
									num3 += 4;
								}
								if (mHasEndingByte)
								{
									array[num3++] = mEndingByte;
								}
								return array;
							}
						}
					}
				}
			}
		}
		return null;
	}

	public void setInputData(byte[] inputData)
	{
		if (inputData == null)
		{
			return;
		}
		int num = inputData.Length;
		if (num > 0)
		{
			MTParser.getHexString(inputData);
			if (mInputData != null)
			{
				int num2 = mInputData.Length;
				byte[] array = new byte[num2 + num];
				Array.Copy(mInputData, 0, array, 0, num2);
				Array.Copy(inputData, 0, array, num2, num);
				mInputData = array;
				MTParser.getHexString(array);
			}
			else
			{
				mInputData = new byte[num];
				Array.Copy(inputData, mInputData, num);
			}
			processInputData();
		}
	}

	protected void processInputData()
	{
		if (mInputData == null)
		{
			return;
		}
		bool flag = true;
		int num = -1;
		if (mHasStartingByte)
		{
			num = Array.IndexOf(mInputData, mStartingByte);
			if (num < 0)
			{
				flag = false;
			}
		}
		if (!flag)
		{
			return;
		}
		bool flag2 = true;
		int num2 = mInputData.Length;
		if (mHasEndingByte)
		{
			num2 = Array.IndexOf(mInputData, mEndingByte);
			if (num2 < 0)
			{
				flag2 = false;
			}
		}
		if (!flag2)
		{
			return;
		}
		int num3 = num2 - num - 1;
		int num4 = num3;
		if (num4 > 0)
		{
			byte[] array = new byte[num4];
			Array.Copy(mInputData, num + 1, array, 0, num4);
			string @string = Encoding.UTF8.GetString(array);
			if (@string != null)
			{
				byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(@string);
				if (byteArrayFromHexString != null && byteArrayFromHexString.Length != 0)
				{
					if (mCRCMode)
					{
						int num5 = byteArrayFromHexString.Length - 2;
						byte[] array2 = new byte[num5];
						Array.Copy(byteArrayFromHexString, 0, array2, 0, num5);
						byte[] array3 = new byte[2];
						Array.Copy(byteArrayFromHexString, byteArrayFromHexString.Length - 2, array3, 0, 2);
						byte[] cRC = getCRC(array2);
						if (cRC != null && cRC.Length == 2 && cRC[0] == array3[0] && cRC[1] == array3[1] && this.OnInputMessage != null)
						{
							this.OnInputMessage(this, array2);
						}
					}
					else if (this.OnInputMessage != null)
					{
						this.OnInputMessage(this, byteArrayFromHexString);
					}
				}
			}
		}
		int num6 = mInputData.Length - num3 - 1;
		if (num > 0)
		{
			num6 = mInputData.Length - num2 - 1;
		}
		if (num6 > 0)
		{
			byte[] destinationArray = new byte[num6];
			Array.Copy(mInputData, num2 + 1, destinationArray, 0, num6);
			mInputData = destinationArray;
		}
		else
		{
			mInputData = null;
		}
		processInputData();
	}
}
