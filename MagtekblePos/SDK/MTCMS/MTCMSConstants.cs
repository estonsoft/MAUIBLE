// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTCMSConstants
internal class MTCMSConstants
{
	public static byte OPERATION_GET = 0;

	public static byte OPERATION_SET = 1;

	public static byte COMMAND_STATUS_OK = 0;

	public static byte COMMAND_STATUS_PENDING = 1;

	public static byte COMMAND_STATUS_TIMEOUT = 2;

	public static byte COMMAND_STATUS_ERROR = 3;

	public static byte EMV_COMMAND_SET_GET_EMV_TAGS = 161;

	public static byte EMV_COMMAND_REQUEST_START_EMV_TRANSACTION = 162;

	public static byte EMV_COMMAND_REQUEST_ATR = 163;

	public static byte EMV_COMMAND_ACQUIRER_RESPONSE = 164;

	public static byte EMV_COMMAND_SET_GET_CA_PUBLIC_KEY = 165;

	public static byte EMV_COMMAND_REQUEST_POWER_UP_DOWN_RESET_ICC = 166;

	public static byte EMV_COMMAND_SEND_GET_ICC_APDU = 167;

	public static byte EMV_COMMAND_GET_KERNEL_INFO = 168;

	public static byte EMV_COMMAND_GET_CHALLENGE_AND_SESSION_KEY = 169;

	public static byte EMV_COMMAND_CONFIRM_SESSION_KEY = 170;

	public static byte EMV_COMMAND_REQUEST_EMV_TRANSACTION_DATA = 171;

	public static byte EMV_COMMAND_MERCHANT_BYPASS_PIN = 172;

	public static byte COMMAND_RESPONSE_ACK = 1;

	public static byte COMMAND_END_SESSION = 2;

	public static byte COMMAND_REQUEST_SWIPE_CARD = 3;

	public static byte COMMAND_REQUEST_PIN_ENTRY = 4;

	public static byte COMMAND_CANCEL_COMMAND = 5;

	public static byte COMMAND_REQUEST_USER_SELECTION = 6;

	public static byte COMMAND_DISPLAY_MESSAGE = 7;

	public static byte COMMAND_REQUEST_DEVICE_STATUS = 8;

	public static byte COMMAND_SET_GET_DEVICE_CONFIGURATION = 9;

	public static byte COMMAND_REQUEST_MSR_DATA = 10;

	public static byte COMMAND_GET_CHALLENGE = 11;

	public static byte COMMAND_SET_BITMAP = 12;

	public static byte COMMAND_SET_PROPERTY = 13;

	public static byte COMMAND_GET_INFORMATION = 14;

	public static byte COMMAND_LOGIN_AUTHENTICATE_LOGOUT = 15;

	public static byte COMMAND_SEND_BIG_BLOCK_DATA_TO_DEVICE = 16;

	public static byte COMMAND_REQUEST_MANUAL_CARD_ENTRY = 17;

	public static byte COMMAND_REQUEST_USER_SIGNATURE = 18;

	public static byte COMMAND_GET_USER_SIGNATURE = 19;

	public static byte COMMAND_REQUEST_USER_DATA_ENTRY = 20;

	public static byte COMMAND_SET_SIGNATURE_CAPTURE_CONFIGURATION = 21;

	public static byte COMMAND_UPDATE_DEVICE = 23;

	public static byte COMMAND_PERFORM_TEST = 24;

	public static byte COMMAND_SET_GET_EXTENDED_DEVICE_CONFIGURATION = 25;

	public static byte COMMAND_REQUEST_DEVICE_INFORMATION = 26;

	public static byte COMMAND_REQUEST_CLEAR_TEXT_USER_DATA_ENTRY = 31;

	public static byte COMMAND_SET_GET_KSN = 48;

	public static byte COMMAND_SET_GET_BIN_TABLE_DATA = 50;

	public static byte COMMAND_REQUEST_DEVICE_CERT = 88;

	public static byte COMMAND_KEY_HANDLING_MANUFACTURING_COMMAND = 88;

	public static byte COMMAND_DEVICE_RESET = byte.MaxValue;

