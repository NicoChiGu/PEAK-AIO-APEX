using System;
using UnityEngine;

// Token: 0x02000365 RID: 869
public class VariantObject : MonoBehaviour
{
	// Token: 0x04001508 RID: 5384
	[Range(0f, 1f)]
	public float spawnChance = 0.5f;

	// Token: 0x04001509 RID: 5385
	public VariantObject.Group group;

	// Token: 0x02000521 RID: 1313
	public enum Group
	{
		// Token: 0x04001BBB RID: 7099
		Default,
		// Token: 0x04001BBC RID: 7100
		One,
		// Token: 0x04001BBD RID: 7101
		Two,
		// Token: 0x04001BBE RID: 7102
		Three
	}
}
