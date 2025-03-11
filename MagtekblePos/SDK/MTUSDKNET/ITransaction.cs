// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.ITransaction
using System.Collections.Generic;


public interface ITransaction
{
	byte Timeout { get; set; }

	List<PaymentMethod> PaymentMethods { get; set; }

	bool QuickChip { get; set; }

	bool EMVOnly { get; set; }

	bool PreventMSRSignatureForCardWithICC { get; set; }

	bool SuppressThankYouMessage { get; set; }

	byte OverrideFinalTransactionMessage { get; set; }

	byte EMVResponseFormat { get; set; }

	byte TransactionType { get; set; }

	string Amount { get; set; }

	string CashBack { get; set; }

	byte[] CurrencyCode { get; set; }

	byte[] CurrencyExponent { get; set; }

	byte[] TransactionCategory { get; set; }

	byte[] MerchantCategory { get; set; }

	byte[] MerchantID { get; set; }

	byte[] MerchantCustomData { get; set; }

	byte ManualEntryType { get; set; }

	byte ManualEntryFormat { get; set; }

	byte ManualEntrySound { get; set; }
}
