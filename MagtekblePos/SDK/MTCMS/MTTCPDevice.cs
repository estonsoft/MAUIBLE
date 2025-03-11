// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTTCPDevice
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

internal class MTTCPDevice
{
	private TcpClient socket;

	private Task readingTask;

	public event EventHandler<byte[]> OnDataReceived;

	public event EventHandler OnError;

	public async Task<bool> Open(string address, int port = 26)
	{
		socket = new TcpClient();
		try
		{
			await socket.ConnectAsync(address, port);
			readingTask = new Task(delegate
			{
				while (true)
				{
					try
					{
						byte[] array = new byte[2048];
						Task<int> task = socket.GetStream().ReadAsync(array, 0, 2048);
						task.Wait();
						if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled && task.Result != 0)
						{
							if (this.OnDataReceived != null)
							{
								this.OnDataReceived(this, array.Take(task.Result).ToArray());
							}
							continue;
						}
					}
					catch (Exception)
					{
					}
					break;
				}
				if (this.OnError != null)
				{
					this.OnError(this, null);
				}
			});
			readingTask.Start();
			return true;
		}
		catch (Exception)
		{
		}
		return false;
	}

	public void Close()
	{
		if (socket != null)
		{
			socket.GetStream().Flush();
			Thread.Sleep(100);
			socket.Close();
			socket = null;
		}
	}

	public async Task<bool> Send(byte[] data)
	{
		if (socket != null)
		{
			try
			{
				await socket.GetStream().WriteAsync(data, 0, data.Length);
				return true;
			}
			catch
			{
			}
		}
		return false;
	}
}
