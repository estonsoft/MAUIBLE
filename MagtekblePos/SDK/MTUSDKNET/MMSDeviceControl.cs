// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSDeviceControl
using System.Text;



public class MMSDeviceControl : BaseDeviceControl
{
	private MMXDevice mMMXDevice;

	private string mPath = "";

	public MMSDeviceControl(MMXDevice device, string path)
	{
		mMMXDevice = device;
		mPath = path;
		mMMXDevice.onMessage += OnMessage;
	}

	public override bool open()
	{
		mMMXDevice.open(mPath);
		return true;
	}

	public override bool close()
	{
		mMMXDevice.close();
		return true;
	}

	public override bool displayMessage(byte messageID, byte timeout)
	{
		Command command = MMSCommandBuilder.DisplayMessageCommand(messageID, timeout);
		sendCommand(command);
		return true;
	}

	public override bool showImage(byte imageID)
	{
		Command command = MMSCommandBuilder.ShowImageCommand(imageID, 0);
		sendCommand(command);
		return true;
	}

	public override bool showImage(ImageData data, byte timeout)
	{
		Command command = MMSCommandBuilder.ShowBitmapImageCommand(data.Data, timeout, data.BackgroundColor);
		sendCommand(command);
		return true;
	}

	public override bool showBarCode(BarCodeRequest request, byte timeout, IData prompt)
	{
		byte[] prompt2 = null;
		if (prompt != null)
		{
			string stringValue = prompt.StringValue;
			if (stringValue.Length > 0)
			{
				prompt2 = Encoding.ASCII.GetBytes(stringValue);
			}
		}
		Command command = MMSCommandBuilder.ShowQRCode(request.Data, timeout, request.ErrorCorrection, request.MaskPattern, request.MinVersion, request.MaxVersion, request.BlockColor, request.BackgroundColor, prompt2);
		sendCommand(command);
		return true;
	}

	public override bool startBarCodeReader(byte timeout, byte encryptionMode)
	{
		Command command = MMSCommandBuilder.EnableBarCodeReader(timeout, encryptionMode);
		sendCommand(command);
		return true;
	}

	public override bool stopBarCodeReader()
	{
		Command command = MMSCommandBuilder.DisableBarCodeReader();
		sendCommand(command);
		return true;
	}

	public override bool deviceReset()
	{
		Command command = MMSCommandBuilder.ResetDeviceCommand();
		sendCommand(command);
		return true;
	}

	protected void sendCommand(Command command)
	{
		Message message = MessageBuilder.BuildMessage();
		message.addMessageInfoForCommand(command.getTagByteArray());
		message.addPayload(command.getByteArray());
		byte[] byteArray = message.getByteArray();
		if (byteArray != null)
		{
			MMXMessage message2 = new MMXMessage(48, byteArray);
			mMMXDevice.sendMessage(message2);
		}
	}

	public override bool send(IData data)
	{
		if (data != null)
		{
			string stringValue = data.StringValue;
			if (!string.IsNullOrEmpty(stringValue))
			{
				byte[] byteArrayFromHexString = TLVParser.getByteArrayFromHexString(stringValue);
				if (byteArrayFromHexString != null && byteArrayFromHexString.Length != 0)
				{
					mMMXDevice.sendMessage(new MMXMessage(48, byteArrayFromHexString));
				}
			}
		}
		return true;
	}

	protected void OnMessage(object sender, byte[] data)
	{
		TLVParser.getHexString(data);
		Message message = MessageParser.GetMessage(data);
		if (message != null && message.isResponse())
		{
			mDeviceResponseData = new BaseData(data);
			if (mDeviceResponseEvent != null)
			{
				mDeviceResponseEvent.Set();
			}
		}
	}
}
