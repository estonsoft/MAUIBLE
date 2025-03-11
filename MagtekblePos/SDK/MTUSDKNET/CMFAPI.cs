// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.CMFAPI
using System.Collections.Generic;
using System.Threading;



public class CMFAPI
{
	public static IDevice createDevice(ConnectionType connectionType, string address, string model, string name)
	{
		string connectionTypeString = CoreAPI.GetConnectionTypeString(connectionType);
		ConnectionInfo connectionInfo = new ConnectionInfo(DeviceType.CMF, connectionType, address);
		DeviceInfo deviceInfo = new DeviceInfo("[" + connectionTypeString + " " + model + "] " + name, model);
		return new CMFDevice(connectionInfo, deviceInfo);
	}

	public static List<IDevice> getDeviceList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		List<IDevice> deviceList = new List<IDevice>();
		AutoResetEvent syncEvent = new AutoResetEvent(initialState: false);
		MTDevice val = new MTDevice();
		val.OnDeviceList += new DeviceListHandler(OnDeviceList);
		val.requestDeviceList((MTConnectionType)0);
		syncEvent.WaitOne(3000);
		return deviceList;
		void OnDeviceList(object sender, MTConnectionType connectionType, List<MTDeviceInformation> cmsDevices)
		{
			if (cmsDevices != null)
			{
				foreach (MTDeviceInformation cmsDevice in cmsDevices)
				{
					IDevice device = createDevice(ConnectionType.USB, cmsDevice.Address, "oDynamo", cmsDevice.Name);
					if (device != null)
					{
						deviceList.Add(device);
					}
				}
			}
			syncEvent.Set();
		}
	}
}
