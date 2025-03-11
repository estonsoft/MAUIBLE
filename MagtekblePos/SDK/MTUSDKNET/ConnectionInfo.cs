// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.ConnectionInfo


public class ConnectionInfo
{
	protected DeviceType mDeviceType;

	protected ConnectionType mConnectionType;

	protected string mAddress;

	protected CertificateInfo mCertificateInfo;

	public ConnectionInfo(DeviceType deviceType, ConnectionType connectionType, string address)
	{
		mDeviceType = deviceType;
		mConnectionType = connectionType;
		mAddress = address;
		mCertificateInfo = null;
	}

	public ConnectionInfo(DeviceType deviceType, ConnectionType connectionType, string address, CertificateInfo certificateInfo)
	{
		mDeviceType = deviceType;
		mConnectionType = connectionType;
		mAddress = address;
		mCertificateInfo = certificateInfo;
	}

	public void setCertificateInfo(CertificateInfo certificateInfo)
	{
		mCertificateInfo = certificateInfo;
	}

	public DeviceType getDeviceType()
	{
		return mDeviceType;
	}

	public ConnectionType getConnectionType()
	{
		return mConnectionType;
	}

	public string getAddress()
	{
		return mAddress;
	}

	public CertificateInfo getCertificateInfo()
	{
		return mCertificateInfo;
	}
}
