using System;
using UnityEngine;

// Token: 0x020002B5 RID: 693
public class ParticleCuller : MonoBehaviour
{
	// Token: 0x060012ED RID: 4845 RVA: 0x000603AF File Offset: 0x0005E5AF
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.cullDistance);
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x000603D1 File Offset: 0x0005E5D1
	public void OnEnable()
	{
		if (ParticleManager.instance != null)
		{
			ParticleManager.instance.Register(this);
		}
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x000603EB File Offset: 0x0005E5EB
	public void OnDisable()
	{
		if (ParticleManager.instance != null)
		{
			ParticleManager.instance.Unregister(this);
		}
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x00060408 File Offset: 0x0005E608
	public void Scan()
	{
		if (Character.observedCharacter)
		{
			bool flag = Vector3.Distance(Character.observedCharacter.Center, base.transform.position) < this.cullDistance;
			for (int i = 0; i < this.systems.Length; i++)
			{
				if (flag && !this.systems[i].isPlaying)
				{
					this.systems[i].Play();
				}
				if (!flag && this.systems[i].isPlaying)
				{
					this.systems[i].Stop();
				}
			}
		}
	}

	// Token: 0x04001198 RID: 4504
	public ParticleSystem[] systems;

	// Token: 0x04001199 RID: 4505
	public float cullDistance = 50f;
}
