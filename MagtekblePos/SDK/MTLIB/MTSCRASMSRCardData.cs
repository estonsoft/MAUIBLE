// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTSCRASMSRCardData
using System;
using System.Text;
using System.Threading;


public class MTSCRASMSRCardData : IMTCardData, IMTCardDataHandler
{
	private enum FORMATCODE
	{
		FormatNA,
		FormatM001,
		FormatQ001,
		FormatQ002,
		FormatQ003
	}

	private static byte[] SOM_DEFAULT = null;

	private static byte[] EOM_DEFAULT = new byte[1] { 13 };

	private static byte FS_DEFAULT = 124;

	private static int FORMATCODE_INDEX = 0;

	private static string FORMATCODE_M001 = "M001";

	private static string FORMATCODE_Q001 = "Q001";

	private static string FORMATCODE_Q002 = "Q002";

	private static string FORMATCODE_Q003 = "Q003";

	private static int M001_TRACK1_MASKED_DATA_INDEX = 1;

	private static int M001_TRACK2_MASKED_DATA_INDEX = 2;

	private static int M001_TRACK3_MASKED_DATA_INDEX = 3;

	private static int M001_TRACK1_ENCRYPTED_DATA_INDEX = 4;

	private static int M001_TRACK2_ENCRYPTED_DATA_INDEX = 5;

	private static int M001_TRACK3_ENCRYPTED_DATA_INDEX = 6;

	private static int M001_MP_STATUS = 7;

	private static int M001_ENCRYPTED_MP_DATA = 8;

	private static int M001_ENCRYPTED_SESSION_ID = 9;

	private static int M001_MSR_DUKPT_KEY_SERIAL_NUMBER = 10;

	private static int M001_MSR_DUKPT_KEY_INFO = 11;

	private static int M001_MP_DUKPT_KEY_SERIAL_NUMBER = 12;

	private static int M001_MP_DUKPT_KEY_INFO = 13;

	private static int M001_DEVICE_SERIAL_NUMBER = 14;

	private static int M001_MAC_DUKPT_KEY_INFO = 15;

	private static int M001_MAC_MESSAGE_LENGTH = 16;

	private static int M001_MAC = 17;

	private static int Q001_TOKEN_DUKPT_KEY_SERIAL_NUMBER = 1;

	private static int Q001_TOKEN_DUKPT_KEY_INFO = 2;

	private static int Q001_QWANTUM_STATUS = 3;

	private static int Q001_QWANTUM_TOKEN = 4;

	private static int Q001_ENCRYPTED_SESSION_ID = 5;

	private static int Q001_QWANTUM_CARD_ID = 6;

	private static int Q001_DEVICE_SERIAL_NUMBER = 7;

	private static int Q001_MAC_DUKPT_KEY_INFO = 8;

	private static int Q001_MAC_MESSAGE_LENGTH = 9;

	private static int Q001_MAC = 10;

	private static int Q002_TOKEN_DUKPT_KEY_SERIAL_NUMBER = 1;

	private static int Q002_TOKEN_DUKPT_KEY_INFO = 2;

	private static int Q002_ENCRYPTED_SESSION_ID = 3;

	private static int Q002_ENCRYPTED_QWANTUM_DATA_BUFFER = 4;

	private static int Q002_DEVICE_SERIAL_NUMBER = 5;

	private static int Q002_MAC_DUKPT_KEY_INFO = 6;

	private static int Q002_MAC_MESSAGE_LENGTH = 7;

	private static int Q002_MAC = 8;

	private static int Q003_CUSTOMER_MESSAGE_CODE = 1;

	private static int Q003_CUSTOMER_MESSAGE_TEXT = 2;

	private byte[] mSOM;

	private byte[] mEOM;

	private byte mFS;

	private byte[] m_smsrData;

	private string m_smsrString;

	private string[] m_smsrArray;

	private bool m_smsrReady;

	private string m_formatCodeString = "";

	private FORMATCODE m_formatCode;

	private ReaderWriterLockSlim m_dataLock;

	public MTSCRASMSRCardData()
	{
		setSOM(SOM_DEFAULT);
		setEOM(EOM_DEFAULT);
		setFS(FS_DEFAULT);
		m_dataLock = new ReaderWriterLockSlim();
		clearData();
	}

	~MTSCRASMSRCardData()
	{
	}

