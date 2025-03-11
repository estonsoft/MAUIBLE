// MTDevice, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTSCRAPOSCardData
using System;
using System.Collections.Generic;
using System.Text;


public class MTSCRAPOSCardData : IMTCardData, IMTCardDataHandler
{
	private int m_threshold;

	private byte[] m_tlvData;

	private List<Dictionary<string, string>> m_parsedTLVData;

	private string m_ResponseData;

	private string m_TLVVersion;

	private string m_MaskedTracks;

	private string m_FormatCode;

	private string m_ResponseType;

	private string[] m_CardDataArray;

	private string m_MaskedTrackArray;

	private string m_MSLocalMerchantData;

	private string m_MSSwipeStatus;

	private string m_MSSecureData;

	private string m_DeviceInfo;

	private string m_DeviceStatus;

	private string m_DeviceConfig;

	private string m_AdditionalInfo;

	private string m_PAN;

	private const int MAX_TK1_LENGTH = 77;

	private const int MAX_TK2_LENGTH = 38;

	~MTSCRAPOSCardData()
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
		m_TLVVersion = "";
		m_MaskedTracks = "";
		m_FormatCode = "";
		m_ResponseData = "";
		m_ResponseType = "";
		m_CardDataArray = null;
		m_MaskedTracks = null;
		m_MSLocalMerchantData = "";
		m_MSSwipeStatus = "";
		m_MSSecureData = "";
		m_DeviceInfo = "";
		m_DeviceStatus = "";
		m_DeviceConfig = "";
		m_AdditionalInfo = "";
		m_PAN = "";
	}

	public void setDataThreshold(int nBytes)
	{
		m_threshold = nBytes;
	}

	public bool isDataReady()
	{
		if (m_tlvData != null && m_tlvData.Length != 0)
		{
			return true;
		}
		return false;
	}

	public void setData(byte[] data)
	{
		clearBuffers();
		m_tlvData = data;
		m_ResponseData = MTParser.getHexString(data);
		m_parsedTLVData = MTParser.parseMTTLVData(data, hasSizeHeader: false);
		string responseType = getResponseType();
		if (string.Compare(responseType, "C101", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(responseType, "C106", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(responseType, "C302", StringComparison.CurrentCultureIgnoreCase) == 0)
		{
			m_TLVVersion = getTagValue("8109");
			m_MSSwipeStatus = getTagValue("C201");
			m_MSLocalMerchantData = getTagValue("C202");
			m_MSSecureData = getTagValue("C203");
			m_DeviceInfo = getTagValue("C302");
			if (m_MSLocalMerchantData.Length <= 0)
			{
				return;
			}
			if (m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion, 16) >= 3)
			{
				m_PAN = getMaskedPAN();
				return;
			}
			string cardIIN = getCardIIN();
			string cardLast = getCardLast4();
			int cardPANLength = getCardPANLength();
			if (cardIIN.Length > 0 && cardLast.Length > 0 && cardPANLength > 0)
			{
				m_AdditionalInfo = getCardExpDate() + getCardServiceCode();
				m_PAN = cardIIN;
				int cardPANLength2 = getCardPANLength();
				for (int i = 0; i < cardPANLength2 - cardIIN.Length - cardLast.Length; i++)
				{
					m_PAN += "0";
				}
				m_PAN += cardLast;
				if (cardPANLength2 > cardLast.Length + cardIIN.Length)
				{
					int num = 0;
					num = cardIIN.Length + (cardPANLength2 - (cardIIN.Length + cardLast.Length)) / 2;
					byte[] bytes = Encoding.UTF8.GetBytes(m_PAN);
					m_PAN = DFM_vFixCheckDigit(bytes, cardPANLength2, (byte)num);
				}
			}
		}
		else if (string.Compare(responseType, "C104", StringComparison.CurrentCultureIgnoreCase) == 0)
		{
			m_DeviceInfo = getTagValue("C302");
			m_TLVVersion = getTagValue("8109");
			m_DeviceStatus = getTagValue("C303");
			m_DeviceConfig = getTagValue("C304");
			m_MSSecureData = getTagValue("C203");
		}
		else if (string.Compare(responseType, "C204", StringComparison.CurrentCultureIgnoreCase) == 0)
		{
			m_DeviceInfo = getTagValue("C302");
			m_TLVVersion = getTagValue("8109");
			m_DeviceStatus = getTagValue("C303");
			m_DeviceConfig = getTagValue("C304");
			m_MSSecureData = getTagValue("C203");
		}
	}

	public void handleData(byte[] data)
	{
		setData(data);
	}

	public byte[] getData()
	{
		return m_tlvData;
	}

	public void clearBuffers()
	{
		clearData();
	}

	public string getMaskedTracks()
	{
		return getTrack1Masked() + getTrack2Masked() + getTrack3Masked();
	}

	public string getTrack1()
	{
		return getTagValueFromMSSecureData("830A", "8211");
	}

	public string getTrack2()
	{
		return getTagValueFromMSSecureData("830B", "8212");
	}

	public string getTrack3()
	{
		return getTagValueFromMSSecureData("830C", "8213");
	}

	public string getTrack1Masked()
	{
		string text = "";
		try
		{
			if (m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion, 16) >= 3)
			{
				if (m_MSLocalMerchantData.Length > 0)
				{
					string tagValue = getTagValue("8221", m_MSLocalMerchantData);
					if (tagValue.Length > 0)
					{
						byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValue);
						text = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
					}
				}
				return text;
			}
			if (m_PAN.Length > 0)
			{
				for (int i = (text = string.Format("%%B{0}^{1}^{2}", new object[3]
				{
					m_PAN,
					getCardName(),
					m_AdditionalInfo
				})).Length; i < 77; i++)
				{
					text += "0";
				}
				text += "?";
			}
		}
		catch (Exception)
		{
		}
		return text;
	}

	public string getTrack2Masked()
	{
		string text = "";
		try
		{
			if (m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion, 16) >= 3)
			{
				if (m_MSLocalMerchantData.Length > 0)
				{
					string tagValue = getTagValue("8222", m_MSLocalMerchantData);
					if (tagValue.Length > 0)
					{
						byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValue);
						text = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
					}
				}
				return text;
			}
			if (m_PAN.Length > 0)
			{
				for (int i = (text = string.Format(";{0}={1}", new object[2] { m_PAN, m_AdditionalInfo })).Length; i < 38; i++)
				{
					text += "0";
				}
				text += "?";
			}
		}
		catch (Exception)
		{
		}
		return text;
	}

	public string getTrack3Masked()
	{
		string result = "";
		try
		{
			string tagValueFromData = getTagValueFromData("8223", m_MSLocalMerchantData);
			if (tagValueFromData.Length > 0)
			{
				byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
				result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getMagnePrint()
	{
		return getTagValueFromMSSecureData("830D", "8214");
	}

	public string getMagnePrintStatus()
	{
		return getTagValueFromMSSecureData("830E", "");
	}

	public string getDeviceSerial()
	{
		string result = "";
		try
		{
			if (m_TLVVersion.Length > 0)
			{
				if (Convert.ToInt32(m_TLVVersion, 16) >= 3 && m_MSSecureData.Length > 0)
				{
					string tagValueFromData = getTagValueFromData("8102", m_MSSecureData);
					if (tagValueFromData.Length > 0)
					{
						byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
						result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
					}
					return result;
				}
				result = ((string.Compare(getResponseType(), "C101", StringComparison.CurrentCultureIgnoreCase) == 0) ? getTagValueFromData("8101", m_MSSecureData) : ((m_DeviceInfo.Length <= 0) ? getTagValueFromData("8101", m_ResponseData) : getTagValueFromData("8101", m_DeviceInfo)));
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getSessionID()
	{
		return getTagValueFromData("8309", m_MSSecureData);
	}

	public string getKSN()
	{
		return getTagValueFromData("8301", m_MSSecureData);
	}

	public string getDeviceName()
	{
		string result = "";
		try
		{
			string tagValueFromData = getTagValueFromData("8104", m_DeviceInfo, m_ResponseData);
			if (tagValueFromData.Length > 0)
			{
				byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
				result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public long getBatteryLevel()
	{
		return getTagLongValueFromData("8140", m_DeviceInfo, m_ResponseData, MTDeviceConstants.BATTERY_LEVEL_NA);
	}

	public long getSwipeCount()
	{
		return getTagLongValueFromData("8141", m_DeviceInfo, m_ResponseData, MTDeviceConstants.SWIPE_COUNT_NA);
	}

	public string getCapMagnePrint()
	{
		return getTagValueFromData("8123", m_DeviceInfo);
	}

	public string getCapMagnePrintEncryption()
	{
		return getTagValueFromData("8123", m_DeviceInfo);
	}

	public string getCapMagneSafe20Encryption()
	{
		return getTagValueFromData("8125", m_DeviceInfo);
	}

	public string getCapMagStripeEncryption()
	{
		return getTagValueFromData("8122", m_DeviceInfo);
	}

	public string getCapMSR()
	{
		return getTagValueFromData("8120", m_DeviceInfo);
	}

	public string getCapTracks()
	{
		return getTagValueFromData("8121", m_DeviceInfo);
	}

	public long getCardDataCRC()
	{
		return 0L;
	}

	public string getCardExpDate()
	{
		string result = "";
		try
		{
			if (m_MSLocalMerchantData.Length > 0)
			{
				if (m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion) >= 3)
				{
					string additionalInfo = getAdditionalInfo();
					if (additionalInfo.Length > 6)
					{
						result = additionalInfo.Substring(0, 4);
					}
					return result;
				}
				string tagValueFromData = getTagValueFromData("8244", m_MSLocalMerchantData);
				if (tagValueFromData.Length > 0)
				{
					byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
					result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getCardIIN()
	{
		string result = "";
		try
		{
			if (m_MSLocalMerchantData.Length > 0)
			{
				if (m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion) >= 3)
				{
					if (m_PAN.Length >= 6)
					{
						result = m_PAN.Substring(0, 6);
					}
					return result;
				}
				string tagValueFromData = getTagValueFromData("8242", m_MSLocalMerchantData);
				if (tagValueFromData.Length > 0)
				{
					byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
					result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getCardLast4()
	{
		string result = "";
		try
		{
			if (m_MSLocalMerchantData.Length > 0)
			{
				if (m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion) >= 3)
				{
					if (m_PAN.Length > 4)
					{
						result = m_PAN.Substring(m_PAN.Length - 4);
					}
					return result;
				}
				string tagValueFromData = getTagValueFromData("8243", m_MSLocalMerchantData);
				if (tagValueFromData.Length > 0)
				{
					byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
					result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getCardName()
	{
		string result = "";
		try
		{
			if (m_MSLocalMerchantData.Length > 0)
			{
				if (m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion) >= 3)
				{
					result = getNameFromMaskedTrack1();
					return result;
				}
				string tagValueFromData = getTagValueFromData("8241", m_MSLocalMerchantData);
				if (tagValueFromData.Length > 0)
				{
					byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
					result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getCardPAN()
	{
		return getMaskedPAN();
	}

	public int getCardPANLength()
	{
		int result = 0;
		try
		{
			if (m_MSSecureData.Length > 0 && m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion) >= 3)
			{
				result = m_PAN.Length;
				return result;
			}
			if (m_MSLocalMerchantData.Length > 0)
			{
				string tagValueFromData = getTagValueFromData("8246", m_MSLocalMerchantData);
				if (tagValueFromData.Length > 0)
				{
					result = Convert.ToInt32(tagValueFromData, 16);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getCardServiceCode()
	{
		string result = "";
		try
		{
			if (m_MSLocalMerchantData.Length > 0)
			{
				if (m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion) >= 3)
				{
					string additionalInfo = getAdditionalInfo();
					if (additionalInfo.Length > 6)
					{
						result = additionalInfo.Substring(4);
					}
					return result;
				}
				string tagValueFromData = getTagValueFromData("8245", m_MSLocalMerchantData);
				if (tagValueFromData.Length > 0)
				{
					byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
					result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getCardStatus()
	{
		return "";
	}

	public string getCardEncodeType()
	{
		return getTagValueFromData("8261", m_MSSwipeStatus);
	}

	public int getDataFieldCount()
	{
		return 0;
	}

	public string getHashCode()
	{
		return getTagValueFromMSSecureData("8308", "");
	}

	public string getDeviceConfig(string configType)
	{
		return getTagValueFromData(configType, m_DeviceInfo);
	}

	public string getEncryptionStatus()
	{
		return getTagValueFromData("8001", m_MSSwipeStatus);
	}

	public string getFirmware()
	{
		string result = "";
		try
		{
			string tagValueFromData = getTagValueFromData("8103", m_DeviceInfo, m_ResponseData);
			if (tagValueFromData.Length > 0)
			{
				byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
				result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getMagTekDeviceSerial()
	{
		string result = "";
		try
		{
			if (m_TLVVersion.Length > 0)
			{
				if (Convert.ToInt32(m_TLVVersion, 16) >= 3 && m_MSSecureData.Length > 0)
				{
					string tagValueFromData = getTagValueFromData("8102", m_MSSecureData);
					if (tagValueFromData.Length > 0)
					{
						byte[] byteArrayFromHexString = MTParser.getByteArrayFromHexString(tagValueFromData);
						result = Encoding.UTF8.GetString(byteArrayFromHexString, 0, byteArrayFromHexString.Length);
					}
					return result;
				}
				string tagValueFromData2 = getTagValueFromData("8102", m_DeviceInfo, m_ResponseData);
				if (tagValueFromData2.Length > 0)
				{
					byte[] byteArrayFromHexString2 = MTParser.getByteArrayFromHexString(tagValueFromData2);
					result = Encoding.UTF8.GetString(byteArrayFromHexString2, 0, byteArrayFromHexString2.Length);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string getResponseType()
	{
		if (m_ResponseType.Length == 0 && m_ResponseData.Length >= 4)
		{
			m_ResponseType = m_ResponseData.Substring(0, 4);
		}
		return m_ResponseType;
	}

	public string getTagValue(string tag, string data)
	{
		return getTagValueFromData(tag, data);
	}

	public string getTLVVersion()
	{
		if (m_TLVVersion.Length == 0)
		{
			m_TLVVersion = getTagValue("8109");
		}
		return m_TLVVersion;
	}

	public string getTrackDecodeStatus()
	{
		return getTagValueFromData("8262", m_MSSwipeStatus);
	}

	private string getResponseData()
	{
		return m_ResponseData;
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

	private string getTagValueFromMSSecureData(string tag1, string tag2)
	{
		string text = "";
		if (m_MSSecureData.Length > 0)
		{
			text = getTagValue(tag1, m_MSSecureData);
			if (m_TLVVersion.Length > 0 && Convert.ToInt32(m_TLVVersion, 16) >= 3)
			{
				return text;
			}
			if (text.Length == 0 && tag2.Length > 0)
			{
				text = getTagValueFromData(tag2, m_MSSecureData);
			}
		}
		return text;
	}

	private string getTagValueFromParsedTLVData(string lpstrTag)
	{
		string result = "";
		foreach (Dictionary<string, string> parsedTLVDatum in m_parsedTLVData)
		{
			if (string.Compare(parsedTLVDatum["tag"], lpstrTag, StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				result = parsedTLVDatum["value"];
				break;
			}
		}
		return result;
	}

	private string getTagValue(string tag)
	{
		return getTagValueFromParsedTLVData(tag);
	}

	private string getTagValueFromData(string tag, string data1, string data2)
	{
		string text = "";
		try
		{
			if (data1.Length > 0)
			{
				text = getTagValueFromData(tag, data1);
				if (text != null && text.Length > 0)
				{
					return text;
				}
			}
			else if (data2.Length > 0)
			{
				text = getTagValueFromData(tag, data2);
			}
		}
		catch (Exception)
		{
		}
		return text;
	}

	private long getTagLongValueFromData(string tag, string data1, string data2, long defaultValue)
	{
		long result = defaultValue;
		try
		{
			string tagValueFromData = getTagValueFromData(tag, data1, data2);
			if (tagValueFromData.Length > 0)
			{
				result = Convert.ToInt64(tagValueFromData, 16);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	private long getTagLongValueFromData(string tag, string data1, string data2)
	{
		long result = 0L;
		try
		{
			string tagValueFromData = getTagValueFromData(tag, data1, data2);
			if (tagValueFromData.Length > 0)
			{
				result = Convert.ToInt64(tagValueFromData, 16);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	private string getTagValueFromData(string tag, string data)
	{
		return getTagValueFromParsedTLVData(tag);
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
