// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PPSCRADevice
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;




public class PPSCRADevice : BaseDevice
{
	public enum OpStatus : byte
	{
		[Description("00 - OK")]
		OK = 0,
		[Description("01 - User Cancel")]
		UserCancel = 1,
		[Description("02 - Time Out")]
		Timeout = 2,
		[Description("03 - Host Cancel")]
		HostCancel = 3,
		[Description("04 - Verify Fail")]
		VerifyFail = 4,
		[Description("05 - Keypad Security")]
		KBSecurity = 5,
		[Description("06 - Calibration Done")]
		CalibrationDone = 6,
		[Description("07 - Write with duplicate RID and Index")]
		DuplidateRIDAndIndex = 7,
		[Description("08 - Write with corrupted Key")]
		CorruptedKey = 8,
		[Description("09 - CA Public Key reached maximum capacity")]
		CAPKMaximumCapacity = 9,
		[Description("0A - CA Public Key read with invalid RID or Index")]
		CAPKInvalidRIDOrIndex = 10,
		[Description("15 - RID error or Index not found")]
		RIDError = 21,
		[Description("80 - Device Error")]
		Err_Security = 128,
		[Description("81 - Device is not idle")]
		Err_NotIdle = 129,
		[Description("82 - Data Error or Bad Parameters")]
		Err_Data = 130,
		[Description("83 - Length Error")]
		Err_Length = 131,
		[Description("84 - PAN Exists")]
		Err_PANExists = 132,
		[Description("85 - No Key or Key is incorrect")]
		KeyIncorrect = 133,
		[Description("86 - Device Busy")]
		DeviceBusy = 134,
		[Description("87 - Device Locked")]
		DeviceLocked = 135,
		[Description("88 - Authentication Required")]
		AuthRequired = 136,
		[Description("89 - Bad Authenication")]
		BadAuthentication = 137,
		[Description("8A - Device not available")]
		DeviceNotAvailable = 138,
		[Description("8B - Amount Needed")]
		AmountNeeded = 139,
		[Description("8C - Battery Critically Low")]
		BatteryLow = 140,
		[Description("8D - Device is resetting")]
		DeviceResetting = 141,
		[Description("90 - Certificate doesn't exist")]
		CertificateNotExist = 144,
		[Description("91 - Expired Certificate")]
		ExpiredCertificate = 145,
		[Description("92 - Invalid Certificate")]
		InvalidCertificate = 146,
		[Description("93 - Revoked Certificate")]
		RevokedCertificate = 147,
		[Description("94 - CRL doesn't exist")]
		CRLNotExist = 148,
		[Description("95 - Certificate Exists")]
		CertificateExists = 149,
		[Description("96 - Duplicate KSN/Key")]
		DuplicateKSNOrKey = 150
	}

	private MTPPSCRA mPPSCRA;

	internal static Dictionary<ConnectionType, string> ConnectionTypeURI = new Dictionary<ConnectionType, string>
	{
		{
			ConnectionType.USB,
			"USB://"
		},
		{
			ConnectionType.BLUETOOTH_LE,
			"BLE://"
		},
		{
			ConnectionType.BLUETOOTH_LE_EMV,
			"BLEEMV://"
		},
		{
			ConnectionType.TCP,
			"IP://"
		},
		{
			ConnectionType.TCP_TLS,
			"TLS12://"
		},
		{
			ConnectionType.TCP_TLS_TRUST,
			"TLS12TRUST://"
		}
	};

	public PPSCRADevice(ConnectionInfo connectionInfo)
	{
		init(connectionInfo, new DeviceInfo());
	}

	public PPSCRADevice(ConnectionInfo connectionInfo, DeviceInfo deviceInfo)
	{
		init(connectionInfo, deviceInfo);
	}