	public static byte RESPONSE_DEVICE_STATE = 32;

	public static byte RESPONSE_USER_DATA_ENTRY = 33;

	public static byte RESPONSE_CARD_STATUS = 34;

	public static byte RESPONSE_CARD_DATA = 35;

	public static byte RESPONSE_PIN = 36;

	public static byte RESPONSE_USER_SELECTION = 37;

	public static byte RESPONSE_DISPLAY_MESSAGE_DONE = 39;

	public static byte RESPONSE_SIGNATURE_CAPTURE_STATE = 40;

	public static byte RESPONSE_SEND_BIG_BLOCK_DATA_TO_HOST = 41;

	public static byte RESPONSE_CLEAR_TEXT_USER_DATA_ENTRY = 46;

	public static byte SET_AMOUNT = 0;

	public static byte SET_PAN = 1;

	public static byte DEVICE_INFORMATION_MODE_PRODUCT_ID = 0;

	public static byte DEVICE_INFORMATION_MODE_MAX_APP_MSG_SIZE = 1;

	public static byte DEVICE_INFORMATION_MODE_CAPABILITY_STRING = 2;

	public static byte DEVICE_INFORMATION_MODE_MANUFACTURER = 3;

	public static byte DEVICE_INFORMATION_MODE_PRODUCT_NAME = 4;

	public static byte DEVICE_INFORMATION_MODE_SERIAL_NUMBER = 5;

	public static byte DEVICE_INFORMATION_MODE_FIRMWARE_NUMBER = 6;

	public static byte DEVICE_INFORMATION_MODE_BUILD_INFO = 7;

	public static byte DEVICE_INFORMATION_MODE_MAC_ADDRESS = 8;

	public static byte DEVICE_INFORMATION_MODE_BOOT1_FIRMWARE_NUMBER = 10;

	public static byte DEVICE_INFORMATION_MODE_BOOT2_FIRMWARE_NUMBER = 11;

	public static byte DISPLAY_MESSAGE_BLANK = 0;

	public static byte DISPLAY_MESSAGE_APPROVED = 1;

	public static byte DISPLAY_MESSAGE_DECLINED = 2;

	public static byte DISPLAY_MESSAGE_CANCELLED = 3;

	public static byte DISPLAY_MESSAGE_THANK_YOU = 4;

	public static byte DISPLAY_MESSAGE_PIN_INVALID = 5;

	public static byte DISPLAY_MESSAGE_PROCESSING = 6;

	public static byte DISPLAY_MESSAGE_PLEASE_WAIT = 7;

	public static byte DISPLAY_MESSAGE_HANDS_OFF = 8;

	public static byte DISPLAY_MESSAGE_PIN_PAD_NOT_AVAILABLE = 9;

	public static byte DISPLAY_MESSAGE_CALL_YOUR_BANK = 10;

	public static byte DISPLAY_MESSAGE_CARD_ERROR = 11;

	public static byte DISPLAY_MESSAGE_NOT_ACCEPTED = 12;

	public static byte DISPLAY_MESSAGE_PROCESSING_ERROR = 13;

	public static byte DISPLAY_MESSAGE_USE_CHIP_READER = 14;

	public static byte DISPLAY_MESSAGE_REFER_TO_YOUR_PAYMENT_DEVICE = 15;

	public static byte DISPLAY_MESSAGE_BITMAP_SLOT0 = 128;

	public static byte DISPLAY_MESSAGE_BITMAP_SLOT1 = 129;

	public static byte DISPLAY_MESSAGE_BITMAP_SLOT2 = 130;

	public static byte DISPLAY_MESSAGE_BITMAP_SLOT3 = 131;

	public static byte DISPLAY_MESSAGE_USER = byte.MaxValue;

	public static byte EMV_MESSAGE_AMOUNT = 1;

	public static byte EMV_MESSAGE_AMOUNT_OK_QUESTION = 2;

	public static byte EMV_MESSAGE_APPROVED = 3;

	public static byte EMV_MESSAGE_CALL_YOUR_BANK = 4;

	public static byte EMV_MESSAGE_CANCEL_OR_ENTER = 5;

