using System;
using Zorro.Core.Serizalization;

// Token: 0x02000111 RID: 273
public class OptionableBoolItemData : DataEntryValue
{
	// Token: 0x060008D3 RID: 2259 RVA: 0x0002FBA9 File Offset: 0x0002DDA9
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteBool(this.HasData);
		if (this.HasData)
		{
			serializer.WriteBool(this.Value);
		}
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x0002FBCB File Offset: 0x0002DDCB
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.HasData = deserializer.ReadBool();
		if (this.HasData)
		{
			this.Value = deserializer.ReadBool();
		}
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x0002FBED File Offset: 0x0002DDED
	public override string ToString()
	{
		if (!this.HasData)
		{
			return "No Data";
		}
		return this.Value.ToString();
	}

	// Token: 0x0400085F RID: 2143
	public bool HasData;

	// Token: 0x04000860 RID: 2144
	public bool Value;
}
