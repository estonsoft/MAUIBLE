// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSDevice
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



public class MMSDevice : BaseDevice, IConfigurationCallback
{
	private MMXDevice mMMXDevice;

	public MMSDevice(ConnectionInfo connectionInfo)
	{
		init(connectionInfo, new DeviceInfo());
	}

	public MMSDevice(ConnectionInfo connectionInfo, DeviceInfo deviceInfo)
	{
		init(connectionInfo, deviceInfo);
	}

	protected override void init(ConnectionInfo connectionInfo, DeviceInfo deviceInfo)
	{
		mConnectionInfo = connectionInfo;
		mDeviceInfo = deviceInfo;
	}

	protected static MMXConnectionType getMMXConnectionType(ConnectionType connectionType)
	{
		MMXConnectionType result = MMXConnectionType.USB;
		switch (connectionType)
		{
		case ConnectionType.USB:
			result = MMXConnectionType.USB;
			break;
		case ConnectionType.BLUETOOTH_LE_EMV:
			result = MMXConnectionType.BLE;
			break;
		case ConnectionType.TCP:
			result = MMXConnectionType.TCP;
			break;
		case ConnectionType.WEBSOCKET:
			result = MMXConnectionType.WEBSOCKET;
			break;
		case ConnectionType.SERIAL:
			result = MMXConnectionType.Serial;
			break;
		}
		return result;
	}

	private MMXDevice getMMXDevice()
	{
		if (mMMXDevice == null)
		{
			ConnectionType connectionType = ConnectionType.USB;
			if (mConnectionInfo != null)
			{
				connectionType = mConnectionInfo.getConnectionType();
				mConnectionInfo.getAddress();
			}
			mMMXDevice = new MMXDevice();
			updateDeviceControl();
			updateDeviceConfiguration();
			mMMXDevice.onConnectionStateChanged += OnConnectionStateChanged;
			mMMXDevice.onMessage += OnMessage;
			mMMXDevice.setConnectionType(getMMXConnectionType(connectionType));
		}
		return mMMXDevice;
	}

	public override IDeviceCapabilities getCapabilities()
	{
		return new MMSDeviceCapabilities(TransactionBuilder.GetPaymentMethods(msr: true, contact: true, contactless: true, manual: true), display: true, pinPad: false, signature: true, autoSignatureCapture: true, sred: true);
	}

	public override IDeviceControl getDeviceControl()
	{
		if (mDeviceControl == null)
		{
			string path = "";
			if (mConnectionInfo != null)
			{
				mConnectionInfo.getConnectionType();
				path = mConnectionInfo.getAddress();
			}
			mDeviceControl = new MMSDeviceControl(getMMXDevice(), path);
		}
		return mDeviceControl;
	}

	public override IDeviceConfiguration getDeviceConfiguration()
	{
		if (mDeviceConfiguration == null)
		{
			mDeviceConfiguration = new MMSDeviceConfiguration(getMMXDevice());
		}
		return mDeviceConfiguration;
	}

	private void updateDeviceControl()
	{
		string path = "";
		if (mConnectionInfo != null)
		{
			mConnectionInfo.getConnectionType();
			path = mConnectionInfo.getAddress();
		}
		mDeviceControl = new MMSDeviceControl(mMMXDevice, path);
	}

	private void updateDeviceConfiguration()
	{
		mDeviceConfiguration = new MMSDeviceConfiguration(mMMXDevice);
	}

	protected void sendCommand(Command command)
	{
		Message message = MessageBuilder.BuildMessage();
		message.addMessageInfoForCommand(command.getTagByteArray());
		message.addPayload(command.getByteArray());
		byte[] byteArray = message.getByteArray();
		if (byteArray != null)
		{
			TLVParser.getHexString(byteArray);
			MMXMessage message2 = new MMXMessage(48, byteArray);
			getMMXDevice().sendMessage(message2);
		}
	}

	public override bool startTransaction(ITransaction transaction)
	{
		bool result = true;
		if (getMMXDevice() != null && transaction != null)
		{
			Task.Factory.StartNew((Func<Task>)async delegate
			{
				if (checkConnectedDevice())
				{
					await Task.Delay(100);
					result = startEMVTransaction(transaction);
				}
			});
		}
		return true;
	}

