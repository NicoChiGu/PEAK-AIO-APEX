using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200013B RID: 315
public class Mirage : MonoBehaviour
{
	// Token: 0x06000A13 RID: 2579 RVA: 0x000357A8 File Offset: 0x000339A8
	public void fadeMirage()
	{
		if (Application.isPlaying)
		{
			MirageManager.instance.sampleCamera();
		}
		this.ps.Play();
		this.setParticleData();
		if (this.hideObjects)
		{
			base.StartCoroutine(this.HideObject());
		}
		this.hasFaded = true;
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x000357E8 File Offset: 0x000339E8
	private IEnumerator HideObject()
	{
		yield return new WaitForSeconds(this.hideDelay);
		for (int i = 0; i < this.objectsToHide.Length; i++)
		{
			this.objectsToHide[i].SetActive(false);
		}
		yield break;
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x000357F7 File Offset: 0x000339F7
	private void setParticleData()
	{
		base.StartCoroutine(this.<setParticleData>g__particleDataRoutine|11_0());
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00035806 File Offset: 0x00033A06
	public void AssignRT(RenderTexture texture)
	{
		this.ps.GetComponent<ParticleSystemRenderer>().material.SetTexture("_MainTex", texture);
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00035823 File Offset: 0x00033A23
	private void Start()
	{
		this.psr = this.ps.GetComponent<ParticleSystemRenderer>();
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00035836 File Offset: 0x00033A36
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		if (Vector3.Distance(Character.observedCharacter.Center, base.transform.position) < this.fadeDistance && !this.hasFaded)
		{
			this.fadeMirage();
		}
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00035876 File Offset: 0x00033A76
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.fadeDistance);
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x000358AC File Offset: 0x00033AAC
	[CompilerGenerated]
	private IEnumerator <setParticleData>g__particleDataRoutine|11_0()
	{
		yield return new WaitForEndOfFrame();
		this.ps.GetCustomParticleData(this.customData, 0);
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[this.ps.particleCount];
		this.ps.GetParticles(array);
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(array[i].position);
			this.customData[i] = new Vector4(vector.x / (float)Screen.width, vector.y / (float)Screen.height, 0f, 1f);
		}
		this.ps.SetCustomParticleData(this.customData, 0);
		yield break;
	}

	// Token: 0x04000972 RID: 2418
	[FormerlySerializedAs("particleSystem")]
	public ParticleSystem ps;

	// Token: 0x04000973 RID: 2419
	public RenderTexture rt;

	// Token: 0x04000974 RID: 2420
	private ParticleSystemRenderer psr;

	// Token: 0x04000975 RID: 2421
	public bool hideObjects;

	// Token: 0x04000976 RID: 2422
	public GameObject[] objectsToHide;

	// Token: 0x04000977 RID: 2423
	public float hideDelay;

	// Token: 0x04000978 RID: 2424
	public float fadeDistance = 10f;

	// Token: 0x04000979 RID: 2425
	private List<Vector4> customData = new List<Vector4>();

	// Token: 0x0400097A RID: 2426
	public bool hasFaded;
}
