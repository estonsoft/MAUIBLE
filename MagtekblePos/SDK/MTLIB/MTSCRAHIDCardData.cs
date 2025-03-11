// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTSCRAHIDCardData
using System;
using System.Text;
using System.Threading;


public class MTSCRAHIDCardData : IMTCardData, IMTCardDataHandler
{
	private const int PID_0X_REPORT_OFFSET_TRACK1_DECODE_STATUS = 0;

	private const int PID_0X_REPORT_OFFSET_TRACK2_DECODE_STATUS = 1;

	private const int PID_0X_REPORT_OFFSET_TRACK3_DECODE_STATUS = 2;

	private const int PID_0X_REPORT_OFFSET_TRACK1_MASKED_DATA_LENGTH = 3;

	private const int PID_0X_REPORT_OFFSET_TRACK2_MASKED_DATA_LENGTH = 4;

	private const int PID_0X_REPORT_OFFSET_TRACK3_MASKED_DATA_LENGTH = 5;

	private const int PID_0X_REPORT_OFFSET_CARD_ENCODE_TYPE = 6;

	private const int PID_0X_REPORT_OFFSET_TRACK1_MASKED_DATA = 7;

	private const int PID_0X_REPORT_OFFSET_TRACK2_MASKED_DATA = 117;

	private const int PID_0X_REPORT_OFFSET_TRACK3_MASKED_DATA = 227;

	private const int PID_0X_REPORT_OFFSET_CARD_STATUS = 337;

	private const int PID_02_REPORT_LENGTH = 337;

	private const int PID_03_REPORT_LENGTH = 338;

	private const int REPORT_OFFSET_TRACK1_DECODE_STATUS = 0;

	private const int REPORT_OFFSET_TRACK2_DECODE_STATUS = 1;

	private const int REPORT_OFFSET_TRACK3_DECODE_STATUS = 2;

	private const int REPORT_OFFSET_TRACK1_ENCRYPTED_DATA_LENGTH = 3;

	private const int REPORT_OFFSET_TRACK2_ENCRYPTED_DATA_LENGTH = 4;

	private const int REPORT_OFFSET_TRACK3_ENCRYPTED_DATA_LENGTH = 5;

	private const int REPORT_OFFSET_CARD_ENCODE_TYPE = 6;

	private const int REPORT_OFFSET_TRACK1_ENCRYPTED_DATA = 7;

	private const int REPORT_OFFSET_TRACK2_ENCRYPTED_DATA = 119;

	private const int REPORT_OFFSET_TRACK3_ENCRYPTED_DATA = 231;

	private const int REPORT_OFFSET_CARD_STATUS = 343;

	private const int REPORT_OFFSET_MAGNEPRINT_STATUS = 344;

	private const int REPORT_OFFSET_MAGNEPRINT_DATA_LENGTH = 348;

	private const int REPORT_OFFSET_MAGNEPRINT_DATA = 349;

	private const int REPORT_OFFSET_DEVICE_SERIAL_NUMBER = 477;

	private const int REPORT_OFFSET_READER_ENCRYPTION_STATUS = 493;

	private const int REPORT_OFFSET_DUKPT_SERIAL_NUMBER_COUNTER = 495;

	private const int REPORT_OFFSET_TRACK1_MASKED_DATA_LENGTH = 505;

	private const int REPORT_OFFSET_TRACK2_MASKED_DATA_LENGTH = 506;

	private const int REPORT_OFFSET_TRACK3_MASKED_DATA_LENGTH = 507;

	private const int REPORT_OFFSET_TRACK1_MASKED_DATA = 508;

	private const int REPORT_OFFSET_TRACK2_MASKED_DATA = 620;

	private const int REPORT_OFFSET_TRACK3_MASKED_DATA = 732;

	private const int REPORT_OFFSET_ENCRYPTION_SESSION_ID = 844;

	private const int REPORT_OFFSET_TRACK1_ABSOLUTE_DATA_LENGTH = 852;

	private const int REPORT_OFFSET_TRACK2_ABSOLUTE_DATA_LENGTH = 853;

	private const int REPORT_OFFSET_TRACK3_ABSOLUTE_DATA_LENGTH = 854;

	private const int REPORT_OFFSET_MAGNEPRINT_ABSOLUTE_DATA_LENGTH = 855;

	private const int REPORT_OFFSET_ENCRYPTION_COUNTER = 856;

	private const int REPORT_OFFSET_MAGNESAFE_VERSION_NUMBER = 859;

	private const int REPORT_OFFSET_SHA1_HASHED_TRACK2_DATA = 867;

	private const int REPORT_OFFSET_REPORT_VERSION = 887;

	private const int REPORT_OFFSET_SHA256_HASHED_TRACK2_DATA = 888;

