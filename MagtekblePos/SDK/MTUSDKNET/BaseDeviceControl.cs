// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.BaseDeviceControl
using System.Threading;


public class BaseDeviceControl : IDeviceControl
{
	protected AutoResetEvent mDeviceResponseEvent;

	protected IData mDeviceResponseData;

	public virtual bool open()
	{
		return false;
	}

	public virtual bool close()
	{
		return false;
	}

	public virtual bool send(IData data)
	{
		return false;
	}

	public virtual IResult sendSync(IData data)
	{
		IResult result = null;
		mDeviceResponseEvent = new AutoResetEvent(initialState: false);
		send(data);
		if (mDeviceResponseEvent.WaitOne(3000))
		{
			if (mDeviceResponseData != null)
			{
				result = new Result(StatusCode.SUCCESS, mDeviceResponseData);
			}
		}
		else
		{
			result = new Result(StatusCode.TIMEOUT);
		}
		mDeviceResponseEvent = null;
		return result;
	}

	public virtual bool sendExtendedCommand(IData data)
	{
		return false;
	}

	public virtual bool endSession()
	{
		return false;
	}

	public virtual bool setDateTime(IData data)
	{
		return false;
	}

	public virtual bool playSound(IData data)
	{
		return false;
	}

	public virtual bool getInput(IData data)
	{
		return false;
	}

	public virtual bool displayMessage(byte messageID, byte timeout)
	{
		return false;
	}

	public virtual bool showImage(byte imageID)
	{
		return false;
	}

	public virtual bool showImage(ImageData data, byte timeout)
	{
		return false;
	}

	public virtual bool showBarCode(BarCodeRequest request, byte timeout, IData prompt = null)
	{
		return false;
	}

	public virtual bool startBarCodeReader(byte timeout, byte encryptionMode)
	{
		return false;
	}

	public virtual bool stopBarCodeReader()
	{
		return false;
	}

	public virtual bool setLatch(bool enableLock)
	{
		return false;
	}

	public virtual bool deviceReset()
	{
		return false;
	}
}
