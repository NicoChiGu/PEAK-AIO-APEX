using System;
using System.Globalization;
using Zorro.Core.Serizalization;

// Token: 0x0200010F RID: 271
public class FloatItemData : DataEntryValue
{
	// Token: 0x060008CB RID: 2251 RVA: 0x0002FB42 File Offset: 0x0002DD42
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteFloat(this.Value);
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0002FB50 File Offset: 0x0002DD50
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.Value = deserializer.ReadFloat();
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0002FB5E File Offset: 0x0002DD5E
	public override string ToString()
	{
		return this.Value.ToString(CultureInfo.InvariantCulture);
	}

	// Token: 0x0400085D RID: 2141
	public float Value;
}
