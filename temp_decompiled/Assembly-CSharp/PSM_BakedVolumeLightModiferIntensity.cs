using System;
using UnityEngine;

// Token: 0x020002DA RID: 730
public class PSM_BakedVolumeLightModiferIntensity : PropSpawnerMod
{
	// Token: 0x060013A2 RID: 5026 RVA: 0x00063CC8 File Offset: 0x00061EC8
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		BakedVolumeLight component = spawned.GetComponent<BakedVolumeLight>();
		if (!component)
		{
			return;
		}
		if (this.customIntensity)
		{
			component.intensity = this.intensity;
		}
		if (this.customColor)
		{
			component.color = this.color;
		}
	}

	// Token: 0x0400122D RID: 4653
	public bool customColor;

	// Token: 0x0400122E RID: 4654
	public Color color = new Color(0.86f, 0.56f, 0.04f, 0.87f);

	// Token: 0x0400122F RID: 4655
	public bool customIntensity;

	// Token: 0x04001230 RID: 4656
	public float intensity = 0.5f;
}
