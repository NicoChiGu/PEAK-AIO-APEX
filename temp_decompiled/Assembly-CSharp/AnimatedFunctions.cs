using System;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class AnimatedFunctions : MonoBehaviour
{
	// Token: 0x06000F41 RID: 3905 RVA: 0x0004B6B7 File Offset: 0x000498B7
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x0004B6C5 File Offset: 0x000498C5
	private void Start()
	{
		this.left = this.character.GetBodypart(BodypartType.Foot_L).GetComponentInChildren<RigCreatorCollider>().GetComponent<Collider>();
		this.right = this.character.GetBodypart(BodypartType.Foot_R).GetComponentInChildren<RigCreatorCollider>().GetComponent<Collider>();
	}

	// Token: 0x04000D88 RID: 3464
	private Collider left;

	// Token: 0x04000D89 RID: 3465
	private Collider right;

	// Token: 0x04000D8A RID: 3466
	private Character character;
}
