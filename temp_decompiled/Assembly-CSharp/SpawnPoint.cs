using System;
using System.Collections.Generic;
using System.Linq;
using Peak.Network;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class SpawnPoint : MonoBehaviour
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x060003A6 RID: 934 RVA: 0x00018459 File Offset: 0x00016659
	public static SpawnPoint LocalSpawnPoint
	{
		get
		{
			return SpawnPoint.GetSpawnPoint(NetCode.Session.SeatNumber);
		}
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0001846C File Offset: 0x0001666C
	public static SpawnPoint GetSpawnPoint(int actorNumber)
	{
		int spawnIndex = actorNumber % SpawnPoint.allSpawnPoints.Count;
		SpawnPoint spawnPoint = Enumerable.FirstOrDefault<SpawnPoint>(SpawnPoint.allSpawnPoints, (SpawnPoint s) => s.index == spawnIndex);
		if (spawnPoint == null)
		{
			spawnPoint = Enumerable.FirstOrDefault<SpawnPoint>(SpawnPoint.allSpawnPoints);
		}
		if (spawnPoint == null)
		{
			Debug.LogError("No spawn points in this scene! Creating a dummy one but it will suck.");
			spawnPoint = new GameObject("DummySpawnPoint").AddComponent<SpawnPoint>();
		}
		return spawnPoint;
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x000184DA File Offset: 0x000166DA
	private void Awake()
	{
		SpawnPoint.allSpawnPoints.Add(this);
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x000184E7 File Offset: 0x000166E7
	private void OnDestroy()
	{
		SpawnPoint.allSpawnPoints.Remove(this);
	}

	// Token: 0x040003FE RID: 1022
	public int index;

	// Token: 0x040003FF RID: 1023
	public bool startPassedOut;

	// Token: 0x04000400 RID: 1024
	public static List<SpawnPoint> allSpawnPoints = new List<SpawnPoint>();
}