	public static byte EMV_MESSAGE_CARD_ERROR = 6;

	public static byte EMV_MESSAGE_DECLINED = 7;

	public static byte EMV_MESSAGE_ENTER_AMOUNT = 8;

	public static byte EMV_MESSAGE_ENTER_PIN = 9;

	public static byte EMV_MESSAGE_INCORRECT_PIN = 10;

	public static byte EMV_MESSAGE_INSERT_CARD = 11;

	public static byte EMV_MESSAGE_NOT_ACCEPTED = 12;

	public static byte EMV_MESSAGE_PIN_OK = 13;

	public static byte EMV_MESSAGE_PLEASE_WAIT = 14;

	public static byte EMV_MESSAGE_PROCESSING_ERROR = 15;

	public static byte EMV_MESSAGE_REMOVE_CARD = 16;

	public static byte EMV_MESSAGE_USE_CHIP_READER = 17;

	public static byte EMV_MESSAGE_USE_MAGSTRIPE = 18;

	public static byte EMV_MESSAGE_TRY_AGAIN = 19;

	public static byte EMV_TAG_OPERATION_READ = 0;

	public static byte EMV_TAG_OPERATION_WRITE = 1;

	public static byte EMV_TAG_OPERATION_READ_ALL = 15;

	public static byte EMV_TAG_OPERATION_FACTORY_DEFAULT = byte.MaxValue;

	public static byte EMV_TAG_TYPE_TERMINAL = 0;

	public static byte EMV_TAG_TYPE_APPLICATION_GROUP = 128;

	public static byte EMV_TAG_TYPE_DRL_GROUP = 192;

	public static byte EMV_DATABASE_CONTACT_L2_EMV_TAGS = 0;

	public static byte EMV_DATABASE_PAYPASS_MASTERCARD = 1;

	public static byte EMV_DATABASE_PAYWAVE_VISA = 2;

	public static byte EMV_DATABASE_EXPRESSPAY_AMEX = 3;

	public static byte EMV_DATABASE_DISCOVER = 4;

	public static byte CA_PUBLIC_KEY_OPERATION_ERASE_ALL_KEYS = 0;

	public static byte CA_PUBLIC_KEY_OPERATION_ERASE_ALL_KEYS_WITH_RID = 1;

	public static byte CA_PUBLIC_KEY_OPERATION_ERASE_SINGLE_KEY = 2;

	public static byte CA_PUBLIC_KEY_OPERATION_ADD_SIGNGLE_KEY = 3;

	public static byte CA_PUBLIC_KEY_OPERATION_READ_ALL_KEYS = 15;

	public static byte SESSION_KEY_MODE_END_SESSION = 0;

	public static byte SESSION_KEY_MODE_CONFIRM_SESSION = 1;

	public static byte ICC_OPERATION_WAIT_AND_POWER_UP = 1;

	public static byte ICC_OPERATION_POWER_DOWN = 2;

	public static byte BITMAP_FLAG_CLEAR = 0;

	public static byte BITMAP_FLAG_SAVE = 1;

	public static byte BITMAP_FLAG_INVERT_AND_SAVE = 2;

	public static byte FUNCTION_KEY_LEFT = 113;

	public static byte FUNCTION_KEY_MIDDLE = 114;

	public static byte FUNCTION_KEY_RIGHT = 116;

	public static byte FUNCTION_KEY_ENTER = 120;

	public static byte PIN_MESSAGE_ENTER_PIN = 0;

	public static byte PIN_MESSAGE_ENTER_PIN_AMOUNT = 1;

	public static byte PIN_MESSAGE_RENTER_PIN_AMOUNT = 2;

	public static byte PIN_MESSAGE_REENTER_PIN = 3;

	public static byte PIN_MESSAGE_VERIFY_PIN = 4;

	public static byte PIN_OPTION_ISO3 = 1;

	public static byte PIN_OPTION_VERIFY_PIN = 2;

	public static byte PIN_OPTION_WAIT_MESSAGE = 4;

	public static byte MANUAL_CARD_OPTION_FIELD_ACCT_DATE_CVC = 0;

	public static byte MANUAL_CARD_OPTION_FIELD_ACCT_DATE = 1;

