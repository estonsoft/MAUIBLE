// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.IMTReportParser
internal interface IMTReportParser
{
	void parseReport(object sender, byte[] pData);

	byte getStatusCode();
}
