using System;
using UnityEngine;

// Token: 0x020002FF RID: 767
[Serializable]
public abstract class PropSpawnerConstraintPost
{
	// Token: 0x1700013B RID: 315
	// (get) Token: 0x060013F0 RID: 5104 RVA: 0x000650FB File Offset: 0x000632FB
	public bool Muted
	{
		get
		{
			return this.mute;
		}
	}

	// Token: 0x060013F1 RID: 5105
	public abstract bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData);

	// Token: 0x060013F2 RID: 5106 RVA: 0x00065103 File Offset: 0x00063303
	public virtual bool Validate(GameObject spawnedProp, PropSpawner.SpawnData spawnData)
	{
		return this.CheckConstraint(spawnedProp, spawnData);
	}

	// Token: 0x04001289 RID: 4745
	public bool mute;
}
