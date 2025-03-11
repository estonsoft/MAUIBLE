// MTLib, Version=1.0.22.1, Culture=neutral, PublicKeyToken=null
// MTLIB.MTDeviceInformation
public class MTDeviceInformation
{
	public string Id { get; set; }

	public string Name { get; set; }

	public string Address { get; set; }

	public ushort ProductId { get; set; }

	public string Model { get; set; }

	public string Serial { get; set; }

	public MTDeviceInformation()
	{
		Id = "";
		Name = "";
		Address = "";
		ProductId = 0;
		Model = "";
		Serial = "";
	}
}
