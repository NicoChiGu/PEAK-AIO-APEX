using System;

// Token: 0x020000EE RID: 238
public class Action_OverrideCamera : ItemAction
{
	// Token: 0x06000842 RID: 2114 RVA: 0x0002DA59 File Offset: 0x0002BC59
	public override void RunAction()
	{
		MainCamera.instance.SetCameraOverride(this.cameraOverride);
	}

	// Token: 0x040007F1 RID: 2033
	public CameraOverride cameraOverride;
}
