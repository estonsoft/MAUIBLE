// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.IDeviceSetup


public interface IDeviceSetup
{
	bool setDateTime(IData data);

	bool setCurrencyCode(IData data);

	bool setCurrencyExponent(IData data);

	bool setMerchantCategoryCode(IData data);

	bool setAccountType(IData data);

	bool setMerchantCustomData(IData data);
}
