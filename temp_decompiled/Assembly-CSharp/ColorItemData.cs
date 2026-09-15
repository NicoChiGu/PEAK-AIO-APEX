using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x0200010E RID: 270
public class ColorItemData : DataEntryValue
{
	// Token: 0x060008C7 RID: 2247 RVA: 0x0002FAB5 File Offset: 0x0002DCB5
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteFloat4(new float4(this.Value.r, this.Value.g, this.Value.b, this.Value.a));
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x0002FAF0 File Offset: 0x0002DCF0
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		float4 @float = deserializer.ReadFloat4();
		this.Value = new Color(@float.x, @float.y, @float.z, @float.w);
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x0002FB27 File Offset: 0x0002DD27
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x0400085C RID: 2140
	public Color Value;
}
