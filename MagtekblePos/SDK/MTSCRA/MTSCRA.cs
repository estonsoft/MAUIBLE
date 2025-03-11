// MTSCRANET, Version=1.0.23.1, Culture=neutral, PublicKeyToken=null
// MTSCRANET.MTSCRA
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class MTSCRA : IMTSCRA, IMTEMV
{
	protected enum ConnectedTransitionState
	{
		NONE,
		SET_SWIPEOUTPUT_USB,
		SET_SWIPEOUTPUT_BLE,
		GET_SOM,
		GET_EOM,
		GET_FS
	}

	protected enum UpdateFirmwareState
	{
		NONE,
		DOWNLOAD_FW_FILE,
		UPDATE_FW_FILE
	}

	public delegate void DeviceListHandler(object sender, MTConnectionType connectionType, List<MTDeviceInformation> deviceList);

	public delegate void DeviceConnectionStateHandler(object sender, MTConnectionState state);

	public delegate void CardDataStateHandler(object sender, MTCardDataState state);

	public delegate void DataReceivedHandler(object sender, IMTCardData cardData);

	public delegate void DeviceResponseHandler(object sender, string data);

	public delegate void DeviceDataReceivedHandler(object sender, byte[] data);

	public delegate void UpdateFirmwareStatusHandler(object sender, int status, int progress);

	public delegate void TransactionStatusHandler(object sender, byte[] data);

	public delegate void DisplayMessageRequestHandler(object sender, byte[] data);

	public delegate void UserSelectionRequestHandler(object sender, byte[] data);

	public delegate void ARQCReceivedHandler(object sender, byte[] data);

	public delegate void TransactionResultHandler(object sender, byte[] data);

	public delegate void NFCMessageHandler(object sender, byte[] data);

	public delegate void EMVCommandResultHandler(object sender, byte[] data);

	public delegate void DeviceExtendedResponseHandler(object sender, string data);

	public delegate void DeviceNotificationHandler(object sender, byte[] data);

	private const string SDK_VERSION = "112.01";

	private MTConnectionType m_connectionType;

	private string m_address;

	private string m_deviceID;

	private int m_dataThreshold;

	private MTSCRADevice m_device;

	private IMTService m_service;

	private MTDataFormat m_dataFormat;

	private IMTCardData m_cardData;

	private byte[] m_deviceData;

	private ReaderWriterLock mReaderWriterLock = new ReaderWriterLock();

	private ReaderWriterLock mFirmwareDownloadLock = new ReaderWriterLock();

	private bool mCommandPending;

	private bool mExtendedCommandPending;

	private bool mEMVCommandPending;

	private AutoResetEvent mDownloadFirmwareACK;

	private int mSwipeOutputRetryCount;

	protected ConnectedTransitionState mConnectedTransitionState;

	protected UpdateFirmwareState mUpdateFirmwareState;

	protected byte[] mFirmwareData;

	protected int mFirmwareDataProgress;

	protected int mFirmwareDataTotal;

	protected int mFirmwareDataProgressReported;

	private CancellationTokenSource mTimeoutCancellationTokenSource;

	private CancellationToken mTimeoutCancellationToken = CancellationToken.None;

	private const int SWIPE_OUTPUT_MAX_RETRIES = 1;

	private const int COMMAND_TIMEOUT = 3000;

	public const int SEND_COMMAND_SUCCESS = 0;

	public const int SEND_COMMAND_ERROR = 9;

	public const int SEND_COMMAND_BUSY = 15;

	public const int UPDATE_FIRMWARE_STARTED = 0;

	public const int UPDATE_FIRMWARE_SUCCESS = 1;

	public const int UPDATE_FIRMWARE_ERROR = 9;

	public const int UPDATE_FIRMWARE_NA = 15;

	public const int FIRMWARE_TYPE_BOOTLOADER = 0;

	public const int FIRMWARE_TYPE_MAINAPP = 1;

	private static AutoResetEvent m_syncExtendedEvent = new AutoResetEvent(initialState: false);

	private string m_syncExtendedData = "";

	public event DeviceListHandler OnDeviceList;

	public event DeviceConnectionStateHandler OnDeviceConnectionStateChanged;

	public event CardDataStateHandler OnCardDataState;

	public event DataReceivedHandler OnDataReceived;

	public event DeviceResponseHandler OnDeviceResponse;

	public event DeviceDataReceivedHandler OnDeviceDataReceived;

	public event TransactionStatusHandler OnTransactionStatus;

	public event DisplayMessageRequestHandler OnDisplayMessageRequest;

	public event UserSelectionRequestHandler OnUserSelectionRequest;

	public event ARQCReceivedHandler OnARQCReceived;

	public event TransactionResultHandler OnTransactionResult;

	public event NFCMessageHandler OnNFCMessage;

	public event EMVCommandResultHandler OnEMVCommandResult;

	public event DeviceExtendedResponseHandler OnDeviceExtendedResponse;

	public event DeviceNotificationHandler OnDeviceNotification;

	public event UpdateFirmwareStatusHandler OnUpdateFirmwareStatu;

	private string getTextValueFromExtendedResponse(string response)
	{
		string result = "";
		if (response.Length > 8)
		{
			byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(response.Substring(8));
			if (byteArrayFromHexString != null)
			{
				result = Encoding.UTF8.GetString(byteArrayFromHexString);
			}
		}
		return result;
	}

	public int updateFirmware(int type, byte[] data)
	{
		if (m_service != null && m_service.getDeviceFeatures().SMSR && m_service.getState() == MTServiceState.Connected && data != null && data.Length != 0 && type == 1)
		{
			updateFWSMSRMainApp(data);
			return 0;
		}
		sendUpdateFirmwareStatus(15);
		return 15;
	}

	protected async void updateFWSMSRMainApp(byte[] data)
	{
		if (data != null)
		{
			int num = data.Length;
			if (num > 0)
			{
				mFirmwareData = new byte[num];
				Array.Copy(data, mFirmwareData, num);
				mUpdateFirmwareState = UpdateFirmwareState.DOWNLOAD_FW_FILE;
				mFirmwareDataTotal = mFirmwareData.Length;
				mFirmwareDataProgress = 0;
				mFirmwareDataProgressReported = 0;
				sendUpdateFirmwareStatus(0);
				sendFirmwareDataAsync();
			}
		}
	}

	private async void sendFirmwareDataAsync()
	{
		await Task.Run(async delegate
		{
			await Task.Delay(10);
			bool result = true;
			while (result)
			{
				result = sendFirmwareData();
				await Task.Delay(10);
			}
		});
	}

	private bool sendFirmwareData()
	{
		int num = 4096;
		if (mFirmwareDataProgress >= mFirmwareDataTotal)
		{
			sendUpdateFirmware();
			return false;
		}
		if (mFirmwareDataProgress + num > mFirmwareDataTotal)
		{
			num = mFirmwareDataTotal - mFirmwareDataProgress;
		}
		byte[] array = new byte[num];
		Array.Copy(mFirmwareData, mFirmwareDataProgress, array, 0, num);
		string text = mFirmwareDataTotal.ToString("X8") + mFirmwareDataProgress.ToString("X8") + MTParser.getHexString(array);
		string text2 = (num + 8).ToString("X4");
		string extendedCommand = "0711" + text2 + text;
		mFirmwareDataProgress += num;
		try
		{
			sendExtendedCommandSync(extendedCommand);
		}
		catch (Exception)
		{
		}
		int num2 = mFirmwareDataProgress * 90 / mFirmwareDataTotal;
		if (num2 >= mFirmwareDataProgressReported + 5)
		{
			mFirmwareDataProgressReported = num2;
			sendUpdateFirmwareStatus(0, num2);
		}
		return true;
	}

	private string sendExtendedCommandSync(string extendedCommand)
	{
		string result = "";
		sendExtendedCommand(extendedCommand);
		if (m_syncExtendedEvent.WaitOne(3000))
		{
			result = m_syncExtendedData;
		}
		return result;
	}

	private void sendUpdateFirmware()
	{
		mUpdateFirmwareState = UpdateFirmwareState.UPDATE_FW_FILE;
		string extendedCommand = "07120000";
		sendUpdateFirmwareStatus(0, 98);
		if (sendExtendedCommandSync(extendedCommand).StartsWith("0000"))
		{
			mUpdateFirmwareState = UpdateFirmwareState.NONE;
			sendUpdateFirmwareStatus(1, 100);
		}
		else
		{
			mUpdateFirmwareState = UpdateFirmwareState.NONE;
			sendUpdateFirmwareStatus(9);
		}
	}

	protected void sendUpdateFirmwareStatus(int status, int progress = 0)
	{
		this.OnUpdateFirmwareStatu(this, status, progress);
	}

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
					if (mConnectedTransitionState == ConnectedTransitionState.SET_SWIPEOUTPUT_USB)
					{
						mConnectedTransitionState = ConnectedTransitionState.NONE;
						if (mSwipeOutputRetryCount < 1)
						{
							retrySetSwipeOutputToUSB();
						}
						else
						{
							disconnectAsync();
						}
					}
					else if (mConnectedTransitionState == ConnectedTransitionState.SET_SWIPEOUTPUT_BLE)
					{
						mConnectedTransitionState = ConnectedTransitionState.NONE;
						if (mSwipeOutputRetryCount < 1)
						{
							retrySetSwipeOutputToBLE();
						}
						else
						{
							disconnectAsync();
						}
					}
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
					mTimeoutCancellationTokenSource.CancelAfter(1);
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
		mReaderWriterLock.AcquireWriterLock(-1);
		mCommandPending = true;
		mReaderWriterLock.ReleaseWriterLock();
		StartCommandTimeout();
	}

	protected void SetExtendedCommandPending()
	{
		mReaderWriterLock.AcquireWriterLock(-1);
		mExtendedCommandPending = true;
		mReaderWriterLock.ReleaseWriterLock();
		StartCommandTimeout();
	}

	protected void SetEMVCommandPending()
	{
		mReaderWriterLock.AcquireWriterLock(-1);
		mEMVCommandPending = true;
		mReaderWriterLock.ReleaseWriterLock();
		StartCommandTimeout();
	}

	protected bool HasCommandPending()
	{
		mReaderWriterLock.AcquireReaderLock(-1);
		bool result = mCommandPending;
		mReaderWriterLock.ReleaseReaderLock();
		return result;
	}

	protected bool HasExtendedCommandPending()
	{
		mReaderWriterLock.AcquireReaderLock(-1);
		bool result = mExtendedCommandPending;
		mReaderWriterLock.ReleaseReaderLock();
		return result;
	}

	protected bool HasEMVCommandPending()
	{
		mReaderWriterLock.AcquireReaderLock(-1);
		bool result = mEMVCommandPending;
		mReaderWriterLock.ReleaseReaderLock();
		return result;
	}

	protected bool HasAnyCommandPending()
	{
		bool result = false;
		mReaderWriterLock.AcquireReaderLock(-1);
		if (mCommandPending || mExtendedCommandPending || mEMVCommandPending)
		{
			result = true;
		}
		mReaderWriterLock.ReleaseReaderLock();
		return result;
	}

	protected void ClearAllPendingCommands()
	{
		StopCommandTimeout();
		mReaderWriterLock.AcquireWriterLock(-1);
		mCommandPending = false;
		mExtendedCommandPending = false;
		mEMVCommandPending = false;
		mReaderWriterLock.ReleaseWriterLock();
	}

	public MTSCRA()
	{
		m_device = new MTSCRADevice();
		m_device.DeviceStateChanged += OnDeviceStateChanged;
		m_device.CardDataStateChanged += OnCardDataStateChanged;
		m_device.CardDataReceived += OnCardDataReceived;
		m_device.DeviceCardDataReceived += OnDeviceCardDataReceived;
		m_device.DeviceResponseReceived += OnDeviceResponseReceived;
		m_device.EMVDataReceived += OnEMVDataReceived;
		m_device.DeviceExtendedResponseReceived += OnDeviceExtendedResponseReceived;
	}

	~MTSCRA()
	{
	}

	public void requestDeviceList(MTConnectionType connectionType)
	{
		switch (connectionType)
		{
		case MTConnectionType.BLE:
			MTBLEService.requestDeviceList(MTDeviceConstants.UUID_SCRA_BLE_DEVICE_READER_SERVICE, OnDeviceListReceived);
			break;
		case MTConnectionType.BLEEMV:
			MTBLEService.requestDeviceList(MTDeviceConstants.UUID_SCRA_BLE_EMV_DEVICE_READER_SERVICE, OnDeviceListReceived);
			break;
		case MTConnectionType.BLEEMVT:
			MTBLEService.requestDeviceList(MTDeviceConstants.UUID_SCRA_BLE_EMV_T_DEVICE_READER_SERVICE, OnDeviceListReceived);
			break;
		case MTConnectionType.USB:
			MTHIDService.requestDeviceList(OnDeviceListReceived);
			break;
		case MTConnectionType.Serial:
			MTSerialService.requestDeviceList(OnDeviceListReceived);
			break;
		case MTConnectionType.Audio:
		case MTConnectionType.Bluetooth:
		case MTConnectionType.Net:
		case MTConnectionType.Net_TLS12:
		case MTConnectionType.Net_TLS12_Trust_All:
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

	public void openDevice()
	{
		switch (m_connectionType)
		{
		case MTConnectionType.Audio:
			m_service = new MTAudioService();
			m_dataFormat = MTDataFormat.SCRA_TLV;
			break;
		case MTConnectionType.BLE:
			m_service = new MTBLEService();
			m_service.setAddress(m_address);
			m_service.setServiceUUID(MTDeviceConstants.UUID_SCRA_BLE_DEVICE_READER_SERVICE);
			m_dataFormat = MTDataFormat.SCRA_HID;
			m_dataThreshold = 200;
			break;
		case MTConnectionType.BLEEMV:
			m_service = new MTBLEService();
			m_service.setAddress(m_address);
			m_service.setServiceUUID(MTDeviceConstants.UUID_SCRA_BLE_EMV_DEVICE_READER_SERVICE);
			m_dataFormat = MTDataFormat.SCRA_HID;
			m_dataThreshold = 200;
			break;
		case MTConnectionType.BLEEMVT:
			m_service = new MTBLEService();
			m_service.setAddress(m_address);
			m_service.setServiceUUID(MTDeviceConstants.UUID_SCRA_BLE_EMV_T_DEVICE_READER_SERVICE);
			m_dataFormat = MTDataFormat.SCRA_HID;
			m_dataThreshold = 200;
			break;
		case MTConnectionType.USB:
			m_service = new MTHIDService();
			m_dataFormat = MTDataFormat.SCRA_HID;
			m_dataThreshold = 336;
			break;
		case MTConnectionType.Serial:
			m_service = new MTSerialService();
			m_service.setAddress(m_address);
			m_dataFormat = MTDataFormat.SCRA_HID;
			m_dataThreshold = 336;
			break;
		}
		if (m_device != null && m_service != null)
		{
			m_service.setAddress(m_address);
			m_device.initialize(m_service, m_dataFormat, m_dataThreshold);
			m_device.open();
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

	public bool isDeviceEMV()
	{
		bool result = false;
		if (m_connectionType == MTConnectionType.BLEEMV || m_connectionType == MTConnectionType.BLEEMVT)
		{
			result = true;
		}
		else if (m_connectionType == MTConnectionType.USB)
		{
			if (m_service != null)
			{
				result = m_service.isServiceEMV();
			}
		}
		else if (m_connectionType == MTConnectionType.Serial)
		{
			result = true;
		}
		return result;
	}

	public bool isDeviceOEM()
	{
		bool result = false;
		if (m_connectionType == MTConnectionType.USB && m_service != null)
		{
			result = m_service.isServiceOEM();
		}
		return result;
	}

	public string getPowerManagementValue()
	{
		string result = "";
		if (m_service != null)
		{
			result = m_service.getDevicePMValue();
		}
		return result;
	}

	public MTDeviceFeatures getDeviceFeatures()
	{
		MTDeviceFeatures result = new MTDeviceFeatures();
		if (m_service != null)
		{
			result = m_service.getDeviceFeatures();
		}
		else
		{
			switch (m_connectionType)
			{
			case MTConnectionType.Audio:
				result = MTAudioService.GetDeviceFeatures();
				break;
			case MTConnectionType.BLE:
				result = MTBLEService.GetDeviceFeatures(MTDeviceConstants.UUID_SCRA_BLE_DEVICE_READER_SERVICE);
				break;
			case MTConnectionType.BLEEMV:
				result = MTBLEService.GetDeviceFeatures(MTDeviceConstants.UUID_SCRA_BLE_EMV_DEVICE_READER_SERVICE);
				break;
			case MTConnectionType.BLEEMVT:
				result = MTBLEService.GetDeviceFeatures(MTDeviceConstants.UUID_SCRA_BLE_EMV_T_DEVICE_READER_SERVICE);
				break;
			case MTConnectionType.USB:
				result = MTHIDService.GetDeviceFeatures(m_address);
				break;
			case MTConnectionType.Serial:
				result = MTSerialService.GetDeviceFeatures();
				break;
			}
		}
		return result;
	}

	public string getMaskedTracks()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getMaskedTracks();
		}
		return result;
	}

	public string getTrack1()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getTrack1();
		}
		return result;
	}

	public string getTrack2()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getTrack2();
		}
		return result;
	}

	public string getTrack3()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getTrack3();
		}
		return result;
	}

	public string getTrack1Masked()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getTrack1Masked();
		}
		return result;
	}

	public string getTrack2Masked()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getTrack2Masked();
		}
		return result;
	}

	public string getTrack3Masked()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getTrack3Masked();
		}
		return result;
	}

	public string getMagnePrint()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getMagnePrint();
		}
		return result;
	}

	public string getMagnePrintStatus()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getMagnePrintStatus();
		}
		return result;
	}

	public string getDeviceSerial()
	{
		string text = "";
		if (m_cardData != null)
		{
			text = m_cardData.getDeviceSerial();
		}
		if (string.IsNullOrWhiteSpace(text) && m_service != null)
		{
			if (getDeviceFeatures().SMSR)
			{
				string response = sendExtendedCommandSync("0000000103");
				text = getTextValueFromExtendedResponse(response);
			}
			else
			{
				text = m_service.getDeviceSerial();
			}
		}
		return text;
	}

	public string getSessionID()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getSessionID();
		}
		return result;
	}

	public string getKSN()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getKSN();
		}
		return result;
	}

	public string getDeviceName()
	{
		string text = "";
		if (m_cardData != null)
		{
			m_cardData.getDeviceName();
		}
		if (string.IsNullOrWhiteSpace(text) && m_service != null)
		{
			text = m_service.getDeviceName();
		}
		return text;
	}

	public void clearBuffers()
	{
		if (m_cardData != null)
		{
			m_cardData.clearBuffers();
		}
		m_deviceData = null;
	}

	public long getBatteryLevel()
	{
		long num = MTDeviceConstants.BATTERY_LEVEL_NA;
		if (m_cardData != null)
		{
			num = m_cardData.getBatteryLevel();
		}
		if ((num < MTDeviceConstants.BATTERY_LEVEL_MIN || num > MTDeviceConstants.BATTERY_LEVEL_MAX) && m_service != null)
		{
			num = m_service.getBatteryLevel();
		}
		return num;
	}

	public long getSwipeCount()
	{
		long result = MTDeviceConstants.SWIPE_COUNT_NA;
		if (m_cardData != null)
		{
			result = m_cardData.getSwipeCount();
		}
		return result;
	}

	public string getCapMagnePrint()
	{
		string text = "";
		if (m_cardData != null)
		{
			text = m_cardData.getCapMagnePrint();
		}
		if (string.IsNullOrEmpty(text) && m_service != null)
		{
			text = m_service.getCapMagnePrint();
		}
		return text;
	}

	public string getCapMagnePrintEncryption()
	{
		string text = "";
		if (m_cardData != null)
		{
			text = m_cardData.getCapMagnePrintEncryption();
		}
		if (string.IsNullOrEmpty(text) && m_service != null)
		{
			text = m_service.getCapMagnePrintEncryption();
		}
		return text;
	}

	public string getCapMagneSafe20Encryption()
	{
		string text = "";
		if (m_cardData != null)
		{
			text = m_cardData.getCapMagneSafe20Encryption();
		}
		if (string.IsNullOrEmpty(text) && m_service != null)
		{
			text = m_service.getCapMagneSafe20Encryption();
		}
		return text;
	}

	public string getCapMagStripeEncryption()
	{
		string encryptionLevel = "";
		if (m_cardData != null)
		{
			encryptionLevel = m_cardData.getCapMagStripeEncryption();
		}
		if (string.IsNullOrEmpty("1") && m_service != null)
		{
			encryptionLevel = m_service.getCapMagStripeEncryption();
		}
		return GetEncryptionFromEncryptionLevel(encryptionLevel);
	}

	public string getCapMSR()
	{
		string text = "";
		if (m_cardData != null)
		{
			text = m_cardData.getCapMSR();
		}
		if (string.IsNullOrEmpty(text) && m_service != null)
		{
			text = m_service.getCapMSR();
		}
		return text;
	}

	public string getCapTracks()
	{
		string text = "";
		if (m_cardData != null)
		{
			text = m_cardData.getCapTracks();
		}
		if (string.IsNullOrEmpty(text) && m_service != null)
		{
			text = m_service.getCapTracks();
		}
		return text;
	}

	public long getCardDataCRC()
	{
		long result = 0L;
		if (m_cardData != null)
		{
			result = m_cardData.getCardDataCRC();
		}
		return result;
	}

	public string getCardExpDate()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getCardExpDate();
		}
		return result;
	}

	public string getCardIIN()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getCardIIN();
		}
		return result;
	}

	public string getCardLast4()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getCardLast4();
		}
		return result;
	}

	public string getCardName()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getCardName();
		}
		return result;
	}

	public string getCardPAN()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getCardPAN();
		}
		return result;
	}

	public int getCardPANLength()
	{
		int result = 0;
		if (m_cardData != null)
		{
			result = m_cardData.getCardPANLength();
		}
		return result;
	}

	public string getCardServiceCode()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getCardServiceCode();
		}
		return result;
	}

	public string getCardStatus()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getCardStatus();
		}
		return result;
	}

	public string getCardEncodeType()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getCardEncodeType();
		}
		return result;
	}

	public int getDataFieldCount()
	{
		int result = 0;
		if (m_cardData != null)
		{
			result = m_cardData.getDataFieldCount();
		}
		return result;
	}

	public string getHashCode()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getHashCode();
		}
		return result;
	}

	public string getDeviceConfig(string configType)
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getDeviceConfig(configType);
		}
		return result;
	}

	public string getEncryptionStatus()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getEncryptionStatus();
		}
		return result;
	}

	public string getFirmware()
	{
		string text = "";
		if (m_cardData != null)
		{
			m_cardData.getFirmware();
		}
		if (string.IsNullOrWhiteSpace(text) && m_service != null)
		{
			if (getDeviceFeatures().SMSR)
			{
				string response = sendExtendedCommandSync("0000000100");
				text = getTextValueFromExtendedResponse(response);
			}
			else
			{
				text = m_service.getFirmwareID();
			}
		}
		return text;
	}

	public string getMagTekDeviceSerial()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getMagTekDeviceSerial();
		}
		return result;
	}

	public string getResponseData()
	{
		string result = "";
		byte[] data = m_cardData.getData();
		if (data != null)
		{
			result = MTParser.getHexString(data);
		}
		return result;
	}

	public string getResponseType()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getResponseType();
		}
		return result;
	}

	public string getTagValue(string tag, string data)
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getTagValue(tag, data);
		}
		return result;
	}

	public string getTLVVersion()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getTLVVersion();
		}
		return result;
	}

	public string getTrackDecodeStatus()
	{
		string result = "";
		if (m_cardData != null)
		{
			result = m_cardData.getTrackDecodeStatus();
		}
		return result;
	}

	public string getSDKVersion()
	{
		return "112.01";
	}

	public int sendCommandToDevice(string command)
	{
		if (HasAnyCommandPending())
		{
			return 15;
		}
		byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(command);
		SetCommandPending();
		if (!m_device.sendCommand(byteArrayFromHexString))
		{
			ClearAllPendingCommands();
			return 9;
		}
		return 0;
	}

	protected void sendCommandToDeviceAsync(string command)
	{
		string state = (string)command.Clone();
		Task.Factory.StartNew((Func<object, Task>)async delegate(object obj)
		{
			await Task.Delay(1);
			sendCommandToDevice((string)obj);
		}, (object)state);
	}

	protected void sendCommandToDeviceAsyncDelayed(string command, int delay)
	{
		string state = (string)command.Clone();
		Task.Factory.StartNew((Func<object, Task>)async delegate(object obj)
		{
			await Task.Delay(delay);
			sendCommandToDevice((string)obj);
		}, (object)state);
	}

	protected void disconnectAsync()
	{
		Task.Factory.StartNew((Func<object, Task>)async delegate
		{
			await Task.Delay(1);
			closeDevice();
		}, (object)null);
	}

	protected void SetDeviceResponseData(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return;
		}
		ClearAllPendingCommands();
		if (mConnectedTransitionState == ConnectedTransitionState.SET_SWIPEOUTPUT_USB || mConnectedTransitionState == ConnectedTransitionState.SET_SWIPEOUTPUT_BLE)
		{
			if (data.Length != 0)
			{
				mConnectedTransitionState = ConnectedTransitionState.NONE;
				sendConnectionStateChanged(MTConnectionState.Connected);
			}
		}
		else if (this.OnDeviceResponse != null)
		{
			this.OnDeviceResponse(this, MTParser.getHexString(data));
		}
	}

	private byte[] getPropertyValueFromExtendedResponse(byte[] data)
	{
		byte[] array = null;
		if (data != null)
		{
			int num = data.Length - 4;
			if (num > 0)
			{
				array = new byte[num];
				Array.Copy(data, 4, array, 0, num);
			}
		}
		return array;
	}

	protected void SetDeviceExtendedResponseData(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			return;
		}
		if (HasEMVCommandPending())
		{
			ClearAllPendingCommands();
			if (this.OnEMVCommandResult != null)
			{
				this.OnEMVCommandResult(this, data);
			}
			return;
		}
		ClearAllPendingCommands();
		if (mConnectedTransitionState != 0)
		{
			if (mConnectedTransitionState == ConnectedTransitionState.GET_SOM)
			{
				byte[] propertyValueFromExtendedResponse = getPropertyValueFromExtendedResponse(data);
				if (m_device != null)
				{
					m_device.setSOM(propertyValueFromExtendedResponse);
				}
				getEOM();
			}
			else if (mConnectedTransitionState == ConnectedTransitionState.GET_EOM)
			{
				byte[] propertyValueFromExtendedResponse2 = getPropertyValueFromExtendedResponse(data);
				if (m_device != null)
				{
					m_device.setEOM(propertyValueFromExtendedResponse2);
				}
				getFS();
			}
			else if (mConnectedTransitionState == ConnectedTransitionState.GET_FS)
			{
				byte[] propertyValueFromExtendedResponse3 = getPropertyValueFromExtendedResponse(data);
				if (propertyValueFromExtendedResponse3 != null && propertyValueFromExtendedResponse3.Length != 0 && m_device != null)
				{
					m_device.setFS(propertyValueFromExtendedResponse3);
				}
				mConnectedTransitionState = ConnectedTransitionState.NONE;
				sendConnectionStateChanged(MTConnectionState.Connected);
			}
		}
		else if (this.OnDeviceExtendedResponse != null)
		{
			m_syncExtendedData = MTParser.getHexString(data);
			m_syncExtendedEvent.Set();
			if (mUpdateFirmwareState == UpdateFirmwareState.NONE)
			{
				this.OnDeviceExtendedResponse(this, MTParser.getHexString(data));
			}
		}
	}

	protected void SetEMVData(byte[] data)
	{
		if (data == null || data.Length <= 2)
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
		if (data[0] == 3)
		{
			switch (data[1])
			{
			case 0:
				if (this.OnTransactionStatus != null)
				{
					this.OnTransactionStatus(this, array);
				}
				break;
			case 1:
				if (this.OnDisplayMessageRequest != null)
				{
					this.OnDisplayMessageRequest(this, array);
				}
				break;
			case 2:
				if (this.OnUserSelectionRequest != null)
				{
					this.OnUserSelectionRequest(this, array);
				}
				break;
			case 3:
				if (this.OnARQCReceived != null)
				{
					this.OnARQCReceived(this, array);
				}
				break;
			case 4:
				if (this.OnTransactionResult != null)
				{
					this.OnTransactionResult(this, array);
				}
				break;
			case 48:
				if (this.OnNFCMessage != null)
				{
					this.OnNFCMessage(this, array);
				}
				break;
			}
		}
		else
		{
			SetDeviceExtendedResponseData(data);
		}
	}

	protected void SetCardData(IMTCardData cardData)
	{
		if (cardData != null)
		{
			m_cardData = cardData;
			if (this.OnDataReceived != null)
			{
				this.OnDataReceived(this, cardData);
			}
		}
	}

	protected void SetDeviceData(byte[] deviceData)
	{
		if (deviceData != null)
		{
			m_deviceData = deviceData;
			if (this.OnDeviceDataReceived != null)
			{
				this.OnDeviceDataReceived(this, deviceData);
			}
		}
	}

	protected string GetEncryptionFromEncryptionLevel(string encryptionLevel)
	{
		string result = "1";
		if (!string.IsNullOrEmpty(encryptionLevel))
		{
			try
			{
				if (int.Parse(encryptionLevel) == 0)
				{
					result = "0";
				}
			}
			catch (Exception)
			{
			}
		}
		return result;
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
		bool flag = true;
		if (state == MTConnectionState.Connected)
		{
			ClearAllPendingCommands();
			mConnectedTransitionState = ConnectedTransitionState.NONE;
			if (m_service != null)
			{
				if (m_service.isOutputChannelConfigurable())
				{
					if (m_connectionType == MTConnectionType.USB)
					{
						flag = false;
						setSwipeOutputToUSB();
					}
					else if (m_connectionType == MTConnectionType.BLE || m_connectionType == MTConnectionType.BLEEMV || m_connectionType == MTConnectionType.BLEEMVT)
					{
						flag = false;
						setSwipeOutputToBLE();
					}
				}
				else if (m_service.getDeviceFeatures().SMSR)
				{
					flag = false;
					getSOM();
				}
			}
		}
		if (flag)
		{
			sendConnectionStateChanged(state);
		}
	}

	protected void getSOM()
	{
		mConnectedTransitionState = ConnectedTransitionState.GET_SOM;
		sendCommandToDeviceAsync("49070000000000011E");
	}

	protected void getEOM()
	{
		mConnectedTransitionState = ConnectedTransitionState.GET_EOM;
		sendCommandToDeviceAsync("490700000000000122");
	}

	protected void getFS()
	{
		mConnectedTransitionState = ConnectedTransitionState.GET_FS;
		sendCommandToDeviceAsync("490700000000000123");
	}

	protected void setSwipeOutputToUSB()
	{
		mConnectedTransitionState = ConnectedTransitionState.SET_SWIPEOUTPUT_USB;
		mSwipeOutputRetryCount = 0;
		sendCommandToDeviceAsync(MTDeviceConstants.SCRA_DEVICE_COMMAND_STRING_SET_CARD_SWIPE_OUTPUT_CHANNEL_OVERRIDE_USB);
	}

	protected void setSwipeOutputToBLE()
	{
		mConnectedTransitionState = ConnectedTransitionState.SET_SWIPEOUTPUT_BLE;
		mSwipeOutputRetryCount = 0;
		sendCommandToDeviceAsync(MTDeviceConstants.SCRA_DEVICE_COMMAND_STRING_SET_CARD_SWIPE_OUTPUT_CHANNEL_OVERRIDE_BLE);
	}

	protected void retrySetSwipeOutputToUSB()
	{
		mConnectedTransitionState = ConnectedTransitionState.SET_SWIPEOUTPUT_USB;
		mSwipeOutputRetryCount++;
		sendCommandToDeviceAsync(MTDeviceConstants.SCRA_DEVICE_COMMAND_STRING_SET_CARD_SWIPE_OUTPUT_CHANNEL_OVERRIDE_USB);
	}

	protected void retrySetSwipeOutputToBLE()
	{
		mConnectedTransitionState = ConnectedTransitionState.SET_SWIPEOUTPUT_BLE;
		mSwipeOutputRetryCount++;
		sendCommandToDeviceAsync(MTDeviceConstants.SCRA_DEVICE_COMMAND_STRING_SET_CARD_SWIPE_OUTPUT_CHANNEL_OVERRIDE_BLE);
	}

	protected void sendConnectionStateChanged(MTConnectionState state)
	{
		if (this.OnDeviceConnectionStateChanged != null)
		{
			this.OnDeviceConnectionStateChanged(this, state);
		}
	}

	protected void OnCardDataStateChanged(object sender, MTCardDataState state)
	{
		if (this.OnCardDataState != null)
		{
			this.OnCardDataState(this, state);
		}
	}

	protected void OnCardDataReceived(object sender, IMTCardData cardData)
	{
		SetCardData(cardData);
	}

	protected void OnDeviceCardDataReceived(object sender, byte[] deviceData)
	{
		SetDeviceData(deviceData);
	}

	protected void OnDeviceResponseReceived(object sender, byte[] data)
	{
		SetDeviceResponseData(data);
	}

	protected void OnDeviceExtendedResponseReceived(object sender, byte[] data)
	{
		SetDeviceExtendedResponseData(data);
	}

	protected void OnEMVDataReceived(object sender, byte[] data)
	{
		SetEMVData(data);
	}

	public void sendExtendedCommandAsync(string command, int delay = 1)
	{
		string state = (string)command.Clone();
		Task.Factory.StartNew((Func<object, Task>)async delegate(object obj)
		{
			await Task.Delay(delay);
			sendExtendedCommand((string)obj);
		}, (object)state);
	}

	public int sendNFCCommand(string command, bool lastCommand = false, bool encrypt = false)
	{
		if (command != null)
		{
			byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(command);
			if (byteArrayFromHexString != null)
			{
				int num = byteArrayFromHexString.Length;
				if (num > 0)
				{
					byte[] array = new byte[18 + num];
					array[0] = 4;
					Array.Copy(byteArrayFromHexString, 0, array, 18, num);
					return sendEMVCommand(new byte[2] { 3, 48 }, array);
				}
			}
		}
		return 9;
	}

	public int sendClassicNFCCommand(string command, bool lastCommand = false, bool encrypt = false)
	{
		if (command != null)
		{
			byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(command);
			if (byteArrayFromHexString != null)
			{
				int num = byteArrayFromHexString.Length;
				if (num > 0)
				{
					byte[] array = new byte[18 + num];
					array[0] = 0;
					Array.Copy(byteArrayFromHexString, 0, array, 18, num);
					return sendEMVCommand(new byte[2] { 3, 48 }, array);
				}
			}
		}
		return 9;
	}

	public int sendDESFireNFCCommand(string command, bool lastCommand = false, bool encrypt = false)
	{
		if (command != null)
		{
			byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(command);
			if (byteArrayFromHexString != null)
			{
				int num = byteArrayFromHexString.Length;
				if (num > 0)
				{
					byte[] array = new byte[18 + num];
					array[0] = 1;
					Array.Copy(byteArrayFromHexString, 0, array, 18, num);
					return sendEMVCommand(new byte[2] { 3, 48 }, array);
				}
			}
		}
		return 9;
	}

	public int sendExtendedCommand(string command)
	{
		if (HasAnyCommandPending())
		{
			return 15;
		}
		byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(command);
		if (byteArrayFromHexString.Length >= 4)
		{
			byte[] command2 = new byte[2]
			{
				byteArrayFromHexString[0],
				byteArrayFromHexString[1]
			};
			int num = byteArrayFromHexString.Length - 4;
			int num2 = byteArrayFromHexString[2] & 0xFF;
			num2 <<= 8;
			num2 += byteArrayFromHexString[3] & 0xFF;
			if (num >= num2)
			{
				byte[] array = new byte[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = byteArrayFromHexString[i + 4];
				}
				SetExtendedCommandPending();
				int num3 = sendExtendedCommandBytes(command2, array);
				if (num3 == 9)
				{
					ClearAllPendingCommands();
				}
				return num3;
			}
		}
		return 9;
	}

	private int sendEMVCommand(byte[] command, byte[] data)
	{
		if (HasAnyCommandPending())
		{
			return 15;
		}
		SetEMVCommandPending();
		int num = sendExtendedCommandBytes(command, data);
		if (num == 9)
		{
			ClearAllPendingCommands();
		}
		return num;
	}

	private int sendExtendedCommandBytes(byte[] command, byte[] data)
	{
		int num = 0;
		int num2 = 0;
		if (command != null)
		{
			num = command.Length;
		}
		if (num != 2)
		{
			return 9;
		}
		if (data != null)
		{
			num2 = data.Length;
		}
		int num3 = 0;
		while (num3 < num2 || num2 == 0)
		{
			int num4 = num2 - num3;
			if (num4 >= 52)
			{
				num4 = 51;
			}
			byte[] array = new byte[8 + num4];
			array[0] = MTEMVDeviceConstants.PROTOCOL_EXTENDER_REQUEST;
			array[1] = (byte)(6 + num4);
			array[2] = (byte)((num3 >> 8) & 0xFF);
			array[3] = (byte)(num3 & 0xFF);
			array[4] = command[0];
			array[5] = command[1];
			array[6] = (byte)((num2 >> 8) & 0xFF);
			array[7] = (byte)(num2 & 0xFF);
			for (int i = 0; i < num4; i++)
			{
				array[8 + i] = data[num3 + i];
			}
			num3 += num4;
			MTParser.getHexString(array);
			if (!m_device.sendExtendedCommandPacket(array))
			{
				return 9;
			}
			if (num2 == 0)
			{
				break;
			}
		}
		return 0;
	}

	public int startTransaction(byte timeLimit, byte cardType, byte option, byte[] amount, byte transactionType, byte[] cashBack, byte[] currencyCode, byte reportingOption)
	{
		byte[] array = new byte[19]
		{
			timeLimit, cardType, option, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0
		};
		int num = amount.Length;
		if (num > 6)
		{
			num = 6;
		}
		for (int i = 0; i < num; i++)
		{
			array[3 + i] = amount[i];
		}
		array[9] = transactionType;
		int num2 = cashBack.Length;
		if (num2 > 6)
		{
			num2 = 6;
		}
		for (int i = 0; i < num2; i++)
		{
			array[10 + i] = cashBack[i];
		}
		array[16] = currencyCode[0];
		array[17] = currencyCode[1];
		array[18] = reportingOption;
		return sendEMVCommand(MTEMVDeviceConstants.EMV_COMMAND_START_TRANSACTION, array);
	}

	public int setUserSelectionResult(byte status, byte selection)
	{
		return sendEMVCommand(data: new byte[2] { status, selection }, command: MTEMVDeviceConstants.EMV_COMMAND_SET_USER_SELECTION_RESULT);
	}

	public int setAcquirerResponse(byte[] response)
	{
		if (response != null && response.Length > 0)
		{
			return sendEMVCommand(MTEMVDeviceConstants.EMV_COMMAND_SET_ACQUIRER_RESPONSE, response);
		}
		return 9;
	}

	public int cancelTransaction()
	{
		return sendEMVCommand(MTEMVDeviceConstants.EMV_COMMAND_CANCEL_TRANSACTION, null);
	}
}
