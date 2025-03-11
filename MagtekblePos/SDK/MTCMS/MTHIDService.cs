// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTHIDService
using System;
using System.Collections.Generic;
using System.Linq;

using MTCMS.WindowsHID;

internal class MTHIDService : MTBaseService
{
	private static List<MTDeviceInformation> MTDeviceList;

	private object mLock = new object();

	private byte[] ODYNAMO_REPORT_HEADER = new byte[2] { 1, 5 };

	private MTHIDDevice m_hidDevice;

	private void handleCommandResponse(byte[] data)
	{
		if (m_deviceDataHandler != null)
		{
			m_deviceDataHandler(data);
		}
	}

	private byte[] handleInterruptData(byte[] data)
	{
		byte[] result = null;
		try
		{
			if (data[0] == 2)
			{
				result = data.Skip(1).ToArray();
			}
		}
		catch
		{
		}
		return result;
	}

	protected void OnFeatureReportReceived(byte[] data)
	{
		try
		{
			if (data != null && data.Length != 0)
			{
				handleCommandResponse(data);
			}
		}
		catch (Exception)
		{
		}
	}

	protected void OnInputReportReceived(byte[] data)
	{
		try
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			lock (mLock)
			{
				byte[] array = null;
				array = handleInterruptData(data);
				if (array != null && array.Length != 0)
				{
					handleCommandResponse(array);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	protected void OnStateChanged(bool isConnected)
	{
		if (isConnected)
		{
			setState(MTServiceState.Connected);
		}
		else
		{
			setState(MTServiceState.Disconnected);
		}
	}

	protected void setStateDisconnected()
	{
		setState(MTServiceState.Disconnected);
	}

	public static void requestDeviceList(Action<MTConnectionType, List<MTDeviceInformation>> deviceListEventHandler)
	{
		//ushort[] pidArray = new ushort[1] { MTDeviceConstants.PID_ODYNAMO };
		//List<HIDDeviceInformation> devices = MTHIDDevice.GetDevices(MTDeviceConstants.VID_MAGTEK, pidArray);
		//if (devices.Count > 0)
		//{
		//	List<MTDeviceInformation> list = new List<MTDeviceInformation>();
		//	int num = 0;
		//	foreach (HIDDeviceInformation item in devices)
		//	{
		//		if (item == null)
		//		{
		//			continue;
		//		}
		//		MTDeviceInformation mTDeviceInformation = new MTDeviceInformation();
		//		if (mTDeviceInformation != null)
		//		{
		//			num++;
		//			mTDeviceInformation.Id = item.InstanceId;
		//			mTDeviceInformation.Name = item.FriendlyName;
		//			mTDeviceInformation.Address = item.DevicePath;
		//			mTDeviceInformation.ProductId = item.ProductId;
		//			if (string.IsNullOrWhiteSpace(mTDeviceInformation.Name))
		//			{
		//				mTDeviceInformation.Name = "MagTek HID Device " + num;
		//			}
		//			list.Add(mTDeviceInformation);
		//		}
		//	}
		//	MTDeviceList = list;
		//	deviceListEventHandler?.Invoke(MTConnectionType.USB, list);
		//}
		//else
		//{
		//	deviceListEventHandler?.Invoke(MTConnectionType.USB, new List<MTDeviceInformation>());
		//}
	}

	public override void connect()
	{
		//if (string.IsNullOrEmpty(m_address))
		//{
		//	ushort[] pidArray = new ushort[1] { MTDeviceConstants.PID_ODYNAMO };
		//	List<HIDDeviceInformation> devices = MTHIDDevice.GetDevices(MTDeviceConstants.VID_MAGTEK, pidArray);
		//	if (devices.Count > 0)
		//	{
		//		m_address = devices[0].DevicePath;
		//	}
		//}
		//m_state = MTServiceState.Disconnected;
		//try
		//{
		//	m_hidDevice = new MTHIDDevice();
		//	m_hidDevice.FeatureReportReceived += OnFeatureReportReceived;
		//	m_hidDevice.InputReportReceived += OnInputReportReceived;
		//	m_hidDevice.StateChanged += OnStateChanged;
		//}
		//catch (Exception)
		//{
		//}
		//if (m_hidDevice != null)
		//{
		//	setState(MTServiceState.Connecting);
		//	try
		//	{
		//		setState(MTServiceState.Connecting);
		//		if (m_hidDevice.Open(m_address))
		//		{
		//			return;
		//		}
		//	}
		//	catch
		//	{
		//		setState(MTServiceState.Disconnected);
		//	}
		//}
		//setState(MTServiceState.Disconnected);
	}

	public override void disconnect()
	{
		//if (m_hidDevice != null)
		//{
		//	setState(MTServiceState.Disconnecting);
		//	try
		//	{
		//		m_hidDevice.Close();
		//	}
		//	finally
		//	{
		//		setState(MTServiceState.Disconnected);
		//		m_hidDevice = null;
		//	}
		//}
	}

	public override void sendData(byte[] data)
	{
		//if (m_hidDevice != null)
		//{
		//	try
		//	{
		//		m_hidDevice.SendData(ODYNAMO_REPORT_HEADER.Concat(data).ToArray());
		//	}
		//	catch (Exception)
		//	{
		//	}
		//}
	}

	public override void setAddress(string address)
	{
		//if (MTDeviceList != null && MTDeviceList.Count > 0)
		//{
		//	if (string.IsNullOrWhiteSpace(address))
		//	{
		//		base.setAddress(MTDeviceList[0].Address);
		//		return;
		//	}
		//	foreach (MTDeviceInformation mTDevice in MTDeviceList)
		//	{
		//		if (string.Compare(mTDevice.Name, address, StringComparison.CurrentCultureIgnoreCase) == 0)
		//		{
		//			base.setAddress(mTDevice.Address);
		//			return;
		//		}
		//		if (string.Compare(mTDevice.Address, address, StringComparison.CurrentCultureIgnoreCase) == 0)
		//		{
		//			base.setAddress(mTDevice.Address);
		//			return;
		//		}
		//	}
		}
		requestDeviceList(delegate
		{
			//if (MTDeviceList != null && MTDeviceList.Count > 0)
			//{
			//	if (string.IsNullOrWhiteSpace(address))
			//	{
			//		base.setAddress(MTDeviceList[0].Address);
			//		return;
			//	}
			//	foreach (MTDeviceInformation mTDevice2 in MTDeviceList)
			//	{
			//		if (string.Compare(mTDevice2.Name, address, StringComparison.CurrentCultureIgnoreCase) == 0)
			//		{
			//			base.setAddress(mTDevice2.Address);
			//			return;
			//		}
			//		if (string.Compare(mTDevice2.Address, address, StringComparison.CurrentCultureIgnoreCase) == 0)
			//		{
			//			base.setAddress(mTDevice2.Address);
			//			return;
			//		}
			//	}
			//}
			//base.setAddress("");
		});
	}
}
