// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.PIN_DATA_PARSER


internal class PIN_DATA_PARSER : PARSER
{
	internal PIN_DATA lastPinData;

	public PIN_DATA_PARSER()
	{
		lastPinData = default(PIN_DATA);
	}

	public override int parse(byte[] data, int dataLen)
	{
		lastPinData.KSN = PARSER.toHexString(data, 2, 10);
		lastPinData.EPB = PARSER.toHexString(data, 12, 8);
		lastPinData.OpStatus = data[1];
		return Status;
	}
}
