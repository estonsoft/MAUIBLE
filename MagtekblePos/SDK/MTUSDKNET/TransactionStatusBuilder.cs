// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.TransactionStatusBuilder
using System;


public class TransactionStatusBuilder
{
	public static string CARD_SWIPED = "card_swiped";

	public static string CARD_INSERTED = "card_inserted";

	public static string CARD_REMOVED = "card_removed";

	public static string CARD_DETECTED = "card_detected";

	public static string CARD_COLLISION = "card_collision";

	public static string TIMED_OUT = "timed_out";

	public static string HOST_CANCELLED = "host_cancelled";

	public static string TRANSACTION_CANCELLED = "transaction_cancelled";

	public static string TRANSACTION_IN_PROGRESS = "transaction_in_progress";

	public static string TRANSACTION_ERROR = "transaction_error";

	public static string TRANSACTION_APPROVED = "transaction_approved";

	public static string TRANSACTION_DECLINED = "transaction_declined";

	public static string TRANSACTION_COMPLETED = "transaction_completed";

	public static string TRANSACTION_FAILED = "transaction_failed";

	public static string TRANSACTION_NOT_ACCEPTED = "transaction_not_accepted";

	public static string SIGNATURE_CAPTURE_REQUESTED = "signature_capture_requested";

	public static string TECHNICAL_FALLBACK = "technical_fallback";

	public static string QUICK_CHIP_DEFERRED = "quick_chip_deferred";

	public static string DATA_ENTERED = "data_entered";

	public static string TRY_ANOTHER_INTERFACE = "try_another_interface";

	public static TransactionStatus GetStatusCode(string data)
	{
		TransactionStatus result = TransactionStatus.NoStatus;
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
				if (text.CompareTo(CARD_SWIPED) == 0)
				{
					result = TransactionStatus.CardSwiped;
				}
				else if (text.CompareTo(CARD_INSERTED) == 0)
				{
					result = TransactionStatus.CardInserted;
				}
				else if (text.CompareTo(CARD_REMOVED) == 0)
				{
					result = TransactionStatus.CardRemoved;
				}
				else if (text.CompareTo(CARD_DETECTED) == 0)
				{
					result = TransactionStatus.CardDetected;
				}
				else if (text.CompareTo(CARD_COLLISION) == 0)
				{
					result = TransactionStatus.CardCollision;
				}
				else if (text.CompareTo(TIMED_OUT) == 0)
				{
					result = TransactionStatus.TimedOut;
				}
				else if (text.CompareTo(HOST_CANCELLED) == 0)
				{
					result = TransactionStatus.HostCancelled;
				}
				else if (text.CompareTo(TRANSACTION_CANCELLED) == 0)
				{
					result = TransactionStatus.TransactionCancelled;
				}
				else if (text.CompareTo(TRANSACTION_IN_PROGRESS) == 0)
				{
					result = TransactionStatus.TransactionInProgress;
				}
				else if (text.CompareTo(TRANSACTION_ERROR) == 0)
				{
					result = TransactionStatus.TransactionError;
				}
				else if (text.CompareTo(TRANSACTION_APPROVED) == 0)
				{
					result = TransactionStatus.TransactionApproved;
				}
				else if (text.CompareTo(TRANSACTION_DECLINED) == 0)
				{
					result = TransactionStatus.TransactionDeclined;
				}
				else if (text.CompareTo(TRANSACTION_COMPLETED) == 0)
				{
					result = TransactionStatus.TransactionCompleted;
				}
				else if (text.CompareTo(TRANSACTION_FAILED) == 0)
				{
					result = TransactionStatus.TransactionFailed;
				}
				else if (text.CompareTo(TRANSACTION_NOT_ACCEPTED) == 0)
				{
					result = TransactionStatus.TransactionNotAccepted;
				}
				else if (text.CompareTo(SIGNATURE_CAPTURE_REQUESTED) == 0)
				{
					result = TransactionStatus.SignatureCaptureRequested;
				}
				else if (text.CompareTo(TECHNICAL_FALLBACK) == 0)
				{
					result = TransactionStatus.TechnicalFallback;
				}
				else if (text.CompareTo(QUICK_CHIP_DEFERRED) == 0)
				{
					result = TransactionStatus.QuickChipDeferred;
				}
				else if (text.CompareTo(DATA_ENTERED) == 0)
				{
					result = TransactionStatus.DataEntered;
				}
				else if (text.CompareTo(TRY_ANOTHER_INTERFACE) == 0)
				{
					result = TransactionStatus.TryAnotherInterface;
				}
			}
			catch (Exception)
			{
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
			if (array.Length > 1)
			{
				result = array[1];
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
			if (array.Length > 2)
			{
				result = array[2];
			}
		}
		return result;
	}

	public static string GetString(TransactionStatus value)
	{
		string result = "";
		switch (value)
		{
		case TransactionStatus.CardSwiped:
			result = CARD_SWIPED;
			break;
		case TransactionStatus.CardInserted:
			result = CARD_INSERTED;
			break;
		case TransactionStatus.CardRemoved:
			result = CARD_REMOVED;
			break;
		case TransactionStatus.CardDetected:
			result = CARD_DETECTED;
			break;
		case TransactionStatus.CardCollision:
			result = CARD_COLLISION;
			break;
		case TransactionStatus.TimedOut:
			result = TIMED_OUT;
			break;
		case TransactionStatus.HostCancelled:
			result = HOST_CANCELLED;
			break;
		case TransactionStatus.TransactionCancelled:
			result = TRANSACTION_CANCELLED;
			break;
		case TransactionStatus.TransactionInProgress:
			result = TRANSACTION_IN_PROGRESS;
			break;
		case TransactionStatus.TransactionError:
			result = TRANSACTION_ERROR;
			break;
		case TransactionStatus.TransactionApproved:
			result = TRANSACTION_APPROVED;
			break;
		case TransactionStatus.TransactionDeclined:
			result = TRANSACTION_DECLINED;
			break;
		case TransactionStatus.TransactionCompleted:
			result = TRANSACTION_COMPLETED;
			break;
		case TransactionStatus.TransactionFailed:
			result = TRANSACTION_FAILED;
			break;
		case TransactionStatus.TransactionNotAccepted:
			result = TRANSACTION_NOT_ACCEPTED;
			break;
		case TransactionStatus.SignatureCaptureRequested:
			result = SIGNATURE_CAPTURE_REQUESTED;
			break;
		case TransactionStatus.TechnicalFallback:
			result = TECHNICAL_FALLBACK;
			break;
		case TransactionStatus.QuickChipDeferred:
			result = QUICK_CHIP_DEFERRED;
			break;
		case TransactionStatus.DataEntered:
			result = DATA_ENTERED;
			break;
		case TransactionStatus.TryAnotherInterface:
			result = TRY_ANOTHER_INTERFACE;
			break;
		}
		return result;
	}
}
