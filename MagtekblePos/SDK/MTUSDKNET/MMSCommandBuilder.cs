// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSCommandBuilder


public class MMSCommandBuilder
{
	public const string COMMAND_INITIATE_TRANSACTION = "1001";

	public const string COMMAND_RESUME_TRANSACTION = "1004";

	public const string COMMAND_CANCEL_TRANSACTION = "1008";

	public const string COMMAND_GET_TRANSACTION_DATA = "1011";

	public const string COMMAND_REQUEST_SIGNATURE = "1801";

	public const string COMMAND_REPORT_SELECTION = "1802";

	public const string COMMAND_DISPLAY_MESSAGE = "1803";

	public const string COMMAND_ENABLE_BCR = "1804";

	public const string COMMAND_SHOW_IMAGE = "1821";

	public const string COMMAND_SHOW_QR_CODE = "1822";

	public const string COMMAND_SHOW_BITMAP_IMAGE = "1823";

	public const string COMMAND_REQUEST_PIN = "2001";

	public const string COMMAND_REQUEST_PAN = "2002";

	public const string COMMAND_RESET_DEVICE = "1F01";

	public const string COMMAND_GET_ITEMS = "D101";

	public const string COMMAND_SET_ITEMS = "D111";

	public const string COMMAND_LOAD_FW_FILE = "D801";

	public const string COMMAND_SEND_FILE = "D811";

	public const string COMMAND_SEND_FILE_UNSECURED = "D812";

	public const string COMMAND_GET_FILE = "D821";

	public const string COMMAND_GET_FILE_INFO = "D825";

	public const string COMMAND_UPDATE_FW_FROM_FILE = "D901";

	public const string COMMAND_ECHO = "DF01";

	public const string COMMAND_PRETTY_PRINT_PAYLOAD = "DF02";

	public const string COMMAND_GET_CHALLENGE = "E001";

	public const string COMMAND_SECURE_WRAPPER = "EEEE";

	public const string COMMAND_TR31_KEY_INJECTION = "EF01";

	public const string COMMAND_GET_KEY_SLOT_INFO = "EF11";

	public const string COMMAND_WRITE_OTP = "F013";

	public const string COMMAND_ACTIVATE_DEVICE = "F016";

	public const string COMMAND_ESTABLISH_EP_KBPK = "F017";

	public const byte BLOBTYPE_CARD_DATA = 1;

	public const byte BLOBTYPE_ARQC = 2;

	public const byte BLOBTYPE_BATCH_DATA = 3;

	public const byte NOTIFICATION_TYPE_TRANSACTION = 16;

	public const byte NOTIFICATION_SECTION_OPERATION = 1;

	public const byte NOTIFICATION_EVENT_COMPLETED = 1;

	public const byte NOTIFICATION_EVENT_HALTED = 2;

	public const byte NOTIFICATION_EVENT_PAUSED = 3;

	public const byte NOTIFICATION_REASON_CARD_SWIPE = 1;

	public const byte NOTIFICATION_REASON_EMV_ARQC = 2;

	public const byte NOTIFICATION_REASON_EMV_BATCH = 3;

	public static Command InitiateTransactionCommand0(byte timeout, byte type, byte mode, byte option)
	{
		Command command = MessageBuilder.BuildCommand("1001");
		command.addParam(129, timeout);
		command.addParam(130, new byte[3] { type, mode, option });
		return command;
	}

	public static Command InitiateTransactionCommand(byte timeout, byte msrMode, byte contactMode, byte contactlessMode, byte[] manualMode, byte[] tracnsactionOptions, byte[] transactionTLVUpdate)
	{
		Command command = MessageBuilder.BuildCommand("1001");
		command.addParam(130, timeout);
		TLVObject tLVObject = new TLVObject(163);
		tLVObject.addTLVObject(new TLVObject(129, msrMode));
		tLVObject.addTLVObject(new TLVObject(130, contactMode));
		tLVObject.addTLVObject(new TLVObject(131, contactlessMode));
		if (manualMode != null)
		{
			tLVObject.addTLVObject(new TLVObject(132, manualMode));
		}
		command.addParam(tLVObject);
		command.addParam(132, tracnsactionOptions);
		command.addParam(134, transactionTLVUpdate);
		return command;
	}

