// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTDeviceConstants
using System;

public class MTDeviceConstants
{
	public static ushort VID_MAGTEK = 2049;

	public static ushort PID_KB = 1;

	public static ushort PID_SWIPE = 17;

	public static ushort PID_BOOTLOADER = 18;

	public static ushort PID_INSERT = 19;

	public static ushort PID_AUDIO = 23;

	public static ushort PID_EMV_SWIPE = 25;

	public static ushort PID_EMV = 26;

	public static ushort PID_TDYNAMO = 28;

	public static ushort PID_KDYNAMO = 29;

	public static ushort PID_DYNAWAVE = 30;

	public static ushort PID_IDYNAMO6 = 31;

	public static ushort PID_IDYNAMO5G3 = 32;

	public static ushort PID_IDYNAMO5G3_KB = 33;

	public static ushort PID_ODM_BOOTLOADER = 21335;

	public static ushort[] PID_SCRA = new ushort[16]
	{
		PID_KB, PID_SWIPE, PID_INSERT, PID_AUDIO, PID_EMV_SWIPE, PID_EMV, PID_TDYNAMO, PID_KDYNAMO, PID_DYNAWAVE, PID_IDYNAMO6,
		PID_IDYNAMO5G3, PID_IDYNAMO5G3_KB, 2, 3, PID_BOOTLOADER, PID_ODM_BOOTLOADER
	};

	public static ushort[] PID_PINPAD = new ushort[2] { 12292, 12297 };

	public static string SCRA_BTH_DEVICE_SPP_SERVICE = "00001101-0000-1000-8000-00805F9B34FB";

	public static string SCRA_BLE_DEVICE_INTERFACE = "781aee18-7733-4ce4-add0-91f41c67b592";

	public static string SCRA_BLE_DEVICE_INFORMATION_SERVICE = "0000180a-0000-1000-8000-00805f9b34fb";

	public static string SCRA_BLE_DEVICE_BATTERY_SERVICE = "0000180f-0000-1000-8000-00805f9b34fb";

	public static string SCRA_BLE_DEVICE_BATTERY_LEVEL = "00002a19-0000-1000-8000-00805f9b34fb";

	public static string SCRA_BLE_DEVICE_READER_SERVICE = "0508e6f8-ad82-898f-f843-e3410cb60102";

	public static string SCRA_BLE_EMV_DEVICE_READER_SERVICE = "0508e6f8-ad82-898f-f843-e3410cb60103";

	public static string SCRA_BLE_EMV_T_DEVICE_READER_SERVICE = "0508e6f8-ad82-898f-f843-e3410cb60104";

	public static string SCRA_BLE_DEVICE_COMMAND_DATA = "0508e6f8-ad82-898f-f843-e3410cb60200";

	public static string SCRA_BLE_DEVICE_CARD_DATA = "0508e6f8-ad82-898f-f843-e3410cb60201";

	public static string SCRA_BLE_DEVICE_NOTIFY_DATA = "0508e6f8-ad82-898f-f843-e3410cb60202";

	public static string SCRA_BLE_DEVICE_READ_STATUS = "0508e6f8-ad82-898f-f843-e3410cb60203";

	public static Guid UUID_SCRA_BTH_DEVICE_SPP_SERVICE = new Guid(SCRA_BTH_DEVICE_SPP_SERVICE);

	public static Guid UUID_SCRA_BLE_DEVICE_INTERFACE = new Guid(SCRA_BLE_DEVICE_INTERFACE);

	public static Guid UUID_SCRA_BLE_DEVICE_INFORMATION_SERVICE = new Guid(SCRA_BLE_DEVICE_INFORMATION_SERVICE);

	public static Guid UUID_SCRA_BLE_DEVICE_READER_SERVICE = new Guid(SCRA_BLE_DEVICE_READER_SERVICE);

