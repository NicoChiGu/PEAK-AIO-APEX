using System;
using UnityEngine;

// Token: 0x0200028F RID: 655
public abstract class LevelGenStep : MonoBehaviour, IMayHaveDeferredStep
{
	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06001229 RID: 4649 RVA: 0x0005C2A0 File Offset: 0x0005A4A0
	public virtual DeferredStepTiming DeferredTiming
	{
		get
		{
			return DeferredStepTiming.None;
		}
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x0005C2A3 File Offset: 0x0005A4A3
	public virtual IDeferredStep ConstructDeferred(IMayHaveDeferredStep _)
	{
		return null;
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x0005C2A6 File Offset: 0x0005A4A6
	public virtual void DeferredGo()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600122C RID: 4652
	public abstract void Execute();

	// Token: 0x0600122D RID: 4653
	public abstract void Clear();
}