	public static Command InitiateTransactionCommand(byte timeout, byte msrMOde, byte contactMode, byte contactlessMode, byte[] manualMode, byte[] tracnsactionOptions, byte[] transactionTLVUpdate, bool suppressThankYou, byte overrideFinalMessage)
	{
		Command command = MessageBuilder.BuildCommand("1001");
		command.addParam(130, timeout);
		TLVObject tLVObject = new TLVObject(163);
		tLVObject.addTLVObject(new TLVObject(129, msrMOde));
		tLVObject.addTLVObject(new TLVObject(130, contactMode));
		tLVObject.addTLVObject(new TLVObject(131, contactlessMode));
		if (manualMode != null)
		{
			tLVObject.addTLVObject(new TLVObject(132, manualMode));
		}
		command.addParam(tLVObject);
		command.addParam(132, tracnsactionOptions);
		command.addParam(134, transactionTLVUpdate);
		if (suppressThankYou || overrideFinalMessage != 0)
		{
			TLVObject tLVObject2 = new TLVObject(172);
			if (suppressThankYou)
			{
				tLVObject2.addTLVObject(new TLVObject(129, null));
			}
			if (overrideFinalMessage != 0)
			{
				tLVObject2.addTLVObject(new TLVObject(130, overrideFinalMessage));
			}
			command.addParam(tLVObject2);
		}
		return command;
	}

	public static Command ResumeTransactionCommand(byte[] dataBytes)
	{
		Command command = MessageBuilder.BuildCommand("1004");
		command.addParam(129, 0);
		command.addParam(132, dataBytes);
		return command;
	}

	public static Command CancelTransactionCommand()
	{
		return MessageBuilder.BuildCommand("1008");
	}

	public static Command GetTransactionDataCommand(byte blobType)
	{
		Command command = MessageBuilder.BuildCommand("1011");
		command.addParam(129, blobType);
		return command;
	}

	public static Command requestPINCommand(byte timeout, byte pinMode, byte minLength, byte maxLength, byte[] pan, byte format)
	{
		Command command = MessageBuilder.BuildCommand("2001");
		command.addParam(129, timeout);
		command.addParam(130, pinMode);
		command.addParam(131, new byte[2] { maxLength, minLength });
		TLVObject tLVObject = new TLVObject(161);
		if (pan != null && pan.Length != 0)
		{
			tLVObject.addTLVObject(new TLVObject(129, (byte)(pan.Length * 2)));
			tLVObject.addTLVObject(new TLVObject(130, pan));
		}
		else
		{
			tLVObject.addTLVObject(new TLVObject(129, 0));
		}
		command.addParam(tLVObject);
		command.addParam(133, format);
		return command;
	}

	public static Command requestPANCommand(byte timeout, byte msrMode, byte contactMode, byte contactlessMode)
	{
		Command command = MessageBuilder.BuildCommand("2002");
		command.addParam(129, timeout);
		TLVObject tLVObject = new TLVObject(163);
		tLVObject.addTLVObject(new TLVObject(129, msrMode));
		tLVObject.addTLVObject(new TLVObject(130, contactMode));
		tLVObject.addTLVObject(new TLVObject(131, contactlessMode));
		command.addParam(tLVObject);
		return command;
	}

	public static Command requestPANCommand(byte timeout, byte msrMode, byte contactMode, byte contactlessMode, byte pinMode, byte minLength, byte maxLength, byte format)
	{
		Command command = MessageBuilder.BuildCommand("2002");
		command.addParam(129, timeout);
		TLVObject tLVObject = new TLVObject(163);
		tLVObject.addTLVObject(new TLVObject(129, msrMode));
		tLVObject.addTLVObject(new TLVObject(130, contactMode));
		tLVObject.addTLVObject(new TLVObject(131, contactlessMode));
		command.addParam(tLVObject);
		TLVObject tLVObject2 = new TLVObject(164);
		tLVObject2.addTLVObject(new TLVObject(130, pinMode));
		tLVObject2.addTLVObject(new TLVObject(131, new byte[2] { maxLength, minLength }));
		tLVObject2.addTLVObject(new TLVObject(133, format));
		command.addParam(tLVObject2);
		return command;
	}

	public static Command RequestSignatureCommand(byte timeout)
	{
		Command command = MessageBuilder.BuildCommand("1801");
		command.addParam(129, timeout);
		return command;
	}

	public static Command ReportSelection(byte status, byte selection)
	{
		Command command = MessageBuilder.BuildCommand("1802");
		command.addParam(129, status);
		command.addParam(130, selection);
		return command;
	}

