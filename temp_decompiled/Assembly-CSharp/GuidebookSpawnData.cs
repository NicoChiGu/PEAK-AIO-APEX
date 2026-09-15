using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class GuidebookSpawnData : MonoBehaviour
{
	// Token: 0x06000251 RID: 593 RVA: 0x000117BE File Offset: 0x0000F9BE
	public bool CanSpawnRightNow()
	{
		return Character.localCharacter.Center.y >= this.minHeightToSpawn;
	}

	// Token: 0x04000239 RID: 569
	public float minHeightToSpawn;
}
