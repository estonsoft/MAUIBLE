// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.IDeviceCapabilities
using System.Collections.Generic;


public interface IDeviceCapabilities
{
	List<PaymentMethod> PaymentMethods { get; }

	bool Display { get; }

	bool PINPad { get; }

	bool Signature { get; }

	bool AutoSignatureCapture { get; }

	bool SRED { get; }

	bool MSRPowerSaver { get; }

	bool BatteryBackedClock { get; }
}
