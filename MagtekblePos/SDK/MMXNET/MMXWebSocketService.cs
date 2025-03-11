// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXWebSocketService
using System;
using System.Configuration;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


internal class MMXWebSocketService : MMXBaseService
{
	private CancellationTokenSource cancelSource;

	private readonly int PortNumber = 9999;

	private Regex ipPattern = new Regex("(?<ip>\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})(:(?<port>\\d+))?/?");

	private Regex addrPattern = new Regex("(\\\\w+)(:(?<port>\\d+))?/?");

	private ClientWebSocket client = new ClientWebSocket();

	internal bool useBinay = true;

	private Semaphore sendingPool = new Semaphore(1, 1);

	public MMXWebSocketService()
	{
		if (!bool.TryParse(ConfigurationManager.AppSettings.Get("BinaryWebSocket"), out useBinay))
		{
			useBinay = true;
		}
	}

	public override void connect()
	{
		try
		{
			release();
			cancelSource = new CancellationTokenSource();
			client.ConnectAsync(new Uri("ws://" + _deviceId), cancelSource.Token).Wait();
			if (!isConneted())
			{
				return;
			}
			Task.Run(async delegate
			{
				try
				{
					byte[] buffer = new byte[2048];
					ArraySegment<byte> seg = new ArraySegment<byte>(buffer, 0, 2048);
					WebSocketReceiveResult webSocketReceiveResult;
					while (true)
					{
						webSocketReceiveResult = await client.ReceiveAsync(seg, cancelSource.Token);
						if (webSocketReceiveResult.MessageType == WebSocketMessageType.Binary)
						{
							if (webSocketReceiveResult.Count > 0)
							{
								FireOnData(buffer.Take(webSocketReceiveResult.Count));
							}
						}
						else
						{
							if (webSocketReceiveResult.MessageType != 0)
							{
								break;
							}
							string @string = Encoding.ASCII.GetString(buffer, 0, webSocketReceiveResult.Count);
							FireOnData(@string.HexStringToByteArray());
						}
					}
					if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
					{
						Debug.LogCommunication("Web Socket closing: ");
					}
				}
				catch (Exception ex)
				{
					Debug.LogCommunication("Web Socket reading error : " + ex.Message);
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

	public override void disconnect()
	{
		release();
	}

	protected void release()
	{
		if (client != null && client.State == WebSocketState.Open)
		{
			client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal", cancelSource.Token).Wait();
		}
		if (cancelSource != null)
		{
			if (!cancelSource.IsCancellationRequested)
			{
				cancelSource.Cancel();
			}
			cancelSource.Dispose();
			cancelSource = null;
		}
		client.Dispose();
		client = new ClientWebSocket();
		SetConnectionState(MMXConnectionState.Disconnected);
	}

	public override void setDeviceID(string ID)
	{
		base.setDeviceID(ID);
	}

	public override string[] getDeviceList()
	{
		return new string[0];
	}

	public override bool isConneted()
	{
		return client.State == WebSocketState.Open;
	}

	public override async void sendData(byte[] data)
	{
		sendingPool.WaitOne();
		try
		{
			if (useBinay)
			{
				await sendBinary(data);
				return;
			}
			string hex = data.ByteArrayToHexString();
			await sendText(hex);
		}
		finally
		{
			sendingPool.Release();
		}
	}

	protected async Task sendText(string hex)
	{
		ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.ASCII.GetBytes(hex));
		await client.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, cancelSource.Token);
	}

	protected async Task sendBinary(byte[] data)
	{
		ArraySegment<byte> buffer = new ArraySegment<byte>(data);
		await client.SendAsync(buffer, WebSocketMessageType.Binary, endOfMessage: true, cancelSource.Token);
	}
}
