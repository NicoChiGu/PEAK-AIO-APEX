using System;
using UnityEngine;

// Token: 0x020002A1 RID: 673
public class MainCamera : MonoBehaviour
{
	// Token: 0x0600126E RID: 4718 RVA: 0x0005DEA6 File Offset: 0x0005C0A6
	private void Awake()
	{
		this.cam = base.GetComponent<Camera>();
		MainCamera.instance = this;
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x0005DEBA File Offset: 0x0005C0BA
	public void SetCameraOverride(CameraOverride setOverride)
	{
		this.camOverride = setOverride;
		this.sinceOverride = 0;
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x0005DECA File Offset: 0x0005C0CA
	private void Update()
	{
		AudioListener.volume = Mathf.Lerp(AudioListener.volume, 1f, 0.1f * Time.deltaTime);
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x0005DEEB File Offset: 0x0005C0EB
	private void LateUpdate()
	{
	}

	// Token: 0x0400111D RID: 4381
	public static MainCamera instance;

	// Token: 0x0400111E RID: 4382
	internal Camera cam;

	// Token: 0x0400111F RID: 4383
	internal CameraOverride camOverride;

	// Token: 0x04001120 RID: 4384
	private int sinceOverride = 10;
}
