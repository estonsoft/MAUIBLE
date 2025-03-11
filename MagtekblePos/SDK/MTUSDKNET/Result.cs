// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.Result


public class Result : IResult
{
	public StatusCode Status { get; set; }

	public IData Data { get; set; }

	public Result(StatusCode status)
	{
		Status = status;
		Data = null;
	}

	public Result(StatusCode status, IData data)
	{
		Status = status;
		Data = null;
		if (data != null)
		{
			Data = data.Clone();
		}
	}
}
