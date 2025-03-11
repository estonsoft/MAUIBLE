// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.IMTCardData
public interface IMTCardData
{
	byte[] getData();

	string getMaskedTracks();

	string getTrack1();

	string getTrack2();

	string getTrack3();

	string getTrack1Masked();

	string getTrack2Masked();

	string getTrack3Masked();

	string getMagnePrint();

	string getMagnePrintStatus();

	string getDeviceSerial();

	string getSessionID();

	string getKSN();

	string getDeviceName();

	long getBatteryLevel();

	long getSwipeCount();

	void clearBuffers();

	string getCapMagnePrint();

	string getCapMagnePrintEncryption();

	string getCapMagneSafe20Encryption();

	string getCapMagStripeEncryption();

	string getCapMSR();

	string getCapTracks();

	long getCardDataCRC();

	string getCardExpDate();

	string getCardIIN();

	string getCardLast4();

	string getCardName();

	string getCardPAN();

	int getCardPANLength();

	string getCardServiceCode();

	string getCardStatus();

	string getCardEncodeType();

	int getDataFieldCount();

	string getHashCode();

	string getDeviceConfig(string configType);

	string getEncryptionStatus();

	string getFirmware();

	string getMagTekDeviceSerial();

	string getResponseType();

	string getTagValue(string tag, string data);

	string getTLVVersion();

	string getTrackDecodeStatus();

	string getTLVPayload();
}
