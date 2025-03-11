// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.DEV_STATE_STAT
public struct DEV_STATE_STAT
{
	public byte nDeviceState;

	public byte nSessionState;

	public byte nDeviceStatus;

	public byte nDevCertStatus;

	public byte nHWStatus;

	public byte nICCMasterSessKeyStatus;

	public override string ToString()
	{
		return $"Device State:{nDeviceState}\n" + $"Session State:{nSessionState}\n" + $"Device Status:{nDeviceStatus}\n" + $"Device Certificate Status:{nDevCertStatus}\n" + $"Hardware Status:{nHWStatus}\n" + $"ICC Master Session Key Status:{nICCMasterSessKeyStatus}\n";
	}
}
