// MLog, Version=1.0.0.2, Culture=neutral, PublicKeyToken=0f9071f8ab811b68
// MagTek.Logger
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MagTek;

[Guid("D563187D-89D3-4A50-8F91-363E55A580DD")]
public class Logger
{
	private static Logger _logger;

	private static NamedPipeServerStream pipeMagTekLogLog;

	private static Regex pipePattern;

	public static Logger Default
	{
		get
		{
			_ = _logger;
			return _logger;
		}
	}

	public event LogReceiveEvent LogReceived;

	static Logger()
	{
		pipePattern = new Regex("\\[(?<Type>\\d+),(?<Data>.+)\\]", RegexOptions.Singleline);
		_logger = new Logger();
		Process currentProcess = Process.GetCurrentProcess();
		pipeMagTekLogLog = new NamedPipeServerStream("MagTekProcess" + currentProcess.Id.ToString("X8"), PipeDirection.InOut, 100, PipeTransmissionMode.Message);
		Task.Run(async delegate
		{
			while (true)
			{
				await pipeMagTekLogLog.WaitForConnectionAsync();
				byte[] buffer = new byte[8192];
				int num = await pipeMagTekLogLog.ReadAsync(buffer, 0, 8192);
				byte[] buffer2 = new byte[2] { 32, 32 };
				await pipeMagTekLogLog.WriteAsync(buffer2, 0, 2);
				pipeMagTekLogLog.Disconnect();
				if (num > 0)
				{
					string @string = Encoding.ASCII.GetString(buffer, 0, num);
					if (pipePattern.IsMatch(@string))
					{
						Match match = pipePattern.Match(@string);
						int num2 = int.Parse(match.Groups["Type"].ToString());
						string info = match.Groups["Data"].ToString();
						LoggerFlags flags = (LoggerFlags)num2;
						Default.Log(flags, info);
					}
					else
					{
						Default.Log(LoggerFlags.LF_TRACE, @string);
					}
				}
			}
		});
	}

	public void Log(LoggerFlags Flags, string Info)
	{
		try
		{
			Task.Run(delegate
			{
				this.LogReceived?.Invoke(Flags, Info);
			});
		}
		catch
		{
		}
	}
}
