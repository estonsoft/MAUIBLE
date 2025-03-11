// MTLib, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTDeviceFeatures
public class MTDeviceFeatures
{
	public bool MSR { get; set; }

	public bool Contact { get; set; }

	public bool Contactless { get; set; }

	public bool PINPad { get; set; }

	public bool MSRPowerSaver { get; set; }

	public bool BatteryBackedClock { get; set; }

	public bool SRED { get; set; }

	public bool SignatureCapture { get; set; }

	public bool ManualEntry { get; set; }

	public bool SMSR { get; set; }

	public bool NFC { get; set; }

	public MTDeviceFeatures()
	{
		MSR = false;
		Contact = false;
		Contactless = false;
		PINPad = false;
		MSRPowerSaver = false;
		BatteryBackedClock = false;
		SRED = false;
		SignatureCapture = false;
		ManualEntry = false;
		SMSR = false;
		NFC = false;
	}
}
