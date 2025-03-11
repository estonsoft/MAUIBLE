// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.FeatureStatusBuilder
using System;


public class FeatureStatusBuilder
{
	public static string FEATURE_SIGNATURE_CAPTURE = "feature_signature_capture";

	public static string FEATURE_PIN_ENTRY = "feature_pin_entry";

	public static string FEATURE_PAN_ENTRY = "feature_pan_entry";

	public static string FEATURE_SHOW_BAR_CODE = "feature_show_bar_code";

	public static string FEATURE_SCAN_BAR_CODE = "feature_scan_bar_code";

	public static string FEATURE_DISPLAY_MESSAGE = "feature_display_message";

	public static string STATUS_SUCCESS = "status_success";

	public static string STATUS_FAILED = "status_failed";

	public static string STATUS_TIMED_OUT = "status_timed_out";

	public static string STATUS_CANCELLED = "status_cancelled";

	public static string STATUS_ERROR = "status_error";

	public static string STATUS_HARDWARE_NA = "status_hardware_na";

	public static DeviceFeature GetDeviceFeature(string data)
	{
		DeviceFeature result = DeviceFeature.None;
		string text = data;
		if (data.Contains(","))
		{
			string[] array = data.Split(new char[1] { ',' });
			if (array.Length != 0)
			{
				text = array[0];
			}
		}
		if (text != null)
		{
			try
			{
				if (text.CompareTo(FEATURE_SIGNATURE_CAPTURE) == 0)
				{
					result = DeviceFeature.SignatureCapture;
				}
				else if (text.CompareTo(FEATURE_PIN_ENTRY) == 0)
				{
					result = DeviceFeature.PINEntry;
				}
				else if (text.CompareTo(FEATURE_PAN_ENTRY) == 0)
				{
					result = DeviceFeature.PANEntry;
				}
				else if (text.CompareTo(FEATURE_SHOW_BAR_CODE) == 0)
				{
					result = DeviceFeature.ShowBarCode;
				}
				else if (text.CompareTo(FEATURE_SCAN_BAR_CODE) == 0)
				{
					result = DeviceFeature.ScanBarCode;
				}
				else if (text.CompareTo(FEATURE_DISPLAY_MESSAGE) == 0)
				{
					result = DeviceFeature.DisplayMessage;
				}
			}
			catch (Exception)
			{
			}
		}
		return result;
	}

	public static FeatureStatus GetFeatureStatus(string data)
	{
		FeatureStatus result = FeatureStatus.NoStatus;
		string text = "";
		if (data.Contains(","))
		{
			string[] array = data.Split(new char[1] { ',' });
			if (array.Length > 1)
			{
				text = array[1];
			}
		}
		if (text != null)
		{
			try
			{
				if (text.CompareTo(STATUS_SUCCESS) == 0)
				{
					result = FeatureStatus.Success;
				}
				else if (text.CompareTo(STATUS_FAILED) == 0)
				{
					result = FeatureStatus.Failed;
				}
				else if (text.CompareTo(STATUS_TIMED_OUT) == 0)
				{
					result = FeatureStatus.TimedOut;
				}
				else if (text.CompareTo(STATUS_CANCELLED) == 0)
				{
					result = FeatureStatus.Cancelled;
				}
				else if (text.CompareTo(STATUS_ERROR) == 0)
				{
					result = FeatureStatus.Error;
				}
				else if (text.CompareTo(STATUS_HARDWARE_NA) == 0)
				{
					result = FeatureStatus.HardwareNA;
				}
			}
			catch (Exception)
			{
			}
		}
		return result;
	}

	public static string GetFeatureString(DeviceFeature value)
	{
		string result = "";
		switch (value)
		{
		case DeviceFeature.SignatureCapture:
			result = FEATURE_SIGNATURE_CAPTURE;
			break;
		case DeviceFeature.PINEntry:
			result = FEATURE_PIN_ENTRY;
			break;
		case DeviceFeature.PANEntry:
			result = FEATURE_PAN_ENTRY;
			break;
		case DeviceFeature.ShowBarCode:
			result = FEATURE_SHOW_BAR_CODE;
			break;
		case DeviceFeature.ScanBarCode:
			result = FEATURE_SCAN_BAR_CODE;
			break;
		case DeviceFeature.DisplayMessage:
			result = FEATURE_DISPLAY_MESSAGE;
			break;
		}
		return result;
	}

	public static string GetStatusString(FeatureStatus value)
	{
		string result = "";
		switch (value)
		{
		case FeatureStatus.Success:
			result = STATUS_SUCCESS;
			break;
		case FeatureStatus.Failed:
			result = STATUS_FAILED;
			break;
		case FeatureStatus.TimedOut:
			result = STATUS_TIMED_OUT;
			break;
		case FeatureStatus.Cancelled:
			result = STATUS_CANCELLED;
			break;
		case FeatureStatus.Error:
			result = STATUS_ERROR;
			break;
		case FeatureStatus.HardwareNA:
			result = STATUS_HARDWARE_NA;
			break;
		}
		return result;
	}
}
