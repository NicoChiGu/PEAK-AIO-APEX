using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class BerryBush : Spawner
{
	// Token: 0x06000623 RID: 1571 RVA: 0x000232CC File Offset: 0x000214CC
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		List<Transform> list2 = new List<Transform>(spawnSpots);
		GameObject gameObject;
		if (this.spawnMode == Spawner.SpawnMode.SingleItem)
		{
			gameObject = this.spawnedObjectPrefab;
		}
		else
		{
			gameObject = LootData.GetRandomItem(this.spawnPool);
		}
		float num = Random.value;
		num = Mathf.Pow(num, this.randomPow);
		int num2 = Mathf.RoundToInt(Mathf.Lerp(this.possibleBerries.x, this.possibleBerries.y, num));
		int num3 = 0;
		while (num3 < spawnSpots.Count && num3 < num2)
		{
			int num4 = Random.Range(0, list2.Count);
			if (!(gameObject == null))
			{
				Item component = PhotonNetwork.InstantiateItemRoom(gameObject.name, list2[num4].position, Quaternion.identity).GetComponent<Item>();
				list.Add(component.GetComponent<PhotonView>());
				if (this.spawnUpTowardsTarget)
				{
					component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
					component.transform.Rotate(Vector3.up, Random.Range(0f, 360f), 1);
				}
				if (this.spawnTransformIsSpawnerTransform)
				{
					component.transform.rotation = list2[num4].rotation;
					component.transform.Rotate(Vector3.up, Random.Range(0f, 360f), 1);
				}
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
			}
			num3++;
		}
		return list;
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x000234B0 File Offset: 0x000216B0
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		for (int i = 0; i < this.spawnSpots.Count; i++)
		{
			Gizmos.DrawSphere(this.spawnSpots[i].position, 0.25f);
		}
	}

	// Token: 0x0400062F RID: 1583
	public Vector2 possibleBerries;

	// Token: 0x04000630 RID: 1584
	public float randomPow = 1f;
}
