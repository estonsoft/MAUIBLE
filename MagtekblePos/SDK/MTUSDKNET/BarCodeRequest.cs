// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.BarCodeRequest


public class BarCodeRequest
{
	public BarCodeType Type { get; set; }

	public BarCodeFormat Format { get; set; }

	public byte[] Data { get; set; }

	public byte[] BlockColor { get; set; }

	public byte[] BackgroundColor { get; set; }

	public byte ErrorCorrection { get; set; }

	public byte MaskPattern { get; set; }

	public byte MinVersion { get; set; }

	public byte MaxVersion { get; set; }

	public BarCodeRequest(BarCodeType type, BarCodeFormat format, byte[] data)
	{
		Type = type;
		Format = format;
		Data = data;
		BlockColor = new byte[3];
		BackgroundColor = new byte[3] { 255, 255, 255 };
		ErrorCorrection = 0;
		MaskPattern = byte.MaxValue;
		MinVersion = 1;
		MaxVersion = 40;
	}

	public BarCodeRequest(BarCodeType type, BarCodeFormat format, byte[] data, byte[] blockColor, byte[] backgroundColor, byte errorCorrection = 0, byte maskPattern = byte.MaxValue, byte minVersion = 1, byte maxVersion = 40)
	{
		Type = type;
		Format = format;
		Data = data;
		BlockColor = blockColor;
		BackgroundColor = backgroundColor;
		ErrorCorrection = errorCorrection;
		MaskPattern = maskPattern;
		MinVersion = minVersion;
		MaxVersion = maxVersion;
	}
}