	public static byte MANUAL_CARD_OPTION_FIELD_ACCT_CVC = 2;

	public static byte MANUAL_CARD_OPTION_FIELD_ACCT = 3;

	public static byte MANUAL_CARD_OPTION_USE_QWICK_CODES_ENTRY = 4;

	public static byte MANUAL_CARD_OPTION_USE_PAN_IN_PIN_BLOCK_CREATION = 8;

	public static byte MANUAL_CARD_OPTION_USE_PAN_MIN_14_MAX_21 = 16;

	public static byte USER_DATA_MODE_ENTER_SSN = 0;

	public static byte USER_DATA_MODE_ENTER_ZIP_CODE = 1;

	public static byte USER_DATA_MODE_ENTER_BIRTHDATE_MMDDYYYY = 2;

	public static byte USER_DATA_MODE_ENTER_BIRTHDATE_MMDDYY = 3;

	public static byte SIGNATURE_OPTION_TIMEOUT_CLEAR_DATA = 0;

	public static byte SIGNATURE_OPTION_TIMEOUT_DO_NOT_CLEAR_DATA = 1;

	public static byte RESPONSE_MESSAGE_TRANSACTION_TYPE = 0;

	public static byte RESPONSE_MESSAGE_AMOUNT_OK = 1;

	public static byte CARD_MESSAGE_SWIPE_CARD_IDLE_ALTERNATING = 0;

	public static byte CARD_MESSAGE_SWIPE_CARD = 1;

	public static byte CARD_MESSAGE_PLEASE_SWIPE_CARD = 2;

	public static byte CARD_MESSAGE_PLEASE_SWIPE_CARD_AGAIN = 3;

	public static byte CARD_MESSAGE_CHIP_ERROR_USE_MAG_STRIPE = 4;

	public static byte SELECT_MESSAGE_CREIT_DEBIT = 0;

	public static byte SELECT_MESSAGE_VERIFY_AMOUNT = 1;

	public static byte SELECT_MESSAGE_CREDIT_OTHER_DEBIT = 2;

	public static byte SELECT_MESSAGE_CREDIT_EBT_DEBIT = 3;

	public static byte SELECT_MESSAGE_CREDIT_GIFT_DEBIT = 4;

	public static byte SELECT_MESSAGE_EBT_GIFT_OTHER = 5;

	public static byte SELECT_MESSAGE_USER_DEFINED = byte.MaxValue;

	public static byte TONE_NO_SOUND = 0;

	public static byte TONE_ONE_BEEP = 1;

	public static byte TONE_TWO_BEEPS = 2;

	public static byte AMOUNT_TYPE_CREDIT = 0;

	public static byte AMOUNT_TYPE_DEBIT = 1;

	public static byte TRANSACTION_CARD_TYPE_MAGNETIC_STRIPE = 1;

	public static byte TRANSACTION_CARD_TYPE_CONTACT_SMART_CARD = 2;

	public static byte TRANSACTION_CARD_TYPE_CONTACTLESS_SMART_CARD = 4;

	public static byte TRANSACTION_TYPE_PAYMENT = 0;

	public static byte TRANSACTION_TYPE_CASH_ADVANCE = 1;

	public static byte TRANSACTION_TYPE_CASHBACK = 2;

	public static byte TRANSACTION_TYPE_GOODS = 4;

	public static byte TRANSACTION_TYPE_SERVICE = 8;

	public static byte TRANSACTION_TYPE_PURCHASE_WITH_CASHBACK = 9;

	public static byte TRANSACTION_TYPE_REFUND = 32;

	public static byte TRANSACTION_OPION_NORMAL = 0;

	public static byte TRANSACTION_OPION_BYPASS_PIN = 1;

	public static byte TRANSACTION_OPION_FORCE_ONLINE = 2;

	public static byte TRANSACTION_OPION_ACQUIRER_NOT_AVAILABLE = 4;

	public static byte CARD_TRACK_STATUS_OK = 0;

	public static byte CARD_TRACK_STATUS_EMPTY = 1;

	public static byte CARD_TRACK_STATUS_ERROR = 2;

