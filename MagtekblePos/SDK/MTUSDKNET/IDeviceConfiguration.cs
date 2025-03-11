// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.IDeviceConfiguration


public interface IDeviceConfiguration
{
	string getDeviceInfo(InfoType infoType);

	byte[] getConfigInfo(byte configType, byte[] data);

	byte[] getKeyInfo(byte keyType, byte[] data);

	byte[] getChallengeToken(byte[] data);

	int setConfigInfo(byte configType, byte[] data, IConfigurationCallback callback);

	int updateKeyInfo(byte keyType, byte[] data, IConfigurationCallback callback);

	int updateFirmware(ushort firmwareType, byte[] data, IConfigurationCallback callback);

	int getFile(byte[] fileID, IConfigurationCallback callback);

	int sendFile(byte[] fileID, byte[] data, IConfigurationCallback callback);

	int sendSecureFile(byte[] fileID, byte[] data, IConfigurationCallback callback);

	int sendImage(byte imageID, byte[] data, IConfigurationCallback callback);

	int setDisplayImage(byte imageID);
}
