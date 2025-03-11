// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.DeviceEventBuilder
using System;


public class DeviceEventBuilder
{
	public static string DEVICE_RESET_OCCURRED = "device_reset_occurred";

	public static string DEVICE_RESET_WILL_OCCUR = "device_reset_will_occur";

	public static DeviceEvent GetEventValue(string data)
	{
		DeviceEvent result = DeviceEvent.None;
		string text = data;
		if (data.Contains(","))
		{
			string[] array = data.Split(new char[1] { ',' });
			if (array.Length != 0)
			{
				text = array[0];
			}
		}
		if (text != null)
		{
			try
			{
				if (text.CompareTo(DEVICE_RESET_OCCURRED) == 0)
				{
					result = DeviceEvent.DeviceResetOccurred;
				}
				else if (text.CompareTo(DEVICE_RESET_WILL_OCCUR) == 0)
				{
					result = DeviceEvent.DeviceResetWillOccur;
				}
			}
			catch (Exception)
			{
			}
		}
		return result;
	}

	public static string GetDetail(string data)
	{
		string result = "";
		if (data.Contains(","))
		{
			string[] array = data.Split(new char[1] { ',' });
			if (array.Length > 1)
			{
				result = array[1];
			}
		}
		return result;
	}

	public static string GetString(DeviceEvent value)
	{
		string result = "";
		switch (value)
		{
		case DeviceEvent.DeviceResetOccurred:
			result = DEVICE_RESET_OCCURRED;
			break;
		case DeviceEvent.DeviceResetWillOccur:
			result = DEVICE_RESET_WILL_OCCUR;
			break;
		}
		return result;
	}
}
