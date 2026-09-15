using System;

// Token: 0x02000156 RID: 342
public class MobItemPhysicsSyncer : ItemPhysicsSyncer
{
	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000B05 RID: 2821 RVA: 0x0003AB96 File Offset: 0x00038D96
	protected override float maxAngleChangePerSecond
	{
		get
		{
			return this.maxAngleUpdatePerSecond;
		}
	}

	// Token: 0x04000A3E RID: 2622
	public float maxAngleUpdatePerSecond = 180f;
}
