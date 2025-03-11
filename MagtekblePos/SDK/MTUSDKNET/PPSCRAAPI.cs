// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PPSCRAAPI
using System.Collections.Generic;
using System.Text;




public class PPSCRAAPI
{
	internal static Dictionary<MTConnectionType, string> MTConnectionTypeURI = new Dictionary<MTConnectionType, string>
	{
		{
			(MTConnectionType)0,
			"UNKNOWN://"
		},
		{
			(MTConnectionType)5,
			"USB://"
		},
		{
			(MTConnectionType)2,
			"BLE://"
		},
		{
			(MTConnectionType)3,
			"BLEEMV://"
		},
		{
			(MTConnectionType)7,
			"IP://"
		},
		{
			(MTConnectionType)8,
			"TLS12://"
		},
		{
			(MTConnectionType)9,
			"TLS12TRUST://"
		}
	};

	public static IDevice createDevice(ConnectionType connectionType, string address, string model, string name, CertificateInfo certificateInfo = null)
	{
		string connectionTypeString = CoreAPI.GetConnectionTypeString(connectionType);
		ConnectionInfo connectionInfo = new ConnectionInfo(DeviceType.PPSCRA, connectionType, address, certificateInfo);
		DeviceInfo deviceInfo = new DeviceInfo("[" + connectionTypeString + " " + model + "] " + name, model);
		return new PPSCRADevice(connectionInfo, deviceInfo);
	}

	public static List<IDevice> getDeviceList()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		List<IDevice> list = new List<IDevice>();
		string deviceList = new MTPPSCRA().getDeviceList();
		if (!string.IsNullOrWhiteSpace(deviceList))
		{
			string[] array = deviceList.Split(new char[1] { ',' });
			if (array != null)
			{
				foreach (string text in array)
				{
					string text2 = text;
					ConnectionType connectionTypeFromMTConnectionType = getConnectionTypeFromMTConnectionType(GetConnectionTypeFromDevicePath(text));
					string name = "DynaPro";
					if (text != null)
					{
						int num = text.IndexOf("://");
						if (num >= 0)
						{
							text2 = text.Substring(num + 3);
							name = text2;
						}
					}
					IDevice device = createDevice(connectionTypeFromMTConnectionType, text2, "DynaPro", name);
					if (device != null)
					{
						list.Add(device);
					}
				}
			}
		}
		return list;
	}

	internal static MTConnectionType GetConnectionTypeFromDevicePath(string devicePath)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		MTConnectionType result = (MTConnectionType)0;
		foreach (KeyValuePair<MTConnectionType, string> item in MTConnectionTypeURI)
		{
			if (devicePath.ToUpper().StartsWith(item.Value))
			{
				result = item.Key;
				break;
			}
		}
		return result;
	}

	internal static string GetConnectionTypeURI(MTConnectionType mtConnectionType)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		string result = "";
		if (MTConnectionTypeURI.ContainsKey(mtConnectionType))
		{
			result = MTConnectionTypeURI[mtConnectionType];
		}
		return result;
	}

	internal static ConnectionType getConnectionTypeFromMTConnectionType(MTConnectionType mtConnectionType)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected I4, but got Unknown
		ConnectionType result = ConnectionType.USB;
		switch (mtConnectionType - 3)
		{
		case 2:
			result = ConnectionType.USB;
			break;
		case 0:
			result = ConnectionType.BLUETOOTH_LE_EMV;
			break;
		case 4:
			result = ConnectionType.TCP;
			break;
		case 5:
			result = ConnectionType.TCP_TLS;
			break;
		case 6:
			result = ConnectionType.TCP_TLS_TRUST;
			break;
		}
		return result;
	}

	internal static PINData getPINData(byte[] dataBytes)
	{
		PINData result = null;
		if (dataBytes != null)
		{
			string @string = Encoding.UTF8.GetString(dataBytes);
			if (@string.Contains(","))
			{
				string[] array = @string.Split(new char[1] { ',' });
				if (array.Length >= 2)
				{
					byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(array[0]);
					result = new PINData(MTParser.getByteArrayFromHexString(array[1]), byteArrayFromHexString, 0, 0);
				}
			}
		}
		return result;
	}
}
