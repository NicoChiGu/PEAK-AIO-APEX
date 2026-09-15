using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000304 RID: 772
public class PSB_ChildSpawners : PostSpawnBehavior
{
	// Token: 0x1700013E RID: 318
	// (get) Token: 0x060013FF RID: 5119 RVA: 0x000652B5 File Offset: 0x000634B5
	protected override DeferredStepTiming DefaultTiming
	{
		get
		{
			return DeferredStepTiming.AfterCurrentStep;
		}
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x000652B8 File Offset: 0x000634B8
	public override void RunBehavior(IEnumerable<GameObject> spawned)
	{
		int num = 0;
		int num2 = 0;
		string text = string.Empty;
		foreach (GameObject gameObject in spawned)
		{
			num2++;
			if (gameObject == null)
			{
				num++;
			}
			else
			{
				text = gameObject.name;
				LevelGenStep[] componentsInChildren = gameObject.GetComponentsInChildren<LevelGenStep>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Execute();
				}
			}
		}
		if (num > 0)
		{
			string text2 = ((text == string.Empty) ? "objects" : text);
			Debug.LogError(string.Format("Found {0} null {1} in our list of {2} total child spawners.", num, text2, num2));
		}
	}
}
