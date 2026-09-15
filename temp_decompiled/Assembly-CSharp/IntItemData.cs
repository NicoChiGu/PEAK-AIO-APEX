using System;
using Zorro.Core.Serizalization;

// Token: 0x02000110 RID: 272
public class IntItemData : DataEntryValue
{
	// Token: 0x060008CF RID: 2255 RVA: 0x0002FB78 File Offset: 0x0002DD78
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteInt(this.Value);
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0002FB86 File Offset: 0x0002DD86
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.Value = deserializer.ReadInt();
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0002FB94 File Offset: 0x0002DD94
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x0400085E RID: 2142
	public int Value;
}