	public override bool requestPIN(PINRequest pinRequest)
	{
		if (mMMXDevice != null)
		{
			Task.Factory.StartNew((Func<Task>)async delegate
			{
				if (checkConnectedDevice())
				{
					await Task.Delay(100);
					string pAN = pinRequest.PAN;
					byte[] pan = null;
					if (pAN != null && pAN.Length > 0)
					{
						pan = TLVParser.getByteArrayFromHexString(pAN.PadRight(12, '0'));
					}
					Command command = MMSCommandBuilder.requestPINCommand(pinRequest.Timeout, pinRequest.PINMode, pinRequest.MinLength, pinRequest.MaxLength, pan, pinRequest.Format);
					sendCommand(command);
				}
			});
		}
		return true;
	}

	public override bool requestPAN(PANRequest panRequest, PINRequest pinRequest)
	{
		if (mMMXDevice != null)
		{
			Task.Factory.StartNew((Func<Task>)async delegate
			{
				if (checkConnectedDevice())
				{
					await Task.Delay(100);
					byte timeout = panRequest.Timeout;
					byte msrMode = 0;
					byte contactMode = 0;
					byte contactlessMode = 0;
					List<PaymentMethod> paymentMethods = panRequest.PaymentMethods;
					if (paymentMethods != null)
					{
						msrMode = (byte)(paymentMethods.Contains(PaymentMethod.MSR) ? 1 : 0);
						contactMode = (byte)(paymentMethods.Contains(PaymentMethod.Contact) ? 1 : 0);
						contactlessMode = (byte)(paymentMethods.Contains(PaymentMethod.Contactless) ? 1 : 0);
					}
					if (pinRequest != null)
					{
						Command command = MMSCommandBuilder.requestPANCommand(timeout, msrMode, contactMode, contactlessMode, pinRequest.PINMode, pinRequest.MinLength, pinRequest.MaxLength, pinRequest.Format);
						sendCommand(command);
					}
					else
					{
						Command command2 = MMSCommandBuilder.requestPANCommand(timeout, msrMode, contactMode, contactlessMode);
						sendCommand(command2);
					}
				}
			});
		}
		return true;
	}

