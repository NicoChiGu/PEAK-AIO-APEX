using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000017 RID: 23
public class CharacterStats : MonoBehaviour
{
	// Token: 0x0600020A RID: 522 RVA: 0x0000FEDC File Offset: 0x0000E0DC
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000FEEC File Offset: 0x0000E0EC
	public void GetCaughtUp()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		List<EndScreen.TimelineInfo> list = Character.localCharacter.refs.stats.timelineInfo;
		for (int i = 0; i < list.Count; i++)
		{
			EndScreen.TimelineInfo timelineInfo = default(EndScreen.TimelineInfo);
			timelineInfo.time = list[i].time;
			timelineInfo.height = this.heightInUnits;
			this.timelineInfo.Add(timelineInfo);
		}
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0000FF64 File Offset: 0x0000E164
	private void CheckHeightAchievement()
	{
		this.heightInUnits = this.character.HipPos().y;
		this.heightInMeters = (float)Mathf.RoundToInt(this.heightInUnits * CharacterStats.unitsToMeters);
		if (this.character.IsLocal && !this.character.data.dead && !this.character.warping && this.character.data.sinceDead > this.tickRate)
		{
			Singleton<AchievementManager>.Instance.RecordMaxHeight(Mathf.RoundToInt(this.heightInMeters));
		}
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000FFF8 File Offset: 0x0000E1F8
	private void Update()
	{
		if (Time.timeSinceLevelLoad < 3f)
		{
			return;
		}
		if (this.character.warping)
		{
			return;
		}
		this.CheckHeightAchievement();
		this.tick += Time.deltaTime;
		if (this.tick > this.tickRate && !this.won && !this.lost)
		{
			this.tick = 0f;
			if (!this.character.IsLocal && this.timelineInfo.Count == 0)
			{
				this.GetCaughtUp();
			}
			this.Record(false, 0f);
		}
	}

	// Token: 0x0600020E RID: 526 RVA: 0x0001008D File Offset: 0x0000E28D
	public EndScreen.TimelineInfo GetFirstTimelineInfo()
	{
		return this.timelineInfo[0];
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0001009B File Offset: 0x0000E29B
	public EndScreen.TimelineInfo GetFinalTimelineInfo()
	{
		List<EndScreen.TimelineInfo> list = this.timelineInfo;
		return list[list.Count - 1];
	}

	// Token: 0x06000210 RID: 528 RVA: 0x000100B0 File Offset: 0x0000E2B0
	public static int UnitsToMeters(float units)
	{
		return Mathf.RoundToInt(Mathf.Min(units, CharacterStats.peakHeightInUnits) * CharacterStats.unitsToMeters);
	}

	// Token: 0x06000211 RID: 529 RVA: 0x000100C8 File Offset: 0x0000E2C8
	private static List<EndScreen.TimelineInfo> Downsample(List<EndScreen.TimelineInfo> originalSeries, int numSamplesNeeded)
	{
		return null;
	}

	// Token: 0x06000212 RID: 530 RVA: 0x000100D8 File Offset: 0x0000E2D8
	public void Record(bool useOverridePosition = false, float overrideHeight = 0f)
	{
		float num = (useOverridePosition ? overrideHeight : this.heightInUnits);
		if (num > 2000f)
		{
			return;
		}
		EndScreen.TimelineInfo timelineInfo = default(EndScreen.TimelineInfo);
		timelineInfo.height = num;
		timelineInfo.time = RunManager.Instance.timeSinceRunStarted;
		if (this.justDied)
		{
			this.justDied = false;
			timelineInfo.died = true;
		}
		else if (this.character.data.dead)
		{
			timelineInfo.dead = true;
		}
		else if (this.justRevived)
		{
			this.justRevived = false;
			timelineInfo.revived = true;
			Debug.LogError("RECORD REVIVED!");
		}
		else if (this.justPassedOut)
		{
			this.justPassedOut = false;
			timelineInfo.justPassedOut = true;
		}
		else if (this.character.data.passedOut)
		{
			timelineInfo.passedOut = true;
		}
		this.timelineInfo.Add(timelineInfo);
	}

	// Token: 0x06000213 RID: 531 RVA: 0x000101B4 File Offset: 0x0000E3B4
	public void Win()
	{
		this.won = true;
		if (this.character.IsLocal)
		{
			EndScreen.TimelineInfo timelineInfo = this.timelineInfo[this.timelineInfo.Count - 1];
			timelineInfo.won = true;
			GlobalEvents.TriggerLocalCharacterWonRun();
			this.timelineInfo[this.timelineInfo.Count - 1] = timelineInfo;
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00010214 File Offset: 0x0000E414
	public void Lose(bool somebodyElseWon)
	{
		this.lost = true;
		this.somebodyElseWon = somebodyElseWon;
	}

	// Token: 0x040001E2 RID: 482
	public static float peakHeightInUnits = 1200f;

	// Token: 0x040001E3 RID: 483
	private Character character;

	// Token: 0x040001E4 RID: 484
	public float heightInUnits;

	// Token: 0x040001E5 RID: 485
	public float heightInMeters;

	// Token: 0x040001E6 RID: 486
	public static float unitsToMeters = 1.6f;

	// Token: 0x040001E7 RID: 487
	private float tick;

	// Token: 0x040001E8 RID: 488
	public float tickRate = 1f;

	// Token: 0x040001E9 RID: 489
	public List<EndScreen.TimelineInfo> timelineInfo = new List<EndScreen.TimelineInfo>();

	// Token: 0x040001EA RID: 490
	public bool won;

	// Token: 0x040001EB RID: 491
	public bool lost;

	// Token: 0x040001EC RID: 492
	public bool somebodyElseWon;

	// Token: 0x040001ED RID: 493
	public bool justDied;

	// Token: 0x040001EE RID: 494
	public bool justPassedOut;

	// Token: 0x040001EF RID: 495
	public bool justRevived;
}
