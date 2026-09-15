using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class MapGenerator : MonoBehaviour
{
	// Token: 0x0600066D RID: 1645 RVA: 0x00024E0C File Offset: 0x0002300C
	public void GenerateAll()
	{
		if (this.seed != 0)
		{
			Debug.Log("Set Seed");
			Random.InitState(this.seed);
		}
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (this.stages[i].gameObject.activeInHierarchy)
			{
				this.stages[i].Generate(0);
				Debug.Log(i.ToString() + " " + Random.state.GetHashCode().ToString());
			}
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00024EA8 File Offset: 0x000230A8
	public void ClearAll()
	{
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (this.stages[i].gameObject.activeInHierarchy)
			{
				this.stages[i].ClearSpawnedObjects();
			}
		}
	}

	// Token: 0x0400067E RID: 1662
	public int seed;

	// Token: 0x0400067F RID: 1663
	public List<MapGenerationStage> stages;
}
