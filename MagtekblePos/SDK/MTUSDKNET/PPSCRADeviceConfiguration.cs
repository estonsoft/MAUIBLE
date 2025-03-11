// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PPSCRADeviceConfiguration
using System.Threading;



public class PPSCRADeviceConfiguration : BaseDeviceConfiguration
{
	private static int COMMAND_RESPONSE_TIMEOUT = 2000;

	private MTPPSCRA mPPSCRA;

	private string mPath = "";

	private IConfigurationCallback mConfigurationCallback;

	private AutoResetEvent mGetConfigEvent;

	private AutoResetEvent mSetConfigEvent;

	private byte mConfigType;

	private ushort mFirmwareType;

	private ushort mFileType;

	public PPSCRADeviceConfiguration(MTPPSCRA ppscra, string path)
	{
		mPPSCRA = ppscra;
		mPath = path;
	}

	public override string getDeviceInfo(InfoType infoType)
	{
		string result = "";
		switch (infoType)
		{
		case InfoType.DeviceSerialNumber:
			result = getDeviceSerial();
			break;
		case InfoType.FirmwareVersion:
			result = getFirmwareVersion();
			break;
		case InfoType.DeviceCapabilities:
			result = getDeviceCapabilities();
			break;
		}
		return result;
	}

	public override byte[] getConfigInfo(byte configType, byte[] data)
	{
		mConfigType = configType;
		byte[] result = null;
		string text = TLVParser.getHexString(configType) + TLVParser.getHexString(data);
		if (configType != 9 && configType != 227)
		{
			mGetConfigEvent = new AutoResetEvent(initialState: false);
			mPPSCRA.sendSpecialCommand(text);
			if (mGetConfigEvent.WaitOne(COMMAND_RESPONSE_TIMEOUT))
			{
				result = mPPSCRA.getSpecialCommand(text);
			}
			mGetConfigEvent = null;
		}
		else
		{
			result = mPPSCRA.getSpecialCommand(text);
		}
		return result;
	}

	public override byte[] getKeyInfo(byte keyType, byte[] data)
	{
		return null;
	}

	public override int setConfigInfo(byte configType, byte[] data, IConfigurationCallback callback)
	{
		mConfigType = configType;
		mConfigurationCallback = callback;
		int result = -1;
		string text = TLVParser.getHexString(configType) + TLVParser.getHexString(data);
		if (configType != 9)
		{
			mSetConfigEvent = new AutoResetEvent(initialState: false);
			mPPSCRA.sendSpecialCommand(text);
			if (mSetConfigEvent.WaitOne(COMMAND_RESPONSE_TIMEOUT))
			{
				result = 0;
			}
			mSetConfigEvent = null;
		}
		return result;
	}

	public override int updateFirmware(ushort firmwareType, byte[] data, IConfigurationCallback callback)
	{
		mFirmwareType = firmwareType;
		mConfigurationCallback = callback;
		return 0;
	}

	public override int sendFile(byte[] fileID, byte[] data, IConfigurationCallback callback)
	{
		return 0;
	}

	public void processDataReady(byte[] data)
	{
		if (mGetConfigEvent != null)
		{
			mGetConfigEvent.Set();
		}
		else if (mSetConfigEvent != null)
		{
			mSetConfigEvent.Set();
		}
	}

	protected string getDeviceSerial()
	{
		int num = 0;
		return mPPSCRA.requestDeviceInformation(5, ref num);
	}

	protected string getFirmwareVersion()
	{
		int num = 0;
		return mPPSCRA.requestDeviceInformation(6, ref num);
	}

	protected string getDeviceCapabilities()
	{
		int num = 0;
		return mPPSCRA.requestDeviceInformation(2, ref num);
	}
}
