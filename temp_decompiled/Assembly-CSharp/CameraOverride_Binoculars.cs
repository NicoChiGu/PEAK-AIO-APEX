using System;
using UnityEngine;

// Token: 0x02000222 RID: 546
public class CameraOverride_Binoculars : CameraOverride
{
	// Token: 0x06001016 RID: 4118 RVA: 0x0004FDE9 File Offset: 0x0004DFE9
	private void Start()
	{
		this.lerpedFOV = this.fov;
	}

	// Token: 0x06001017 RID: 4119 RVA: 0x0004FDF8 File Offset: 0x0004DFF8
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(Character.localCharacter.data.lookDirection);
		this.fov = Mathf.Lerp(this.fov, this.lerpedFOV, Time.deltaTime * 5f);
	}

	// Token: 0x06001018 RID: 4120 RVA: 0x0004FE54 File Offset: 0x0004E054
	public void AdjustFOV(float value)
	{
		this.lerpedFOV += value;
		this.lerpedFOV = Mathf.Clamp(this.lerpedFOV, this.minFov, this.maxFov);
	}

	// Token: 0x04000E81 RID: 3713
	public float minFov;

	// Token: 0x04000E82 RID: 3714
	public float maxFov;

	// Token: 0x04000E83 RID: 3715
	public float fovChangeRate;

	// Token: 0x04000E84 RID: 3716
	public float lerpedFOV;
}
