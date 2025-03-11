// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTSCRADevice
using System;
using System.Threading;


public class MTSCRADevice : IMTSCRADevice
{
	public delegate void DeviceStateHandler(object sender, MTConnectionState state);

	public delegate void CardDataStateHandler(object sender, MTCardDataState state);

	public delegate void CardDataHandler(object sender, IMTCardData scraData);

	public delegate void DeviceResponseHandler(object sender, byte[] data);

	public delegate void DeviceCardDataHandler(object sender, byte[] deviceData);

	public delegate void EMVDataHandler(object sender, byte[] data);

	public delegate void DeviceExtendedResponseHandler(object sender, byte[] data);

	private int m_dataThreshold;

	private IMTService m_service;

	private MTDataFormat m_dataFormat;

	private IMTCardData m_scraData;

	private IMTCardDataHandler m_scraDataHandler;

	private MTConnectionState m_connectionState;

	private MTCardDataState m_cardDataState;

	private int m_emvDataLen;

	private byte[] m_emvData;

	private int m_extendedCommanResponseLen;

	private byte[] m_extendedCommanResponse;

	private AutoResetEvent mExtendedCommandACK;

	private byte[] mSOM;

	private byte[] mEOM;

	private byte[] mFS;

	public event DeviceStateHandler DeviceStateChanged;

	public event CardDataStateHandler CardDataStateChanged;

	public event CardDataHandler CardDataReceived;

	public event DeviceCardDataHandler DeviceCardDataReceived;

	public event DeviceResponseHandler DeviceResponseReceived;

	public event EMVDataHandler EMVDataReceived;

	public event DeviceExtendedResponseHandler DeviceExtendedResponseReceived;

	public bool initialize(IMTService service, MTDataFormat dataFormat, int dataThreshold)
	{
		m_dataThreshold = dataThreshold;
		m_service = service;
		m_dataFormat = dataFormat;
		m_scraData = null;
		m_scraDataHandler = null;
		m_connectionState = MTConnectionState.Disconnected;
		m_cardDataState = MTCardDataState.DataNotReady;
		return true;
	}

	public void setSOM(byte[] somBytes)
	{
		mSOM = somBytes;
	}

	public void setEOM(byte[] eomBytes)
	{
		mEOM = eomBytes;
	}

	public void setFS(byte[] fsBytes)
	{
		mFS = fsBytes;
	}

