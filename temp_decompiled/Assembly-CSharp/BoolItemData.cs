using System;
using Zorro.Core.Serizalization;

// Token: 0x0200010D RID: 269
public class BoolItemData : DataEntryValue
{
	// Token: 0x060008C3 RID: 2243 RVA: 0x0002FA84 File Offset: 0x0002DC84
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteBool(this.Value);
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0002FA92 File Offset: 0x0002DC92
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.Value = deserializer.ReadBool();
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x0002FAA0 File Offset: 0x0002DCA0
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x0400085B RID: 2139
	public bool Value;
}
