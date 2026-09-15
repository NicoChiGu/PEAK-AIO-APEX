using System;
using UnityEngine;

// Token: 0x0200023E RID: 574
public abstract class CustomSpawnCondition : MonoBehaviour
{
	// Token: 0x060010D7 RID: 4311
	public abstract bool CheckCondition(PropSpawner.SpawnData data);
}
