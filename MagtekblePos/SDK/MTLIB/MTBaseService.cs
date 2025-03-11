// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTBaseService
using System;


public class MTBaseService : IMTService
{
	protected Action<MTServiceState> m_serviceStateHandler;

	protected Action<byte[]> m_commandDataHandler;

	protected Action<byte[]> m_deviceDataHandler;

	protected Action<byte[]> m_cardDataHandler;

	protected Action<MTCardDataError> m_cardDataErrorHandler;

	protected MTServiceState m_state;

	protected string m_address;

	protected string m_deviceID;

	protected Guid m_serviceGuid;

	public MTBaseService()
	{
		m_address = string.Empty;
		m_deviceID = string.Empty;
	}

	~MTBaseService()
	{
	}

	protected void setState(MTServiceState state)
	{
		if (m_state != state)
		{
			m_state = state;
			if (m_serviceStateHandler != null)
			{
				m_serviceStateHandler(m_state);
			}
		}
	}

	public MTServiceState getState()
	{
		return m_state;
	}

	public void setServiceUUID(Guid serviceGuid)
	{
		m_serviceGuid = serviceGuid;
	}

	public virtual void initialize(Action<MTServiceState> serviceStateHandler, Action<byte[]> commandDataHandler, Action<byte[]> deviceDataHandler, Action<byte[]> cardDataEventHandler, Action<MTCardDataError> cardDataErrorHandler)
	{
		m_serviceStateHandler = serviceStateHandler;
		m_commandDataHandler = commandDataHandler;
		m_deviceDataHandler = deviceDataHandler;
		m_cardDataHandler = cardDataEventHandler;
		m_cardDataErrorHandler = cardDataErrorHandler;
	}

	public virtual void setAddress(string address)
	{
		m_address = address;
	}

	public virtual void setDeviceID(string deviceID)
	{
		m_deviceID = deviceID;
	}

	public virtual void connect()
	{
	}

	public virtual void disconnect()
	{
	}

	public virtual bool sendData(byte[] data)
	{
		return false;
	}

	public virtual long getBatteryLevel()
	{
		return MTDeviceConstants.BATTERY_LEVEL_NA;
	}

	public virtual string getDeviceName()
	{
		return "";
	}

	public virtual string getFirmwareID()
	{
		return "";
	}

	public virtual string getDeviceSerial()
	{
		return "";
	}

	public virtual string getCapMagnePrint()
	{
		return "";
	}

	public virtual string getCapMagnePrintEncryption()
	{
		return "";
	}

	public virtual string getCapMagneSafe20Encryption()
	{
		return "";
	}

	public virtual string getCapMagStripeEncryption()
	{
		return "";
	}

	public virtual string getCapMSR()
	{
		return "";
	}

	public virtual string getCapTracks()
	{
		return "";
	}

	public virtual bool isOutputChannelConfigurable()
	{
		return false;
	}

	public virtual bool isServiceEMV()
	{
		return false;
	}

	public virtual bool isServiceOEM()
	{
		return false;
	}

	public virtual string getDevicePMValue()
	{
		return "";
	}

	public virtual MTDeviceFeatures getDeviceFeatures()
	{
		return new MTDeviceFeatures();
	}
}
