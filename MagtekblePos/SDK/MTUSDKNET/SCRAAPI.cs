// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.SCRAAPI
using System.Collections.Generic;
using System.Threading;
using MTLIB;
using MTSCRANET;


public class SCRAAPI
{
	public static IDevice createDevice(ConnectionType connectionType, string address, string model, string name)
	{
		string connectionTypeString = CoreAPI.GetConnectionTypeString(connectionType);
		ConnectionInfo connectionInfo = new ConnectionInfo(DeviceType.SCRA, connectionType, address);
		DeviceInfo deviceInfo = new DeviceInfo("[" + connectionTypeString + " " + model + "] " + name, model);
		return new SCRADevice(connectionInfo, deviceInfo);
	}

	public static List<IDevice> getDeviceList()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		List<IDevice> deviceList = new List<IDevice>();
		AutoResetEvent usbDeviceEvent = new AutoResetEvent(initialState: false);
		AutoResetEvent bleemvDeviceEvent = new AutoResetEvent(initialState: false);
		AutoResetEvent bleemvtDeviceEvent = new AutoResetEvent(initialState: false);
		MTSCRA val = new MTSCRA();
		val.OnDeviceList += new DeviceListHandler(OnDeviceList);
		val.requestDeviceList((MTConnectionType)5);
		usbDeviceEvent.WaitOne(3000);
		val.requestDeviceList((MTConnectionType)3);
		bleemvDeviceEvent.WaitOne(3000);
		val.requestDeviceList((MTConnectionType)10);
		bleemvtDeviceEvent.WaitOne(3000);
		return deviceList;
		void OnDeviceList(object sender, MTConnectionType connectionType, List<MTDeviceInformation> scraDevices)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Invalid comparison between Unknown and I4
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Invalid comparison between Unknown and I4
			if ((int)connectionType == 5)
			{
				if (scraDevices != null)
				{
					foreach (MTDeviceInformation scraDevice in scraDevices)
					{
						IDevice device = createDevice(ConnectionType.USB, scraDevice.Address, scraDevice.Model, scraDevice.Name);
						if (device != null)
						{
							deviceList.Add(device);
						}
					}
				}
				usbDeviceEvent.Set();
			}
			else if ((int)connectionType == 3)
			{
				if (scraDevices != null)
				{
					foreach (MTDeviceInformation scraDevice2 in scraDevices)
					{
						IDevice device2 = createDevice(ConnectionType.BLUETOOTH_LE_EMV, scraDevice2.Address, scraDevice2.Model, scraDevice2.Name);
						if (device2 != null)
						{
							deviceList.Add(device2);
						}
					}
				}
				bleemvDeviceEvent.Set();
			}
			else if ((int)connectionType == 10)
			{
				if (scraDevices != null)
				{
					foreach (MTDeviceInformation scraDevice3 in scraDevices)
					{
						IDevice device3 = createDevice(ConnectionType.BLUETOOTH_LE_EMVT, scraDevice3.Address, scraDevice3.Model, scraDevice3.Name);
						if (device3 != null)
						{
							deviceList.Add(device3);
						}
					}
				}
				bleemvtDeviceEvent.Set();
			}
		}
	}
}
