// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.IMTPPSCRA


public interface IMTPPSCRA
{
	bool setConnectionType(MTConnectionType connectionType);

	bool openDevice();

	void closeDevice();
}
