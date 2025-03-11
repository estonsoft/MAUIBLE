// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTEMVDeviceConstants
public class MTEMVDeviceConstants
{
	public static byte PROTOCOL_EXTENDER_REQUEST = 73;

	public static byte PROTOCOL_EXTENDER_RESPONSE = 10;

	public static byte PROTOCOL_EXTENDER_REQUEST_PENDING = 11;

	public static byte[] EMV_COMMAND_START_TRANSACTION = new byte[2] { 3, 0 };

	public static byte[] EMV_COMMAND_GET_STATUS = new byte[2] { 3, 1 };

	public static byte[] EMV_COMMAND_SET_USER_SELECTION_RESULT = new byte[2] { 3, 2 };

	public static byte[] EMV_COMMAND_SET_ACQUIRER_RESPONSE = new byte[2] { 3, 3 };

	public static byte[] EMV_COMMAND_CANCEL_TRANSACTION = new byte[2] { 3, 4 };

	public static byte[] EMV_COMMAND_SET_TERMINAL_CONFIGURATION = new byte[2] { 3, 5 };

	public static byte[] EMV_COMMAND_GET_TERMINAL_CONFIGURATION = new byte[2] { 3, 6 };

	public static byte[] EMV_COMMAND_SET_APPLICATION_CONFIGURATION = new byte[2] { 3, 7 };

	public static byte[] EMV_COMMAND_GET_APPLICATION_CONFIGURATION = new byte[2] { 3, 8 };

	public static byte[] EMV_COMMAND_SET_ACQUIRER_CA_PULIC_KEY = new byte[2] { 3, 9 };

	public static byte[] EMV_COMMAND_GET_ACQUIRER_CA_PULIC_KEY = new byte[2] { 3, 10 };

	public static byte[] EMV_COMMAND_GET_KERNEL_INFORMATION = new byte[2] { 3, 11 };

	public static byte[] EMV_COMMAND_SET_DATE_TIME = new byte[2] { 3, 12 };

	public static byte[] EMV_COMMAND_GET_DATE_TIME = new byte[2] { 3, 13 };

	public static byte[] EMV_COMMAND_COMMIT_CONFIGURATION = new byte[2] { 3, 14 };

	public static byte[] EMV_EVENT_TRANSACTION_STATUS = new byte[2] { 3, 0 };

	public static byte[] EMV_EVENT_DISPLAY_MESSAGE_REQUEST = new byte[2] { 3, 1 };

	public static byte[] EMV_EVENT_USER_SELECTION_REQUEST = new byte[2] { 3, 2 };

	public static byte[] EMV_EVENT_ARQC_RECEIVED = new byte[2] { 3, 3 };

	public static byte[] EMV_EVENT_TRANSACTION_RESULT = new byte[2] { 3, 4 };

	public static byte CARD_TYPE_MAGNETIC_STRIPE = 1;

	public static byte CARD_TYPE_CONTACT_SMART_CARD = 2;

	public static byte CARD_TYPE_CONTACTLESS_SMART_CARD = 4;

	public static byte TRANSACTION_TYPE_PAYMENT = 0;

	public static byte TRANSACTION_TYPE_CASH_ADVANCE = 1;

	public static byte TRANSACTION_TYPE_CASHBACK = 2;

	public static byte TRANSACTION_TYPE_GOODS = 4;

	public static byte TRANSACTION_TYPE_SERVICE = 8;

	public static byte TRANSACTION_TYPE_CASHBACK_CONTACTLESS = 9;

	public static byte TRANSACTION_TYPE_INTERNATIONAL_GOODS = 16;

	public static byte TRANSACTION_TYPE_REFUND = 32;

	public static byte TRANSACTION_TYPE_INTERNATIONAL_CASH = 64;

	public static byte TRANSACTION_TYPE_DOMESTIC_CASH = 128;

	public static byte TRANSACTION_OPION_NORMAL = 0;

	public static byte TRANSACTION_OPION_BYPASS_PIN = 1;

	public static byte TRANSACTION_OPION_FORCE_ONLINE = 2;

	public static byte TRANSACTION_OPION_ACQUIRER_NOT_AVAILABLE = 4;

	public static byte SELECTION_TYPE_APPLICATION = 0;

	public static byte SELECTION_TYPE_LANGUAGE = 1;

	public static byte SELECTION_STATUS_COMPLETED = 0;

	public static byte SELECTION_STATUS_CANCELLED = 1;

	public static byte SELECTION_STATUS_TIMED_OUT = 2;

	public static byte[] CURRENCY_US_DOLLAR = new byte[2] { 8, 64 };

	public static byte[] CURRENCY_EURO = new byte[2] { 9, 120 };

	public static byte[] CURRENCY_UK_POUND = new byte[2] { 8, 38 };
}
