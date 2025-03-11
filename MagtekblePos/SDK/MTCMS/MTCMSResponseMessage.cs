// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTCMSResponseMessage


public class MTCMSResponseMessage : MTCMSMessage
{
	public MTCMSResponseMessage(int applicationID, int commandID, int resultCode, int dataTag, byte[] data)
		: base(2, applicationID, commandID, resultCode, dataTag, data)
	{
	}
}
