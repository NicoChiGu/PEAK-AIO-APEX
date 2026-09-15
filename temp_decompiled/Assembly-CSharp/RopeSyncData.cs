using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x02000177 RID: 375
public struct RopeSyncData : IBinarySerializable
{
	// Token: 0x06000BE5 RID: 3045 RVA: 0x0003F960 File Offset: 0x0003DB60
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteBool(this.isVisible);
		serializer.WriteBool(this.updateVisualizerManually);
		if (this.segments == null)
		{
			serializer.WriteUshort(0);
			return;
		}
		ushort num = (ushort)this.segments.Length;
		serializer.WriteUshort(num);
		for (int i = 0; i < (int)num; i++)
		{
			this.segments[i].Serialize(serializer);
		}
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x0003F9C4 File Offset: 0x0003DBC4
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.isVisible = deserializer.ReadBool();
		this.updateVisualizerManually = deserializer.ReadBool();
		ushort num = deserializer.ReadUShort();
		this.segments = new RopeSyncData.SegmentData[(int)num];
		for (int i = 0; i < (int)num; i++)
		{
			this.segments[i] = IBinarySerializable.Deserialize<RopeSyncData.SegmentData>(deserializer);
		}
	}

	// Token: 0x04000B05 RID: 2821
	public RopeSyncData.SegmentData[] segments;

	// Token: 0x04000B06 RID: 2822
	public bool isVisible;

	// Token: 0x04000B07 RID: 2823
	public bool updateVisualizerManually;

	// Token: 0x02000486 RID: 1158
	public struct SegmentData : IBinarySerializable
	{
		// Token: 0x06001B86 RID: 7046 RVA: 0x0008295C File Offset: 0x00080B5C
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat3(this.position);
			serializer.WriteQuaternion(this.rotation);
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x00082976 File Offset: 0x00080B76
		public void Deserialize(BinaryDeserializer deserializer)
		{
			this.position = deserializer.ReadFloat3();
			this.rotation = deserializer.ReadQuaternion();
		}

		// Token: 0x0400196B RID: 6507
		public float3 position;

		// Token: 0x0400196C RID: 6508
		public Quaternion rotation;
	}
}