	public static Command DisplayMessageCommand(byte messageID, byte timeout)
	{
		Command command = MessageBuilder.BuildCommand("1803");
		command.addParam(129, timeout);
		command.addParam(130, messageID);
		return command;
	}

	public static Command ShowImageCommand(byte imageID, byte displayTime)
	{
		Command command = MessageBuilder.BuildCommand("1821");
		command.addParam(129, imageID);
		command.addParam(131, displayTime);
		return command;
	}

	public static Command ShowBitmapImageCommand(byte[] data, byte displayTime, byte[] background)
	{
		Command command = MessageBuilder.BuildCommand("1823");
		command.addParam(129, displayTime);
		if (background != null)
		{
			command.addParam(130, background);
		}
		command.addParam(133, data);
		return command;
	}

	public static Command ShowQRCode(byte[] data, byte displayTime, byte errorCorrection, byte mask, byte minVersion, byte maxVersion, byte[] blockColor, byte[] background, byte[] prompt)
	{
		Command command = MessageBuilder.BuildCommand("1822");
		command.addParam(129, displayTime);
		command.addParam(130, data);
		command.addParam(131, errorCorrection);
		command.addParam(132, mask);
		command.addParam(133, minVersion);
		command.addParam(134, maxVersion);
		command.addParam(135, blockColor);
		command.addParam(136, background);
		if (prompt != null)
		{
			command.addParam(137, prompt);
		}
		return command;
	}

	public static Command EnableBarCodeReader(byte timeout, byte encryptionMode)
	{
		Command command = MessageBuilder.BuildCommand("1804");
		command.addParam(129, 1);
		command.addParam(130, timeout);
		command.addParam(131, encryptionMode);
		return command;
	}

	public static Command EnableBarCodeReader(byte timeout, byte encryptionMode, byte[] customCommand)
	{
		Command command = MessageBuilder.BuildCommand("1804");
		command.addParam(129, 1);
		command.addParam(130, timeout);
		command.addParam(131, encryptionMode);
		if (customCommand != null)
		{
			command.addParam(132, customCommand);
		}
		return command;
	}

	public static Command DisableBarCodeReader()
	{
		Command command = MessageBuilder.BuildCommand("1804");
		command.addParam(129, 0);
		return command;
	}

	public static Command ResetDeviceCommand()
	{
		return MessageBuilder.BuildCommand("1F01");
	}

	public static Command GetItemsCommand(byte functionOID, byte[] oidBytes)
	{
		Command command = MessageBuilder.BuildCommand("D101");
		command.addParam(129, TLVParser.getByteArrayFromHexString("2B06010401F609"));
		command.addParam(133, functionOID);
		command.addParam(137, oidBytes);
		return command;
	}

	public static Command SetItemsCommand(byte functionOID, byte[] oidBytes)
	{
		Command command = MessageBuilder.BuildCommand("D111");
		command.addParam(129, TLVParser.getByteArrayFromHexString("2B06010401F609"));
		command.addParam(133, functionOID);
		command.addParam(137, oidBytes);
		return command;
	}

	public static Command LoadFirmwareCommand(byte indicator, byte[] fileID, byte[] fileHash, byte[] payload)
	{
		Command command = MessageBuilder.BuildCommand("D801");
		command.addParam(129, indicator);
		command.addParam(133, fileID);
		command.addParam(134, fileHash);
		command.addParam(135, payload);
		return command;
	}

	public static Command LoadFirmwareCommand(byte indicator, string fileIDHexString, string fileHashHexString, string payloadHexString)
	{
		return LoadFirmwareCommand(indicator, TLVParser.getByteArrayFromHexString(fileIDHexString), TLVParser.getByteArrayFromHexString(fileHashHexString), TLVParser.getByteArrayFromHexString(payloadHexString));
	}

	public static Command UpdateFirmwareFromFileCommand(byte indicator, byte options, byte[] fileID, byte[] fileHash)
	{
		Command command = MessageBuilder.BuildCommand("D901");
		command.addParam(129, indicator);
		command.addParam(130, options);
		command.addParam(133, fileID);
		command.addParam(134, fileHash);
		return command;
	}

	public static Command UpdateFirmwareFromFileCommand(byte indicator, byte options, string fileIDHexString, string fileHashHexString)
	{
		return UpdateFirmwareFromFileCommand(indicator, options, TLVParser.getByteArrayFromHexString(fileIDHexString), TLVParser.getByteArrayFromHexString(fileHashHexString));
	}

