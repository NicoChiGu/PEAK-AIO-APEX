using System;
using Zorro.Core.Serizalization;

// Token: 0x0200010B RID: 267
public abstract class DataEntryValue : IBinarySerializable
{
	// Token: 0x060008B3 RID: 2227 RVA: 0x0002F6E8 File Offset: 0x0002D8E8
	public void Serialize(BinarySerializer serializer)
	{
		this.SerializeValue(serializer);
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0002F6F1 File Offset: 0x0002D8F1
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.DeserializeValue(deserializer);
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0002F6FA File Offset: 0x0002D8FA
	public virtual void Init()
	{
	}

	// Token: 0x060008B6 RID: 2230
	public abstract void SerializeValue(BinarySerializer serializer);

	// Token: 0x060008B7 RID: 2231
	public abstract void DeserializeValue(BinaryDeserializer deserializer);

	// Token: 0x060008B8 RID: 2232 RVA: 0x0002F6FC File Offset: 0x0002D8FC
	public static byte GetTypeValue(Type type)
	{
		if (type == typeof(IntItemData))
		{
			return 1;
		}
		if (type == typeof(OptionableIntItemData))
		{
			return 2;
		}
		if (type == typeof(BoolItemData))
		{
			return 3;
		}
		if (type == typeof(FloatItemData))
		{
			return 4;
		}
		if (type == typeof(OptionableBoolItemData))
		{
			return 5;
		}
		if (type == typeof(BackpackData))
		{
			return 6;
		}
		if (type == typeof(ColorItemData))
		{
			return 7;
		}
		return 0;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0002F798 File Offset: 0x0002D998
	public static DataEntryValue GetNewFromValue(byte value)
	{
		switch (value)
		{
		case 1:
			return new IntItemData();
		case 2:
			return new OptionableIntItemData();
		case 3:
			return new BoolItemData();
		case 4:
			return new FloatItemData();
		case 5:
			return new OptionableBoolItemData();
		case 6:
			return new BackpackData();
		case 7:
			return new ColorItemData();
		default:
			throw new NotImplementedException();
		}
	}
}
