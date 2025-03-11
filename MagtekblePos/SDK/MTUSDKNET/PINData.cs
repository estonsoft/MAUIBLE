// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PINData
public class PINData
{
	public byte[] PINBlock { get; set; }

	public byte[] KSN { get; set; }

	public byte Format { get; set; }

	public byte EncryptionType { get; set; }

	public PINData(byte[] pinBlock, byte[] ksn, byte format, byte encryptionType)
	{
		PINBlock = pinBlock;
		KSN = ksn;
		Format = format;
		EncryptionType = encryptionType;
	}
}
