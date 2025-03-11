// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PANData


public class PANData
{
	public byte[] Data { get; set; }

	public byte[] KSN { get; set; }

	public byte EncryptionType { get; set; }

	public PINData PINData { get; set; }

	public PANData(byte[] data, byte[] ksn, byte encryptionType, PINData pinData = null)
	{
		Data = data;
		KSN = ksn;
		EncryptionType = encryptionType;
		PINData = pinData;
	}
}
