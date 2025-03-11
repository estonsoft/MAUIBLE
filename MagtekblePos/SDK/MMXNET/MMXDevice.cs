// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXDevice
using System;
using System.Threading.Tasks;


public class MMXDevice
{
	internal MMXConnectionType mConnectionType;

	internal IMMXService mService;

	internal IMMXMessageSender mMessageSender;

	internal IMMXMessageReceiver mMessageReceiver;

	private object messagelock = new object();

	private ActionQueue<byte[]> sendingQueue = new ActionQueue<byte[]>();

	public bool isOpen
	{
		get
		{
			if (mService != null)
			{
				return mService.isConneted();
			}
			return false;
		}
	}

	public event EventHandler<byte[]> onMessage;

	public event EventHandler onCancel;

	public event EventHandler<string> onError;

	public event EventHandler<MMXConnectionState> onConnectionStateChanged;

	public void initialize()
	{
		try
		{
			mConnectionType = MMXConnectionType.USB;
		}
		catch (Exception)
		{
		}
	}

	private void setConnectionType_internal(MMXConnectionType ConnectionType)
	{
		if (!isOpen)
		{
			switch (ConnectionType)
			{
			case MMXConnectionType.USB:
				mService = new MMXUSBService();
				mMessageSender = new MMXMessageSender();
				mMessageReceiver = new MMXMessageReceiver();
				break;
			case MMXConnectionType.TCP:
				mService = new MMXTCPService();
				mMessageSender = new MMXMessageSender();
				mMessageReceiver = new MMXMessageReceiver();
				break;
			case MMXConnectionType.WEBSOCKET:
				mService = new MMXWebSocketService();
				mMessageSender = new MMXPassthroughMessageSender();
				mMessageReceiver = new MMXPassthroughMessageReceiver();
				break;
			case MMXConnectionType.Serial:
				mService = new MMXSerialService();
				mMessageSender = new MMXPassthroughMessageSender();
				mMessageReceiver = new MMXPassthroughMessageReceiver();
				break;
			case MMXConnectionType.BLE:
				break;
			}
		}
	}

	public void setConnectionType(MMXConnectionType ConnectionType)
	{
		if (!isOpen)
		{
			mConnectionType = ConnectionType;
		}
	}

	public string[] getDeviceList()
	{
		return mService.getDeviceList();
	}

	public void open(string DeviceName = "")
	{
		if (isOpen)
		{
			return;
		}
		setConnectionType_internal(mConnectionType);
		try
		{
			mMessageSender.initialize(mService);
			mMessageReceiver.initialize(mService);
			if (mService is MMXTCPService)
			{
				mMessageReceiver.mDiscardExtraData = false;
			}
			else
			{
				mMessageReceiver.mDiscardExtraData = true;
			}
			mService.setDeviceID(DeviceName);
			mService.onConnectionChanged += MService_onConnectionChanged;
			mService.connect();
			mMessageReceiver.OnMessage += OnMessageReceived;
			mMessageReceiver.OnCancel += MMessageReceiver_OnCancel;
			mMessageReceiver.OnError += MMessageReceiver_OnError;
		}
		catch (Exception)
		{
		}
	}

	private void MMessageReceiver_OnError(object sender, string e)
	{
		this.onError?.Invoke(this, e);
	}

	private void MMessageReceiver_OnCancel(object sender, EventArgs e)
	{
		this.onCancel?.Invoke(this, e);
	}

	private void MService_onConnectionChanged(object sender, MMXConnectionState e)
	{
		try
		{
			this.onConnectionStateChanged?.Invoke(this, e);
		}
		catch
		{
		}
	}

	public void close()
	{
		if (!isOpen)
		{
			return;
		}
		try
		{
			mMessageReceiver.OnMessage -= OnMessageReceived;
			mMessageReceiver.OnCancel -= MMessageReceiver_OnCancel;
			mMessageReceiver.OnError -= MMessageReceiver_OnError;
		}
		catch
		{
		}
		try
		{
			mService.disconnect();
			mService.onConnectionChanged -= MService_onConnectionChanged;
		}
		catch (Exception)
		{
		}
	}

	public void cancelDataTransfer()
	{
		try
		{
			if (mMessageSender != null)
			{
				mMessageSender.cancel();
			}
		}
		catch
		{
		}
	}

	public void sendMessage(MMXMessage message, Action<float> ProgressUpdate = null)
	{
		try
		{
			if (mMessageSender != null)
			{
				mMessageSender.send(message, ProgressUpdate);
			}
		}
		catch (Exception)
		{
		}
	}

	internal void OnMessageReceived(object sender, byte[] data)
	{
		lock (messagelock)
		{
			try
			{
				this.onMessage?.Invoke(this, data);
			}
			catch
			{
			}
		}
	}

	public byte[] SendAndReceive(byte[] data, int TimeOut = 1000, Action<float> ProgressUpdate = null, bool needResponse = true)
	{
		TaskCompletionSource<byte[]> completeBytes = new TaskCompletionSource<byte[]>();
		bool IsResultSet = false;
		EventHandler<byte[]> value = delegate(object o, byte[] d)
		{
			if (!IsResultSet)
			{
				IsResultSet = true;
				if (needResponse)
				{
					if (d != null && d.Length >= 8 && d[4] == 130)
					{
						completeBytes.TrySetResult(d);
					}
				}
				else
				{
					completeBytes.TrySetResult(d);
				}
			}
		};
		onMessage += value;
		EventHandler value2 = delegate
		{
			if (!IsResultSet)
			{
				IsResultSet = true;
				completeBytes.TrySetCanceled();
			}
		};
		onCancel += value2;
		EventHandler<MMXConnectionState> value3 = delegate
		{
			if (!IsResultSet)
			{
				IsResultSet = true;
				completeBytes.TrySetException(new Exception("Device Disconnected"));
			}
		};
		onConnectionStateChanged += value3;
		try
		{
			sendMessage(new MMXMessage(48, data), ProgressUpdate);
			completeBytes.Task.Wait(TimeOut);
			if (completeBytes.Task.IsCompleted)
			{
				return completeBytes.Task.Result;
			}
		}
		finally
		{
			onMessage -= value;
			onCancel -= value2;
			onConnectionStateChanged -= value3;
		}
		return null;
	}

	public Task<byte[]> SendAndReceiveAsync(byte[] data, int TimeOut = 1000, Action<float> ProgressUpdate = null, bool needResponse = true)
	{
		return sendingQueue.Queue(() => SendAndReceive(data, TimeOut, ProgressUpdate, needResponse));
	}
}
