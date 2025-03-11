// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTDevice
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


public class MTDevice
{
	public delegate void DeviceListHandler(object sender, MTConnectionType connectionType, List<MTDeviceInformation> deviceList);

	public delegate void DeviceConnectionStateHandler(object sender, MTConnectionState state);

	public delegate void DeviceDataStringHandler(object sender, string dataString);

	public delegate void DeviceDataBytesHandler(object sender, byte[] dataBytes);

	public delegate void DeviceResponseMessageHandler(object sender, MTCMSResponseMessage response);

	public delegate void DeviceNotificationMessageHandler(object sender, MTCMSNotificationMessage notification);

	private MTConnectionType m_connectionType;

	private string m_address;

	private string m_deviceID;

	private MTCMSDevice m_device;

	private IMTService m_service;

	private ReaderWriterLockSlim mReaderWriterLockSlim = new ReaderWriterLockSlim();

	private bool mCommandPending;

	private CancellationTokenSource mTimeoutCancellationTokenSource;

	private CancellationToken mTimeoutCancellationToken = CancellationToken.None;

	private const int COMMAND_TIMEOUT = 3000;

	public const int SEND_SUCCESS = 0;

	public const int SEND_ERROR = 9;

	public const int SEND_BUSY = 15;

	public event DeviceListHandler OnDeviceList;

	public event DeviceConnectionStateHandler OnDeviceConnectionStateChanged;

	public event DeviceDataStringHandler OnDeviceDataString;

	public event DeviceDataBytesHandler OnDeviceDataBytes;

	public event DeviceResponseMessageHandler OnDeviceResponseMessage;

	public event DeviceNotificationMessageHandler OnDeviceNotificationMessage;

	protected void StartCommandTimeout()
	{
		mTimeoutCancellationTokenSource = new CancellationTokenSource();
		mTimeoutCancellationToken = mTimeoutCancellationTokenSource.Token;
		Task.Factory.StartNew((Func<Task>)async delegate
		{
			bool taskCanceled = false;
			try
			{
				await Task.Delay(3000, mTimeoutCancellationToken);
			}
			catch (TaskCanceledException)
			{
				taskCanceled = true;
			}
			catch (Exception)
			{
			}
			try
			{
				if (!taskCanceled && !mTimeoutCancellationToken.IsCancellationRequested)
				{
					ClearAllPendingCommands();
				}
			}
			catch (Exception)
			{
			}
			mTimeoutCancellationToken = CancellationToken.None;
		});
	}

	protected void StopCommandTimeout()
	{
		try
		{
			if (mTimeoutCancellationToken != CancellationToken.None)
			{
				if (mTimeoutCancellationToken.CanBeCanceled && mTimeoutCancellationTokenSource != null)
				{
					mTimeoutCancellationTokenSource.Cancel();
				}
				mTimeoutCancellationToken = CancellationToken.None;
			}
		}
		catch (Exception)
		{
		}
	}

	protected void SetCommandPending()
	{
		mReaderWriterLockSlim.EnterWriteLock();
		mCommandPending = true;
		mReaderWriterLockSlim.ExitWriteLock();
		StartCommandTimeout();
	}

	protected bool HasCommandPending()
	{
		mReaderWriterLockSlim.EnterReadLock();
		bool result = mCommandPending;
		mReaderWriterLockSlim.ExitReadLock();
		return result;
	}

	protected bool HasAnyCommandPending()
	{
		bool result = false;
		mReaderWriterLockSlim.EnterReadLock();
		if (mCommandPending)
		{
			result = true;
		}
		mReaderWriterLockSlim.ExitReadLock();
		return result;
	}

	protected void ClearAllPendingCommands()
	{
		StopCommandTimeout();
		mReaderWriterLockSlim.EnterWriteLock();
		mCommandPending = false;
		mReaderWriterLockSlim.ExitWriteLock();
	}

