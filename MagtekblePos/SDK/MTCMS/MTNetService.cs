// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTNetService
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


internal class MTNetService : MTBaseService
{
	private const string TCP_HEADER = "IP://";

	private string mIPAddress;

	private string mPort = "5000";

	private MTTCPDevice mTCPDevice = new MTTCPDevice();

	public MTNetService()
	{
		mTCPDevice.OnDataReceived += OnDataReceived;
		mTCPDevice.OnError += OnError;
	}

	~MTNetService()
	{
	}

	public void OnError(object sender, EventArgs e)
	{
		setStateDisconnected();
	}

	public void OnDataReceived(object sender, byte[] e)
	{
		if (m_deviceDataHandler != null)
		{
			m_deviceDataHandler(e);
		}
	}

	protected void setStateDisconnected()
	{
		setState(MTServiceState.Disconnected);
	}

	public static void requestDeviceList(Action<MTConnectionType, List<MTDeviceInformation>> deviceListEventHandler)
	{
	}

	public override void connect()
	{
		m_state = MTServiceState.Disconnected;
		setState(MTServiceState.Connecting);
		int.TryParse(mPort, out var result);
		mTCPDevice.Open(mIPAddress, result).ContinueWith(delegate(Task<bool> t)
		{
			if (t.Result)
			{
				setState(MTServiceState.Connected);
			}
			else
			{
				setState(MTServiceState.Disconnected);
			}
		});
	}

	public override void disconnect()
	{
		setState(MTServiceState.Disconnecting);
		mTCPDevice.Close();
		setState(MTServiceState.Disconnected);
	}

	public override void sendData(byte[] data)
	{
		mTCPDevice.Send(data);
	}

	public override void setAddress(string address)
	{
		base.setAddress(address);
		string text = address.ToUpper();
		if (text.StartsWith("IP://"))
		{
			text = text.Substring("IP://".Length);
		}
		int num = text.IndexOf(':');
		if (num >= 0)
		{
			mIPAddress = text.Substring(0, num);
			mPort = uint.Parse(text.Substring(num + 1)).ToString();
		}
		else
		{
			mIPAddress = text;
			mPort = "5000";
		}
	}
}
