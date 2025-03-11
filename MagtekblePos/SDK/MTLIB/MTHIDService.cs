// MTServiceNET, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTHIDService
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using MTLIB.WindowsHID;

public class MTHIDService : MTBaseService
{
	private static int COMMAND_RESPONSE_TIMEOUT = 2000;

	private static List<MTDeviceInformation> MTDeviceList;

	private MTHIDDevice m_hidDevice;

	private AutoResetEvent m_firmwareIDEvent;

	private string m_firmwareID = "";

	private AutoResetEvent m_deviceSerialEvent;

	private string m_deviceSerial = "";

	private AutoResetEvent m_capMagneSafe20EncryptionEvent;

	private string m_capMagneSafe20Encryption = "";

	private AutoResetEvent m_capMagStripeEncryptionEvent;

	private string m_capMagStripeEncryption = "";

	private AutoResetEvent m_capTracksEvent;

	private string m_capTracks = "";

	~MTHIDService()
	{
	}

	public static void requestDeviceList(Action<MTConnectionType, List<MTDeviceInformation>> deviceListEventHandler)
	{
		List<HIDDeviceInformation> devices = MTHIDDevice.GetDevices(MTDeviceConstants.VID_MAGTEK, MTDeviceConstants.PID_SCRA);
		List<MTDeviceInformation> list = new List<MTDeviceInformation>();
		int num = 0;
		if (devices.Count > 0)
		{
			foreach (HIDDeviceInformation item in devices)
			{
				if (item == null)
				{
					continue;
				}
				MTDeviceInformation mTDeviceInformation = new MTDeviceInformation();
				if (mTDeviceInformation != null)
				{
					num++;
					mTDeviceInformation.Id = item.InstanceId;
					mTDeviceInformation.Name = item.FriendlyName;
					mTDeviceInformation.Address = item.DevicePath;
					mTDeviceInformation.ProductId = item.ProductId;
					mTDeviceInformation.Model = MTHIDDevice.GetDeviceModel(item.ProductId);
					mTDeviceInformation.Serial = item.Serial;
					if (string.IsNullOrWhiteSpace(mTDeviceInformation.Name))
					{
						mTDeviceInformation.Name = mTDeviceInformation.Model + " " + num;
					}
					list.Add(mTDeviceInformation);
				}
			}
		}
		deviceListEventHandler?.Invoke(MTConnectionType.USB, list);
	}

	private void handleCommandResponse(byte[] data)
	{
		if (m_firmwareIDEvent != null)
		{
			if (data.Length >= 1 && data[0] == 0)
			{
				int num = data[1];
				if (data.Length - 2 >= num)
				{
					byte[] array = new byte[num];
					Array.Copy(data, 2, array, 0, num);
					m_firmwareID = Encoding.UTF8.GetString(array, 0, num);
				}
				else
				{
					m_firmwareID = "";
				}
			}
			m_firmwareIDEvent.Set();
		}
		else if (m_deviceSerialEvent != null)
		{
			if (data.Length >= 1 && data[0] == 0)
			{
				int num2 = data[1];
				if (data.Length - 2 >= num2)
				{
					byte[] array2 = new byte[num2];
					Array.Copy(data, 2, array2, 0, num2);
					m_deviceSerial = Encoding.UTF8.GetString(array2, 0, num2);
				}
				else
				{
					m_deviceSerial = "";
				}
			}
			m_deviceSerialEvent.Set();
		}
		else if (m_capMagneSafe20EncryptionEvent != null)
		{
			if (data.Length >= 1 && data[0] == 0)
			{
				int num3 = data[1];
				if (data.Length >= 1)
				{
					byte[] array3 = new byte[num3];
					Array.Copy(data, 2, array3, 0, num3);
					m_capMagneSafe20Encryption = MTParser.getHexString(array3);
				}
				else
				{
					m_capMagneSafe20Encryption = "";
				}
			}
			m_capMagneSafe20EncryptionEvent.Set();
		}
		else if (m_capMagStripeEncryptionEvent != null)
		{
			if (data.Length >= 1 && data[0] == 0)
			{
				int num4 = data[1];
				if (data.Length >= 1)
				{
					byte[] array4 = new byte[num4];
					Array.Copy(data, 2, array4, 0, num4);
					m_capMagStripeEncryption = MTParser.getHexString(array4);
				}
				else
				{
					m_capMagStripeEncryption = "";
				}
			}
			m_capMagStripeEncryptionEvent.Set();
		}
		else if (m_capTracksEvent != null)
		{
			if (data.Length >= 1 && data[0] == 0)
			{
				int num5 = data[1];
				if (data.Length >= 1)
				{
					byte[] array5 = new byte[num5];
					Array.Copy(data, 2, array5, 0, num5);
					m_capTracks = MTParser.getHexString(array5);
				}
				else
				{
					m_capTracks = "";
				}
			}
			m_capTracksEvent.Set();
		}
		if (m_commandDataHandler != null)
		{
			m_commandDataHandler(data);
		}
	}

