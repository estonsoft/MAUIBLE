// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.ConnectionStateBuilder
using System;


public class ConnectionStateBuilder
{
	public static string DISCONNECTED = "disconnected";

	public static string CONNECTING = "connecting";

	public static string ERROR = "error";

	public static string CONNECTED = "connected";

	public static string DISCONNECTING = "disconnecting";

	public static ConnectionState GetValue(string data)
	{
		ConnectionState result = ConnectionState.Unknown;
		if (data != null)
		{
			try
			{
				if (data.CompareTo(DISCONNECTED) == 0)
				{
					result = ConnectionState.Disconnected;
				}
				else if (data.CompareTo(CONNECTING) == 0)
				{
					result = ConnectionState.Connecting;
				}
				else if (data.CompareTo(ERROR) == 0)
				{
					result = ConnectionState.Error;
				}
				else if (data.CompareTo(CONNECTED) == 0)
				{
					result = ConnectionState.Connected;
				}
				else if (data.CompareTo(DISCONNECTING) == 0)
				{
					result = ConnectionState.Disconnecting;
				}
			}
			catch (Exception)
			{
			}
		}
		return result;
	}

	public static string GetString(ConnectionState value)
	{
		string result = "";
		switch (value)
		{
		case ConnectionState.Connecting:
			result = CONNECTING;
			break;
		case ConnectionState.Connected:
			result = CONNECTED;
			break;
		case ConnectionState.Disconnecting:
			result = DISCONNECTING;
			break;
		case ConnectionState.Disconnected:
			result = DISCONNECTED;
			break;
		case ConnectionState.Error:
			result = ERROR;
			break;
		}
		return result;
	}
}
