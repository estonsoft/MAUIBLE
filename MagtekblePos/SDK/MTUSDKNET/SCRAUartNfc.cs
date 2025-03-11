// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.SCRAUartNfc
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;




internal class SCRAUartNfc
{
	public delegate void DebugInfoHandler(object sender, string data);

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

	private MTSCRA m_SCRA;

	private byte[] m_uartDataReceived;

	private int m_emvDataLen;

	private byte[] m_emvData;

	[CompilerGenerated]
	private DeviceResponseHandler m_OnDeviceResponse;

	[CompilerGenerated]
	private TransactionStatusHandler m_OnTransactionStatus;

	[CompilerGenerated]
	private DisplayMessageRequestHandler m_OnDisplayMessageRequest;

	[CompilerGenerated]
	private UserSelectionRequestHandler m_OnUserSelectionRequest;

	[CompilerGenerated]
	private ARQCReceivedHandler m_OnARQCReceived;

	[CompilerGenerated]
	private TransactionResultHandler m_OnTransactionResult;

	public event DeviceResponseHandler OnDeviceResponse
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			DeviceResponseHandler val = this.m_OnDeviceResponse;
			DeviceResponseHandler val2;
			do
			{
				val2 = val;
				DeviceResponseHandler value2 = (DeviceResponseHandler)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnDeviceResponse, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			DeviceResponseHandler val = this.m_OnDeviceResponse;
			DeviceResponseHandler val2;
			do
			{
				val2 = val;
				DeviceResponseHandler value2 = (DeviceResponseHandler)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnDeviceResponse, value2, val2);
			}
			while (val != val2);
		}
	}

	public event TransactionStatusHandler OnTransactionStatus
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			TransactionStatusHandler val = this.m_OnTransactionStatus;
			TransactionStatusHandler val2;
			do
			{
				val2 = val;
				TransactionStatusHandler value2 = (TransactionStatusHandler)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnTransactionStatus, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			TransactionStatusHandler val = this.m_OnTransactionStatus;
			TransactionStatusHandler val2;
			do
			{
				val2 = val;
				TransactionStatusHandler value2 = (TransactionStatusHandler)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnTransactionStatus, value2, val2);
			}
			while (val != val2);
		}
	}

	public event DisplayMessageRequestHandler OnDisplayMessageRequest
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			DisplayMessageRequestHandler val = this.m_OnDisplayMessageRequest;
			DisplayMessageRequestHandler val2;
			do
			{
				val2 = val;
				DisplayMessageRequestHandler value2 = (DisplayMessageRequestHandler)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnDisplayMessageRequest, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			DisplayMessageRequestHandler val = this.m_OnDisplayMessageRequest;
			DisplayMessageRequestHandler val2;
			do
			{
				val2 = val;
				DisplayMessageRequestHandler value2 = (DisplayMessageRequestHandler)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnDisplayMessageRequest, value2, val2);
			}
			while (val != val2);
		}
	}

	public event UserSelectionRequestHandler OnUserSelectionRequest
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			UserSelectionRequestHandler val = this.m_OnUserSelectionRequest;
			UserSelectionRequestHandler val2;
			do
			{
				val2 = val;
				UserSelectionRequestHandler value2 = (UserSelectionRequestHandler)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnUserSelectionRequest, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			UserSelectionRequestHandler val = this.m_OnUserSelectionRequest;
			UserSelectionRequestHandler val2;
			do
			{
				val2 = val;
				UserSelectionRequestHandler value2 = (UserSelectionRequestHandler)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnUserSelectionRequest, value2, val2);
			}
			while (val != val2);
		}
	}

	public event ARQCReceivedHandler OnARQCReceived
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			ARQCReceivedHandler val = this.m_OnARQCReceived;
			ARQCReceivedHandler val2;
			do
			{
				val2 = val;
				ARQCReceivedHandler value2 = (ARQCReceivedHandler)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnARQCReceived, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			ARQCReceivedHandler val = this.m_OnARQCReceived;
			ARQCReceivedHandler val2;
			do
			{
				val2 = val;
				ARQCReceivedHandler value2 = (ARQCReceivedHandler)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnARQCReceived, value2, val2);
			}
			while (val != val2);
		}
	}

	public event TransactionResultHandler OnTransactionResult
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			TransactionResultHandler val = this.m_OnTransactionResult;
			TransactionResultHandler val2;
			do
			{
				val2 = val;
				TransactionResultHandler value2 = (TransactionResultHandler)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnTransactionResult, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			TransactionResultHandler val = this.m_OnTransactionResult;
			TransactionResultHandler val2;
			do
			{
				val2 = val;
				TransactionResultHandler value2 = (TransactionResultHandler)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_OnTransactionResult, value2, val2);
			}
			while (val != val2);
		}
	}

	public event DebugInfoHandler OnDebugInfo;

	public SCRAUartNfc(MTSCRA scra)
	{
		m_SCRA = scra;
		m_uartDataReceived = null;
		m_emvDataLen = 0;
		m_emvData = null;
		if (mSLIP)
		{
			resetSLIPDecoder();
		}
	}

	private byte[] buildExtendedCommand(byte[] command, byte[] data)
	{
		byte[] array = null;
		if (command != null && command.Length >= 2 && data != null)
		{
			int num = data.Length;
			array = new byte[num + 4];
			array[0] = command[0];
			array[1] = command[1];
			array[2] = (byte)((num >> 8) & 0xFF);
			array[3] = (byte)(num & 0xFF);
			Array.Copy(data, 0, array, 4, data.Length);
		}
		return array;
	}

	protected void sendDebugInfo(string data)
	{
		if (this.OnDebugInfo != null)
		{
			this.OnDebugInfo(this, data);
		}
	}

	public int sendExtendedCommandBytes(byte[] command, byte[] data)
	{
		int num = 9;
		int num2 = 0;
		int num3 = 0;
		if (command != null)
		{
			num2 = command.Length;
		}
		if (num2 != 2)
		{
			return num;
		}
		if (data != null)
		{
			num3 = data.Length;
		}
		int num4 = 0;
		while (num4 < num3 || num3 == 0)
		{
			int num5 = num3 - num4;
			if (num5 >= 52)
			{
				num5 = 51;
			}
			byte[] array = new byte[8 + num5];
			array[0] = MTEMVDeviceConstants.PROTOCOL_EXTENDER_REQUEST;
			array[1] = (byte)(6 + num5);
			array[2] = (byte)((num4 >> 8) & 0xFF);
			array[3] = (byte)(num4 & 0xFF);
			array[4] = command[0];
			array[5] = command[1];
			array[6] = (byte)((num3 >> 8) & 0xFF);
			array[7] = (byte)(num3 & 0xFF);
			for (int i = 0; i < num5; i++)
			{
				array[8 + i] = data[num4 + i];
			}
			num4 += num5;
			num = sendData(MTParser.getHexString(array));
			if (num == 9)
			{
				return num;
			}
			if (num3 == 0)
			{
				break;
			}
		}
		return num;
	}

	public int sendData(string data)
	{
		int num = 9;
		if (m_SCRA != null)
		{
			byte[] array = null;
			byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(data);
			if (byteArrayFromHexString != null && byteArrayFromHexString.Length != 0)
			{
				int num2 = byteArrayFromHexString.Length;
				byte[] array2 = new byte[num2 + 3];
				array2[0] = 5;
				array2[1] = (byte)((num2 >> 8) & 0xFF);
				array2[2] = (byte)(num2 & 0xFF);
				array2[3] = 0;
				Array.Copy(byteArrayFromHexString, 0, array2, 3, num2);
				array = ((!mSLIP) ? buildASCIIData(array2) : buildSLIPData(array2));
			}
			byte[] array3 = new byte[1];
			if (array != null)
			{
				array3 = new byte[array.Length + 1];
				array3[0] = 0;
				Array.Copy(array, 0, array3, 1, array.Length);
			}
			byte[] command = new byte[2] { 4, 0 };
			string hexString = MTParser.getHexString(buildExtendedCommand(command, array3));
			sendDebugInfo("Send UART Extended Command: " + hexString);
			num = m_SCRA.sendExtendedCommand(hexString);
			if (num != 0)
			{
				sendDebugInfo("SendExtendedCommand error=" + num);
			}
		}
		return num;
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

	public void processDeviceExtendedResponse(byte[] dataBytes)
	{
		if (dataBytes == null || !isUARTDataNotification(dataBytes))
		{
			return;
		}
		int num = 5;
		int num2 = dataBytes.Length - num;
		if (num2 > 0)
		{
			byte[] array = new byte[num2];
			Array.Copy(dataBytes, num, array, 0, num2);
			if (array != null)
			{
				processUARTData(array);
			}
		}
	}

	protected bool isUARTDataNotification(byte[] dataBytes)
	{
		bool result = false;
		if (dataBytes != null && dataBytes.Length >= 3 && dataBytes[0] == 4 && dataBytes[1] == 0)
		{
			result = true;
		}
		return result;
	}

	protected void processUARTData(byte[] dataBytes)
	{
		if (dataBytes != null)
		{
			if (mSLIP)
			{
				processSLIPData(dataBytes);
			}
			else
			{
				processASCIIData(dataBytes);
			}
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
			processNotificationData(array);
			break;
		case 3:
		{
			byte[] data2 = MTRLEData.decodeRLEData(array);
			processNotificationData(data2);
			break;
		}
		case 4:
			if (this.OnDeviceResponse != null)
			{
				string hexString = MTParser.getHexString(array);
				this.OnDeviceResponse.Invoke((object)this, hexString);
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
		if (m_uartDataReceived != null)
		{
			int num2 = m_uartDataReceived.Length;
			array = new byte[num2 + num];
			Array.Copy(m_uartDataReceived, 0, array, 0, num2);
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
				string hexString = MTParser.getHexString(array2);
				sendDebugInfo("UART Data=" + hexString);
				string @string = Encoding.UTF8.GetString(array2);
				sendDebugInfo("UART Data ASCII=" + @string);
				byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(@string);
				if (byteArrayFromHexString != null)
				{
					int num6 = byteArrayFromHexString.Length;
					if (num6 > 1)
					{
						byte b = byteArrayFromHexString[0];
						byte[] array3 = new byte[num6 - 1];
						Array.Copy(byteArrayFromHexString, 1, array3, 0, num6 - 1);
						switch (b)
						{
						case 2:
							processNotificationData(array3);
							break;
						case 4:
						{
							string hexString2 = MTParser.getHexString(array3);
							if (this.OnDeviceResponse != null)
							{
								this.OnDeviceResponse.Invoke((object)this, hexString2);
							}
							break;
						}
						}
					}
				}
			}
			num4 = i + 1;
		}
		int num7 = num3 - num4;
		if (num7 > 0)
		{
			m_uartDataReceived = new byte[num7];
			Array.Copy(array, num4, m_uartDataReceived, 0, num7);
		}
		else
		{
			m_uartDataReceived = null;
		}
	}

	private void processNotificationData(byte[] data)
	{
		if (data != null && data.Length >= 8)
		{
			int num = data[0] & 0xFF;
			num <<= 8;
			num += data[1] & 0xFF;
			int num2 = data[2] & 0xFF;
			num2 <<= 8;
			num2 += data[3] & 0xFF;
			byte[] array = new byte[2]
			{
				data[4],
				data[5]
			};
			int num3 = data[6] & 0xFF;
			num3 <<= 8;
			num3 += data[7] & 0xFF;
			if (m_emvData == null)
			{
				m_emvDataLen = 0;
				m_emvData = new byte[num3 + 4];
				m_emvData[0] = array[0];
				m_emvData[1] = array[1];
				m_emvData[2] = data[6];
				m_emvData[3] = data[7];
			}
			if (data.Length >= num + 8)
			{
				Array.Copy(data, 8, m_emvData, num2 + 4, num);
				m_emvDataLen += num;
			}
			if (m_emvDataLen >= num3)
			{
				processEMVData(m_emvData);
				m_emvDataLen = 0;
				m_emvData = null;
			}
		}
	}

	protected void processEMVData(byte[] data)
	{
		if (data == null || data.Length <= 4)
		{
			return;
		}
		byte[] array = null;
		int num = data.Length - 4;
		if (num > 0)
		{
			array = new byte[num];
			Array.Copy(data, 4, array, 0, num);
		}
		if (data[0] != 3)
		{
			return;
		}
		switch (data[1])
		{
		case 0:
			if (this.OnTransactionStatus != null)
			{
				this.OnTransactionStatus.Invoke((object)this, array);
			}
			break;
		case 1:
			if (this.OnDisplayMessageRequest != null)
			{
				this.OnDisplayMessageRequest.Invoke((object)this, array);
			}
			break;
		case 2:
			if (this.OnUserSelectionRequest != null)
			{
				this.OnUserSelectionRequest.Invoke((object)this, array);
			}
			break;
		case 3:
			if (this.OnARQCReceived != null)
			{
				this.OnARQCReceived.Invoke((object)this, array);
			}
			break;
		case 4:
			if (this.OnTransactionResult != null)
			{
				this.OnTransactionResult.Invoke((object)this, array);
			}
			break;
		}
	}
}
