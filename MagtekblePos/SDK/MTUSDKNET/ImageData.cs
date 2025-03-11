// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.ImageData


public class ImageData
{
	public ImageType Type { get; set; }

	public byte[] Data { get; set; }

	public byte[] BackgroundColor { get; set; }

	public ImageData(ImageType type, byte[] data)
	{
		Type = type;
		Data = data;
		BackgroundColor = new byte[3] { 255, 255, 255 };
	}

	public ImageData(ImageType type, byte[] data, byte[] backgroundColor)
	{
		Type = type;
		Data = data;
		BackgroundColor = backgroundColor;
	}
}
