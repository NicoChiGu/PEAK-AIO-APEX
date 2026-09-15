using System;
using UnityEngine;

// Token: 0x02000329 RID: 809
public class SetRockColors : MonoBehaviour
{
	// Token: 0x060014C8 RID: 5320 RVA: 0x00069AB4 File Offset: 0x00067CB4
	private void Start()
	{
		foreach (Material material in this.matsToEdit)
		{
			material.SetColor("_TopColor", this.topColor);
			material.SetColor("_Tint", this.tint);
		}
	}

	// Token: 0x0400133D RID: 4925
	public Color topColor;

	// Token: 0x0400133E RID: 4926
	[ColorUsage(false, true)]
	public Color tint;

	// Token: 0x0400133F RID: 4927
	public Material[] matsToEdit;
}
