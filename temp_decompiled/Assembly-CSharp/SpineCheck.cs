using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200033E RID: 830
public class SpineCheck : CustomSpawnCondition
{
	// Token: 0x06001562 RID: 5474 RVA: 0x0006D5BC File Offset: 0x0006B7BC
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		Transform transform = base.transform.Find("Spine");
		for (int i = 0; i < transform.childCount - 1; i++)
		{
			Transform child = transform.GetChild(i);
			Transform child2 = transform.GetChild(i + 1);
			if (HelperFunctions.LineCheck(child.position, child2.position, this.layerType, 0f, 1).transform)
			{
				return false;
			}
		}
		this.successEvent.Invoke();
		return true;
	}

	// Token: 0x04001409 RID: 5129
	public HelperFunctions.LayerType layerType;

	// Token: 0x0400140A RID: 5130
	public UnityEvent successEvent;
}
