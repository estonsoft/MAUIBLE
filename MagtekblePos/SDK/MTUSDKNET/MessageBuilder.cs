// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MessageBuilder


public class MessageBuilder
{
	private static byte API_VER;

	public static Message BuildMessage()
	{
		return new Message(new byte[2] { 170, API_VER }, null);
	}

	public static Message BuildMessage(byte[] dataBytes)
	{
		return new Message(new byte[2] { 170, API_VER }, dataBytes);
	}

	public static Message BuildMessage(string dataHexString)
	{
		return BuildMessage(TLVParser.getByteArrayFromHexString(dataHexString));
	}

	public static Command BuildCommand(byte[] commandBytes)
	{
		return new Command(commandBytes, null);
	}

	public static Command BuildCommand(string commandHexString)
	{
		return BuildCommand(TLVParser.getByteArrayFromHexString(commandHexString));
	}

	public static TLVObject BuildMessageInfoForCommand(byte[] commandBytes)
	{
		return BuildMessageInfoForCommand(TLVParser.getHexString(commandBytes));
	}

	public static TLVObject BuildMessageInfoForCommand(string commandHexString)
	{
		return new TLVObject("81", "0100" + commandHexString);
	}

	public static TLVObject BuildMessageInfoForDataFile(byte[] commandIDBytes, byte[] fileIDBytes)
	{
		return BuildMessageInfoForDataFile(TLVParser.getHexString(commandIDBytes), TLVParser.getHexString(fileIDBytes));
	}

	public static TLVObject BuildMessageInfoForDataFile(string commandIDHexString, string fileIDHexString)
	{
		return new TLVObject("81", "0400" + commandIDHexString + fileIDHexString);
	}

	public static TLVObject BuildMessagePayload(byte[] payloadBytes)
	{
		return new TLVObject(TLVParser.getByteArrayFromHexString("84"), payloadBytes);
	}

	public static TLVObject BuildMessagePayload(string payloadHexString)
	{
		return BuildMessagePayload(TLVParser.getByteArrayFromHexString(payloadHexString));
	}
}