	private const int REPORT_OFFSET_MAGNEPRINT_KSN = 920;

	private const int REPORT_OFFSET_BATTERY_LEVEL = 930;

	private const int REPORT_LENGTH_MAGNEPRINT_STATUS = 4;

	private const int REPORT_LENGTH_DEVICE_SERIAL_NUMBER = 15;

	private const int REPORT_LENGTH_ENCRYPTION_SESSION_ID = 8;

	private const int REPORT_LENGTH_DUKPT_SERIAL_NUMBER_COUNTER = 10;

	private int m_threshold;

	private byte[] m_hidData;

	private ReaderWriterLockSlim m_dataLock;

	public MTSCRAHIDCardData()
	{
		m_dataLock = new ReaderWriterLockSlim();
		clearData();
	}

	~MTSCRAHIDCardData()
	{
	}

	public void setSOM(byte[] somBytes)
	{
	}

	public void setEOM(byte[] eomBytes)
	{
	}

	public void setFS(byte fsByte)
	{
	}

	public void clearData()
	{
		m_dataLock.EnterReadLock();
		m_hidData = null;
		m_dataLock.ExitReadLock();
	}

	public void setDataThreshold(int nBytes)
	{
		m_threshold = nBytes;
	}

	public bool isDataReady()
	{
		m_dataLock.EnterReadLock();
		if (m_hidData != null && m_hidData.Length >= m_threshold)
		{
			m_dataLock.ExitReadLock();
			return true;
		}
		m_dataLock.ExitReadLock();
		return false;
	}

	public void setData(byte[] data)
	{
		m_dataLock.EnterReadLock();
		_ = m_hidData;
		m_dataLock.ExitReadLock();
		try
		{
			m_dataLock.EnterWriteLock();
			if (m_hidData == null)
			{
				m_hidData = data;
			}
			else
			{
				byte[] array = new byte[m_hidData.Length + data.Length];
				Array.Copy(m_hidData, 0, array, 0, m_hidData.Length);
				Array.Copy(data, 0, array, m_hidData.Length, data.Length);
				m_hidData = array;
			}
		}
		catch (Exception)
		{
		}
		m_dataLock.ExitWriteLock();
	}

	public void handleData(byte[] data)
	{
		setData(data);
	}

	public byte[] getData()
	{
		return m_hidData;
	}

	public void clearBuffers()
	{
		clearData();
	}

	protected byte[] getData(int offsetLength, int offsetStart)
	{
		byte[] array = null;
		try
		{
			if (m_hidData != null)
			{
				int num = 0;
				if (m_hidData.Length >= offsetLength)
				{
					num = m_hidData[offsetLength];
				}
				if (num > 0)
				{
					array = new byte[num];
					Array.Copy(m_hidData, offsetStart, array, 0, num);
				}
			}
		}
		catch (Exception)
		{
		}
		return array;
	}