	public MTDevice()
	{
		m_device = new MTCMSDevice();
		m_device.DeviceStateChanged += OnDeviceStateChanged;
		m_device.DeviceResponseReceived += OnDeviceResponseReceived;
	}

	~MTDevice()
	{
	}

	public void requestDeviceList(MTConnectionType connectionType)
	{
		switch (connectionType)
		{
		case MTConnectionType.USB:
			MTHIDService.requestDeviceList(OnDeviceListReceived);
			break;
		case MTConnectionType.Serial:
			MTSerialService.requestDeviceList(OnDeviceListReceived);
			break;
		case MTConnectionType.IP:
		case MTConnectionType.BLE:
			break;
		}
	}

	public void setConnectionType(MTConnectionType connectionType)
	{
		m_connectionType = connectionType;
	}

	public void setAddress(string deviceAddress)
	{
		m_address = deviceAddress;
	}

	public void setDeviceID(string deviceID)
	{
		m_deviceID = deviceID;
	}

	public MTConnectionType getConnectionType()
	{
		return m_connectionType;
	}

	public string getAddress()
	{
		return m_address;
	}

	public string getDeviceID()
	{
		return m_deviceID;
	}

	public void openDevice()
	{
		if (!isDeviceConnected())
		{
			switch (m_connectionType)
			{
			case MTConnectionType.USB:
				m_service = new MTHIDService();
				m_service.setAddress(m_address);
				m_service.setDeviceID(m_deviceID);
				break;
			case MTConnectionType.IP:
				m_service = new MTNetService();
				m_service.setAddress(m_address);
				break;
			case MTConnectionType.Serial:
				m_service = new MTSerialService();
				m_service.setAddress(m_address);
				break;
			}
			if (m_device != null && m_service != null)
			{
				m_device.initialize(m_service);
				m_device.open();
			}
		}
	}

	public void closeDevice()
	{
		if (m_device != null)
		{
			m_device.close();
		}
	}

	public bool isDeviceConnected()
	{
		if (m_service != null && m_service.getState() == MTServiceState.Connected)
		{
			return true;
		}
		return false;
	}

