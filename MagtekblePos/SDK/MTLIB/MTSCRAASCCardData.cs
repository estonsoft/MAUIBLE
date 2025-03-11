// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTSCRAASCCardData
using System;
using System.Text;
using System.Threading;


public class MTSCRAASCCardData : IMTCardData, IMTCardDataHandler
{
	private int CARDDATA_FORMATINDEX;

	private int CARDDATA_MASKEDTRACKS;

	private int CARDDATA_DEVICE_ENCRYPTION_STATUS = 1;

	private int CARDDATA_ENCRYPTED_TRACK1 = 2;

	private int CARDDATA_ENCRYPTED_TRACK2 = 3;

	private int CARDDATA_ENCRYPTED_TRACK3 = 4;

	private int CARDDATA_MAGNEPRINT_STATUS = 5;

	private int CARDDATA_ENCRYPTED_MAGNEPRINT = 6;

	private int CARDDATA_DEVICE_SERIAL_NUMBER = 7;

	private int CARDDATA_ENCRYPTED_SESSIONID = 8;

	private int CARDDATA_DEVICE_KSN = 9;

	private int CARDDATA_ENCRYPTED_SESSIONCOUNTER = 9;

	private int CARDDATA_HASHOFPAN = 9;

	private int CARDDATA_CRC = 10;

	private int CARDDATA_FORMATCODE = 11;

	private int CARDDATA_MIN_FIELDCOUNT = 13;

	private static string m_formatCode = "";

	private byte[] m_ascData;

	private string m_ascString;

	private string[] m_ascArray;

	private string m_maskedString;

	private string[] m_maskedArray;

	private bool m_eofFound;

	private byte BYTE_EOF = 13;

	private ReaderWriterLockSlim m_dataLock;

	public MTSCRAASCCardData()
	{
		m_dataLock = new ReaderWriterLockSlim();
		clearData();
	}