	public static byte CARD_TRACK_STATUS_DISABLED = 3;

	public static byte BIG_BUFFER_TYPE_SIGNATURE_CAPTURE_DATA = 0;

	public static byte BIG_BUFFER_TYPE_DEVICE_CERT = 2;

	public static byte BIG_BUFFER_TYPE_PERFORM_TEST_APDU = 24;

	public static byte BIG_BUFFER_TYPE_SET_BIN = 50;

	public static byte BIG_BUFFER_TYPE_CSR = 67;

	public static byte BIG_BUFFER_TYPE_TAG_DATA = 161;

	public static byte BIG_BUFFER_TYPE_AUTHORIZATION_REQUEST = 164;

	public static byte BIG_BUFFER_TYPE_CA_PUBLIC_KEY = 165;

	public static byte BIG_BUFFER_TYPE_ATR = 166;

	public static byte BIG_BUFFER_TYPE_R_APDU = 167;

	public static byte BIG_BUFFER_TYPE_BATCH_OR_REVERSAL_DATA = 171;

	public static byte OPERATION_STATUS_OK = 0;

	public static byte OPERATION_STATUS_USER_CANCEL = 1;

	public static byte OPERATION_STATUS_TIMEOUT = 2;

	public static byte OPERATION_STATUS_HOST_CANCEL = 3;

	public static byte OPERATION_STATUS_VERIFY_FAIL = 4;

	public static byte OPERATION_STATUS_KEYPAD_SECURITY = 5;

	public static byte OPERATION_STATUS_CALIBRATION_DONE = 6;

	public static byte OPERATION_STATUS_WRITE_WITH_DUPLICATE_RID_AND_INDEX = 7;

	public static byte OPERATION_STATUS_WRITE_WITH_CORRUPTED_KEY = 8;

	public static byte OPERATION_STATUS_CA_PUBLIC_KEY_REACHED_MAXIMUM_CAPACITY = 9;

	public static byte OPERATION_STATUS_CA_PUBLIC_KEY_READ_WITH_INVALID_RID_OR_INDEX = 10;

	public static byte ACK_STATUS_OK = 0;

	public static byte ACK_STATUS_SYSTEM_ERROR = 128;

	public static byte ACK_STATUS_SYSTEM_NOT_IDLE = 129;

	public static byte ACK_STATUS_DATA_ERROR = 130;

	public static byte ACK_STATUS_LENGTH_ERROR = 131;

	public static byte ACK_STATUS_PAN_EXISTS = 132;

	public static byte ACK_STATUS_NO_KEY_OR_KEY_IS_INCORRECT = 133;

	public static byte ACK_STATUS_SYSTEM_BUSY = 134;

	public static byte ACK_STATUS_SYSTEM_LOCKED = 135;

	public static byte ACK_STATUS_AUTH_REQUIRED = 136;

	public static byte ACK_STATUS_BAD_AUTH = 137;

	public static byte ACK_STATUS_SYSTEM_NOT_AVAILABLE = 138;

	public static byte ACK_STATUS_AMOUNT_NEEDED = 139;

	public static byte ACK_STATUS_CERT_NON_EXIST = 144;

	public static byte ACK_STATUS_EXPIRED_CERT_CRL = 145;

	public static byte ACK_STATUS_INVALID_CERT_CRL_MESSAGE = 146;

	public static byte ACK_STATUS_REVOKED_CERT_CRL = 147;

	public static byte ACK_STATUS_CRL_NON_EXIST = 148;

	public static byte ACK_STATUS_CERT_EXISTS = 149;

	public static byte ACK_STATUS_DUPLICATE_KSN_KEY = 150;

	public static byte DEVICE_STATE_IDLE = 0;

	public static byte DEVICE_STATE_SESSION = 1;

	public static byte DEVICE_STATE_WAIT_FOR_CARD = 2;

	public static byte DEVICE_STATE_WAIT_FOR_PIN = 3;

	public static byte DEVICE_STATE_WAIT_FOR_SELECTION = 4;

	public static byte DEVICE_STATE_DISPLAY_MESSAGE = 5;

