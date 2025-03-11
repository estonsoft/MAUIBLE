// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTPPSCRADevice
using System;


public class MTPPSCRADevice : IMTPPSCRADevice
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
				m_service.initialize(OnServiceState, OnCommandData, null, null, null);
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
				m_service.sendData(command);
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

	protected void OnCommandData(byte[] data)
	{
		if (this.DeviceResponseReceived != null)
		{
			this.DeviceResponseReceived(this, data);
		}
	}
}
