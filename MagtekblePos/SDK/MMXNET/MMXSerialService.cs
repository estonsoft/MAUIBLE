// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXSerialService
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

internal class MMXSerialService : MMXBaseService
{
	private SerialPort serialPort = new SerialPort();

	private MTSliperOut slipOut = new MTSliperOut();

	private MTSlipperIn slipIn = new MTSlipperIn();

	private SerialProtocol protocol = new SerialProtocol();

	private byte[] _toResend;

	private object sendingLock = new object();

	public MMXSerialService()
	{
		serialPort.BaudRate = 115200;
		protocol.Resend += Protocol_Resend;
		slipIn.OnDataReady += SlipIn_OnDataReady;
		serialPort.DataReceived += SerialPort_DataReceived;
		serialPort.ErrorReceived += SerialPort_ErrorReceived;
	}

	private void Protocol_Resend(object sender, bool e)
	{
		if (_toResend != null)
		{
			sendData(_toResend);
		}
	}

	private void SlipIn_OnDataReady(object sender, byte[] e)
	{
		byte[] array = protocol.processIncomingData(e);
		if (array != null)
		{
			FireOnData(array);
		}
	}

	private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
	{
		serialPort.Close();
		SetConnectionState(MMXConnectionState.Disconnected);
	}

	private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		while (serialPort.BytesToRead > 0)
		{
			byte[] array = new byte[serialPort.BytesToRead];
			int num = serialPort.Read(array, 0, array.Length);
			if (num > 0)
			{
				byte[] data = utils.SubArray(array, 0, num);
				Debug.LogCommunication("Read COM: " + data.ByteArrayToHexString());
				slipIn.Add(data);
				continue;
			}
			break;
		}
	}

	public static string[] GetDeviceList()
	{
		return SerialPort.GetPortNames();
	}

	public override void connect()
	{
		try
		{
			SetConnectionState(MMXConnectionState.Connecting);
			string[] array = _deviceId.Split(',');
			serialPort.PortName = array[0].Trim();
			foreach (string item in array.Skip(1))
			{
				string[] array2 = item.Split('=');
				if (array2.Count() == 2 && string.Compare(array2[0], "BAUDRATE", ignoreCase: true) == 0)
				{
					if (array2[1].StartsWith("x"))
					{
						serialPort.BaudRate = 115200 * int.Parse(array2[1].Substring(1));
					}
					else
					{
						serialPort.BaudRate = int.Parse(array2[1]);
					}
				}
			}
			if (!string.IsNullOrWhiteSpace(_deviceId))
			{
				serialPort.Open();
				if (serialPort.IsOpen)
				{
					SetConnectionState(MMXConnectionState.Connected);
					return;
				}
			}
		}
		catch
		{
		}
		SetConnectionState(MMXConnectionState.Disconnected);
	}

	public override void disconnect()
	{
		SetConnectionState(MMXConnectionState.Disconnecting);
		if (serialPort != null)
		{
			serialPort.Close();
		}
		SetConnectionState(MMXConnectionState.Disconnected);
	}

	public override void sendData(byte[] data)
	{
		_toResend = data;
		IEnumerable<byte[]> enumerable = protocol.buildPackets(data);
		lock (sendingLock)
		{
			if (serialPort == null)
			{
				return;
			}
			foreach (byte[] item in enumerable)
			{
				byte[] array = slipOut.Convert(item.ToArray());
				Debug.LogCommunication("Write COM: " + array.ByteArrayToHexString());
				serialPort.Write(array, 0, array.Length);
			}
		}
	}

	public override string[] getDeviceList()
	{
		return GetDeviceList();
	}

	public override bool isConneted()
	{
		if (serialPort != null)
		{
			return serialPort.IsOpen;
		}
		return false;
	}
}
