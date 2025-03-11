// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.SCRADevice
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


public class SCRADevice : BaseDevice
{
	private MTSCRA mSCRA;

	private SCRASpiMsr mSpiMsr;

	private SCRAUartNfc mUartNfc;

	private bool mOEMContactInProgress;

	private bool mOEMContactlessInProgress;

	public SCRADevice(ConnectionInfo connectionInfo)
	{
		init(connectionInfo, new DeviceInfo());
	}

	public SCRADevice(ConnectionInfo connectionInfo, DeviceInfo deviceInfo)
	{
		init(connectionInfo, deviceInfo);
	}

	protected override void init(ConnectionInfo connectionInfo, DeviceInfo deviceInfo)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		mConnectionInfo = connectionInfo;
		mDeviceInfo = deviceInfo;
		ConnectionType connectionType = ConnectionType.USB;
		string text = "";
		if (mConnectionInfo != null)
		{
			connectionType = mConnectionInfo.getConnectionType();
			text = mConnectionInfo.getAddress();
		}
		mSCRA = new MTSCRA();
		mSCRA.OnDeviceConnectionStateChanged += new DeviceConnectionStateHandler(OnDeviceConnectionStateChanged);
		mSCRA.OnDeviceResponse += new DeviceResponseHandler(OnDeviceResponse);
		mSCRA.OnDeviceExtendedResponse += new DeviceExtendedResponseHandler(OnDeviceExtendedResponse);
		mSCRA.OnDataReceived += new DataReceivedHandler(OnDataReceived);
		mSCRA.OnEMVCommandResult += new EMVCommandResultHandler(OnEMVCommandResult);
		mSCRA.OnTransactionStatus += new TransactionStatusHandler(OnTransactionStatus);
		mSCRA.OnDisplayMessageRequest += new DisplayMessageRequestHandler(OnDisplayMessageRequest);
		mSCRA.OnUserSelectionRequest += new UserSelectionRequestHandler(OnUserSelectionRequest);
		mSCRA.OnARQCReceived += new ARQCReceivedHandler(OnARQCReceived);
		mSCRA.OnTransactionResult += new TransactionResultHandler(OnTransactionResult);
		mSCRA.setConnectionType(getMTConnectionType(connectionType));
		if (connectionType == ConnectionType.SERIAL)
		{
			text = "PORT=" + text + ",BAUDRATE=115200";
		}
		mSCRA.setAddress(text);
	}

	protected static MTConnectionType getMTConnectionType(ConnectionType connectionType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		MTConnectionType result = (MTConnectionType)5;
		switch (connectionType)
		{
		case ConnectionType.USB:
			result = (MTConnectionType)5;
			break;
		case ConnectionType.BLUETOOTH_LE_EMV:
			result = (MTConnectionType)3;
			break;
		case ConnectionType.BLUETOOTH_LE_EMVT:
			result = (MTConnectionType)10;
			break;
		case ConnectionType.BLUETOOTH_LE:
			result = (MTConnectionType)2;
			break;
		case ConnectionType.SERIAL:
			result = (MTConnectionType)6;
			break;
		case ConnectionType.AUDIO:
			result = (MTConnectionType)1;
			break;
		}
		return result;
	}

	public override IDeviceCapabilities getCapabilities()
	{
		IDeviceCapabilities result = null;
		if (mSCRA != null)
		{
			MTDeviceFeatures deviceFeatures = mSCRA.getDeviceFeatures();
			if (deviceFeatures != null)
			{
				result = new SCRADeviceCapabilities(deviceFeatures.MSR, deviceFeatures.Contact, deviceFeatures.Contactless, deviceFeatures.ManualEntry, deviceFeatures.MSRPowerSaver, deviceFeatures.BatteryBackedClock);
			}
		}
		return result;
	}

	public override IDeviceControl getDeviceControl()
	{
		if (mDeviceControl == null)
		{
			mDeviceControl = new SCRADeviceControl(mSCRA);
		}
		return mDeviceControl;
	}

	private void resetOEMDevice()
	{
		mSpiMsr = null;
		mUartNfc = null;
		mOEMContactInProgress = false;
		mOEMContactlessInProgress = false;
	}

	private void setupOEMDevice()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		if (mSCRA.isDeviceOEM())
		{
			mOEMContactInProgress = false;
			mOEMContactlessInProgress = false;
			if (mSpiMsr == null)
			{
				mSpiMsr = new SCRASpiMsr(mSCRA);
				mSpiMsr.OnDataReceived += OnSpiMsrDataReceived;
				mSpiMsr.OnResponseReceived += OnSpiMsrResponseReceived;
				mSpiMsr.OnDebugInfo += OnSpiMsrDebugInfo;
			}
			if (mUartNfc == null)
			{
				mUartNfc = new SCRAUartNfc(mSCRA);
				mUartNfc.OnDebugInfo += OnUartNfcDebugInfo;
				mUartNfc.OnDeviceResponse += new DeviceResponseHandler(OnUartNfcDeviceResponse);
				mUartNfc.OnTransactionStatus += new TransactionStatusHandler(OnUartTransactionStatus);
				mUartNfc.OnDisplayMessageRequest += new DisplayMessageRequestHandler(OnUartDisplayMessageRequest);
				mUartNfc.OnUserSelectionRequest += new UserSelectionRequestHandler(OnUartUserSelectionRequest);
				mUartNfc.OnARQCReceived += new ARQCReceivedHandler(OnUartARQCReceived);
				mUartNfc.OnTransactionResult += new TransactionResultHandler(OnUartTransactionResult);
			}
		}
	}

	private string getSetDateTimeCommand()
	{
		DateTime now = DateTime.Now;
		int month = now.Month;
		int day = now.Day;
		int hour = now.Hour;
		int minute = now.Minute;
		int second = now.Second;
		int num = now.Year - 2008;
		string text = $"{month:X2}{day:X2}{hour:X2}{minute:X2}{second:X2}00{num:X2}";
		return "491E0000030C00180000000000000000000000000000000000" + text;
	}

	private void sendSetDateTimeCommand()
	{
		string setDateTimeCommand = getSetDateTimeCommand();
		getDeviceControl().send(new BaseData(setDateTimeCommand));
	}

	private void setMSRPower(bool state)
	{
		string stringValue = "5801" + (state ? "01" : "00");
		getDeviceControl().send(new BaseData(stringValue));
	}

	public override bool startTransaction(ITransaction transaction)
	{
		bool result = true;
		if (mSCRA != null && transaction != null)
		{
			Task.Factory.StartNew((Func<Task>)async delegate
			{
				if (checkConnectedDevice())
				{
					List<PaymentMethod> paymentMethods = transaction.PaymentMethods;
					if (paymentMethods != null)
					{
						if (paymentMethods.Count == 1 && paymentMethods[0] == PaymentMethod.MSR && !transaction.EMVOnly)
						{
							if (mSCRA.getDeviceFeatures().MSRPowerSaver)
							{
								setMSRPower(state: true);
							}
						}
						else
						{
							result = startEMVTransaction(transaction);
						}
					}
				}
			});
		}
		return true;
	}

	private byte getCardTypeValue(List<PaymentMethod> paymentMethods)
	{
		byte b = 0;
		if (paymentMethods != null)
		{
			using List<PaymentMethod>.Enumerator enumerator = paymentMethods.GetEnumerator();
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case PaymentMethod.MSR:
					b |= 1;
					break;
				case PaymentMethod.Contact:
					b |= 2;
					break;
				case PaymentMethod.Contactless:
					if (mSCRA.getDeviceFeatures().Contactless)
					{
						b |= 4;
					}
					break;
				}
			}
		}
		return b;
	}

	protected bool startEMVTransaction(ITransaction transaction)
	{
		if (mSCRA != null)
		{
			if (!mSCRA.getDeviceFeatures().BatteryBackedClock)
			{
				sendSetDateTimeCommand();
			}
			byte timeLimit = 60;
			byte cardType = getCardTypeValue(transaction.PaymentMethods);
			byte option = 0;
			if (transaction.QuickChip)
			{
				option |= 128;
			}
			byte[] amount = BaseDevice.GetN12Bytes(transaction.Amount);
			byte transactionType = 0;
			byte[] cashBack = BaseDevice.GetN12Bytes(transaction.CashBack);
			byte[] currencyCode = new byte[2] { 8, 64 };
			byte reportingOption = 2;
			if (mSCRA.isDeviceOEM())
			{
				if (transaction.PaymentMethods.Contains(PaymentMethod.Contactless))
				{
					startOEMTransaction(timeLimit, 4, option, amount, transactionType, cashBack, currencyCode, reportingOption);
				}
				if (transaction.PaymentMethods.Contains(PaymentMethod.Contact))
				{
					mOEMContactInProgress = true;
					mSCRA.startTransaction(timeLimit, (byte)2, option, amount, transactionType, cashBack, currencyCode, reportingOption);
				}
			}
			else if (mConnectionInfo.getConnectionType() == ConnectionType.SERIAL)
			{
				Task.Factory.StartNew((Func<Task>)async delegate
				{
					await Task.Delay(500);
					mSCRA.startTransaction(timeLimit, cardType, option, amount, transactionType, cashBack, currencyCode, reportingOption);
				});
			}
			else
			{
				mSCRA.startTransaction(timeLimit, cardType, option, amount, transactionType, cashBack, currencyCode, reportingOption);
			}
		}
		return true;
	}

	public void startOEMTransaction(byte timeLimit, byte cardType, byte option, byte[] amount, byte transactionType, byte[] cashBack, byte[] currencyCode, byte reportingOption)
	{
		if (mSCRA != null && mUartNfc != null)
		{
			string setDateTimeCommand = getSetDateTimeCommand();
			mUartNfc.sendData(setDateTimeCommand);
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
			byte[] command = new byte[2] { 3, 0 };
			mOEMContactlessInProgress = true;
			sendUartNfcExtendedCommand(command, array);
		}
	}

	public int sendUartNfcExtendedCommand(byte[] command, byte[] data)
	{
		int result = 9;
		if (mSCRA.isDeviceConnected() && mUartNfc != null)
		{
			result = mUartNfc.sendExtendedCommandBytes(command, data);
		}
		return result;
	}

	public override bool cancelTransaction()
	{
		if (mSCRA != null)
		{
			if (mSCRA.isDeviceOEM())
			{
				if (mOEMContactlessInProgress)
				{
					byte[] command = new byte[2] { 3, 4 };
					sendUartNfcExtendedCommand(command, null);
					mOEMContactlessInProgress = false;
				}
				if (mOEMContactInProgress)
				{
					mSCRA.cancelTransaction();
					mOEMContactInProgress = false;
				}
			}
			else
			{
				mSCRA.cancelTransaction();
			}
		}
		return true;
	}

	public override bool sendSelection(IData data)
	{
		if (mSCRA != null)
		{
			byte[] byteArray = data.ByteArray;
			if (byteArray != null && byteArray.Length >= 2)
			{
				if (mSCRA.isDeviceOEM())
				{
					byte[] command = new byte[2] { 3, 2 };
					sendUartNfcExtendedCommand(command, data.ByteArray);
				}
				else
				{
					mSCRA.setUserSelectionResult(byteArray[0], byteArray[1]);
				}
			}
		}
		return true;
	}

	public override bool sendAuthorization(IData data)
	{
		if (mSCRA != null)
		{
			if (mSCRA.isDeviceOEM())
			{
				byte[] command = new byte[2] { 3, 3 };
				sendUartNfcExtendedCommand(command, data.ByteArray);
			}
			else
			{
				mSCRA.setAcquirerResponse(data.ByteArray);
			}
		}
		return true;
	}

	protected void OnDeviceConnectionStateChanged(object sender, MTConnectionState state)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected I4, but got Unknown
		ConnectionState connectionStateValue = ConnectionState.Unknown;
		switch ((int)state)
		{
		case 1:
			connectionStateValue = ConnectionState.Connecting;
			break;
		case 3:
			connectionStateValue = ConnectionState.Connected;
			setupOEMDevice();
			break;
		case 4:
			connectionStateValue = ConnectionState.Disconnecting;
			break;
		case 0:
			connectionStateValue = ConnectionState.Disconnected;
			resetOEMDevice();
			break;
		}
		updateConnectionStateValue(connectionStateValue);
	}

	protected void OnDeviceResponse(object sender, string data)
	{
		IData data2 = new BaseData(data);
		sendEvent(EventType.DeviceResponse, data2);
	}

	protected void OnDeviceExtendedResponse(object sender, string data)
	{
		if (data.Length <= 0)
		{
			return;
		}
		byte[] byteArrayFromHexString = TLVParser.getByteArrayFromHexString(data);
		IData data2 = new BaseData(data);
		sendEvent(EventType.DeviceExtendedResponse, data2);
		if (mSCRA.isDeviceOEM())
		{
			if (mSpiMsr != null)
			{
				mSpiMsr.processDeviceExtendedResponse(byteArrayFromHexString);
			}
			if (mUartNfc != null)
			{
				mUartNfc.processDeviceExtendedResponse(byteArrayFromHexString);
			}
		}
	}

	protected void OnDataReceived(object sender, IMTCardData cardData)
	{
		if (mSCRA.isDeviceOEM())
		{
			if (mOEMContactlessInProgress)
			{
				byte[] command = new byte[2] { 3, 4 };
				sendUartNfcExtendedCommand(command, null);
				mOEMContactlessInProgress = false;
			}
			if (mOEMContactInProgress)
			{
				mSCRA.cancelTransaction();
				mOEMContactInProgress = false;
			}
		}
		IData data = new BaseData("Card Swiped");
		sendEvent(EventType.DisplayMessage, data);
		IData data2 = new BaseData(SCRACardDataBuilder.buildTLVPayload(cardData));
		sendEvent(EventType.CardData, data2);
	}

	protected void OnEMVCommandResult(object sender, byte[] data)
	{
		string hexString = TLVParser.getHexString(data);
		if (!hexString.StartsWith("0000"))
		{
			IData data2 = new BaseData(TransactionStatusBuilder.TRANSACTION_ERROR + ",FFFF," + hexString);
			sendEvent(EventType.TransactionStatus, data2);
		}
	}

	protected void OnTransactionStatus(object sender, byte[] data)
	{
		if (data == null || data.Length < 3)
		{
			return;
		}
		byte b = data[0];
		byte b2 = data[2];
		string hexString = TLVParser.getHexString(data);
		switch (b)
		{
		case 1:
			updateTransactionStatusValue(TransactionStatus.CardInserted);
			if (mOEMContactInProgress && mOEMContactlessInProgress)
			{
				byte[] command = new byte[2] { 3, 4 };
				sendUartNfcExtendedCommand(command, null);
				mOEMContactlessInProgress = false;
			}
			break;
		case 3:
			updateTransactionStatusValue(TransactionStatus.TransactionInProgress);
			break;
		case 5:
			updateTransactionStatusValue(TransactionStatus.TimedOut);
			break;
		case 6:
			switch (b2)
			{
			case 18:
				updateTransactionStatusValue(TransactionStatus.TransactionError, "TransactionError", hexString);
				break;
			case 19:
				updateTransactionStatusValue(TransactionStatus.TransactionApproved);
				break;
			case 20:
				updateTransactionStatusValue(TransactionStatus.TransactionDeclined);
				break;
			}
			break;
		case 7:
			updateTransactionStatusValue(TransactionStatus.HostCancelled);
			break;
		case 8:
			updateTransactionStatusValue(TransactionStatus.CardRemoved);
			break;
		case 9:
			if (mOEMContactlessInProgress && mOEMContactInProgress)
			{
				mSCRA.cancelTransaction();
				mOEMContactInProgress = false;
			}
			break;
		case 2:
		case 4:
			break;
		}
	}

	protected void OnDisplayMessageRequest(object sender, byte[] data)
	{
		if (mSCRA.isDeviceOEM())
		{
			if (sender == mUartNfc)
			{
				if (!mOEMContactlessInProgress)
				{
					return;
				}
			}
			else if (!mOEMContactInProgress)
			{
				return;
			}
		}
		string stringValue = "";
		if (data != null && data.Length != 0)
		{
			stringValue = Encoding.UTF8.GetString(data);
		}
		IData data2 = new BaseData(stringValue);
		sendEvent(EventType.DisplayMessage, data2);
	}

	protected void OnUserSelectionRequest(object sender, byte[] data)
	{
		if (mSCRA.isDeviceOEM())
		{
			if (sender == mUartNfc)
			{
				if (!mOEMContactlessInProgress)
				{
					return;
				}
			}
			else if (!mOEMContactInProgress)
			{
				return;
			}
		}
		processSelectionRequest(data);
	}

	protected void processSelectionRequest(byte[] data)
	{
		IData data2 = new BaseData(data);
		sendEvent(EventType.InputRequest, data2);
	}

	protected void OnARQCReceived(object sender, byte[] data)
	{
		if (mSCRA.isDeviceOEM())
		{
			if (sender == mUartNfc)
			{
				if (!mOEMContactlessInProgress)
				{
					return;
				}
			}
			else if (!mOEMContactInProgress)
			{
				return;
			}
		}
		if (data != null && data.Length > 2)
		{
			byte[] array = new byte[data.Length];
			Array.Copy(data, 0, array, 0, data.Length);
			IData data2 = new BaseData(array);
			sendEvent(EventType.AuthorizationRequest, data2);
		}
	}

	protected void OnTransactionResult(object sender, byte[] data)
	{
		if (mSCRA.isDeviceOEM())
		{
			if (sender == mUartNfc)
			{
				if (!mOEMContactlessInProgress)
				{
					return;
				}
			}
			else if (!mOEMContactInProgress)
			{
				return;
			}
		}
		if (data != null && data.Length > 3)
		{
			byte[] array = new byte[data.Length - 1];
			Array.Copy(data, 1, array, 0, data.Length - 1);
			IData data2 = new BaseData(array);
			sendEvent(EventType.TransactionResult, data2);
		}
	}

	protected void OnSpiMsrDebugInfo(object sender, string data)
	{
	}

	protected void OnSpiMsrDataReceived(object sender, IMTCardData cardData)
	{
		OnDataReceived(mSpiMsr, cardData);
	}

	protected void OnSpiMsrResponseReceived(object sender, string response)
	{
	}

	protected void OnUartNfcDebugInfo(object sender, string data)
	{
	}

	protected void OnUartNfcDeviceResponse(object sender, string data)
	{
	}

	protected void OnUartTransactionStatus(object sender, byte[] data)
	{
		OnTransactionStatus(mUartNfc, data);
	}

	protected void OnUartDisplayMessageRequest(object sender, byte[] data)
	{
		OnDisplayMessageRequest(mUartNfc, data);
	}

	protected void OnUartUserSelectionRequest(object sender, byte[] data)
	{
		OnUserSelectionRequest(mUartNfc, data);
	}

	protected void OnUartARQCReceived(object sender, byte[] data)
	{
		OnARQCReceived(mUartNfc, data);
	}

	protected void OnUartTransactionResult(object sender, byte[] data)
	{
		OnTransactionResult(mUartNfc, data);
	}
}
