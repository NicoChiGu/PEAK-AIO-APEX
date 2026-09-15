using System;
using UnityEngine;

// Token: 0x02000152 RID: 338
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemCenter : MonoBehaviour
{
	// Token: 0x06000AD6 RID: 2774 RVA: 0x00039ECB File Offset: 0x000380CB
	private void Start()
	{
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x00039ECD File Offset: 0x000380CD
	private void Update()
	{
		this.setPosition();
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x00039ED8 File Offset: 0x000380D8
	public void setPosition()
	{
		if (this.psr == null)
		{
			this.psr = base.GetComponent<ParticleSystemRenderer>();
			this.material = this.psr.material;
		}
		this.pos = base.transform.position;
		this.material.SetVector(ParticleSystemCenter.Center, this.pos);
	}

	// Token: 0x04000A15 RID: 2581
	private static readonly int Center = Shader.PropertyToID("_Center");

	// Token: 0x04000A16 RID: 2582
	private Vector3 pos;

	// Token: 0x04000A17 RID: 2583
	public Material material;

	// Token: 0x04000A18 RID: 2584
	private ParticleSystemRenderer psr;
}
