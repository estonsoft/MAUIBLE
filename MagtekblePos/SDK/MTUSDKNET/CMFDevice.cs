// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.CMFDevice
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



public class CMFDevice : BaseDevice
{
	protected const int BIG_BLOCK_DATA_SIZE = 900;

	protected const int BIG_BLOCK_DATA_SMALL_SIZE = 45;

	private MTDevice mDevice;

	protected int mReceivingBigBlockDataTotalLength;

	protected int mReceivingBigBlockDataReceivedLength;

	protected int mReceivingBigBlockDataLastPacketID;

	protected byte[] mRecevingBigBlockData;

	protected bool mSendingBigBlockData;

	protected byte[] mBigBlockData;

	protected int mBigBlockByteCount;

	protected int mBigBlockPacketCount;

	public CMFDevice(ConnectionInfo connectionInfo)
	{
		init(connectionInfo, new DeviceInfo());
	}

	public CMFDevice(ConnectionInfo connectionInfo, DeviceInfo deviceInfo)
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
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		mConnectionInfo = connectionInfo;
		mDeviceInfo = deviceInfo;
		ConnectionType connectionType = ConnectionType.USB;
		string address = "";
		if (mConnectionInfo != null)
		{
			connectionType = mConnectionInfo.getConnectionType();
			address = mConnectionInfo.getAddress();
		}
		mDevice = new MTDevice();
		mDevice.OnDeviceConnectionStateChanged += new DeviceConnectionStateHandler(OnDeviceConnectionStateChanged);
		mDevice.OnDeviceDataString += new DeviceDataStringHandler(OnDeviceDataString);
		mDevice.OnDeviceDataBytes += new DeviceDataBytesHandler(OnDeviceDataBytes);
		mDevice.OnDeviceResponseMessage += new DeviceResponseMessageHandler(OnDeviceResponseMessage);
		mDevice.OnDeviceNotificationMessage += new DeviceNotificationMessageHandler(OnDeviceNotificationMessage);
		mDevice.setConnectionType(getMTConnectionType(connectionType));
		mDevice.setAddress(address);
		mSendingBigBlockData = false;
	}

	protected static MTConnectionType getMTConnectionType(ConnectionType connectionType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		MTConnectionType result = (MTConnectionType)0;
		switch (connectionType)
		{
		case ConnectionType.USB:
			result = (MTConnectionType)0;
			break;
		case ConnectionType.TCP:
			result = (MTConnectionType)1;
			break;
		case ConnectionType.SERIAL:
			result = (MTConnectionType)2;
			break;
		}
		return result;
	}

	public override IDeviceCapabilities getCapabilities()
	{
		IDeviceCapabilities result = null;
		if (mDevice != null)
		{
			result = new CMFDeviceCapabilities();
		}
		return result;
	}

	public override IDeviceControl getDeviceControl()
	{
		if (mDeviceControl == null)
		{
			mDeviceControl = new CMFDeviceControl(mDevice);
		}
		return mDeviceControl;
	}

	public override bool startTransaction(ITransaction transaction)
	{
		bool result = true;
		if (mDevice != null && transaction != null)
		{
			Task.Factory.StartNew((Func<Task>)async delegate
			{
				if (checkConnectedDevice())
				{
					await Task.Delay(1000);
					List<PaymentMethod> paymentMethods = transaction.PaymentMethods;
					if (paymentMethods != null)
					{
						if (paymentMethods.Count == 1 && paymentMethods[0] == PaymentMethod.MSR && !transaction.EMVOnly)
						{
							IData data = new BaseData("READY FOR CARD SWIPE");
							sendEvent(EventType.DisplayMessage, data);
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

	protected bool startEMVTransaction(ITransaction transaction)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		byte[] array = new byte[19];
		Array.Clear(array, 0, 19);
		array[0] = 48;
		array[1] = getCardTypeValue(transaction.PaymentMethods);
		array[2] = (byte)(transaction.QuickChip ? 128 : 0);
		Array.Copy(BaseDevice.GetN12Bytes(transaction.Amount), 0, array, 3, 6);
		array[9] = 0;
		Array.Copy(BaseDevice.GetN12Bytes(transaction.CashBack), 0, array, 10, 6);
		Array.Copy(new byte[2] { 8, 64 }, 0, array, 16, 2);
		array[18] = 2;
		MTCMSRequestMessage message = new MTCMSRequestMessage(7, 0, 196, array);
		sendToDevice((MTCMSMessage)(object)message);
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

	public override bool cancelTransaction()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		MTCMSRequestMessage message = new MTCMSRequestMessage(7, 4, 196, (byte[])null);
		sendToDevice((MTCMSMessage)(object)message);
		return true;
	}

	public override bool sendSelection(IData data)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		MTCMSRequestMessage message = new MTCMSRequestMessage(7, 2, 196, new byte[2] { 0, 0 });
		sendToDevice((MTCMSMessage)(object)message);
		return true;
	}

	public override bool sendAuthorization(IData data)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		byte[] byteArray = data.ByteArray;
		byte[] array = null;
		int num = byteArray.Length - 2;
		if (num > 0)
		{
			array = new byte[num];
			Array.Copy(byteArray, 2, array, 0, num);
		}
		if (array != null)
		{
			MTCMSRequestMessage message = new MTCMSRequestMessage(7, 3, 196, array);
			if (array.Length > getBigBlockDataSize())
			{
				sendBigBlocksToDevice((MTCMSMessage)(object)message);
			}
			else
			{
				sendToDevice((MTCMSMessage)(object)message);
			}
		}
		return true;
	}

	private int sendBigBlocksToDevice(MTCMSMessage message)
	{
		if (mSendingBigBlockData)
		{
			return -1;
		}
		mBigBlockData = message.getMessageBytes();
		mBigBlockByteCount = 0;
		mBigBlockPacketCount = 0;
		sendBigBlockDataLength();
		return 0;
	}

	private int getBigBlockDataLength()
	{
		int result = 0;
		if (mBigBlockData != null)
		{
			result = mBigBlockData.Length;
		}
		return result;
	}

	private void sendBigBlockDataLength()
	{
		int bigBlockDataLength = getBigBlockDataLength();
		if (bigBlockDataLength > 0)
		{
			mSendingBigBlockData = true;
			byte[] packetIDByteArray = getPacketIDByteArray(0);
			byte[] lengthByteArray = getLengthByteArray(2, 4);
			byte[] lengthByteArray2 = getLengthByteArray(4, bigBlockDataLength);
			byte[] arrayBytes = getArrayBytes(packetIDByteArray, lengthByteArray, lengthByteArray2);
			mBigBlockPacketCount = 1;
			sendBigBlockDataCommand(arrayBytes);
		}
	}

	private void sendBigBlockDataCommand(byte[] data)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		MTCMSRequestMessage message = new MTCMSRequestMessage(1, 16, 196, data);
		sendToDevice((MTCMSMessage)(object)message);
	}

	private byte[] buildBigBlockDataBytes(int packetID, byte[] value)
	{
		byte[] result = null;
		if (value != null)
		{
			byte[] packetIDByteArray = getPacketIDByteArray(packetID);
			byte[] lengthByteArray = getLengthByteArray(2, value.Length);
			result = getArrayBytes(packetIDByteArray, lengthByteArray, value);
		}
		return result;
	}

	private bool uploadNextDataBlock()
	{
		int bigBlockDataLength = getBigBlockDataLength();
		if (mBigBlockByteCount < bigBlockDataLength)
		{
			int num = bigBlockDataLength - mBigBlockByteCount;
			int bigBlockDataSize = getBigBlockDataSize();
			if (num > bigBlockDataSize)
			{
				num = bigBlockDataSize;
			}
			byte[] array = new byte[num];
			if (array != null)
			{
				Array.Copy(mBigBlockData, mBigBlockByteCount, array, 0, num);
				byte[] data = buildBigBlockDataBytes(mBigBlockPacketCount, array);
				mBigBlockPacketCount++;
				mBigBlockByteCount += num;
				sendBigBlockDataCommand(data);
			}
			return true;
		}
		mSendingBigBlockData = false;
		return false;
	}

	private int getBigBlockDataSize()
	{
		int result = 900;
		if (mConnectionInfo != null && mConnectionInfo.getConnectionType() == ConnectionType.USB)
		{
			result = 45;
		}
		return result;
	}

	private byte[] getLengthByteArray(int lenBytesNeeded, int len)
	{
		byte[] array = new byte[lenBytesNeeded];
		Array.Clear(array, 0, array.Length);
		if (len > 0)
		{
			for (int i = 0; i < lenBytesNeeded; i++)
			{
				array[i] = (byte)((len >> i * 8) & 0xFF);
			}
		}
		return array;
	}

	private byte[] getPacketIDByteArray(int id)
	{
		return getLengthByteArray(2, id);
	}

	private byte[] getArrayBytes(byte[] array1, byte[] array2, byte[] array3)
	{
		int num = 0;
		if (array1 != null)
		{
			num += array1.Length;
		}
		if (array2 != null)
		{
			num += array2.Length;
		}
		if (array3 != null)
		{
			num += array3.Length;
		}
		byte[] array4 = null;
		if (num > 0)
		{
			int num2 = 0;
			array4 = new byte[num];
			if (array1 != null)
			{
				Array.Copy(array1, 0, array4, num2, array1.Length);
				num2 += array1.Length;
			}
			if (array2 != null)
			{
				Array.Copy(array2, 0, array4, num2, array2.Length);
				num2 += array2.Length;
			}
			if (array3 != null)
			{
				Array.Copy(array3, 0, array4, num2, array3.Length);
				num2 += array3.Length;
			}
		}
		return array4;
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
			resetReceivingBigBlockData();
			setupSubscription(subscribe: true);
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

	protected void OnDeviceDataString(object sender, string dataString)
	{
		IData data = new BaseData(dataString);
		sendEvent(EventType.DeviceResponse, data);
	}

	protected void OnDeviceDataBytes(object sender, byte[] dataBytes)
	{
	}

	protected void OnDeviceResponseMessage(object sender, MTCMSResponseMessage response)
	{
		processResponseMessage(response);
	}

	protected void OnDeviceNotificationMessage(object sender, MTCMSNotificationMessage notification)
	{
		processNotificationMessage(notification);
	}

	private void setupSubscription(bool subscribe)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		MTCMSRequestMessage message = new MTCMSRequestMessage(1, 80, 196, new byte[2]
		{
			(byte)(subscribe ? 1 : 2),
			0
		});
		sendToDevice((MTCMSMessage)(object)message);
	}

	private int sendToDevice(MTCMSMessage message)
	{
		TLVParser.getHexString(message.getMessageBytes());
		return mDevice.sendMTCMSMessage(message);
	}

	private void resetReceivingBigBlockData()
	{
		mReceivingBigBlockDataTotalLength = 0;
		mReceivingBigBlockDataReceivedLength = 0;
		mReceivingBigBlockDataLastPacketID = 0;
		mRecevingBigBlockData = null;
	}

	private void requestMSRData()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		MTCMSRequestMessage message = new MTCMSRequestMessage(4, 18, 196, (byte[])null);
		sendToDevice((MTCMSMessage)(object)message);
	}

	private void processPINCVMRequest()
	{
		requestPAN();
	}

	private void requestPAN()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		MTCMSRequestMessage message = new MTCMSRequestMessage(5, 1, 196, (byte[])null);
		sendToDevice((MTCMSMessage)(object)message);
	}

	private void processRequestPANResponse(byte[] data)
	{
		sendPINCVMResponse(5, null);
	}

	private void sendPINCVMResponse(byte result, byte[] pinData)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		int num = 4;
		if (pinData != null)
		{
			num += pinData.Length;
		}
		byte[] array = new byte[num];
		array[0] = 223;
		array[1] = 69;
		array[2] = 1;
		array[3] = result;
		if (num > 4)
		{
			Array.Copy(pinData, 0, array, 4, num - 4);
		}
		MTCMSResponseMessage message = new MTCMSResponseMessage(7, 136, 0, 224, array);
		sendToDevice((MTCMSMessage)(object)message);
	}

	private void processResponseMessage(MTCMSResponseMessage response)
	{
		int applicationID = ((MTCMSMessage)response).getApplicationID();
		int commandID = ((MTCMSMessage)response).getCommandID();
		int resultCode = ((MTCMSMessage)response).getResultCode();
		byte[] data = ((MTCMSMessage)response).getData();
		switch (applicationID)
		{
		case 1:
			processGeneralResponse(commandID, resultCode, data);
			break;
		case 4:
			processMSRResponse(commandID, resultCode, data);
			break;
		case 5:
			processPANResponse(commandID, resultCode, data);
			break;
		case 0:
		case 2:
		case 3:
		case 6:
		case 7:
		case 8:
		case 9:
			break;
		}
	}

	private void processNotificationMessage(MTCMSNotificationMessage notification)
	{
		int applicationID = ((MTCMSMessage)notification).getApplicationID();
		int commandID = ((MTCMSMessage)notification).getCommandID();
		int resultCode = ((MTCMSMessage)notification).getResultCode();
		byte[] data = ((MTCMSMessage)notification).getData();
		if (applicationID <= 4)
		{
			switch (applicationID)
			{
			case 1:
				processGeneralNotification(commandID, resultCode, data);
				break;
			case 4:
				processMSRNotification(commandID, resultCode, data);
				break;
			}
		}
		else if (applicationID != 7)
		{
			_ = 9;
		}
		else
		{
			processEMVL2Notification(commandID, resultCode, data);
		}
	}

	private void processGeneralResponse(int commandID, int resultCode, byte[] data)
	{
		if (commandID != 4 && commandID == 16)
		{
			processSendBigBlockDataResponse(resultCode);
		}
	}

	private void processSendBigBlockDataResponse(int resultCode)
	{
		if (mSendingBigBlockData)
		{
			if (resultCode == 0)
			{
				uploadNextDataBlock();
			}
			else
			{
				mSendingBigBlockData = false;
			}
		}
	}

	private void processMSRResponse(int commandID, int resultCode, byte[] data)
	{
		if (commandID == 18)
		{
			sendEvent(EventType.DisplayMessage, new BaseData("CARD SWIPED"));
			IData data2 = new BaseData(TLVParser.getHexString(data));
			sendEvent(EventType.CardData, data2);
		}
	}

	private void processPANResponse(int commandID, int resultCode, byte[] data)
	{
		if (commandID == 1)
		{
			processRequestPANResponse(data);
		}
	}

	private void processGeneralNotification(int commandID, int resultCode, byte[] data)
	{
		switch (commandID)
		{
		default:
			_ = 64;
			break;
		case 16:
			processBigBlockDataNotification(data);
			break;
		case 4:
			break;
		}
	}

	private void processBigBlockDataNotification(byte[] data)
	{
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		if (data == null)
		{
			return;
		}
		int num = data.Length;
		if (num < 2)
		{
			return;
		}
		int num2 = (data[1] << 8) + data[0];
		if (num2 == 0)
		{
			mReceivingBigBlockDataLastPacketID = 0;
			mReceivingBigBlockDataReceivedLength = 0;
			if (num < 4)
			{
				return;
			}
			int num3 = (data[3] << 8) + data[2];
			if (num3 > 0 && num >= num3 + 4)
			{
				byte[] array = new byte[num3];
				Array.Copy(data, 4, array, 0, num3);
				int num4 = 0;
				for (int i = 0; i < num3; i++)
				{
					num4 += (array[i] & 0xFF) << i * 8;
				}
				if (num4 > 0)
				{
					mRecevingBigBlockData = new byte[num4];
					Array.Clear(mRecevingBigBlockData, 0, num4);
					mReceivingBigBlockDataTotalLength = num4;
				}
			}
		}
		else
		{
			if (num2 != mReceivingBigBlockDataLastPacketID + 1)
			{
				return;
			}
			mReceivingBigBlockDataLastPacketID = num2;
			if (num < 4)
			{
				return;
			}
			int num5 = (data[3] << 8) + data[2];
			if (num5 <= 0 || num < num5 + 4)
			{
				return;
			}
			Array.Copy(data, 4, mRecevingBigBlockData, mReceivingBigBlockDataReceivedLength, num5);
			mReceivingBigBlockDataReceivedLength += num5;
			if (mReceivingBigBlockDataReceivedLength >= mReceivingBigBlockDataTotalLength)
			{
				MTCMSMessage val = new MTCMSMessage(mRecevingBigBlockData);
				if (val.getMessageType() == 2)
				{
					MTCMSResponseMessage response = new MTCMSResponseMessage(val.getApplicationID(), val.getCommandID(), val.getResultCode(), val.getDataTag(), val.getData());
					OnDeviceResponseMessage(this, response);
				}
				else if (val.getMessageType() == 3)
				{
					MTCMSNotificationMessage notification = new MTCMSNotificationMessage(val.getApplicationID(), val.getCommandID(), val.getResultCode(), val.getDataTag(), val.getData());
					OnDeviceNotificationMessage(this, notification);
				}
			}
		}
	}

	private void processMSRNotification(int commandID, int resultCode, byte[] data)
	{
		if (commandID != 17)
		{
			_ = 18;
		}
		else
		{
			requestMSRData();
		}
	}

	private void processEMVL2Notification(int commandID, int resultCode, byte[] data)
	{
		switch (commandID)
		{
		case 129:
			processDisplayMessageRequest(data);
			break;
		case 130:
			processUserSelectionRequest(data);
			break;
		case 131:
			processARQC(data);
			break;
		case 4:
			processCancelTransaction(data);
			break;
		case 132:
			processTransactionResult(data);
			break;
		case 136:
			processPINCVMRequest();
			break;
		}
	}

	private void processDisplayMessageRequest(byte[] data)
	{
		string stringValue = "";
		if (data != null && data.Length != 0)
		{
			stringValue = Encoding.UTF8.GetString(data);
		}
		IData data2 = new BaseData(stringValue);
		sendEvent(EventType.DisplayMessage, data2);
	}

	protected void processUserSelectionRequest(byte[] data)
	{
		IData data2 = new BaseData(data);
		sendEvent(EventType.InputRequest, data2);
	}

	protected List<string> getSelectionList(byte[] data, int offset)
	{
		List<string> list = new List<string>();
		if (data != null)
		{
			int num = data.Length;
			if (num >= offset)
			{
				int num2 = offset;
				for (int i = offset; i < num; i++)
				{
					if (data[i] == 0)
					{
						int num3 = i - num2;
						if (num3 >= 0)
						{
							byte[] array = new byte[num3];
							Array.Copy(data, num2, array, 0, num3);
							string @string = Encoding.UTF8.GetString(array);
							list.Add(@string);
						}
						num2 = i + 1;
					}
				}
			}
		}
		return list;
	}

	private void processARQC(byte[] data)
	{
		if (data != null && data.Length >= 2)
		{
			byte[] array = new byte[data.Length];
			Array.Copy(data, 0, array, 0, data.Length);
			IData data2 = new BaseData(array);
			sendEvent(EventType.AuthorizationRequest, data2);
		}
	}

	private void processCancelTransaction(byte[] data)
	{
		updateTransactionStatusValue(TransactionStatus.TransactionCancelled);
	}

	private void processTransactionResult(byte[] data)
	{
		if (data != null && data.Length > 3)
		{
			byte[] array = new byte[data.Length - 1];
			Array.Copy(data, 1, array, 0, data.Length - 1);
			IData data2 = new BaseData(array);
			sendEvent(EventType.TransactionResult, data2);
		}
	}
}
