// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.UserEventBuilder
using System;


public class UserEventBuilder
{
	public static string CONTACTLESS_CARD_PRESENTED = "contactless_card_presented";

	public static string CONTACTLESS_CARD_REMOVED = "contactless_card_removed";

	public static string CARD_SEATED = "card_seated";

	public static string CARD_UNSEATED = "card_unseated";

	public static string CARD_SWIPED = "card_swiped";

	public static string TOUCH_PRESENTED = "touch_presented";

	public static string TOUCH_REMOVED = "touch_removed";

	public static UserEvent GetValue(string valueString)
	{
		UserEvent result = UserEvent.None;
		if (valueString != null)
		{
			try
			{
				if (valueString.CompareTo(CONTACTLESS_CARD_PRESENTED) == 0)
				{
					result = UserEvent.ContactlessCardPresented;
				}
				else if (valueString.CompareTo(CONTACTLESS_CARD_REMOVED) == 0)
				{
					result = UserEvent.ContactlessCardRemoved;
				}
				else if (valueString.CompareTo(CARD_SEATED) == 0)
				{
					result = UserEvent.CardSeated;
				}
				else if (valueString.CompareTo(CARD_UNSEATED) == 0)
				{
					result = UserEvent.CardUnseated;
				}
				else if (valueString.CompareTo(CARD_SWIPED) == 0)
				{
					result = UserEvent.CardSwiped;
				}
				else if (valueString.CompareTo(TOUCH_PRESENTED) == 0)
				{
					result = UserEvent.TouchPresented;
				}
				else if (valueString.CompareTo(TOUCH_REMOVED) == 0)
				{
					result = UserEvent.TouchRemoved;
				}
			}
			catch (Exception)
			{
			}
		}
		return result;
	}

	public static string GetString(UserEvent value)
	{
		string result = "";
		switch (value)
		{
		case UserEvent.ContactlessCardPresented:
			result = CONTACTLESS_CARD_PRESENTED;
			break;
		case UserEvent.ContactlessCardRemoved:
			result = CONTACTLESS_CARD_REMOVED;
			break;
		case UserEvent.CardSeated:
			result = CARD_SEATED;
			break;
		case UserEvent.CardUnseated:
			result = CARD_UNSEATED;
			break;
		case UserEvent.CardSwiped:
			result = CARD_SWIPED;
			break;
		case UserEvent.TouchPresented:
			result = TOUCH_PRESENTED;
			break;
		case UserEvent.TouchRemoved:
			result = TOUCH_REMOVED;
			break;
		}
		return result;
	}
}
