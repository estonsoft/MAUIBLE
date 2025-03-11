// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.MTPPReportParser
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagTek;



internal class MTPPReportParser : IMTReportParser
{
	internal MTPPSCRA m_IPAD;

	internal CARD_DATA_PARSER cardDataParser = new CARD_DATA_PARSER();

	internal PIN_DATA_PARSER pinDataParser = new PIN_DATA_PARSER();

	internal byte opStatus;

	private byte ackStatus;

	private byte[] CmdAckdata = new byte[2];

	internal byte _processSpecialCommand;

	private int _lastCommand;

	private int _lastCardCommand;

	private byte[] _bigBlockData;

	private Dictionary<int, byte[]> _bigBlockDataList = new Dictionary<int, byte[]>();

	private int _bigBlockDataLength;

	private int _bigBlockDataBufferLength;

	private int _bigBlockDataType;

	private int _bigBlockSubId;

	private int _bigBlockDataStatus;

	private string pinDataSeparator = ",";

	private Dictionary<int, Action<byte[]>> mReport;

	public string CardRequestFieldSeparator = ",";

	private Dictionary<int, Action<byte[]>> OnDeviceReport
	{
		get
		{
			if (mReport == null)
			{
				mReport = createDeviceReport();
			}
			return mReport;
		}
	}

	public event OnDataReadyCompleteEvent onDataReady;

	public MTPPReportParser(MTPPSCRA ipad)
	{
		m_IPAD = ipad;
	}

	private void consolePrint(string message)
	{
	}

	internal void setAckStatus(byte newStatus)
	{
		opStatus = 0;
		ackStatus = newStatus;
	}

	internal byte getAckStatus()
	{
		return ackStatus;
	}

	internal void setOpStatus(byte newOpStatus)
	{
		ackStatus = 0;
		opStatus = newOpStatus;
	}

	public byte getStatusCode()
	{
		if (ackStatus != 0)
		{
			return ackStatus;
		}
		return opStatus;
	}

	private void setLastCommand(int command)
	{
		_lastCommand = command;
		if (command == 166 || command == 164 || command == 162 || command == 3 || command == 17 || command == 171)
		{
			_lastCardCommand = command;
		}
	}

	protected int sendSpecialCommand(byte[] data)
	{
		setLastCommand(data[1]);
		return m_IPAD.sendSpecialCommand(data);
	}

	public void parseReport(object sender, byte[] pData)
	{
		if (this.onDataReady != null)
		{
			this.onDataReady(pData);
		}
		int num = pData[0];
		if (num == 1)
		{
			setAckStatus(pData[1]);
		}
		if (num == 33 || num == 34 || num == 36 || num == 37 || num == 39 || num == 40 || num == 46)
		{
			setOpStatus(pData[1]);
		}
		if (_processSpecialCommand != 0)
		{
			if (num == 1 && _processSpecialCommand != 88)
			{
				if (_processSpecialCommand == pData[2])
				{
					_processSpecialCommand = byte.MaxValue;
				}
				else if (_processSpecialCommand == byte.MaxValue)
				{
					_processSpecialCommand = 0;
				}
				else
				{
					_processSpecialCommand = 0;
				}
				setLastCommand(pData[2]);
			}
			if (_processSpecialCommand != 0)
			{
				_processSpecialCommand = 0;
				return;
			}
		}
		if (OnDeviceReport.ContainsKey(pData[0]))
		{
			if (pData.Length < 64)
			{
				byte[] array = new byte[64];
				Array.Copy(pData, array, pData.Length);
				OnDeviceReport[pData[0]](array);
			}
			else
			{
				OnDeviceReport[pData[0]](pData);
			}
		}
	}

	private Dictionary<int, Action<byte[]>> createDeviceReport()
	{
		return new Dictionary<int, Action<byte[]>>
		{
			{ 1, Report_0x01 },
			{ 32, Report_0x20 },
			{ 33, Report_0x21 },
			{ 34, Report_0x22 },
			{ 35, Report_0x23 },
			{ 36, Report_0x24 },
			{ 37, Report_0x25 },
			{ 38, Report_0x26 },
			{ 39, Report_0x27 },
			{ 40, Report_0x28 },
			{ 41, Report_0x29 },
			{ 42, Report_0x2A },
			{ 44, Report_0x2C },
			{ 45, Report_0x2D },
			{ 46, Report_0x2E },
			{ 47, Report_0x2F },
			{ 48, Report_0x30 },
			{ 224, Report_0xE0 }
		};
	}

