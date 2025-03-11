// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.CARD_DATA_PARSER
using System.Text;


internal class CARD_DATA_PARSER : PARSER
{
	public byte[] _cardStatus = new byte[3];

	public byte[] _cardData = new byte[1024];

	public int _cardDataLength;

	public bool dataAvailable;

	public CARD_DATA lastCardData;

	public CARD_DATA_PARSER()
	{
		lastCardData = default(CARD_DATA);
	}

	private int parseCardData(byte[] data, int dataLen, ref CARD_DATA pCard)
	{
		pCard.clear();
		int num = 0;
		int num2 = 0;
		while (dataLen > 0 && data[num] == 35)
		{
			byte b = data[num + 3];
			switch (data[num + 1])
			{
			case 1:
				pCard.Track1Length = b;
				pCard.Track1Status = data[num + 2];
				pCard.Track1 = Encoding.UTF8.GetString(data, num + 4, b);
				break;
			case 2:
				pCard.Track2Length = b;
				pCard.Track2Status = data[num + 2];
				pCard.Track2 = Encoding.UTF8.GetString(data, num + 4, b);
				break;
			case 3:
				pCard.Track3Length = b;
				pCard.Track3Status = data[num + 2];
				pCard.Track3 = Encoding.UTF8.GetString(data, num + 4, b);
				break;
			case 4:
				pCard.EncTrack1Length = b;
				pCard.EncTrack1Status = data[num + 2];
				pCard.EncTrack1 = PARSER.toHexString(data, num + 4, b);
				break;
			case 5:
				pCard.EncTrack2Length = b;
				pCard.EncTrack2Status = data[num + 2];
				pCard.EncTrack2 = PARSER.toHexString(data, num + 4, b);
				break;
			case 6:
				pCard.EncTrack3Length = b;
				pCard.EncTrack3Status = data[num + 2];
				pCard.EncTrack3 = PARSER.toHexString(data, num + 4, b);
				break;
			case 7:
				pCard.EncMPLength = b;
				pCard.EncMPStatus = data[num + 2];
				pCard.EncMP = PARSER.toHexString(data, num + 4, b);
				break;
			case 99:
				pCard.KSN = PARSER.toHexString(data, num + 4, 10);
				pCard.MPSTS = PARSER.toHexString(data, num + 14, 4);
				break;
			case 64:
				pCard.PANInfoLength = b;
				pCard.PANInfo = PARSER.toHexString(data, num + 4, b);
				break;
			case 65:
				pCard.SerialNumber = PARSER.toHexString(data, num + 4, b);
				break;
			case 100:
				pCard.CBCMAC = PARSER.toHexString(data, num + 4, b);
				break;
			}
			num += b + 4;
			num2 += b + 4;
		}
		if (num2 > 0)
		{
			pCard.DataType = 34;
			pCard.CardOperationStatus = _cardStatus[0];
			pCard.CardStatus = _cardStatus[1];
			pCard.CardType = _cardStatus[2];
			pCard.MSStatus = 0;
		}
		return num2;
	}

	public override int parse(byte[] data, int dataLen)
	{
		Status = parseCardData(data, dataLen, ref lastCardData);
		return Status;
	}

	public void reset()
	{
		_cardDataLength = 0;
	}

	public void getCardStatus(ref int operationStatus, ref int cardStatus, ref int cardType)
	{
		operationStatus = _cardStatus[0];
		cardStatus = _cardStatus[1];
		cardType = _cardStatus[2];
	}
}
