using System;
using UnityEngine;

// Token: 0x020002DB RID: 731
public class PSM_ChildSpawners : PropSpawnerMod
{
	// Token: 0x060013A4 RID: 5028 RVA: 0x00063D40 File Offset: 0x00061F40
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData _)
	{
		LevelGenStep[] componentsInChildren = spawned.GetComponentsInChildren<LevelGenStep>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Execute();
		}
	}
}
