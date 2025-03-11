// MTServiceNET, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTSerialService
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;


public class MTSerialService : MTBaseService
{
	private bool mSLIP = true;

	private const byte SLIP_PACKET_START_END = 192;

	private const byte SLIP_ESCAPE = 219;

	private const byte SLIP_ESCAPE_C0 = 220;

	private const byte SLIP_ESCAPE_DB = 221;

	private const int MAX_PACKET_SIZE = 4096;

	private bool mPacketStarted;

	private bool mLastByteIsPacketStart;

	private bool mLastByteIsEscape;

	private int mPacketSize;

	private byte[] mPacket = new byte[4096];

	private MTSerialDevice mSerialDevice = new MTSerialDevice();

	private byte[] mDataReceived;

	private string mPortName { get; set; }

	private int mBaudRate { get; set; }

	private int mDataBits { get; set; }

	private Parity mParity { get; set; }

	private StopBits mStopBits { get; set; }

	private Handshake mHandshake { get; set; }

	public MTSerialService()
	{
		mBaudRate = 115200;
		mDataBits = 8;
		mParity = Parity.None;
		mStopBits = StopBits.One;
		mHandshake = Handshake.None;
		mSerialDevice.OnDataReceived += OnDataReceived;
		mSerialDevice.OnError += OnError;
	}

	~MTSerialService()
	{
	}

	public static void requestDeviceList(Action<MTConnectionType, List<MTDeviceInformation>> deviceListEventHandler)
	{
		List<string> portNames = MTSerialDevice.GetPortNames();
		List<MTDeviceInformation> list = new List<MTDeviceInformation>();
		if (portNames != null)
		{
			int num = 0;
			foreach (string item in portNames)
			{
				if (item != null)
				{
					MTDeviceInformation mTDeviceInformation = new MTDeviceInformation();
					if (mTDeviceInformation != null)
					{
						num++;
						mTDeviceInformation.Id = item;
						mTDeviceInformation.Name = item;
						mTDeviceInformation.Address = item;
						list.Add(mTDeviceInformation);
					}
				}
			}
		}
		deviceListEventHandler?.Invoke(MTConnectionType.Serial, list);
	}

	protected void setStateDisconnected()
	{
		setState(MTServiceState.Disconnected);
	}

	public override MTDeviceFeatures getDeviceFeatures()
	{
		return new MTDeviceFeatures
		{
			MSR = false,
			Contact = false,
			Contactless = true
		};
	}

	public static MTDeviceFeatures GetDeviceFeatures()
	{
		return new MTDeviceFeatures
		{
			MSR = false,
			Contact = false,
			Contactless = true
		};
	}

