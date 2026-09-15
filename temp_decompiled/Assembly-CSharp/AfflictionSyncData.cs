using System;
using System.Collections.Generic;
using Peak.Afflictions;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x02000004 RID: 4
public struct AfflictionSyncData : IBinarySerializable
{
	// Token: 0x06000003 RID: 3 RVA: 0x00002060 File Offset: 0x00000260
	public void Serialize(BinarySerializer serializer)
	{
		if (this.afflictions == null)
		{
			this.afflictions = new List<Affliction>();
		}
		serializer.WriteInt(this.afflictions.Count);
		for (int i = 0; i < this.afflictions.Count; i++)
		{
			serializer.WriteInt((int)this.afflictions[i].GetAfflictionType());
			this.afflictions[i].Serialize(serializer);
		}
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000020D0 File Offset: 0x000002D0
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.afflictions = new List<Affliction>();
		int num = deserializer.ReadInt();
		for (int i = 0; i < num; i++)
		{
			Affliction.AfflictionType afflictionType = (Affliction.AfflictionType)deserializer.ReadInt();
			Affliction affliction = Affliction.CreateBlankAffliction(afflictionType);
			if (affliction == null)
			{
				Debug.LogError("FAILED TO CREATE AFFLICTION OF TYPE '" + afflictionType.ToString() + "'! Affliction.CreateBlankAffliction() is likely missing a case for this type.");
				return;
			}
			affliction.Deserialize(deserializer);
			this.afflictions.Add(affliction);
		}
	}

	// Token: 0x04000001 RID: 1
	public List<Affliction> afflictions;
}
