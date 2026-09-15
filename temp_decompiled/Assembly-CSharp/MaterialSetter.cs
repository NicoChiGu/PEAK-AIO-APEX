using System;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class MaterialSetter : MonoBehaviour
{
	// Token: 0x06000A11 RID: 2577 RVA: 0x00035770 File Offset: 0x00033970
	public void setMaterial()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = this.material;
		}
	}

	// Token: 0x04000971 RID: 2417
	public Material material;
}