	~MTSCRAASCCardData()
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
		m_ascData = null;
		m_ascString = "";
		m_ascArray = null;
		m_maskedString = "";
		m_maskedArray = null;
		m_eofFound = false;
		m_dataLock.ExitReadLock();
	}

	public void setDataThreshold(int nBytes)
	{
	}

	public bool isDataReady()
	{
		return m_eofFound;
	}

	protected string getFieldValue(int index)
	{
		string result = "";
		try
		{
			if (m_ascArray.Length >= CARDDATA_MIN_FIELDCOUNT)
			{
				result = m_ascArray[index];
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	protected string getFieldValueFromMaskedArray(string startKey)
	{
		string result = "";
		try
		{
			string text = "";
			for (int i = 0; i < m_maskedArray.Length; i++)
			{
				text = m_maskedArray[i];
				if (text.StartsWith(startKey))
				{
					result = text;
					break;
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	protected void parseASCData()
	{
		if (m_ascData == null || m_ascData.Length < 1)
		{
			return;
		}
		m_ascString = Encoding.UTF8.GetString(m_ascData, 0, m_ascData.Length);
		string[] separator = new string[1] { "|" };
		m_ascArray = m_ascString.Split(separator, StringSplitOptions.None);
		if (m_ascArray.Length < CARDDATA_MIN_FIELDCOUNT)
		{
			return;
		}
		m_formatCode = m_ascArray[m_ascArray.Length - 1].Replace("\r", "");
		m_maskedString = m_ascArray[CARDDATA_MASKEDTRACKS];
		if (m_maskedString.Length > 0)
		{
			string[] separator2 = new string[1] { "?" };
			m_maskedArray = m_maskedString.Split(separator2, StringSplitOptions.None);
			for (int i = 0; i < m_maskedArray.Length; i++)
			{
				if (m_maskedArray[i].Length > 0)
				{
					m_maskedArray[i] += "?";
				}
			}
		}
		if (m_formatCode.Equals("0001", StringComparison.CurrentCultureIgnoreCase))
		{
			CARDDATA_FORMATINDEX = 1;
		}
		else if (m_formatCode.Equals("0008", StringComparison.CurrentCultureIgnoreCase))
		{
			CARDDATA_FORMATINDEX = 1;
		}
		else if (m_formatCode.Equals("0009", StringComparison.CurrentCultureIgnoreCase))
		{
			CARDDATA_FORMATINDEX = 2;
		}
		else
		{
			CARDDATA_FORMATINDEX = 0;
		}
		CARDDATA_CRC = 10 + CARDDATA_FORMATINDEX;
		CARDDATA_FORMATCODE = 11 + CARDDATA_FORMATINDEX;
	}

	public void setData(byte[] data)
	{
		m_ascData = null;
		m_eofFound = true;
		if (data != null)
		{
			int num = data.Length;
			if (num > 0)
			{
				m_ascData = new byte[num];
				Array.Copy(data, 0, m_ascData, 0, num);
				parseASCData();
			}
		}
	}

	public void handleData(byte[] data)
	{
		m_dataLock.EnterReadLock();
		_ = m_ascData;
		m_dataLock.ExitReadLock();
		m_dataLock.EnterWriteLock();
		if (m_ascData == null)
		{
			m_eofFound = false;
			m_ascData = data;
		}
		else
		{
			byte[] array = new byte[m_ascData.Length + data.Length];
			Array.Copy(m_ascData, 0, array, 0, m_ascData.Length);
			Array.Copy(data, 0, array, m_ascData.Length, data.Length);
			m_ascData = array;
		}
		if (data != null && !m_eofFound)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] == BYTE_EOF)
				{
					parseASCData();
					m_eofFound = true;
				}
			}
		}
		m_dataLock.ExitWriteLock();
	}

	public byte[] getData()
	{
		return m_ascData;
	}

	public void clearBuffers()
	{
		clearData();
	}

	public string getMaskedTracks()
	{
		return getFieldValue(CARDDATA_MASKEDTRACKS);
	}

	public string getTrack1()
	{
		return getFieldValue(CARDDATA_ENCRYPTED_TRACK1);
	}

	public string getTrack2()
	{
		return getFieldValue(CARDDATA_ENCRYPTED_TRACK2);
	}

	public string getTrack3()
	{
		return getFieldValue(CARDDATA_ENCRYPTED_TRACK3);
	}

	public string getTrack1Masked()
	{
		return getFieldValueFromMaskedArray("%");
	}

	public string getTrack2Masked()
	{
		return getFieldValueFromMaskedArray(";");
	}

	public string getTrack3Masked()
	{
		string text = "";
		text = getFieldValueFromMaskedArray("+");
		if (text.Length < 1)
		{
			text = getFieldValueFromMaskedArray("#");
		}
		return text;
	}

	public string getMagnePrint()
	{
		string result = "";
		try
		{
			if (m_ascArray.Length >= CARDDATA_MIN_FIELDCOUNT)
			{
				result = m_ascArray[CARDDATA_ENCRYPTED_MAGNEPRINT];
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getMagnePrintStatus()
	{
		return getFieldValue(CARDDATA_MAGNEPRINT_STATUS);
	}

	public string getDeviceSerial()
	{
		return getFieldValue(CARDDATA_DEVICE_SERIAL_NUMBER);
	}

	public string getSessionID()
	{
		return getFieldValue(CARDDATA_ENCRYPTED_SESSIONID);
	}

	public string getKSN()
	{
		return getFieldValue(CARDDATA_DEVICE_KSN);
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
		string fieldValue = getFieldValue(CARDDATA_CRC);
		long result = 0L;
		try
		{
			long.TryParse(fieldValue, out result);
		}
		catch (Exception)
		{
		}
		return result;
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
			result = ((m_ascArray != null) ? m_ascArray.Length : 0);
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
		return getFieldValue(CARDDATA_DEVICE_ENCRYPTION_STATUS);
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