	private int findKey(byte[] data, byte[] key)
	{
		int result = -1;
		if (data != null && data.Length != 0)
		{
			if (key == null)
			{
				result = 0;
			}
			else
			{
				int num = data.Length;
				int num2 = key.Length;
				for (int i = 0; i < 1 + num - num2; i++)
				{
					bool flag = true;
					for (int j = 0; j < num2; j++)
					{
						if (data[i + j] != key[j])
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						result = i;
						break;
					}
				}
			}
		}
		return result;
	}

	public void setSOM(byte[] somBytes)
	{
		mSOM = somBytes;
	}

	public void setEOM(byte[] eomBytes)
	{
		mEOM = eomBytes;
	}

	public void setFS(byte fsByte)
	{
		mFS = fsByte;
	}

	public void clearData()
	{
		m_dataLock.EnterReadLock();
		m_smsrData = null;
		m_smsrString = "";
		m_smsrArray = null;
		m_smsrReady = false;
		m_formatCodeString = "";
		m_formatCode = FORMATCODE.FormatNA;
		m_dataLock.ExitReadLock();
	}

	public void setDataThreshold(int nBytes)
	{
	}

	public bool isDataReady()
	{
		return m_smsrReady;
	}

	protected void parseSMSRData()
	{
		if (m_smsrData == null)
		{
			return;
		}
		int num = m_smsrData.Length;
		if (num < 3)
		{
			return;
		}
		int num2 = findKey(m_smsrData, mSOM);
		int num3 = findKey(m_smsrData, mEOM);
		if (num2 >= 0 && num3 >= 0 && num3 > num2)
		{
			int num4 = 0;
			if (mSOM != null)
			{
				num4 = mSOM.Length;
			}
			int num5 = num2 + num4;
			int count = num3 - num5;
			m_smsrString = Encoding.UTF8.GetString(m_smsrData, num5, count);
		}
		else
		{
			m_smsrString = Encoding.UTF8.GetString(m_smsrData, 0, num);
		}
		string text = new string(new char[1] { (char)mFS });
		string[] separator = new string[1] { text };
		m_smsrArray = m_smsrString.Split(separator, StringSplitOptions.None);
		if (m_smsrArray.Length != 0)
		{
			m_formatCode = FORMATCODE.FormatNA;
			m_formatCodeString = m_smsrArray[0];
			if (m_formatCodeString.Equals(FORMATCODE_M001, StringComparison.CurrentCultureIgnoreCase))
			{
				m_formatCode = FORMATCODE.FormatM001;
			}
			else if (m_formatCodeString.Equals(FORMATCODE_Q001, StringComparison.CurrentCultureIgnoreCase))
			{
				m_formatCode = FORMATCODE.FormatQ001;
			}
			else if (m_formatCodeString.Equals(FORMATCODE_Q002, StringComparison.CurrentCultureIgnoreCase))
			{
				m_formatCode = FORMATCODE.FormatQ002;
			}
			else if (m_formatCodeString.Equals(FORMATCODE_Q003, StringComparison.CurrentCultureIgnoreCase))
			{
				m_formatCode = FORMATCODE.FormatQ003;
			}
		}
	}

	public void setData(byte[] data)
	{
		m_smsrData = null;
		m_smsrReady = true;
		if (data != null)
		{
			int num = data.Length;
			if (num > 0)
			{
				m_smsrData = new byte[num];
				Array.Copy(data, 0, m_smsrData, 0, num);
				parseSMSRData();
			}
		}
	}

	public void handleData(byte[] data)
	{
		m_dataLock.EnterWriteLock();
		int num = 0;
		if (data != null && data.Length != 0)
		{
			int num2 = data.Length - 7;
			int num3 = data[0];
			if (num2 > 0 && num2 + 6 >= num3)
			{
				int num4 = (data[1] << 8) + data[2];
				_ = data[3];
				_ = data[4];
				num = (data[5] << 8) + data[6];
				if (num4 == 0)
				{
					m_smsrData = new byte[num2];
					Array.Copy(data, 7, m_smsrData, 0, num2);
				}
				else if (m_smsrData != null)
				{
					int num5 = m_smsrData.Length;
					if (num4 == num5)
					{
						byte[] array = new byte[num5 + num2];
						Array.Copy(m_smsrData, 0, array, 0, num5);
						Array.Copy(data, 7, array, num5, num2);
						m_smsrData = array;
					}
				}
			}
		}
		m_dataLock.ExitWriteLock();
		if (num > 0 && m_smsrData != null && m_smsrData.Length >= num)
		{
			m_smsrReady = true;
			parseSMSRData();
		}
	}

