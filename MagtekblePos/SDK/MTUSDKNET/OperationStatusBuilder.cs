// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.OperationStatusBuilder
using System;


public class OperationStatusBuilder
{
	public static string OPERATION_STARTED = "operation_started";

	public static string OPERATION_WARNING = "operation_warning";

	public static string OPERATION_FAILED = "operation_failed";

	public static string OPERATION_DONE = "operation_done";

	public static OperationStatus GetStatusCode(string data)
	{
		OperationStatus result = OperationStatus.NoStatus;
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
				if (text.CompareTo(OPERATION_STARTED) == 0)
				{
					result = OperationStatus.Started;
				}
				else if (text.CompareTo(OPERATION_WARNING) == 0)
				{
					result = OperationStatus.Warning;
				}
				else if (text.CompareTo(OPERATION_FAILED) == 0)
				{
					result = OperationStatus.Failed;
				}
				else if (text.CompareTo(OPERATION_DONE) == 0)
				{
					result = OperationStatus.Done;
				}
			}
			catch (Exception)
			{
			}
		}
		return result;
	}

	public static string GetOperationDetail(string data)
	{
		string result = "";
		if (data.Contains(","))
		{
			string[] array = data.Split(new char[1] { ',' });
			if (array.Length > 1)
			{
				result = array[1];
			}
		}
		return result;
	}

	public static string GetStatusDetail(string data)
	{
		string result = "";
		if (data.Contains(","))
		{
			string[] array = data.Split(new char[1] { ',' });
			if (array.Length > 2)
			{
				result = array[2];
			}
		}
		return result;
	}

	public static string GetDeviceDetail(string data)
	{
		string result = "";
		if (data.Contains(","))
		{
			string[] array = data.Split(new char[1] { ',' });
			if (array.Length > 3)
			{
				result = array[3];
			}
		}
		return result;
	}

	public static string GetString(OperationStatus value)
	{
		string result = "";
		switch (value)
		{
		case OperationStatus.Started:
			result = OPERATION_STARTED;
			break;
		case OperationStatus.Warning:
			result = OPERATION_WARNING;
			break;
		case OperationStatus.Failed:
			result = OPERATION_FAILED;
			break;
		case OperationStatus.Done:
			result = OPERATION_DONE;
			break;
		}
		return result;
	}
}
