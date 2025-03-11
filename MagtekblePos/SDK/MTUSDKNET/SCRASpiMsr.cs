// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.SCRASpiMsr
using System;
using System.Threading.Tasks;




internal class SCRASpiMsr
{
	public delegate void DataReceivedHandler(object sender, IMTCardData cardData);

	public delegate void ResponseReceivedHandler(object sender, string response);

	public delegate void DebugInfoHandler(object sender, string data);

	private MTSCRA m_SCRA;

	private bool m_spiStatusRequestPending;

	private bool m_spiDataRequestPending;

	private byte[] m_spiDataReceived;

	private byte[] m_spiHeadData;

	private int m_spiHeadDataLength;

	public event DataReceivedHandler OnDataReceived;

	public event ResponseReceivedHandler OnResponseReceived;

	public event DebugInfoHandler OnDebugInfo;

	public SCRASpiMsr(MTSCRA scra)
	{
		m_SCRA = scra;
		m_spiStatusRequestPending = false;
		m_spiDataRequestPending = false;
		m_spiDataReceived = null;
		m_spiHeadData = null;
		m_spiHeadDataLength = 0;
		requestSPIStatus();
	}

	protected void sendDebugInfo(string data)
	{
		if (this.OnDebugInfo != null)
		{
			this.OnDebugInfo(this, data);
		}
	}

	public void processDeviceExtendedResponse(byte[] dataBytes)
	{
		if (dataBytes != null)
		{
			if (m_spiStatusRequestPending)
			{
				processSPIStatus(dataBytes);
			}
			else if (m_spiDataRequestPending)
			{
				processSPIData(dataBytes);
			}
			else if (isSPIDataNotification(dataBytes))
			{
				requestSPIData(5);
			}
		}
	}

	protected bool isSPIDataNotification(byte[] dataBytes)
	{
		bool result = false;
		if (dataBytes != null && dataBytes.Length >= 6 && dataBytes[0] == 5 && dataBytes[1] == 0 && (dataBytes[5] & 2) != 0)
		{
			result = true;
		}
		return result;
	}

	protected void requestSPIStatus()
	{
		Task.Factory.StartNew((Func<Task>)async delegate
		{
			await Task.Delay(100);
			if (m_SCRA != null)
			{
				string text = "0501";
				string text2 = "0001";
				string text3 = "00";
				sendDebugInfo("[Request SPI Status]");
				m_spiStatusRequestPending = true;
				m_SCRA.sendExtendedCommand(text + text2 + text3);
			}
		});
	}

	protected void requestSPIData(int length)
	{
		Task.Factory.StartNew((Func<object?, Task>)async delegate
		{
			await Task.Delay(50);
			writeSPIData(length);
		}, (object?)length);
	}

	protected void writeSPIData(int len)
	{
		if (len > 0)
		{
			string dataString = new string('F', len * 2);
			sendDebugInfo("[Request SPI Data] Length=" + len);
			m_spiDataRequestPending = true;
			int num = sendData(dataString);
			if (num != 0)
			{
				sendDebugInfo("[Request SPI Data] *** Result=" + num);
			}
		}
	}

	public int sendData(string dataString)
	{
		int result = 9;
		if (m_SCRA != null && dataString != null && dataString.Length > 0)
		{
			int length = dataString.Length / 2 + 1;
			string twoBytesLengthString = getTwoBytesLengthString(length);
			string text = "0500" + twoBytesLengthString + "00" + dataString;
			result = m_SCRA.sendExtendedCommand(text);
		}
		return result;
	}

	protected string getTwoBytesLengthString(int length)
	{
		byte[] array = new byte[2];
		array[1] = (byte)((length % 256) & 0xFF);
		array[0] = (byte)(((length >> 8) % 256) & 0xFF);
		return MTParser.getHexString(array);
	}

	protected void processSPIStatus(byte[] dataBytes)
	{
		bool flag = false;
		if (dataBytes != null && dataBytes.Length >= 5 && dataBytes[4] == 5)
		{
			flag = true;
		}
		m_spiStatusRequestPending = false;
		if (flag)
		{
			requestSPIData(5);
		}
	}

