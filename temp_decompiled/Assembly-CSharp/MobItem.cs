using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class MobItem : Item
{
	// Token: 0x060002F4 RID: 756 RVA: 0x00014A2C File Offset: 0x00012C2C
	protected override void Awake()
	{
		base.Awake();
		this.mob = base.GetComponent<Mob>();
		this.syncer = base.GetComponent<MobItemPhysicsSyncer>();
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00014A4C File Offset: 0x00012C4C
	protected override void Start()
	{
		base.Start();
		this.mob.forceNoMovement = base.itemState != ItemState.Ground || !base.photonView.IsMine;
		if (base.cooking.timesCookedLocal > 0)
		{
			this.mob.anim.Play("ScorpionCooked", 0, 1f);
		}
		if (base.itemState == ItemState.Held && Character.localCharacter.data.currentItem == this)
		{
			this.mob.SetForcedTarget(Character.localCharacter);
		}
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00014ADC File Offset: 0x00012CDC
	protected override void Update()
	{
		this.syncer.shouldSync = !this.mob.sleeping;
		this.UIData.hasMainInteract = base.cooking.timesCookedLocal > 0;
		this.canUseOnFriend = base.cooking.timesCookedLocal > 0;
		if (base.cooking.timesCookedLocal > 0)
		{
			this.mob.mobState = Mob.MobState.Dead;
			return;
		}
		if (this.mob.sleeping)
		{
			return;
		}
		this.mob.forceNoMovement = base.itemState != ItemState.Ground || !base.photonView.IsMine;
		if (base.photonView.IsMine && base.itemState == ItemState.Ground)
		{
			base.ForceSyncForFrames(10);
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00014B99 File Offset: 0x00012D99
	public override bool CanUsePrimary()
	{
		return base.cooking.timesCookedLocal > 0;
	}

	// Token: 0x040002C2 RID: 706
	protected Mob mob;

	// Token: 0x040002C3 RID: 707
	private MobItemPhysicsSyncer syncer;

	// Token: 0x040002C4 RID: 708
	public float sleepDistance = 50f;

	// Token: 0x040002C5 RID: 709
	public Animator anim;
}
