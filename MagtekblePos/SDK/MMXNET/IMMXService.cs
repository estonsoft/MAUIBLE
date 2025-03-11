// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.IMMXService
using System;

internal interface IMMXService
{
	event EventHandler<byte[]> onData;

	event EventHandler<MMXConnectionState> onConnectionChanged;

	string[] getDeviceList();

	void setDeviceID(string ID);

	void connect();

	void disconnect();

	void sendData(byte[] data);

	bool isConneted();
}
