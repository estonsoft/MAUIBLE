// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.IMTCardDataHandler
public interface IMTCardDataHandler
{
	void setDataThreshold(int nBytes);

	void setData(byte[] data);

	void handleData(byte[] data);

	bool isDataReady();

	void clearData();
}