	public override bool requestSignature(byte timeout)
	{
		if (mMMXDevice != null)
		{
			Task.Factory.StartNew((Func<Task>)async delegate
			{
				if (checkConnectedDevice())
				{
					await Task.Delay(500);
					Command command = MMSCommandBuilder.RequestSignatureCommand(timeout);
					sendCommand(command);
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
				case PaymentMethod.ManualEntry:
					b |= 8;
					break;
				}
			}
		}
		return b;
	}

	protected bool startEMVTransaction(ITransaction transaction)
	{
		byte timeout = transaction.Timeout;
		byte msrMOde = 0;
		byte contactMode = 0;
		byte contactlessMode = 0;
		byte[] manualMode = null;
		byte[] array = new byte[2];
		if (transaction.QuickChip)
		{
			array[1] |= 1;
		}
		if (transaction.EMVResponseFormat == 0)
		{
			array[1] |= 2;
		}
		if (transaction.PreventMSRSignatureForCardWithICC)
		{
			array[1] |= 4;
		}
		byte[] transactionTLVUpdate = null;
		if (transaction != null)
		{
			List<PaymentMethod> paymentMethods = transaction.PaymentMethods;
			if (paymentMethods != null)
			{
				if (paymentMethods.Contains(PaymentMethod.ManualEntry))
				{
					manualMode = new byte[3] { transaction.ManualEntryFormat, transaction.ManualEntryType, transaction.ManualEntrySound };
				}
				else
				{
					msrMOde = (byte)((!transaction.EMVOnly) ? (paymentMethods.Contains(PaymentMethod.MSR) ? 128 : 0) : (paymentMethods.Contains(PaymentMethod.MSR) ? 1 : 0));
					contactMode = (byte)(paymentMethods.Contains(PaymentMethod.Contact) ? 1 : 0);
					contactlessMode = (byte)(paymentMethods.Contains(PaymentMethod.Contactless) ? 1 : 0);
				}
			}
			TLVObject tLVObject = new TLVObject(164);
			tLVObject.addTLVObject(new TLVObject(156, transaction.TransactionType));
			tLVObject.addTLVObject(new TLVObject(TLVParser.getByteArrayFromHexString("9F02"), BaseDevice.GetN12Bytes(transaction.Amount)));
			tLVObject.addTLVObject(new TLVObject(TLVParser.getByteArrayFromHexString("9F03"), BaseDevice.GetN12Bytes(transaction.CashBack)));
			if (transaction.CurrencyCode != null)
			{
				tLVObject.addTLVObject(new TLVObject(TLVParser.getByteArrayFromHexString("5F2A"), transaction.CurrencyCode));
			}
			if (transaction.CurrencyExponent != null)
			{
				tLVObject.addTLVObject(new TLVObject(TLVParser.getByteArrayFromHexString("5F36"), transaction.CurrencyExponent));
			}
			if (transaction.TransactionCategory != null)
			{
				tLVObject.addTLVObject(new TLVObject(TLVParser.getByteArrayFromHexString("9F53"), transaction.TransactionCategory));
			}
			if (transaction.MerchantCategory != null)
			{
				tLVObject.addTLVObject(new TLVObject(TLVParser.getByteArrayFromHexString("9F15"), transaction.MerchantCategory));
			}
			if (transaction.MerchantID != null)
			{
				tLVObject.addTLVObject(new TLVObject(TLVParser.getByteArrayFromHexString("9F16"), transaction.MerchantID));
			}
			if (transaction.MerchantCustomData != null)
			{
				tLVObject.addTLVObject(new TLVObject(TLVParser.getByteArrayFromHexString("9F7C"), transaction.MerchantCustomData));
			}
			transactionTLVUpdate = tLVObject.getValueByteArray();
		}
		if (!transaction.SuppressThankYouMessage)
		{
			_ = transaction.OverrideFinalTransactionMessage;
		}
		Command command = MMSCommandBuilder.InitiateTransactionCommand(timeout, msrMOde, contactMode, contactlessMode, manualMode, array, transactionTLVUpdate, transaction.SuppressThankYouMessage, transaction.OverrideFinalTransactionMessage);
		sendCommand(command);
		return true;
	}

	public override bool cancelTransaction()
	{
		Command command = MMSCommandBuilder.CancelTransactionCommand();
		sendCommand(command);
		return true;
	}

	public override bool sendSelection(IData data)
	{
		if (data != null)
		{
			byte[] byteArray = data.ByteArray;
			Command command = MMSCommandBuilder.ReportSelection(byteArray[0], byteArray[1]);
			sendCommand(command);
		}
		return true;
	}

	public override bool sendAuthorization(IData data)
	{
		if (data != null)
		{
			Command command = MMSCommandBuilder.ResumeTransactionCommand(data.ByteArray);
			sendCommand(command);
		}
		return true;
	}

	protected void OnConnectionStateChanged(object sender, MMXConnectionState state)
	{
		ConnectionState connectionStateValue = ConnectionState.Unknown;
		switch (state)
		{
		case MMXConnectionState.Connecting:
			connectionStateValue = ConnectionState.Connecting;
			break;
		case MMXConnectionState.Connected:
			connectionStateValue = ConnectionState.Connected;
			break;
		case MMXConnectionState.Disconnecting:
			connectionStateValue = ConnectionState.Disconnecting;
			break;
		case MMXConnectionState.Disconnected:
			connectionStateValue = ConnectionState.Disconnected;
			break;
		}
		updateConnectionStateValue(connectionStateValue);
	}

	protected void OnMessage(object sender, byte[] data)
	{
		string hexString = TLVParser.getHexString(data);
		Message message = MessageParser.GetMessage(data);
		if (message == null)
		{
			return;
		}
		if (message.isResponse())
		{
			IData data2 = new BaseData(hexString);
			sendEvent(EventType.DeviceResponse, data2);
			processResponse(message);
			if (mDeviceConfiguration != null)
			{
				try
				{
					((MMSDeviceConfiguration)mDeviceConfiguration).processResponse(message);
				}
				catch (Exception)
				{
				}
			}
		}
		else if (message.isNotification())
		{
			IData data3 = new BaseData(hexString);
			sendEvent(EventType.DeviceNotification, data3);
			processNotification(message);
			if (mDeviceConfiguration != null)
			{
				try
				{
					((MMSDeviceConfiguration)mDeviceConfiguration).processNotification(message);
				}
				catch (Exception)
				{
				}
			}
		}
		else
		{
			if (!message.isDataFile())
			{
				return;
			}
			IData data4 = new BaseData(hexString);
			sendEvent(EventType.DeviceDataFile, data4);
			if (mDeviceConfiguration != null)
			{
				try
				{
					((MMSDeviceConfiguration)mDeviceConfiguration).processDataFile(message);
				}
				catch (Exception)
				{
				}
			}
		}
	}

	protected void sendOperationStatus(OperationStatus statusValue, string opeationDetail, string statusDetail, string deviceDetail)
	{
		IData data = new BaseData(OperationStatusBuilder.GetString(statusValue) + "," + opeationDetail + "," + statusDetail + "," + deviceDetail);
		sendEvent(EventType.OperationStatus, data);
	}

	protected void processResponse(Message message)
	{
		MMSResponse mMSResponse = new MMSResponse(message);
		if (mMSResponse == null)
		{
			return;
		}
		byte[] requestID = mMSResponse.getRequestID();
		if (requestID == null || requestID.Length != 2)
		{
			return;
		}
		byte[] statusCodeValue = mMSResponse.getStatusCodeValue();
		if (statusCodeValue != null && statusCodeValue.Length >= 4)
		{
			byte b = statusCodeValue[0];
			string hexString = TLVParser.getHexString(statusCodeValue);
			string hexString2 = TLVParser.getHexString(requestID);
			switch (b)
			{
			case 0:
				sendOperationStatus(OperationStatus.Done, hexString2, "Operation Done", hexString);
				break;
			case 1:
				sendOperationStatus(OperationStatus.Started, hexString2, "Operation Started", hexString);
				break;
			case 64:
				sendOperationStatus(OperationStatus.Warning, hexString2, "Operation Warning", hexString);
				sendOperationStatus(OperationStatus.Done, hexString2, "Operation Done", hexString);
				break;
			case 65:
				sendOperationStatus(OperationStatus.Started, hexString2, "Operation Started", hexString);
				sendOperationStatus(OperationStatus.Warning, hexString2, "Operation Warning", hexString);
				break;
			case 128:
				sendOperationStatus(OperationStatus.Failed, hexString2, "Operation Failed", hexString);
				break;
			case 129:
				sendOperationStatus(OperationStatus.Started, hexString2, "Operation Running", hexString);
				sendOperationStatus(OperationStatus.Failed, hexString2, "Operation Failed", hexString);
				break;
			}
		}
		if (requestID[0] != 16 || requestID[1] != 17)
		{
			return;
		}
		ResponsePayload responsePayload = MessageParser.GetResponsePayload(mMSResponse.getPayload());
		if (responsePayload == null)
		{
			return;
		}
		byte b2 = 0;
		byte[] paramValue = responsePayload.getParamValue("81");
		if (paramValue != null && paramValue.Length != 0)
		{
			b2 = paramValue[0];
		}
		byte[] payloadValue = responsePayload.getPayloadValue();
		if (payloadValue != null)
		{
			switch (b2)
			{
			case 2:
				sendAuthorizationRequestEvent(payloadValue);
				break;
			case 3:
				sendTransactionResultEvent(payloadValue);
				break;
			}
		}
	}

	protected void sendAuthorizationRequestEvent(byte[] data)
	{
		if (data != null && data.Length >= 2)
		{
			byte[] array = new byte[data.Length];
			Array.Copy(data, 0, array, 0, data.Length);
			IData data2 = new BaseData(array);
			sendEvent(EventType.AuthorizationRequest, data2);
		}
	}

	protected void sendTransactionResultEvent(byte[] data)
	{
		if (data != null && data.Length >= 2)
		{
			byte[] array = new byte[data.Length];
			Array.Copy(data, 0, array, 0, data.Length);
			IData data2 = new BaseData(array);
			sendEvent(EventType.TransactionResult, data2);
		}
	}

	private byte[] getTLVPayload(byte[] data)
	{
		byte[] array = null;
		if (data != null && data.Length > 2)
		{
			int num = ((data[0] & 0xFF) << 8) + (data[1] & 0xFF);
			array = new byte[num];
			Array.Copy(data, 2, array, 0, num);
		}
		return array;
	}

	public void checkResultForSignatureData(byte[] data)
	{
		if (data.Length <= 0)
		{
			return;
		}
		List<TLVObject> list = TLVParser.parseTLVByteArray(getTLVPayload(data));
		if (list == null)
		{
			return;
		}
		TLVObject tLVObject = TLVParser.findFromListByTagHexString(list, "DFDF3E");
		if (tLVObject != null)
		{
			byte[] valueByteArray = tLVObject.getValueByteArray();
			if (valueByteArray != null)
			{
				int num = valueByteArray.Length;
				byte[] array = new byte[num];
				Array.Copy(valueByteArray, 0, array, 0, num);
				IData data2 = new BaseData(array);
				sendEvent(EventType.Signature, data2);
			}
		}
	}

	private void processSignatureData(byte[] sigValues)
	{
		if (sigValues != null)
		{
			int num = sigValues.Length;
			byte[] array = new byte[num];
			Array.Copy(sigValues, 0, array, 0, num);
			IData data = new BaseData(array);
			sendEvent(EventType.Signature, data);
		}
	}

	public void OnProgress(int progress)
	{
	}

	public void OnResult(StatusCode status, byte[] data)
	{
		int num = data.Length - 8;
		if (num <= 0)
		{
			return;
		}
		byte[] array = new byte[num];
		Array.Copy(data, 8, array, 0, num);
		List<TLVObject> list = TLVParser.parseTLVByteArray(array);
		if (list == null)
		{
			return;
		}
		TLVObject tLVObject = TLVParser.findFromListByTagHexString(list, "CE");
		if (tLVObject == null)
		{
			return;
		}
		List<TLVObject> list2 = TLVParser.parseTLVByteArray(tLVObject.getValueByteArray());
		if (list2 == null)
		{
			return;
		}
		TLVParser.findFromListByTagHexString(list2, "81")?.getValueByteArray();
		TLVObject tLVObject2 = TLVParser.findFromListByTagHexString(list2, "82");
		if (tLVObject2 == null)
		{
			return;
		}
		byte[] valueByteArray = tLVObject2.getValueByteArray();
		if (valueByteArray != null)
		{
			int num2 = valueByteArray.Length / 2;
			byte[] array2 = new byte[num2];
			for (int i = 0; i < num2; i++)
			{
				array2[i] = valueByteArray[2 * i + 1];
			}
			IData data2 = new BaseData(array2);
			sendEvent(EventType.Signature, data2);
		}
	}

	public IResult OnCalculateMAC(byte macType, byte[] data)
	{
		return new Result(StatusCode.UNAVAILABLE);
	}

	protected void processNotification(Message message)
	{
		MMSNotification mMSNotification = new MMSNotification(message);
		if (mMSNotification == null)
		{
			return;
		}
		byte[] notificationID = mMSNotification.getNotificationID();
		byte[] notificationCode = mMSNotification.getNotificationCode();
		byte b = notificationID[0];
		byte b2 = notificationID[1];
		string hexString = TLVParser.getHexString(message.getByteArray());
		switch (b)
		{
		case 1:
		{
			byte b9 = notificationCode[0];
			byte b10 = notificationCode[1];
			byte b11 = notificationCode[2];
			byte b12 = notificationCode[3];
			switch (b2)
			{
			case 1:
				switch (b10)
				{
				case 1:
					switch (b11)
					{
					case 1:
						switch (b9)
						{
						case 8:
							updateTransactionStatusValue(TransactionStatus.CardSwiped, "Card Swiped", hexString);
							break;
						case 7:
							updateTransactionStatusValue(TransactionStatus.DataEntered, "Data Entered", hexString);
							break;
						}
						break;
					case 2:
						updateTransactionStatusValue(TransactionStatus.CardInserted, "Card Inserted", hexString);
						break;
					case 3:
						updateTransactionStatusValue(TransactionStatus.CardRemoved, "Card Removed", hexString);
						break;
					case 4:
						updateTransactionStatusValue(TransactionStatus.CardDetected, "Card Detected", hexString);
						break;
					case 5:
						updateTransactionStatusValue(TransactionStatus.CardCollision, "Card Collision", hexString);
						break;
					}
					break;
				case 8:
					switch (b11)
					{
					case 2:
						switch (b12)
						{
						case 1:
							requestARQC();
							break;
						case 2:
						{
							byte[] payloadValue3 = MessageParser.GetNotificationPayload(mMSNotification.getPayload()).getPayloadValue();
							sendAuthorizationRequestEvent(payloadValue3);
							break;
						}
						}
						break;
					case 3:
						switch (b12)
						{
						case 1:
							requestBatchData();
							break;
						case 2:
						{
							byte[] payloadValue2 = MessageParser.GetNotificationPayload(mMSNotification.getPayload()).getPayloadValue();
							sendTransactionResultEvent(payloadValue2);
							checkResultForSignatureData(payloadValue2);
							break;
						}
						}
						break;
					}
					break;
				default:
					_ = 16;
					break;
				}
				break;
			case 5:
				switch (b10)
				{
				case 1:
					updateTransactionStatusValue(TransactionStatus.TimedOut, "Timed Out", hexString);
					break;
				case 2:
					switch (b11)
					{
					case 0:
						if (b12 == 2)
						{
							updateTransactionStatusValue(TransactionStatus.TechnicalFallback, "Technical Fallback", hexString);
						}
						break;
					case 1:
						updateTransactionStatusValue(TransactionStatus.TryAnotherInterface, "Try Another Interface", hexString);
						break;
					case 15:
						switch (b12)
						{
						case 2:
							updateTransactionStatusValue(TransactionStatus.TechnicalFallback, "Technical Fallback", hexString);
							break;
						case 0:
							updateTransactionStatusValue(TransactionStatus.TechnicalFallback, "Technical Fallback", hexString);
							break;
						}
						break;
					case 2:
						updateTransactionStatusValue(TransactionStatus.TransactionApproved, "Transaction Approved (Offline)", hexString);
						if (b12 == 1)
						{
							updateTransactionStatusValue(TransactionStatus.SignatureCaptureRequested, "Signature Capture Requested", hexString);
						}
						break;
					case 3:
						updateTransactionStatusValue(TransactionStatus.TransactionDeclined, "Transaction Declined (Offline)", hexString);
						break;
					case 4:
						updateTransactionStatusValue(TransactionStatus.TransactionFailed, "Transaction Failed (Offline)", hexString);
						break;
					case 5:
						updateTransactionStatusValue(TransactionStatus.TransactionNotAccepted, "Transaction Not Accepted  (Offline)", hexString);
						break;
					case 6:
						updateTransactionStatusValue(TransactionStatus.TransactionApproved, "Transaction Approved", hexString);
						if (b12 == 1)
						{
							updateTransactionStatusValue(TransactionStatus.SignatureCaptureRequested, "Signature Capture Requested", hexString);
						}
						break;
					case 7:
						updateTransactionStatusValue(TransactionStatus.QuickChipDeferred, "Quick Chip Deferred", hexString);
						if (b12 == 1)
						{
							updateTransactionStatusValue(TransactionStatus.SignatureCaptureRequested, "Signature Capture Requested", hexString);
						}
						break;
					case 8:
						updateTransactionStatusValue(TransactionStatus.TransactionFailed, "Transaction Failed", hexString);
						break;
					case 9:
						updateTransactionStatusValue(TransactionStatus.TransactionNotAccepted, "Transaction Not Accepted", hexString);
						break;
					case 10:
						updateTransactionStatusValue(TransactionStatus.TransactionCancelled, "Transaction Cancelled", hexString);
						break;
					}
					break;
				case 3:
					switch (b11)
					{
					case 1:
						updateTransactionStatusValue(TransactionStatus.HostCancelled, "Host Cancelled", hexString);
						break;
					case 2:
						updateTransactionStatusValue(TransactionStatus.TransactionCancelled, "Transaction Cancelled (User cancel)", hexString);
						break;
					case 5:
						updateTransactionStatusValue(TransactionStatus.TransactionCancelled, "Transaction Cancelled (Cancel due to card read error)", hexString);
						break;
					}
					break;
				case 4:
					switch (b11)
					{
					case 0:
						updateTransactionStatusValue(TransactionStatus.TransactionCompleted, "Transaction Completed", hexString);
						if (b12 == 1)
						{
							updateTransactionStatusValue(TransactionStatus.SignatureCaptureRequested, "Signature Capture Requested", hexString);
						}
						break;
					case 7:
						updateTransactionStatusValue(TransactionStatus.TransactionDeclined, "Transaction Declined", hexString);
						break;
					}
					break;
				}
				break;
			}
			break;
		}
		case 2:
		{
			byte b13 = notificationCode[0];
			byte b14 = notificationCode[1];
			byte b15 = notificationCode[2];
			byte b16 = notificationCode[3];
			switch (b2)
			{
			case 1:
				if (b14 != 1)
				{
					break;
				}
				switch (b15)
				{
				case 1:
					switch (b13)
					{
					case 8:
						updateTransactionStatusValue(TransactionStatus.CardSwiped, "Card Swiped", hexString);
						break;
					case 7:
						updateTransactionStatusValue(TransactionStatus.DataEntered, "Data Entered", hexString);
						break;
					}
					break;
				case 2:
					updateTransactionStatusValue(TransactionStatus.CardInserted, "Card Inserted", hexString);
					break;
				case 3:
					updateTransactionStatusValue(TransactionStatus.CardRemoved, "Card Removed", hexString);
					break;
				case 4:
					updateTransactionStatusValue(TransactionStatus.CardDetected, "Card Detected", hexString);
					break;
				case 5:
					updateTransactionStatusValue(TransactionStatus.CardCollision, "Card Collision", hexString);
					break;
				}
				break;
			case 5:
				if (b13 != 1 || b14 != 2)
				{
					break;
				}
				switch (b15)
				{
				case 1:
				{
					if (b16 != 1)
					{
						break;
					}
					NotificationPayload notificationPayload3 = MessageParser.GetNotificationPayload(mMSNotification.getPayload());
					if (notificationPayload3 == null)
					{
						break;
					}
					byte[] payloadValue4 = notificationPayload3.getPayloadValue();
					if (payloadValue4 == null)
					{
						break;
					}
					List<TLVObject> list = TLVParser.parseTLVByteArray(payloadValue4);
					if (list != null)
					{
						TLVObject tLVObject = TLVParser.findFromListByTagHexString(list, "DFDF59");
						if (tLVObject != null && tLVObject.getValueByteArray() != null)
						{
							IData data4 = new BaseData(payloadValue4);
							sendEvent(EventType.PANData, data4);
						}
					}
					IData data5 = new BaseData(payloadValue4);
					sendEvent(EventType.PINData, data5);
					break;
				}
				case 2:
					switch (b16)
					{
					case 1:
						sendFeatureStatusEvent(DeviceFeature.PINEntry, FeatureStatus.TimedOut);
						break;
					case 2:
						sendFeatureStatusEvent(DeviceFeature.PINEntry, FeatureStatus.HardwareNA);
						break;
					case 3:
						sendFeatureStatusEvent(DeviceFeature.PINEntry, FeatureStatus.Cancelled);
						break;
					case 4:
						sendFeatureStatusEvent(DeviceFeature.PINEntry, FeatureStatus.Error);
						break;
					case 5:
						sendFeatureStatusEvent(DeviceFeature.PINEntry, FeatureStatus.Failed);
						break;
					case 6:
						sendFeatureStatusEvent(DeviceFeature.PANEntry, FeatureStatus.Failed);
						break;
					}
					break;
				}
				break;
			}
			break;
		}
		case 16:
		{
			byte b7 = notificationCode[0];
			byte b8 = notificationCode[1];
			_ = notificationCode[2];
			_ = notificationCode[3];
			if (b2 != 1)
			{
				break;
			}
			switch (b7)
			{
			case 0:
			{
				string eventDetail = "";
				byte[] payload2 = mMSNotification.getPayload();
				if (payload2 != null)
				{
					eventDetail = TLVParser.getHexString(payload2);
				}
				switch (b8)
				{
				case 0:
					sendDeviceEvent(DeviceEvent.DeviceResetOccurred, eventDetail);
					break;
				case 1:
					sendDeviceEvent(DeviceEvent.DeviceResetWillOccur, eventDetail);
					break;
				}
				break;
			}
			case 1:
				switch (b8)
				{
				case 0:
					sendUserEvent(UserEvent.ContactlessCardPresented);
					break;
				case 1:
					sendUserEvent(UserEvent.ContactlessCardRemoved);
					break;
				case 2:
					sendUserEvent(UserEvent.CardSeated);
					break;
				case 3:
					sendUserEvent(UserEvent.CardUnseated);
					break;
				case 4:
					sendUserEvent(UserEvent.CardSwiped);
					break;
				case 5:
					sendUserEvent(UserEvent.TouchPresented);
					break;
				case 6:
					sendUserEvent(UserEvent.TouchRemoved);
					break;
				}
				break;
			}
			break;
		}
		case 24:
		{
			byte b3 = notificationCode[0];
			byte b4 = notificationCode[1];
			byte b5 = notificationCode[2];
			byte b6 = notificationCode[3];
			switch (b2)
			{
			case 3:
				if (b3 != 2)
				{
					break;
				}
				switch (b4)
				{
				case 1:
				{
					if (b5 == 0 || (b5 != 1 && b5 != 2) || b6 != 0)
					{
						break;
					}
					byte[] array = null;
					NotificationPayload notificationPayload2 = MessageParser.GetNotificationPayload(mMSNotification.getPayload());
					if (notificationPayload2 != null)
					{
						array = notificationPayload2.getParamValue("83");
					}
					if (array != null)
					{
						string stringValue = "";
						if (array != null && array.Length != 0)
						{
							stringValue = Encoding.UTF8.GetString(array);
						}
						IData data3 = new BaseData(stringValue);
						sendEvent(EventType.DisplayMessage, data3);
					}
					break;
				}
				case 2:
					if (b5 == 0)
					{
						NotificationPayload notificationPayload = MessageParser.GetNotificationPayload(mMSNotification.getPayload());
						byte[] tagValue = notificationPayload.getTagValue("81");
						byte[] tagValue2 = notificationPayload.getTagValue("82");
						byte[] tagValue3 = notificationPayload.getTagValue("83");
						processSelectionRequest(tagValue, tagValue2, tagValue3);
					}
					break;
				}
				break;
			case 5:
				switch (b3)
				{
				case 1:
					if (b4 != 1)
					{
						break;
					}
					switch (b5)
					{
					case 1:
						if (b6 == 1)
						{
							byte[] additionalDetails = mMSNotification.getAdditionalDetails();
							TLVParser.getHexString(additionalDetails);
							getDeviceConfiguration().getFile(additionalDetails, this);
						}
						break;
					case 2:
						switch (b6)
						{
						case 1:
							sendFeatureStatusEvent(DeviceFeature.SignatureCapture, FeatureStatus.TimedOut);
							break;
						case 2:
							sendFeatureStatusEvent(DeviceFeature.SignatureCapture, FeatureStatus.HardwareNA);
							break;
						}
						break;
					}
					break;
				case 2:
					if (b4 == 1 && b5 == 0)
					{
						sendFeatureStatusEvent(DeviceFeature.DisplayMessage, FeatureStatus.TimedOut);
					}
					break;
				case 4:
					if (b4 != 3)
					{
						break;
					}
					switch (b5)
					{
					case 1:
						switch (b6)
						{
						case 0:
						{
							byte[] payloadValue = MessageParser.GetNotificationPayload(mMSNotification.getPayload()).getPayloadValue();
							if (payloadValue != null)
							{
								IData data2 = new BaseData(payloadValue);
								sendEvent(EventType.BarCodeData, data2);
							}
							break;
						}
						case 1:
						{
							byte[] payload = mMSNotification.getPayload();
							if (payload != null)
							{
								IData data = new BaseData(payload);
								sendEvent(EventType.BarCodeData, data);
							}
							break;
						}
						}
						break;
					case 2:
						if (b6 == 1)
						{
							sendFeatureStatusEvent(DeviceFeature.ScanBarCode, FeatureStatus.TimedOut);
						}
						else
						{
							sendFeatureStatusEvent(DeviceFeature.ScanBarCode, FeatureStatus.Failed);
						}
						break;
					}
					break;
				}
				break;
			}
			break;
		}
		}
	}

	protected void processSelectionRequest(byte[] data1, byte[] data2, byte[] data3)
	{
		if (data3 != null && data3.Length != 0)
		{
			int num = data3.Length + 3;
			byte[] array = new byte[num];
			array[0] = 0;
			array[1] = data1[0];
			Array.Copy(data3, 0, array, 2, data3.Length);
			array[num - 1] = 0;
			IData data4 = new BaseData(array);
			sendEvent(EventType.InputRequest, data4);
		}
	}

	protected void requestCardData()
	{
		requestTransactionData(1);
	}

	protected void requestARQC()
	{
		requestTransactionData(2);
	}

	protected void requestBatchData()
	{
		requestTransactionData(3);
	}

	protected void requestTransactionData(byte dataType)
	{
		Command transactionDataCommand = MMSCommandBuilder.GetTransactionDataCommand(dataType);
		sendCommand(transactionDataCommand);
	}

	protected void processCardData(byte[] payload)
	{
		Command command = MessageParser.GetCommand(payload);
		if (command == null)
		{
			return;
		}
		MMSTransactionResult mMSTransactionResult = new MMSTransactionResult(command);
		mMSTransactionResult.getInfo();
		IData data = new BaseData(TLVParser.getHexString(mMSTransactionResult.getCardDataValue()));
		sendEvent(EventType.CardData, data);
		TLVObject cardDataObject = mMSTransactionResult.getCardDataObject();
		if (cardDataObject == null)
		{
			return;
		}
		MMSCardData mMSCardData = new MMSCardData(cardDataObject);
		if (mMSCardData == null)
		{
			return;
		}
		mMSCardData.getCardDescriptor();
		List<MMSTrackData> tracks = mMSCardData.getTracks();
		if (tracks != null)
		{
			foreach (MMSTrackData item in tracks)
			{
				item.getDataDescriptor();
				byte[] data2 = item.getData();
				if (data2 != null)
				{
					Encoding.ASCII.GetString(data2);
				}
			}
		}
		List<MMSEncryptedBlock> encryptedBlocks = mMSCardData.getEncryptedBlocks();
		if (encryptedBlocks == null)
		{
			return;
		}
		foreach (MMSEncryptedBlock item2 in encryptedBlocks)
		{
			item2.getKSNString();
		}
	}
}
