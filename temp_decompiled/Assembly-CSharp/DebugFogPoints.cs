using System;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class DebugFogPoints : MonoBehaviour
{
	// Token: 0x060005CE RID: 1486 RVA: 0x0002120F File Offset: 0x0001F40F
	private void Start()
	{
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x00021211 File Offset: 0x0001F411
	private void Update()
	{
		this.fogRenderer.material.SetVector("_FogCenter", this.fogPoint.position);
	}

	// Token: 0x040005F4 RID: 1524
	public Transform fogPoint;

	// Token: 0x040005F5 RID: 1525
	public Renderer fogRenderer;
}
