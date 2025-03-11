// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.BaseDevice
using System;
using System.Threading;


public class BaseDevice : EventPublisher, IDevice
{
	protected ConnectionInfo mConnectionInfo;

	protected ConnectionState mConnectionState = ConnectionState.Disconnected;

	protected DeviceInfo mDeviceInfo;

	protected IDeviceControl mDeviceControl;

	protected IDeviceConfiguration mDeviceConfiguration;

	protected TransactionStatus mTransactionStatus = TransactionStatus.NoTransaction;

	protected AutoResetEvent mConnectedEvent = new AutoResetEvent(initialState: false);

	public string Name
	{
		get
		{
			if (mDeviceInfo != null)
			{
				return mDeviceInfo.getName();
			}
			return "NA";
		}
	}

	public BaseDevice()
	{
		init(null, new DeviceInfo());
	}

	public BaseDevice(ConnectionInfo connectionInfo)
	{
		init(connectionInfo, new DeviceInfo());
	}

	public BaseDevice(ConnectionInfo connectionInfo, DeviceInfo deviceInfo)
	{
		init(connectionInfo, deviceInfo);
	}

	protected virtual void init(ConnectionInfo connectionInfo, DeviceInfo deviceInfo)
	{
		mConnectionInfo = connectionInfo;
		mDeviceInfo = deviceInfo;
	}

	public ConnectionInfo getConnectionInfo()
	{
		return mConnectionInfo;
	}

	public ConnectionState getConnectionState()
	{
		return mConnectionState;
	}

	public DeviceInfo getDeviceInfo()
	{
		return mDeviceInfo;
	}

	public bool subscribeAll(IEventSubscriber eventSubscriber)
	{
		return addSubscriber(eventSubscriber);
	}

	public bool unsubscribeAll(IEventSubscriber eventSubscriber)
	{
		return removeSubscriber(eventSubscriber);
	}

	public virtual IDeviceCapabilities getCapabilities()
	{
		return null;
	}

	public virtual IDeviceControl getDeviceControl()
	{
		return mDeviceControl;
	}

	public virtual IDeviceConfiguration getDeviceConfiguration()
	{
		return mDeviceConfiguration;
	}

	public virtual bool startTransaction(ITransaction transaction)
	{
		return false;
	}

	public virtual bool cancelTransaction()
	{
		return false;
	}

	public virtual bool sendSelection(IData data)
	{
		return false;
	}

	public virtual bool sendAuthorization(IData data)
	{
		return false;
	}

	public virtual bool requestPIN(PINRequest pinRequest)
	{
		return false;
	}

	public virtual bool requestPAN(PANRequest panRequest, PINRequest pinRequest = null)
	{
		return false;
	}

	public virtual bool requestSignature(byte timeout)
	{
		return false;
	}

	protected void sendCardData(string data)
	{
	}

	protected void sendEMVTransactionResult(string data)
	{
	}

	protected static string GetN12String(string Value)
	{
		double num = 0.0;
		try
		{
			num = double.Parse(Value);
		}
		catch (Exception)
		{
		}
		return num.ToString("N").Replace(",", "").Replace(".", "")
			.PadLeft(12, '0');
	}

	protected static byte[] GetN12Bytes(string Value)
	{
		return TLVParser.getByteArrayFromHexString(GetN12String(Value));
	}

	protected override void sendEvent(EventType eventType, IData data)
	{
		base.sendEvent(eventType, data);
	}

	protected void updateConnectionStateValue(ConnectionState connectionStateValue)
	{
		mConnectionState = connectionStateValue;
		if (mConnectionState == ConnectionState.Connected)
		{
			mConnectedEvent.Set();
		}
		IData data = new BaseData(ConnectionStateBuilder.GetString(connectionStateValue));
		sendEvent(EventType.ConnectionState, data);
	}

	protected void updateTransactionStatusValue(TransactionStatus transactionStatusValue)
	{
		mTransactionStatus = transactionStatusValue;
		IData data = new BaseData(TransactionStatusBuilder.GetString(transactionStatusValue));
		sendEvent(EventType.TransactionStatus, data);
	}

	protected void updateTransactionStatusValue(TransactionStatus transactionStatusValue, string statusDetail, string deviceDetail)
	{
		mTransactionStatus = transactionStatusValue;
		IData data = new BaseData(TransactionStatusBuilder.GetString(transactionStatusValue) + "," + statusDetail + "," + deviceDetail);
		sendEvent(EventType.TransactionStatus, data);
	}

	protected void sendDeviceEvent(DeviceEvent eventValue, string eventDetail)
	{
		string text = DeviceEventBuilder.GetString(eventValue);
		if (!string.IsNullOrEmpty(eventDetail))
		{
			text = text + "," + eventDetail;
		}
		IData data = new BaseData(text);
		sendEvent(EventType.DeviceEvent, data);
	}

	protected void sendUserEvent(UserEvent eventValue)
	{
		IData data = new BaseData(UserEventBuilder.GetString(eventValue));
		sendEvent(EventType.UserEvent, data);
	}

	protected void sendFeatureStatusEvent(DeviceFeature feature, FeatureStatus status)
	{
		string featureString = FeatureStatusBuilder.GetFeatureString(feature);
		string statusString = FeatureStatusBuilder.GetStatusString(status);
		IData data = new BaseData(featureString + "," + statusString);
		sendEvent(EventType.FeatureStatus, data);
	}

	protected bool checkConnectedDevice()
	{
		bool result = false;
		if (mConnectionState == ConnectionState.Connected)
		{
			result = true;
		}
		else
		{
			if (mConnectionState == ConnectionState.Disconnected)
			{
				getDeviceControl().open();
			}
			if (mConnectedEvent.WaitOne(5000) && mConnectionState == ConnectionState.Connected)
			{
				result = true;
			}
		}
		return result;
	}
}
