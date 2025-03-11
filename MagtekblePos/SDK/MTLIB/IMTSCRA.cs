// MTLib, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.IMTSCRA


public interface IMTSCRA
{
	void requestDeviceList(MTConnectionType connectionType);

	void setConnectionType(MTConnectionType connectionType);

	void setAddress(string deviceAddress);

	void openDevice();

	void closeDevice();

	bool isDeviceConnected();

	bool isDeviceEMV();

	bool isDeviceOEM();

	string getPowerManagementValue();

	MTDeviceFeatures getDeviceFeatures();

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

	void clearBuffers();

	long getBatteryLevel();

	long getSwipeCount();

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

	string getResponseData();

	string getResponseType();

	string getTagValue(string tag, string data);

	string getTLVVersion();

	string getTrackDecodeStatus();

	string getSDKVersion();

	int sendCommandToDevice(string command);

	int sendExtendedCommand(string command);

	int sendNFCCommand(string command, bool lastCommand = false, bool encrypt = false);

	int sendClassicNFCCommand(string command, bool lastCommand = false, bool encrypt = false);

	int sendDESFireNFCCommand(string command, bool lastCommand = false, bool encrypt = false);

	int updateFirmware(int type, byte[] data);
}
