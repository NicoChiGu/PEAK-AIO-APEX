using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200024D RID: 589
public class DispelFogField : MonoBehaviour
{
	// Token: 0x0600110E RID: 4366 RVA: 0x00055EF8 File Offset: 0x000540F8
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.innerRadius);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(base.transform.position, this.outerRadius);
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x00055F45 File Offset: 0x00054145
	public void OnDisable()
	{
		Singleton<OrbFogHandler>.Instance.dispelFogAmount = 0f;
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x00055F58 File Offset: 0x00054158
	public void Update()
	{
		float num = Vector3.Distance(Character.observedCharacter.Center, base.transform.position);
		if (Character.observedCharacter && num <= this.outerRadius)
		{
			Singleton<OrbFogHandler>.Instance.dispelFogAmount = Mathf.InverseLerp(this.outerRadius, this.innerRadius, num);
			return;
		}
		Singleton<OrbFogHandler>.Instance.dispelFogAmount = 0f;
	}

	// Token: 0x04000F7B RID: 3963
	public float innerRadius = 7.5f;

	// Token: 0x04000F7C RID: 3964
	public float outerRadius = 12.5f;

	// Token: 0x04000F7D RID: 3965
	private float lastEnteredTime;

	// Token: 0x04000F7E RID: 3966
	private bool inflicting;
}
