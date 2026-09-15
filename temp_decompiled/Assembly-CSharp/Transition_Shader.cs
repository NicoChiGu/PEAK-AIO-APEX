using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class Transition_Shader : Transition
{
	// Token: 0x060015F5 RID: 5621 RVA: 0x00071527 File Offset: 0x0006F727
	private void Awake()
	{
		this.rend = base.GetComponent<MeshRenderer>();
		this.mat = Object.Instantiate<Material>(this.rend.sharedMaterial);
		this.rend.sharedMaterial = this.mat;
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x0007155C File Offset: 0x0006F75C
	public override IEnumerator TransitionIn(float speed = 1f)
	{
		float c = 0f;
		float t = this.inCurve.keys[this.inCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.inSpeed;
			this.mat.SetFloat("_Progress", c);
			this.mat.SetInt("_In", 1);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x00071572 File Offset: 0x0006F772
	public override IEnumerator TransitionOut(float speed = 1f)
	{
		float c = 0f;
		float t = this.outCurve.keys[this.outCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.outSpeed;
			this.mat.SetFloat("_Progress", c);
			this.mat.SetInt("_In", 0);
			yield return null;
		}
		yield break;
	}

	// Token: 0x040014D2 RID: 5330
	private MeshRenderer rend;

	// Token: 0x040014D3 RID: 5331
	private Material mat;

	// Token: 0x040014D4 RID: 5332
	public float inSpeed = 1f;

	// Token: 0x040014D5 RID: 5333
	public AnimationCurve inCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040014D6 RID: 5334
	public float outSpeed = 1f;

	// Token: 0x040014D7 RID: 5335
	public AnimationCurve outCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
}
