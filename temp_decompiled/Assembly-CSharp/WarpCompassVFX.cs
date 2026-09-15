using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200036E RID: 878
public class WarpCompassVFX : ItemVFX
{
	// Token: 0x06001657 RID: 5719 RVA: 0x00073B65 File Offset: 0x00071D65
	private new void Start()
	{
		base.Start();
		GameUtils instance = GameUtils.instance;
		instance.OnUpdatedFeedData = (Action)Delegate.Combine(instance.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x00073B93 File Offset: 0x00071D93
	private void OnDestroy()
	{
		GameUtils instance = GameUtils.instance;
		instance.OnUpdatedFeedData = (Action)Delegate.Remove(instance.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x00073BBC File Offset: 0x00071DBC
	protected override void Update()
	{
		base.Update();
		float num = this.item.castProgress;
		if ((!this.item.isUsingPrimary || this.item.finishedCast) && this.timeStartedBeingUsedOnMe == 0f)
		{
			num = 0f;
		}
		else if (this.timeStartedBeingUsedOnMe > 0f)
		{
			num = (Time.time - this.timeStartedBeingUsedOnMe) / this.item.totalSecondaryUsingTime;
		}
		this.warpPost.enabled = this.warpPost.weight > 0.01f;
		this.warpPost2.enabled = this.warpPost2.weight > 0.01f;
		if (this.warpPost2.weight >= 1f)
		{
			this.warpPost.weight = 0f;
		}
		else
		{
			this.warpPost.weight = Mathf.Lerp(this.warpPost.weight, num, Time.deltaTime * 10f);
		}
		this.warpPost2.weight = this.warpPost2Curve.Evaluate(this.warpPost.weight);
		this.compassPointer.speedMultiplier = 1f + this.warpPost.weight * 4f;
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x00073CF8 File Offset: 0x00071EF8
	protected override void Shake()
	{
		GamefeelHandler.instance.AddPerlinShake(this.warpPost.weight * this.shakeAmount * Time.deltaTime * 100f, 0.2f, 15f);
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x00073D2C File Offset: 0x00071F2C
	private void OnUpdatedFeedData()
	{
		bool flag = false;
		using (List<FeedData>.Enumerator enumerator = GameUtils.instance.GetFeedDataForReceiver(Character.localCharacter.photonView.ViewID).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.itemID == this.item.itemID)
				{
					flag = true;
					if (this.timeStartedBeingUsedOnMe == 0f)
					{
						this.timeStartedBeingUsedOnMe = Time.time;
					}
				}
			}
		}
		if (!flag)
		{
			this.timeStartedBeingUsedOnMe = 0f;
		}
	}

	// Token: 0x04001538 RID: 5432
	public Volume warpPost;

	// Token: 0x04001539 RID: 5433
	public Volume warpPost2;

	// Token: 0x0400153A RID: 5434
	public float maxCastProgress = 1.1f;

	// Token: 0x0400153B RID: 5435
	public AnimationCurve warpPost2Curve;

	// Token: 0x0400153C RID: 5436
	public CompassPointer compassPointer;

	// Token: 0x0400153D RID: 5437
	public float timeStartedBeingUsedOnMe;
}
