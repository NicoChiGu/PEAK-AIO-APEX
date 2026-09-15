using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// Token: 0x020002C1 RID: 705
[Serializable]
public class MouseLookHelper
{
	// Token: 0x06001328 RID: 4904 RVA: 0x000614E7 File Offset: 0x0005F6E7
	public void Init(Transform character, Transform camera)
	{
		this.m_CharacterTargetRot = character.localRotation;
		this.m_CameraTargetRot = camera.localRotation;
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x00061504 File Offset: 0x0005F704
	public void LookRotation(Transform character, Transform camera)
	{
		float num = CrossPlatformInputManager.GetAxis("Mouse X") * this.XSensitivity;
		float num2 = CrossPlatformInputManager.GetAxis("Mouse Y") * this.YSensitivity;
		this.m_CharacterTargetRot *= Quaternion.Euler(0f, num, 0f);
		this.m_CameraTargetRot *= Quaternion.Euler(-num2, 0f, 0f);
		if (this.clampVerticalRotation)
		{
			this.m_CameraTargetRot = this.ClampRotationAroundXAxis(this.m_CameraTargetRot);
		}
		if (this.smooth)
		{
			character.localRotation = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, this.smoothTime * Time.deltaTime);
			camera.localRotation = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, this.smoothTime * Time.deltaTime);
			return;
		}
		character.localRotation = this.m_CharacterTargetRot;
		camera.localRotation = this.m_CameraTargetRot;
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x000615FC File Offset: 0x0005F7FC
	private Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1f;
		float num = 114.59156f * Mathf.Atan(q.x);
		num = Mathf.Clamp(num, this.MinimumX, this.MaximumX);
		q.x = Mathf.Tan(0.008726646f * num);
		return q;
	}

	// Token: 0x040011C8 RID: 4552
	public float XSensitivity = 2f;

	// Token: 0x040011C9 RID: 4553
	public float YSensitivity = 2f;

	// Token: 0x040011CA RID: 4554
	public bool clampVerticalRotation = true;

	// Token: 0x040011CB RID: 4555
	public float MinimumX = -90f;

	// Token: 0x040011CC RID: 4556
	public float MaximumX = 90f;

	// Token: 0x040011CD RID: 4557
	public bool smooth;

	// Token: 0x040011CE RID: 4558
	public float smoothTime = 5f;

	// Token: 0x040011CF RID: 4559
	private Quaternion m_CharacterTargetRot;

	// Token: 0x040011D0 RID: 4560
	private Quaternion m_CameraTargetRot;
}
