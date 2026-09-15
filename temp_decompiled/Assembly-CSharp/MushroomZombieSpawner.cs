using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class MushroomZombieSpawner : MonoBehaviour
{
	// Token: 0x06000A66 RID: 2662 RVA: 0x00037330 File Offset: 0x00035530
	private void OnDestroy()
	{
		if (ZombieManager.Instance)
		{
			ZombieManager.Instance.spawners.Remove(this);
		}
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x00037350 File Offset: 0x00035550
	private void Start()
	{
		if (Ascents.currentAscent == -1)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (this.cullRandomly && Random.Range(0f, 1f) > 0.333f)
		{
			Debug.Log("Destroying zombie spawner.");
			Object.Destroy(base.gameObject);
			return;
		}
		if (!ZombieManager.Instance.spawners.Contains(this))
		{
			ZombieManager.Instance.spawners.Add(this);
		}
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x000373C8 File Offset: 0x000355C8
	public bool ReadyToSpawn()
	{
		if (this.spawnedZombie != null)
		{
			return false;
		}
		if (this.spawned)
		{
			return false;
		}
		using (List<Character>.Enumerator enumerator = Character.AllCharacters.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.Center, base.transform.position) <= this.mushroomZombiePrefab.distanceToEnable)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x00037458 File Offset: 0x00035658
	public void Spawn()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.spawned = true;
		Debug.Log("Spawning new zombie");
		this.spawnedZombie = PhotonNetwork.Instantiate(this.mushroomZombiePrefab.gameObject.name, base.transform.position, base.transform.rotation, 0, null).GetComponent<MushroomZombie>();
		this.spawnedZombie.spawner = this;
	}

	// Token: 0x040009D0 RID: 2512
	public MushroomZombie mushroomZombiePrefab;

	// Token: 0x040009D1 RID: 2513
	public MushroomZombie spawnedZombie;

	// Token: 0x040009D2 RID: 2514
	public const float SPAWN_CHANCE = 0.333f;

	// Token: 0x040009D3 RID: 2515
	public bool cullRandomly = true;

	// Token: 0x040009D4 RID: 2516
	private bool spawned;
}
