// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.InputRequest
using System;
using System.Collections.Generic;
using System.Text;

public class InputRequest
{
	public static byte INPUT_TYPE_APPLICATION = 0;

	public static byte INPUT_TYPE_LANGUAGE = 1;

	public static byte INPUT_STATUS_COMPLETED = 0;

	public static byte INPUT_STATUS_CANCELLED = 1;

	public static byte INPUT_STATUS_TIMED_OUT = 2;

	public byte Type { get; set; }

	public byte Timeout { get; set; }

	public string Title { get; set; }

	public List<string> SelectionList { get; set; }

	public InputRequest()
	{
	}

	public InputRequest(byte[] data)
	{
		init(data);
	}

	public void init(byte[] data)
	{
		if (data != null && data.Length > 2)
		{
			Type = data[0];
			Timeout = data[1];
			List<string> selectionList = getSelectionList(data, 2);
			if (selectionList.Count > 1)
			{
				Title = selectionList[0];
				selectionList.RemoveAt(0);
				SelectionList = selectionList;
			}
		}
	}

	protected List<string> getSelectionList(byte[] data, int offset)
	{
		List<string> list = new List<string>();
		if (data != null)
		{
			int num = data.Length;
			if (num >= offset)
			{
				int num2 = offset;
				for (int i = offset; i < num; i++)
				{
					if (data[i] == 0)
					{
						int num3 = i - num2;
						if (num3 >= 0)
						{
							byte[] array = new byte[num3];
							Array.Copy(data, num2, array, 0, num3);
							string @string = Encoding.UTF8.GetString(array);
							list.Add(@string);
						}
						num2 = i + 1;
					}
				}
			}
		}
		return list;
	}
}
