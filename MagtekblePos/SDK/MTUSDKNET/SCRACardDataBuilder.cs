// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.SCRACardDataBuilder
using System.Text;



internal class SCRACardDataBuilder
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

	protected static string buildTLV(string tag, string value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string hexString = TLVParser.getHexString(TLVParser.getLengthByteArray(value.Length / 2));
		stringBuilder.Append(tag);
		stringBuilder.Append(hexString);
		stringBuilder.Append(value);
		return stringBuilder.ToString();
	}

	public static string buildTLVPayload(IMTCardData cardData)
	{
		string deviceSerial = cardData.getDeviceSerial();
		string hexString = TLVParser.getHexString(Encoding.UTF8.GetBytes(deviceSerial));
		string text = buildTLV("DFDF25", hexString);
		string hexString2 = TLVParser.getHexString(Encoding.UTF8.GetBytes(cardData.getTrack1Masked()));
		string hexString3 = TLVParser.getHexString(Encoding.UTF8.GetBytes(cardData.getTrack2Masked()));
		string hexString4 = TLVParser.getHexString(Encoding.UTF8.GetBytes(cardData.getTrack3Masked()));
		string trackDecodeStatus = cardData.getTrackDecodeStatus();
		string value = trackDecodeStatus.Substring(0, 2);
		string value2 = trackDecodeStatus.Substring(2, 2);
		string value3 = trackDecodeStatus.Substring(4, 2);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(buildTLV("DFDF31", hexString2));
		stringBuilder.Append(buildTLV("DFDF33", hexString3));
		stringBuilder.Append(buildTLV("DFDF35", hexString4));
		stringBuilder.Append(buildTLV("DFDF30", value));
		stringBuilder.Append(buildTLV("DFDF32", value2));
		stringBuilder.Append(buildTLV("DFDF34", value3));
		stringBuilder.Append(buildTLV("DFDF37", cardData.getTrack1()));
		stringBuilder.Append(buildTLV("DFDF39", cardData.getTrack2()));
		stringBuilder.Append(buildTLV("DFDF3B", cardData.getTrack3()));
		stringBuilder.Append(buildTLV("DFDF3C", cardData.getMagnePrint()));
		stringBuilder.Append(buildTLV("DFDF43", cardData.getMagnePrintStatus()));
		stringBuilder.Append(buildTLV("DFDF50", cardData.getKSN()));
		string text2 = buildTLV("F4", stringBuilder.ToString());
		return buildTLV("FA", text + text2);
	}
}