	public static Guid UUID_SCRA_BLE_EMV_DEVICE_READER_SERVICE = new Guid(SCRA_BLE_EMV_DEVICE_READER_SERVICE);

	public static Guid UUID_SCRA_BLE_EMV_T_DEVICE_READER_SERVICE = new Guid(SCRA_BLE_EMV_T_DEVICE_READER_SERVICE);

	public static Guid UUID_SCRA_BLE_DEVICE_BATTERY_SERVICE = new Guid(SCRA_BLE_DEVICE_BATTERY_SERVICE);

	public static Guid UUID_SCRA_BLE_DEVICE_BATTERY_LEVEL = new Guid(SCRA_BLE_DEVICE_BATTERY_LEVEL);

	public static Guid UUID_SCRA_BLE_DEVICE_COMMAND_DATA = new Guid(SCRA_BLE_DEVICE_COMMAND_DATA);

	public static Guid UUID_SCRA_BLE_DEVICE_CARD_DATA = new Guid(SCRA_BLE_DEVICE_CARD_DATA);

	public static Guid UUID_SCRA_BLE_DEVICE_NOTIFY_DATA = new Guid(SCRA_BLE_DEVICE_NOTIFY_DATA);

	public static Guid UUID_SCRA_BLE_DEVICE_READ_STATUS = new Guid(SCRA_BLE_DEVICE_READ_STATUS);

	public static string PPSCRA_BLE_DEVICE_READER_SERVICE = "0508e6f8-ad82-898f-f843-e3410cb60101";

	public static string PPSCRA_BLE_DEVICE_WRITE_LEN = "0508e6f8-ad82-898f-f843-e3410cb60220";

	public static string PPSCRA_BLE_DEVICE_WRITE_DATA = "0508e6f8-ad82-898f-f843-e3410cb60221";

	public static string PPSCRA_BLE_DEVICE_NOTIFY_LEN = "0508e6f8-ad82-898f-f843-e3410cb60222";

	public static string PPSCRA_BLE_DEVICE_READ_DATA = "0508e6f8-ad82-898f-f843-e3410cb60223";

	public static Guid UUID_PPSCRA_BLE_DEVICE_READER_SERVICE = new Guid(PPSCRA_BLE_DEVICE_READER_SERVICE);

	public static Guid UUID_PPSCRA_BLE_DEVICE_WRITE_LEN = new Guid(PPSCRA_BLE_DEVICE_WRITE_LEN);

	public static Guid UUID_PPSCRA_BLE_DEVICE_WRITE_DATA = new Guid(PPSCRA_BLE_DEVICE_WRITE_DATA);

	public static Guid UUID_PPSCRA_BLE_DEVICE_NOTIFY_LEN = new Guid(PPSCRA_BLE_DEVICE_NOTIFY_LEN);

	public static Guid UUID_PPSCRA_BLE_DEVICE_READ_DATA = new Guid(PPSCRA_BLE_DEVICE_READ_DATA);

	public static byte[] SCRA_DEVICE_COMMAND_GET_BATTERY_LEVEL = new byte[2] { 69, 0 };

	public static byte[] SCRA_DEVICE_COMMAND_RESET = new byte[2] { 2, 0 };

	public static byte[] SCRA_DEVICE_COMMAND_GET_INTERFACE_TYPE = new byte[6] { 70, 4, 1, 0, 0, 17 };

	public static byte[] SCRA_DEVICE_COMMAND_SET_INTERFACE_TYPE_HID = new byte[7] { 70, 5, 1, 0, 1, 17, 0 };

	public static byte[] SCRA_DEVICE_COMMAND_SET_INTERFACE_TYEP_KEYBOARD_EMULATION = new byte[7] { 70, 5, 1, 0, 1, 17, 1 };

	public static byte[] SCRA_DEVICE_COMMAND_SET_INTERFACE_TYPE_GATT = new byte[7] { 70, 5, 1, 0, 1, 17, 2 };

