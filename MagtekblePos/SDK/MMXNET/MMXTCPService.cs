// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXTCPService
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


internal class MMXTCPService : MMXBaseService
{
	private TcpClient client;

	private NetworkStream stream;

	private CancellationTokenSource cancelSource;

	private readonly int PortNumber = 9999;

	private Regex ipPattern = new Regex("(?<ip>\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})(:(?<port>\\d+))?/?");

	public override void connect()
	{
		try
		{
			release();
			string text = "";
			int port = PortNumber;
			if (ipPattern.IsMatch(_deviceId))
			{
				Match match = ipPattern.Match(_deviceId);
				text = match.Groups["ip"].ToString();
				if (match.Groups["port"].Success)
				{
					port = int.Parse(match.Groups["port"].ToString());
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			client = new TcpClient();
			client.Connect(text, port);
			stream = client.GetStream();
			cancelSource = new CancellationTokenSource();
			Task.Run(delegate
			{
				try
				{
					byte[] array = new byte[2048];
					while (stream != null && stream.CanRead)
					{
						int num = stream.Read(array, 0, 2048);
						Debug.LogDevice(array.Take(num).ToArray().ByteArrayToHexString());
						if (num <= 0)
						{
							Debug.LogCommunication("Shutdown!");
							break;
						}
						FireOnData(array.Take(num));
					}
				}
				catch (Exception ex)
				{
					Debug.LogCommunication("TCP reading error : " + ex.Message);
				}
				cancelSource = null;
				release();
			}, cancelSource.Token);
			SetConnectionState(MMXConnectionState.Connected);
		}
		catch
		{
		}
	}

	protected void release()
	{
		if (cancelSource != null)
		{
			if (!cancelSource.IsCancellationRequested)
			{
				cancelSource.Cancel();
			}
			cancelSource.Dispose();
			cancelSource = null;
		}
		if (stream != null)
		{
			stream.Close();
			stream.Dispose();
			stream = null;
		}
		if (client != null)
		{
			client.Close();
			client.Dispose();
			client = null;
		}
		SetConnectionState(MMXConnectionState.Disconnected);
	}

	public override void disconnect()
	{
		release();
	}

	public override void sendData(byte[] data)
	{
		if (stream != null)
		{
			stream.Write(data, 0, data.Length);
		}
	}

	public override string[] getDeviceList()
	{
		return new string[0];
	}

	public override bool isConneted()
	{
		if (client != null && client.Connected)
		{
			return stream != null;
		}
		return false;
	}
}
