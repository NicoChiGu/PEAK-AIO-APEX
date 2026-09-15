using System;
using UnityEngine;

// Token: 0x020002F1 RID: 753
[Serializable]
public abstract class PropSpawnerConstraint
{
	// Token: 0x17000139 RID: 313
	// (get) Token: 0x060013CF RID: 5071 RVA: 0x00064815 File Offset: 0x00062A15
	public bool Muted
	{
		get
		{
			return this.mute;
		}
	}

	// Token: 0x060013D0 RID: 5072
	public abstract bool CheckConstraint(PropSpawner.SpawnData spawnData);

	// Token: 0x060013D1 RID: 5073 RVA: 0x0006481D File Offset: 0x00062A1D
	public virtual bool Validate(GameObject _, PropSpawner.SpawnData spawnData)
	{
		return this.CheckConstraint(spawnData);
	}

	// Token: 0x04001266 RID: 4710
	public bool mute;
}
