// MTServiceNET, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTSerialDevice
using System;
using System.Collections.Generic;
using System.IO.Ports;

internal class MTSerialDevice
{
	private const int DATA_BUFFER_SIZE = 4096;

	protected SerialPort mSerialPort;

	public event EventHandler<byte[]> OnDataReceived;

	public event EventHandler OnError;

	internal static List<string> GetPortNames()
	{
		return new List<string>(SerialPort.GetPortNames());
	}

	public MTSerialDevice()
	{
		mSerialPort = new SerialPort();
	}

	~MTSerialDevice()
	{
	}

	public bool Open(string portName, int baudRate, int dataBits, StopBits stopBits, Handshake handshake, Parity parity)
	{
		try
		{
			mSerialPort.PortName = portName;
			mSerialPort.BaudRate = baudRate;
			mSerialPort.DataBits = dataBits;
			mSerialPort.Parity = parity;
			mSerialPort.StopBits = stopBits;
			mSerialPort.Handshake = handshake;
			mSerialPort.Open();
			mSerialPort.DataReceived += OnSerialPortDataReceived;
			return true;
		}
		catch (Exception)
		{
			if (this.OnError != null)
			{
				this.OnError(this, null);
			}
			return false;
		}
	}

	public bool Close()
	{
		try
		{
			mSerialPort.DataReceived -= OnSerialPortDataReceived;
			mSerialPort.Close();
			mSerialPort = null;
			return true;
		}
		catch (Exception)
		{
			if (this.OnError != null)
			{
				this.OnError(this, null);
			}
			return false;
		}
	}

	public bool Send(byte[] data)
	{
		if (data != null)
		{
			mSerialPort.Write(data, 0, data.Length);
		}
		return true;
	}

	private void OnSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		try
		{
			if (mSerialPort == null || !mSerialPort.IsOpen)
			{
				return;
			}
			byte[] array = new byte[4096];
			if (array == null)
			{
				return;
			}
			bool flag = true;
			do
			{
				int num = mSerialPort.Read(array, 0, 4096);
				if (num > 0)
				{
					byte[] array2 = new byte[num];
					Array.Copy(array, 0, array2, 0, num);
					if (this.OnDataReceived != null)
					{
						this.OnDataReceived(this, array2);
					}
				}
				if (num < 4096)
				{
					flag = false;
				}
			}
			while (flag);
		}
		catch (Exception)
		{
		}
	}
}
