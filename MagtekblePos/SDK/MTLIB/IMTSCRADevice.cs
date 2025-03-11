// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.IMTSCRADevice


internal interface IMTSCRADevice
{
	bool initialize(IMTService service, MTDataFormat dataFormat, int dataThreshold);

	bool open();

	void close();

	bool sendCommand(byte[] command);
}