	public static byte[] SCRA_DEVICE_GET_LED_FUNCTIONALITY_CONTROL = new byte[6] { 70, 4, 1, 0, 0, 19 };

	public static byte[] SCRA_DEVICE_SET_LED_FUNCTIONALITY_CONTROL_OFF = new byte[7] { 70, 5, 1, 0, 1, 19, 0 };

	public static byte[] SCRA_DEVICE_SET_LED_FUNCTIONALITY_CONTROL_ON = new byte[7] { 70, 5, 1, 0, 1, 19, 1 };

	public static byte[] SCRA_DEVICE_GET_CARD_SWIPE_OUTPUT_CHANNEL = new byte[3] { 0, 1, 95 };

	public static byte[] SCRA_DEVICE_SET_CARD_SWIPE_OUTPUT_CHANNEL_USB = new byte[4] { 1, 1, 95, 0 };

	public static byte[] SCRA_DEVICE_SET_CARD_SWIPE_OUTPUT_CHANNEL_BLE = new byte[4] { 1, 1, 95, 1 };

	public static byte[] SCRA_DEVICE_SET_CARD_SWIPE_OUTPUT_CHANNEL_OVERRIDE_USB = new byte[3] { 72, 1, 0 };

	public static byte[] SCRA_DEVICE_SET_CARD_SWIPE_OUTPUT_CHANNEL_OVERRIDE_BLE = new byte[3] { 72, 1, 1 };

	public static byte[] SCRA_DEVICE_COMMAND_BLE_DISCONNECT = new byte[5] { 70, 3, 1, 0, 11 };

	public static byte[] SCRA_DEVICE_COMMAND_BLE_GET_BATTERY_LEVEL = new byte[6] { 70, 4, 1, 0, 0, 5 };

	public static byte[] SCRA_DEVICE_COMMAND_GET_FIRMWARE_ID = new byte[3] { 0, 1, 0 };

	public static byte[] SCRA_DEVICE_COMMAND_GET_DEVICE_SERIAL = new byte[3] { 0, 1, 3 };

	public static byte[] SCRA_DEVICE_COMMAND_GET_CAP_MAGNESAFE20_ENCRYPTION = new byte[3] { 0, 1, 54 };

	public static byte[] SCRA_DEVICE_COMMAND_GET_CAP_MAGSTRIPE_ENCRYPTION = new byte[2] { 21, 0 };

	public static byte[] SCRA_DEVICE_COMMAND_GET_CAP_TRACKS = new byte[3] { 0, 1, 5 };

	public static string SCRA_DEVICE_COMMAND_STRING_AUDIO_DISCOVERY = "C10206C20503840900";

	public static string SCRA_DEVICE_COMMAND_STRING_BLE_GET_BATTERY_LEVEL = "460401000005";

	public static string SCRA_DEVICE_COMMAND_STRING_BLE_GET_INTERFACE_TYPE = "460401000011";

	public static string SCRA_DEVICE_COMMAND_STRING_GET_FIRMWARE_ID = "000100";

	public static string SCRA_DEVICE_COMMAND_STRING_GET_DEVICE_SERIAL = "000103";

	public static string SCRA_DEVICE_COMMAND_STRING_SET_CARD_SWIPE_OUTPUT_CHANNEL_OVERRIDE_USB = "480100";

	public static string SCRA_DEVICE_COMMAND_STRING_SET_CARD_SWIPE_OUTPUT_CHANNEL_OVERRIDE_BLE = "480101";

	public static string SCRA_DEVICE_COMMAND_STRING_SET_LED_OFF = "4D0100";

	public static string SCRA_DEVICE_COMMAND_STRING_SET_LED_ON = "4D0101";

	public static long BATTERY_LEVEL_NA = -1L;

	public static long BATTERY_LEVEL_MIN = 0L;

	public static long BATTERY_LEVEL_MAX = 100L;

	public static long SWIPE_COUNT_NA = -1L;
}
