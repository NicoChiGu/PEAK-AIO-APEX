using System;
using System.Collections.Generic;

// Token: 0x0200028D RID: 653
public struct ExecuteDeferredStepList : IDeferredStep
{
	// Token: 0x06001226 RID: 4646 RVA: 0x0005C23C File Offset: 0x0005A43C
	public ExecuteDeferredStepList(List<IDeferredStep> steps)
	{
		this._steps = steps;
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x0005C248 File Offset: 0x0005A448
	public void DeferredGo()
	{
		foreach (IDeferredStep deferredStep in this._steps)
		{
			deferredStep.DeferredGo();
		}
	}

	// Token: 0x040010AC RID: 4268
	private List<IDeferredStep> _steps;
}
