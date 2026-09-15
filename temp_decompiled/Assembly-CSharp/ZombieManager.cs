using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

// Token: 0x020001F5 RID: 501
public class ZombieManager : MonoBehaviourPunCallbacks
{
	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000F1D RID: 3869 RVA: 0x0004A048 File Offset: 0x00048248
	public static ZombieManager Instance
	{
		get
		{
			if (ZombieManager._instance == null)
			{
				ZombieManager._instance = GameUtils.instance.GetComponent<ZombieManager>();
				if (ZombieManager._instance == null)
				{
					ZombieManager._instance = GameUtils.instance.gameObject.AddComponent<ZombieManager>();
				}
			}
			return ZombieManager._instance;
		}
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0004A098 File Offset: 0x00048298
	public void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		for (int i = this.zombies.Count - 1; i >= 0; i--)
		{
			MushroomZombie mushroomZombie = this.zombies[i];
			if (mushroomZombie.ReadyToDisable())
			{
				mushroomZombie.DestroyZombie();
				return;
			}
		}
		if (this.zombies.Count < this.maxActiveZombies)
		{
			for (int j = this.spawners.Count - 1; j >= 0; j--)
			{
				MushroomZombieSpawner mushroomZombieSpawner = this.spawners[j];
				if (mushroomZombieSpawner.ReadyToSpawn())
				{
					mushroomZombieSpawner.Spawn();
					return;
				}
			}
		}
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x0004A128 File Offset: 0x00048328
	[PunRPC]
	private void RPC_EnableZombie(int zombieID)
	{
		MushroomZombie component = PhotonNetwork.GetPhotonView(zombieID).GetComponent<MushroomZombie>();
		if (component)
		{
			base.StartCoroutine(this.EnableZombie(component));
		}
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x0004A157 File Offset: 0x00048357
	private IEnumerator EnableZombie(MushroomZombie zombie)
	{
		yield return null;
		if (zombie)
		{
			zombie.character.Start();
			zombie.StartSleeping();
			zombie.StartCoroutine(zombie.RevealZombie());
		}
		yield break;
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x0004A166 File Offset: 0x00048366
	public void RegisterZombie(MushroomZombie zombie)
	{
		if (!this.zombies.Contains(zombie))
		{
			this.zombies.Add(zombie);
		}
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x0004A182 File Offset: 0x00048382
	public void DeRegisterZombie(MushroomZombie zombie)
	{
		this.zombies.Remove(zombie);
	}

	// Token: 0x04000D21 RID: 3361
	private static ZombieManager _instance;

	// Token: 0x04000D22 RID: 3362
	public List<MushroomZombie> zombies = new List<MushroomZombie>();

	// Token: 0x04000D23 RID: 3363
	public List<MushroomZombieSpawner> spawners = new List<MushroomZombieSpawner>();

	// Token: 0x04000D24 RID: 3364
	public int maxActiveZombies;
}
