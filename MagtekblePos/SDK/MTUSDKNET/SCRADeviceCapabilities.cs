// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.SCRADeviceCapabilities


public class SCRADeviceCapabilities : BaseDeviceCapabilities
{
	public SCRADeviceCapabilities(bool msr, bool contact, bool contactless, bool manual, bool msrPowerSaver, bool batteryBackedClock)
		: base(TransactionBuilder.GetPaymentMethods(msr, contact, contactless, manual), display: false, pinPad: false, signature: false, autoSignatureCapture: false, sred: false, msrPowerSaver, batteryBackedClock)
	{
	}
}