	protected void updateCommunicationParameters()
	{
		mPortName = "COM1";
		mBaudRate = 9600;
		mDataBits = 8;
		mParity = Parity.None;
		mStopBits = StopBits.One;
		string[] array = m_address.ToUpper().Replace(" ", "").Split(',');
		if (array == null)
		{
			return;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.StartsWith("PORT="))
			{
				mPortName = getParamStringValue(text);
			}
			else if (text.StartsWith("BAUDRATE="))
			{
				mBaudRate = getParamIntValue(text);
			}
			else if (text.StartsWith("PARITY="))
			{
				string paramStringValue = getParamStringValue(text);
				if (paramStringValue != null)
				{
					if (paramStringValue.CompareTo("N") == 0)
					{
						mParity = Parity.None;
					}
					else if (paramStringValue.CompareTo("E") == 0)
					{
						mParity = Parity.Even;
					}
					else if (paramStringValue.CompareTo("O") == 0)
					{
						mParity = Parity.Odd;
					}
					else if (paramStringValue.CompareTo("S") == 0)
					{
						mParity = Parity.Space;
					}
					else if (paramStringValue.CompareTo("M") == 0)
					{
						mParity = Parity.Mark;
					}
				}
			}
			else if (text.StartsWith("DATABITS="))
			{
				mDataBits = getParamIntValue(text);
			}
			else
			{
				if (!text.StartsWith("STOPBITS="))
				{
					continue;
				}
				string paramStringValue2 = getParamStringValue(text);
				if (paramStringValue2 != null)
				{
					if (paramStringValue2.CompareTo("1") == 0)
					{
						mStopBits = StopBits.One;
					}
					else if (paramStringValue2.CompareTo("1.5") == 0)
					{
						mStopBits = StopBits.OnePointFive;
					}
					else if (paramStringValue2.CompareTo("2") == 0)
					{
						mStopBits = StopBits.Two;
					}
				}
			}
		}
	}

	protected string getParamStringValue(string param)
	{
		string result = "";
		int num = param.IndexOf('=');
		if (num >= 0)
		{
			int num2 = num + 1;
			if (num2 < param.Length)
			{
				result = param.Substring(num2);
			}
		}
		return result;
	}

	protected int getParamIntValue(string param)
	{
		int result = 0;
		int num = param.IndexOf('=');
		if (num >= 0)
		{
			int num2 = num + 1;
			if (num2 < param.Length)
			{
				string text = param.Substring(num2);
				if (text != null)
				{
					try
					{
						result = int.Parse(text);
					}
					catch (Exception)
					{
					}
				}
			}
		}
		return result;
	}

	public override void connect()
	{
		m_state = MTServiceState.Disconnected;
		setState(MTServiceState.Connecting);
		if (mSLIP)
		{
			resetSLIPDecoder();
		}
		try
		{
			mDataReceived = null;
			updateCommunicationParameters();
			mSerialDevice.Open(mPortName, mBaudRate, mDataBits, mStopBits, mHandshake, mParity);
			setState(MTServiceState.Connected);
		}
		catch (Exception)
		{
			setState(MTServiceState.Disconnected);
		}
	}

	public override void disconnect()
	{
		setState(MTServiceState.Disconnecting);
		try
		{
			mSerialDevice.Close();
			setState(MTServiceState.Disconnected);
		}
		catch (Exception)
		{
		}
	}

	public override bool sendData(byte[] data)
	{
		bool result = false;
		if (data != null && data.Length != 0)
		{
			int num = data.Length;
			byte[] array = new byte[num + 3];
			array[0] = 5;
			array[1] = (byte)((num >> 8) & 0xFF);
			array[2] = (byte)(num & 0xFF);
			Array.Copy(data, 0, array, 3, num);
			byte[] array2 = null;
			array2 = ((!mSLIP) ? buildASCIIData(array) : buildSLIPData(array));
			if (array2 != null)
			{
				result = mSerialDevice.Send(array2);
			}
		}
		return result;
	}

	private void OnError(object sender, EventArgs e)
	{
		setStateDisconnected();
	}

	private void OnDataReceived(object sender, byte[] data)
	{
		if (mSLIP)
		{
			processSLIPData(data);
		}
		else
		{
			processASCIIData(data);
		}
	}

	private void resetSLIPDecoder()
	{
		mPacketStarted = false;
		mLastByteIsPacketStart = false;
		mLastByteIsEscape = false;
		mPacketSize = 0;
		mPacket = new byte[4096];
	}

	private void processSLIPPacket(byte[] data)
	{
		if (data == null)
		{
			return;
		}
		int num = data.Length - 3;
		if (num <= 0)
		{
			return;
		}
		byte b = data[0];
		byte[] array = new byte[num];
		Array.Copy(data, 3, array, 0, num);
		switch (b)
		{
		case 2:
			if (m_deviceDataHandler != null)
			{
				m_deviceDataHandler(array);
			}
			break;
		case 3:
		{
			byte[] obj = MTRLEData.decodeRLEData(array);
			if (m_deviceDataHandler != null)
			{
				m_deviceDataHandler(obj);
			}
			break;
		}
		case 4:
			if (m_commandDataHandler != null)
			{
				m_commandDataHandler(array);
			}
			break;
		}
	}

	private void decodeSLIPByte(byte slipByte)
	{
		if (slipByte == 192)
		{
			if (mPacketStarted)
			{
				if (mLastByteIsPacketStart)
				{
					mPacketStarted = true;
				}
				else
				{
					mPacketStarted = false;
					if (mPacketSize > 0)
					{
						byte[] array = new byte[mPacketSize];
						Array.Copy(mPacket, 0, array, 0, mPacketSize);
						processSLIPPacket(array);
					}
				}
			}
			else
			{
				mPacketStarted = true;
			}
			mPacketSize = 0;
			mLastByteIsPacketStart = true;
			return;
		}
		if (mPacketStarted)
		{
			if (mLastByteIsEscape)
			{
				switch (slipByte)
				{
				case 220:
					mPacket[mPacketSize] = 192;
					mPacketSize++;
					break;
				case 221:
					mPacket[mPacketSize] = 219;
					mPacketSize++;
					break;
				}
				mLastByteIsEscape = false;
			}
			else if (slipByte == 219)
			{
				mLastByteIsEscape = true;
			}
			else
			{
				mPacket[mPacketSize] = slipByte;
				mPacketSize++;
				mLastByteIsEscape = false;
			}
		}
		mLastByteIsPacketStart = false;
	}

	private void processSLIPData(byte[] data)
	{
		if (data != null)
		{
			int num = data.Length;
			for (int i = 0; i < num; i++)
			{
				decodeSLIPByte(data[i]);
			}
		}
	}

	private byte[] buildSLIPData(byte[] data)
	{
		byte[] array = new byte[2048];
		int num = 0;
		if (data != null)
		{
			int num2 = data.Length;
			int i = 0;
			int num3 = 0;
			array[num3++] = 192;
			for (; i < num2; i++)
			{
				if (data[i] == 192)
				{
					array[num3++] = 219;
					array[num3++] = 220;
				}
				else if (data[i] == 219)
				{
					array[num3++] = 219;
					array[num3++] = 221;
				}
				else
				{
					array[num3++] = data[i];
				}
			}
			array[num3++] = 192;
			num = num3;
		}
		byte[] array2 = null;
		if (num > 0)
		{
			array2 = new byte[num];
			Array.Copy(array, 0, array2, 0, num);
		}
		return array2;
	}

	private byte[] buildASCIIData(byte[] data)
	{
		byte[] array = null;
		if (data != null && data.Length != 0)
		{
			string hexString = MTParser.getHexString(data);
			byte[] bytes = Encoding.UTF8.GetBytes(hexString);
			if (bytes != null)
			{
				int num = bytes.Length + 1;
				array = new byte[num];
				Array.Copy(bytes, 0, array, 0, bytes.Length);
				array[num - 1] = 13;
			}
		}
		return array;
	}

	protected void processASCIIData(byte[] dataBytes)
	{
		if (dataBytes == null)
		{
			return;
		}
		int num = dataBytes.Length;
		if (num <= 0)
		{
			return;
		}
		byte[] array = null;
		if (mDataReceived != null)
		{
			int num2 = mDataReceived.Length;
			array = new byte[num2 + num];
			Array.Copy(mDataReceived, 0, array, 0, num2);
			Array.Copy(dataBytes, 0, array, num2, num);
		}
		else
		{
			array = new byte[num];
			Array.Copy(dataBytes, 0, array, 0, num);
		}
		int num3 = array.Length;
		if (num3 <= 0)
		{
			return;
		}
		int num4 = 0;
		for (int i = 0; i < num3; i++)
		{
			if (array[i] != 13)
			{
				continue;
			}
			int num5 = i - num4;
			if (num5 > 0)
			{
				byte[] array2 = new byte[num5];
				Array.Copy(array, num4, array2, 0, num5);
				MTParser.getHexString(array2);
				byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(Encoding.UTF8.GetString(array2));
				if (byteArrayFromHexString != null)
				{
					int num6 = byteArrayFromHexString.Length;
					if (num6 > 0)
					{
						switch (byteArrayFromHexString[0])
						{
						case 2:
							if (num6 >= 9)
							{
								byte[] array4 = new byte[num6 - 1];
								Array.Copy(byteArrayFromHexString, 1, array4, 0, num6 - 1);
								if (m_deviceDataHandler != null)
								{
									m_deviceDataHandler(array4);
								}
							}
							break;
						case 4:
							if (num6 > 1)
							{
								byte[] array3 = new byte[num6 - 1];
								Array.Copy(byteArrayFromHexString, 1, array3, 0, num6 - 1);
								if (m_commandDataHandler != null)
								{
									m_commandDataHandler(array3);
								}
							}
							break;
						}
					}
				}
			}
			num4 = i + 1;
		}
		int num7 = num3 - num4;
		if (num7 > 0)
		{
			mDataReceived = new byte[num7];
			Array.Copy(array, num4, mDataReceived, 0, num7);
		}
		else
		{
			mDataReceived = null;
		}
	}
}
