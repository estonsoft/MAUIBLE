// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// MTUSDKNET, Version=1.1.9.0, Culture=neutral, PublicKeyToken=null
// MTUSDKNET.TLVObject
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class TLVObject
{
	protected byte[] mTag;

	protected byte[] mValue;

	protected List<TLVObject> mTLVObjects;

	public TLVObject()
	{
	}

	public TLVObject(string tag, string value)
	{
		initialize(TLVParser.getByteArrayFromHexString(tag), TLVParser.getByteArrayFromHexString(value));
	}

	public TLVObject(string tag)
	{
		initialize(TLVParser.getByteArrayFromHexString(tag), null);
	}

	public TLVObject(byte tagByte, byte[] value)
	{
		initialize(new byte[1] { tagByte }, value);
	}

	public TLVObject(byte[] tagBytes, byte[] valueBytes)
	{
		initialize(tagBytes, valueBytes);
	}

	public TLVObject(byte tagByte)
	{
		initialize(new byte[1] { tagByte }, null);
	}

	public TLVObject(byte tagByte, byte valueByte)
	{
		initialize(new byte[1] { tagByte }, new byte[1] { valueByte });
	}

	public TLVObject(byte[] tagBytes)
	{
		initialize(tagBytes, null);
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
		return TLVParser.isPrimitiveTagByteArray(mTag);
	}

	public bool isConstructedObject()
	{
		return TLVParser.isConstructedTagByteArray(mTag);
	}

	public bool addTLVObject(TLVObject tlvObject)
	{
		if (mTLVObjects == null)
		{
			mTLVObjects = new List<TLVObject>();
		}
		if (tlvObject != null)
		{
			mTLVObjects.Add(tlvObject);
		}
		return true;
	}

	public bool removeTLVObject(TLVObject tlvObject)
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
			result = TLVParser.getHexString(tLVByteArray);
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
		byte[] lengthByteArray = TLVParser.getLengthByteArray(len);
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
		return TLVParser.getHexString(getTagByteArray());
	}

	public byte[] getTagByteArray()
	{
		return mTag;
	}

	public string getValueHexString()
	{
		return TLVParser.getHexString(getValueByteArray());
	}

	public byte[] getValueByteArray()
	{
		byte[] result = null;
		if (mTLVObjects != null)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			foreach (TLVObject mTLVObject in mTLVObjects)
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

	public List<TLVObject> getValueTLVObjectList()
	{
		return mTLVObjects;
	}

	public TLVObject findByTopLevelTagHexString(string tlvTagHexString)
	{
		return findByTopLevelTagByteArray(TLVParser.getByteArrayFromHexString(tlvTagHexString));
	}

	public TLVObject findByTagHexString(string tlvTagHexString)
	{
		return findByTagByteArray(TLVParser.getByteArrayFromHexString(tlvTagHexString));
	}

	public TLVObject findChildByTagHexString(string tlvTagHexString)
	{
		return findChildByTagByteArray(TLVParser.getByteArrayFromHexString(tlvTagHexString));
	}

	public TLVObject findByTopLevelTagByteArray(byte[] tlvTagByteArray)
	{
		if (tlvTagByteArray != null)
		{
			if (mTag != null && tlvTagByteArray.SequenceEqual(mTag))
			{
				return this;
			}
			if (mTLVObjects != null)
			{
				foreach (TLVObject mTLVObject in mTLVObjects)
				{
					if (mTLVObject != null)
					{
						byte[] tagByteArray = mTLVObject.getTagByteArray();
						if (tagByteArray != null && tlvTagByteArray.SequenceEqual(tagByteArray))
						{
							return mTLVObject;
						}
					}
				}
			}
		}
		return null;
	}

	public TLVObject findByTagByteArray(byte[] tlvTagByteArray)
	{
		if (tlvTagByteArray != null)
		{
			if (mTag != null && tlvTagByteArray.SequenceEqual(mTag))
			{
				return this;
			}
			if (mTLVObjects != null)
			{
				foreach (TLVObject mTLVObject in mTLVObjects)
				{
					if (mTLVObject != null)
					{
						TLVObject tLVObject = mTLVObject.findByTagByteArray(tlvTagByteArray);
						if (tLVObject != null)
						{
							return tLVObject;
						}
					}
				}
			}
		}
		return null;
	}

	public TLVObject findChildByTagByteArray(byte[] tlvTagByteArray)
	{
		if (tlvTagByteArray != null && mTLVObjects != null)
		{
			foreach (TLVObject mTLVObject in mTLVObjects)
			{
				if (mTLVObject != null)
				{
					TLVObject tLVObject = mTLVObject.findByTagByteArray(tlvTagByteArray);
					if (tLVObject != null)
					{
						return tLVObject;
					}
				}
			}
		}
		return null;
	}
}
