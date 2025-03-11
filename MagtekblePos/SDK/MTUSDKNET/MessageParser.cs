// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MessageParser
using System;


public class MessageParser
{
	public static Message GetMessage(byte[] data)
	{
		Message result = null;
		if (data != null)
		{
			int num = data.Length;
			if (num >= 2)
			{
				byte[] tag = new byte[2]
				{
					data[0],
					data[1]
				};
				int num2 = num - 2;
				if (num2 > 0)
				{
					byte[] array = new byte[num2];
					Array.Copy(data, 2, array, 0, num2);
					result = new Message(tag, array);
				}
			}
		}
		return result;
	}

	public static Command GetCommand(byte[] data)
	{
		Command result = null;
		if (data != null)
		{
			int num = data.Length;
			if (num >= 2)
			{
				byte[] tag = new byte[2]
				{
					data[0],
					data[1]
				};
				int num2 = num - 2;
				if (num2 > 0)
				{
					byte[] array = new byte[num2];
					Array.Copy(data, 2, array, 0, num2);
					result = new Command(tag, array);
				}
			}
		}
		return result;
	}

	public static ResponsePayload GetResponsePayload(byte[] data)
	{
		ResponsePayload result = null;
		if (data != null)
		{
			int num = data.Length;
			if (num >= 2)
			{
				byte[] tag = new byte[2]
				{
					data[0],
					data[1]
				};
				int num2 = num - 2;
				if (num2 > 0)
				{
					byte[] array = new byte[num2];
					Array.Copy(data, 2, array, 0, num2);
					result = new ResponsePayload(tag, array);
				}
			}
		}
		return result;
	}

	public static NotificationPayload GetNotificationPayload(byte[] data)
	{
		NotificationPayload result = null;
		if (data != null)
		{
			int num = data.Length;
			if (num >= 2)
			{
				byte[] tag = new byte[2]
				{
					data[0],
					data[1]
				};
				int num2 = num - 2;
				if (num2 > 0)
				{
					byte[] array = new byte[num2];
					Array.Copy(data, 2, array, 0, num2);
					result = new NotificationPayload(tag, array);
				}
			}
		}
		return result;
	}
}