	private void Report_0x01(byte[] pData)
	{
		if (_processSpecialCommand == pData[2])
		{
			_processSpecialCommand = byte.MaxValue;
		}
		else if (_processSpecialCommand == byte.MaxValue)
		{
			_processSpecialCommand = 0;
		}
		else
		{
			_processSpecialCommand = 0;
		}
		if (pData[2] == 5)
		{
			_ = pData[1];
		}
		setLastCommand(pData[2]);
	}

	private void Report_0xE0(byte[] pData)
	{
		m_IPAD.sendSpecialCommand(new byte[4]
		{
			2,
			224,
			pData[1],
			0
		}, stopTrackingOnDataReady: false);
	}

	private void Report_0x20(byte[] data)
	{
		if (_lastCommand == 10 || _lastCommand == 3 || _lastCommand == 17)
		{
			if (cardDataParser.dataAvailable && (data[2] & 8) == 0)
			{
				try
				{
					onCardRequestCompleteEvent(cardDataParser._cardData, cardDataParser._cardDataLength);
				}
				catch (Exception ex)
				{
					consolePrint(ex.Message);
				}
			}
			cardDataParser.dataAvailable = (data[2] & 8) != 0;
		}
		if (data.Length >= 7 && m_IPAD.EventDeviceStateUpdated != null)
		{
			DEV_STATE_STAT_PARSER dEV_STATE_STAT_PARSER = new DEV_STATE_STAT_PARSER();
			dEV_STATE_STAT_PARSER.parse(data, data.Length);
			m_IPAD.EventDeviceStateUpdated(dEV_STATE_STAT_PARSER.devState);
		}
	}

	private void Report_0x21(byte[] data)
	{
		if (m_IPAD.EventOnUserDataEntry != null)
		{
			USER_ENTRY_DATA userData = default(USER_ENTRY_DATA);
			userData.MSRKsn = PARSER.toHexString(data, 2, 10);
			userData.EDB = PARSER.toHexString(data, 12, 8);
			userData.OpStatus = data[1];
			m_IPAD.EventOnUserDataEntry(userData);
		}
	}

	private void Report_0x22(byte[] pData)
	{
		setOpStatus(pData[1]);
		Array.Copy(pData, 1, cardDataParser._cardStatus, 0, 3);
		if (_lastCardCommand == 166)
		{
			if (opStatus == 0)
			{
				requestATRData();
			}
			else
			{
				onATRRequestComplete(getStatusCode(), null, 0);
			}
		}
		else if (_lastCardCommand == 164 || _lastCardCommand == 162)
		{
			_ = _lastCardCommand;
			_ = 162;
			if (getStatusCode() == 0)
			{
				requestTransactionData();
				if (getAckStatus() == 128)
				{
					_lastCardCommand = 162;
				}
			}
		}
		else if (_lastCardCommand == 3 || _lastCardCommand == 17)
		{
			if (getStatusCode() == 0)
			{
				requestMSRData();
			}
			else
			{
				onCardRequestCompleteEvent(null, 0);
			}
		}
	}

	private void Report_0x23(byte[] data)
	{
		int num = data[3] + 4;
		Array.Copy(data, 0, cardDataParser._cardData, cardDataParser._cardDataLength, num);
		cardDataParser._cardDataLength += num;
	}

	private void Report_0x24(byte[] data)
	{
		pinDataParser.parse(data, data.Length);
		if (m_IPAD.EventPinRequest != null)
		{
			m_IPAD.EventPinRequest(pinDataParser.lastPinData.ToSeparatedString(pinDataSeparator));
		}
	}

	private void Report_0x25(byte[] data)
	{
		if (m_IPAD.EventKeyInput != null)
		{
			m_IPAD.EventKeyInput(data[1], data[2]);
		}
	}

	private void Report_0x26(byte[] data)
	{
		try
		{
			if (m_IPAD.EventDeviceMessageSelectMenuItem != null)
			{
				m_IPAD.EventDeviceMessageSelectMenuItem(data[1], data[2], data[3], data[4]);
			}
		}
		catch (Exception)
		{
		}
	}

