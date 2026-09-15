using System;

// Token: 0x02000021 RID: 33
public interface IInteractibleConstant : IInteractible
{
	// Token: 0x0600025B RID: 603
	bool IsConstantlyInteractable(Character interactor);

	// Token: 0x0600025C RID: 604
	float GetInteractTime(Character interactor);

	// Token: 0x0600025D RID: 605
	void Interact_CastFinished(Character interactor);

	// Token: 0x0600025E RID: 606
	void CancelCast(Character interactor);

	// Token: 0x0600025F RID: 607
	void ReleaseInteract(Character interactor);

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000260 RID: 608
	bool holdOnFinish { get; }
}
