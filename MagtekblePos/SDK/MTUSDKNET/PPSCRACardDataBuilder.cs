// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PPSCRACardDataBuilder
using System.Text;



internal class PPSCRACardDataBuilder
{
	protected const string TAG_GENERIC_DATA = "FA";

	protected const string TAG_DEVICE_SN = "DFDF25";

	protected const string TAG_MSR_DATA = "F4";

	protected const string TAG_TRACK1 = "DFDF31";

	protected const string TAG_TRACK2 = "DFDF33";

	protected const string TAG_TRACK3 = "DFDF35";

	protected const string TAG_TRACK1_STATUS = "DFDF30";

	protected const string TAG_TRACK2_STATUS = "DFDF32";

	protected const string TAG_TRACK3_STATUS = "DFDF34";

	protected const string TAG_ENC_TRACK1 = "DFDF37";

	protected const string TAG_ENC_TRACK2 = "DFDF39";

	protected const string TAG_ENC_TRACK3 = "DFDF3B";

	protected const string TAG_ENC_MP = "DFDF3C";

	protected const string TAG_ENC_MP_STATUS = "DFDF43";

	protected const string TAG_KSN = "DFDF50";

	protected const string TAG_CARD_TYPE = "DFDF52";

	protected static string buildTLV(string tag, string value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string hexString = TLVParser.getHexString(TLVParser.getLengthByteArray(value.Length / 2));
		stringBuilder.Append(tag);
		stringBuilder.Append(hexString);
		stringBuilder.Append(value);
		return stringBuilder.ToString();
	}

	public static string buildTLVPayload(ref CARD_DATA cardData)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		string serialNumber = ((CARD_DATA)(cardData)).SerialNumber;
		string hexString = TLVParser.getHexString(Encoding.UTF8.GetBytes(serialNumber));
		string text = buildTLV("DFDF25", hexString);
		string hexString2 = TLVParser.getHexString(cardData.CardStatus);
		string text2 = buildTLV("DFDF52", hexString2);
		string hexString3 = TLVParser.getHexString(Encoding.UTF8.GetBytes(((CARD_DATA)(cardData)).Track1));
		string hexString4 = TLVParser.getHexString(Encoding.UTF8.GetBytes(((CARD_DATA)(cardData)).Track2));
		string hexString5 = TLVParser.getHexString(Encoding.UTF8.GetBytes(((CARD_DATA)(cardData)).Track3));
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(buildTLV("DFDF31", hexString3));
		stringBuilder.Append(buildTLV("DFDF33", hexString4));
		stringBuilder.Append(buildTLV("DFDF35", hexString5));
		stringBuilder.Append(buildTLV("DFDF30", TLVParser.getHexString(cardData.Track1Status)));
		stringBuilder.Append(buildTLV("DFDF32", TLVParser.getHexString(cardData.Track2Status)));
		stringBuilder.Append(buildTLV("DFDF34", TLVParser.getHexString(cardData.Track3Status)));
		stringBuilder.Append(buildTLV("DFDF37", ((CARD_DATA)(cardData)).EncTrack1));
		stringBuilder.Append(buildTLV("DFDF39", ((CARD_DATA)(cardData)).EncTrack2));
		stringBuilder.Append(buildTLV("DFDF3B", ((CARD_DATA)(cardData)).EncTrack3));
		stringBuilder.Append(buildTLV("DFDF3C", ((CARD_DATA)(cardData)).EncMP));
		stringBuilder.Append(buildTLV("DFDF43", ((CARD_DATA)(cardData)).MPSTS));
		stringBuilder.Append(buildTLV("DFDF50", ((CARD_DATA)(cardData)).KSN));
		string text3 = buildTLV("F4", stringBuilder.ToString());
		return buildTLV("FA", text + text2 + text3);
	}
}
