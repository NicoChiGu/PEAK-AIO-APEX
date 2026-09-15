using System;
using UnityEngine;

// Token: 0x020002EF RID: 751
[Serializable]
public class MatAndID
{
	// Token: 0x060013CC RID: 5068 RVA: 0x000647B6 File Offset: 0x000629B6
	public MatAndID(Material mat, int id)
	{
		this.mat = mat;
		this.id = id;
	}

	// Token: 0x04001263 RID: 4707
	public Material mat;

	// Token: 0x04001264 RID: 4708
	public int id;
}
