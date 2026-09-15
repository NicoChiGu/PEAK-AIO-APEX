using System;
using System.Collections.Generic;
using Zorro.Core.Serizalization;

// Token: 0x0200003C RID: 60
public struct StatusSyncData : IBinarySerializable
{
	// Token: 0x060003AC RID: 940 RVA: 0x0001850C File Offset: 0x0001670C
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteInt(this.statusList.Count);
		for (int i = 0; i < this.statusList.Count; i++)
		{
			serializer.WriteFloat(this.statusList[i]);
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x00018554 File Offset: 0x00016754
	public void Deserialize(BinaryDeserializer deserializer)
	{
		int num = deserializer.ReadInt();
		this.statusList = new List<float>();
		for (int i = 0; i < num; i++)
		{
			this.statusList.Add(deserializer.ReadFloat());
		}
	}

	// Token: 0x04000401 RID: 1025
	public List<float> statusList;
}
