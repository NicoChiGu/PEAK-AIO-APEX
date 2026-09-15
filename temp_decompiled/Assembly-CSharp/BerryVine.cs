using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class BerryVine : Spawner
{
	// Token: 0x06000626 RID: 1574 RVA: 0x0002350C File Offset: 0x0002170C
	protected override List<Transform> GetSpawnSpots()
	{
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		List<Transform> list = new List<Transform>();
		for (int i = 1; i < componentsInChildren.Length - 1; i++)
		{
			list.Add(componentsInChildren[i].transform);
		}
		return list;
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00023548 File Offset: 0x00021748
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		List<Transform> list2 = new List<Transform>(spawnSpots);
		GameObject gameObject = this.spawns.GetSpawns(1, true)[0];
		float num = Random.value;
		num = Mathf.Pow(num, this.randomPow);
		int num2 = Mathf.RoundToInt(Mathf.Lerp(this.possibleBerries.x, this.possibleBerries.y, num));
		int num3 = 0;
		while (num3 < spawnSpots.Count && num3 < num2)
		{
			int num4 = Random.Range(0, list2.Count);
			Item component = PhotonNetwork.InstantiateItemRoom(gameObject.name, list2[num4].position + this.spawnOffsetWorldSpace, Quaternion.identity).GetComponent<Item>();
			list.Add(component.GetComponent<PhotonView>());
			if (this.spawnUpTowardsTarget)
			{
				component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
			}
			component.transform.rotation = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
			if (component != null)
			{
				component.GetComponent<PhotonView>().RPC("SetKinematicRPC", 3, new object[]
				{
					true,
					component.transform.position,
					component.transform.rotation
				});
			}
			list2.RemoveAt(num4);
			num3++;
		}
		return list;
	}

	// Token: 0x04000631 RID: 1585
	public Vector2 possibleBerries;

	// Token: 0x04000632 RID: 1586
	public Vector3 spawnOffsetWorldSpace;

	// Token: 0x04000633 RID: 1587
	public float randomPow = 1f;
}
