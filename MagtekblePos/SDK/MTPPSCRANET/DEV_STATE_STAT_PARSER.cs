// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.DEV_STATE_STAT_PARSER


internal class DEV_STATE_STAT_PARSER : PARSER
{
	public DEV_STATE_STAT devState;

	public override int parse(byte[] data, int dataLen)
	{
		devState.nDeviceState = data[1];
		devState.nSessionState = data[2];
		devState.nDeviceStatus = data[3];
		devState.nDevCertStatus = data[4];
		devState.nHWStatus = data[5];
		devState.nICCMasterSessKeyStatus = data[6];
		return Status;
	}
}
