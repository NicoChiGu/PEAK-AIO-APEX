using System;
using Zorro.Core.Serizalization;

// Token: 0x02000112 RID: 274
public class OptionableIntItemData : DataEntryValue
{
	// Token: 0x060008D7 RID: 2263 RVA: 0x0002FC10 File Offset: 0x0002DE10
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteBool(this.HasData);
		if (this.HasData)
		{
			serializer.WriteInt(this.Value);
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0002FC32 File Offset: 0x0002DE32
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.HasData = deserializer.ReadBool();
		if (this.HasData)
		{
			this.Value = deserializer.ReadInt();
		}
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x0002FC54 File Offset: 0x0002DE54
	public override string ToString()
	{
		if (!this.HasData)
		{
			return "No Data";
		}
		return this.Value.ToString();
	}

	// Token: 0x04000861 RID: 2145
	public bool HasData;

	// Token: 0x04000862 RID: 2146
	public int Value;
}
