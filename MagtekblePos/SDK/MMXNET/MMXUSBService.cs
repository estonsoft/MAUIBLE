// MMXNET, Version=1.0.0.15, Culture=neutral, PublicKeyToken=null
// MMXNET.MMXUSBService
using System;
using System.Collections.Generic;
using System.Linq;

using MTLIB.WindowsHID;

internal class MMXUSBService : MMXBaseService
{
	private static readonly ushort[] ApolloPIDs = new ushort[2] { 8224, 8227 };

	private static readonly byte ApolloReportID = 0;

	private List<Tuple<string, Func<Hid>>> DeviceList;

	private Hid hid;

	private object sendingLock = new object();

	private static List<Tuple<string, string, Func<Hid>>> getAllDevices()
	{
		List<Tuple<string, string, Func<Hid>>> list = new List<Tuple<string, string, Func<Hid>>>();
		ushort[] apolloPIDs = ApolloPIDs;
		foreach (ushort pid in apolloPIDs)
		{
			list.AddRange(Hid.getList(pid));
		}
		return list;
	}

	public static string[] GetDeviceList()
	{
		new List<Tuple<string, string, Func<Hid>>>();
		try
		{
			return (from x in getAllDevices()
				select x.Item1 + "|" + x.Item2).ToArray();
		}
		catch
		{
		}
		return new string[0];
	}

	public override void connect()
	{
		try
		{
			SetConnectionState(MMXConnectionState.Connecting);
			List<Tuple<string, string, Func<Hid>>> allDevices = getAllDevices();
			if (string.IsNullOrWhiteSpace(_deviceId) && allDevices != null && allDevices.Count > 0)
			{
				_deviceId = allDevices[0].Item2;
			}
			if (!string.IsNullOrWhiteSpace(_deviceId))
			{
				Func<Hid> item = allDevices.First((Tuple<string, string, Func<Hid>> x) => x.Item2 == _deviceId).Item3;
				hid = item();
				if (hid != null)
				{
					hid.InputReceived += Hid_InputReceived;
					hid.OnConnected += Hid_OnConnected;
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

	private void Hid_OnConnected(object sender, bool e)
	{
		SetConnectionState(e ? MMXConnectionState.Connected : MMXConnectionState.Disconnected);
	}

	private void Hid_InputReceived(object sender, byte[] e)
	{
		FireOnData(e.Skip(1));
	}

	public override void disconnect()
	{
		SetConnectionState(MMXConnectionState.Disconnecting);
		if (hid != null)
		{
			hid.Close();
			hid = null;
		}
		SetConnectionState(MMXConnectionState.Disconnected);
	}

	public override void sendData(byte[] data)
	{
		lock (sendingLock)
		{
			hid.Send(new byte[1] { ApolloReportID }.Concat(data).ToArray());
		}
	}

	public override string[] getDeviceList()
	{
		return GetDeviceList();
	}

	public override bool isConneted()
	{
		if (hid != null)
		{
			return hid.IsOpen;
		}
		return false;
	}
}