	private void Report_0x27(byte[] data)
	{
		if (m_IPAD.EventDisplayRequest != null)
		{
			m_IPAD.EventDisplayRequest(data[1]);
		}
	}

	private void Report_0x28(byte[] data)
	{
		_ = data[1];
		if (data[1] == 0 && data[3] + (data[4] << 8) > 0)
		{
			Task.Run(delegate
			{
				int num = requestSignatureData();
				if (num != 0)
				{
					m_IPAD.EventOnError(num);
				}
			});
		}
		else if (data[1] == 2)
		{
			Task.Run(delegate
			{
				if (requestSignatureData() != 0 || getAckStatus() != 0)
				{
					onSignatureDataArriveComplete(data[1], null, 0);
				}
			});
		}
		else
		{
			onSignatureDataArriveComplete(data[1], null, 0);
		}
	}

	private void Report_0x29(byte[] pData)
	{
		if (_defProcessBigBlockData(pData, pData.Length) == 0)
		{
			switch (pData[1])
			{
			case 0:
				onSignatureDataArriveComplete(opStatus, _bigBlockData, _bigBlockDataLength);
				break;
			case 2:
				onDeviceCertComplete(_bigBlockData, _bigBlockDataLength);
				break;
			case 24:
				onPerformTestAPDU(_bigBlockData, _bigBlockDataLength);
				break;
			case 32:
				onPayPassTLV(_bigBlockData, _bigBlockDataLength);
				break;
			case 33:
				onPayPassKernelMessageType(_bigBlockData, _bigBlockDataLength);
				break;
			case 161:
				onRequestSetEMVTagsComplete(_bigBlockData, _bigBlockDataLength);
				break;
			case 164:
				onRequestAcquirerResponseWithData(_bigBlockData, _bigBlockDataLength);
				break;
			case 165:
				onSetCAPublicKeyComplete(_bigBlockData, _bigBlockDataLength);
				break;
			case 166:
				onATRRequestComplete(getStatusCode(), _bigBlockData, _bigBlockDataLength);
				break;
			case 167:
				onSendICCAPDUComplete(_bigBlockData, _bigBlockDataLength);
				break;
			case 171:
				onSendAcquirerResponseComplete(_bigBlockData, _bigBlockDataLength);
				break;
			}
		}
	}

	private void Report_0x2A(byte[] pData)
	{
		setAckStatus(pData[1]);
		if (m_IPAD.EventOnProgressUpdated != null)
		{
			Dictionary<int, OnProgressUpdateEvent> dictionary = new Dictionary<int, OnProgressUpdateEvent>
			{
				{ 23, m_IPAD.EventOnProgressUpdated },
				{ 12, m_IPAD.EventOnProgressUpdated }
			};
			if (dictionary.ContainsKey(pData[2]))
			{
				dictionary[pData[2]](getStatusCode(), pData[2], 2.0);
			}
			else
			{
				m_IPAD.EventOnProgressUpdated(getStatusCode(), pData[2], 2.0);
			}
		}
	}

	private void Report_0x2C(byte[] pData)
	{
		onEMVCardHolderStatusUpdated(pData, pData.Length);
	}

	private void Report_0x2D(byte[] pData)
	{
		onBLEModuleControlData(pData, pData.Length);
	}

	private void Report_0x2E(byte[] data)
	{
		if (m_IPAD.EventClearTextUserDataEntry == null)
		{
			return;
		}
		CLEAR_TEXT_USER_ENTRY_DATA pClearTextUserEntryData = default(CLEAR_TEXT_USER_ENTRY_DATA);
		pClearTextUserEntryData.OpStatus = data[1];
		if (data.Length >= 13)
		{
			pClearTextUserEntryData.UserDataMode = data[2];
			pClearTextUserEntryData.DataLen = data[3];
			byte[] array = new byte[pClearTextUserEntryData.DataLen];
			if (pClearTextUserEntryData.DataLen > 0)
			{
				Array.Copy(data, 4, array, 0, pClearTextUserEntryData.DataLen);
			}
			pClearTextUserEntryData.Data = Encoding.UTF8.GetString(array, 0, array.Length);
		}
		else
		{
			pClearTextUserEntryData.UserDataMode = 0;
			pClearTextUserEntryData.DataLen = 0;
			pClearTextUserEntryData.Data = "";
		}
		m_IPAD.EventClearTextUserDataEntry(pClearTextUserEntryData);
	}

