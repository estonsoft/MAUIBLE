// MTServiceNET, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTBLEService
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MTLIB.WindowsBLE;

public class MTBLEService : MTBaseService
{
	private static int COMMAND_RESPONSE_TIMEOUT = 2000;

	private static List<MTDeviceInformation> MTDeviceList;

	private MTBLEDevice m_bleDevice;

	private AutoResetEvent m_batteryLevelEvent;

	private long m_batteryLevel;

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

	private byte[] m_dataRead;

	private static List<Dictionary<string, string>> m_deviceMap = null;

	private string m_servicePath;

	private object m_notificationLock = new object();

	private static string getServicePath(List<Dictionary<string, string>> deviceMap, string devicePath)
	{
		string value = "";
		if (deviceMap != null)
		{
			foreach (Dictionary<string, string> item in deviceMap)
			{
				if (item.TryGetValue("devicePath", out var value2) && string.Compare(devicePath, value2, ignoreCase: true) == 0 && item.TryGetValue("servicePath", out value))
				{
					break;
				}
			}
		}
		return value;
	}

	~MTBLEService()
	{
	}

	public override void setAddress(string address)
	{
		m_address = address;
		m_servicePath = getServicePath(m_deviceMap, m_address);
	}

	public static void requestDeviceList(Guid serviceGuid, Action<MTConnectionType, List<MTDeviceInformation>> deviceListEventHandler)
	{
		MTConnectionType mTConnectionType = MTConnectionType.BLEEMV;
		if (serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_EMV_DEVICE_READER_SERVICE))
		{
			mTConnectionType = MTConnectionType.BLEEMV;
		}
		else if (serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_EMV_T_DEVICE_READER_SERVICE))
		{
			mTConnectionType = MTConnectionType.BLEEMVT;
		}
		else if (serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_DEVICE_READER_SERVICE))
		{
			mTConnectionType = MTConnectionType.BLE;
		}
		List<BLEDeviceInformation> devices = MTBLEDevice.GetDevices(MTDeviceConstants.UUID_SCRA_BLE_DEVICE_INTERFACE);
		List<BLEDeviceInformation> devices2 = MTBLEDevice.GetDevices(serviceGuid);
		m_deviceMap = new List<Dictionary<string, string>>();
		if (devices2.Count > 0)
		{
			List<MTDeviceInformation> list = new List<MTDeviceInformation>();
			int num = 0;
			foreach (BLEDeviceInformation item in devices2)
			{
				if (item == null)
				{
					continue;
				}
				MTDeviceInformation mTDeviceInformation = new MTDeviceInformation();
				if (mTDeviceInformation == null)
				{
					continue;
				}
				num++;
				mTDeviceInformation.Name = item.FriendlyName;
				mTDeviceInformation.Address = item.DevicePath;
				mTDeviceInformation.Model = GetDeviceModel(mTConnectionType);
				if (string.IsNullOrWhiteSpace(mTDeviceInformation.Name))
				{
					foreach (BLEDeviceInformation item2 in devices)
					{
						if (item2 != null && string.Compare(item2.InstanceId, item.ParentInstanceId, ignoreCase: true) == 0)
						{
							mTDeviceInformation.Name = item2.FriendlyName;
							mTDeviceInformation.Address = item2.DevicePath;
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							dictionary.Add("devicePath", item2.DevicePath);
							dictionary.Add("servicePath", item.DevicePath);
							m_deviceMap.Add(dictionary);
							break;
						}
					}
					if (string.IsNullOrWhiteSpace(mTDeviceInformation.Name))
					{
						mTDeviceInformation.Name = mTDeviceInformation.Model + " " + num;
					}
				}
				list.Add(mTDeviceInformation);
			}
			deviceListEventHandler?.Invoke(mTConnectionType, list);
		}
		else
		{
			deviceListEventHandler?.Invoke(mTConnectionType, new List<MTDeviceInformation>());
		}
	}

	internal static string GetDeviceModel(MTConnectionType connectionType)
	{
		string result = "eDynamo";
		switch (connectionType)
		{
		case MTConnectionType.BLEEMV:
			result = "eDynamo";
			break;
		case MTConnectionType.BLEEMVT:
			result = "tDynamo";
			break;
		case MTConnectionType.BLE:
			result = "DynaMax";
			break;
		}
		return result;
	}

	protected void OnStateChanged(bool state)
	{
		if (state)
		{
			setState(MTServiceState.Connected);
			m_dataRead = null;
		}
		else
		{
			setState(MTServiceState.Disconnected);
		}
	}

	protected void OnCardDataReceived(byte[] data)
	{
		_ = data?.LongLength;
	}

	protected void OnNotificationReceived(byte[] data)
	{
		lock (m_notificationLock)
		{
			int num = data.Length;
			if (num <= 0)
			{
				return;
			}
			uint num2 = 0u;
			if (num >= 4)
			{
				num2 = (uint)(((data[3] & 0xFF) << 8) + (data[2] & 0xFF));
			}
			if (num2 == 0)
			{
				return;
			}
			byte[] array = null;
			if (data[0] == 0)
			{
				array = m_bleDevice.ReadCommandData((int)num2);
				if (array != null && array.Length != 0)
				{
					handleCommandResponse(array);
				}
				byte[] data2 = new byte[3]
				{
					data[0],
					data[1],
					0
				};
				m_bleDevice.WriteStatus(data2);
			}
			else
			{
				if (data[0] != 1)
				{
					return;
				}
				array = m_bleDevice.ReadCardData((int)num2);
				if (array != null)
				{
					if (array.Length != 0)
					{
						if (m_dataRead == null)
						{
							m_dataRead = array;
						}
						else
						{
							byte[] array2 = new byte[m_dataRead.Length + array.Length];
							Array.Copy(m_dataRead, 0, array2, 0, m_dataRead.Length);
							Array.Copy(array, 0, array2, m_dataRead.Length, array.Length);
							m_dataRead = array2;
						}
					}
					if (array.Length < 512 && handleDeviceData(m_dataRead))
					{
						m_dataRead = null;
					}
				}
				byte[] data3 = new byte[3]
				{
					data[0],
					data[1],
					0
				};
				m_bleDevice.WriteStatus(data3);
			}
		}
	}

	private bool handleDeviceData(byte[] dataRead)
	{
		bool result = false;
		byte b = 0;
		byte[] array = null;
		if (dataRead != null && dataRead.Length > 2)
		{
			b = dataRead[0];
			int num = dataRead[1] & 0xFF;
			num <<= 8;
			num += dataRead[2] & 0xFF;
			if (num > 0)
			{
				int num2 = dataRead.Length - 3;
				if (num2 > 0)
				{
					byte[] array2 = new byte[num2];
					Array.Copy(dataRead, 3, array2, 0, num2);
					switch (b)
					{
					case 0:
						array = array2;
						break;
					case 1:
						array = handleCompressedData(array2, num);
						break;
					case 2:
						array = array2;
						break;
					case 3:
						array = handleCompressedData(array2, num);
						break;
					}
				}
			}
		}
		if (array != null && array.Length != 0)
		{
			_ = (byte[])array.Clone();
			switch (b)
			{
			case 0:
			case 1:
				if (m_cardDataHandler != null)
				{
					m_cardDataHandler(array);
				}
				break;
			case 2:
			case 3:
				if (m_deviceDataHandler != null)
				{
					m_deviceDataHandler(array);
				}
				break;
			}
			result = true;
		}
		return result;
	}

	public static byte[] handleCompressedData(byte[] data, int reportLength)
	{
		byte[] array = MTRLEData.decodeRLEData(data);
		if (array != null)
		{
			int num = array.Length;
			if (reportLength != num)
			{
				return null;
			}
		}
		return array;
	}

	private void handleCommandResponse(byte[] data)
	{
		if (m_batteryLevelEvent != null)
		{
			if (isPM2Device())
			{
				if (data.Length >= 6)
				{
					if (data[0] == 0 && data[2] == 1 && data[3] == 1 && data[4] == 0)
					{
						m_batteryLevel = data[5];
					}
					else
					{
						m_batteryLevel = MTDeviceConstants.BATTERY_LEVEL_NA;
					}
				}
			}
			else if (data.Length >= 3)
			{
				if (data[0] == 0 && data[1] == 1)
				{
					m_batteryLevel = data[2];
				}
				else
				{
					m_batteryLevel = MTDeviceConstants.BATTERY_LEVEL_NA;
				}
			}
			m_batteryLevelEvent.Set();
		}
		else if (m_firmwareIDEvent != null)
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

	public override void connect()
	{
		m_bleDevice = new MTBLEDevice();
		if (m_bleDevice != null)
		{
			setState(MTServiceState.Connecting);
			m_bleDevice.connect(m_serviceGuid, m_address, m_servicePath, m_deviceID, OnStateChanged, OnCardDataReceived, OnNotificationReceived);
		}
	}

	public override void disconnect()
	{
		if (m_bleDevice != null)
		{
			if (m_state == MTServiceState.Connected)
			{
				sendData(MTDeviceConstants.SCRA_DEVICE_COMMAND_BLE_DISCONNECT);
			}
			setState(MTServiceState.Disconnecting);
			Task.Factory.StartNew((Func<object, Task>)async delegate
			{
				await Task.Delay(1000);
				m_bleDevice.Close();
			}, (object)null);
		}
	}

	public override bool sendData(byte[] data)
	{
		bool result = false;
		if (data != null && data.Length != 0)
		{
			result = m_bleDevice.WriteData(data);
		}
		return result;
	}

	protected bool isPM2Device()
	{
		bool result = false;
		try
		{
			result = string.Compare(getDevicePMValue(), "PM2", ignoreCase: true) == 0;
		}
		catch (Exception)
		{
		}
		return result;
	}

	public override long getBatteryLevel()
	{
		long result = MTDeviceConstants.BATTERY_LEVEL_NA;
		if (m_state == MTServiceState.Connected)
		{
			m_batteryLevelEvent = new AutoResetEvent(initialState: false);
			getDevicePMValue();
			if (isPM2Device())
			{
				sendData(MTDeviceConstants.SCRA_DEVICE_COMMAND_BLE_GET_BATTERY_LEVEL);
			}
			else
			{
				sendData(MTDeviceConstants.SCRA_DEVICE_COMMAND_GET_BATTERY_LEVEL);
			}
			if (m_batteryLevelEvent.WaitOne(COMMAND_RESPONSE_TIMEOUT))
			{
				result = m_batteryLevel;
			}
			m_batteryLevelEvent = null;
		}
		return result;
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
			bool flag = false;
			for (int i = 0; i < 10; i++)
			{
				if (m_capMagneSafe20EncryptionEvent.WaitOne(10))
				{
					flag = true;
					break;
				}
				Thread.Sleep(COMMAND_RESPONSE_TIMEOUT / 10);
			}
			if (flag)
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
		return true;
	}

	public override bool isServiceEMV()
	{
		return true;
	}

	public override bool isServiceOEM()
	{
		return false;
	}

	public override string getDevicePMValue()
	{
		string result = "";
		if (m_serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_DEVICE_READER_SERVICE))
		{
			result = "PM2";
		}
		else if (m_serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_EMV_DEVICE_READER_SERVICE))
		{
			result = "PM3";
		}
		else if (m_serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_EMV_T_DEVICE_READER_SERVICE))
		{
			result = "PM5";
		}
		return result;
	}

	public override MTDeviceFeatures getDeviceFeatures()
	{
		MTDeviceFeatures mTDeviceFeatures = new MTDeviceFeatures();
		mTDeviceFeatures.MSR = true;
		if (m_serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_DEVICE_READER_SERVICE))
		{
			mTDeviceFeatures.BatteryBackedClock = true;
		}
		else if (m_serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_EMV_DEVICE_READER_SERVICE))
		{
			mTDeviceFeatures.Contact = true;
			mTDeviceFeatures.BatteryBackedClock = true;
		}
		else if (m_serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_EMV_T_DEVICE_READER_SERVICE))
		{
			mTDeviceFeatures.Contact = true;
			mTDeviceFeatures.Contactless = true;
			mTDeviceFeatures.MSRPowerSaver = true;
		}
		return mTDeviceFeatures;
	}

	public static MTDeviceFeatures GetDeviceFeatures(Guid serviceGuid)
	{
		MTDeviceFeatures mTDeviceFeatures = new MTDeviceFeatures();
		mTDeviceFeatures.MSR = true;
		if (serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_DEVICE_READER_SERVICE))
		{
			mTDeviceFeatures.BatteryBackedClock = true;
		}
		else if (serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_EMV_DEVICE_READER_SERVICE))
		{
			mTDeviceFeatures.Contact = true;
			mTDeviceFeatures.BatteryBackedClock = true;
		}
		else if (serviceGuid.Equals(MTDeviceConstants.UUID_SCRA_BLE_EMV_T_DEVICE_READER_SERVICE))
		{
			mTDeviceFeatures.Contact = true;
			mTDeviceFeatures.Contactless = true;
			mTDeviceFeatures.MSRPowerSaver = true;
		}
		return mTDeviceFeatures;
	}
}
