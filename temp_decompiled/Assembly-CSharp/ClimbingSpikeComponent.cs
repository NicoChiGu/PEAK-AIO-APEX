using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000019 RID: 25
public class ClimbingSpikeComponent : ItemComponent
{
	// Token: 0x06000223 RID: 547 RVA: 0x000106B9 File Offset: 0x0000E8B9
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x06000224 RID: 548 RVA: 0x000106BB File Offset: 0x0000E8BB
	private void Start()
	{
		this.item.overrideUsability = Optionable<bool>.Some(false);
	}

	// Token: 0x04000201 RID: 513
	public GameObject hammeredVersionPrefab;

	// Token: 0x04000202 RID: 514
	public GameObject climbingSpikePreviewPrefab;

	// Token: 0x04000203 RID: 515
	public float climbingSpikeStartDistance;

	// Token: 0x04000204 RID: 516
	public float climbingSpikePreviewDisableDistance;

	// Token: 0x04000205 RID: 517
	public float climbingSpikeStartDistanceGrounded;

	// Token: 0x04000206 RID: 518
	public float climbingSpikePreviewDisableDistanceGrounded;
}
