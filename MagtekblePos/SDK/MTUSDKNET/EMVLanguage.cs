// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.EMVLanguage


public class EMVLanguage
{
	public static EMVLanguage LANGUAGE_ENGLISH = new EMVLanguage(new byte[2] { 101, 110 }, "English");

	public static EMVLanguage LANGUAGE_FRENCH = new EMVLanguage(new byte[2] { 102, 114 }, "Français");

	public static EMVLanguage LANGUAGE_GERMAN = new EMVLanguage(new byte[2] { 100, 101 }, "Deutsch");

	public static EMVLanguage LANGUAGE_ITALIAN = new EMVLanguage(new byte[2] { 102, 116 }, "Italiano");

	public static EMVLanguage LANGUAGE_SPANISH = new EMVLanguage(new byte[2] { 101, 115 }, "Español");

	public static EMVLanguage LANGUAGE_CHINESE = new EMVLanguage(new byte[2] { 122, 104 }, "中文");

	public static EMVLanguage[] LANGUAGE_LIST = new EMVLanguage[6] { LANGUAGE_ENGLISH, LANGUAGE_FRENCH, LANGUAGE_GERMAN, LANGUAGE_ITALIAN, LANGUAGE_SPANISH, LANGUAGE_CHINESE };

	private byte[] m_code;

	private string m_name;

	public byte[] Code
	{
		get
		{
			return m_code;
		}
		set
		{
			m_code = ((value != null) ? ((byte[])value.Clone()) : null);
		}
	}

	public string Name
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}

	public static EMVLanguage GetLanguage(byte[] code)
	{
		EMVLanguage result = null;
		if (code != null && code.Length == 2)
		{
			for (int i = 0; i < LANGUAGE_LIST.Length; i++)
			{
				byte[] code2 = LANGUAGE_LIST[i].Code;
				if (code[0] == code2[0] && code[1] == code2[1])
				{
					result = LANGUAGE_LIST[i];
					break;
				}
			}
		}
		return result;
	}

	public EMVLanguage(byte[] code, string name)
	{
		m_code = ((code != null) ? ((byte[])code.Clone()) : null);
		m_name = name;
	}

	public EMVLanguage(EMVLanguage language)
	{
		m_code = ((language.m_code != null) ? ((byte[])language.m_code.Clone()) : null);
		m_name = language.m_name;
	}
}
