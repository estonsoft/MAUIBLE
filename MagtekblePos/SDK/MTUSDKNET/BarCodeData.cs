// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.BarCodeData
public class BarCodeData
{
	public byte[] Data { get; set; }

	public bool Encrypted { get; set; }

	public byte EncryptionType { get; set; }

	public byte[] KSN { get; set; }

	public BarCodeData(byte[] data, bool encrypted, byte encryptionType = 0, byte[] ksn = null)
	{
		Data = data;
		Encrypted = encrypted;
		EncryptionType = encryptionType;
		KSN = ksn;
	}
}
