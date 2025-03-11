// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.DeviceInfo
public class DeviceInfo
{
	protected string mName;

	protected string mModel;

	public DeviceInfo(string name = "", string model = "")
	{
		mName = name;
		mModel = model;
	}

	public string getName()
	{
		return mName;
	}

	public string getModel()
	{
		return mModel;
	}
}
