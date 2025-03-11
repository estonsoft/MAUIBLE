// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PPSCRADeviceControl
using System;



public class PPSCRADeviceControl : BaseDeviceControl
{
	private PPSCRADevice mDevice;

	private MTPPSCRA mPPSCRA;

	private string mPath = "";

	private CertificateInfo mCertificateInfo;

	public PPSCRADeviceControl(PPSCRADevice device, MTPPSCRA ppscra, string path, CertificateInfo certificateInfo)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		mDevice = device;
		mPPSCRA = ppscra;
		mPath = path;
		mCertificateInfo = certificateInfo;
		mPPSCRA.onDataReady += new OnDataReadyCompleteEvent(OnDataReady);
	}

	public override bool open()
	{
		if (mPath.ToUpper().Contains("TLS12") && mCertificateInfo != null)
		{
			mPPSCRA.loadClientCertificate(mCertificateInfo.getFormat(), mCertificateInfo.getData(), mCertificateInfo.getPassword());
		}
		mPPSCRA.openDevice(mPath);
		return true;
	}

	public override bool close()
	{
		mPPSCRA.closeDevice();
		return true;
	}

	public override bool displayMessage(byte messageID, byte timeout)
	{
		int num = 0;
		mPPSCRA.setDisplayMessage((int)timeout, (int)messageID, ref num);
		return true;
	}

	public override bool send(IData data)
	{
		if (data != null)
		{
			byte[] byteArrayFromHexString = TLVParser.getByteArrayFromHexString(data.StringValue);
			if (byteArrayFromHexString != null)
			{
				int num = byteArrayFromHexString.Length - 1;
				if (num > 0)
				{
					byte b = byteArrayFromHexString[0];
					byte[] array = new byte[num];
					Array.Copy(byteArrayFromHexString, 1, array, 0, num);
					switch (b)
					{
					case 0:
					{
						byte[] specialCommand = mPPSCRA.getSpecialCommand(TLVParser.getHexString(array));
						if (mDevice != null)
						{
							mDevice.OnDataReady(specialCommand);
						}
						break;
					}
					case 1:
						mPPSCRA.sendSpecialCommand(TLVParser.getHexString(array));
						break;
					}
				}
			}
		}
		return true;
	}

	public override bool endSession()
	{
		mPPSCRA.endSession(0);
		return true;
	}

	protected void OnDataReady(byte[] data)
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
