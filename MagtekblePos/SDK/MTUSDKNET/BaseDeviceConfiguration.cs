// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.BaseDeviceConfiguration


public class BaseDeviceConfiguration : IDeviceConfiguration
{
	public virtual string getDeviceInfo(InfoType infoType)
	{
		return "";
	}

	public virtual byte[] getConfigInfo(byte configType, byte[] data)
	{
		return null;
	}

	public virtual byte[] getKeyInfo(byte keyType, byte[] data)
	{
		return null;
	}

	public virtual byte[] getChallengeToken(byte[] data)
	{
		return null;
	}

	public virtual int setConfigInfo(byte configType, byte[] data, IConfigurationCallback callback)
	{
		return -1;
	}

	public virtual int updateKeyInfo(byte keyType, byte[] data, IConfigurationCallback callback)
	{
		return -1;
	}

	public virtual int updateFirmware(ushort firmwareType, byte[] data, IConfigurationCallback callback)
	{
		return -1;
	}

	public virtual int getFile(byte[] fileID, IConfigurationCallback callback)
	{
		return -1;
	}

	public virtual int sendFile(byte[] fileID, byte[] data, IConfigurationCallback callback)
	{
		return -1;
	}

	public virtual int sendSecureFile(byte[] fileID, byte[] data, IConfigurationCallback callback)
	{
		return -1;
	}

	public virtual int sendImage(byte imageID, byte[] data, IConfigurationCallback callback)
	{
		return -1;
	}

	public virtual int setDisplayImage(byte imageID)
	{
		return -1;
	}
}
