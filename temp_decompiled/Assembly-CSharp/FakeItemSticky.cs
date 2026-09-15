using System;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class FakeItemSticky : FakeItem
{
	// Token: 0x06000233 RID: 563 RVA: 0x00010926 File Offset: 0x0000EB26
	private void Start()
	{
		if (Application.isPlaying)
		{
			CollisionModifier component = this.physicalCollider.GetComponent<CollisionModifier>();
			component.onCollide = (Action<Character, CollisionModifier, Collision, Bodypart>)Delegate.Combine(component.onCollide, new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide));
		}
	}

	// Token: 0x06000234 RID: 564 RVA: 0x0001095C File Offset: 0x0000EB5C
	private void OnCollide(Character character, CollisionModifier modifier, Collision collision, Bodypart bodyPart)
	{
		if (!character.IsLocal)
		{
			return;
		}
		if (character.data.isInvincible)
		{
			return;
		}
		if (this.collided)
		{
			return;
		}
		this.collided = true;
		FakeItemManager.Instance.photonView.RPC("RPC_RequestStickFakeItemToPlayer", 2, new object[]
		{
			character.photonView.ViewID,
			this.index,
			(int)bodyPart.partType,
			base.transform.position - bodyPart.transform.position
		});
	}

	// Token: 0x06000235 RID: 565 RVA: 0x000109FF File Offset: 0x0000EBFF
	public override void UnPickUpVisibly()
	{
		base.UnPickUpVisibly();
		this.collided = false;
	}

	// Token: 0x0400020E RID: 526
	public bool stickOnCollision = true;

	// Token: 0x0400020F RID: 527
	public Collider physicalCollider;

	// Token: 0x04000210 RID: 528
	private bool collided;
}
