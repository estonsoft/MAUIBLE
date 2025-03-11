// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.BaseDeviceCapabilities
using System.Collections.Generic;


public class BaseDeviceCapabilities : IDeviceCapabilities
{
	public List<PaymentMethod> PaymentMethods { get; }

	public bool Display { get; }

	public bool PINPad { get; }

	public bool Signature { get; }

	public bool AutoSignatureCapture { get; }

	public bool SRED { get; }

	public bool MSRPowerSaver { get; }

	public bool BatteryBackedClock { get; }

	public BaseDeviceCapabilities(List<PaymentMethod> paymentMethods, bool display, bool pinPad, bool signature, bool autoSignatureCapture, bool sred, bool msrPowerSaver, bool batteryBackedClock)
	{
		PaymentMethods = paymentMethods;
		Display = display;
		PINPad = pinPad;
		Signature = signature;
		AutoSignatureCapture = autoSignatureCapture;
		SRED = sred;
		MSRPowerSaver = msrPowerSaver;
		BatteryBackedClock = batteryBackedClock;
	}
}
