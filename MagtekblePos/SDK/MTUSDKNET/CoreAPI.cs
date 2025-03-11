// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.CoreAPI
using System.Collections.Generic;
public class CoreAPI
{
	private const int CORE_API_VERSION = 119;

	internal static Dictionary<ConnectionType, string> ConnectionTypeString = new Dictionary<ConnectionType, string>
	{
		{
			ConnectionType.USB,
			"USB"
		},
		{
			ConnectionType.BLUETOOTH_LE_EMV,
			"BLE"
		},
		{
			ConnectionType.BLUETOOTH_LE_EMVT,
			"BLE_T"
		},
		{
			ConnectionType.TCP,
			"TCP"
		},
		{
			ConnectionType.TCP_TLS,
			"TLS"
		},
		{
			ConnectionType.TCP_TLS_TRUST,
			"TLS_TRUST"
		},
		{
			ConnectionType.WEBSOCKET,
			"WebSocket"
		},
		{
			ConnectionType.SERIAL,
			"Serial"
		},
		{
			ConnectionType.VIRTUAL,
			"VDEV"
		}
	};

	public static int getAPIVersion()
	{
		return 119;
	}

	public static MTSCRA createSCRA()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		return new MTSCRA();
	}

	public static MTPPSCRA createPPSCRA()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		return new MTPPSCRA();
	}

	public static MTDevice createCMSDevice()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		return new MTDevice();
	}

	public static MMXDevice createMMSDevice()
	{
		return new MMXDevice();
	}

	public static List<IDevice> getDeviceList()
	{
		List<IDevice> deviceList = MMSAPI.getDeviceList();
		List<IDevice> deviceList2 = PPSCRAAPI.getDeviceList();
		List<IDevice> list = new List<IDevice>();
		list.AddRange(deviceList);
		list.AddRange(deviceList2);
		return list;
	}

	public static List<IDevice> getDeviceList(List<DeviceType> deviceTypes)
	{
		List<IDevice> list = new List<IDevice>();
		foreach (DeviceType deviceType in deviceTypes)
		{
			List<IDevice> list2 = null;
			switch (deviceType)
			{
			case DeviceType.SCRA:
				list2 = SCRAAPI.getDeviceList();
				break;
			case DeviceType.PPSCRA:
				list2 = PPSCRAAPI.getDeviceList();
				break;
			case DeviceType.CMF:
				list2 = CMFAPI.getDeviceList();
				break;
			case DeviceType.MMS:
				list2 = MMSAPI.getDeviceList();
				break;
			}
			if (list2 != null)
			{
				list.AddRange(list2);
			}
		}
		return list;
	}

	public static List<IDevice> getDeviceList(DeviceType deviceType)
	{
		return getDeviceList(new List<DeviceType> { deviceType });
	}

	public static IDevice createDevice(DeviceType deviceType, ConnectionType connectionType, string address, string model, string name, string serial, CertificateInfo certificateInfo = null)
	{
		IDevice result = null;
		switch (deviceType)
		{
		case DeviceType.SCRA:
			result = SCRAAPI.createDevice(connectionType, address, model, name);
			break;
		case DeviceType.PPSCRA:
			result = PPSCRAAPI.createDevice(connectionType, address, model, name, certificateInfo);
			break;
		case DeviceType.CMF:
			result = CMFAPI.createDevice(connectionType, address, model, name);
			break;
		case DeviceType.MMS:
			result = MMSAPI.createDevice(connectionType, address, model, name);
			break;
		}
		return result;
	}

	internal static string GetConnectionTypeString(ConnectionType connectionType)
	{
		string result = "";
		if (ConnectionTypeString.ContainsKey(connectionType))
		{
			result = ConnectionTypeString[connectionType];
		}
		return result;
	}
}
