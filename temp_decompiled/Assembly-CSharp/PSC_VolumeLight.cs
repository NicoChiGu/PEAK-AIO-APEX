using System;
using UnityEngine;

// Token: 0x020002FD RID: 765
public class PSC_VolumeLight : PropSpawnerConstraint, IValidationConstraint
{
	// Token: 0x060013EC RID: 5100 RVA: 0x00065098 File Offset: 0x00063298
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		Color color = LightVolume.Instance().SamplePosition(spawnData.pos);
		return color.a > this.minMax.x && color.a < this.minMax.y;
	}

	// Token: 0x04001288 RID: 4744
	public Vector2 minMax = new Vector2(0f, 0.5f);
}