	protected override void init(ConnectionInfo connectionInfo, DeviceInfo deviceInfo)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		mConnectionInfo = connectionInfo;
		mDeviceInfo = deviceInfo;
		ConnectionType connectionType = ConnectionType.USB;
		if (mConnectionInfo != null)
		{
			connectionType = mConnectionInfo.getConnectionType();
		}
		mPPSCRA = new MTPPSCRA();
		mPPSCRA.onDeviceConnectionStateChanged += new OnDeviceConnectionStateChanged(OnDeviceConnectionStateChanged);
		mPPSCRA.onDataReady += new OnDataReadyCompleteEvent(OnDataReady);
		mPPSCRA.onEMVDataComplete += new OnEMVCompleteDataCompleteEvent(OnEMVDataComplete);
		mPPSCRA.onEmvTransactionComplete += new OnEMVTransactionCompleteEvent(OnEmvTransactionComplete);
		mPPSCRA.onCardRequestComplete += new OnCardRequestCompleteEvent(OnCardRequestComplete);
		mPPSCRA.onPinRequestComplete += new OnPINRequestCompleteEvent(OnPinRequestComplete);
		mPPSCRA.onSignatureArrived += new OnSignatureArriveCompleteEvent(OnSignatureArrived);
		if ((connectionType == ConnectionType.TCP_TLS || connectionType == ConnectionType.TCP_TLS_TRUST) && mConnectionInfo != null)
		{
			CertificateInfo certificateInfo = mConnectionInfo.getCertificateInfo();
			if (certificateInfo != null)
			{
				mPPSCRA.loadClientCertificate(certificateInfo.getFormat(), certificateInfo.getData(), certificateInfo.getPassword());
			}
		}
	}

	public override IDeviceCapabilities getCapabilities()
	{
		IDeviceCapabilities result = null;
		if (mPPSCRA != null)
		{
			result = new PPSCRADeviceCapabilities(TransactionBuilder.GetPaymentMethods(msr: true, contact: true, contactless: true, manual: true), display: true, pinPad: true, signature: true, autoSignatureCapture: true, sred: true);
		}
		return result;
	}

	internal static string GetConnectionTypeURI(ConnectionType connectionType)
	{
		string result = "";
		if (ConnectionTypeURI.ContainsKey(connectionType))
		{
			result = ConnectionTypeURI[connectionType];
		}
		return result;
	}

	protected string getDevicePath()
	{
		string result = "";
		if (mConnectionInfo != null)
		{
			ConnectionType connectionType = mConnectionInfo.getConnectionType();
			result = string.Concat(str1: mConnectionInfo.getAddress(), str0: GetConnectionTypeURI(connectionType));
		}
		return result;
	}

	public override IDeviceControl getDeviceControl()
	{
		if (mDeviceControl == null && mConnectionInfo != null)
		{
			mDeviceControl = new PPSCRADeviceControl(this, mPPSCRA, getDevicePath(), mConnectionInfo.getCertificateInfo());
		}
		return mDeviceControl;
	}

	public override IDeviceConfiguration getDeviceConfiguration()
	{
		if (mDeviceConfiguration == null)
		{
			mDeviceConfiguration = new PPSCRADeviceConfiguration(mPPSCRA, getDevicePath());
		}
		return mDeviceConfiguration;
	}

	public override bool startTransaction(ITransaction transaction)
	{
		bool result = true;
		if (mPPSCRA != null && transaction != null)
		{
			Task.Factory.StartNew((Func<Task>)async delegate
			{
				if (checkConnectedDevice())
				{
					IData data = new BaseData("Please follow instruction shown on the reader");
					sendEvent(EventType.DisplayMessage, data);
					await Task.Delay(500);
					List<PaymentMethod> paymentMethods = transaction.PaymentMethods;
					if (paymentMethods != null)
					{
						if (paymentMethods.Count == 1 && paymentMethods[0] == PaymentMethod.ManualEntry)
						{
							result = startManualEntry(transaction);
						}
						else if (paymentMethods.Count == 1 && paymentMethods[0] == PaymentMethod.MSR && !transaction.EMVOnly)
						{
							result = startMSRTransaction();
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

	protected bool startManualEntry(ITransaction transaction)
	{
		if (mPPSCRA != null)
		{
			mPPSCRA.endSession(0);
			int num = 30;
			int num2 = 1;
			int num3 = 0;
			int num4 = 0;
			if (mPPSCRA.requestManualCardData(num, num2, num3, ref num4) != 0)
			{
				updateTransactionStatusValue(TransactionStatus.TransactionFailed);
			}
		}
		return true;
	}

	protected bool startMSRTransaction()
	{
		if (mPPSCRA != null)
		{
			mPPSCRA.endSession(0);
			int num = 30;
			int num2 = 1;
			int num3 = 1;
			if (mPPSCRA.requestCard(num, num2, num3, "|") != 0)
			{
				updateTransactionStatusValue(TransactionStatus.TransactionFailed);
			}
		}
		return true;
	}

	protected bool startEMVTransaction(ITransaction transaction)
	{
		if (mPPSCRA != null)
		{
			int cardTypeValue = getCardTypeValue(transaction.PaymentMethods);
			int num = 30;
			int num2 = 30;
			int num3 = 1;
			int num4 = 0;
			byte b = 4;
			byte[] n12Bytes = BaseDevice.GetN12Bytes(transaction.Amount);
			byte[] n12Bytes2 = BaseDevice.GetN12Bytes(transaction.CashBack);
			byte[] array = new byte[45];
			array[15] = (byte)(transaction.QuickChip ? 1u : 0u);
			if (mPPSCRA.requestSmartCard(cardTypeValue, num, num2, num3, num4, n12Bytes, (int)b, n12Bytes2, array) != 0)
			{
				updateTransactionStatusValue(TransactionStatus.TransactionFailed);
			}
		}
		return true;
	}

	public override bool cancelTransaction()
	{
		if (mPPSCRA != null)
		{
			try
			{
				mPPSCRA.cancelOperation();
				mPPSCRA.endSession(0);
			}
			catch (Exception)
			{
			}
		}
		return true;
	}

	public override bool sendAuthorization(IData data)
	{
		if (mPPSCRA != null)
		{
			byte[] byteArray = data.ByteArray;
			mPPSCRA.sendAcquirerResponse(byteArray, byteArray.Length);
		}
		return true;
	}

	public override bool requestPIN(PINRequest pinRequest)
	{
		if (mPPSCRA != null)
		{
			Task.Factory.StartNew((Func<Task>)async delegate
			{
				if (checkConnectedDevice())
				{
					IData data = new BaseData("Please follow instruction shown on the reader");
					sendEvent(EventType.DisplayMessage, data);
					await Task.Delay(500);
					int num = 1;
					if (mPPSCRA.requestPIN((int)pinRequest.Timeout, (int)pinRequest.PINMode, (int)pinRequest.MinLength, (int)pinRequest.MaxLength, (int)pinRequest.Tone, num, "|") != 0)
					{
						sendFeatureStatusEvent(DeviceFeature.PINEntry, FeatureStatus.Failed);
					}
				}
			});
		}
		return true;
	}

	public override bool requestSignature(byte timeout)
	{
		if (mPPSCRA != null)
		{
			Task.Factory.StartNew((Func<Task>)async delegate
			{
				if (checkConnectedDevice())
				{
					IData data = new BaseData("Please follow instruction shown on the reader");
					sendEvent(EventType.DisplayMessage, data);
					await Task.Delay(500);
					int num = 1;
					int num2 = 1;
					if (mPPSCRA.requestSignature((int)timeout, num, num2) != 0)
					{
						sendFeatureStatusEvent(DeviceFeature.SignatureCapture, FeatureStatus.Failed);
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
					b |= 4;
					break;
				}
			}
		}
		return b;
	}

	protected void OnDeviceConnectionStateChanged(MTConnectionState state)
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
			break;
		case 4:
			connectionStateValue = ConnectionState.Disconnecting;
			break;
		case 0:
			connectionStateValue = ConnectionState.Disconnected;
			break;
		}
		updateConnectionStateValue(connectionStateValue);
	}

	public void OnDataReady(byte[] data)
	{
		IData data2 = new BaseData(TLVParser.getHexString(data));
		sendEvent(EventType.DeviceResponse, data2);
		if (data != null && mDeviceConfiguration != null)
		{
			try
			{
				((PPSCRADeviceConfiguration)mDeviceConfiguration).processDataReady(data);
			}
			catch (Exception)
			{
			}
		}
	}

	protected void OnEMVDataComplete(byte opStatus, byte[] data)
	{
		switch (opStatus)
		{
		case 1:
			updateTransactionStatusValue(TransactionStatus.TransactionCancelled, "Transaction Cancelled (User cancel)", "");
			break;
		case 2:
			updateTransactionStatusValue(TransactionStatus.TimedOut, "Timed Out", "");
			break;
		case 3:
			updateTransactionStatusValue(TransactionStatus.TransactionCancelled, "Transaction Cancelled", "");
			break;
		}
		if (data != null && data.Length >= 2)
		{
			byte[] array = new byte[data.Length];
			Array.Copy(data, 0, array, 0, data.Length);
			IData data2 = new BaseData(array);
			sendEvent(EventType.AuthorizationRequest, data2);
		}
	}

	protected void OnEmvTransactionComplete(byte opStatus, byte[] data)
	{
		switch (opStatus)
		{
		case 1:
			updateTransactionStatusValue(TransactionStatus.TransactionCancelled, "Transaction Cancelled (User cancel)", "");
			break;
		case 2:
			updateTransactionStatusValue(TransactionStatus.TimedOut, "Timed Out", "");
			break;
		case 3:
			updateTransactionStatusValue(TransactionStatus.TransactionCancelled, "Transaction Cancelled", "");
			break;
		}
		if (data != null && data.Length >= 2)
		{
			byte[] array = new byte[data.Length];
			Array.Copy(data, 0, array, 0, data.Length);
			IData data2 = new BaseData(array);
			sendEvent(EventType.TransactionResult, data2);
		}
		if (mPPSCRA != null)
		{
			mPPSCRA.endSession(0);
		}
	}

	protected void OnCardRequestComplete(string cardData)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		CARD_DATA cardDataInfo = mPPSCRA.getCardDataInfo();
		switch (cardDataInfo.CardOperationStatus)
		{
		case 1:
			updateTransactionStatusValue(TransactionStatus.TransactionCancelled, "Transaction Cancelled (User cancel)", "");
			break;
		case 2:
			updateTransactionStatusValue(TransactionStatus.TimedOut, "Timed Out", "");
			break;
		case 3:
			updateTransactionStatusValue(TransactionStatus.HostCancelled, "Host Cancelled", "");
			break;
		}
		IData data = new BaseData(PPSCRACardDataBuilder.buildTLVPayload(cardDataInfo));
		sendEvent(EventType.CardData, data);
		if (mPPSCRA != null)
		{
			mPPSCRA.endSession(0);
		}
	}

	protected void OnPinRequestComplete(string pinData)
	{
		if (pinData != null)
		{
			try
			{
				if (pinData.Contains(","))
				{
					string[] array = pinData.Split(new char[1] { ',' });
					if (array.Length > 2)
					{
						switch (short.Parse(array[2]))
						{
						case 1:
							sendFeatureStatusEvent(DeviceFeature.PINEntry, FeatureStatus.Cancelled);
							break;
						case 2:
							sendFeatureStatusEvent(DeviceFeature.PINEntry, FeatureStatus.TimedOut);
							break;
						case 3:
							sendFeatureStatusEvent(DeviceFeature.PINEntry, FeatureStatus.Cancelled);
							break;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}
		IData data = new BaseData(Encoding.UTF8.GetBytes(pinData));
		sendEvent(EventType.PINData, data);
		if (mPPSCRA != null)
		{
			mPPSCRA.endSession(0);
		}
	}

	protected void OnSignatureArrived(byte opStatus, byte[] sigData)
	{
		switch (opStatus)
		{
		case 1:
			sendFeatureStatusEvent(DeviceFeature.SignatureCapture, FeatureStatus.Cancelled);
			break;
		case 2:
			sendFeatureStatusEvent(DeviceFeature.SignatureCapture, FeatureStatus.TimedOut);
			break;
		case 3:
			sendFeatureStatusEvent(DeviceFeature.SignatureCapture, FeatureStatus.Cancelled);
			break;
		}
		IData data = new BaseData(sigData);
		sendEvent(EventType.Signature, data);
		if (mPPSCRA != null)
		{
			mPPSCRA.endSession(0);
		}
	}
}
