using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002A6 RID: 678
public class MountainProgressHandler : Singleton<MountainProgressHandler>
{
	// Token: 0x17000124 RID: 292
	// (get) Token: 0x0600128E RID: 4750 RVA: 0x0005E899 File Offset: 0x0005CA99
	// (set) Token: 0x0600128F RID: 4751 RVA: 0x0005E8A1 File Offset: 0x0005CAA1
	public int maxProgressPointReached { get; private set; }

	// Token: 0x06001290 RID: 4752 RVA: 0x0005E8AA File Offset: 0x0005CAAA
	private void Start()
	{
		this.InitProgressPoints();
		GameHandler.GetService<RichPresenceService>().SetState(RichPresenceState.Status_Shore);
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x0005E8C0 File Offset: 0x0005CAC0
	private void InitProgressPoints()
	{
		if (!Singleton<MapHandler>.Instance)
		{
			return;
		}
		List<MountainProgressHandler.ProgressPoint> list = new List<MountainProgressHandler.ProgressPoint>();
		using (List<Biome.BiomeType>.Enumerator enumerator = Singleton<MapHandler>.Instance.biomes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Biome.BiomeType biomeInCurrentMap = enumerator.Current;
				IEnumerable<MountainProgressHandler.ProgressPoint> enumerable = Enumerable.Where<MountainProgressHandler.ProgressPoint>(this.progressPoints, (MountainProgressHandler.ProgressPoint point) => point.biome == biomeInCurrentMap);
				list.AddRange(enumerable);
			}
		}
		list.Add(Enumerable.Last<MountainProgressHandler.ProgressPoint>(this.progressPoints));
		this.progressPoints = list.ToArray();
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x0005E96C File Offset: 0x0005CB6C
	public void SetSegmentComplete(int segment)
	{
		Debug.Log("Segment complete: " + segment.ToString());
		MountainProgressHandler.ProgressPoint progressPoint = this.progressPoints[segment];
		progressPoint.Reached = true;
		this.TriggerReached(progressPoint);
		if (segment > this.maxProgressPointReached)
		{
			this.maxProgressPointReached = segment;
		}
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x0005E9B6 File Offset: 0x0005CBB6
	private void Update()
	{
		this.CheckProgress(true);
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x0005E9C0 File Offset: 0x0005CBC0
	public void CheckProgress(bool playAnimation = true)
	{
		if (!Singleton<MapHandler>.Instance)
		{
			base.enabled = false;
			Debug.LogWarning("No MapHandler in Scene, so no progress to check.");
			return;
		}
		for (int i = 0; i < this.progressPoints.Length; i++)
		{
			if (!this.progressPoints[i].Reached)
			{
				if (this.progressPoints[i].transform != null)
				{
					this.progressPoints[i].Reached = this.CheckReached(this.progressPoints[i]);
				}
				if (playAnimation && this.progressPoints[i].Reached)
				{
					this.TriggerReached(this.progressPoints[i]);
				}
			}
		}
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x0005EA5D File Offset: 0x0005CC5D
	public void DebugTriggerReached()
	{
		this.TriggerReached(this.progressPoints[this.debugProgress]);
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x0005EA72 File Offset: 0x0005CC72
	public void TriggerReached(MountainProgressHandler.ProgressPoint progressPoint)
	{
		if (Time.time > 2f)
		{
			this.CheckAreaAchievement(progressPoint);
			GUIManager.instance.SetHeroTitle(progressPoint.localizedTitle, progressPoint.clip);
			GameHandler.GetService<RichPresenceService>().SetState(MountainProgressHandler.<TriggerReached>g__GetRichPresenceState|14_0(progressPoint));
		}
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x0005EAAD File Offset: 0x0005CCAD
	public bool IsAtPeak(Transform tf)
	{
		return this.IsAtPeak(tf.position);
	}

	// Token: 0x06001298 RID: 4760 RVA: 0x0005EABB File Offset: 0x0005CCBB
	public bool IsAtPeak(Vector3 position)
	{
		return this.progressPoints != null && this.progressPoints.Length != 0 && position.z > Enumerable.Last<MountainProgressHandler.ProgressPoint>(this.progressPoints).transform.position.z;
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x0005EAF4 File Offset: 0x0005CCF4
	private bool CheckReached(MountainProgressHandler.ProgressPoint point)
	{
		return Character.localCharacter && (Character.localCharacter.Center.z > point.transform.position.z && !Character.localCharacter.data.dead && (Singleton<MapHandler>.Instance.BiomeIsPresent(point.biome) || point.biome == Biome.BiomeType.Peak));
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x0005EB60 File Offset: 0x0005CD60
	private void CheckAreaAchievement(MountainProgressHandler.ProgressPoint pointReached)
	{
		if (Character.localCharacter.data.dead)
		{
			return;
		}
		Debug.Log("Checking achievement. We just reached: " + pointReached.title);
		for (int i = 0; i < this.progressPoints.Length; i++)
		{
			if (this.progressPoints[i].achievement == pointReached.achievement)
			{
				return;
			}
			if (this.progressPoints[i].achievement != ACHIEVEMENTTYPE.NONE)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(this.progressPoints[i].achievement);
			}
			if (this.progressPoints[i].biome == Biome.BiomeType.Mesa)
			{
				Singleton<AchievementManager>.Instance.TestCoolCucumberAchievement();
			}
			else if (this.progressPoints[i].biome == Biome.BiomeType.Alpine)
			{
				Singleton<AchievementManager>.Instance.TestBundledUpAchievement();
			}
			else if (this.progressPoints[i].biome == Biome.BiomeType.Roots)
			{
				Singleton<AchievementManager>.Instance.TestTreadLightlyAchievement();
			}
		}
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x0005EC44 File Offset: 0x0005CE44
	[CompilerGenerated]
	internal static RichPresenceState <TriggerReached>g__GetRichPresenceState|14_0(MountainProgressHandler.ProgressPoint p)
	{
		string title = p.title;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(title);
		if (num <= 2351483618U)
		{
			if (num <= 359194709U)
			{
				if (num != 283912690U)
				{
					if (num == 359194709U)
					{
						if (title == "TROPICS")
						{
							return RichPresenceState.Status_Tropics;
						}
					}
				}
				else if (title == "SHORE")
				{
					return RichPresenceState.Status_Shore;
				}
			}
			else if (num != 1404730151U)
			{
				if (num == 2351483618U)
				{
					if (title == "ROOTS")
					{
						return RichPresenceState.Status_Roots;
					}
				}
			}
			else if (title == "MESA")
			{
				return RichPresenceState.Status_Mesa;
			}
		}
		else if (num <= 2698620434U)
		{
			if (num != 2684125921U)
			{
				if (num == 2698620434U)
				{
					if (title == "PEAK")
					{
						return RichPresenceState.Status_Peak;
					}
				}
			}
			else if (title == "CALDERA")
			{
				return RichPresenceState.Status_Caldera;
			}
		}
		else if (num != 3092950474U)
		{
			if (num == 3587017822U)
			{
				if (title == "ALPINE")
				{
					return RichPresenceState.Status_Alpine;
				}
			}
		}
		else if (title == "THE KILN")
		{
			return RichPresenceState.Status_Kiln;
		}
		Debug.LogError("Failed to find Rich Presence State from " + p.title);
		return RichPresenceState.Status_Shore;
	}

	// Token: 0x04001154 RID: 4436
	public MountainProgressHandler.ProgressPoint[] progressPoints;

	// Token: 0x04001155 RID: 4437
	public MountainProgressHandler.ProgressPoint tombProgressPoint;

	// Token: 0x04001157 RID: 4439
	public int debugProgress;

	// Token: 0x020004E8 RID: 1256
	[Serializable]
	public class ProgressPoint
	{
		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06001CF6 RID: 7414 RVA: 0x00087002 File Offset: 0x00085202
		// (set) Token: 0x06001CF7 RID: 7415 RVA: 0x0008700A File Offset: 0x0008520A
		public bool Reached { get; set; }

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06001CF8 RID: 7416 RVA: 0x00087013 File Offset: 0x00085213
		public string localizedTitle
		{
			get
			{
				return LocalizedText.GetText(this.title, true);
			}
		}

		// Token: 0x04001AEA RID: 6890
		public Transform transform;

		// Token: 0x04001AEB RID: 6891
		public string title;

		// Token: 0x04001AEC RID: 6892
		public AudioClip clip;

		// Token: 0x04001AED RID: 6893
		public ACHIEVEMENTTYPE achievement;

		// Token: 0x04001AEE RID: 6894
		public Biome.BiomeType biome;
	}
}