	public static Command GetFileCommand(byte[] fileID)
	{
		Command command = MessageBuilder.BuildCommand("D821");
		command.addParam(129, fileID);
		return command;
	}

	public static Command SendFileCommand(byte[] fileID, byte[] fileHash, byte[] payload)
	{
		Command command = MessageBuilder.BuildCommand("D811");
		command.addParam(129, fileID);
		byte[] byteArrayFromHexString = TLVParser.getByteArrayFromHexString(payload.Length.ToString("X8"));
		TLVObject tLVObject = new TLVObject(162);
		tLVObject.addTLVObject(new TLVObject(129, byteArrayFromHexString));
		tLVObject.addTLVObject(new TLVObject(130, new byte[1] { 4 }));
		tLVObject.addTLVObject(new TLVObject(131, fileHash));
		command.addParam(tLVObject);
		TLVObject tLVObject2 = new TLVObject(163);
		string hexString = TLVParser.getHexString(fileID);
		tLVObject2.addTLVObject(new TLVObject(129, TLVParser.getByteArrayFromASCIIString(hexString)));
		command.addParam(tLVObject2);
		command.addParam(135, 1);
		return command;
	}

	public static Command SendFileCommand(string fileIDHexString, string fileHashHexString, string payloadHexString)
	{
		return SendFileCommand(TLVParser.getByteArrayFromHexString(fileIDHexString), TLVParser.getByteArrayFromHexString(fileHashHexString), TLVParser.getByteArrayFromHexString(payloadHexString));
	}

	public static Command SendFileUnsecuredCommand(byte[] fileID, byte[] fileHash, byte[] payload)
	{
		Command command = MessageBuilder.BuildCommand("D812");
		command.addParam(129, fileID);
		byte[] byteArrayFromHexString = TLVParser.getByteArrayFromHexString(payload.Length.ToString("X8"));
		TLVObject tLVObject = new TLVObject(162);
		tLVObject.addTLVObject(new TLVObject(129, byteArrayFromHexString));
		tLVObject.addTLVObject(new TLVObject(130, new byte[1] { 4 }));
		tLVObject.addTLVObject(new TLVObject(131, fileHash));
		command.addParam(tLVObject);
		TLVObject tLVObject2 = new TLVObject(163);
		string hexString = TLVParser.getHexString(fileID);
		tLVObject2.addTLVObject(new TLVObject(129, TLVParser.getByteArrayFromASCIIString(hexString)));
		command.addParam(tLVObject2);
		command.addParam(135, 1);
		return command;
	}

	public static Command SendFileUnsecuredCommand(string fileIDHexString, string fileHashHexString, string payloadHexString)
	{
		return SendFileUnsecuredCommand(TLVParser.getByteArrayFromHexString(fileIDHexString), TLVParser.getByteArrayFromHexString(fileHashHexString), TLVParser.getByteArrayFromHexString(payloadHexString));
	}

	public static Command EchoCommand(byte[] dataBytes, byte[] data2Bytes, byte[] data3Bytes)
	{
		Command command = MessageBuilder.BuildCommand("DF01");
		command.addParam(129, dataBytes);
		if (data2Bytes != null || data3Bytes != null)
		{
			TLVObject tLVObject = new TLVObject(162);
			if (data2Bytes != null)
			{
				tLVObject.addTLVObject(new TLVObject(129, data2Bytes));
			}
			if (data3Bytes != null)
			{
				tLVObject.addTLVObject(new TLVObject(130, data3Bytes));
			}
			command.addParam(tLVObject);
		}
		return command;
	}

	public static Command EchoCommand(string dataHexString, byte[] data2Bytes, byte[] data3Byte)
	{
		return EchoCommand(TLVParser.getByteArrayFromHexString(dataHexString), data2Bytes, data3Byte);
	}

	public static Command PrettyPrintPayloadCommand(byte[] dataBytes)
	{
		Command command = MessageBuilder.BuildCommand("DF02");
		command.addParam(132, dataBytes);
		return command;
	}

	public static Command PrettyPrintPayloadCommand(string dataHexString)
	{
		return PrettyPrintPayloadCommand(TLVParser.getByteArrayFromHexString(dataHexString));
	}

