// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.AMKInfo
public struct AMKInfo
{
	public int Status;

	public string KCV;

	public string KeyLabel;

	public override string ToString()
	{
		return $"Status:{Status}, KCV:{KCV}, KeyLabel{KeyLabel}";
	}
}
