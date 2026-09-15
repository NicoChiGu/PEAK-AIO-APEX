using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002BC RID: 700
public class PerlinShake : MonoBehaviour
{
	// Token: 0x06001312 RID: 4882 RVA: 0x00060D85 File Offset: 0x0005EF85
	private void Start()
	{
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x00060D88 File Offset: 0x0005EF88
	private void Update()
	{
		Vector2 zero = Vector2.zero;
		for (int i = this.shakes.Count - 1; i >= 0; i--)
		{
			zero.x += (Mathf.PerlinNoise(Time.time * this.shakes[i].scale, 0f) - 0.5f) * this.shakes[i].amount * (this.shakes[i].duration / this.shakes[i].startDuration);
			zero.y += (Mathf.PerlinNoise(0f, Time.time * this.shakes[i].scale) - 0.5f) * this.shakes[i].amount * (this.shakes[i].duration / this.shakes[i].startDuration);
			this.shakes[i].duration -= Time.deltaTime;
			if (this.shakes[i].duration < 0f)
			{
				this.shakes.RemoveAt(i);
			}
		}
		base.transform.localEulerAngles = zero;
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x00060EDC File Offset: 0x0005F0DC
	public void AddShake(float amount = 1f, float duration = 0.2f, float scale = 15f)
	{
		PerlinShakeInstance perlinShakeInstance = new PerlinShakeInstance();
		perlinShakeInstance.amount = amount;
		perlinShakeInstance.duration = duration;
		perlinShakeInstance.startDuration = duration;
		perlinShakeInstance.scale = scale;
		this.shakes.Add(perlinShakeInstance);
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x00060F18 File Offset: 0x0005F118
	public void AddShake(Vector3 pos, float amount = 1f, float duration = 0.2f, float scale = 15f, float range = 50f)
	{
		float num = Mathf.InverseLerp(range, 0f, Vector3.Distance(MainCamera.instance.transform.position, pos));
		if (num <= 0.001f)
		{
			return;
		}
		this.AddShake(amount * num, duration * num, scale);
	}

	// Token: 0x040011BE RID: 4542
	public List<PerlinShakeInstance> shakes = new List<PerlinShakeInstance>();
}
