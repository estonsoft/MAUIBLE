// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.IPADDevInfo
public struct IPADDevInfo
{
	public string Model;

	public string DevicePath;

	public string Serial;

	public string FWVersion;

	public int Version;

	public int PID;

	public int VID;

	public void clear()
	{
		VID = 0;
		PID = 0;
		Version = 0;
		FWVersion = "";
		Serial = "";
		DevicePath = "";
		Model = "";
	}

	public override string ToString()
	{
		return $"VID={VID:x} VID={PID:x} Version = {Version}\n" + "Model:" + Model + "\nSerial:" + Serial + "\nFWVersion:" + FWVersion + "\nDevicePath:" + DevicePath;
	}
}
