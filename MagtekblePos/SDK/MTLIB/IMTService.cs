// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.IMTService
using System;


public interface IMTService
{
	void initialize(Action<MTServiceState> serviceStateHandler, Action<byte[]> commandDataHandler, Action<byte[]> deviceDataHandler, Action<byte[]> cardDataEventHandler, Action<MTCardDataError> cardDataErrorHandler);

	void setAddress(string address);

	void setDeviceID(string deviceID);

	void setServiceUUID(Guid serviceUUID);

	MTServiceState getState();

	void connect();

	void disconnect();

	bool sendData(byte[] data);

	long getBatteryLevel();

	string getDeviceName();

	string getFirmwareID();

	string getDeviceSerial();

	string getCapMagnePrint();

	string getCapMagnePrintEncryption();

	string getCapMagneSafe20Encryption();

	string getCapMagStripeEncryption();

	string getCapMSR();

	string getCapTracks();

	bool isOutputChannelConfigurable();

	bool isServiceEMV();

	bool isServiceOEM();

	string getDevicePMValue();

	MTDeviceFeatures getDeviceFeatures();
}
