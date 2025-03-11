// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.IPADCapability
using System.Collections.Generic;


internal class IPADCapability
{
	private Dictionary<string, int> capabilities = new Dictionary<string, int>();

	private string _Capability;

	public string Capability
	{
		get
		{
			return _Capability;
		}
		set
		{
			capabilities.Clear();
			_Capability = value;
			if (string.IsNullOrWhiteSpace(value))
			{
				return;
			}
			string[] array = _Capability.ToUpper().Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split('=');
				if (array2.Length >= 2)
				{
					capabilities.Add(array2[0], int.Parse(array2[1]));
				}
			}
		}
	}

	public IPADCapabilityValue SignatureCapture => GetValue("SC");

	public IPADCapabilityValue Version => GetValue("V", IPADCapabilityValue.KernelVersion5);

	public IPADCapabilityValue SRED => GetValue("SR", IPADCapabilityValue.NotSupported);

	public IPADCapabilityValue TokenReversal => GetValue("TR", IPADCapabilityValue.NotSupported);

	public IPADCapabilityValue MagneSafe2 => GetValue("MS2");

	public IPADCapabilityValue PINFixedKey => GetValue("PFK", IPADCapabilityValue.NotSupported);

	public IPADCapabilityValue UserDataEntry => GetValue("UDE", IPADCapabilityValue.L2);

	public IPADCapabilityValue ContactEMV => GetValue("CE", IPADCapabilityValue.L2);

	public IPADCapabilityValue ContactlessEMV => GetValue("CLE", IPADCapabilityValue.L2);

	public IPADCapabilityValue DelayResponse => GetValue("DR", IPADCapabilityValue.NotSupported);

	private IPADCapabilityValue GetValue(string Key, IPADCapabilityValue DefaultValue = IPADCapabilityValue.L1)
	{
		if (capabilities.ContainsKey(Key.ToUpper()))
		{
			return (IPADCapabilityValue)capabilities[Key.ToUpper()];
		}
		return DefaultValue;
	}
}
