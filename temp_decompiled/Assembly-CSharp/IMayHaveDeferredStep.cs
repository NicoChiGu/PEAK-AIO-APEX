using System;

// Token: 0x0200028B RID: 651
public interface IMayHaveDeferredStep
{
	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06001223 RID: 4643
	DeferredStepTiming DeferredTiming { get; }

	// Token: 0x06001224 RID: 4644
	IDeferredStep ConstructDeferred(IMayHaveDeferredStep source);
}
