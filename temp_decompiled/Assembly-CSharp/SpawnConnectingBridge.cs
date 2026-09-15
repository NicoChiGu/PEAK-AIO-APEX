using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class SpawnConnectingBridge : CustomSpawnCondition
{
	// Token: 0x06000D4A RID: 3402 RVA: 0x00042C38 File Offset: 0x00040E38
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		SpawnConnectingBridge.parent = base.transform.parent;
		this.treePlatforms = SpawnConnectingBridge.parent.GetComponentsInChildren<TreePlatform>();
		Debug.Log("Checking: " + this.treePlatforms.Length.ToString());
		return true;
	}

	// Token: 0x04000B81 RID: 2945
	public static Transform parent;

	// Token: 0x04000B82 RID: 2946
	public TreePlatform[] treePlatforms;
}
