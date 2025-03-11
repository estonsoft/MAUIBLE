// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.Transaction
using System.Collections.Generic;


public class Transaction : ITransaction
{
	public byte Timeout { get; set; }

	public List<PaymentMethod> PaymentMethods { get; set; }

	public bool QuickChip { get; set; }

	public bool EMVOnly { get; set; }

	public bool PreventMSRSignatureForCardWithICC { get; set; }

	public bool SuppressThankYouMessage { get; set; }

	public byte OverrideFinalTransactionMessage { get; set; }

	public byte EMVResponseFormat { get; set; }

	public byte TransactionType { get; set; }

	public string Amount { get; set; }

	public string CashBack { get; set; }

	public byte[] CurrencyCode { get; set; }

	public byte[] CurrencyExponent { get; set; }

	public byte[] TransactionCategory { get; set; }

	public byte[] MerchantCategory { get; set; }

	public byte[] MerchantID { get; set; }

	public byte[] MerchantCustomData { get; set; }

	public byte ManualEntryType { get; set; }

	public byte ManualEntryFormat { get; set; }

	public byte ManualEntrySound { get; set; }

	public Transaction()
	{
		init();
	}

	public Transaction(List<PaymentMethod> paymentMethods, string amount, string cashBack, bool quickChip, bool emvOnly)
	{
		init();
		PaymentMethods = paymentMethods;
		Amount = amount;
		CashBack = cashBack;
		QuickChip = quickChip;
		EMVOnly = emvOnly;
	}

	public Transaction(byte timeout, List<PaymentMethod> paymentMethods, string amount, string cashBack, bool quickChip, bool emvOnly, byte transactionType)
	{
		init();
		Timeout = timeout;
		PaymentMethods = paymentMethods;
		Amount = amount;
		CashBack = cashBack;
		QuickChip = quickChip;
		EMVOnly = emvOnly;
		TransactionType = transactionType;
	}

	protected void init()
	{
		Timeout = 60;
		QuickChip = true;
		EMVOnly = true;
		EMVResponseFormat = 0;
		PreventMSRSignatureForCardWithICC = false;
		SuppressThankYouMessage = false;
		OverrideFinalTransactionMessage = 0;
		TransactionType = 0;
		CurrencyCode = null;
		CurrencyExponent = null;
		TransactionCategory = null;
		MerchantCategory = null;
		MerchantID = null;
		MerchantCustomData = null;
		ManualEntryType = 0;
		ManualEntryFormat = 0;
		ManualEntrySound = 0;
	}
}
