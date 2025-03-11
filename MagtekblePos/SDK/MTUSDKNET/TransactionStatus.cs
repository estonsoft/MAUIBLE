// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.TransactionStatus
public enum TransactionStatus
{
	NoStatus,
	NoTransaction,
	CardSwiped,
	CardInserted,
	CardRemoved,
	CardDetected,
	CardCollision,
	TimedOut,
	HostCancelled,
	TransactionCancelled,
	TransactionInProgress,
	TransactionError,
	TransactionApproved,
	TransactionDeclined,
	TransactionCompleted,
	TransactionFailed,
	TransactionNotAccepted,
	SignatureCaptureRequested,
	TechnicalFallback,
	QuickChipDeferred,
	DataEntered,
	TryAnotherInterface
}
