// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTPayloadBuilder
using System;
using System.Text;


public class MTPayloadBuilder
{
	protected const string TAG_GENERIC_DATA = "FA";

	protected const string TAG_DEVICE_SN = "DFDF25";

	protected const string TAG_MSR_DATA = "F4";

	protected const string TAG_ENC_TRACK1 = "DFDF37";

	protected const string TAG_ENC_TRACK2 = "DFDF39";

	protected const string TAG_ENC_TRACK3 = "DFDF3B";

	protected const string TAG_ENC_MP = "DFDF3C";

	protected const string TAG_ENC_MP_STATUS = "DFDF43";

	protected const string TAG_KSN = "DFDF50";

	protected static string buildTLV(string tag, string value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = value.Length / 2;
		byte[] array;
		if (num < 128)
		{
			array = new byte[1] { (byte)num };
		}
		else
		{
			int i;
			for (i = 1; (double)num / Math.Pow(256.0, i) >= 1.0; i++)
			{
			}
			array = new byte[i + 1];
			array[0] = (byte)(128 + i);
			int num2 = i;
			for (int j = 0; j < i; j++)
			{
				num2--;
				array[j + 1] = (byte)((num >> num2 * 8) & 0xFF);
			}
		}
		string hexString = MTParser.getHexString(array);
		stringBuilder.Append(tag);
		stringBuilder.Append(hexString);
		stringBuilder.Append(value);
		return stringBuilder.ToString();
	}

	public static string buildTLVPayload(IMTCardData cardData)
	{
		string deviceSerial = cardData.getDeviceSerial();
		string hexString = MTParser.getHexString(Encoding.UTF8.GetBytes(deviceSerial));
		string text = buildTLV("DFDF25", hexString);
		StringBuilder stringBuilder = new StringBuilder();
		string track = cardData.getTrack1();
		string track2 = cardData.getTrack2();
		string track3 = cardData.getTrack3();
		string magnePrint = cardData.getMagnePrint();
		string magnePrintStatus = cardData.getMagnePrintStatus();
		string kSN = cardData.getKSN();
		stringBuilder.Append(buildTLV("DFDF37", track));
		stringBuilder.Append(buildTLV("DFDF39", track2));
		stringBuilder.Append(buildTLV("DFDF3B", track3));
		stringBuilder.Append(buildTLV("DFDF3C", magnePrint));
		stringBuilder.Append(buildTLV("DFDF43", magnePrintStatus));
		stringBuilder.Append(buildTLV("DFDF50", kSN));
		string text2 = buildTLV("F4", stringBuilder.ToString());
		return buildTLV("FA", text + text2);
	}
}
