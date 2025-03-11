// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MTLIB.MTSliperOut
using System.Collections.Generic;

public class MTSliperOut
{
	public byte[] Convert(IEnumerable<byte> data)
	{
		List<byte> list = new List<byte>();
		list.Add(192);
		foreach (byte datum in data)
		{
			switch (datum)
			{
			case 192:
				list.Add(219);
				list.Add(220);
				break;
			case 219:
				list.Add(219);
				list.Add(221);
				break;
			default:
				list.Add(datum);
				break;
			}
		}
		list.Add(192);
		return list.ToArray();
	}
}
