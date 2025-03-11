// MTServiceNET, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTAudioService
using System;

using MTLIB.Audio;

public class MTAudioService : MTBaseService
{
	private AudioReader m_audioReader;

	public MTAudioService()
	{
		m_state = MTServiceState.Disconnected;
		try
		{
			m_audioReader = new AudioReader();
		}
		catch (Exception)
		{
		}
	}

	~MTAudioService()
	{
	}

	public override void setAddress(string address)
	{
	}

	public override string getDevicePMValue()
	{
		return "PM4";
	}

	public override MTDeviceFeatures getDeviceFeatures()
	{
		return new MTDeviceFeatures
		{
			MSR = true
		};
	}

	public static MTDeviceFeatures GetDeviceFeatures()
	{
		return new MTDeviceFeatures
		{
			MSR = true
		};
	}

	public override void connect()
	{
		if (m_audioReader != null)
		{
			m_audioReader.DeviceEventStateChange += OnDeviceEventStateChange;
			m_audioReader.DeviceEventDataChange += OnDeviceEventDataChange;
			m_audioReader.DeviceEventDataError += OnDeviceEventDataError;
			m_audioReader.DeviceEventDataSample += OnDeviceEventDataSample;
			setState(MTServiceState.Connecting);
			m_audioReader.openDevice();
		}
	}

	public override void disconnect()
	{
		if (m_audioReader != null)
		{
			setState(MTServiceState.Disconnecting);
			m_audioReader.closeDevice();
		}
	}

	public override bool sendData(byte[] data)
	{
		bool result = false;
		if (m_audioReader != null)
		{
			result = m_audioReader.sendCommandToDevice(MTParser.getHexString(data));
		}
		return result;
	}

	protected void OnDeviceEventStateChange(object sender, MTServiceState state)
	{
		setState(state);
	}

	protected void OnDeviceEventDataStart(object sender)
	{
	}

	protected void OnDeviceEventDataChange(object sender, byte[] data)
	{
		if (m_cardDataHandler == null)
		{
			return;
		}
		try
		{
			bool flag = false;
			if (data != null && data.Length >= 2 && data[0] == 193 && (data[1] == 1 || data[1] == 6))
			{
				flag = true;
			}
			if (flag)
			{
				if (m_cardDataHandler != null)
				{
					m_cardDataHandler(data);
				}
			}
			else if (m_commandDataHandler != null)
			{
				m_commandDataHandler(data);
			}
		}
		catch (Exception)
		{
		}
	}

	protected void OnDeviceEventDataError(object sender, MTCardDataError error)
	{
		if (m_cardDataErrorHandler != null)
		{
			m_cardDataErrorHandler(error);
		}
	}

	protected void OnDeviceEventDataSample(object sender, short value)
	{
	}
}
