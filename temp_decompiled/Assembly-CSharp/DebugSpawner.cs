using System;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class DebugSpawner : MonoBehaviour
{
	// Token: 0x060010F2 RID: 4338 RVA: 0x00055B0C File Offset: 0x00053D0C
	private void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(48))
		{
			this.Spawn();
		}
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x00055B24 File Offset: 0x00053D24
	private void Spawn()
	{
		Object.Instantiate<GameObject>(this.objToSpawn, HelperFunctions.LineCheck(MainCamera.instance.transform.position, MainCamera.instance.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.TerrainMap, 0f, 1).point, Quaternion.identity);
	}

	// Token: 0x04000F6D RID: 3949
	public GameObject objToSpawn;
}
