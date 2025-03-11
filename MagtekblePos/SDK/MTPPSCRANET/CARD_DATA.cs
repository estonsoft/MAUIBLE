// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.CARD_DATA
using System;

public struct CARD_DATA
{
	public byte DataType;

	public byte CardOperationStatus;

	public byte CardStatus;

	public byte CardType;

	public byte Track1Length;

	public byte Track2Length;

	public byte Track3Length;

	public byte EncTrack1Length;

	public byte EncTrack2Length;

	public byte EncTrack3Length;

	public byte EncMPLength;

	public byte Track1Status;

	public byte Track2Status;

	public byte Track3Status;

	public byte EncTrack1Status;

	public byte EncTrack2Status;

	public byte EncTrack3Status;

	public byte EncMPStatus;

	public byte MSStatus;

	private string _MPSTS;

	private string _Track1;

	private string _Track2;

	private string _Track3;

	private string _EncTrack1;

	private string _EncTrack2;

	private string _EncTrack3;

	private string _EncMP;

	private string _KSN;

	private string _CBCMAC;

	private string _SerialNumber;

	public byte PANInfoLength;

	private string _PANInfo;

	public uint reserved;

	public string MPSTS
	{
		get
		{
			return _MPSTS;
		}
		internal set
		{
			_MPSTS = value;
		}
	}

	public string Track1
	{
		get
		{
			return _Track1;
		}
		internal set
		{
			_Track1 = value;
		}
	}

	public string Track2
	{
		get
		{
			return _Track2;
		}
		internal set
		{
			_Track2 = value;
		}
	}

	public string Track3
	{
		get
		{
			return _Track3;
		}
		internal set
		{
			_Track3 = value;
		}
	}

	public string EncTrack1
	{
		get
		{
			return _EncTrack1;
		}
		internal set
		{
			_EncTrack1 = value;
		}
	}

	public string EncTrack2
	{
		get
		{
			return _EncTrack2;
		}
		internal set
		{
			_EncTrack2 = value;
		}
	}

	public string EncTrack3
	{
		get
		{
			return _EncTrack3;
		}
		internal set
		{
			_EncTrack3 = value;
		}
	}

	public string EncMP
	{
		get
		{
			return _EncMP;
		}
		internal set
		{
			_EncMP = value;
		}
	}

	public string KSN
	{
		get
		{
			return _KSN;
		}
		internal set
		{
			_KSN = value;
		}
	}

	public string CBCMAC
	{
		get
		{
			return _CBCMAC;
		}
		internal set
		{
			_CBCMAC = value;
		}
	}

	public string SerialNumber
	{
		get
		{
			return _SerialNumber;
		}
		set
		{
			_SerialNumber = value;
		}
	}

	public string PANInfo
	{
		get
		{
			return _PANInfo;
		}
		set
		{
			_PANInfo = value;
		}
	}

	private string getSubString(string from, string start, string end)
	{
		try
		{
			int num = from.IndexOf(start);
			int num2 = from.IndexOf(end, num + 1);
			if (num >= 0 && num2 >= 0)
			{
				return from.Substring(num, num2 - num);
			}
		}
		catch (Exception)
		{
		}
		return "";
	}

	public string getPAN()
	{
		try
		{
			string text = getSubString(Track2, ";", "=").Substring(1).Trim();
			if (text.Length > 0)
			{
				return text;
			}
			return getSubString(Track1, "%", "^").Substring(2).Trim();
		}
		catch (Exception)
		{
		}
		return "";
	}

	private string[] getISOname()
	{
		string[] array = new string[4] { "", "", "", "" };
		try
		{
			string text = getSubString(Track1, "^", "^").Substring(1);
			if (text.Contains("$"))
			{
				return null;
			}
			if (text.Contains("/"))
			{
				array = text.Split(new char[3] { '/', ' ', '.' }, 4);
			}
			else
			{
				string[] array2 = text.Split(new char[1] { ' ' }, 4);
				array[1] = array2[0];
				if (array2.Length == 2)
				{
					array[0] = array2[1];
				}
				else if (array2.Length > 2)
				{
					array[2] = array[1];
					array[0] = array[2];
					if (array2.Length == 3)
					{
						array[3] = array2[3];
					}
				}
			}
		}
		catch (Exception)
		{
		}
		return array;
	}

	private string[] getAAMVAname()
	{
		string text = "";
		string[] array = new string[4] { "", "", "", "" };
		try
		{
			if (Track1.StartsWith("%"))
			{
				int num = Track1.IndexOf("^");
				num = ((num < 16) ? (num + 1) : 16);
				int num2 = Track1.IndexOf("^", num);
				if (num2 <= num)
				{
					num2 = Track1.Length;
				}
				if (num2 - num > 35)
				{
					num2 = num + 35;
				}
				text = Track1.Substring(num, num2 - num);
			}
		}
		catch (Exception)
		{
		}
		if (text.Length > 0)
		{
			string[] array2 = text.Split(new char[1] { '$' }, 3);
			array[0] = array2[0];
			if (array2.Length > 1)
			{
				array[1] = array2[1];
			}
			if (array2.Length > 2)
			{
				array[2] = array2[2];
			}
		}
		return array;
	}

