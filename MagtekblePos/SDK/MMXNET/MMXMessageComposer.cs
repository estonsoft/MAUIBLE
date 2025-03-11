// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXMessageComposer
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


internal class MMXMessageComposer
{
	private const byte PACKET_TYPE_SINGLE_DATA = 0;

	private const byte PACKET_TYPE_START_DATA = 1;

	private const byte PACKET_TYPE_CONTINUE_DATA = 2;

	private const byte PACKET_TYPE_END_DATA = 3;

	private const byte PACKET_TYPE_CANCEL = 4;

	private int START_PAYLOAD_SIZE = 59;

	private int PACKET_CONTINUE_DATA_SIZE = 61;

	private int END_DATA_SIZE = 62;

	private int SINGLE_DATA_SIZE = 62;

	private int PacketSize = 64;

	protected IMMXService mService;

	private byte[] BytesLeft;

	private SemaphoreSlim sendingLock = new SemaphoreSlim(0, 1);

	private bool cancelIssued;

	private Dictionary<ushort, byte[]> packs = new Dictionary<ushort, byte[]>();

	private ulong pack_total_size;

	public event EventHandler<byte[]> PayloadReceived;

	public event EventHandler<string> OnError;

	public MMXMessageComposer(IMMXService Service)
	{
		mService = Service;
		if (mService != null)
		{
			PacketSize = 64;
			START_PAYLOAD_SIZE = PacketSize - 5;
			PACKET_CONTINUE_DATA_SIZE = PacketSize - 3;
			END_DATA_SIZE = PacketSize - 2;
			SINGLE_DATA_SIZE = PacketSize - 2;
		}
	}

	public MMXMessageComposer()
	{
	}

	~MMXMessageComposer()
	{
	}

	protected void SendReceived(byte[] Data)
	{
		Debug.LogCommunication(" <- " + BitConverter.ToString(Data));
		this.PayloadReceived?.Invoke(this, Data);
	}

	public void SendCancel()
	{
		if (mService != null)
		{
			mService.sendData(new byte[3] { 4, 0, 0 });
			cancelIssued = true;
		}
	}

	public void SendPayload(byte[] Data, Action<float> ProgressUpdate = null)
	{
		Debug.LogCommunication(" -> " + BitConverter.ToString(Data));
		cancelIssued = false;
		List<byte[]> list = _buildPackages(Data);
		for (int i = 0; i < list.Count; i++)
		{
			if (mService != null)
			{
				if (cancelIssued)
				{
					Debug.LogCommunication("Transfering canceled");
					break;
				}
				mService.sendData(list[i]);
				try
				{
					ProgressUpdate?.Invoke((float)(i + 1) / (float)(list.Count + 1));
				}
				catch
				{
				}
			}
		}
	}

	public void ParseReport(byte[] Data, out byte[] Rest)
	{
		Debug.LogProtocol("Parsing - " + BitConverter.ToString(Data));
		if (Data[0] == 0)
		{
			byte b = Data[1];
			byte[] data = Data.Skip(2).Take(b).ToArray();
			SendReceived(data);
			Rest = Data.Skip(2 + b).ToArray();
		}
		else if (Data[0] == 1)
		{
			packs.Clear();
			pack_total_size = (ulong)((Data[1] << 24) + (Data[2] << 16) + (Data[3] << 8) + Data[4]);
			packs[0] = Data.Skip(5).Take(START_PAYLOAD_SIZE).ToArray();
			Rest = Data.Skip(PacketSize).ToArray();
		}
		else if (Data[0] == 2)
		{
			if (packs.Count <= 0)
			{
				this.OnError?.Invoke(this, "Missing start package.");
			}
			else
			{
				ushort key = (ushort)((Data[1] << 8) + Data[2]);
				packs[key] = Data.Skip(3).Take(PACKET_CONTINUE_DATA_SIZE).ToArray();
			}
			Rest = Data.Skip(PacketSize).ToArray();
		}
		else if (Data[0] == 3)
		{
			ushort num = (ushort)packs.Count;
			byte b2 = Data[1];
			if (num < 1)
			{
				this.OnError?.Invoke(this, "No start package but receive end.");
			}
			else
			{
				packs[num] = Data.Skip(2).Take(b2).ToArray();
				List<byte> list = new List<byte>();
				try
				{
					for (ushort num2 = 0; num2 <= num; num2++)
					{
						list.AddRange(packs[num2]);
					}
					SendReceived(list.ToArray());
					packs.Clear();
				}
				catch
				{
					this.OnError?.Invoke(this, "Missing package");
				}
			}
			Rest = Data.Skip(2 + b2).ToArray();
		}
		else
		{
			if (Data[0] == 4)
			{
				int num3 = ((Data.Length > 2) ? ((Data[1] << 8) + Data[2]) : (-1));
				Rest = null;
				throw new OperationCanceledException($"data transfering is canceled. {num3:X4}");
			}
			Rest = null;
			this.OnError?.Invoke(this, "invalid report identifier.");
		}
	}

	private byte[] _buildSingleMessage(byte[] payload)
	{
		List<byte> list = new List<byte>();
		list.Add(0);
		list.Add((byte)payload.Count());
		list.AddRange(payload);
		return list.ToArray();
	}

	private List<byte[]> _buildBigBlocks(byte[] payload)
	{
		List<byte[]> list = new List<byte[]>();
		List<byte> list2 = new List<byte>();
		list2.Add(1);
		list2.AddRange(BitConverter.GetBytes(payload.Count()).Reverse().ToArray());
		list2.AddRange(payload.Take(START_PAYLOAD_SIZE));
		list.Add(list2.ToArray());
		ushort num = 1;
		int i;
		for (i = START_PAYLOAD_SIZE; i < payload.Count() - END_DATA_SIZE; i += PACKET_CONTINUE_DATA_SIZE)
		{
			List<byte> list3 = new List<byte>();
			list3.Add(2);
			list3.AddRange(BitConverter.GetBytes(num).Reverse().ToArray());
			list3.AddRange(payload.Skip(i).Take(PACKET_CONTINUE_DATA_SIZE));
			list.Add(list3.ToArray());
			num++;
		}
		List<byte> list4 = new List<byte>();
		list4.Add(3);
		list4.Add((byte)(payload.Count() - i));
		list4.AddRange(payload.Skip(i));
		list.Add(list4.ToArray());
		return list;
	}

	private List<byte[]> _buildPackages(byte[] payload)
	{
		if (payload.Count() > SINGLE_DATA_SIZE)
		{
			return _buildBigBlocks(payload);
		}
		return new List<byte[]> { _buildSingleMessage(payload) };
	}
}
