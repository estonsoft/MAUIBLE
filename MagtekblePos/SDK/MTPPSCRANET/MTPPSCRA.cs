// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.MTPPSCRA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public class MTPPSCRA
{
	internal MTConnectionType m_connectionType;

	private MTConnectionState m_connectionState;

	private const int TIME_OUT = 120;

	private const int ASYNC_TIMEOUT = 2000;

	private const int SHORT_WAIT_IN_SECOND = 2;

	private MTPPReportParser m_ReportParser;

	//private MTPPSCRAWrapper m_PPSCRA;

	private Dictionary<int, string> m_DeviceInfo_Rep1A = new Dictionary<int, string>();

	private bool m_CanncelOperation;

	private IPADCapability DeviceCapability = new IPADCapability();

	private string _DeviceSerialNumber = "";

	public static bool GetDeviceInfo = true;

	private bool onDataReadyEveryThing = true;

	private SemaphoreSlim lockConnectionSequence = new SemaphoreSlim(1, 1);

	private Task userConnectionStateChangedTask;

	private static List<Tuple<MTConnectionType, MTDeviceInformation>> m_DeviceList = new List<Tuple<MTConnectionType, MTDeviceInformation>>();

	internal OnErrorEvent EventOnError => this.onError;

	internal OnUserDataEntryCompleteEvent EventOnUserDataEntry => this.onUserDataEntry;

	internal OnSignatureArriveCompleteEvent EventOnSignatureArrived => this.onSignatureArrived;

	internal OnProgressUpdateEvent EventOnProgressUpdated => this.onProgressUpdated;

	internal OnPowerUpICCCompleteEvent EventOnPowerUpICC => this.onPowerUpICC;

	internal OnPINRequestCompleteEvent EventPinRequest => this.onPinRequestComplete;

	internal OnCardHolderStateChangeCompleteEvent EventCardHolderStateChanged => this.onCardHolderStateChanged;

	internal OnPayPassMessageEvent EventPayPassKernelMessage => this.onPayPassKernelMessage;

	internal OnPayPassMessageTypeEvent EventPayPassKernelMessageType => this.onPayPassKernelMessageType;

	internal OnPerformTestAPDU EventPerformTestAPDU => this.onPerformTestAPDU;

	internal OnCardRequestCompleteEvent EventCardRequest => this.onCardRequestComplete;

	internal OnDataReadyCompleteEvent EventDataReady => this.onDataReady;

	internal OnDisplayRequestCompleteEvent EventDisplayRequest => this.onDisplayRequestComplete;

	internal OnKeyInputCompleteEvent EventKeyInput => this.onKeyInput;

	internal OnGetCAPublicKeyCompleteEvent EventCAPublicKey => this.onGetCAPublicKey;

	internal OnEMVTransactionCompleteEvent EventEMVTransactionCompleted => this.onEmvTransactionComplete;

	internal OnEMVTagsCompleteEvent EventEMVTagsCompleted => this.onEMVTagsComplete;

	internal OnEMVCompleteDataCompleteEvent EventEMVDataComplete => this.onEMVDataComplete;

	internal OnDeviceStateUpdateCompleteEvent EventDeviceStateUpdated => this.onDeviceStateUpdated;

	internal OnClearTextUserDataEntryCompleteEvent EventClearTextUserDataEntry => this.onClearTextUserDataEntry;

	internal OnAPDUArriveCompleteEvent EventAPDUArrived => this.onAPDUArrived;

	internal deviceMessageTipOrCashbackEvent EventDeviceMessageTipOrCashback => this.deviceMessageTipOrCashback;

	internal deviceMessageSelectedMenuItemEvent EventDeviceMessageSelectMenuItem => this.deviceMessageSelectedMenuItem;

	internal deviceMessageSessionStartedEvent EventDeviceMessageSessionStarted => this.deviceMessageSessionStarted;

	private byte OpStatus
	{
		get
		{
			return m_ReportParser.getStatusCode();
		}
		set
		{
			m_ReportParser.setOpStatus(value);
		}
	}

	public int status => getStatusCode();

	public CARD_DATA card => m_ReportParser.cardDataParser.lastCardData;

	public string productID
	{
		get
		{
			int opStatus = 0;
			return requestDeviceInformation(0, ref opStatus);
		}
	}

	public string deviceSerial
	{
		get
		{
			int opStatus = 0;
			return requestDeviceInformation(5, ref opStatus);
		}
	}

	public string deviceModel
	{
		get
		{
			int opStatus = 0;
			return requestDeviceInformation(4, ref opStatus);
		}
	}

	public string deviceFirmwareVersion
	{
		get
		{
			int opStatus = 0;
			return requestDeviceInformation(6, ref opStatus);
		}
	}

	public bool isDeviceConnected => m_connectionState != MTConnectionState.Disconnected;

	public PIN_DATA pin => m_ReportParser.pinDataParser.lastPinData;

	public event OnErrorEvent onError;

	public event OnDataReadyCompleteEvent onDataReady;

	public event OnPowerUpICCCompleteEvent onPowerUpICC;

	public event OnAPDUArriveCompleteEvent onAPDUArrived;

	public event OnGetCAPublicKeyCompleteEvent onGetCAPublicKey;

	public event OnEMVTagsCompleteEvent onEMVTagsComplete;

	public event OnPINRequestCompleteEvent onPinRequestComplete;

	public event OnKeyInputCompleteEvent onKeyInput;

	public event OnDisplayRequestCompleteEvent onDisplayRequestComplete;

	public event OnSignatureArriveCompleteEvent onSignatureArrived;

	public event OnCardRequestCompleteEvent onCardRequestComplete;

	public event OnUserDataEntryCompleteEvent onUserDataEntry;

	public event OnClearTextUserDataEntryCompleteEvent onClearTextUserDataEntry;

	public event OnDeviceStateUpdateCompleteEvent onDeviceStateUpdated;

	public event OnEMVCompleteDataCompleteEvent onEMVDataComplete;

	public event OnEMVTransactionCompleteEvent onEmvTransactionComplete;

	public event OnCardHolderStateChangeCompleteEvent onCardHolderStateChanged;

	public event OnProgressUpdateEvent onProgressUpdated;

	public event OnPayPassMessageEvent onPayPassKernelMessage;

	public event OnPayPassMessageTypeEvent onPayPassKernelMessageType;

	public event OnPerformTestAPDU onPerformTestAPDU;

	public event OnDeviceConnectionStateChanged onDeviceConnectionStateChanged;

	public event deviceMessageTipOrCashbackEvent deviceMessageTipOrCashback;

	public event deviceMessageSelectedMenuItemEvent deviceMessageSelectedMenuItem;

	public event deviceMessageSessionStartedEvent deviceMessageSessionStarted;

	public MTPPSCRA()
	{
		m_connectionState = MTConnectionState.Disconnected;
		m_ReportParser = new MTPPReportParser(this);
		m_PPSCRA = new MTPPSCRAWrapper();
		m_PPSCRA.OnDeviceConnectionStateChanged += OnDeviceConnectionStateChanged;
		m_PPSCRA.OnDeviceReport += OnDeviceResponse;
	}

	internal int sendSpecialCommand(byte[] data, bool stopTrackingOnDataReady = true)
	{
		try
		{
			onDataReadyEveryThing = !stopTrackingOnDataReady;
			if (!isDeviceConnected)
			{
				return 5;
			}
			m_PPSCRA.sendSpecialCommand(data);
		}
		catch
		{
			return 7;
		}
		return 0;
	}

	private byte[] doSpecialCommand(byte[] data, int timeout)
	{
		int iRet = 0;
		int millisecondsTimeout = timeout * 1000;
		if (iRet != 0)
		{
			return null;
		}
		byte[] outData = null;
		ManualResetEvent waitEvt = new ManualResetEvent(initialState: false);
		OnDataReadyCompleteEvent value = delegate(byte[] receive)
		{
			if (receive[0] == 1)
			{
				if (data[1] == receive[2] || (data[1] == 31 && data[2] == 25 && receive[1] == 135 && receive[2] == 9))
				{
					m_ReportParser.setAckStatus(receive[1]);
					outData = new byte[receive.Length];
					Array.Copy(receive, outData, outData.Length);
					waitEvt.Set();
				}
				else
				{
					m_ReportParser.setAckStatus(receive[1]);
					outData = new byte[receive.Length];
					Array.Copy(receive, outData, outData.Length);
					waitEvt.Set();
				}
			}
			else if (receive[0] == data[1])
			{
				outData = new byte[receive.Length];
				Array.Copy(receive, outData, outData.Length);
				waitEvt.Set();
			}
		};
		Task task = new Task(delegate
		{
			iRet = sendSpecialCommand(data);
		});
		m_ReportParser.onDataReady += value;
		OnDeviceConnectionStateChanged value2 = delegate
		{
			waitEvt.Set();
		};
		onDeviceConnectionStateChanged += value2;
		waitEvt.Reset();
		task.Start();
		if (!waitEvt.WaitOne(millisecondsTimeout))
		{
			iRet = 1;
		}
		m_ReportParser.onDataReady -= value;
		onDeviceConnectionStateChanged -= value2;
		return outData;
	}

	internal int doCommand(byte[] data, int timeout = 120)
	{
		try
		{
			if (!isDeviceConnected)
			{
				return 5;
			}
			byte[] array = doSpecialCommand(data, timeout);
			if (array == null)
			{
				return 1;
			}
			if (array.Length > 2 && array[2] != data[1])
			{
				return 7;
			}
			switch (array[1])
			{
			case 0:
				return 0;
			case 16:
				return 0;
			default:
				if (getStatusCode() == 134)
				{
					return 15;
				}
				return 0;
			}
		}
		catch
		{
			return 1;
		}
	}

	private byte[] setGetSpecialCommandSync(byte[] command)
	{
		byte b = command[0];
		command[0] = MTPPSCRAConstants.OPERATION_SET;
		int num = doCommand(command);
		byte[] result = null;
		if (num == 0)
		{
			command[0] = MTPPSCRAConstants.OPERATION_GET;
			result = doSpecialCommand(command, 120);
		}
		command[0] = b;
		return result;
	}

	private async Task<byte[]> setGetSpecialCommand(byte[] command, int TimeOutValue = 120)
	{
		return await Task.Run(delegate
		{
			byte b = command[0];
			command[0] = MTPPSCRAConstants.OPERATION_SET;
			int num = doCommand(command, TimeOutValue);
			byte[] result = null;
			if (num == 0)
			{
				command[0] = MTPPSCRAConstants.OPERATION_GET;
				result = doSpecialCommand(command, TimeOutValue);
			}
			command[0] = b;
			return result;
		});
	}

	private async Task<int> doCommandTask(byte[] data, int timeout)
	{
		return await Task.Run(() => doCommand(data, timeout));
	}

	public string getSDKVersion()
	{
		return typeof(MTPPSCRA).GetTypeInfo().Assembly.GetName().Version.ToString();
	}

	private async Task<List<Tuple<MTConnectionType, MTDeviceInformation>>> findDeviceSync(int timeOut)
	{
		TaskCompletionSource<bool> tcs = null;
		List<Tuple<MTConnectionType, MTDeviceInformation>> allDevices = new List<Tuple<MTConnectionType, MTDeviceInformation>>();
		Dictionary<MTConnectionType, string> dictionary = new Dictionary<MTConnectionType, string>
		{
			{
				MTConnectionType.USB,
				MTPPSCRAWrapper.StringFromConnectionType(MTConnectionType.USB)
			},
			{
				MTConnectionType.BLEEMV,
				MTPPSCRAWrapper.StringFromConnectionType(MTConnectionType.BLEEMV)
			}
		};
		MTPPSCRAWrapper.DeviceListHandler tmp = delegate(object sender, MTConnectionType connectionType, List<MTDeviceInformation> deviceList)
		{
			foreach (MTDeviceInformation device in deviceList)
			{
				allDevices.Add(new Tuple<MTConnectionType, MTDeviceInformation>(connectionType, device));
			}
			try
			{
				if (tcs != null)
				{
					tcs.SetResult(result: true);
				}
			}
			catch
			{
			}
		};
		m_PPSCRA.OnDeviceList += tmp;
		foreach (KeyValuePair<MTConnectionType, string> item in dictionary)
		{
			tcs = new TaskCompletionSource<bool>();
			m_PPSCRA.requestDeviceList(item.Key);
			await tcs.Task;
		}
		m_PPSCRA.OnDeviceList -= tmp;
		return allDevices;
	}

	internal async Task GetDeviceCapabilityInfo()
	{
		IPADCapability deviceCapability = DeviceCapability;
		deviceCapability.Capability = await requestDeviceInformationAsync(2, null);
		MTPPSCRA mTPPSCRA = this;
		_ = mTPPSCRA._DeviceSerialNumber;
		mTPPSCRA._DeviceSerialNumber = await requestDeviceInformationAsync(5, null);
	}

	protected async void OnDeviceConnectionStateChanged(object sender, MTConnectionState state)
	{
		await lockConnectionSequence.WaitAsync();
		m_connectionState = state;
		if (state == MTConnectionState.Connected)
		{
			try
			{
				if (GetDeviceInfo)
				{
					int retry = 60;
					while (retry-- > 0)
					{
						byte[] array = doSpecialCommand(new byte[3] { 1, 2, 0 }, 2);
						if (array != null && array[0] == 1 && array[1] == 0 && array[2] == 2)
						{
							break;
						}
						if (isDeviceOpened())
						{
							retry = 0;
							break;
						}
						await Task.Delay(1000);
					}
					if (retry > 0)
					{
						await GetDeviceCapabilityInfo();
					}
				}
			}
			catch (Exception)
			{
			}
		}
		else if (state == MTConnectionState.Disconnected)
		{
			m_DeviceInfo_Rep1A.Clear();
			if (this.onError != null)
			{
				this.onError(4);
			}
		}
		if (this.onDeviceConnectionStateChanged != null)
		{
			if (userConnectionStateChangedTask == null || userConnectionStateChangedTask.IsCompleted)
			{
				userConnectionStateChangedTask = Task.Run(delegate
				{
					this.onDeviceConnectionStateChanged(state);
				});
			}
			else
			{
				userConnectionStateChangedTask = userConnectionStateChangedTask.ContinueWith(delegate
				{
					this.onDeviceConnectionStateChanged(state);
				});
			}
		}
		lockConnectionSequence.Release();
	}

	protected void OnDeviceResponse(object sender, byte[] data)
	{
		if (data[0] == 41)
		{
			m_ReportParser.parseReport(sender, data);
		}
		else
		{
			Task.Run(delegate
			{
				m_ReportParser.parseReport(sender, data);
			});
		}
		try
		{
			if (onDataReadyEveryThing)
			{
				Task.Run(delegate
				{
					this.onDataReady?.Invoke(data);
				});
			}
		}
		catch
		{
		}
	}

	public string getDeviceList()
	{
		string list = "";
		Task.Run(delegate
		{
			getDeviceListAsync().ContinueWith(delegate(Task<string> result)
			{
				if (result.IsCompleted)
				{
					list = result.Result;
				}
			}).Wait();
		}).Wait();
		return list;
	}

	private async Task<string> getDeviceListAsync()
	{
		lock (m_DeviceList)
		{
			m_DeviceList.Clear();
		}
		List<Tuple<MTConnectionType, MTDeviceInformation>> list = await findDeviceSync(5);
		StringBuilder stringBuilder = new StringBuilder();
		if (list != null)
		{
			foreach (Tuple<MTConnectionType, MTDeviceInformation> item in list)
			{
				string text = MTPPSCRAWrapper.StringFromConnectionType(item.Item1) + item.Item2.Name;
				stringBuilder.Append(text + ",");
				m_DeviceList.Add(item);
			}
		}
		return stringBuilder.ToString().Trim(',');
	}

	private bool openMatchDevice(string deviceURI, List<Tuple<MTConnectionType, MTDeviceInformation>> m_DeviceList)
	{
		foreach (Tuple<MTConnectionType, MTDeviceInformation> m_Device in m_DeviceList)
		{
			if (string.Compare(MTPPSCRAWrapper.StringFromConnectionType(m_Device.Item1) + m_Device.Item2.Name, deviceURI, ignoreCase: true) == 0)
			{
				m_connectionType = m_Device.Item1;
				m_PPSCRA.setConnectionType(m_Device.Item1);
				m_PPSCRA.setAddress(m_Device.Item2.Address);
				if (string.IsNullOrWhiteSpace(m_Device.Item2.Id))
				{
					m_PPSCRA.setDeviceID(m_Device.Item2.Name);
				}
				else
				{
					m_PPSCRA.setDeviceID(m_Device.Item2.Id);
				}
				m_PPSCRA.openDevice();
				return true;
			}
		}
		return false;
	}

	private int openDeviceSync(string deviceURI = "")
	{
		m_DeviceInfo_Rep1A.Clear();
		int result = 0;
		string text = (string.IsNullOrEmpty(deviceURI) ? "USB://" : deviceURI.ToUpper());
		if (text.StartsWith("IP://") && text.Contains(";TLS=1.2"))
		{
			m_connectionType = MTConnectionType.Net_TLS12;
			m_PPSCRA.setConnectionType(MTConnectionType.Net_TLS12);
			m_PPSCRA.setAddress(text.Replace("IP://", "TLS12://").Replace(";TLS=1.2", ""));
			m_PPSCRA.openDevice();
		}
		else if (text.StartsWith("IP://"))
		{
			m_connectionType = MTConnectionType.Net;
			m_PPSCRA.setConnectionType(MTConnectionType.Net);
			m_PPSCRA.setAddress(deviceURI);
			m_PPSCRA.openDevice();
		}
		else if (text.StartsWith("TLS12://"))
		{
			m_connectionType = MTConnectionType.Net_TLS12;
			m_PPSCRA.setConnectionType(MTConnectionType.Net_TLS12);
			m_PPSCRA.setAddress(deviceURI);
			m_PPSCRA.openDevice();
		}
		else if (text.StartsWith("TLS12TRUST://"))
		{
			m_connectionType = MTConnectionType.Net_TLS12_Trust_All;
			m_PPSCRA.setConnectionType(MTConnectionType.Net_TLS12_Trust_All);
			m_PPSCRA.setAddress(deviceURI);
			m_PPSCRA.openDevice();
		}
		else if (text.StartsWith("USB://") || text.StartsWith("BLEEMV://"))
		{
			if (!openMatchDevice(deviceURI, m_DeviceList))
			{
				m_connectionType = MTConnectionType.USB;
				m_PPSCRA.setConnectionType(MTConnectionType.USB);
				m_PPSCRA.setAddress("");
				m_PPSCRA.openDevice();
			}
		}
		else
		{
			result = 5;
		}
		return result;
	}

	public int openDevice(string deviceURI)
	{
		TaskCompletionSource<bool> success = new TaskCompletionSource<bool>();
		OnDeviceConnectionStateChanged value = delegate(MTConnectionState s)
		{
			if (s == MTConnectionState.Connected)
			{
				success.TrySetResult(result: true);
			}
			if (s == MTConnectionState.Disconnected)
			{
				success.TrySetResult(result: false);
			}
		};
		onDeviceConnectionStateChanged += value;
		Task.Run(() => openDeviceSync(deviceURI)).Wait();
		success.Task.Wait();
		onDeviceConnectionStateChanged -= value;
		if (!success.Task.Result)
		{
			return 5;
		}
		return 0;
	}

	private async Task<int> openDeviceAsyncInternal(string deviceURI)
	{
		try
		{
			if (isDeviceConnected)
			{
				return 0;
			}
			ManualResetEvent waitEvt = new ManualResetEvent(initialState: false);
			waitEvt.Reset();
			MTPPSCRAWrapper.DeviceConnectionStateHandler tmp = delegate(object sender, MTConnectionState state)
			{
				if (state == MTConnectionState.Connected || state == MTConnectionState.Disconnected)
				{
					waitEvt.Set();
				}
			};
			m_PPSCRA.OnDeviceConnectionStateChanged += tmp;
			await Task.Run(() => openDeviceSync(deviceURI));
			int result = ((!waitEvt.WaitOne(120000)) ? 5 : 0);
			m_PPSCRA.OnDeviceConnectionStateChanged -= tmp;
			if (m_connectionState != MTConnectionState.Connecting && m_connectionState != MTConnectionState.Connected)
			{
				result = 5;
			}
			return result;
		}
		catch
		{
			return 255;
		}
	}

	public int closeDevice()
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		try
		{
			m_DeviceInfo_Rep1A.Clear();
			m_PPSCRA.closeDevice();
		}
		catch (Exception)
		{
			return 255;
		}
		return 0;
	}

	public bool isDeviceOpened()
	{
		try
		{
			return isDeviceConnected;
		}
		catch
		{
			return false;
		}
	}

	public int deviceReset()
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		try
		{
			byte[] data = new byte[4]
			{
				MTPPSCRAConstants.OPERATION_SET,
				255,
				0,
				0
			};
			int result = sendSpecialCommand(data);
			closeDevice();
			return result;
		}
		catch
		{
			return 255;
		}
	}

	public int getStatusCode()
	{
		return OpStatus;
	}

	private int getAckStatus()
	{
		return m_ReportParser.getAckStatus();
	}

	public int cancelOperation()
	{
		try
		{
			if (!isDeviceConnected)
			{
				return 5;
			}
			m_CanncelOperation = true;
			byte[] data = new byte[3]
			{
				MTPPSCRAConstants.OPERATION_SET,
				MTPPSCRAConstants.COMMAND_CANCEL_COMMAND,
				0
			};
			return doCommand(data);
		}
		catch
		{
			return 255;
		}
	}

	public int requestBypassPINCommand()
	{
		if (!isDeviceConnected)
		{
			return 5;
		}
		if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L2)
		{
			return 7;
		}
		byte[] data = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			MTPPSCRAConstants.EMV_COMMAND_MERCHANT_BYPASS_PIN,
			0
		};
		return doCommand(data);
	}

	public int setPAN(string PAN)
	{
		if (!isDeviceConnected)
		{
			return 5;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(PAN);
		byte b = (byte)bytes.Length;
		byte[] array = new byte[25]
		{
			MTPPSCRAConstants.OPERATION_SET,
			13,
			1,
			b,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};
		if (bytes.Length > 19 || bytes.Length < 8)
		{
			return 6;
		}
		Array.Copy(bytes, 0, array, 4, bytes.Length);
		return doCommand(array);
	}

	internal async Task<int> setPANAsync(string PAN)
	{
		if (!isDeviceConnected)
		{
			return 5;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(PAN);
		byte b = (byte)bytes.Length;
		byte[] array = new byte[25]
		{
			MTPPSCRAConstants.OPERATION_SET,
			13,
			1,
			b,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};
		Array.Copy(bytes, 0, array, 4, bytes.Length);
		return await doCommandTask(array, 120);
	}

	private int setAmountSetup(byte amountType, string amount, out byte[] cmdSendAmount)
	{
		cmdSendAmount = null;
		if (!isDeviceConnected)
		{
			return 5;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(amount);
		byte b = (byte)bytes.Length;
		cmdSendAmount = new byte[16]
		{
			MTPPSCRAConstants.OPERATION_SET,
			13,
			0,
			b,
			amountType,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};
		Array.Copy(bytes, 0, cmdSendAmount, 5, bytes.Length);
		return 0;
	}

	public int setAmount(byte amountType, string amount, ref int status)
	{
		int num = setAmountSetup(amountType, amount, out var cmdSendAmount);
		if (num != 0)
		{
			return num;
		}
		num = doCommand(cmdSendAmount, 2);
		status = getStatusCode();
		return num;
	}

	public int endSession(int displayMessage = 0)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		byte[] data = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			MTPPSCRAConstants.COMMAND_END_SESSION,
			(byte)displayMessage
		};
		return doCommand(data);
	}

	public byte[] requestChallengeAndSessionKey()
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return null;
		}
		if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L1)
		{
			return null;
		}
		byte[] data = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			169,
			12
		};
		if (doCommand(data) == 0 && getAckStatus() == 0)
		{
			return doSpecialCommand(new byte[2]
			{
				MTPPSCRAConstants.OPERATION_GET,
				169
			}, 120);
		}
		return null;
	}

	public byte[] requestChallengeAndSessionForInformation()
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return null;
		}
		return requestChallengeAndSessionKey();
	}

	public int requestConfirmSession(int mode, byte[] encryptedRandomNumber, byte[] encryptedSerialNumber, byte[] cmac, ref int status)
	{
		if (!isDeviceConnected)
		{
			return 5;
		}
		if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L1)
		{
			return 7;
		}
		byte[] array = new byte[19]
		{
			MTPPSCRAConstants.OPERATION_SET,
			170,
			(byte)mode,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};
		if (encryptedRandomNumber != null && encryptedSerialNumber != null && cmac != null)
		{
			Array.Copy(encryptedRandomNumber, 0, array, 3, 4);
			Array.Copy(encryptedSerialNumber, 0, array, 7, 4);
			Array.Copy(cmac, 0, array, 11, 8);
		}
		int result = doCommand(array, 2);
		status = getStatusCode();
		return result;
	}

	public int endL1Session(ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		return requestConfirmSession(0, null, null, null, ref opStatus);
	}

	public int requestPowerUpResetICC(byte waitTime, byte operation)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L1)
		{
			return 7;
		}
		return doCommand(new byte[4]
		{
			MTPPSCRAConstants.OPERATION_SET,
			166,
			waitTime,
			operation
		});
	}

	public int requestPowerDownICC(byte waitTime)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L1)
		{
			return 7;
		}
		return doCommand(new byte[4]
		{
			MTPPSCRAConstants.OPERATION_SET,
			166,
			waitTime,
			2
		});
	}

	public int requestICCAPDU(byte[] apdu, int apduLen)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L1)
		{
			return 7;
		}
		int[] status = new int[1];
		sendBigBlockDataAsync(167, apdu, status).ContinueWith(delegate(Task<int> t)
		{
			if (t.IsCompleted && status[0] == 0)
			{
				byte[] data = new byte[18]
				{
					MTPPSCRAConstants.OPERATION_SET,
					167,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0
				};
				sendSpecialCommand(data);
			}
		});
		return 0;
	}

	public int requestICCAPDUForInformation(byte[] apdu, int apdulen)
	{
		return requestICCAPDU(apdu, apdulen);
	}

	public int sendSpecialCommand(string command)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		byte[] array = PARSER.toByteArray("01" + command);
		if (array.Length < 2)
		{
			return 6;
		}
		m_ReportParser._processSpecialCommand = array[1];
		return sendSpecialCommand(array, stopTrackingOnDataReady: false);
	}

	public byte[] getSpecialCommand(string command)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return null;
		}
		if (command.Equals(""))
		{
			return new byte[0];
		}
		byte[] array = PARSER.toByteArray("00" + command);
		if (array.Length < 2)
		{
			return new byte[0];
		}
		return doSpecialCommand(array, 2);
	}

	private async Task<byte[]> getSpecialCommandAsync(string command)
	{
		return await Task.Run(() => doSpecialCommand(PARSER.toByteArray("00" + command), 120));
	}

	private int requestEMVTagsSync(int tagType, int tagOperation, byte[] inputTLVData, int inputDataLength, byte database, byte transactionType, byte[] reserved, bool waitForComplete)
	{
		int num = 0;
		int opStatus = 0;
		switch (tagOperation)
		{
		case 0:
			tagOperation = 0;
			break;
		case 1:
			tagOperation = 15;
			inputDataLength = 0;
			break;
		case 2:
			tagOperation = 0;
			break;
		case 3:
			tagOperation = 15;
			inputDataLength = 0;
			break;
		case 4:
			tagOperation = 1;
			break;
		case 5:
			tagOperation = 1;
			break;
		case 6:
			tagOperation = 0;
			break;
		case 7:
			tagOperation = 1;
			break;
		case 8:
			tagOperation = 15;
			break;
		}
		if (transactionType == 1)
		{
			transactionType = ((DeviceCapability.DelayResponse == IPADCapabilityValue.L1) ? ((byte)1) : ((byte)0));
		}
		byte[] array = new byte[10]
		{
			MTPPSCRAConstants.OPERATION_SET,
			161,
			(byte)tagType,
			(byte)tagOperation,
			database,
			transactionType,
			0,
			0,
			0,
			0
		};
		try
		{
			if (reserved != null)
			{
				Array.Copy(reserved, 0, array, 5, 4);
			}
		}
		catch
		{
		}
		if (inputTLVData != null && inputDataLength > 0)
		{
			num = sendBigBlockData(161, inputTLVData, ref opStatus);
		}
		if (num == 0)
		{
			if (waitForComplete)
			{
				AutoResetEvent drArrived = new AutoResetEvent(initialState: false);
				OnProgressUpdateEvent value = delegate
				{
					drArrived.Set();
				};
				onProgressUpdated += value;
				try
				{
					num = doCommand(array);
					if (getAckStatus() == 16)
					{
						drArrived.WaitOne(120000);
					}
				}
				catch (Exception)
				{
					num = 255;
				}
				finally
				{
					onProgressUpdated -= value;
				}
			}
			else
			{
				num = sendSpecialCommand(array);
			}
		}
		return num;
	}

	public int requestSetEMVTags(int tagType, int tagOperation, byte[] inputTLVData, int inputDataLength, byte database, byte option, byte[] reserved)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		return requestEMVTagsSync(tagType, tagOperation, inputTLVData, inputDataLength, database, option, reserved, waitForComplete: true);
	}

	public int requestGetEMVTags(int tagType, int tagOperation, byte[] inputTLVData, int inputDataLength, byte database, byte option, byte[] reserved)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		return requestEMVTagsSync(tagType, tagOperation, inputTLVData, inputDataLength, database, option, reserved, waitForComplete: true);
	}

	public int setCAPublicKey(int operation, byte[] keyBlock, int keyBlockLen)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L2)
		{
			return 7;
		}
		byte[] cmdSetCaPublicKey = new byte[10]
		{
			MTPPSCRAConstants.OPERATION_SET,
			165,
			(byte)operation,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};
		if (keyBlock != null && keyBlock.Length != 0 && keyBlockLen > 0)
		{
			int[] Status = new int[1];
			int retCode = 0;
			Task.Run(async () => await sendBigBlockDataAsync(165, keyBlock, Status)).ContinueWith(delegate(Task<int> t)
			{
				retCode = t.Result;
				if (t.IsCompleted && retCode == 0 && Status[0] == 0)
				{
					retCode = doCommand(cmdSetCaPublicKey);
				}
			}).Wait(120000);
			return retCode;
		}
		return doCommand(cmdSetCaPublicKey);
	}

	public int setDisplayMessage(int waitTime, int messageID, ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		_ = 255;
		byte[] data = new byte[4]
		{
			MTPPSCRAConstants.OPERATION_SET,
			7,
			(byte)waitTime,
			(byte)messageID
		};
		int result = doCommand(data);
		opStatus = getStatusCode();
		return result;
	}

	public int sendBigBlockData(int dataTypeID, byte[] data, ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		int result = sendBigBlockDataSync(dataTypeID, data, data.Length, (data.Length > 65535) ? 1 : 0);
		opStatus = getStatusCode();
		return result;
	}

	internal async Task<int> sendBigBlockDataAsync(int dataTypeID, byte[] data, int[] opStatus)
	{
		return await sendBigBlockDataAsync(dataTypeID, data, data.Length, (data.Length > 65535) ? 1 : 0);
	}

	private int sendStartBlockData(int dataTypeID, int length, int deviceType, bool waitForComplete)
	{
		int num = 255;
		if (deviceType == 0 && length > 65536)
		{
			return 7;
		}
		if (deviceType > 1)
		{
			return 7;
		}
		byte[] data = new byte[9]
		{
			MTPPSCRAConstants.OPERATION_SET,
			MTPPSCRAConstants.COMMAND_SEND_BIG_BLOCK_DATA_TO_DEVICE,
			(byte)dataTypeID,
			0,
			(byte)length,
			(byte)(length >> 8),
			(byte)(length >> 16),
			(byte)(length >> 24),
			(byte)deviceType
		};
		if (waitForComplete)
		{
			return doCommand(data);
		}
		return sendSpecialCommand(data);
	}

	private int sendOneBlockData(int dataTypeID, int packetId, byte[] packageData, int packetLength, bool waitForComplete)
	{
		int num = 255;
		byte[] array = new byte[5]
		{
			MTPPSCRAConstants.OPERATION_SET,
			MTPPSCRAConstants.COMMAND_SEND_BIG_BLOCK_DATA_TO_DEVICE,
			(byte)dataTypeID,
			(byte)packetId,
			(byte)packetLength
		};
		byte[] array2 = new byte[65];
		Array.Copy(array, 0, array2, 0, array.Length);
		if (packageData == null || packetLength > 60 || packetLength <= 0 || packetId < 1 || packetId > 255)
		{
			return 6;
		}
		Array.Copy(packageData, 0, array2, 5, packageData.Length);
		if (waitForComplete)
		{
			return doCommand(array2);
		}
		return sendSpecialCommand(array2);
	}

	private int sendBigBlockDataSync(int dataTypeID, byte[] data, int dataLength, int deviceType)
	{
		int num = 255;
		if (data == null || dataLength == 0)
		{
			return 6;
		}
		m_CanncelOperation = false;
		int num2 = 0;
		num = sendStartBlockData(dataTypeID, dataLength, deviceType, waitForComplete: true);
		if (num == 0 && getAckStatus() == 0)
		{
			long num3 = (dataLength + 59) / 60;
			long num4 = num3;
			int num5 = 1;
			do
			{
				if (num5 > 255)
				{
					num5 = 1;
				}
				int num6 = ((num3 > 1) ? 60 : (dataLength % 60));
				if (num6 == 0)
				{
					num6 = 60;
				}
				byte[] array = new byte[num6];
				Array.Copy(data, num2, array, 0, array.Length);
				num = sendOneBlockData(dataTypeID, num5++, array, array.Length, waitForComplete: true);
				if (dataTypeID == 12 || dataTypeID == 23)
				{
					try
					{
						if (this.onProgressUpdated != null)
						{
							this.onProgressUpdated((byte)getStatusCode(), dataTypeID, ((double)num4 - (double)num3 + 1.0) / (double)num4);
						}
					}
					catch
					{
					}
				}
				num2 += 60;
			}
			while (--num3 > 0 && num == 0 && getAckStatus() == 0);
		}
		if (num != 0 && m_CanncelOperation)
		{
			num = 2;
		}
		return num;
	}

	private async Task<int> sendBigBlockDataAsync(int dataTypeID, byte[] data, int dataLength, int deviceType)
	{
		int iRet = 255;
		if (data == null || dataLength == 0)
		{
			return 6;
		}
		m_CanncelOperation = false;
		try
		{
			await Task.Run(delegate
			{
				int num = 0;
				iRet = sendStartBlockData(dataTypeID, dataLength, deviceType, waitForComplete: true);
				if (iRet == 0 && getAckStatus() == 0)
				{
					long num2 = (dataLength + 59) / 60;
					long num3 = num2;
					int num4 = 1;
					do
					{
						if (num4 > 255)
						{
							num4 = 1;
						}
						int num5 = ((num2 > 1) ? 60 : (dataLength % 60));
						if (num5 == 0)
						{
							num5 = 60;
						}
						byte[] array = new byte[num5];
						Array.Copy(data, num, array, 0, array.Length);
						iRet = sendOneBlockData(dataTypeID, num4++, array, array.Length, waitForComplete: true);
						if (dataTypeID == 12 || dataTypeID == 23)
						{
							try
							{
								if (this.onProgressUpdated != null)
								{
									this.onProgressUpdated((byte)getStatusCode(), dataTypeID, ((double)num3 - (double)num2 + 1.0) / (double)num3);
								}
							}
							catch
							{
							}
						}
						num += 60;
					}
					while (--num2 > 0 && iRet == 0 && getAckStatus() == 0);
				}
			});
		}
		catch
		{
		}
		if (iRet != 0 && m_CanncelOperation)
		{
			iRet = 2;
		}
		return iRet;
	}

	public int sendBitmap(int slot, int option, byte[] bitmapData, ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		int num = 0;
		int opStatus2 = 0;
		if (bitmapData != null && bitmapData.Length != 0)
		{
			_ = bitmapData.Length;
			num = sendBigBlockData(MTPPSCRAConstants.COMMAND_SET_BITMAP, bitmapData, ref opStatus2);
		}
		byte[] cmdBitmap = new byte[4]
		{
			MTPPSCRAConstants.OPERATION_SET,
			MTPPSCRAConstants.COMMAND_SET_BITMAP,
			(byte)(slot - 1),
			(byte)option
		};
		if (num == 0 && getAckStatus() == 0)
		{
			TaskCompletionSource<int> complete = new TaskCompletionSource<int>();
			OnProgressUpdateEvent value = delegate(byte opSts, int item, double progress)
			{
				if (opSts != 0)
				{
					complete.SetResult(opSts);
				}
				if (progress == 2.0)
				{
					complete.SetResult(0);
				}
			};
			onProgressUpdated += value;
			Task.Run(delegate
			{
				int num2 = doCommand(cmdBitmap);
				if (num2 != 0)
				{
					complete.SetResult(num2);
				}
				if (getAckStatus() != 16)
				{
					complete.SetResult(getAckStatus());
				}
			});
			complete.Task.Wait(120000);
			onProgressUpdated -= value;
			num = ((!complete.Task.IsCompleted) ? 255 : complete.Task.Result);
		}
		opStatus = getAckStatus();
		return num;
	}

	public IPADDevInfo getIPADInfoData()
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return default(IPADDevInfo);
		}
		int opStatus = 0;
		IPADDevInfo result = default(IPADDevInfo);
		result.Model = requestDeviceInformation(4, ref opStatus);
		result.Serial = requestDeviceInformation(5, ref opStatus);
		result.FWVersion = requestDeviceInformation(6, ref opStatus);
		string text = requestDeviceInformation(0, ref opStatus);
		result.PID = Convert.ToInt32(text.Equals("") ? "0" : text, 16);
		result.VID = 2049;
		result.Version = 0;
		return result;
	}

	private async Task<string> getDeviceInformation(int mode, int[] opStatus)
	{
		if (!m_DeviceInfo_Rep1A.ContainsKey(mode))
		{
			byte[] command = new byte[3]
			{
				MTPPSCRAConstants.OPERATION_SET,
				MTPPSCRAConstants.COMMAND_REQUEST_DEVICE_INFORMATION,
				(byte)mode
			};
			byte[] array = await setGetSpecialCommand(command, 6);
			if (array != null)
			{
				if (opStatus != null)
				{
					opStatus[0] = getAckStatus();
				}
				string value = Encoding.UTF8.GetString(array, 2, array.Length - 2).TrimEnd('\0', ' ');
				m_DeviceInfo_Rep1A.Add(mode, value);
			}
		}
		if (!m_DeviceInfo_Rep1A.ContainsKey(mode))
		{
			return "";
		}
		return m_DeviceInfo_Rep1A[mode];
	}

	public string requestDeviceInformation(int mode, ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return "";
		}
		if (!m_DeviceInfo_Rep1A.ContainsKey(mode))
		{
			byte[] getSpecialCommandSync = new byte[3]
			{
				MTPPSCRAConstants.OPERATION_SET,
				MTPPSCRAConstants.COMMAND_REQUEST_DEVICE_INFORMATION,
				(byte)mode
			};
			byte[] array = setGetSpecialCommandSync(getSpecialCommandSync);
			if (array != null)
			{
				opStatus = getAckStatus();
				string value = Encoding.UTF8.GetString(array, 2, array.Length - 2).TrimEnd('\0', ' ');
				m_DeviceInfo_Rep1A.Add(mode, value);
			}
		}
		if (!m_DeviceInfo_Rep1A.ContainsKey(mode))
		{
			return "";
		}
		return m_DeviceInfo_Rep1A[mode];
	}

	private async Task<string> requestDeviceInformationAsync(int mode, int[] opStatus)
	{
		return await getDeviceInformation(mode, opStatus);
	}

	public DEV_STATE_STAT requestDeviceStatus(ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return default(DEV_STATE_STAT);
		}
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		byte[] data = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			8,
			0
		};
		DEV_STATE_STAT ds = default(DEV_STATE_STAT);
		OnDeviceStateUpdateCompleteEvent value = delegate(DEV_STATE_STAT status)
		{
			ds = status;
			tcs.SetResult(result: true);
		};
		onDeviceStateUpdated += value;
		doCommand(data);
		tcs.Task.Wait(2000);
		onDeviceStateUpdated -= value;
		if (tcs.Task.IsCompleted)
		{
			opStatus = getStatusCode();
			return ds;
		}
		opStatus = 1;
		return ds;
	}

	public int requestKernelInformation(int kernelId, byte[] kernelInfoBuffer)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		ERROR result = ERROR.ERROR_NO_ERR;
		byte[] getSpecialCommandSync = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			168,
			(byte)kernelId
		};
		byte[] array = setGetSpecialCommandSync(getSpecialCommandSync);
		if (array != null)
		{
			if (kernelInfoBuffer.Length < 3 + array[2])
			{
				result = ERROR.ERROR_BUFFER_INSUFFICIENT;
			}
			else
			{
				Array.Copy(array, kernelInfoBuffer, 3 + array[2]);
			}
		}
		else
		{
			result = ERROR.ERROR_DEVICE_TIMEOUT;
		}
		return (int)result;
	}

	public byte[] getBINTableData()
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return null;
		}
		byte[] array = doSpecialCommand(new byte[2]
		{
			MTPPSCRAConstants.OPERATION_GET,
			50
		}, 2);
		if (array[0] == 50)
		{
			int num = array[1];
			if (array.Length > num + 2)
			{
				Array.Resize(ref array, num + 2);
			}
			return array;
		}
		return null;
	}

	public int setBINTableData(byte[] binTable, byte reserved, ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		int num = 0;
		int num2 = sendBigBlockDataSync(50, binTable, binTable.Length, 0);
		if (num2 == 0 && num == 0)
		{
			num2 = doCommand(new byte[3] { 1, 50, 0 }, 2);
		}
		opStatus = getAckStatus();
		return num2;
	}

	public string getKSN()
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return "";
		}
		byte b = 0;
		byte[] getSpecialCommandSync = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			MTPPSCRAConstants.COMMAND_SET_GET_KSN,
			b
		};
		byte[] array = setGetSpecialCommandSync(getSpecialCommandSync);
		if (array != null)
		{
			byte[] array2 = new byte[10];
			Array.Copy(array, 2, array2, 0, 10);
			return BitConverter.ToString(array2).Replace("-", "");
		}
		return "";
	}

	public int setKSNEncryptedData(int waitTime, byte[] data)
	{
		bool flag = true;
		int num = waitTime;
		if (waitTime > 255 || waitTime <= 0)
		{
			num = 0;
		}
		byte[] data2 = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			49,
			(byte)num
		};
		int num2 = 0;
		_ = new int[1];
		if (data != null && data.Length != 0)
		{
			num2 = sendBigBlockDataSync(49, data, data.Length, 0);
		}
		if (num2 == 0 && flag)
		{
			num2 = doCommand(data2, 120 + num);
		}
		return num2;
	}

	public int requestCard(int waitTime, int displayMessage, int beepTones, string fieldSeparator)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		m_ReportParser.CardRequestFieldSeparator = fieldSeparator;
		int num = ((waitTime <= 255 && waitTime > 0) ? waitTime : 0);
		byte[] data = new byte[5]
		{
			MTPPSCRAConstants.OPERATION_SET,
			3,
			(byte)num,
			(byte)displayMessage,
			(byte)beepTones
		};
		return doCommand(data);
	}

	public int requestManualCardData(int waitTime, int beepTones, int options, ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		byte b = (byte)((waitTime <= 255 && waitTime > 0) ? ((uint)waitTime) : 0u);
		byte[] data = new byte[5]
		{
			MTPPSCRAConstants.OPERATION_SET,
			MTPPSCRAConstants.COMMAND_REQUEST_MANUAL_CARD_ENTRY,
			b,
			(byte)options,
			(byte)beepTones
		};
		int result = doCommand(data, 120 + b);
		opStatus = getStatusCode();
		return result;
	}

	public int requestUserDataEntry(int waitTime, int displayMessage, int beepTones, ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		if (GetDeviceInfo && DeviceCapability.UserDataEntry != IPADCapabilityValue.L1 && DeviceCapability.UserDataEntry != IPADCapabilityValue.L2)
		{
			return 7;
		}
		byte b = (byte)((waitTime <= 255 && waitTime > 0) ? ((uint)waitTime) : 0u);
		byte[] data = new byte[5]
		{
			MTPPSCRAConstants.OPERATION_SET,
			20,
			b,
			(byte)displayMessage,
			(byte)beepTones
		};
		int result = doCommand(data);
		opStatus = getStatusCode();
		return result;
	}

	public int requestClearTextUserDataEntry(int waitTime, int mode, int beepTones)
	{
		if (GetDeviceInfo && DeviceCapability.UserDataEntry != IPADCapabilityValue.L2)
		{
			return 7;
		}
		OpStatus = 0;
		int num = (byte)((waitTime <= 255 && waitTime > 0) ? waitTime : 0);
		byte[] data = new byte[5]
		{
			MTPPSCRAConstants.OPERATION_SET,
			31,
			(byte)num,
			(byte)mode,
			(byte)beepTones
		};
		return doCommand(data, num + 10);
	}

	public int requestResponse(int waitTime, int selectMsg, int keyMask, int beepTones)
	{
		OpStatus = 0;
		byte b = (byte)((waitTime <= 255 && waitTime > 0) ? ((uint)waitTime) : 0u);
		if (waitTime == -1)
		{
			beepTones += 100;
		}
		byte[] data = new byte[6]
		{
			MTPPSCRAConstants.OPERATION_SET,
			6,
			b,
			(byte)selectMsg,
			(byte)keyMask,
			(byte)beepTones
		};
		return doCommand(data, 120 + b);
	}

	public int confirmAmount(int waitTime, int beepTones)
	{
		return requestResponse(waitTime, 1, 6, beepTones);
	}

	public int selectCreditDebit(int waitTime, int beepTones)
	{
		return requestResponse(waitTime, 0, 5, beepTones);
	}

	public int requestPIN(int waitTime, int pinMode, int minPinLength, int maxPinLength, int beepTones, int option, string fieldSep)
	{
		int num = (maxPinLength << 4) + minPinLength;
		byte b = (byte)((waitTime <= 255 && waitTime > 0) ? ((uint)waitTime) : 0u);
		byte[] data = new byte[7]
		{
			MTPPSCRAConstants.OPERATION_SET,
			4,
			b,
			(byte)pinMode,
			(byte)num,
			(byte)beepTones,
			(byte)option
		};
		return doCommand(data);
	}

	public int requestSignature(int waitTime, int beepTones, int option)
	{
		if (GetDeviceInfo && DeviceCapability.SignatureCapture == IPADCapabilityValue.NotSupported)
		{
			return 7;
		}
		OpStatus = 0;
		int num = ((waitTime <= 255 && waitTime > 0) ? waitTime : 0);
		byte[] data = new byte[5]
		{
			MTPPSCRAConstants.OPERATION_SET,
			18,
			(byte)num,
			(byte)option,
			(byte)beepTones
		};
		return doCommand(data);
	}

	public int requestSmartCard(int cardType, int confirmationTime, int pinEnteringTime, int beepTones, int option, byte[] Amount, int transactionType, byte[] cashback, byte[] reserved)
	{
		if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L2)
		{
			return 7;
		}
		if (!BitConverter.ToString(Amount).Replace("-", "").All((char x) => x >= '0' && x <= '9'))
		{
			return 6;
		}
		if (!BitConverter.ToString(cashback).Replace("-", "").All((char x) => x >= '0' && x <= '9'))
		{
			return 6;
		}
		byte[] first = new byte[8]
		{
			MTPPSCRAConstants.OPERATION_SET,
			162,
			(byte)confirmationTime,
			(byte)pinEnteringTime,
			0,
			(byte)beepTones,
			(byte)cardType,
			(byte)option
		};
		byte[] second = new byte[1] { (byte)transactionType };
		if (reserved == null)
		{
			reserved = new byte[28];
		}
		else if (reserved.Count() > 44)
		{
			reserved = reserved.Take(44).ToArray();
		}
		return doCommand(first.Concat(Amount).Concat(second).Concat(cashback)
			.Concat(reserved)
			.ToArray());
	}

	public int sendAcquirerResponse(byte[] responseData, int dataLength)
	{
		if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L2)
		{
			return 7;
		}
		int[] opStatus = new int[1];
		TaskCompletionSource<int> iResult = new TaskCompletionSource<int>();
		Task.Run(delegate
		{
			int iRet = 0;
			sendBigBlockDataAsync(164, responseData, opStatus).ContinueWith(delegate(Task<int> t)
			{
				if (t.IsCompleted && t.Result == 0)
				{
					byte[] data = new byte[14]
					{
						MTPPSCRAConstants.OPERATION_SET,
						164,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					iRet = sendSpecialCommand(data);
				}
				else if (t.IsCompleted)
				{
					iRet = t.Result;
				}
				else
				{
					iRet = 255;
				}
				iResult.SetResult(iRet);
			});
		});
		if (iResult.Task.Wait(2000))
		{
			return iResult.Task.Result;
		}
		return 1;
	}

	public CARD_DATA getCardDataInfo()
	{
		return m_ReportParser.cardDataParser.lastCardData;
	}

	public string getDeviceFirmwareVersion()
	{
		return deviceFirmwareVersion;
	}

	public string getDeviceSerial()
	{
		return deviceSerial;
	}

	public string getDeviceModel()
	{
		return deviceModel;
	}

	public string getProductID()
	{
		return productID;
	}

	public byte[] requestDeviceConfiguration(ref int opStatus)
	{
		return requestDeviceConfigurationForInformation(ref opStatus);
	}

	public byte[] requestDeviceConfigurationForInformation(ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return null;
		}
		byte[] data = new byte[10]
		{
			MTPPSCRAConstants.OPERATION_GET,
			9,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};
		byte[] result = doSpecialCommand(data, 2);
		opStatus = getStatusCode();
		return result;
	}

	public bool isDeviceSRED()
	{
		int opStatus = 0;
		DeviceCapability.Capability = requestDeviceInformation(2, ref opStatus);
		return DeviceCapability.SRED == IPADCapabilityValue.L1;
	}

	public int updateFirmware(byte[] firmwareData, ref int opStatus)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		int opStatus2 = 0;
		int num = sendBigBlockData(23, firmwareData, ref opStatus2);
		if (num == 0 && opStatus2 == 0)
		{
			num = doCommand(new byte[10]
			{
				MTPPSCRAConstants.OPERATION_SET,
				23,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			});
		}
		opStatus = getAckStatus();
		return num;
	}

	public string getPINKSN()
	{
		return m_ReportParser.pinDataParser.lastPinData.KSN;
	}

	public int getSessionState()
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		int opStatus = 0;
		return requestDeviceStatus(ref opStatus).nSessionState;
	}

	public PIN_DATA getPINData()
	{
		return pin;
	}

	public string getPAN()
	{
		return getCardDataInfo().getPAN();
	}

	public string getEncodeType()
	{
		int operationStatus = 0;
		int cardStatus = 0;
		int cardType = 0;
		m_ReportParser.cardDataParser.getCardStatus(ref operationStatus, ref cardStatus, ref cardType);
		return $"{cardType}";
	}

	public string getTrack1()
	{
		return getCardDataInfo().EncTrack1;
	}

	public string getTrack2()
	{
		return getCardDataInfo().EncTrack2;
	}

	public string getTrack3()
	{
		return getCardDataInfo().EncTrack3;
	}

	public string getTrack1Masked()
	{
		return getCardDataInfo().Track1;
	}

	public string getTrack2Masked()
	{
		return getCardDataInfo().Track2;
	}

	public string getTrack3Masked()
	{
		return getCardDataInfo().Track3;
	}

	public string getMaskedTracks()
	{
		return getTrack1Masked() + getTrack2Masked() + getTrack3Masked();
	}

	public string getMagnePrint()
	{
		return getCardDataInfo().EncMP;
	}

	public string getMagnePrintStatus()
	{
		return $"{getCardDataInfo().MPSTS}";
	}

	public string getTrack1DecodeStatus()
	{
		return $"{getCardDataInfo().EncTrack1Status}";
	}

	public string getTrack2DecodeStatus()
	{
		return $"{getCardDataInfo().EncTrack2Status}";
	}

	public string getTrack3DecodeStatus()
	{
		return $"{getCardDataInfo().EncTrack3Status}";
	}

	public string getLastName()
	{
		return getCardDataInfo().getLastName();
	}

	public string getFirstName()
	{
		return getCardDataInfo().getFirstName();
	}

	public string getMiddleName()
	{
		return getCardDataInfo().getMiddleName();
	}

	public string getExpDate()
	{
		return getCardDataInfo().getExpDate();
	}

	public string getPINStatusCode()
	{
		if (pin.EPB == "" || pin.KSN == "")
		{
			return "";
		}
		return $"{pin.OpStatus}";
	}

	public string getEPB()
	{
		return m_ReportParser.pinDataParser.lastPinData.EPB;
	}

	public void clearBuffer()
	{
		m_ReportParser.cardDataParser.lastCardData.clear();
		m_ReportParser.pinDataParser.lastPinData.clear();
	}

	public string getKeyInfo(int KeyInfoId)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return "";
		}
		string result = "";
		byte[] getSpecialCommandSync = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			14,
			(byte)(KeyInfoId & 0xFF)
		};
		byte[] array = setGetSpecialCommandSync(getSpecialCommandSync);
		if (array != null && array.Length > 3)
		{
			result = BitConverter.ToString(array).Replace("-", "");
		}
		return result;
	}

	public AMKInfo getAMKInfo()
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return default(AMKInfo);
		}
		AMKInfo result = default(AMKInfo);
		try
		{
			string keyInfo = getKeyInfo(9);
			if (keyInfo != null && keyInfo.Length > 3)
			{
				byte[] array = new byte[keyInfo.Length / 2];
				for (int i = 0; i < keyInfo.Length / 2; i++)
				{
					array[i] = Convert.ToByte(keyInfo.Substring(i * 2, 2), 16);
				}
				result.Status = array[2];
				int num = array[3];
				if (num > 3)
				{
					byte[] array2 = new byte[3];
					Array.Copy(array, 4, array2, 0, 3);
					result.KCV = BitConverter.ToString(array2).Replace("-", "");
				}
				if (num > 4)
				{
					byte[] array3 = new byte[num - 4];
					Array.Copy(array, 8, array3, 0, num - 4);
					result.KeyLabel = Encoding.UTF8.GetString(array3, 0, array3.Length);
				}
			}
		}
		catch
		{
		}
		return result;
	}

	public int requestPowerUpResetICCSync(int waitTime, int operation, ref EMV_DATA atr, ref int opStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onATRReceived = new AutoResetEvent(initialState: false);
		EMV_DATA atrReceived = default(EMV_DATA);
		OnPowerUpICCCompleteEvent value = delegate(byte status, byte[] data)
		{
			atrReceived.Data = data;
			atrReceived.Length = ((data != null) ? data.Length : 0);
			atrReceived.OpStatus = status;
			onATRReceived.Set();
		};
		onPowerUpICC += value;
		int num = 0;
		try
		{
			num = requestPowerUpResetICC((byte)waitTime, (byte)operation);
			opStatus = getStatusCode();
			if (waitTime == 0)
			{
				waitTime = 65536;
			}
			if (num == 0 && opStatus == 0 && onATRReceived.WaitOne((int)((double)waitTime * 1.3 * 1000.0)))
			{
				atr = atrReceived;
				opStatus = atrReceived.OpStatus;
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onPowerUpICC -= value;
			opStatus = getStatusCode();
		}
		return num;
	}

	public int requestICCAPDUSync(byte[] apdu, int apduLen, byte[] pData, ref int pdwDataLen, ref int outOpStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		TaskCompletionSource<int> iResult = new TaskCompletionSource<int>();
		int opStatus = 0;
		int outDataLength = pdwDataLen;
		OnAPDUArriveCompleteEvent value = delegate(byte status, byte[] data)
		{
			if (pData != null && data != null && data.Length < outDataLength)
			{
				Array.Copy(data, pData, data.Length);
				outDataLength = data.Length;
			}
			else
			{
				outDataLength = 0;
			}
			opStatus = status;
			iResult.SetResult(0);
		};
		onAPDUArrived += value;
		int iRet = 0;
		try
		{
			Task.Run(delegate
			{
				iRet = requestICCAPDU(apdu, apduLen);
				opStatus = getStatusCode();
			});
			iResult.Task.Wait(2000);
			if (iResult.Task.IsCompleted)
			{
				iRet = iResult.Task.Result;
			}
		}
		catch (Exception)
		{
			iRet = 255;
		}
		finally
		{
			onAPDUArrived -= value;
			outOpStatus = opStatus;
			pdwDataLen = outDataLength;
		}
		return iRet;
	}

	public int requestSignatureSync(int waitTime, int beepTones, int option, ref SIG_DATA sig, ref int opStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onSigReceived = new AutoResetEvent(initialState: false);
		SIG_DATA sigData = default(SIG_DATA);
		int sigStatus = 0;
		OnSignatureArriveCompleteEvent value = delegate(byte status, byte[] data)
		{
			sigData.Data = data;
			if (data != null)
			{
				sigData.Sig_Length = data.Length;
			}
			sigStatus = status;
			onSigReceived.Set();
		};
		onSignatureArrived += value;
		int num = 0;
		try
		{
			num = requestSignature(waitTime, beepTones, option);
			sigStatus = getStatusCode();
			if (num == 0 && sigStatus == 0 && onSigReceived.WaitOne((int)((double)waitTime * 1.3 * 1000.0)))
			{
				sig = sigData;
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onSignatureArrived -= value;
			opStatus = sigStatus;
		}
		return num;
	}

	public int requestResponseSync(int waitTime, int selectMsg, int keyMask, int beepTones, ref byte outKey, ref int outOpStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onMsgReceived = new AutoResetEvent(initialState: false);
		int opStatus = 0;
		byte key = 0;
		OnKeyInputCompleteEvent value = delegate(byte status, byte keyPressed)
		{
			opStatus = status;
			key = keyPressed;
			onMsgReceived.Set();
		};
		onKeyInput += value;
		int num = 0;
		try
		{
			num = requestResponse(waitTime, selectMsg, keyMask, beepTones);
			opStatus = getStatusCode();
			if (num == 0 && opStatus == 0)
			{
				onMsgReceived.WaitOne((int)((double)waitTime * 1.3 * 1000.0));
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onKeyInput -= value;
			outKey = key;
			outOpStatus = opStatus;
		}
		return num;
	}

	public int confirmAmountSync(int waitTime, int beepTones, ref byte key, ref int opStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		return requestResponseSync(waitTime, 1, 6, beepTones, ref key, ref opStatus);
	}

	public int selectCreditDebitSync(int waitTime, int beepTones, ref byte key, ref int opStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		return requestResponseSync(waitTime, 0, 5, beepTones, ref key, ref opStatus);
	}

	public int requestPINSync(int waitTime, int pinMode, int minPinLength, int maxPinLength, int beepTones, int option, ref PIN_DATA outPin, ref int opStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onPinReceived = new AutoResetEvent(initialState: false);
		OnPINRequestCompleteEvent value = delegate
		{
			onPinReceived.Set();
		};
		onPinRequestComplete += value;
		int num = 0;
		try
		{
			num = requestPIN(waitTime, pinMode, minPinLength, maxPinLength, beepTones, option, "|");
			opStatus = getStatusCode();
			if (num == 0 && opStatus == 0 && onPinReceived.WaitOne((int)((double)waitTime * 1.3 * 1000.0)))
			{
				outPin = pin;
				opStatus = getStatusCode();
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onPinRequestComplete -= value;
		}
		return num;
	}

	public int requestCardSync(int waitTime, int displayMessage, int beepTones, ref CARD_DATA outCard, ref int opStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onCardReceived = new AutoResetEvent(initialState: false);
		OnCardRequestCompleteEvent value = delegate
		{
			onCardReceived.Set();
		};
		onCardRequestComplete += value;
		int num = 0;
		try
		{
			num = requestCard(waitTime, displayMessage, beepTones, "|");
			opStatus = getStatusCode();
			if (num == 0 && opStatus == 0 && onCardReceived.WaitOne((int)((double)waitTime * 1.3 * 1000.0)))
			{
				outCard = card;
				opStatus = getStatusCode();
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onCardRequestComplete -= value;
		}
		return num;
	}

	public int requestManualCardDataSync(int waitTime, int beepTones, int options, ref CARD_DATA outCard, ref int outOpStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onCardReceived = new AutoResetEvent(initialState: false);
		int opStatus = 0;
		OnCardRequestCompleteEvent value = delegate
		{
			onCardReceived.Set();
		};
		onCardRequestComplete += value;
		int num = 0;
		try
		{
			num = requestManualCardData(waitTime, beepTones, options, ref opStatus);
			opStatus = getStatusCode();
			if (num == 0 && opStatus == 0 && onCardReceived.WaitOne((int)((double)waitTime * 1.3 * 1000.0)))
			{
				outCard = card;
				opStatus = getStatusCode();
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onCardRequestComplete -= value;
			outOpStatus = opStatus;
		}
		return num;
	}

	public int requestUserDataEntrySync(int waitTime, int displayMessageID, int beepTones, ref USER_ENTRY_DATA userData, ref int outOpStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onUserDataReceived = new AutoResetEvent(initialState: false);
		int opStatus = 0;
		USER_ENTRY_DATA dataReceived = default(USER_ENTRY_DATA);
		OnUserDataEntryCompleteEvent value = delegate(USER_ENTRY_DATA data)
		{
			dataReceived = data;
			onUserDataReceived.Set();
		};
		onUserDataEntry += value;
		int num = 0;
		try
		{
			num = requestUserDataEntry(waitTime, displayMessageID, beepTones, ref opStatus);
			opStatus = getStatusCode();
			if (num == 0 && opStatus == 0 && onUserDataReceived.WaitOne((int)((double)waitTime * 1.3 * 1000.0)))
			{
				opStatus = getStatusCode();
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onUserDataEntry -= value;
			userData = dataReceived;
			outOpStatus = opStatus;
		}
		return num;
	}

	public int requestClearTextUserDataEntrySync(int waitTime, int displayMessageID, int beepTones, ref CLEAR_TEXT_USER_ENTRY_DATA userData, ref int outOpStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onUserDataReceived = new AutoResetEvent(initialState: false);
		int opStatus = 0;
		CLEAR_TEXT_USER_ENTRY_DATA dataReceived = default(CLEAR_TEXT_USER_ENTRY_DATA);
		OnClearTextUserDataEntryCompleteEvent value = delegate(CLEAR_TEXT_USER_ENTRY_DATA data)
		{
			dataReceived = data;
			opStatus = data.OpStatus;
			onUserDataReceived.Set();
		};
		onClearTextUserDataEntry += value;
		int num = 0;
		try
		{
			num = requestClearTextUserDataEntry(waitTime, displayMessageID, beepTones);
			opStatus = getStatusCode();
			if (num == 0 && opStatus == 0 && onUserDataReceived.WaitOne((int)((double)waitTime * 1.3 * 1000.0)))
			{
				opStatus = getStatusCode();
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onClearTextUserDataEntry -= value;
			userData = dataReceived;
			outOpStatus = opStatus;
		}
		return num;
	}

	public int requestSmartCardSync(int cardType, int confirmationTime, int pinEnteringTime, int beepTones, int option, byte[] Amount, int transactionType, byte[] cashBack, byte[] rfu, ref ACQUIRER_DATA arqcTag, ref int opOutStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		int iRet = 0;
		int opStatus = 0;
		ACQUIRER_DATA arqc = default(ACQUIRER_DATA);
		TaskCompletionSource<byte[]> emvDataResult = new TaskCompletionSource<byte[]>();
		OnEMVCompleteDataCompleteEvent value = delegate(byte status, byte[] data)
		{
			arqc.OpStatus = status;
			if (data != null)
			{
				arqc.Data = data;
				arqc.Length = data.Length;
			}
			emvDataResult.SetResult(data);
		};
		OnDeviceConnectionStateChanged value2 = delegate
		{
			arqc.OpStatus = byte.MaxValue;
			iRet = 5;
			arqc.Length = 0;
			emvDataResult.SetResult(null);
		};
		OnEMVTransactionCompleteEvent value3 = delegate(byte status, byte[] data)
		{
			arqc.OpStatus = status;
			if (data != null)
			{
				arqc.Data = data;
				arqc.Length = data.Length;
			}
			emvDataResult.SetResult(data);
		};
		onDeviceConnectionStateChanged += value2;
		onEMVDataComplete += value;
		onEmvTransactionComplete += value3;
		try
		{
			Task.Run(delegate
			{
				iRet = requestSmartCard(cardType, confirmationTime, pinEnteringTime, beepTones, option, Amount, transactionType, cashBack, rfu);
				opStatus = getStatusCode();
				arqc.OpStatus = (byte)opStatus;
				if (iRet != 0 || opStatus != 0)
				{
					emvDataResult.SetResult(null);
				}
			});
			if (emvDataResult.Task.Wait((confirmationTime + pinEnteringTime + 120) * 1200))
			{
				arqcTag = arqc;
				opStatus = arqc.OpStatus;
			}
		}
		catch (Exception)
		{
			iRet = 255;
		}
		finally
		{
			onDeviceConnectionStateChanged -= value2;
			onEMVDataComplete -= value;
			onEmvTransactionComplete -= value3;
			opOutStatus = opStatus;
		}
		return iRet;
	}

	public EMVResponse startEMVTransaction(int cardType, int confirmationTime, int pinEnteringTime, int beepTones, int option, byte[] Amount, int transactionType, byte[] cashBack)
	{
		EMVResponse result = new EMVResponse();
		if (!isDeviceOpened())
		{
			result.StatusCode = 5;
			return result;
		}
		int iRet = 0;
		int opStatus = 0;
		TaskCompletionSource<byte[]> emvDataResult = new TaskCompletionSource<byte[]>();
		OnEMVCompleteDataCompleteEvent value = delegate(byte status, byte[] data)
		{
			result.OpStatus = status;
			if (data != null)
			{
				result.ResponseData = data;
				result.ResponseType = EMVResponseType.ARQC;
			}
			emvDataResult.SetResult(data);
		};
		OnDeviceConnectionStateChanged value2 = delegate
		{
			result.OpStatus = 255;
			result.StatusCode = 5;
			emvDataResult.SetResult(null);
		};
		OnEMVTransactionCompleteEvent value3 = delegate(byte status, byte[] data)
		{
			result.OpStatus = status;
			if (data != null)
			{
				result.ResponseData = data;
				result.ResponseType = EMVResponseType.BatchData;
			}
			emvDataResult.SetResult(data);
		};
		onDeviceConnectionStateChanged += value2;
		onEMVDataComplete += value;
		onEmvTransactionComplete += value3;
		try
		{
			Task.Run(delegate
			{
				iRet = requestSmartCard(cardType, confirmationTime, pinEnteringTime, beepTones, option, Amount, transactionType, cashBack, null);
				if (iRet != 0 || opStatus != 0)
				{
					emvDataResult.SetResult(null);
				}
			});
			emvDataResult.Task.Wait((confirmationTime + pinEnteringTime + 120) * 1200);
		}
		catch (Exception)
		{
			iRet = 255;
		}
		finally
		{
			onDeviceConnectionStateChanged -= value2;
			onEMVDataComplete -= value;
			onEmvTransactionComplete -= value3;
		}
		return result;
	}

	public EMVResponse continueEMVTransaction(byte[] responseData)
	{
		EMVResponse result = new EMVResponse();
		if (!isDeviceOpened())
		{
			result.StatusCode = 5;
			return result;
		}
		AutoResetEvent onTransactionReceived = new AutoResetEvent(initialState: false);
		OnEMVTransactionCompleteEvent value = delegate(byte status, byte[] data)
		{
			result.OpStatus = status;
			if (data != null)
			{
				result.ResponseData = data;
				result.ResponseType = EMVResponseType.BatchData;
			}
			onTransactionReceived.Set();
		};
		onEmvTransactionComplete += value;
		int opStatus = 0;
		int iRet = 0;
		try
		{
			if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L2)
			{
				result.StatusCode = 7;
			}
			else
			{
				Task.Run(delegate
				{
					iRet = sendBigBlockDataSync(164, responseData, responseData.Length, 0);
					opStatus = getStatusCode();
					if (iRet == 0 && opStatus == 0)
					{
						byte[] data2 = new byte[14]
						{
							MTPPSCRAConstants.OPERATION_SET,
							164,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0
						};
						iRet = doCommand(data2);
					}
					opStatus = getStatusCode();
					if (iRet == 0 && opStatus == 0 && onTransactionReceived.WaitOne())
					{
						opStatus = getStatusCode();
					}
				}).Wait();
			}
		}
		catch (Exception)
		{
			iRet = 255;
		}
		finally
		{
			onEmvTransactionComplete -= value;
			result.OpStatus = opStatus;
		}
		return result;
	}

	public int sendAcquirerResponseSync(byte[] responseData, int responseDataLength, ref EMV_DATA merchantData, ref int outOpStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onTransactionReceived = new AutoResetEvent(initialState: false);
		EMV_DATA merchant = default(EMV_DATA);
		OnEMVTransactionCompleteEvent value = delegate(byte status, byte[] data)
		{
			merchant.OpStatus = status;
			if (data != null)
			{
				merchant.Data = data;
				merchant.Length = data.Length;
			}
			onTransactionReceived.Set();
		};
		onEmvTransactionComplete += value;
		int opStatus = 0;
		int iRet = 0;
		try
		{
			if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L2)
			{
				return 7;
			}
			Task.Run(delegate
			{
				iRet = sendBigBlockDataSync(164, responseData, responseDataLength, 0);
				opStatus = getStatusCode();
				if (iRet == 0 && opStatus == 0)
				{
					byte[] data2 = new byte[14]
					{
						MTPPSCRAConstants.OPERATION_SET,
						164,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					iRet = doCommand(data2);
				}
				opStatus = getStatusCode();
				if (iRet == 0 && opStatus == 0 && onTransactionReceived.WaitOne())
				{
					opStatus = getStatusCode();
				}
			}).Wait();
		}
		catch (Exception)
		{
			iRet = 255;
		}
		finally
		{
			onEmvTransactionComplete -= value;
			merchantData = merchant;
			outOpStatus = opStatus;
		}
		return iRet;
	}

	public int requestGetEMVTagsSync(int tagType, int tagOperation, byte[] inputTLVData, int inputDataLength, byte database, byte option, byte[] reserved, ref EMV_DATA emvTags, ref int outOpStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onTagsReceived = new AutoResetEvent(initialState: false);
		EMV_DATA tags = default(EMV_DATA);
		int num = 0;
		OnEMVTagsCompleteEvent value = delegate(byte status, byte[] data)
		{
			tags.OpStatus = status;
			if (data != null)
			{
				tags.Data = data;
				tags.Length = data.Length;
			}
			onTagsReceived.Set();
		};
		onEMVTagsComplete += value;
		int num2 = 0;
		try
		{
			if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L2)
			{
				return 7;
			}
			num2 = requestEMVTagsSync(tagType, tagOperation, inputTLVData, inputDataLength, database, option, reserved, waitForComplete: true);
			num = getStatusCode();
			if (num2 == 0 && num == 0)
			{
				if (onTagsReceived.WaitOne(2000))
				{
					emvTags = tags;
					num = tags.OpStatus;
				}
				else
				{
					emvTags = tags;
				}
			}
		}
		catch (Exception)
		{
			num2 = 255;
		}
		finally
		{
			onEMVTagsComplete -= value;
			outOpStatus = num;
		}
		return num2;
	}

	public int requestSetEMVTagsSync(int tagType, int tagOperation, byte[] inputTLVData, int inputDataLength, byte database, byte option, byte[] reserved, ref EMV_DATA emvTags, ref int opStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		int result = requestEMVTagsSync(tagType, tagOperation, inputTLVData, inputDataLength, database, option, reserved, waitForComplete: true);
		opStatus = getStatusCode();
		return result;
	}

	public int setCAPublicKeySync(int operation, byte[] keyBlock, int keyBlockLength, byte[] key, ref int keyLen, ref int outOpStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onCAPKReceived = new AutoResetEvent(initialState: false);
		int opStatus = 0;
		byte[] CAPK = null;
		int CAPKLen = 0;
		OnGetCAPublicKeyCompleteEvent value = delegate(byte status, byte[] data)
		{
			if (data != null)
			{
				CAPK = data;
				CAPKLen = data.Length;
			}
			opStatus = status;
			onCAPKReceived.Set();
		};
		onGetCAPublicKey += value;
		int num = 0;
		try
		{
			if (!isDeviceOpened())
			{
				return 5;
			}
			if (GetDeviceInfo && DeviceCapability.ContactEMV != IPADCapabilityValue.L2)
			{
				return 7;
			}
			num = setCAPublicKey(operation, keyBlock, keyBlockLength);
			opStatus = getStatusCode();
			if (num == 0 && opStatus == 0 && (operation == 4 || operation == 15) && !onCAPKReceived.WaitOne(2000))
			{
				num = 1;
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onGetCAPublicKey -= value;
			outOpStatus = opStatus;
			keyLen = CAPKLen;
			if (key != null && CAPK != null)
			{
				Array.Copy(CAPK, 0, key, 0, (CAPKLen > keyLen) ? keyLen : CAPKLen);
			}
		}
		return num;
	}

	public int setDisplayMessageSync(int waitTime, int messageID, ref int outOpStatus)
	{
		if (!isDeviceOpened())
		{
			return 5;
		}
		AutoResetEvent onMsgReceived = new AutoResetEvent(initialState: false);
		int opStatus = 0;
		OnDisplayRequestCompleteEvent value = delegate(byte status)
		{
			opStatus = status;
			onMsgReceived.Set();
		};
		onDisplayRequestComplete += value;
		int num = 0;
		try
		{
			num = setDisplayMessage(waitTime, messageID, ref opStatus);
			if (num == 0 && opStatus == 0)
			{
				onMsgReceived.WaitOne((int)((double)waitTime * 1.3 * 1000.0));
			}
		}
		catch (Exception)
		{
			num = 255;
		}
		finally
		{
			onDisplayRequestComplete -= value;
			outOpStatus = opStatus;
		}
		return num;
	}

	public byte getTransactionCurrencyExponent()
	{
		EMV_DATA emvTags = default(EMV_DATA);
		int outOpStatus = 0;
		int num = requestGetEMVTagsSync(0, 1, null, 0, 0, 0, null, ref emvTags, ref outOpStatus);
		try
		{
			if (num == 0 && outOpStatus == 0 && emvTags.OpStatus == 0)
			{
				byte[] data = emvTags.getData();
				if (data != null && data.Length > 2)
				{
					int num2 = data[0] * 256 + data[1];
					if (num2 <= data.Length - 2)
					{
						return utils.HexStringToByteArray(TLV.getTagValue(TLV.parseTLVData(data.Skip(2).Take(num2).ToArray()), "5F36"))[0];
					}
				}
			}
		}
		catch
		{
		}
		return byte.MaxValue;
	}

	private string getTLVLengthString(int Length)
	{
		string text = "00";
		if (Length < 128)
		{
			return Length.ToString("X2");
		}
		if (Length < 256)
		{
			return "81" + Length.ToString("X2");
		}
		if (Length < 65536)
		{
			return "82" + Length.ToString("X4");
		}
		if (Length < 16777216)
		{
			return "83" + Length.ToString("X6");
		}
		return "84" + Length.ToString("X8");
	}

	public string getTLVPayload()
	{
		CARD_DATA cardDataInfo = getCardDataInfo();
		string text = "DFDF37" + getTLVLengthString(cardDataInfo.EncTrack1Length) + cardDataInfo.EncTrack1;
		string text2 = "DFDF39" + getTLVLengthString(cardDataInfo.EncTrack2Length) + cardDataInfo.EncTrack2;
		string text3 = "DFDF3B" + getTLVLengthString(cardDataInfo.EncTrack3Length) + cardDataInfo.EncTrack3;
		string text4 = "DFDF3C" + getTLVLengthString(cardDataInfo.EncMPLength) + cardDataInfo.EncMP;
		string text5 = "DFDF3D" + (string.IsNullOrEmpty(cardDataInfo.MPSTS) ? "00" : getTLVLengthString(cardDataInfo.MPSTS.Length / 2)) + cardDataInfo.MPSTS;
		string text6 = "DFDF50" + (string.IsNullOrEmpty(cardDataInfo.KSN) ? "00" : getTLVLengthString(cardDataInfo.KSN.Length / 2)) + cardDataInfo.KSN;
		string text7 = text + text2 + text3 + text4 + text5 + text6;
		string text8 = "F4" + getTLVLengthString(text7.Length / 2) + text7;
		string text9 = "DFDF2500";
		if (!string.IsNullOrWhiteSpace(cardDataInfo.SerialNumber))
		{
			string text10 = BitConverter.ToString(Encoding.UTF8.GetBytes(cardDataInfo.SerialNumber)).Replace("-", "");
			text9 = "DFDF25" + getTLVLengthString(cardDataInfo.SerialNumber.Length) + text10;
		}
		string text11 = text9 + text8;
		return "FA" + getTLVLengthString(text11.Length / 2) + text11;
	}

	public int loadClientCertificate(string format, byte[] data, string password)
	{
		try
		{
			MTTLS12Server.ServerCert = (MTTLS12TrustAll.ClientCert = (MTTLS12.ClientCert = new X509Certificate2(data, password)));
			return 0;
		}
		catch
		{
		}
		return 1;
	}

	public int requestTipOrCashback(byte waitTime, byte mode, byte tone, byte[] amount, byte[] taxAmount, byte[] taxRate, byte tipMode, byte option1, byte option2, byte option3, byte[] reserved)
	{
		byte[] array = new byte[49];
		array[0] = MTPPSCRAConstants.OPERATION_SET;
		array[1] = 160;
		array[2] = waitTime;
		array[3] = mode;
		array[4] = tone;
		try
		{
			Array.Copy(amount, 0, array, 5, 6);
			Array.Copy(taxAmount, 0, array, 11, 6);
			Array.Copy(taxRate, 0, array, 17, 3);
		}
		catch
		{
			return 6;
		}
		array[20] = tipMode;
		array[21] = option1;
		array[22] = option2;
		array[23] = option3;
		try
		{
			if (reserved != null)
			{
				int length = ((reserved.Length > 25) ? 25 : reserved.Length);
				Array.Copy(reserved, 0, array, 24, length);
			}
		}
		catch
		{
		}
		return sendSpecialCommand(array);
	}

	public int getSelectedMenuItem(byte waitTime, byte mode, byte tone, byte[] data)
	{
		if (m_connectionState != MTConnectionState.Connected)
		{
			return 5;
		}
		int opStatus = 0;
		int num = sendBigBlockData(22, data, ref opStatus);
		if (num == 0 && opStatus == 0)
		{
			num = doCommand(new byte[5]
			{
				MTPPSCRAConstants.OPERATION_SET,
				MTPPSCRAConstants.COMMAND_SELECT_MENU_ITEM,
				waitTime,
				mode,
				tone
			});
		}
		return num;
	}
}
