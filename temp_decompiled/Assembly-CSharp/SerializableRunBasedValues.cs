using System;
using System.Collections.Generic;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000181 RID: 385
public class SerializableRunBasedValues : IBinarySerializable
{
	// Token: 0x06000C34 RID: 3124 RVA: 0x00041048 File Offset: 0x0003F248
	internal void PrimeExistingAchievements()
	{
		this.steamAchievementsPreviouslyUnlocked.Clear();
		foreach (object obj in Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			ACHIEVEMENTTYPE achievementtype = (ACHIEVEMENTTYPE)obj;
			if (Singleton<AchievementManager>.Instance.IsAchievementUnlocked(achievementtype))
			{
				this.steamAchievementsPreviouslyUnlocked.Add(achievementtype);
			}
		}
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x000410C8 File Offset: 0x0003F2C8
	public void Serialize(BinarySerializer serializer)
	{
		this.SerializeRunBasedValues(serializer);
		this.SerializeUshortList(this.runBasedFruitsEaten, serializer);
		this.SerializeUshortList(this.shroomBerriesEaten, serializer);
		this.SerializeUshortList(this.gourmandRequirementsEaten, serializer);
		this.SerializeAchievementList(this.achievementsEarnedThisRun, serializer);
		this.SerializeAchievementList(this.steamAchievementsPreviouslyUnlocked, serializer);
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x00041120 File Offset: 0x0003F320
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.DeserializeRunBasedValues(deserializer);
		this.runBasedFruitsEaten = this.DeserializeUshortList(deserializer);
		this.shroomBerriesEaten = this.DeserializeUshortList(deserializer);
		this.gourmandRequirementsEaten = this.DeserializeUshortList(deserializer);
		this.achievementsEarnedThisRun = this.DeserializeAchievementList(deserializer);
		this.steamAchievementsPreviouslyUnlocked = this.DeserializeAchievementList(deserializer);
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x00041178 File Offset: 0x0003F378
	public void SerializeRunBasedValues(BinarySerializer serializer)
	{
		int count = this.runBasedInts.Count;
		serializer.WriteInt(count);
		foreach (KeyValuePair<RUNBASEDVALUETYPE, int> keyValuePair in this.runBasedInts)
		{
			serializer.WriteInt((int)keyValuePair.Key);
			serializer.WriteInt(keyValuePair.Value);
		}
		int count2 = this.runBasedFloats.Count;
		serializer.WriteInt(count2);
		foreach (KeyValuePair<RUNBASEDVALUETYPE, float> keyValuePair2 in this.runBasedFloats)
		{
			serializer.WriteInt((int)keyValuePair2.Key);
			serializer.WriteFloat(keyValuePair2.Value);
		}
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x0004125C File Offset: 0x0003F45C
	public void DeserializeRunBasedValues(BinaryDeserializer deserializer)
	{
		this.runBasedInts.Clear();
		int num = deserializer.ReadInt();
		for (int i = 0; i < num; i++)
		{
			RUNBASEDVALUETYPE runbasedvaluetype = (RUNBASEDVALUETYPE)deserializer.ReadInt();
			int num2 = deserializer.ReadInt();
			this.runBasedInts.TryAdd(runbasedvaluetype, num2);
		}
		this.runBasedFloats.Clear();
		int num3 = deserializer.ReadInt();
		for (int j = 0; j < num3; j++)
		{
			RUNBASEDVALUETYPE runbasedvaluetype2 = (RUNBASEDVALUETYPE)deserializer.ReadInt();
			float num4 = deserializer.ReadFloat();
			this.runBasedFloats.TryAdd(runbasedvaluetype2, num4);
		}
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x000412E8 File Offset: 0x0003F4E8
	public void SerializeUshortList(List<ushort> list, BinarySerializer serializer)
	{
		if (list == null)
		{
			list = new List<ushort>();
		}
		serializer.WriteInt(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			serializer.WriteUshort(list[i]);
		}
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0004132C File Offset: 0x0003F52C
	public List<ushort> DeserializeUshortList(BinaryDeserializer deserializer)
	{
		List<ushort> list = new List<ushort>();
		int num = deserializer.ReadInt();
		for (int i = 0; i < num; i++)
		{
			list.Add(deserializer.ReadUShort());
		}
		return list;
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x00041360 File Offset: 0x0003F560
	public void SerializeAchievementList(List<ACHIEVEMENTTYPE> list, BinarySerializer serializer)
	{
		if (list == null)
		{
			list = new List<ACHIEVEMENTTYPE>();
		}
		serializer.WriteInt(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			serializer.WriteInt((int)list[i]);
		}
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x000413A4 File Offset: 0x0003F5A4
	public List<ACHIEVEMENTTYPE> DeserializeAchievementList(BinaryDeserializer deserializer)
	{
		List<ACHIEVEMENTTYPE> list = new List<ACHIEVEMENTTYPE>();
		int num = deserializer.ReadInt();
		for (int i = 0; i < num; i++)
		{
			list.Add((ACHIEVEMENTTYPE)deserializer.ReadInt());
		}
		return list;
	}

	// Token: 0x04000B4C RID: 2892
	internal Dictionary<RUNBASEDVALUETYPE, int> runBasedInts = new Dictionary<RUNBASEDVALUETYPE, int>();

	// Token: 0x04000B4D RID: 2893
	internal Dictionary<RUNBASEDVALUETYPE, float> runBasedFloats = new Dictionary<RUNBASEDVALUETYPE, float>();

	// Token: 0x04000B4E RID: 2894
	internal List<ushort> runBasedFruitsEaten = new List<ushort>();

	// Token: 0x04000B4F RID: 2895
	internal List<ushort> shroomBerriesEaten = new List<ushort>();

	// Token: 0x04000B50 RID: 2896
	internal List<ushort> nonToxicMushroomsEaten = new List<ushort>();

	// Token: 0x04000B51 RID: 2897
	internal List<ushort> gourmandRequirementsEaten = new List<ushort>();

	// Token: 0x04000B52 RID: 2898
	internal List<ACHIEVEMENTTYPE> achievementsEarnedThisRun = new List<ACHIEVEMENTTYPE>();

	// Token: 0x04000B53 RID: 2899
	internal List<int> completedAscentsThisRun = new List<int>();

	// Token: 0x04000B54 RID: 2900
	internal List<ACHIEVEMENTTYPE> steamAchievementsPreviouslyUnlocked = new List<ACHIEVEMENTTYPE>();
}
