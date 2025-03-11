// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.TransactionBuilder
using System.Collections.Generic;


public class TransactionBuilder
{
	public static List<PaymentMethod> GetPaymentMethods(bool msr, bool contact, bool contactless, bool manual)
	{
		List<PaymentMethod> list = new List<PaymentMethod>();
		if (msr)
		{
			list.Add(PaymentMethod.MSR);
		}
		if (contact)
		{
			list.Add(PaymentMethod.Contact);
		}
		if (contactless)
		{
			list.Add(PaymentMethod.Contactless);
		}
		if (manual)
		{
			list.Add(PaymentMethod.ManualEntry);
		}
		return list;
	}
}
