// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.IDeviceControl


public interface IDeviceControl
{
	bool open();

	bool close();

	bool send(IData data);

	IResult sendSync(IData data);

	bool sendExtendedCommand(IData data);

	bool endSession();

	bool setDateTime(IData data);

	bool playSound(IData data);

	bool getInput(IData data);

	bool displayMessage(byte messageID, byte timeout);

	bool showImage(byte imageID);

	bool showImage(ImageData data, byte timeout);

	bool showBarCode(BarCodeRequest request, byte timeout, IData prompt = null);

	bool startBarCodeReader(byte timeout, byte encryptionMode);

	bool stopBarCodeReader();

	bool setLatch(bool enableLock);

	bool deviceReset();
}
