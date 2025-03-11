// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXAPI
public class MMXAPI
{
	public static string[] GetDeviceList(MMXConnectionType connectionType)
	{
		string[] result = null;
		switch (connectionType)
		{
		case MMXConnectionType.USB:
			result = MMXUSBService.GetDeviceList();
			break;
		case MMXConnectionType.Serial:
			result = MMXSerialService.GetDeviceList();
			break;
		}
		return result;
	}
}