	protected void OnFeatureReportReceived(byte[] data)
	{
		try
		{
			if (data == null)
			{
				return;
			}
			int num = data.Length;
			if (num > 0)
			{
				int num2 = 1;
				if (num > 2)
				{
					num2 = data[1] + 2;
				}
				byte[] array = new byte[num2];
				Array.Copy(data, 0, array, 0, num2);
				handleCommandResponse(array);
			}
		}
		catch (Exception)
		{
		}
	}

	protected void OnInputReportReceived(byte[] data)
	{
		try
		{
			if (data == null)
			{
				return;
			}
			int num = data.Length - 1;
			if (num <= 0)
			{
				return;
			}
			byte b = data[0];
			byte[] array = new byte[num];
			Array.Copy(data, 1, array, 0, num);
			if (b == 0 || b == 1)
			{
				if (m_cardDataHandler != null)
				{
					m_cardDataHandler(array);
				}
			}
			else if (m_deviceDataHandler != null)
			{
				m_deviceDataHandler(array);
			}
		}
		catch (Exception)
		{
		}
	}

	protected void OnStateChanged(bool isConnected)
	{
		if (isConnected)
		{
			setState(MTServiceState.Connected);
		}
		else
		{
			setState(MTServiceState.Disconnected);
		}
	}

	public override void connect()
	{
		if (string.IsNullOrEmpty(m_address))
		{
			List<HIDDeviceInformation> devices = MTHIDDevice.GetDevices(MTDeviceConstants.VID_MAGTEK, MTDeviceConstants.PID_SCRA);
			if (devices.Count > 0)
			{
				m_address = devices[0].DevicePath;
			}
		}
		m_state = MTServiceState.Disconnected;
		try
		{
			m_hidDevice = new MTHIDDevice();
			m_hidDevice.FeatureReportReceived += OnFeatureReportReceived;
			m_hidDevice.InputReportReceived += OnInputReportReceived;
			m_hidDevice.StateChanged += OnStateChanged;
		}
		catch (Exception)
		{
		}
		if (m_hidDevice != null)
		{
			setState(MTServiceState.Connecting);
			if (m_hidDevice.Open(m_address))
			{
				return;
			}
		}
		setState(MTServiceState.Disconnected);
	}

	public override void disconnect()
	{
		if (m_hidDevice != null)
		{
			setState(MTServiceState.Disconnecting);
			try
			{
				m_hidDevice.Close();
			}
			finally
			{
				setState(MTServiceState.Disconnected);
				m_hidDevice = null;
			}
		}
	}

	public override bool sendData(byte[] data)
	{
		bool result = false;
		if (m_hidDevice != null)
		{
			try
			{
				result = m_hidDevice.SendData(data);
			}
			catch (Exception)
			{
			}
		}
		return result;
	}

	public override long getBatteryLevel()
	{
		return MTDeviceConstants.BATTERY_LEVEL_MAX;
	}

	public override string getDeviceName()
	{
		string result = "";
		if (MTDeviceList != null)
		{
			foreach (MTDeviceInformation mTDevice in MTDeviceList)
			{
				if (mTDevice != null && string.Compare(m_address, mTDevice.Address, ignoreCase: true) == 0)
				{
					result = mTDevice.Name;
					break;
				}
			}
		}
		return result;
	}

