// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.MMSDeviceCapabilities
using System.Collections.Generic;


public class MMSDeviceCapabilities : BaseDeviceCapabilities
{
	public MMSDeviceCapabilities(List<PaymentMethod> paymentMethods, bool display, bool pinPad, bool signature, bool autoSignatureCapture, bool sred)
		: base(paymentMethods, display, pinPad, signature, autoSignatureCapture, sred, msrPowerSaver: false, batteryBackedClock: true)
	{
	}
}
