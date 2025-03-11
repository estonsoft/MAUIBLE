// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.IMTPPSCRADevice


internal interface IMTPPSCRADevice
{
	bool initialize(IMTService service);

	bool open();

	void close();

	bool sendCommand(byte[] command);
}
