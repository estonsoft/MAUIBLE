// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXBaseService
using System;
using System.Collections.Generic;
using System.Linq;


internal class MMXBaseService : IMMXService
{
	protected string _deviceId;

	protected MMXConnectionState connectionState;

	public event EventHandler<byte[]> onData;

	public event EventHandler<MMXConnectionState> onConnectionChanged;

	~MMXBaseService()
	{
	}

	protected void FireOnData(IEnumerable<byte> data)
	{
		this.onData?.Invoke(this, data.ToArray());
	}

	protected void SetConnectionState(MMXConnectionState state)
	{
		if (state != connectionState)
		{
			connectionState = state;
			this.onConnectionChanged?.Invoke(this, state);
		}
	}

	public virtual void connect()
	{
	}

	public virtual void disconnect()
	{
	}

	public virtual void sendData(byte[] data)
	{
	}

	public virtual string[] getDeviceList()
	{
		throw new NotImplementedException();
	}

	public virtual void setDeviceID(string ID)
	{
		_deviceId = ID;
	}

	public virtual bool isConneted()
	{
		throw new NotImplementedException();
	}
}
