using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class LavaPost : MonoBehaviour
{
	// Token: 0x060011F8 RID: 4600 RVA: 0x0005B138 File Offset: 0x00059338
	private void Start()
	{
		this.rend = base.GetComponent<MeshRenderer>();
		Shader.SetGlobalFloat("LavaAlpha", 0f);
		this.lava1Height = this.lava1.position.y;
		this.lava2Height = this.lava2.position.y;
		this.currentLavaHeight = this.lava1Height;
		this.lastLavaHeight = this.lava2Height;
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x0005B1A4 File Offset: 0x000593A4
	private void LateUpdate()
	{
		if (this.lava1 == null)
		{
			return;
		}
		bool flag = MainCamera.instance.transform.position.z < this.thresholdTransform.position.z;
		if (this.firstIsActive != flag)
		{
			this.alpha = Mathf.MoveTowards(this.alpha, 0f, Time.deltaTime);
			if (this.alpha < 0.001f)
			{
				this.firstIsActive = flag;
			}
		}
		else
		{
			this.alpha = Mathf.MoveTowards(this.alpha, 1f, Time.deltaTime);
		}
		float num = (this.firstIsActive ? this.lava1Height : this.lava2Height);
		if (this.lastLavaHeight != num)
		{
			this.lastLavaHeight = num;
			base.StopAllCoroutines();
			base.StartCoroutine(this.lavaMove(num));
		}
		if (!this.blending)
		{
			float num2 = (this.firstIsActive ? this.lava1.position.y : this.lava2.position.y);
			Shader.SetGlobalFloat("LavaHeight", num2);
		}
		if (MainCamera.instance.transform.position.z < this.lavaFadeIn.position.z)
		{
			this.rend.enabled = false;
		}
		else
		{
			this.rend.enabled = true;
		}
		Shader.SetGlobalFloat("LavaStart", this.lavaStart.position.z);
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x0005B30F File Offset: 0x0005950F
	public IEnumerator lavaMove(float newLavaHeight)
	{
		this.blending = true;
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			this.currentLavaHeight = Mathf.Lerp(this.currentLavaHeight, newLavaHeight, normalizedTime / 3f);
			Shader.SetGlobalFloat("LavaHeight", this.currentLavaHeight);
			normalizedTime += Time.deltaTime;
			yield return null;
		}
		this.blending = false;
		yield break;
	}

	// Token: 0x04001078 RID: 4216
	private MeshRenderer rend;

	// Token: 0x04001079 RID: 4217
	public Transform lava1;

	// Token: 0x0400107A RID: 4218
	public Transform lava2;

	// Token: 0x0400107B RID: 4219
	private float lava1Height;

	// Token: 0x0400107C RID: 4220
	private float lava2Height;

	// Token: 0x0400107D RID: 4221
	public Transform thresholdTransform;

	// Token: 0x0400107E RID: 4222
	public Transform lavaFadeIn;

	// Token: 0x0400107F RID: 4223
	public Transform lavaStart;

	// Token: 0x04001080 RID: 4224
	private float alpha;

	// Token: 0x04001081 RID: 4225
	private bool firstIsActive;

	// Token: 0x04001082 RID: 4226
	private float currentLavaHeight;

	// Token: 0x04001083 RID: 4227
	private float lastLavaHeight;

	// Token: 0x04001084 RID: 4228
	private bool blending;
}
