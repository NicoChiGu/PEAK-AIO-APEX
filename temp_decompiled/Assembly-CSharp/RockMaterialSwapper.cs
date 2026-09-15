using System;
using UnityEngine;

// Token: 0x02000314 RID: 788
public class RockMaterialSwapper : MonoBehaviour
{
	// Token: 0x06001461 RID: 5217 RVA: 0x000677E0 File Offset: 0x000659E0
	private void Start()
	{
		Transform[] array = this.parents;
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer[] componentsInChildren = array[i].GetComponentsInChildren<MeshRenderer>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].sharedMaterial = this.mat;
			}
		}
	}

	// Token: 0x040012F1 RID: 4849
	public Transform[] parents;

	// Token: 0x040012F2 RID: 4850
	public Material mat;
}
