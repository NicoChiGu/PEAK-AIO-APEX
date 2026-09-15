using System;
using System.Collections.Generic;
using Zorro.Core.Serizalization;

// Token: 0x0200003F RID: 63
public struct ThornSyncData : IBinarySerializable
{
	// Token: 0x060003D0 RID: 976 RVA: 0x00018B90 File Offset: 0x00016D90
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteInt(this.stuckThornIndices.Count);
		for (int i = 0; i < this.stuckThornIndices.Count; i++)
		{
			serializer.WriteUshort(this.stuckThornIndices[i]);
		}
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00018BD8 File Offset: 0x00016DD8
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.stuckThornIndices = new List<ushort>();
		int num = deserializer.ReadInt();
		ushort num2 = 0;
		while ((int)num2 < num)
		{
			this.stuckThornIndices.Add(deserializer.ReadUShort());
			num2 += 1;
		}
	}

	// Token: 0x04000417 RID: 1047
	public List<ushort> stuckThornIndices;
}
