using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x02000157 RID: 343
public struct ItemPhysicsSyncData : IBinarySerializable
{
	// Token: 0x06000B07 RID: 2823 RVA: 0x0003ABB1 File Offset: 0x00038DB1
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteFloat3(this.position);
		serializer.WriteQuaternion(this.rotation);
		serializer.WriteHalf3((half3)this.linearVelocity);
		serializer.WriteHalf3((half3)this.angularVelocity);
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x0003ABED File Offset: 0x00038DED
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.position = deserializer.ReadFloat3();
		this.rotation = deserializer.ReadQuaternion();
		this.linearVelocity = deserializer.ReadHalf3();
		this.angularVelocity = deserializer.ReadHalf3();
	}

	// Token: 0x04000A3F RID: 2623
	public float3 position;

	// Token: 0x04000A40 RID: 2624
	public Quaternion rotation;

	// Token: 0x04000A41 RID: 2625
	public float3 linearVelocity;

	// Token: 0x04000A42 RID: 2626
	public float3 angularVelocity;
}
