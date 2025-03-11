// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTCMSDevice
using System;
using System.Threading.Tasks;


internal class MTCMSDevice : IMTCMSDevice
{
	public delegate void DeviceStateHandler(object sender, MTConnectionState state);

	public delegate void DeviceResponseHandler(object sender, byte[] data);

	private int m_dataThreshold;

	private IMTService m_service;

	private MTConnectionState m_connectionState;

	public event DeviceStateHandler DeviceStateChanged;

	public event DeviceResponseHandler DeviceResponseReceived;

	public bool initialize(IMTService service)
	{
		m_service = service;
		m_connectionState = MTConnectionState.Disconnected;
		return true;
	}

	public bool open()
	{
		try
		{
			if (m_service != null)
			{
				m_service.initialize(OnServiceState, OnDeviceData);
				m_service.connect();
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	public void close()
	{
		try
		{
			m_connectionState = MTConnectionState.Disconnected;
			if (m_service != null)
			{
				m_service.disconnect();
			}
		}
		catch (Exception)
		{
		}
	}

	public bool sendCommand(byte[] command)
	{
		try
		{
			if (m_service != null)
			{
				if (command != null)
				{
					int num = command.Length;
					if (num > 0)
					{
						byte[] array = new byte[num];
						Array.Copy(command, 0, array, 0, num);
						Task.Factory.StartNew((Func<object, Task>)async delegate
						{
							await Task.Delay(1);
							m_service.sendData(command);
						}, (object)array);
					}
				}
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	private MTConnectionState getConnectionState(MTServiceState serviceState)
	{
		MTConnectionState result = MTConnectionState.Error;
		switch (serviceState)
		{
		case MTServiceState.Disconnected:
			result = MTConnectionState.Disconnected;
			break;
		case MTServiceState.Connected:
			result = MTConnectionState.Connected;
			break;
		case MTServiceState.Connecting:
			result = MTConnectionState.Connecting;
			break;
		case MTServiceState.Disconnecting:
			result = MTConnectionState.Disconnecting;
			break;
		}
		return result;
	}

	private void setConnectionState(MTConnectionState connectionState)
	{
		if (connectionState != m_connectionState)
		{
			m_connectionState = connectionState;
			if (this.DeviceStateChanged != null)
			{
				this.DeviceStateChanged(this, connectionState);
			}
		}
	}

	protected void OnServiceState(MTServiceState serviceState)
	{
		MTConnectionState connectionState = getConnectionState(serviceState);
		setConnectionState(connectionState);
	}

	protected void OnDeviceData(byte[] data)
	{
		if (this.DeviceResponseReceived != null)
		{
			this.DeviceResponseReceived(this, data);
		}
	}
}
