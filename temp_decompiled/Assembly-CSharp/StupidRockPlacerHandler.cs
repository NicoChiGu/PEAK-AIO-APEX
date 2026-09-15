using System;
using UnityEngine;

// Token: 0x02000347 RID: 839
public class StupidRockPlacerHandler : MonoBehaviour
{
	// Token: 0x06001589 RID: 5513 RVA: 0x0006EC2B File Offset: 0x0006CE2B
	private void Start()
	{
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x0006EC30 File Offset: 0x0006CE30
	private void ReDo()
	{
		StupidRockPlacer[] componentsInChildren = base.GetComponentsInChildren<StupidRockPlacer>();
		StupidRockPlacer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		foreach (StupidRockPlacer stupidRockPlacer in componentsInChildren)
		{
			int num = stupidRockPlacer.amount;
			stupidRockPlacer.amount = (int)(this.amount * (float)stupidRockPlacer.amount);
			stupidRockPlacer.Go();
			stupidRockPlacer.amount = num;
		}
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x0006EC9C File Offset: 0x0006CE9C
	private void Clear()
	{
		StupidRockPlacer[] componentsInChildren = base.GetComponentsInChildren<StupidRockPlacer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Clear();
		}
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x0006ECC6 File Offset: 0x0006CEC6
	private void Update()
	{
	}

	// Token: 0x04001459 RID: 5209
	public float amount = 1f;
}
