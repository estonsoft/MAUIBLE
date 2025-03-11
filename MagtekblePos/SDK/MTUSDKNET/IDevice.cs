// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.IDevice


public interface IDevice
{
	string Name { get; }

	ConnectionInfo getConnectionInfo();

	ConnectionState getConnectionState();

	DeviceInfo getDeviceInfo();

	IDeviceCapabilities getCapabilities();

	IDeviceControl getDeviceControl();

	IDeviceConfiguration getDeviceConfiguration();

	bool subscribeAll(IEventSubscriber eventCallback);

	bool unsubscribeAll(IEventSubscriber eventCallback);

	bool startTransaction(ITransaction transaction);

	bool cancelTransaction();

	bool sendSelection(IData data);

	bool sendAuthorization(IData data);

	bool requestPIN(PINRequest pinRequest);

	bool requestPAN(PANRequest panRequest, PINRequest pinRequest = null);

	bool requestSignature(byte timeout);
}
