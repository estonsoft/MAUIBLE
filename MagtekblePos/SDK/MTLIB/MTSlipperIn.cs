// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MTLIB.MTSlipperIn
using System;
using System.Collections.Generic;
using System.Linq;

public class MTSlipperIn
{
	private const byte SLIP_BYTE = 192;

	private bool start;

	private List<byte> Buffer = new List<byte>();

	private object lockObject = new object();

	public event EventHandler<byte[]> OnDataReady;

	public void Add(byte[] data)
	{
		lock (lockObject)
		{
			try
			{
				Buffer.AddRange(data);
				while (Buffer.Count > 0)
				{
					int num = Buffer.FindIndex((byte b) => b == 192);
					if (num < 0)
					{
						break;
					}
					start = !start;
					if (!start)
					{
						if (num == 0)
						{
							Buffer.RemoveAt(0);
							start = true;
							continue;
						}
						byte[] array = Unslip(Buffer.Take(num));
						if (array.Length != 0 && this.OnDataReady != null)
						{
							this.OnDataReady(this, array);
						}
						Buffer.RemoveRange(0, num + 1);
					}
					else
					{
						Buffer.RemoveRange(0, num + 1);
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public void Clear()
	{
		Buffer.Clear();
		start = false;
	}

	private byte[] Unslip(IEnumerable<byte> data)
	{
		List<byte> list = new List<byte>();
		bool flag = false;
		foreach (byte datum in data)
		{
			if (datum == 219)
			{
				flag = true;
				continue;
			}
			if (flag && datum == 220)
			{
				list.Add(192);
			}
			else if (flag && datum == 221)
			{
				list.Add(219);
			}
			else
			{
				list.Add(datum);
			}
			flag = false;
		}
		return list.ToArray();
	}
}
