// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTCMSRequestMessage


public class MTCMSRequestMessage : MTCMSMessage
{
	public MTCMSRequestMessage(int applicationID, int commandID, int dataTag, byte[] data)
		: base(1, applicationID, commandID, 0, dataTag, data)
	{
	}
}