	public byte[] getData()
	{
		return m_smsrData;
	}

	public void clearBuffers()
	{
		clearData();
	}

	protected string getFieldValue(int index)
	{
		string result = "";
		try
		{
			if (m_smsrArray.Length > index)
			{
				result = m_smsrArray[index];
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
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_TRACK1_ENCRYPTED_DATA_INDEX);
		}
		return result;
	}

	public string getTrack2()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_TRACK2_ENCRYPTED_DATA_INDEX);
		}
		return result;
	}

	public string getTrack3()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_TRACK3_ENCRYPTED_DATA_INDEX);
		}
		return result;
	}

	public string getTrack1Masked()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_TRACK1_MASKED_DATA_INDEX);
		}
		return result;
	}

	public string getTrack2Masked()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_TRACK2_MASKED_DATA_INDEX);
		}
		return result;
	}

	public string getTrack3Masked()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_TRACK3_MASKED_DATA_INDEX);
		}
		return result;
	}

	public string getMagnePrint()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_ENCRYPTED_MP_DATA);
		}
		return result;
	}

	public string getMagnePrintStatus()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_MP_STATUS);
		}
		return result;
	}

	public string getDeviceSerial()
	{
		string result = "";
		switch (m_formatCode)
		{
		case FORMATCODE.FormatM001:
			result = getFieldValue(M001_DEVICE_SERIAL_NUMBER);
			break;
		case FORMATCODE.FormatQ001:
			result = getFieldValue(Q001_DEVICE_SERIAL_NUMBER);
			break;
		case FORMATCODE.FormatQ002:
			result = getFieldValue(Q002_DEVICE_SERIAL_NUMBER);
			break;
		}
		return result;
	}

	public string getSessionID()
	{
		string result = "";
		switch (m_formatCode)
		{
		case FORMATCODE.FormatM001:
			result = getFieldValue(M001_ENCRYPTED_SESSION_ID);
			break;
		case FORMATCODE.FormatQ001:
			result = getFieldValue(Q001_ENCRYPTED_SESSION_ID);
			break;
		case FORMATCODE.FormatQ002:
			result = getFieldValue(Q002_ENCRYPTED_SESSION_ID);
			break;
		}
		return result;
	}

	public string getKSN()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_MSR_DUKPT_KEY_SERIAL_NUMBER);
		}
		return result;
	}

	public string getDeviceName()
	{
		return "";
	}

	public long getBatteryLevel()
	{
		return MTDeviceConstants.BATTERY_LEVEL_NA;
	}

	public long getSwipeCount()
	{
		return MTDeviceConstants.SWIPE_COUNT_NA;
	}

	public string getCapMagnePrint()
	{
		return "01";
	}

	public string getCapMagnePrintEncryption()
	{
		return "01";
	}

	public string getCapMagneSafe20Encryption()
	{
		return "00";
	}

	public string getCapMagStripeEncryption()
	{
		return "0001";
	}

	public string getCapMSR()
	{
		return "01";
	}

	public string getCapTracks()
	{
		return "07";
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
		return "";
	}

	public string getCardEncodeType()
	{
		return "";
	}

	public int getDataFieldCount()
	{
		int result = 0;
		try
		{
			result = ((m_smsrArray != null) ? m_smsrArray.Length : 0);
		}
		catch (Exception)
		{
		}
		return result;
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
		return "";
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
		string result = "";
		try
		{
			string track = getTrack1();
			string track2 = getTrack2();
			string track3 = getTrack3();
			string text = "00";
			string text2 = "00";
			string text3 = "00";
			if (track.Equals("%E?", StringComparison.CurrentCultureIgnoreCase))
			{
				text = "01";
			}
			else
			{
				track.Equals("", StringComparison.CurrentCultureIgnoreCase);
			}
			if (track2.Equals(";E?", StringComparison.CurrentCultureIgnoreCase))
			{
				text2 = "01";
			}
			else
			{
				track2.Equals("", StringComparison.CurrentCultureIgnoreCase);
			}
			if (track3.Equals("+E?", StringComparison.CurrentCultureIgnoreCase) || track3.Equals(";E?", StringComparison.CurrentCultureIgnoreCase))
			{
				text3 = "01";
			}
			else
			{
				track3.Equals("", StringComparison.CurrentCultureIgnoreCase);
			}
			result = text + text2 + text3;
		}
		catch (Exception)
		{
		}
		return result;
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
		return m_formatCodeString;
	}

	public bool isQwantumCard()
	{
		return m_formatCode == FORMATCODE.FormatQ001;
	}

	public bool isQwantumBuffer()
	{
		return m_formatCode == FORMATCODE.FormatQ002;
	}

	public bool isCustomerMessage()
	{
		return m_formatCode == FORMATCODE.FormatQ003;
	}

	public string getMSRDUKPTKeySerialNumber()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_MSR_DUKPT_KEY_SERIAL_NUMBER);
		}
		return result;
	}

	public string getMSRDUKPTKeyInfo()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_MSR_DUKPT_KEY_INFO);
		}
		return result;
	}

	public string getMPDUKPTKeySerialNumber()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_MP_DUKPT_KEY_SERIAL_NUMBER);
		}
		return result;
	}

	public string getMPDUKPTKeyInfo()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatM001)
		{
			result = getFieldValue(M001_MP_DUKPT_KEY_INFO);
		}
		return result;
	}

	public string getTokenDUKPTKeySerialNumber()
	{
		string result = "";
		switch (m_formatCode)
		{
		case FORMATCODE.FormatQ001:
			result = getFieldValue(Q001_TOKEN_DUKPT_KEY_SERIAL_NUMBER);
			break;
		case FORMATCODE.FormatQ002:
			result = getFieldValue(Q002_TOKEN_DUKPT_KEY_SERIAL_NUMBER);
			break;
		}
		return result;
	}

	public string getTokenDUKPTKeyInfo()
	{
		string result = "";
		switch (m_formatCode)
		{
		case FORMATCODE.FormatQ001:
			result = getFieldValue(Q001_TOKEN_DUKPT_KEY_INFO);
			break;
		case FORMATCODE.FormatQ002:
			result = getFieldValue(Q002_TOKEN_DUKPT_KEY_INFO);
			break;
		}
		return result;
	}

	public string getQwantumStatus()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatQ001)
		{
			result = getFieldValue(Q001_QWANTUM_STATUS);
		}
		return result;
	}

	public string getQwantumToken()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatQ001)
		{
			result = getFieldValue(Q001_QWANTUM_TOKEN);
		}
		return result;
	}

	public string getQwantumCardID()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatQ001)
		{
			result = getFieldValue(Q001_QWANTUM_CARD_ID);
		}
		return result;
	}

	public string getEncryptedQwantumDataBuffer()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatQ002)
		{
			result = getFieldValue(Q002_ENCRYPTED_QWANTUM_DATA_BUFFER);
		}
		return result;
	}

	public string getMACDUKPTKeyInfo()
	{
		string result = "";
		switch (m_formatCode)
		{
		case FORMATCODE.FormatM001:
			result = getFieldValue(M001_MAC_DUKPT_KEY_INFO);
			break;
		case FORMATCODE.FormatQ001:
			result = getFieldValue(Q001_MAC_DUKPT_KEY_INFO);
			break;
		case FORMATCODE.FormatQ002:
			result = getFieldValue(Q002_MAC_DUKPT_KEY_INFO);
			break;
		}
		return result;
	}

	public string getMACMessageLength()
	{
		string result = "";
		switch (m_formatCode)
		{
		case FORMATCODE.FormatM001:
			result = getFieldValue(M001_MAC_MESSAGE_LENGTH);
			break;
		case FORMATCODE.FormatQ001:
			result = getFieldValue(Q001_MAC_MESSAGE_LENGTH);
			break;
		case FORMATCODE.FormatQ002:
			result = getFieldValue(Q002_MAC_MESSAGE_LENGTH);
			break;
		}
		return result;
	}

	public string getMAC()
	{
		string result = "";
		switch (m_formatCode)
		{
		case FORMATCODE.FormatM001:
			result = getFieldValue(M001_MAC);
			break;
		case FORMATCODE.FormatQ001:
			result = getFieldValue(Q001_MAC);
			break;
		case FORMATCODE.FormatQ002:
			result = getFieldValue(Q002_MAC);
			break;
		}
		return result;
	}

	public string getCustomerMessageCode()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatQ003)
		{
			result = getFieldValue(Q003_CUSTOMER_MESSAGE_CODE);
		}
		return result;
	}

	public string getCustomerMessageText()
	{
		string result = "";
		FORMATCODE formatCode = m_formatCode;
		if (formatCode == FORMATCODE.FormatQ003)
		{
			result = getFieldValue(Q003_CUSTOMER_MESSAGE_TEXT);
		}
		return result;
	}
}