	public static byte DEVICE_STATE_TEST = 6;

	public static byte DEVICE_STATE_MANUAL_CARD_ENTRY = 7;

	public static byte DEVICE_STATE_WAIT_FOR_SIGNATURE_CAPTURE = 8;

	public static byte DEVICE_STATE_WAIT_FOR_USER_ENTRY = 9;

	public static byte DEVICE_STATE_SMART_CARD = 10;

	public static byte DEVICE_STATE_ICC_KERNEL_TEST = 11;

	public static byte DEVICE_STATE_EMV_TRANSACTION = 12;

	public static byte CARD_TYPE_OTHER_ = 0;

	public static byte CARD_TYPE_FINANCIAL = 1;

	public static byte CARD_TYPE_AAMVA = 2;

	public static byte CARD_TYPE_MANUAL = 3;

	public static byte CARD_TYPE_UNKNOWN = 4;

	public static byte CARD_TYPE_ICC = 5;

	public static byte CARD_TYPE_CONTACTLESS_ICC = 6;

	public static byte CARD_STATUS_OK = 0;

	public static byte CARD_STATUS_TRACK1_ERROR = 2;

	public static byte CARD_STATUS_TRACK2_ERROR = 4;

	public static byte CARD_STATUS_TRACK3_ERROR = 8;

	public static byte KEY_MASK_LEFT = 1;

	public static byte KEY_MASK_MIDDLE = 2;

	public static byte KEY_MASK_RIGHT = 4;

	public static byte KEY_MASK_ENTER = 8;

	public static byte DEVICE_STATUS_OK = 0;

	public static byte DEVICE_STATUS_NO_PIN_KEY = 1;

	public static byte DEVICE_STATUS_PIN_KEY_EXHAUSTED = 2;

	public static byte DEVICE_STATUS_PIN_KEY_NOT_BOUND = 3;

	public static byte DEVICE_STATUS_NO_MSR_KEY = 4;

	public static byte DEVICE_STATUS_MSR_KEY_EXHAUSTED = 8;

	public static byte DEVICE_STATUS_MSR_KEY_NOT_BOUND = 13;

	public static byte DEVICE_STATUS_TAMPER_DETECTED = 32;

	public static byte DEVICE_STATUS_NOT_AUTHORIZED = 64;

	public static byte DEVICE_STATUS_SYSTEM_ERROR = 128;

	public static byte SESSION_STATE_AMOUNT_SENT = 1;

	public static byte SESSION_STATE_EXTERNAL_PAN_SENT = 2;

	public static byte SESSION_STATE_PAN_PARSED_FROM_CARD = 4;

	public static byte SESSION_STATE_CARD_DATA_AVAILABLE = 8;

	public static byte SESSION_STATE_POWER_CHANGE_OCCURRED = 128;

	public static byte DEVICE_CERTIFICATE_STATUS_DEVICE_CERT = 1;

	public static byte DEVICE_CERTIFICATE_STATUS_DEVICE_CA = 2;

	public static byte DEVICE_CERTIFICATE_STATUS_PIN_CA = 4;

	public static byte DEVICE_CERTIFICATE_STATUS_MSR_CA = 8;

	public static byte DEVICE_CERTIFICATE_STATUS_MSR_CRL = 128;

	public static byte HARDWARE_STATUS_TEMPER_SENSORS_ACTIVE = 1;

	public static byte HARDWARE_STATUS_MAGHEAD_PROGRAMMED = 2;

	public static byte HARDWARE_STATUS_KEYPAD_CALIBRATED = 4;

	public static byte HARDWARE_STATUS_KEYPAD_ACTIVATED = 8;

	public static byte KERNEL_INFO_VERSION_L1_KERNEL = 0;

	public static byte KERNEL_INFO_VERSION_L2_KERNEL = 1;

	public static byte KERNEL_INFO_CHECKSUM_SIGNATURE_L1_KERNEL = 2;

	public static byte KERNEL_INFO_CHECKSUM_SIGNATURE_L2_KERNEL = 3;

	public static byte KERNEL_INFO_CHECKSUM_SIGNATURE_L2_KERNEL_AND_CONFIG = 4;
}
