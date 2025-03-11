// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTCMSNotificationMessage


public class MTCMSNotificationMessage : MTCMSMessage
{
	public MTCMSNotificationMessage(int applicationID, int commandID, int resultCode, int dataTag, byte[] data)
		: base(3, applicationID, commandID, resultCode, dataTag, data)
	{
	}
}