	public override string getFirmwareID()
	{
		string result = "";
		if (m_state == MTServiceState.Connected)
		{
			m_firmwareIDEvent = new AutoResetEvent(initialState: false);
			sendData(MTDeviceConstants.SCRA_DEVICE_COMMAND_GET_FIRMWARE_ID);
			if (m_firmwareIDEvent.WaitOne(COMMAND_RESPONSE_TIMEOUT))
			{
				result = m_firmwareID;
			}
			m_firmwareIDEvent = null;
		}
		return result;
	}

	public override string getDeviceSerial()
	{
		string result = "";
		if (m_state == MTServiceState.Connected)
		{
			m_deviceSerialEvent = new AutoResetEvent(initialState: false);
			sendData(MTDeviceConstants.SCRA_DEVICE_COMMAND_GET_DEVICE_SERIAL);
			if (m_deviceSerialEvent.WaitOne(COMMAND_RESPONSE_TIMEOUT))
			{
				result = m_deviceSerial;
			}
			m_deviceSerialEvent = null;
		}
		return result;
	}

	public override string getCapMagneSafe20Encryption()
	{
		string result = "";
		if (m_state == MTServiceState.Connected)
		{
			m_capMagneSafe20EncryptionEvent = new AutoResetEvent(initialState: false);
			sendData(MTDeviceConstants.SCRA_DEVICE_COMMAND_GET_CAP_MAGNESAFE20_ENCRYPTION);
			if (m_capMagneSafe20EncryptionEvent.WaitOne(COMMAND_RESPONSE_TIMEOUT))
			{
				result = m_capMagneSafe20Encryption;
			}
			m_capMagneSafe20EncryptionEvent = null;
		}
		return result;
	}

	public override string getCapMagStripeEncryption()
	{
		string result = "";
		if (m_state == MTServiceState.Connected)
		{
			m_capMagStripeEncryptionEvent = new AutoResetEvent(initialState: false);
			sendData(MTDeviceConstants.SCRA_DEVICE_COMMAND_GET_CAP_MAGSTRIPE_ENCRYPTION);
			if (m_capMagStripeEncryptionEvent.WaitOne(COMMAND_RESPONSE_TIMEOUT))
			{
				result = m_capMagStripeEncryption;
			}
			m_capMagStripeEncryptionEvent = null;
		}
		return result;
	}

	public override string getCapMSR()
	{
		return "1";
	}

	public override string getCapTracks()
	{
		string result = "";
		if (m_state == MTServiceState.Connected)
		{
			m_capTracksEvent = new AutoResetEvent(initialState: false);
			sendData(MTDeviceConstants.SCRA_DEVICE_COMMAND_GET_CAP_TRACKS);
			if (m_capTracksEvent.WaitOne(COMMAND_RESPONSE_TIMEOUT))
			{
				result = m_capTracks;
			}
			m_capTracksEvent = null;
		}
		return result;
	}

	public override bool isOutputChannelConfigurable()
	{
		bool result = false;
		if (m_hidDevice != null)
		{
			result = m_hidDevice.isOutputChannelConfigurable();
		}
		return result;
	}

	public override bool isServiceEMV()
	{
		bool result = false;
		if (m_hidDevice != null)
		{
			result = m_hidDevice.IsDeviceEMV();
		}
		return result;
	}

	public override bool isServiceOEM()
	{
		bool result = false;
		if (m_hidDevice != null)
		{
			result = m_hidDevice.IsDeviceOEM();
		}
		return result;
	}

	public override string getDevicePMValue()
	{
		string result = "";
		if (m_hidDevice != null)
		{
			result = m_hidDevice.getDevicePMValue();
		}
		return result;
	}

	public override MTDeviceFeatures getDeviceFeatures()
	{
		MTDeviceFeatures result = new MTDeviceFeatures();
		if (m_hidDevice != null)
		{
			result = m_hidDevice.getDeviceFeatures();
		}
		return result;
	}

	public static MTDeviceFeatures GetDeviceFeatures(string path)
	{
		return MTHIDDevice.GetDeviceFeatures(path);
	}
}
