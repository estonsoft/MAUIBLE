// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.PINRequest
public class PINRequest
{
	public byte Timeout { get; set; }

	public byte PINMode { get; set; }

	public byte MinLength { get; set; }

	public byte MaxLength { get; set; }

	public byte Tone { get; set; }

	public byte Format { get; set; }

	public string PAN { get; set; }

	public PINRequest()
	{
		Timeout = 60;
		PINMode = 0;
		MinLength = 4;
		MaxLength = 12;
		Tone = 1;
		Format = 0;
		PAN = "";
	}

	public PINRequest(byte timeout, byte pinMode, byte minLength, byte maxLength, byte tone, byte format, string pan = "")
	{
		Timeout = timeout;
		PINMode = pinMode;
		MinLength = minLength;
		MaxLength = maxLength;
		Tone = tone;
		Format = format;
		PAN = pan;
	}
}
