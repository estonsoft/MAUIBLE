// MTCMSNET, Version=1.0.12.1, Culture=neutral, PublicKeyToken=null
// MTCMS.MTTLVObject
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


internal class MTTLVObject
{
	private byte[] mTag;

	private byte[] mValue;

	private List<MTTLVObject> mTLVObjects;

	public MTTLVObject(string tag, string value)
	{
		initialize(MTTLVParser.getByteArrayFromHexString(tag), MTTLVParser.getByteArrayFromHexString(value));
	}

	public MTTLVObject(string tag)
	{
		initialize(MTTLVParser.getByteArrayFromHexString(tag), null);
	}

	public MTTLVObject(byte[] tag, byte[] value)
	{
		initialize(tag, value);
	}

	public MTTLVObject(byte[] tag)
	{
		initialize(tag, null);
	}

	protected void initialize(byte[] tag, byte[] value)
	{
		mTag = null;
		mValue = null;
		mTLVObjects = null;
		if (tag != null)
		{
			mTag = (byte[])tag.Clone();
		}
		if (value != null)
		{
			mValue = (byte[])value.Clone();
		}
	}

	public bool isPrimitiveObject()
	{
		return MTTLVParser.isPrimitiveTagByteArray(mTag);
	}

	public bool isConstructedObject()
	{
		return MTTLVParser.isConstructedTagByteArray(mTag);
	}

	public bool addTLVObject(MTTLVObject tlvObject)
	{
		if (mTLVObjects == null)
		{
			mTLVObjects = new List<MTTLVObject>();
		}
		if (tlvObject != null)
		{
			mTLVObjects.Add(tlvObject);
		}
		return true;
	}

	public bool removeTLVObject(MTTLVObject tlvObject)
	{
		if (mTLVObjects == null)
		{
			return false;
		}
		mTLVObjects.Remove(tlvObject);
		return true;
	}

	public string getTLVHexString()
	{
		string result = "";
		byte[] tLVByteArray = getTLVByteArray();
		if (tLVByteArray != null)
		{
			result = MTTLVParser.getHexString(tLVByteArray);
		}
		return result;
	}

	public byte[] getTLVByteArray()
	{
		byte[] valueByteArray = getValueByteArray();
		int len = 0;
		if (valueByteArray != null)
		{
			len = valueByteArray.Length;
		}
		byte[] lengthByteArray = MTTLVParser.getLengthByteArray(len);
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(mTag);
		binaryWriter.Write(lengthByteArray);
		if (valueByteArray != null)
		{
			binaryWriter.Write(valueByteArray);
		}
		return memoryStream.ToArray();
	}

	public string getTagHexString()
	{
		return MTTLVParser.getHexString(getTagByteArray());
	}

	public byte[] getTagByteArray()
	{
		return mTag;
	}

	public string getValueHexString()
	{
		string result = "";
		byte[] valueByteArray = getValueByteArray();
		if (valueByteArray != null)
		{
			result = MTTLVParser.getHexString(valueByteArray);
		}
		return result;
	}

	public string getValueTextString()
	{
		string result = "";
		if (getValueByteArray() != null)
		{
			result = Encoding.UTF8.GetString(getValueByteArray());
		}
		return result;
	}

	public byte[] getValueByteArray()
	{
		byte[] result = null;
		if (mTLVObjects != null)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			foreach (MTTLVObject mTLVObject in mTLVObjects)
			{
				binaryWriter.Write(mTLVObject.getTLVByteArray());
			}
			result = memoryStream.ToArray();
		}
		else if (mValue != null)
		{
			result = (byte[])mValue.Clone();
		}
		return result;
	}

	public List<MTTLVObject> getValueTLVObjectList()
	{
		return mTLVObjects;
	}

	public MTTLVObject findByTagHexString(string tlvTagHexString)
	{
		return findByTagByteArray(MTTLVParser.getByteArrayFromHexString(tlvTagHexString));
	}

	public MTTLVObject findByTagByteArray(byte[] tlvTagByteArray)
	{
		if (tlvTagByteArray != null)
		{
			if (mTag != null && tlvTagByteArray.SequenceEqual(mTag))
			{
				return this;
			}
			if (mTLVObjects != null)
			{
				foreach (MTTLVObject mTLVObject in mTLVObjects)
				{
					if (mTLVObject != null)
					{
						MTTLVObject mTTLVObject = mTLVObject.findByTagByteArray(tlvTagByteArray);
						if (mTTLVObject != null)
						{
							return mTTLVObject;
						}
					}
				}
			}
		}
		return null;
	}
}
