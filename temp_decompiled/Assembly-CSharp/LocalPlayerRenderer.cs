using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000296 RID: 662
public class LocalPlayerRenderer : MonoBehaviour
{
	// Token: 0x0600123F RID: 4671 RVA: 0x0005C598 File Offset: 0x0005A798
	private void Start()
	{
		Character componentInParent = base.GetComponentInParent<Character>();
		if (componentInParent && componentInParent.IsLocal)
		{
			base.GetComponent<MeshRenderer>().shadowCastingMode = this.renderMode;
		}
	}

	// Token: 0x040010BF RID: 4287
	public ShadowCastingMode renderMode;
}
