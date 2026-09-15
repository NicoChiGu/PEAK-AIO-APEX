using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000359 RID: 857
public class Transition_CanvasGroup : Transition
{
	// Token: 0x060015F1 RID: 5617 RVA: 0x00071486 File Offset: 0x0006F686
	private void Awake()
	{
		this.gr = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x00071494 File Offset: 0x0006F694
	public override IEnumerator TransitionIn(float speed = 1f)
	{
		float c = 0f;
		float t = this.inCurve.keys[this.inCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.inSpeed;
			this.gr.alpha = this.inCurve.Evaluate(c);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x000714AA File Offset: 0x0006F6AA
	public override IEnumerator TransitionOut(float speed = 1f)
	{
		float c = 0f;
		float t = this.outCurve.keys[this.outCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.outSpeed;
			this.gr.alpha = this.outCurve.Evaluate(c);
			yield return null;
		}
		yield break;
	}

	// Token: 0x040014CD RID: 5325
	private CanvasGroup gr;

	// Token: 0x040014CE RID: 5326
	public float inSpeed = 1f;

	// Token: 0x040014CF RID: 5327
	public AnimationCurve inCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040014D0 RID: 5328
	public float outSpeed = 1f;

	// Token: 0x040014D1 RID: 5329
	public AnimationCurve outCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
}
