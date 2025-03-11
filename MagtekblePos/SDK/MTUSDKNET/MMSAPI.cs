// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSAPI
using System.Collections.Generic;



public class MMSAPI
{
	public static IDevice createDevice(ConnectionType connectionType, string address, string model, string name)
	{
		string connectionTypeString = CoreAPI.GetConnectionTypeString(connectionType);
		ConnectionInfo connectionInfo = new ConnectionInfo(DeviceType.MMS, connectionType, address);
		DeviceInfo deviceInfo = new DeviceInfo("[" + connectionTypeString + " " + model + "] " + name, model);
		return new MMSDevice(connectionInfo, deviceInfo);
	}

	public static List<IDevice> getDeviceList()
	{
		List<IDevice> list = new List<IDevice>();
		string[] deviceList = MMXAPI.GetDeviceList(MMXConnectionType.USB);
		int num = 1;
		string[] array = deviceList;
		foreach (string obj in array)
		{
			string model = "DynaFlex";
			string name = "DynaFlex-" + num;
			string[] array2 = obj.Split(new char[1] { '|' });
			string text = "";
			if (array2 != null)
			{
				if (array2.Length > 1)
				{
					text = array2[1];
				}
				if (text != null && text.Contains("pid_2023"))
				{
					model = "DynaProx";
					name = "DynaProx-" + num;
				}
				if (array2.Length != 0)
				{
					name = array2[0];
				}
			}
			IDevice device = createDevice(ConnectionType.USB, text, model, name);
			if (device != null)
			{
				list.Add(device);
			}
			num++;
		}
		array = MMXAPI.GetDeviceList(MMXConnectionType.Serial);
		foreach (string text2 in array)
		{
			string model2 = "DynaProx";
			string name2 = "DynaProx-" + text2;
			IDevice device2 = createDevice(ConnectionType.SERIAL, text2, model2, name2);
			if (device2 != null)
			{
				list.Add(device2);
			}
		}
		return list;
	}

	internal static PINData getPINData(byte[] dataBytes)
	{
		PINData result = null;
		if (dataBytes != null)
		{
			List<TLVObject> list = TLVParser.parseTLVByteArray(dataBytes);
			if (list != null)
			{
				TLVObject tLVObject = TLVParser.findFromListByTagHexString(list, "F5");
				if (tLVObject != null)
				{
					byte format = 0;
					byte[] pinBlock = null;
					byte[] ksn = null;
					byte encryptionType = 0;
					TLVObject tLVObject2 = tLVObject.findChildByTagHexString("DFDF71");
					if (tLVObject2 != null)
					{
						byte[] valueByteArray = tLVObject2.getValueByteArray();
						if (valueByteArray != null && valueByteArray.Length != 0)
						{
							format = valueByteArray[0];
						}
					}
					TLVObject tLVObject3 = tLVObject.findChildByTagHexString("99");
					if (tLVObject3 != null)
					{
						pinBlock = tLVObject3.getValueByteArray();
					}
					TLVObject tLVObject4 = tLVObject.findChildByTagHexString("DFDF41");
					if (tLVObject4 != null)
					{
						ksn = tLVObject4.getValueByteArray();
					}
					TLVObject tLVObject5 = tLVObject.findChildByTagHexString("DFDF42");
					if (tLVObject5 != null)
					{
						byte[] valueByteArray2 = tLVObject5.getValueByteArray();
						if (valueByteArray2 != null && valueByteArray2.Length != 0)
						{
							encryptionType = valueByteArray2[0];
						}
					}
					result = new PINData(pinBlock, ksn, format, encryptionType);
				}
			}
		}
		return result;
	}

	internal static PANData getPANData(byte[] dataBytes)
	{
		PANData result = null;
		if (dataBytes != null)
		{
			List<TLVObject> list = TLVParser.parseTLVByteArray(dataBytes);
			if (list != null)
			{
				byte[] data = null;
				byte[] ksn = null;
				byte encryptionType = 0;
				TLVObject tLVObject = TLVParser.findFromListByTagHexString(list, "DFDF59");
				if (tLVObject != null)
				{
					data = tLVObject.getValueByteArray();
				}
				TLVObject tLVObject2 = TLVParser.findFromListByTagHexString(list, "DFDF56");
				if (tLVObject2 != null)
				{
					ksn = tLVObject2.getValueByteArray();
				}
				TLVObject tLVObject3 = TLVParser.findFromListByTagHexString(list, "DFDF57");
				if (tLVObject3 != null)
				{
					byte[] valueByteArray = tLVObject3.getValueByteArray();
					if (valueByteArray != null && valueByteArray.Length != 0)
					{
						encryptionType = valueByteArray[0];
					}
				}
				PINData pINData = getPINData(dataBytes);
				result = new PANData(data, ksn, encryptionType, pINData);
			}
		}
		return result;
	}

	internal static BarCodeData getBarCodeData(byte[] dataBytes)
	{
		BarCodeData result = null;
		if (dataBytes != null)
		{
			List<TLVObject> list = TLVParser.parseTLVByteArray(dataBytes);
			if (list != null)
			{
				byte[] data = null;
				byte[] ksn = null;
				byte encryptionType = 0;
				TLVObject tLVObject = TLVParser.findFromListByTagHexString(list, "DF74");
				if (tLVObject != null)
				{
					data = tLVObject.getValueByteArray();
					result = new BarCodeData(data, encrypted: false, 0);
				}
				else
				{
					TLVObject tLVObject2 = TLVParser.findFromListByTagHexString(list, "DFDF59");
					if (tLVObject2 != null)
					{
						data = tLVObject2.getValueByteArray();
					}
					TLVObject tLVObject3 = TLVParser.findFromListByTagHexString(list, "DFDF50");
					if (tLVObject3 != null)
					{
						ksn = tLVObject3.getValueByteArray();
					}
					TLVObject tLVObject4 = TLVParser.findFromListByTagHexString(list, "DFDF51");
					if (tLVObject4 != null)
					{
						byte[] valueByteArray = tLVObject4.getValueByteArray();
						if (valueByteArray != null && valueByteArray.Length != 0)
						{
							encryptionType = valueByteArray[0];
						}
					}
					result = new BarCodeData(data, encrypted: true, encryptionType, ksn);
				}
			}
		}
		return result;
	}
}
