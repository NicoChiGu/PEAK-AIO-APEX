using System;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class MirageLuggage : MonoBehaviour
{
	// Token: 0x06000A1C RID: 2588 RVA: 0x000358BB File Offset: 0x00033ABB
	private void OnEnable()
	{
		this.setMirageState(1f);
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x000358C8 File Offset: 0x00033AC8
	private void setMirageState(float mirageState)
	{
		this.renderers = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < this.renderers.Length; i++)
		{
			Material[] materials = this.renderers[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].SetFloat("_DoMirage", mirageState);
			}
		}
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0003591E File Offset: 0x00033B1E
	private void Update()
	{
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x00035920 File Offset: 0x00033B20
	private void OnDisable()
	{
		this.setMirageState(0f);
	}

	// Token: 0x0400097B RID: 2427
	private Renderer[] renderers;
}
