using System;
using UnityEngine;

// Token: 0x0200029F RID: 671
public class MirrorReflectionScript : MonoBehaviour
{
	// Token: 0x0600126A RID: 4714 RVA: 0x0005DE07 File Offset: 0x0005C007
	private void Start()
	{
		this.childScript = base.gameObject.transform.parent.gameObject.GetComponentInChildren<MirrorCameraScript>();
		if (this.childScript == null)
		{
			Debug.LogError("Child script (MirrorCameraScript) should be in sibling object");
		}
	}

	// Token: 0x0600126B RID: 4715 RVA: 0x0005DE41 File Offset: 0x0005C041
	private void OnWillRenderObject()
	{
		this.childScript.RenderMirror();
	}

	// Token: 0x04001113 RID: 4371
	private MirrorCameraScript childScript;
}
