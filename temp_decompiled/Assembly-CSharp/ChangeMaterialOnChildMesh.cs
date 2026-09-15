using System;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class ChangeMaterialOnChildMesh : MonoBehaviour
{
	// Token: 0x0600101E RID: 4126 RVA: 0x0004FFB8 File Offset: 0x0004E1B8
	public void Go()
	{
		MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material = this.material;
		}
	}

	// Token: 0x04000E8B RID: 3723
	public Material material;
}
