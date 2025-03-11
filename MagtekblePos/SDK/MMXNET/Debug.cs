// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.Debug
using System;
using System.IO;
using System.Reflection;

internal class Debug
{
	private static Action<string> logErrAction;

	private static Action<string> logCommunicationAction;

	private static Action<string> logDeviceAction;

	private static Action<string> logTraceAction;

	private static Action<string> logProtocolAction;

	static Debug()
	{
		try
		{
			Assembly assembly = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mlog.dll"));
			if (assembly != null)
			{
				PropertyInfo property = assembly.GetType("MagTek.Logger").GetProperty("Default");
				object logClass = property.GetValue(null);
				MethodInfo logMethod = assembly.GetType("MagTek.Logger").GetMethod("Log");
				Type type = assembly.GetType("MagTek.LoggerFlags");
				object TraceFlag = Enum.Parse(type, "LF_TRACE");
				object ErrorFlag = Enum.Parse(type, "LF_ERROR");
				object CommFlag = Enum.Parse(type, "LF_COMMUNICATION");
				object ProtocolFlag = Enum.Parse(type, "LF_PROTOCOL");
				object DeviceFlag = Enum.Parse(type, "LF_DEVICE");
				logErrAction = delegate(string x)
				{
					logMethod.Invoke(logClass, new object[2] { ErrorFlag, x });
				};
				logTraceAction = delegate(string x)
				{
					logMethod.Invoke(logClass, new object[2] { TraceFlag, x });
				};
				logCommunicationAction = delegate(string x)
				{
					logMethod.Invoke(logClass, new object[2] { CommFlag, x });
				};
				logProtocolAction = delegate(string x)
				{
					logMethod.Invoke(logClass, new object[2] { ProtocolFlag, x });
				};
				logDeviceAction = delegate(string x)
				{
					logMethod.Invoke(logClass, new object[2] { DeviceFlag, x });
				};
			}
		}
		catch (Exception)
		{
		}
	}

	public static void LogDevice(string Info)
	{
		if (logDeviceAction != null)
		{
			logDeviceAction(Info);
		}
	}

	public static void LogCommunication(string Info)
	{
		if (logCommunicationAction != null)
		{
			logCommunicationAction(Info);
		}
	}

	public static void LogError(string Info)
	{
		if (logErrAction != null)
		{
			logErrAction(Info);
		}
	}

	public static void LogTrace(string Info)
	{
		if (logTraceAction != null)
		{
			logTraceAction(Info);
		}
	}

	public static void LogProtocol(string Info)
	{
		if (logProtocolAction != null)
		{
			logProtocolAction(Info);
		}
	}
}
