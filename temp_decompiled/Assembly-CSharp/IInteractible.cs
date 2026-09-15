using System;
using UnityEngine;

// Token: 0x02000020 RID: 32
public interface IInteractible
{
	// Token: 0x06000253 RID: 595
	bool IsInteractible(Character interactor);

	// Token: 0x06000254 RID: 596
	void Interact(Character interactor);

	// Token: 0x06000255 RID: 597
	void HoverEnter();

	// Token: 0x06000256 RID: 598
	void HoverExit();

	// Token: 0x06000257 RID: 599
	Vector3 Center();

	// Token: 0x06000258 RID: 600
	Transform GetTransform();

	// Token: 0x06000259 RID: 601
	string GetInteractionText();

	// Token: 0x0600025A RID: 602
	string GetName();
}
