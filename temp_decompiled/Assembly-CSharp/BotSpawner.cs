using System;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200006A RID: 106
public class BotSpawner : MonoBehaviour
{
	// Token: 0x060004EA RID: 1258 RVA: 0x0001D158 File Offset: 0x0001B358
	private void Go()
	{
		this.SpawnBot(PatrolBoss.me.transform.position);
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x0001D170 File Offset: 0x0001B370
	public void SpawnBot(Vector3 spawnPosition)
	{
		bool flag = false;
		for (int i = 0; i < 10; i++)
		{
			if (this.<SpawnBot>g__TrySpawnBot|2_0(spawnPosition + ExtSwizzle.xoy(ExtMath.RandInsideUnitCircle()) * 2f))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Debug.LogWarning("Could not spawn troop");
		}
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x0001D1C8 File Offset: 0x0001B3C8
	[CompilerGenerated]
	private bool <SpawnBot>g__TrySpawnBot|2_0(Vector3 spawnPosition)
	{
		foreach (Collider collider in Physics.OverlapSphere(spawnPosition, 2f))
		{
			if (collider.gameObject.layer != LayerMask.NameToLayer("Terrain") && collider.gameObject.layer != LayerMask.NameToLayer("Prop"))
			{
				return false;
			}
		}
		Object.Instantiate<GameObject>(this.botPrefab, spawnPosition, Quaternion.identity);
		Debug.Log("Spawn Bot");
		return true;
	}

	// Token: 0x04000557 RID: 1367
	public GameObject botPrefab;
}
