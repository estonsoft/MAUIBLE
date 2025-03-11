// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.CertificateInfo
public class CertificateInfo
{
	protected string mFormat;

	protected byte[] mData;

	protected string mPassword;

	public CertificateInfo(string format, byte[] data, string password)
	{
		mFormat = format;
		mData = data;
		mPassword = password;
	}

	public string getFormat()
	{
		return mFormat;
	}

	public byte[] getData()
	{
		return mData;
	}

	public string getPassword()
	{
		return mPassword;
	}
}