	private void Report_0x2F(byte[] pData)
	{
		try
		{
			if (MTPPSCRA.GetDeviceInfo)
			{
				Task deviceCapabilityInfo = m_IPAD.GetDeviceCapabilityInfo();
				deviceCapabilityInfo.ConfigureAwait(continueOnCapturedContext: false);
				deviceCapabilityInfo.Wait(5000);
			}
			m_IPAD.EventDeviceMessageSessionStarted(pData[1], pData.Skip(2).Take(2).ToArray());
		}
		catch
		{
		}
	}

	private void Report_0x30(byte[] pData)
	{
		try
		{
			byte[] amount = utils.SubArray(pData, 3, 6);
			byte[] tax = utils.SubArray(pData, 9, 6);
			byte[] taxRate = utils.SubArray(pData, 15, 3);
			byte[] tipOrCashback = utils.SubArray(pData, 18, 6);
			m_IPAD.EventDeviceMessageTipOrCashback(pData[1], pData[2], amount, tax, taxRate, tipOrCashback, pData[24]);
		}
		catch
		{
		}
	}

	private int requestATRData()
	{
		byte[] buffer;
		return requestATRData(out buffer);
	}

	private int requestATRData(out byte[] buffer, bool waitForComplete = false)
	{
		buffer = null;
		byte[] data = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			163,
			0
		};
		int result = 255;
		if (!waitForComplete)
		{
			result = sendSpecialCommand(data);
		}
		return result;
	}

	private int requestTransactionData()
	{
		byte[] buffer;
		return requestEMVTransactionData(out buffer, waitForComplete: false);
	}

	private int requestEMVTransactionData(out byte[] buffer, bool waitForComplete)
	{
		buffer = null;
		byte[] data = new byte[6]
		{
			MTPPSCRAConstants.OPERATION_SET,
			171,
			0,
			0,
			0,
			0
		};
		int result = 255;
		if (!waitForComplete)
		{
			result = sendSpecialCommand(data);
		}
		return result;
	}

	private int requestMSRData()
	{
		byte[] buffer;
		return requestMSRData(out buffer, waitForComplete: false);
	}

	private int requestMSRData(out byte[] buffer, bool waitForComplete)
	{
		buffer = null;
		byte[] data = new byte[3]
		{
			MTPPSCRAConstants.OPERATION_SET,
			10,
			0
		};
		int result = 255;
		cardDataParser.reset();
		if (!waitForComplete)
		{
			result = sendSpecialCommand(data);
		}
		return result;
	}

	private int requestSignatureData()
	{
		byte[] signatureDataBuffer;
		return getSignature(out signatureDataBuffer, waitForComplete: false);
	}

	private int getSignature(out byte[] signatureDataBuffer, bool waitForComplete)
	{
		signatureDataBuffer = null;
		byte[] data = new byte[2]
		{
			MTPPSCRAConstants.OPERATION_SET,
			MTPPSCRAConstants.COMMAND_GET_USER_SIGNATURE
		};
		int result = 255;
		if (!waitForComplete)
		{
			setLastCommand(MTPPSCRAConstants.COMMAND_GET_USER_SIGNATURE);
			result = m_IPAD.doCommand(data);
		}
		return result;
	}

	private int _defProcessBigBlockData(byte[] dataBuffer, int dataLength)
	{
		int num = dataBuffer[2];
		int result = -1;
		switch (num)
		{
		case 0:
		{
			if (_bigBlockData != null)
			{
				_bigBlockData = null;
			}
			_bigBlockDataBufferLength = 0;
			int num2 = (dataBuffer[5] << 8) + dataBuffer[4];
			if (num2 > 0)
			{
				_bigBlockData = new byte[num2];
				_bigBlockDataBufferLength = num2;
			}
			_bigBlockDataType = dataBuffer[1];
			_bigBlockDataStatus = dataBuffer[3];
			_bigBlockDataLength = 0;
			result = num2;
			break;
		}
		case 99:
			result = 0;
			break;
		}
		if (num != 99)
		{
			int count = dataBuffer[3];
			if (_bigBlockDataList != null)
			{
				if (num > 0)
				{
					_bigBlockDataList[num] = dataBuffer.Skip(4).Take(count).ToArray();
				}
				if (_bigBlockDataList.Sum((KeyValuePair<int, byte[]> x) => x.Value.Length) == _bigBlockDataBufferLength && _bigBlockDataList.Count == _bigBlockDataList.Max((KeyValuePair<int, byte[]> x) => x.Key))
				{
					IEnumerable<byte> enumerable = new List<byte>();
					foreach (KeyValuePair<int, byte[]> item in _bigBlockDataList.OrderBy((KeyValuePair<int, byte[]> x) => x.Key).ToList())
					{
						enumerable = enumerable.Concat(item.Value);
					}
					_bigBlockData = enumerable.ToArray();
					_bigBlockDataList.Clear();
				}
			}
			if (num != 0)
			{
				result = num;
			}
		}
		_bigBlockSubId = num;
		return result;
	}

	private void onCardRequestCompleteEvent(byte[] data, int len)
	{
		if (data == null)
		{
			cardDataParser.lastCardData.clear();
			cardDataParser.lastCardData.CardOperationStatus = getStatusCode();
		}
		else
		{
			cardDataParser.parse(data, len);
			cardDataParser.lastCardData.CardOperationStatus = getStatusCode();
		}
		if (m_IPAD.EventCardRequest != null)
		{
			m_IPAD.EventCardRequest(cardDataParser.lastCardData.ToSeparatedString(CardRequestFieldSeparator));
		}
	}

	private void onSignatureDataArriveComplete(byte opStatus, byte[] data, int dataLen)
	{
		if (m_IPAD.EventOnSignatureArrived != null)
		{
			m_IPAD.EventOnSignatureArrived(opStatus, data);
		}
	}

	private void onDeviceCertComplete(byte[] data, int dataLen)
	{
	}

	private void onPerformTestAPDU(byte[] data, int dataLen)
	{
		if (m_IPAD.EventPerformTestAPDU != null)
		{
			m_IPAD.EventPerformTestAPDU(data);
		}
	}

	private void onPayPassTLV(byte[] data, int dataLen)
	{
		if (m_IPAD.EventPayPassKernelMessage != null)
		{
			m_IPAD.EventPayPassKernelMessage(data);
		}
	}

	private void onPayPassKernelMessageType(byte[] data, int dataLen)
	{
		if (m_IPAD.EventPayPassKernelMessageType != null)
		{
			m_IPAD.EventPayPassKernelMessageType(data);
		}
	}

	private void onRequestSetEMVTagsComplete(byte[] data, int dataLen)
	{
		if (m_IPAD.EventEMVTagsCompleted != null)
		{
			m_IPAD.EventEMVTagsCompleted(getAckStatus(), data);
		}
	}

	private void onRequestAcquirerResponseWithData(byte[] data, int dataLen)
	{
		if (m_IPAD.EventEMVDataComplete != null)
		{
			m_IPAD.EventEMVDataComplete(getAckStatus(), data);
		}
	}

	private void onSetCAPublicKeyComplete(byte[] data, int dataLen)
	{
		if (m_IPAD.EventCAPublicKey != null)
		{
			m_IPAD.EventCAPublicKey(getAckStatus(), data);
		}
	}

	private void onATRRequestComplete(byte opStatus, byte[] data, int dataLen)
	{
		if (m_IPAD.EventOnPowerUpICC != null)
		{
			m_IPAD.EventOnPowerUpICC(opStatus, data);
		}
	}

	private void onSendICCAPDUComplete(byte[] data, int dataLen)
	{
		if (m_IPAD.EventAPDUArrived != null)
		{
			m_IPAD.EventAPDUArrived(getStatusCode(), data);
		}
	}

	private void onSendAcquirerResponseComplete(byte[] data, int dataLen)
	{
		if (m_IPAD.EventEMVTransactionCompleted != null)
		{
			m_IPAD.EventEMVTransactionCompleted(getAckStatus(), data);
		}
	}

	private void onEMVCardHolderStatusUpdated(byte[] data, int dataLen)
	{
		if (m_IPAD.EventCardHolderStateChanged != null)
		{
			m_IPAD.EventCardHolderStateChanged(data[1]);
		}
	}

	private void onBLEModuleControlData(byte[] data, int dataLen)
	{
	}
}
