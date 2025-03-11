// MTLib, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.IMTEMV
public interface IMTEMV
{
	int startTransaction(byte timeLimit, byte cardType, byte option, byte[] amount, byte transactionType, byte[] cashBack, byte[] currencyCode, byte reportingOption);

	int setUserSelectionResult(byte status, byte selection);

	int setAcquirerResponse(byte[] response);

	int cancelTransaction();

	int sendExtendedCommand(string command);
}