	public static Command GetChallengeCommand(byte[] dataBytes)
	{
		Command command = MessageBuilder.BuildCommand("E001");
		command.addParam(129, dataBytes);
		return command;
	}

	public static Command GetChallengeCommand(string dataHexString)
	{
		return GetChallengeCommand(TLVParser.getByteArrayFromHexString(dataHexString));
	}

	public static Command SecureWrapperCommand(byte[] keyInfo, byte[] operations, byte[] deviceSerial, byte[] tokenBytes, byte[] payloadBytes)
	{
		Command command = MessageBuilder.BuildCommand("EEEE");
		TLVObject tLVObject = new TLVObject(161);
		tLVObject.addTLVObject(new TLVObject(129, operations));
		tLVObject.addTLVObject(new TLVObject(132, null));
		tLVObject.addTLVObject(new TLVObject(133, null));
		TLVObject tLVObject2 = new TLVObject(168);
		tLVObject2.addTLVObject(new TLVObject(129, keyInfo));
		tLVObject2.addTLVObject(new TLVObject(130, null));
		tLVObject2.addTLVObject(new TLVObject(134, null));
		tLVObject2.addTLVObject(new TLVObject(136, null));
		tLVObject.addTLVObject(tLVObject2);
		TLVObject tlvObject = new TLVObject(169);
		tLVObject.addTLVObject(tlvObject);
		command.addParam(tLVObject);
		command.addParam(130, deviceSerial);
		command.addParam(131, tokenBytes);
		command.addParam(132, payloadBytes);
		return command;
	}

	public static Command SecureWrapperCommand(string keyInfoHexString, string operationsHexString, string deviceSerialHexString, string tokenHexString, byte[] payloadBytes)
	{
		return SecureWrapperCommand(TLVParser.getByteArrayFromHexString(keyInfoHexString), TLVParser.getByteArrayFromHexString(operationsHexString), TLVParser.getByteArrayFromHexString(deviceSerialHexString), TLVParser.getByteArrayFromHexString(tokenHexString), payloadBytes);
	}

	public static void AddSignatureParam(Command command, byte[] signatureBytes)
	{
		command.addParam(158, signatureBytes);
	}

	public static void AddSignatureParam(Command command, string signatureHexString)
	{
		AddSignatureParam(command, TLVParser.getByteArrayFromHexString(signatureHexString));
	}

	public static Command TR31KeyInjectionCommand(byte[] keyBlockBytes)
	{
		Command command = MessageBuilder.BuildCommand("EF01");
		command.addParam(132, keyBlockBytes);
		return command;
	}

	public static Command TR31KeyInjectionCommand(string keyBlockHexString)
	{
		return TR31KeyInjectionCommand(TLVParser.getByteArrayFromHexString(keyBlockHexString));
	}

	public static Command GetKeySlotInfoCommand(byte[] keySlot)
	{
		Command command = MessageBuilder.BuildCommand("EF11");
		command.addParam(129, keySlot);
		return command;
	}

	public static Command GetKeySlotInfoCommand(string keySlotHexString)
	{
		return GetKeySlotInfoCommand(TLVParser.getByteArrayFromHexString(keySlotHexString));
	}

	public static Command WriteOTPCommand()
	{
		return MessageBuilder.BuildCommand("F013");
	}

	public static Command ActivateDeviceCommand(byte[] timeBytes)
	{
		Command command = MessageBuilder.BuildCommand("F016");
		command.addParam(129, timeBytes);
		return command;
	}

	public static Command ActivateDeviceCommand(string timeHexString)
	{
		return ActivateDeviceCommand(TLVParser.getByteArrayFromHexString(timeHexString));
	}

	public static Command EstablishEPKBPKCommand(byte[] epkBytes, byte[] nonceBytes)
	{
		Command command = MessageBuilder.BuildCommand("F017");
		TLVObject tLVObject = new TLVObject(161);
		tLVObject.addTLVObject(new TLVObject(129, 0));
		tLVObject.addTLVObject(new TLVObject(130, 0));
		command.addParam(tLVObject);
		command.addParam(131, epkBytes);
		command.addParam(132, nonceBytes);
		return command;
	}

	public static Command EstablishEPKBPKCommand(string epkHexString, string nonceHexString)
	{
		return EstablishEPKBPKCommand(TLVParser.getByteArrayFromHexString(epkHexString), TLVParser.getByteArrayFromHexString(nonceHexString));
	}
}