	public bool open()
	{
		try
		{
			m_scraData = null;
			m_emvDataLen = 0;
			m_emvData = null;
			m_extendedCommanResponseLen = 0;
			m_extendedCommanResponse = null;
			if (m_service != null)
			{
				m_service.initialize(OnServiceState, OnCommandData, OnDeviceData, OnCardData, OnCardDataError);
				m_service.connect();
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	public void close()
	{
		try
		{
			if (m_service != null)
			{
				m_service.disconnect();
			}
		}
		catch (Exception)
		{
		}
	}

	public bool sendCommand(byte[] command)
	{
		try
		{
			if (m_service != null)
			{
				m_service.sendData(command);
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	public bool sendExtendedCommandPacket(byte[] command)
	{
		bool result = false;
		try
		{
			if (m_service != null)
			{
				mExtendedCommandACK = new AutoResetEvent(initialState: false);
				m_service.sendData(command);
				if (mExtendedCommandACK != null && mExtendedCommandACK.WaitOne(2000))
				{
					result = true;
					mExtendedCommandACK = null;
				}
				else
				{
					mExtendedCommandACK = null;
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	private MTConnectionState getConnectionState(MTServiceState serviceState)
	{
		MTConnectionState result = MTConnectionState.Error;
		switch (serviceState)
		{
		case MTServiceState.Disconnected:
			result = MTConnectionState.Disconnected;
			break;
		case MTServiceState.Connected:
			result = MTConnectionState.Connected;
			break;
		case MTServiceState.Connecting:
			result = MTConnectionState.Connecting;
			break;
		case MTServiceState.Disconnecting:
			result = MTConnectionState.Disconnecting;
			break;
		}
		return result;
	}

	private void setConnectionState(MTConnectionState connectionState)
	{
		if (connectionState != m_connectionState)
		{
			m_connectionState = connectionState;
			if (this.DeviceStateChanged != null)
			{
				this.DeviceStateChanged(this, connectionState);
			}
		}
	}

	private void setCardDataState(MTCardDataState cardDataState)
	{
		if (cardDataState != m_cardDataState)
		{
			m_cardDataState = cardDataState;
			if (this.CardDataStateChanged != null)
			{
				this.CardDataStateChanged(this, cardDataState);
			}
		}
	}

	protected void OnServiceState(MTServiceState serviceState)
	{
		MTConnectionState connectionState = getConnectionState(serviceState);
		setConnectionState(connectionState);
	}

	protected void OnCommandData(byte[] data)
	{
		handleCommandData(data);
	}

	protected void OnDeviceData(byte[] data)
	{
		handleDeviceData(data);
	}

	private void handleDeviceData(byte[] data)
	{
		if (data == null || data.Length < 8)
		{
			return;
		}
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
			if (this.EMVDataReceived != null)
			{
				this.EMVDataReceived(this, m_emvData);
			}
			m_emvDataLen = 0;
			m_emvData = null;
		}
	}

	private void setExtendedCommandACK()
	{
		try
		{
			if (mExtendedCommandACK != null)
			{
				mExtendedCommandACK.Set();
			}
		}
		catch (Exception)
		{
		}
	}

	protected void handleCommandData(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return;
		}
		if (data[0] == MTEMVDeviceConstants.PROTOCOL_EXTENDER_RESPONSE)
		{
			setExtendedCommandACK();
			if (data.Length < 8)
			{
				byte[] data2 = new byte[2]
				{
					data[4],
					data[5]
				};
				if (this.DeviceExtendedResponseReceived != null)
				{
					this.DeviceExtendedResponseReceived(this, data2);
				}
				m_extendedCommanResponseLen = 0;
				m_extendedCommanResponse = null;
				return;
			}
			int num = data[1] & 0xFF;
			num -= 6;
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
			if (num2 == 0)
			{
				m_extendedCommanResponseLen = 0;
				m_extendedCommanResponse = new byte[num3 + 4];
				m_extendedCommanResponse[0] = array[0];
				m_extendedCommanResponse[1] = array[1];
				m_extendedCommanResponse[2] = data[6];
				m_extendedCommanResponse[3] = data[7];
			}
			if (data.Length - 8 >= num)
			{
				Array.Copy(data, 8, m_extendedCommanResponse, num2 + 4, num);
				m_extendedCommanResponseLen += num;
			}
			if (m_extendedCommanResponseLen >= num3)
			{
				if (this.DeviceExtendedResponseReceived != null)
				{
					this.DeviceExtendedResponseReceived(this, m_extendedCommanResponse);
				}
				m_extendedCommanResponseLen = 0;
				m_extendedCommanResponse = null;
			}
			else
			{
				byte[] command = new byte[2] { 74, 0 };
				sendCommand(command);
			}
		}
		else if (data[0] == MTEMVDeviceConstants.PROTOCOL_EXTENDER_REQUEST_PENDING)
		{
			setExtendedCommandACK();
		}
		else
		{
			setExtendedCommandACK();
			if (this.DeviceResponseReceived != null)
			{
				this.DeviceResponseReceived(this, data);
			}
		}
	}

	protected void OnCardData(byte[] data)
	{
		handleCardData(data);
	}

	protected void OnCardDataError(MTCardDataError error)
	{
		setCardDataState(MTCardDataState.DataError);
	}

	private void handleCardData(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return;
		}
		if (m_scraData == null)
		{
			switch (m_dataFormat)
			{
			case MTDataFormat.SCRA_TLV:
				m_scraDataHandler = (IMTCardDataHandler)(m_scraData = new MTSCRATLVCardData());
				break;
			case MTDataFormat.SCRA_HID:
				if (m_service.getDeviceFeatures().SMSR)
				{
					MTSCRASMSRCardData mTSCRASMSRCardData = (MTSCRASMSRCardData)(m_scraDataHandler = (IMTCardDataHandler)(m_scraData = new MTSCRASMSRCardData()));
					if (mTSCRASMSRCardData != null)
					{
						if (mSOM != null)
						{
							mTSCRASMSRCardData.setSOM(mSOM);
						}
						if (mEOM != null)
						{
							mTSCRASMSRCardData.setEOM(mEOM);
						}
						if (mFS != null && mFS.Length != 0)
						{
							mTSCRASMSRCardData.setFS(mFS[0]);
						}
					}
				}
				else
				{
					m_scraDataHandler = (IMTCardDataHandler)(m_scraData = new MTSCRAHIDCardData());
					m_scraDataHandler.setDataThreshold(m_dataThreshold);
				}
				break;
			case MTDataFormat.SCRA_ASC:
				m_scraDataHandler = (IMTCardDataHandler)(m_scraData = new MTSCRAASCCardData());
				break;
			case MTDataFormat.SCRA_POS:
				m_scraDataHandler = (IMTCardDataHandler)(m_scraData = new MTSCRAPOSCardData());
				break;
			}
		}
		if (m_scraDataHandler == null)
		{
			return;
		}
		if (this.DeviceCardDataReceived != null && data != null)
		{
			this.DeviceCardDataReceived(this, data);
		}
		m_scraDataHandler.handleData(data);
		if (m_scraDataHandler.isDataReady())
		{
			setCardDataState(MTCardDataState.DataReady);
			if (this.CardDataReceived != null)
			{
				this.CardDataReceived(this, m_scraData);
			}
			m_scraDataHandler.clearData();
		}
		else
		{
			setCardDataState(MTCardDataState.DataNotReady);
		}
	}
}
