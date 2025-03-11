// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.CMFDeviceControl



public class CMFDeviceControl : BaseDeviceControl
{
	private MTDevice mDevice;

	public CMFDeviceControl(MTDevice device)
	{
		mDevice = device;
	}

	public override bool open()
	{
		mDevice.setConnectionType((MTConnectionType)0);
		mDevice.openDevice();
		return true;
	}

	public override bool close()
	{
		mDevice.closeDevice();
		return true;
	}

	public override bool send(IData data)
	{
		mDevice.sendDataString(data.StringValue);
		return true;
	}
}