	public int sendDataString(string dataString)
	{
		int result = 9;
		try
		{
			if (dataString != null)
			{
				byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(dataString);
				if (byteArrayFromHexString != null)
				{
					result = sendDataBytes(byteArrayFromHexString);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public int sendDataBytes(byte[] dataBytes)
	{
		int result = 9;
		if (isDeviceConnected())
		{
			if (HasAnyCommandPending())
			{
				return 15;
			}
			SetCommandPending();
			if (m_device.sendCommand(dataBytes))
			{
				result = 0;
			}
			else
			{
				ClearAllPendingCommands();
				result = 9;
			}
		}
		return result;
	}

	public int sendMTCMSMessage(MTCMSMessage message)
	{
		int result = 9;
		try
		{
			if (message != null)
			{
				byte[] messageBytes = message.getMessageBytes();
				if (messageBytes != null)
				{
					result = sendDataBytes(messageBytes);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	protected void SetDeviceResponseData(byte[] dataBytes)
	{
		ClearAllPendingCommands();
		if (dataBytes != null && dataBytes.Length != 0)
		{
			if (this.OnDeviceDataBytes != null)
			{
				this.OnDeviceDataBytes(this, dataBytes);
			}
			if (this.OnDeviceDataString != null)
			{
				this.OnDeviceDataString(this, MTParser.getHexString(dataBytes));
			}
			processDeviceResponseDataStream(dataBytes);
		}
	}

	protected void processDeviceResponseData(byte[] dataBytes)
	{
		MTCMSMessage mTCMSMessage = new MTCMSMessage(dataBytes);
		if (mTCMSMessage == null)
		{
			return;
		}
		if (mTCMSMessage.getMessageType() == 2)
		{
			if (this.OnDeviceResponseMessage != null)
			{
				MTCMSResponseMessage response = new MTCMSResponseMessage(mTCMSMessage.getApplicationID(), mTCMSMessage.getCommandID(), mTCMSMessage.getResultCode(), mTCMSMessage.getDataTag(), mTCMSMessage.getData());
				this.OnDeviceResponseMessage(this, response);
			}
		}
		else if (mTCMSMessage.getMessageType() == 3 && this.OnDeviceNotificationMessage != null)
		{
			MTCMSNotificationMessage notification = new MTCMSNotificationMessage(mTCMSMessage.getApplicationID(), mTCMSMessage.getCommandID(), mTCMSMessage.getResultCode(), mTCMSMessage.getDataTag(), mTCMSMessage.getData());
			this.OnDeviceNotificationMessage(this, notification);
		}
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

	protected void processDeviceResponseDataStream(byte[] dataBytes)
	{
		if (dataBytes == null)
		{
			return;
		}
		List<MTTLVObject> list = MTTLVParser.parseTLVByteArray(dataBytes, nestedParsing: false);
		if (list == null)
		{
			return;
		}
		int messageType = 0;
		int applicationID = 0;
		int commandID = 0;
		int resultCode = 0;
		int dataTag = 0;
		byte[] data = null;
		int num = 0;
		foreach (MTTLVObject item in list)
		{
			if (item == null)
			{
				continue;
			}
			byte[] tagByteArray = item.getTagByteArray();
			if (tagByteArray == null || tagByteArray.Length != 1)
			{
				continue;
			}
			switch (tagByteArray[0])
			{
			case 192:
				if (num > 0)
				{
					processMTCMSMessage(new MTCMSMessage(messageType, applicationID, commandID, resultCode, dataTag, data));
				}
				messageType = getFirstByteValue(item.getValueByteArray());
				applicationID = 0;
				commandID = 0;
				resultCode = 0;
				dataTag = 0;
				data = null;
				num++;
				break;
			case 193:
				applicationID = getFirstByteValue(item.getValueByteArray());
				break;
			case 194:
				commandID = getFirstByteValue(item.getValueByteArray());
				break;
			case 195:
				resultCode = getFirstByteValue(item.getValueByteArray());
				break;
			case 196:
				dataTag = 196;
				data = item.getValueByteArray();
				break;
			case 224:
				dataTag = 224;
				data = item.getValueByteArray();
				break;
			}
		}
		if (num > 0)
		{
			processMTCMSMessage(new MTCMSMessage(messageType, applicationID, commandID, resultCode, dataTag, data));
		}
	}

	protected void processMTCMSMessage(MTCMSMessage message)
	{
		if (message == null)
		{
			return;
		}
		if (message.getMessageType() == 2)
		{
			if (this.OnDeviceResponseMessage != null)
			{
				MTCMSResponseMessage response = new MTCMSResponseMessage(message.getApplicationID(), message.getCommandID(), message.getResultCode(), message.getDataTag(), message.getData());
				this.OnDeviceResponseMessage(this, response);
			}
		}
		else if (message.getMessageType() == 3 && this.OnDeviceNotificationMessage != null)
		{
			MTCMSNotificationMessage notification = new MTCMSNotificationMessage(message.getApplicationID(), message.getCommandID(), message.getResultCode(), message.getDataTag(), message.getData());
			this.OnDeviceNotificationMessage(this, notification);
		}
	}

	protected void OnDeviceListReceived(MTConnectionType connectionType, List<MTDeviceInformation> deviceList)
	{
		if (this.OnDeviceList != null)
		{
			this.OnDeviceList(this, connectionType, deviceList);
		}
	}

	protected void OnDeviceStateChanged(object sender, MTConnectionState state)
	{
		if (this.OnDeviceConnectionStateChanged != null)
		{
			this.OnDeviceConnectionStateChanged(this, state);
		}
	}

	protected void OnDeviceResponseReceived(object sender, byte[] data)
	{
		SetDeviceResponseData(data);
	}
}
