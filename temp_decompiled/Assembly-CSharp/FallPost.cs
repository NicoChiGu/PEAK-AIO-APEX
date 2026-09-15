using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200025B RID: 603
public class FallPost : MonoBehaviour
{
	// Token: 0x06001137 RID: 4407 RVA: 0x00056945 File Offset: 0x00054B45
	private void Start()
	{
		this.vol = base.GetComponent<Volume>();
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x00056954 File Offset: 0x00054B54
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.vol.enabled = this.vol.weight > 0.0001f;
		if (Character.localCharacter.data.fallSeconds > 0f)
		{
			this.vol.weight = Mathf.Lerp(this.vol.weight, 1f, Time.deltaTime);
			return;
		}
		this.vol.weight = Mathf.Lerp(this.vol.weight, 0f, Time.deltaTime);
	}

	// Token: 0x04000FAD RID: 4013
	private Volume vol;
}