	protected string getDataAsHexString(int offsetLength, int offsetStart)
	{
		string result = "";
		try
		{
			byte[] data = getData(offsetLength, offsetStart);
			if (data != null && data.Length != 0)
			{
				result = MTParser.getHexString(data);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	protected string getDataAsString(int offsetLength, int offsetStart)
	{
		string result = "";
		try
		{
			byte[] data = getData(offsetLength, offsetStart);
			if (data != null && data.Length != 0)
			{
				result = Encoding.UTF8.GetString(data, 0, data.Length);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	protected byte[] getDataWithLength(int offsetStart, int lenData)
	{
		byte[] array = null;
		try
		{
			if (m_hidData != null && lenData > 0)
			{
				array = new byte[lenData];
				Array.Copy(m_hidData, offsetStart, array, 0, lenData);
			}
		}
		catch (Exception)
		{
		}
		return array;
	}

	protected string getDataWithLengthAsHexString(int offsetStart, int lenData)
	{
		string result = "";
		try
		{
			byte[] dataWithLength = getDataWithLength(offsetStart, lenData);
			if (dataWithLength != null && dataWithLength.Length != 0)
			{
				result = MTParser.getHexString(dataWithLength);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	protected string getDataWithLengthAsString(int offsetStart, int lenData)
	{
		string result = "";
		try
		{
			byte[] dataWithLength = getDataWithLength(offsetStart, lenData);
			if (dataWithLength != null && dataWithLength.Length != 0 && dataWithLength[0] != 0)
			{
				result = Encoding.UTF8.GetString(dataWithLength, 0, dataWithLength.Length);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getMaskedTracks()
	{
		return getTrack1Masked() + getTrack2Masked() + getTrack3Masked();
	}

	public string getTrack1()
	{
		if (m_hidData != null && m_hidData.Length > 505)
		{
			return getDataAsHexString(3, 7);
		}
		return getDataAsString(3, 7);
	}

	public string getTrack2()
	{
		if (m_hidData != null && m_hidData.Length > 506)
		{
			return getDataAsHexString(4, 119);
		}
		return getDataAsString(4, 117);
	}

	public string getTrack3()
	{
		if (m_hidData != null && m_hidData.Length > 507)
		{
			return getDataAsHexString(5, 231);
		}
		return getDataAsString(5, 227);
	}

	public string getTrack1Masked()
	{
		return getDataAsString(505, 508);
	}

	public string getTrack2Masked()
	{
		return getDataAsString(506, 620);
	}

	public string getTrack3Masked()
	{
		return getDataAsString(507, 732);
	}

	public string getMagnePrint()
	{
		return getDataAsHexString(348, 349);
	}

	public string getMagnePrintStatus()
	{
		return getDataWithLengthAsHexString(344, 4);
	}

	public string getDeviceSerial()
	{
		return getDataWithLengthAsString(477, 15);
	}

	public string getSessionID()
	{
		return getDataWithLengthAsHexString(844, 8);
	}

	public string getKSN()
	{
		return getDataWithLengthAsHexString(495, 10);
	}

	public string getDeviceName()
	{
		return "";
	}

	public long getBatteryLevel()
	{
		try
		{
			int num = 930;
			if (m_hidData != null && num < m_hidData.Length)
			{
				return m_hidData[num];
			}
		}
		catch (Exception)
		{
		}
		return MTDeviceConstants.BATTERY_LEVEL_NA;
	}

	public long getSwipeCount()
	{
		return MTDeviceConstants.SWIPE_COUNT_NA;
	}

	public string getCapMagnePrint()
	{
		return "";
	}

	public string getCapMagnePrintEncryption()
	{
		return "";
	}

	public string getCapMagneSafe20Encryption()
	{
		return "";
	}

	public string getCapMagStripeEncryption()
	{
		return "";
	}

	public string getCapMSR()
	{
		return "";
	}

	public string getCapTracks()
	{
		return "";
	}

	public long getCardDataCRC()
	{
		return 0L;
	}

	public string getCardExpDate()
	{
		string result = "";
		string additionalInfo = getAdditionalInfo();
		if (additionalInfo.Length > 6)
		{
			result = additionalInfo.Substring(0, 4);
		}
		return result;
	}

	public string getCardIIN()
	{
		string result = "";
		string maskedPAN = getMaskedPAN();
		if (maskedPAN.Length >= 6)
		{
			result = maskedPAN.Substring(0, 6);
		}
		return result;
	}

	public string getCardLast4()
	{
		string result = "";
		string maskedPAN = getMaskedPAN();
		if (maskedPAN.Length >= 4)
		{
			result = maskedPAN.Substring(maskedPAN.Length - 4);
		}
		return result;
	}

	public string getCardName()
	{
		return getNameFromMaskedTrack1();
	}

	public string getCardPAN()
	{
		return getMaskedPAN();
	}

	public int getCardPANLength()
	{
		int result = 0;
		string maskedPAN = getMaskedPAN();
		if (maskedPAN.Length >= 3)
		{
			result = maskedPAN.Length;
		}
		return result;
	}

	public string getCardServiceCode()
	{
		string result = "";
		string additionalInfo = getAdditionalInfo();
		if (additionalInfo.Length > 6)
		{
			result = additionalInfo.Substring(4);
		}
		return result;
	}

	public string getCardStatus()
	{
		return getDataWithLengthAsHexString(343, 1);
	}

	public string getCardEncodeType()
	{
		return getDataWithLengthAsHexString(6, 1);
	}

	public int getDataFieldCount()
	{
		return 0;
	}

	public string getHashCode()
	{
		return "";
	}

	public string getDeviceConfig(string configType)
	{
		return "";
	}

	public string getEncryptionStatus()
	{
		return getDataWithLengthAsHexString(493, 2);
	}

	public string getFirmware()
	{
		return "";
	}

	public string getMagTekDeviceSerial()
	{
		return "";
	}

	public string getResponseType()
	{
		return "";
	}

	public string getTagValue(string tag, string data)
	{
		return "";
	}

	public string getTLVVersion()
	{
		return "";
	}

	public string getTrackDecodeStatus()
	{
		return getDataWithLengthAsHexString(0, 3);
	}

	private string getMaskedPAN()
	{
		string text = "";
		text = getPANFromMaskedTrack2();
		if (text.Trim().Length == 0)
		{
			text = getPANFromMaskedTrack1();
		}
		return text;
	}

	private string getNameFromMaskedTrack1()
	{
		string result = "";
		string text = "";
		try
		{
			text = getTrack1Masked();
			if (text.Length > 0)
			{
				int num = text.IndexOf("^");
				int num2 = -1;
				if (num != -1)
				{
					num2 = text.IndexOf("^", num + 1);
				}
				if (num != -1 && num2 != -1)
				{
					int num3 = num + 1;
					int length = num2 - num3;
					result = text.Substring(num3, length);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	private string getPANFromMaskedTrack1()
	{
		string result = "";
		string text = "";
		try
		{
			text = getTrack1Masked();
			if (text.Length > 0)
			{
				int num = text.IndexOf("%");
				int num2 = text.IndexOf("^");
				if (num != -1 && num2 != -1)
				{
					int num3 = num + 2;
					int length = num2 - num3;
					result = text.Substring(num3, length);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	private string getPANFromMaskedTrack2()
	{
		string result = "";
		string text = "";
		try
		{
			text = getTrack2Masked();
			if (text.Length > 0)
			{
				int num = text.IndexOf(";");
				int num2 = text.IndexOf("=");
				if (num != -1 && num2 != -1)
				{
					int num3 = num + 1;
					int length = num2 - num3;
					result = text.Substring(num3, length);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	private string getAdditionalInfo()
	{
		string text = "";
		text = getAdditionalInfoFromMaskedTrack2();
		if (text.Trim().Length == 0)
		{
			text = getAdditionalInfoFromMaskedTrack1();
		}
		return text;
	}

	private string getAdditionalInfoFromMaskedTrack1()
	{
		string result = "";
		string text = "";
		try
		{
			text = getTrack1Masked();
			if (text.Length > 0)
			{
				string[] separator = new string[1] { "^" };
				string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if (array != null && array.Length > 2 && array[1].Length > 7)
				{
					result = array[2].Substring(0, 7);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	private string getAdditionalInfoFromMaskedTrack2()
	{
		string result = "";
		string text = "";
		try
		{
			text = getTrack2Masked();
			if (text.Length > 0)
			{
				int num = text.IndexOf("=");
				if (num != -1)
				{
					result = text.Substring(num + 1, 7);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	private string DFM_vFixCheckDigit(byte[] ryubInPAN, int iubPANLength, byte iubFixPos)
	{
		byte b = (byte)iubPANLength;
		byte b2 = (byte)(ryubInPAN[b - 1] - Convert.ToByte('0'));
		b -= 2;
		while (true)
		{
			byte b3 = (byte)((byte)(ryubInPAN[b] - Convert.ToByte('0')) * 2);
			if (b3 > 9)
			{
				b2++;
				b3 -= 10;
			}
			b2 += b3;
			if (b == 0)
			{
				break;
			}
			b--;
			b2 += (byte)(ryubInPAN[b] - Convert.ToByte('0'));
			if (b == 0)
			{
				break;
			}
			b--;
		}
		b = (byte)iubPANLength;
		b2 %= 10;
		if (b2 != 0)
		{
			b2 = (byte)(10 - b2);
			if ((b - 1 - iubFixPos) % 2 > 0)
			{
				b2 = ((b2 % 2 <= 0) ? ((byte)(b2 / 2)) : ((byte)((b2 + 9) / 2)));
			}
		}
		if (b2 != 0)
		{
			b2 += Convert.ToByte('0');
			ryubInPAN[iubFixPos] = b2;
		}
		return Encoding.UTF8.GetString(ryubInPAN, 0, ryubInPAN.Length);
	}

	public string getTLVPayload()
	{
		return MTPayloadBuilder.buildTLVPayload(this);
	}

	public string getMessageID()
	{
		return "";
	}

	public bool isQwantumCard()
	{
		return false;
	}

	public bool isQwantumBuffer()
	{
		return false;
	}

	public bool isCustomerMessage()
	{
		return false;
	}

	public string getMSRDUKPTKeySerialNumber()
	{
		return "";
	}

	public string getMSRDUKPTKeyInfo()
	{
		return "";
	}

	public string getMPDUKPTKeySerialNumber()
	{
		return "";
	}

	public string getMPDUKPTKeyInfo()
	{
		return "";
	}

	public string getTokenDUKPTKeySerialNumber()
	{
		return "";
	}

	public string getTokenDUKPTKeyInfo()
	{
		return "";
	}

	public string getQwantumStatus()
	{
		return "";
	}

	public string getQwantumToken()
	{
		return "";
	}

	public string getQwantumCardID()
	{
		return "";
	}

	public string getEncryptedQwantumDataBuffer()
	{
		return "";
	}

	public string getMACDUKPTKeyInfo()
	{
		return "";
	}

	public string getMACMessageLength()
	{
		return "";
	}

	public string getMAC()
	{
		return "";
	}

	public string getCustomerMessageCode()
	{
		return "";
	}

	public string getCustomerMessageText()
	{
		return "";
	}
}
