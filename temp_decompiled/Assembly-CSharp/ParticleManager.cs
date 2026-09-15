using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public class ParticleManager : MonoBehaviour
{
	// Token: 0x060012F2 RID: 4850 RVA: 0x000604A8 File Offset: 0x0005E6A8
	private void Awake()
	{
		ParticleManager.instance = this;
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x000604B0 File Offset: 0x0005E6B0
	private void Update()
	{
		if (this.particles.Count == 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			this.particles[this.currentIndex].Scan();
			this.currentIndex = (this.currentIndex + 1) % this.particles.Count;
		}
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x00060507 File Offset: 0x0005E707
	public void Register(ParticleCuller particle)
	{
		if (!this.particles.Contains(particle))
		{
			this.particles.Add(particle);
		}
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x00060523 File Offset: 0x0005E723
	public void Unregister(ParticleCuller particle)
	{
		this.particles.Remove(particle);
		if (this.currentIndex >= this.particles.Count)
		{
			this.currentIndex = 0;
		}
	}

	// Token: 0x0400119A RID: 4506
	public static ParticleManager instance;

	// Token: 0x0400119B RID: 4507
	public List<ParticleCuller> particles = new List<ParticleCuller>();

	// Token: 0x0400119C RID: 4508
	private int currentIndex;

	// Token: 0x0400119D RID: 4509
	private const int particlesPerFrame = 3;
}
