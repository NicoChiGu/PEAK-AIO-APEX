using System;
using UnityEngine;

// Token: 0x020002B4 RID: 692
public class ParticleCountCuller : MonoBehaviour
{
	// Token: 0x060012EA RID: 4842 RVA: 0x000602EC File Offset: 0x0005E4EC
	private void Start()
	{
		this.ps = base.GetComponent<ParticleSystem>();
		this.startVal = this.ps.emission.rateOverTimeMultiplier;
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x00060320 File Offset: 0x0005E520
	private void Update()
	{
		float num = Mathf.Clamp01((Vector3.Distance(base.transform.position, Character.observedCharacter.Head) - this.minDistance) / this.maxDistance);
		this.attenuationVal = Mathf.Lerp(1f, 0.2f, num);
		this.ps.emission.rateOverTimeMultiplier = this.attenuationVal * this.startVal;
	}

	// Token: 0x04001193 RID: 4499
	private ParticleSystem ps;

	// Token: 0x04001194 RID: 4500
	public float minDistance = 20f;

	// Token: 0x04001195 RID: 4501
	public float maxDistance = 80f;

	// Token: 0x04001196 RID: 4502
	[Range(0f, 1f)]
	public float attenuationVal;

	// Token: 0x04001197 RID: 4503
	private float startVal;
}
