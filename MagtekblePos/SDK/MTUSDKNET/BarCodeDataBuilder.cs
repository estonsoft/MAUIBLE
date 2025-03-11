// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.BarCodeDataBuilder


public class BarCodeDataBuilder
{
	public static BarCodeData GetBarCodeData(DeviceType deviceType, byte[] dataBytes)
	{
		BarCodeData result = null;
		if (deviceType == DeviceType.MMS)
		{
			result = MMSAPI.getBarCodeData(dataBytes);
		}
		return result;
	}
}
