// MLog, Version=1.0.0.2, Culture=neutral, PublicKeyToken=0f9071f8ab811b68
// MagTek.LoggerFlags
using System.ComponentModel;

public enum LoggerFlags
{
	[Description("Device")]
	LF_DEVICE = 2,
	[Description("Communication")]
	LF_COMMUNICATION = 4,
	[Description("Protocol")]
	LF_PROTOCOL = 8,
	[Description("Function")]
	LF_FUNCTION = 0x10,
	[Description("Application")]
	LF_APPLICATION = 0x20,
	[Description("Trace")]
	LF_TRACE = 0x100,
	[Description("Error")]
	LF_ERROR = 0x200,
	[Description("Critical")]
	LF_CRITICAL = 0x400
}
