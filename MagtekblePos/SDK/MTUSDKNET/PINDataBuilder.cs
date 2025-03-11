// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PINDataBuilder


public class PINDataBuilder
{
	public static PINData GetPINData(DeviceType deviceType, byte[] dataBytes)
	{
		PINData result = null;
		switch (deviceType)
		{
		case DeviceType.PPSCRA:
			result = PPSCRAAPI.getPINData(dataBytes);
			break;
		case DeviceType.MMS:
			result = MMSAPI.getPINData(dataBytes);
			break;
		}
		return result;
	}
}
