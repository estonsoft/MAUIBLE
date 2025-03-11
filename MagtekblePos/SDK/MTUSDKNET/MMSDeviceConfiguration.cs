// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSDeviceConfiguration
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



public class MMSDeviceConfiguration : BaseDeviceConfiguration
{
	private static int COMMAND_RESPONSE_TIMEOUT = 2000;

	private static int KEY_UPDATE_COMMAND_RESPONSE_TIMEOUT = 15000;

	private MMXDevice mMMXDevice;

	private AutoResetEvent mResponseEvent;

	private byte mResponseOperationStatus = byte.MaxValue;

	private ResponsePayload mResponsePayload;

	private MMSResponse mResponse;

	private string mDeviceSerial = "";

	private string mChallengeToken = "";

	private IConfigurationCallback mConfigurationCallback;

	private byte mConfigType;

	private byte mKeyInfoType;

	private ushort mFirmwareType;

	private byte[] mData;

	private byte[] mFileID;

	private float mProgressValue;

	private AutoResetEvent mLoadFirmwareEvent;

	private bool mLoadFirmwareResult;

	private AutoResetEvent mCommitFirmwareEvent;

	private bool mCommitFirmwareResult;

	public MMSDeviceConfiguration(MMXDevice device)
	{
		mMMXDevice = device;
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
		case InfoType.Boot1Version:
			result = getBoot1Version();
			break;
		case InfoType.Boot0Version:
			result = getBoot0Version();
			break;
		case InfoType.FirmwareHash:
			result = getFirmwareHash();
			break;
		case InfoType.TamperStatus:
			result = getTamperStatus();
			break;
		case InfoType.OperationStatus:
			result = getOperationStatus();
			break;
		case InfoType.OfflineDetail:
			result = getOfflineDetail();
			break;
		}
		return result;
	}

	public override byte[] getConfigInfo(byte configType, byte[] data)
	{
		return getItemsValue(configType, data);
	}

	public override byte[] getKeyInfo(byte keyType, byte[] data)
	{
		mKeyInfoType = keyType;
		byte[] result = null;
		Command keySlotInfoCommand = MMSCommandBuilder.GetKeySlotInfoCommand(data);
		ResponsePayload responsePayload = sendCommandAndReceive(keySlotInfoCommand);
		if (responsePayload != null)
		{
			result = responsePayload.getValueByteArray();
		}
		return result;
	}

	public override int updateKeyInfo(byte keyType, byte[] data, IConfigurationCallback callback)
	{
		mKeyInfoType = keyType;
		mConfigurationCallback = callback;
		mData = (byte[])data.Clone();
		Task.Run(async delegate
		{
			Command command = MMSCommandBuilder.TR31KeyInjectionCommand(data);
			StatusCode status = sendCommandSync(command, KEY_UPDATE_COMMAND_RESPONSE_TIMEOUT);
			sendCallbackOnResult(status, null);
		});
		return 0;
	}

	public override byte[] getChallengeToken(byte[] data)
	{
		byte[] result = null;
		if (executeGetChallege(data))
		{
			result = TLVParser.getByteArrayFromHexString(mChallengeToken);
		}
		return result;
	}

	public override int setConfigInfo(byte configType, byte[] data, IConfigurationCallback callback)
	{
		mConfigType = configType;
		mConfigurationCallback = callback;
		mData = (byte[])data.Clone();
		Task.Run(async delegate
		{
			StatusCode status = StatusCode.ERROR;
			byte[] array = setItemsData(mConfigType, mData);
			if (array != null)
			{
				status = StatusCode.SUCCESS;
			}
			sendCallbackOnResult(status, array);
		});
		return 0;
	}

	protected void sendCallbackOnProgress(int progress)
	{
		try
		{
			if (mConfigurationCallback != null)
			{
				mConfigurationCallback.OnProgress(progress);
			}
		}
		catch (Exception)
		{
		}
	}

	protected void sendCallbackOnResult(StatusCode status, byte[] data)
	{
		try
		{
			if (mConfigurationCallback != null)
			{
				mConfigurationCallback.OnResult(status, data);
			}
		}
		catch (Exception)
		{
		}
	}

	protected IResult sendCallbackOnCalculateMAC(byte macType, byte[] data)
	{
		IResult result = new Result(StatusCode.UNAVAILABLE);
		try
		{
			if (mConfigurationCallback != null)
			{
				result = mConfigurationCallback.OnCalculateMAC(macType, data);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public override int updateFirmware(ushort firmwareType, byte[] data, IConfigurationCallback callback)
	{
		mFirmwareType = firmwareType;
		mData = (byte[])data.Clone();
		mConfigurationCallback = callback;
		mProgressValue = -1f;
		Task.Run(async delegate
		{
			sendAndUpdateFirmware();
		});
		return 0;
	}

	protected async void sendAndUpdateFirmware()
	{
		if (mData == null || mData.Length == 0)
		{
			return;
		}
		await Task.Run(async delegate
		{
			bool updateFirmwareResult = false;
			mLoadFirmwareEvent = new AutoResetEvent(initialState: false);
			mLoadFirmwareResult = false;
			mCommitFirmwareEvent = new AutoResetEvent(initialState: false);
			mCommitFirmwareResult = false;
			try
			{
				updateProgress(0.02f);
				SHA256 sHA = SHA256.Create();
				byte[] hash = sHA.ComputeHash(mData);
				byte[] fileID = new byte[2]
				{
					(byte)(mFirmwareType >> 8),
					(byte)(mFirmwareType & 0xFF)
				};
				Command command = MMSCommandBuilder.LoadFirmwareCommand(3, fileID, hash, mData);
				bool loadFirmwareResponseResult = false;
				bool loadFirmwareResult = false;
				byte[] array = await sendFirmwareWithProgress(command);
				if (array != null)
				{
					Message message = MessageParser.GetMessage(array);
					if (message != null && message.isResponse())
					{
						MMSResponse mMSResponse = new MMSResponse(message);
						if (mMSResponse != null && mMSResponse.getOperationStatus() == 0)
						{
							loadFirmwareResponseResult = true;
						}
					}
				}
				if (loadFirmwareResponseResult)
				{
					updateProgress(0.95f);
					if (mLoadFirmwareEvent.WaitOne(10000) && mLoadFirmwareResult)
					{
						loadFirmwareResult = true;
					}
				}
				if (loadFirmwareResult)
				{
					updateProgress(0.97f);
					Command command2 = MMSCommandBuilder.UpdateFirmwareFromFileCommand(3, 0, fileID, hash);
					bool flag = false;
					byte[] array2 = sendAndReceiveCommand(command2);
					if (array2 != null)
					{
						Message message2 = MessageParser.GetMessage(array2);
						if (message2 != null && message2.isResponse())
						{
							MMSResponse mMSResponse2 = new MMSResponse(message2);
							if (mMSResponse2 != null && mMSResponse2.getOperationStatus() == 0)
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						updateProgress(0.98f);
						if (mCommitFirmwareEvent.WaitOne(20000) && mCommitFirmwareResult)
						{
							updateFirmwareResult = true;
						}
					}
				}
			}
			catch (Exception)
			{
			}
			if (updateFirmwareResult)
			{
				updateProgress(1f);
				sendCallbackOnResult(StatusCode.SUCCESS, null);
			}
			else
			{
				sendCallbackOnResult(StatusCode.ERROR, null);
			}
			mLoadFirmwareEvent = null;
			mLoadFirmwareResult = false;
			mCommitFirmwareEvent = null;
			mCommitFirmwareResult = false;
		});
	}

	protected async Task<byte[]> sendFileWithProgress(byte[] commandID, byte[] fileID, byte[] fileData)
	{
		byte[] result = null;
		Message message = MessageBuilder.BuildMessage();
		message.addMessageInfoForDataFile(commandID, fileID);
		message.addPayload(fileData);
		byte[] byteArray = message.getByteArray();
		if (byteArray != null)
		{
			result = mMMXDevice.SendAndReceive(byteArray, 60000, delegate(float x)
			{
				updateProgress(0.05f + x * 0.9f);
			});
		}
		return result;
	}

	protected async Task<byte[]> sendFirmwareWithProgress(Command command)
	{
		byte[] result = null;
		Message message = MessageBuilder.BuildMessage();
		message.addMessageInfoForCommand(command.getTagByteArray());
		message.addPayload(command.getByteArray());
		byte[] byteArray = message.getByteArray();
		if (byteArray != null)
		{
			result = await mMMXDevice.SendAndReceiveAsync(byteArray, 60000, delegate(float x)
			{
				updateProgress(0.05f + x * 0.9f);
			});
		}
		return result;
	}

	protected byte[] sendAndReceiveCommand(Command command)
	{
		Message message = MessageBuilder.BuildMessage();
		message.addMessageInfoForCommand(command.getTagByteArray());
		message.addPayload(command.getByteArray());
		return sendAndReceiveMessage(message);
	}

	protected byte[] sendAndReceiveSecuredCommand(byte[] commandID, byte[] payload)
	{
		Message message = MessageBuilder.BuildMessage();
		message.addMessageInfoForCommand(commandID);
		message.addPayload(payload);
		return sendAndReceiveMessage(message);
	}

	protected byte[] sendAndReceiveMessage(Message message)
	{
		byte[] result = null;
		byte[] byteArray = message.getByteArray();
		if (byteArray != null)
		{
			result = mMMXDevice.SendAndReceive(byteArray, 5000);
		}
		return result;
	}

	private void updateProgress(float value)
	{
		if (!(value * 100f >= mProgressValue + 1f))
		{
			return;
		}
		try
		{
			if (value * 100f > mProgressValue)
			{
				mProgressValue = value * 100f;
			}
			sendCallbackOnProgress((int)mProgressValue);
		}
		catch
		{
		}
	}

	public override int getFile(byte[] fileID, IConfigurationCallback callback)
	{
		mFileID = (byte[])fileID.Clone();
		mConfigurationCallback = callback;
		mProgressValue = -1f;
		Task.Run(async delegate
		{
			prepareAndGetFile();
		});
		return 0;
	}

	public override int sendFile(byte[] fileID, byte[] data, IConfigurationCallback callback)
	{
		mFileID = (byte[])fileID.Clone();
		mData = (byte[])data.Clone();
		mConfigurationCallback = callback;
		mProgressValue = -1f;
		Task.Run(async delegate
		{
			prepareAndSendFile();
		});
		return 0;
	}

	public override int sendSecureFile(byte[] fileID, byte[] data, IConfigurationCallback callback)
	{
		mFileID = (byte[])fileID.Clone();
		mData = (byte[])data.Clone();
		mConfigurationCallback = callback;
		mProgressValue = -1f;
		Task.Run(async delegate
		{
			prepareAndSendSecureFile();
		});
		return 0;
	}

	public override int sendImage(byte imageID, byte[] data, IConfigurationCallback callback)
	{
		byte[] fileID = new byte[4]
		{
			2,
			0,
			0,
			(byte)(imageID - 1)
		};
		return sendFile(fileID, data, callback);
	}

	public override int setDisplayImage(byte imageID)
	{
		int result = -1;
		byte[] obj = new byte[11]
		{
			226, 9, 227, 7, 225, 5, 225, 3, 193, 1,
			0
		};
		obj[10] = imageID;
		byte[] oidTagTreeAndValue = obj;
		if (setItemsData(1, oidTagTreeAndValue) != null)
		{
			result = 0;
		}
		return result;
	}

	protected async void prepareAndGetFile()
	{
		if (mFileID != null && mFileID.Length >= 4)
		{
			await Task.Run(async delegate
			{
				try
				{
					updateProgress(0.02f);
					Command fileCommand = MMSCommandBuilder.GetFileCommand(mFileID);
					ResponsePayload responsePayload = sendCommandAndReceive(fileCommand);
					if (responsePayload != null)
					{
						TLVParser.getHexString(responsePayload.getParamValue("81"));
						TLVObject paramObject = responsePayload.getParamObject("A2");
						if (paramObject != null)
						{
							paramObject.findByTagHexString("81").getValueByteArray();
							paramObject.findByTagHexString("82").getValueByteArray();
							paramObject.findByTagHexString("83").getValueByteArray();
							updateProgress(0.1f);
							return;
						}
					}
				}
				catch (Exception)
				{
				}
				sendCallbackOnResult(StatusCode.ERROR, null);
			});
		}
		else
		{
			sendCallbackOnResult(StatusCode.UNAVAILABLE, null);
		}
	}

	protected async void prepareAndSendFile()
	{
		if (mData == null || mData.Length == 0)
		{
			return;
		}
		await Task.Run(async delegate
		{
			try
			{
				updateProgress(0.02f);
				byte[] fileHash = SHA256.Create().ComputeHash(mData);
				Command command = MMSCommandBuilder.SendFileUnsecuredCommand(mFileID, fileHash, mData);
				bool flag = false;
				if (sendCommandSync(command, COMMAND_RESPONSE_TIMEOUT) == StatusCode.SUCCESS)
				{
					flag = true;
				}
				if (flag)
				{
					updateProgress(0.04f);
					bool fileSent = false;
					byte[] tagByteArray = command.getTagByteArray();
					byte[] array = await sendFileWithProgress(tagByteArray, mFileID, mData);
					if (array != null)
					{
						Message message = MessageParser.GetMessage(array);
						if (message != null && message.isResponse())
						{
							MMSResponse mMSResponse = new MMSResponse(message);
							if (mMSResponse != null && mMSResponse.getOperationStatus() == 0)
							{
								fileSent = true;
							}
						}
					}
					if (fileSent)
					{
						updateProgress(1f);
						sendCallbackOnResult(StatusCode.SUCCESS, null);
					}
					else
					{
						sendCallbackOnResult(StatusCode.ERROR, null);
					}
				}
				else
				{
					sendCallbackOnResult(StatusCode.ERROR, null);
				}
			}
			catch (Exception)
			{
			}
		});
	}

	protected async void prepareAndSendSecureFile()
	{
		if (mData == null || mData.Length == 0)
		{
			return;
		}
		await Task.Run(async delegate
		{
			try
			{
				if (!executeGetChallege(new byte[2] { 223, 0 }))
				{
					sendCallbackOnResult(StatusCode.ERROR, null);
				}
				else
				{
					updateProgress(0.02f);
					byte[] fileHash = SHA256.Create().ComputeHash(mData);
					Command command = MMSCommandBuilder.SendFileCommand(mFileID, fileHash, mData);
					Command command2 = MMSCommandBuilder.SecureWrapperCommand(payloadBytes: command.getByteArray(), keyInfoHexString: "1111", operationsHexString: "0303060208", deviceSerialHexString: mDeviceSerial, tokenHexString: mChallengeToken);
					byte[] byteArray2 = command2.getByteArray();
					if (byteArray2 != null)
					{
						byte[] array = null;
						IResult result = sendCallbackOnCalculateMAC(0, byteArray2);
						if (result.Status == StatusCode.SUCCESS)
						{
							IData data = result.Data;
							if (data != null)
							{
								array = data.ByteArray;
							}
						}
						if (array != null)
						{
							command2.addParam(158, array);
						}
					}
					bool flag = false;
					byte[] tagByteArray = command.getTagByteArray();
					byte[] byteArray3 = command2.getByteArray();
					byte[] array2 = sendAndReceiveSecuredCommand(tagByteArray, byteArray3);
					if (array2 != null)
					{
						Message message = MessageParser.GetMessage(array2);
						if (message != null && message.isResponse())
						{
							MMSResponse mMSResponse = new MMSResponse(message);
							if (mMSResponse != null && mMSResponse.getOperationStatus() == 0)
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						updateProgress(0.04f);
						bool fileSent = false;
						byte[] array3 = await sendFileWithProgress(tagByteArray, mFileID, mData);
						if (array3 != null)
						{
							Message message2 = MessageParser.GetMessage(array3);
							if (message2 != null && message2.isResponse())
							{
								MMSResponse mMSResponse2 = new MMSResponse(message2);
								if (mMSResponse2 != null && mMSResponse2.getOperationStatus() == 0)
								{
									fileSent = true;
								}
							}
						}
						if (fileSent)
						{
							updateProgress(1f);
							sendCallbackOnResult(StatusCode.SUCCESS, null);
						}
						else
						{
							sendCallbackOnResult(StatusCode.ERROR, null);
						}
					}
					else
					{
						sendCallbackOnResult(StatusCode.ERROR, null);
					}
				}
			}
			catch (Exception)
			{
			}
		});
	}

	protected bool executeGetChallege(byte[] data)
	{
		bool result = false;
		Command challengeCommand = MMSCommandBuilder.GetChallengeCommand(data);
		ResponsePayload responsePayload = sendCommandAndReceive(challengeCommand);
		if (responsePayload != null)
		{
			mDeviceSerial = TLVParser.getHexString(responsePayload.getParamValue("82"));
			mChallengeToken = TLVParser.getHexString(responsePayload.getParamValue("83"));
			result = true;
		}
		return result;
	}

	protected string getDeviceSerial()
	{
		byte[] itemsValue = getItemsValue(2, "E208E106E104E102C100");
		string text = "";
		if (itemsValue != null)
		{
			text = TLVParser.getHexString(itemsValue);
		}
		if (text.Length > 7)
		{
			text = text.Substring(0, 7);
		}
		return text;
	}

	protected string getFirmwareVersion()
	{
		byte[] itemsValue = getItemsValue(2, "E108E206E204E202C200");
		string result = "";
		if (itemsValue != null)
		{
			result = Encoding.UTF8.GetString(itemsValue);
		}
		return result;
	}

	protected string getDeviceCapabilities()
	{
		byte[] itemsValue = getItemsValue(2, "E208E106E104E102C200");
		string result = "";
		if (itemsValue != null)
		{
			result = Encoding.UTF8.GetString(itemsValue);
		}
		return result;
	}

	protected string getBoot1Version()
	{
		byte[] itemsValue = getItemsValue(2, "E108E206E104E102C200");
		string result = "";
		if (itemsValue != null)
		{
			result = Encoding.UTF8.GetString(itemsValue);
		}
		return result;
	}

	protected string getBoot0Version()
	{
		byte[] itemsValue = getItemsValue(2, "E108E206E104E202C200");
		string result = "";
		if (itemsValue != null)
		{
			result = Encoding.UTF8.GetString(itemsValue);
		}
		return result;
	}

	protected string getFirmwareHash()
	{
		byte[] itemsValue = getItemsValue(2, "E108E206E604E102C100");
		string result = "";
		if (itemsValue != null)
		{
			result = Encoding.UTF8.GetString(itemsValue);
		}
		return result;
	}

	protected string getTamperStatus()
	{
		byte[] itemsValue = getItemsValue(2, "E308E106E104E202C300");
		string result = "";
		if (itemsValue != null)
		{
			result = TLVParser.getHexString(itemsValue);
		}
		return result;
	}

	protected string getOperationStatus()
	{
		byte[] itemsValue = getItemsValue(2, "E308E106E204E102C100");
		string result = "";
		if (itemsValue != null)
		{
			result = TLVParser.getHexString(itemsValue);
		}
		return result;
	}

	protected string getOfflineDetail()
	{
		byte[] itemsValue = getItemsValue(2, "E308E106E204E102C200");
		string result = "";
		if (itemsValue != null)
		{
			result = TLVParser.getHexString(itemsValue);
		}
		return result;
	}

	protected byte[] getItemsValue(byte functionOID, string oidString)
	{
		return getItemsValue(functionOID, TLVParser.getByteArrayFromHexString(oidString));
	}

	protected byte[] getItemsValue(byte functionOID, byte[] oidTagTree)
	{
		byte[] result = null;
		byte[] itemsData = getItemsData(functionOID, oidTagTree);
		if (itemsData != null)
		{
			List<TLVObject> list = TLVParser.parseTLVByteArray(itemsData);
			if (list != null && list.Count > 0)
			{
				TLVObject tLVObject = list[0];
				if (tLVObject != null && oidTagTree != null)
				{
					int num = oidTagTree.Length;
					if (num >= 2)
					{
						byte[] tlvTagByteArray = new byte[1] { oidTagTree[num - 2] };
						TLVObject tLVObject2 = tLVObject.findByTagByteArray(tlvTagByteArray);
						if (tLVObject2 != null)
						{
							result = tLVObject2.getValueByteArray();
						}
					}
				}
			}
		}
		return result;
	}

	protected byte[] getItemsData(byte functionOID, string oidString)
	{
		return getItemsData(functionOID, TLVParser.getByteArrayFromHexString(oidString));
	}

	protected byte[] getItemsData(byte functionOID, byte[] oidTagTree)
	{
		byte[] result = null;
		Command itemsCommand = MMSCommandBuilder.GetItemsCommand(functionOID, oidTagTree);
		ResponsePayload responsePayload = sendCommandAndReceive(itemsCommand);
		if (responsePayload != null)
		{
			result = responsePayload.getParamValue("89");
		}
		return result;
	}

	protected byte[] setItemsData(byte functionOID, byte[] oidTagTreeAndValue)
	{
		byte[] result = null;
		Command command = MMSCommandBuilder.SetItemsCommand(functionOID, oidTagTreeAndValue);
		ResponsePayload responsePayload = sendCommandAndReceive(command);
		if (responsePayload != null)
		{
			result = responsePayload.getParamValue("89");
		}
		return result;
	}

	protected void sendCommand(Command command)
	{
		Message message = MessageBuilder.BuildMessage();
		message.addMessageInfoForCommand(command.getTagByteArray());
		message.addPayload(command.getByteArray());
		byte[] byteArray = message.getByteArray();
		if (byteArray != null)
		{
			TLVParser.getHexString(byteArray);
			MMXMessage message2 = new MMXMessage(48, byteArray);
			mMMXDevice.sendMessage(message2);
		}
	}

	protected StatusCode sendCommandSync(Command command, int timeout)
	{
		StatusCode result = StatusCode.TIMEOUT;
		mResponseEvent = new AutoResetEvent(initialState: false);
		sendCommand(command);
		if (mResponseEvent.WaitOne(timeout))
		{
			result = ((mResponseOperationStatus != 0) ? StatusCode.ERROR : StatusCode.SUCCESS);
		}
		mResponseEvent = null;
		return result;
	}

	protected ResponsePayload sendCommandAndReceive(Command command)
	{
		ResponsePayload result = null;
		mResponseEvent = new AutoResetEvent(initialState: false);
		sendCommand(command);
		if (mResponseEvent.WaitOne(COMMAND_RESPONSE_TIMEOUT))
		{
			result = mResponsePayload;
		}
		mResponseEvent = null;
		return result;
	}

	public void processResponse(Message message)
	{
		MMSResponse mMSResponse = new MMSResponse(message);
		if (mMSResponse != null && mResponseEvent != null)
		{
			mResponse = mMSResponse;
			mResponseOperationStatus = mMSResponse.getOperationStatus();
			byte[] payload = mMSResponse.getPayload();
			mResponsePayload = MessageParser.GetResponsePayload(payload);
			mResponseEvent.Set();
		}
	}

	protected void notifyLoadFirmwareResult(bool result)
	{
		if (mLoadFirmwareEvent != null)
		{
			mLoadFirmwareResult = result;
			mLoadFirmwareEvent.Set();
		}
	}

	protected void notifyCommitFirmwareResult(bool result)
	{
		if (mCommitFirmwareEvent != null)
		{
			mCommitFirmwareResult = result;
			mCommitFirmwareEvent.Set();
		}
	}

	public void processDataFile(Message message)
	{
		MMSDataFile mMSDataFile = new MMSDataFile(message);
		if (mMSDataFile != null)
		{
			mMSDataFile.getCommandID();
			mMSDataFile.getFileType();
			byte[] payload = mMSDataFile.getPayload();
			updateProgress(1f);
			sendCallbackOnResult(StatusCode.SUCCESS, payload);
		}
	}

	public void processNotification(Message message)
	{
		MMSNotification mMSNotification = new MMSNotification(message);
		if (mMSNotification == null)
		{
			return;
		}
		byte[] notificationID = mMSNotification.getNotificationID();
		byte[] notificationCode = mMSNotification.getNotificationCode();
		if (notificationID[0] != 9)
		{
			return;
		}
		if (notificationID[1] == 5)
		{
			if (notificationCode[2] == 9)
			{
				notifyLoadFirmwareResult(result: true);
			}
			else if (notificationCode[2] == 10)
			{
				notifyCommitFirmwareResult(result: true);
			}
		}
		else if (notificationID[1] == 6)
		{
			if (notificationCode[2] == 9)
			{
				notifyLoadFirmwareResult(result: false);
			}
			else if (notificationCode[2] == 10)
			{
				notifyCommitFirmwareResult(result: false);
			}
		}
	}
}
