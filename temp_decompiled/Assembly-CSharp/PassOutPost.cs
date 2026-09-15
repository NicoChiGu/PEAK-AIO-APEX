using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020002B7 RID: 695
public class PassOutPost : MonoBehaviour
{
	// Token: 0x060012F7 RID: 4855 RVA: 0x0006055F File Offset: 0x0005E75F
	private void Start()
	{
		this.vol = base.GetComponent<Volume>();
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x00060570 File Offset: 0x0005E770
	private void Update()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		this.vol.enabled = this.vol.weight > 0.0001f;
		if (Character.localCharacter.data.fullyPassedOut)
		{
			this.vol.weight = 0f;
			return;
		}
		this.vol.weight = Character.localCharacter.data.passOutValue;
	}

	// Token: 0x0400119E RID: 4510
	private Volume vol;
}
