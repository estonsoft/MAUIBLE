// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.SCRADeviceControl



public class SCRADeviceControl : BaseDeviceControl
{
	protected MTSCRA mSCRA;

	public SCRADeviceControl(MTSCRA scra)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		mSCRA = scra;
		mSCRA.OnDeviceResponse += new DeviceResponseHandler(OnDeviceResponse);
	}

	public override bool open()
	{
		mSCRA.openDevice();
		return true;
	}

	public override bool close()
	{
		mSCRA.closeDevice();
		return true;
	}

	public override bool send(IData data)
	{
		if (data != null)
		{
			mSCRA.sendCommandToDevice(data.StringValue);
		}
		return true;
	}

	public override bool sendExtendedCommand(IData data)
	{
		if (data != null)
		{
			mSCRA.sendExtendedCommand(data.StringValue);
		}
		return true;
	}

	protected void OnDeviceResponse(object sender, string data)
	{
		if (data != null)
		{
			mDeviceResponseData = new BaseData(data);
			if (mDeviceResponseEvent != null)
			{
				mDeviceResponseEvent.Set();
			}
		}
	}
}
