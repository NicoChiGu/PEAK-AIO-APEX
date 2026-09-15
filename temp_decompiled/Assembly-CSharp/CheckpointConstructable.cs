using System;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class CheckpointConstructable : Constructable
{
	// Token: 0x0600089A RID: 2202 RVA: 0x0002EE58 File Offset: 0x0002D058
	public override GameObject FinishConstruction()
	{
		GameObject gameObject = base.FinishConstruction();
		CheckpointFlag checkpointFlag;
		if (gameObject && gameObject.TryGetComponent<CheckpointFlag>(ref checkpointFlag))
		{
			checkpointFlag.Initialize(this.item.holderCharacter);
		}
		else
		{
			Debug.LogWarning("Failed to construct our checkpoint flag! That's bad, right?");
		}
		return gameObject;
	}
}
