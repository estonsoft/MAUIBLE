// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PANDataBuilder


public class PANDataBuilder
{
	public static PANData GetPANData(DeviceType deviceType, byte[] dataBytes)
	{
		PANData result = null;
		if (deviceType == DeviceType.MMS)
		{
			result = MMSAPI.getPANData(dataBytes);
		}
		return result;
	}
}