	private string[] getName()
	{
		if (CardType == 2)
		{
			return getAAMVAname();
		}
		string[] array = getISOname();
		if (array == null)
		{
			array = getAAMVAname();
		}
		return array;
	}

	public string getLastName()
	{
		string[] name = getName();
		if (name != null && name.Length != 0)
		{
			return name[0];
		}
		return "";
	}

	public string getFirstName()
	{
		string[] name = getName();
		if (name != null && name.Length > 1)
		{
			return name[1];
		}
		return "";
	}

	public string getMiddleName()
	{
		string[] name = getName();
		if (name != null && name.Length > 2)
		{
			return name[2];
		}
		return "";
	}

	public string getExpDate()
	{
		string result = "";
		try
		{
			if (Track2.StartsWith(";"))
			{
				int num = Track2.IndexOf('=');
				result = Track2.Substring(num + 1, 4);
			}
			else if (Track1.StartsWith("%"))
			{
				int num2 = Track1.LastIndexOf("^");
				result = Track1.Substring(num2 + 1, 4);
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public void clear()
	{
		_MPSTS = "";
		_Track1 = "";
		_Track2 = "";
		_Track3 = "";
		_EncTrack1 = "";
		_EncTrack2 = "";
		_EncTrack3 = "";
		_EncMP = "";
		_KSN = "";
		_CBCMAC = "";
		_SerialNumber = "";
		_PANInfo = "";
		CardType = 0;
		CardOperationStatus = 0;
		CardStatus = 0;
		Track1Status = 0;
		Track2Status = 0;
		Track3Status = 0;
		EncTrack1Status = 0;
		EncTrack2Status = 0;
		EncTrack3Status = 0;
		EncMPStatus = 0;
		Track1Length = (Track2Length = (Track3Length = (EncTrack1Length = (EncTrack2Length = (EncTrack3Length = 0)))));
		EncMPLength = (CardType = (PANInfoLength = 0));
	}

	public override string ToString()
	{
		return $"Card Type = {CardType}  |  Operation Status = {CardOperationStatus}  |  Card Status = {CardStatus}\n" + $"Track 1 ({Track1Status}) :{Track1}\n" + $"Track 2 ({Track2Status}) :{Track2}\n" + $"Track 3 ({Track3Status}) :{Track3}\n" + $"Encrypted Track 1 ({EncTrack1Status}) :{EncTrack1}\n" + $"Encrypted Track 2 ({EncTrack2Status}) :{EncTrack2}\n" + $"Encrypted Track 3 ({EncTrack3Status}) :{EncTrack3}\n" + $"Enc MP ({EncMPStatus}) :{EncMP}\n" + $"MPSTS ({MPSTS}) \n" + $"KSN ({KSN}) \n" + $"SerialNumber ({SerialNumber}) \n" + $"PAN ({PANInfo}) \n" + $"CBCMAC ({CBCMAC})";
	}

	public string ToSeparatedString(string separator)
	{
		return string.Format("CardType={0}{4}OperationStatus={1}{4}CardStatus={2}{4}DataType={3}{4}", CardType, CardOperationStatus, CardStatus, DataType, separator) + string.Format("Track1Status={0}{3}Track1Length={2}{3}Track1={1}{3}", Track1Status, Track1, Track1Length, separator) + string.Format("Track2Status={0}{3}Track2Length={2}{3}Track2={1}{3}", Track2Status, Track2, Track2Length, separator) + string.Format("Track3Status={0}{3}Track3Length={2}{3}Track3={1}{3}", Track3Status, Track3, Track3Length, separator) + string.Format("EncTrack1Status={0}{3}EncTrack1Length={2}{3}EncTrack1={1}{3}", EncTrack1Status, EncTrack1, EncTrack1Length, separator) + string.Format("EncTrack2Status={0}{3}EncTrack2Length={2}{3}EncTrack2={1}{3}", EncTrack2Status, EncTrack2, EncTrack2Length, separator) + string.Format("EncTrack3Status={0}{3}EncTrack3Length={2}{3}EncTrack3={1}{3}", EncTrack3Status, EncTrack3, EncTrack3Length, separator) + string.Format("EncMPStatus={0}{3}EncMPLength={2}{3}EncMP={1}{3}", EncMPStatus, EncMP, EncMPLength, separator) + string.Format("MPSTS={0}{2}MSStatus={1}{2}", MPSTS, MSStatus, separator) + $"KSN={KSN}{separator}" + $"SerialNumber={SerialNumber}{separator}" + $"PAN={PANInfo}{separator}" + $"CBCMAC={CBCMAC}";
	}
}