	protected void processSPIData(byte[] dataBytes)
	{
		m_spiDataRequestPending = false;
		if (dataBytes == null || dataBytes.Length < 2 || dataBytes[0] != 0 || dataBytes[1] != 0)
		{
			return;
		}
		int num = 4;
		if (dataBytes.Length > num)
		{
			int num2 = dataBytes.Length - num;
			byte[] array = null;
			if (m_spiDataReceived != null)
			{
				int num3 = m_spiDataReceived.Length;
				array = new byte[num3 + num2];
				Array.Copy(m_spiDataReceived, 0, array, 0, num3);
				Array.Copy(dataBytes, num, array, num3, num2);
			}
			else
			{
				array = new byte[num2];
				Array.Copy(dataBytes, num, array, 0, num2);
			}
			m_spiDataReceived = array;
			processSPIDataReceived();
		}
	}

	protected void processSPIDataReceived()
	{
		if (m_spiDataReceived == null)
		{
			return;
		}
		if (m_spiHeadDataLength > 0)
		{
			processSPIHeadData(m_spiDataReceived);
			m_spiDataReceived = null;
			return;
		}
		for (int i = 0; i < m_spiDataReceived.Length; i++)
		{
			if (m_spiDataReceived[i] == byte.MaxValue)
			{
				continue;
			}
			if (m_spiDataReceived[i] == 1)
			{
				i++;
				if (i + 1 < m_spiDataReceived.Length)
				{
					byte b = m_spiDataReceived[i++];
					byte b2 = m_spiDataReceived[i++];
					m_spiHeadDataLength = (b << 8) + b2;
					int num = m_spiDataReceived.Length - i;
					if (num > 0)
					{
						byte[] array = new byte[num];
						Array.Copy(m_spiDataReceived, i, array, 0, num);
						m_spiDataReceived = null;
						processSPIHeadData(array);
					}
					else
					{
						m_spiDataReceived = null;
						processSPIHeadData(null);
					}
					break;
				}
				requestSPIData(5);
			}
			else
			{
				requestSPIData(5);
			}
		}
	}

	protected void processSPIHeadData(byte[] newData)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		if (newData != null)
		{
			int num = newData.Length;
			if (m_spiHeadData != null)
			{
				int num2 = m_spiHeadData.Length;
				byte[] array = new byte[num2 + num];
				Array.Copy(m_spiHeadData, 0, array, 0, num2);
				Array.Copy(newData, 0, array, num2, num);
				m_spiHeadData = array;
			}
			else
			{
				m_spiHeadData = new byte[num];
				Array.Copy(newData, 0, m_spiHeadData, 0, num);
			}
		}
		if (m_spiHeadDataLength <= 0)
		{
			return;
		}
		int num3 = m_spiHeadDataLength;
		if (m_spiHeadData != null)
		{
			num3 -= m_spiHeadData.Length;
		}
		if (num3 > 0)
		{
			requestSPIData(num3);
			return;
		}
		int num4 = m_spiHeadData.Length;
		if (num4 > 1)
		{
			if (m_spiHeadData[0] == 2)
			{
				MTSCRAASCCardData val = new MTSCRAASCCardData();
				byte[] array2 = new byte[num4 - 1];
				Array.Copy(m_spiHeadData, 1, array2, 0, num4 - 1);
				val.setData(array2);
				if (this.OnDataReceived != null)
				{
					this.OnDataReceived(this, (IMTCardData)(object)val);
				}
			}
			else if (m_spiHeadData[0] == 1)
			{
				byte[] array3 = new byte[num4 - 1];
				Array.Copy(m_spiHeadData, 1, array3, 0, num4 - 1);
				if (this.OnResponseReceived != null)
				{
					string hexString = MTParser.getHexString(array3);
					this.OnResponseReceived(this, hexString);
				}
			}
		}
		m_spiHeadData = null;
		m_spiHeadDataLength = 0;
	}
}
