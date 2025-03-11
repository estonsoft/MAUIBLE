// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.SCRAUartMsr
using System;
using System.Text;



internal class SCRAUartMsr
{
	public delegate void DataReceivedHandler(object sender, string cardData);

	public delegate void DebugInfoHandler(object sender, string data);

	private MTSCRA m_SCRA;

	private byte[] m_uartDataReceived;

	public event DataReceivedHandler OnDataReceived;

	public event DebugInfoHandler OnDebugInfo;

	public SCRAUartMsr(MTSCRA scra)
	{
		m_SCRA = scra;
		m_uartDataReceived = null;
	}

	private byte[] buildExtendedCommand(byte[] command, byte[] data)
	{
		byte[] array = null;
		if (command != null && command.Length >= 2 && data != null)
		{
			int num = data.Length;
			array = new byte[num + 4];
			array[0] = command[0];
			array[1] = command[1];
			array[2] = (byte)((num >> 8) & 0xFF);
			array[3] = (byte)(num & 0xFF);
			Array.Copy(data, 0, array, 4, data.Length);
		}
		return array;
	}

	protected void sendDebugInfo(string data)
	{
		if (this.OnDebugInfo != null)
		{
			this.OnDebugInfo(this, data);
		}
	}

	public int sendData(string dataString)
	{
		int num = 9;
		if (m_SCRA != null)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(dataString);
			int num2 = bytes.Length + 2;
			byte[] array = new byte[num2];
			array[0] = 0;
			Array.Copy(bytes, 0, array, 1, bytes.Length);
			array[num2 - 1] = 13;
			byte[] command = new byte[2] { 4, 0 };
			string hexString = TLVParser.getHexString(buildExtendedCommand(command, array));
			sendDebugInfo("Send UART Extended Command: " + hexString);
			num = m_SCRA.sendExtendedCommand(hexString);
			if (num != 0)
			{
				sendDebugInfo("SendExtendedCommand error=" + num);
			}
		}
		return num;
	}

	public void processDeviceExtendedResponse(byte[] dataBytes)
	{
		if (dataBytes != null && isUARTDataNotification(dataBytes))
		{
			processUARTData(dataBytes);
		}
	}

	protected bool isUARTDataNotification(byte[] dataBytes)
	{
		bool result = false;
		if (dataBytes != null && dataBytes.Length >= 3 && dataBytes[0] == 4 && dataBytes[1] == 0)
		{
			result = true;
		}
		return result;
	}

	protected void processUARTData(byte[] dataBytes)
	{
		if (dataBytes == null || dataBytes.Length < 5)
		{
			return;
		}
		int num = dataBytes.Length - 5;
		if (num <= 0)
		{
			return;
		}
		byte[] array = null;
		if (m_uartDataReceived != null)
		{
			int num2 = m_uartDataReceived.Length;
			array = new byte[num2 + num];
			Array.Copy(m_uartDataReceived, 0, array, 0, num2);
			Array.Copy(dataBytes, 5, array, num2, num);
		}
		else
		{
			array = new byte[num];
			Array.Copy(dataBytes, 5, array, 0, num);
		}
		int num3 = array.Length;
		if (num3 <= 0)
		{
			return;
		}
		int num4 = 0;
		for (int i = 0; i < num3; i++)
		{
			if (array[i] != 13)
			{
				continue;
			}
			int num5 = i - num4;
			if (num5 > 0)
			{
				byte[] array2 = new byte[num5];
				Array.Copy(array, num4, array2, 0, num5);
				string hexString = TLVParser.getHexString(array2);
				sendDebugInfo("UART Data=" + hexString);
				string @string = Encoding.UTF8.GetString(array2);
				if (this.OnDataReceived != null)
				{
					this.OnDataReceived(this, @string);
				}
			}
			num4 = i + 1;
		}
		int num6 = num3 - num4;
		if (num6 > 0)
		{
			m_uartDataReceived = new byte[num6];
			Array.Copy(array, num4, m_uartDataReceived, 0, num6);
		}
		else
		{
			m_uartDataReceived = null;
		}
	}
}
