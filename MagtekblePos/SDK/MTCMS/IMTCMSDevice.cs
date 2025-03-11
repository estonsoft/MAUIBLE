// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.IMTCMSDevice


internal interface IMTCMSDevice
{
	bool initialize(IMTService service);

	bool open();

	void close();

	bool sendCommand(byte[] command);
}
