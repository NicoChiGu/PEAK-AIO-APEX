using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class BingBongShieldWhileHolding : ItemComponent
{
	// Token: 0x06000880 RID: 2176 RVA: 0x0002E669 File Offset: 0x0002C869
	private void Start()
	{
		this.TryApplyInvincibility();
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0002E674 File Offset: 0x0002C874
	private void Update()
	{
		this.tick += Time.deltaTime;
		if (this.tick >= 1.5f)
		{
			this.TryApplyInvincibility();
			this.tick = 0f;
		}
		if (!this.wasHeldByLocal && Character.localCharacter.data.currentItem == this.item)
		{
			this.wasHeldByLocal = true;
		}
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0002E6DC File Offset: 0x0002C8DC
	private void TryApplyInvincibility()
	{
		if (Character.localCharacter && Character.localCharacter.data.currentItem == this.item)
		{
			Character.localCharacter.refs.afflictions.AddAffliction(new Affliction_BingBongShield
			{
				totalTime = 2f
			}, false);
		}
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x0002E736 File Offset: 0x0002C936
	private void OnDestroy()
	{
		if (this.wasHeldByLocal)
		{
			Character.localCharacter.refs.afflictions.RemoveAffliction(Affliction.AfflictionType.BingBongShield);
		}
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0002E756 File Offset: 0x0002C956
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x04000822 RID: 2082
	private bool wasHeldByLocal;

	// Token: 0x04000823 RID: 2083
	private float tick;
}
