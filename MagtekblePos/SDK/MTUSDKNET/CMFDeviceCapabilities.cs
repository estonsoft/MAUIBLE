// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.CMFDeviceCapabilities


public class CMFDeviceCapabilities : BaseDeviceCapabilities
{
	public CMFDeviceCapabilities()
		: base(TransactionBuilder.GetPaymentMethods(msr: true, contact: true, contactless: false, manual: false), display: false, pinPad: false, signature: false, autoSignatureCapture: false, sred: false, msrPowerSaver: false, batteryBackedClock: true)
	{
	}
}
