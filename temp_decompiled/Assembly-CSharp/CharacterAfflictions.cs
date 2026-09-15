using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Peak.Afflictions;
using Peak.Dev;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Core.Serizalization;

// Token: 0x0200000C RID: 12
[ConsoleClassCustomizer("Afflictions")]
public class CharacterAfflictions : MonoBehaviourPunCallbacks, IPrettyPrintable
{
	// Token: 0x14000002 RID: 2
	// (add) Token: 0x060000D6 RID: 214 RVA: 0x00006570 File Offset: 0x00004770
	// (remove) Token: 0x060000D7 RID: 215 RVA: 0x000065A8 File Offset: 0x000047A8
	public event Action<Character> StatusesUpdated;

	// Token: 0x060000D8 RID: 216 RVA: 0x000065DD File Offset: 0x000047DD
	private void GetThorns()
	{
		this.physicalThorns = Enumerable.ToList<ThornOnMe>(base.GetComponentsInChildren<ThornOnMe>(true));
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x000065F4 File Offset: 0x000047F4
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.InitStatusArrays();
		this.InitThorns();
		this.m_inAirport = SceneManager.GetActiveScene().name == "Airport";
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00006636 File Offset: 0x00004836
	public string ToPrettyString()
	{
		return CharacterAfflictions.PrettyPrintStaminaBar(this.currentStatuses, 40, true);
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00006648 File Offset: 0x00004848
	public static string PrettyPrintStaminaBar(float[] statuses, int stamBarCharLength = 40, bool applyColor = true)
	{
		CharacterAfflictions.<>c__DisplayClass59_0 CS$<>8__locals1;
		CS$<>8__locals1.stamBarCharLength = stamBarCharLength;
		CS$<>8__locals1.applyColor = applyColor;
		CS$<>8__locals1.applyColor = false;
		int numStatusTypes = CharacterAfflictions.NumStatusTypes;
		if (statuses.Length != numStatusTypes)
		{
			Debug.LogError(string.Format("Can't pretty print statuses because there are {0} types in game but ", numStatusTypes) + string.Format("statuses array was only {0}", statuses.Length));
			return string.Empty;
		}
		StaminaBar bar = GUIManager.instance.bar;
		if (bar.afflictions.Length != numStatusTypes)
		{
			Debug.LogWarning("Oops! Shouldn't there be the same number of statuses as there are affliction bars?");
		}
		Color[] array = new Color[numStatusTypes];
		foreach (BarAffliction barAffliction in bar.afflictions)
		{
			array[(int)barAffliction.afflictionType] = barAffliction.icon.color;
		}
		CS$<>8__locals1.totalNumChars = (float)CS$<>8__locals1.stamBarCharLength;
		float num = Enumerable.Sum(statuses);
		float num2 = Mathf.Clamp01(1f - num);
		CS$<>8__locals1.sb = new StringBuilder();
		CS$<>8__locals1.numCharsAdded = 0;
		CS$<>8__locals1.sb.Append("[");
		Color color;
		ColorUtility.TryParseHtmlString("#59C800", ref color);
		CharacterAfflictions.<PrettyPrintStaminaBar>g__AddCharacters|59_1('=', num2, color, ref CS$<>8__locals1);
		for (int j = 0; j < numStatusTypes; j++)
		{
			CharacterAfflictions.<PrettyPrintStaminaBar>g__AddCharacters|59_1("/".get_Chars(j % "/".Length), statuses[j], array[j], ref CS$<>8__locals1);
		}
		CS$<>8__locals1.sb.Append("]");
		return CS$<>8__locals1.sb.ToString();
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000067C3 File Offset: 0x000049C3
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		this.PushStatuses(newPlayer);
		this.PushThorns(newPlayer);
	}

	// Token: 0x060000DD RID: 221 RVA: 0x000067DC File Offset: 0x000049DC
	private void InitStatusArrays()
	{
		this.currentStatuses = new float[CharacterAfflictions.NumStatusTypes];
		this.currentIncrementalStatuses = new float[this.currentStatuses.Length];
		this.currentDecrementalStatuses = new float[this.currentStatuses.Length];
		this.lastAddedStatus = new float[this.currentStatuses.Length];
		this.lastAddedIncrementalStatus = new float[this.currentStatuses.Length];
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00006848 File Offset: 0x00004A48
	private void InitThorns()
	{
		this.availableThornIndices.Clear();
		this.thornData = new ThornSyncData
		{
			stuckThornIndices = new List<ushort>()
		};
		ushort num = 0;
		while ((int)num < this.physicalThorns.Count)
		{
			if (!(this.physicalThorns[(int)num] == null))
			{
				this.physicalThorns[(int)num].character = this.character;
				this.availableThornIndices.Add(num);
				this.physicalThorns[(int)num].gameObject.SetActive(false);
			}
			num += 1;
		}
	}

	// Token: 0x060000DF RID: 223 RVA: 0x000068E0 File Offset: 0x00004AE0
	private void Update()
	{
		for (int i = this.afflictionList.Count - 1; i >= 0; i--)
		{
			this.afflictionList[i].UpdateEffectNetworked();
			if (this.character.photonView.IsMine && this.afflictionList[i].Tick())
			{
				this.character.refs.afflictions.RemoveAffliction(this.afflictionList[i], false, true);
			}
		}
		if (this.character.photonView.IsMine)
		{
			this.UpdateNormalStatuses();
			this.UpdateThorns();
			if (this._pushStatuses)
			{
				this.ActuallyPushStatuses();
			}
		}
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x0000698C File Offset: 0x00004B8C
	private void UpdateThorns()
	{
		for (int i = this.physicalThorns.Count - 1; i >= 0; i--)
		{
			if (this.physicalThorns[i] != null && this.physicalThorns[i].ShouldPopOut())
			{
				this.RemoveThorn(this.physicalThorns[i]);
			}
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x000069EC File Offset: 0x00004BEC
	internal void UpdateWeight()
	{
		int num = 0;
		int num2 = 0;
		float currentStatus = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Thorns);
		for (int i = 0; i < this.character.player.itemSlots.Length; i++)
		{
			ItemSlot itemSlot = this.character.player.itemSlots[i];
			if (itemSlot.prefab != null)
			{
				num += itemSlot.prefab.CarryWeight;
			}
		}
		BackpackSlot backpackSlot = this.character.player.backpackSlot;
		BackpackData backpackData;
		if (!backpackSlot.IsEmpty() && backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			for (int j = 0; j < backpackData.itemSlots.Length; j++)
			{
				ItemSlot itemSlot2 = backpackData.itemSlots[j];
				if (!itemSlot2.IsEmpty())
				{
					num += itemSlot2.prefab.CarryWeight;
				}
			}
		}
		ItemSlot itemSlot3 = this.character.player.GetItemSlot(250);
		if (!itemSlot3.IsEmpty())
		{
			num += itemSlot3.prefab.CarryWeight;
		}
		if (this.character.data.carriedPlayer != null)
		{
			num += 8;
		}
		foreach (StickyItemComponent stickyItemComponent in StickyItemComponent.ALL_STUCK_ITEMS)
		{
			if (stickyItemComponent.stuckToCharacter == this.character)
			{
				num += stickyItemComponent.addWeightToStuckPlayer;
				num2 += stickyItemComponent.addThornsToStuckPlayer;
			}
		}
		if (this.character.data.currentStickyItem)
		{
			num2 += this.character.data.currentStickyItem.addThornsToStuckPlayer;
		}
		num2 += this.GetTotalThornStatusIncrements();
		if (this.character.data.isSkeleton)
		{
			num2 = 0;
		}
		float num3 = 0.025f * (float)num2;
		if (num3 > currentStatus)
		{
			this.StatusSFX(CharacterAfflictions.STATUSTYPE.Thorns, num3 - currentStatus);
			if (this.character.IsLocal && this.character == Character.observedCharacter)
			{
				GUIManager.instance.AddStatusFX(CharacterAfflictions.STATUSTYPE.Thorns, num3 - currentStatus);
			}
			this.PlayParticle(CharacterAfflictions.STATUSTYPE.Thorns);
		}
		this.SetStatus(CharacterAfflictions.STATUSTYPE.Weight, 0.025f * (float)num, true);
		this.SetStatus(CharacterAfflictions.STATUSTYPE.Thorns, 0.025f * (float)num2, true);
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00006C34 File Offset: 0x00004E34
	private void UpdateNormalStatuses()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy) > 0f && Time.time - this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Drowsy) > this.drowsyReductionCooldown)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.drowsyReductionPerSecond * Time.deltaTime, false, true);
		}
		if (!this.character.IsLocal)
		{
			return;
		}
		if (Ascents.isNightCold && Singleton<MountainProgressHandler>.Instance && Singleton<MountainProgressHandler>.Instance.maxProgressPointReached < 3 && DayNightManager.instance != null && DayNightManager.instance.isDay < 0.5f)
		{
			this.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, Time.deltaTime * (1f - DayNightManager.instance.isDay) * Ascents.nightColdRate, false, true, true);
		}
		if (this.character.data.fullyConscious)
		{
			this.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, Time.deltaTime * this.hungerPerSecond * Ascents.hungerRateMultiplier, false, true, true);
		}
		if (this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) > 0f && Time.time - this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Poison) > this.poisonReductionCooldown)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Poison, this.poisonReductionPerSecond * Time.deltaTime, false, true);
		}
		this.isWebbed = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Web) > 0f;
		if (this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot) > 0f && Time.time - this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Hot) > this.hotReductionCooldown)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, this.hotReductionPerSecond * Time.deltaTime, false, true);
		}
		if ((this.character.data.fullyConscious || !this.willZombify) && this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Spores) > 0f && Time.time - this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Spores) > this.sporesReductionCooldown)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Spores, this.sporesReductionPerSecond * Time.deltaTime, false, true);
		}
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00006E04 File Offset: 0x00005004
	public void AddAffliction(Affliction affliction, bool fromRPC = false)
	{
		if (this.character.data.isCarried)
		{
			return;
		}
		if (affliction == null)
		{
			Debug.LogError("Trying to add null affliction");
			return;
		}
		if (affliction.worksOnBot)
		{
			if (!base.photonView.IsMine && !fromRPC)
			{
				return;
			}
		}
		else if (this.character.isBot || (!this.character.IsLocal && !fromRPC))
		{
			return;
		}
		Affliction affliction2 = affliction.Copy();
		Affliction affliction3;
		if (this.HasAfflictionType(this.afflictionList, affliction2.GetAfflictionType(), out affliction3))
		{
			affliction3.Stack(affliction2);
		}
		else
		{
			this.afflictionList.Add(affliction2);
			affliction2.character = this.character;
			affliction2.OnApplied();
			Debug.Log(string.Format("Added {0} to {1}", affliction2.GetAfflictionType(), this.character.gameObject.name));
		}
		if (!fromRPC && this.character.IsLocal)
		{
			this.PushAfflictions(null);
		}
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00006EF0 File Offset: 0x000050F0
	public void RemoveAffliction(Affliction affliction, bool fromRPC = false, bool pushAfflictions = true)
	{
		if (!this.character.IsLocal && !fromRPC)
		{
			return;
		}
		this.afflictionList.Remove(affliction);
		affliction.OnRemoved();
		Debug.Log(string.Format("Removed {0} to {1}", affliction.GetAfflictionType(), this.character.gameObject.name));
		if (!fromRPC && this.character.IsLocal && pushAfflictions)
		{
			this.PushAfflictions(null);
		}
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00006F68 File Offset: 0x00005168
	public void RemoveAffliction(Affliction.AfflictionType afflictionType)
	{
		Affliction affliction;
		if (this.HasAfflictionType(afflictionType, out affliction))
		{
			this.RemoveAffliction(affliction, false, true);
		}
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00006F8C File Offset: 0x0000518C
	public float GetCurrentStatus(CharacterAfflictions.STATUSTYPE statusType)
	{
		if (ArrayExtensions.WithinRange<float>(this.currentStatuses, (int)statusType))
		{
			return this.currentStatuses[(int)statusType];
		}
		return 0f;
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00006FB8 File Offset: 0x000051B8
	public float GetIncrementalStatus(CharacterAfflictions.STATUSTYPE statusType)
	{
		return this.currentIncrementalStatuses[(int)statusType];
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00006FD0 File Offset: 0x000051D0
	public float LastAddedStatus(CharacterAfflictions.STATUSTYPE statusType)
	{
		return this.lastAddedStatus[(int)statusType];
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00006FE8 File Offset: 0x000051E8
	public float LastAddedIncrementalStatus(CharacterAfflictions.STATUSTYPE statusType)
	{
		return this.lastAddedIncrementalStatus[(int)statusType];
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x060000EA RID: 234 RVA: 0x00006FFF File Offset: 0x000051FF
	public bool shouldPassOut
	{
		get
		{
			return this.statusSum > 0.99f;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x060000EB RID: 235 RVA: 0x00007010 File Offset: 0x00005210
	public float statusSum
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < this.currentStatuses.Length; i++)
			{
				num += this.currentStatuses[i];
			}
			return num;
		}
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00007044 File Offset: 0x00005244
	public float RoundStatus(float startingValue)
	{
		int num = Mathf.RoundToInt(40f);
		return (float)Mathf.RoundToInt(startingValue * (float)num) / (float)num;
	}

	// Token: 0x060000ED RID: 237 RVA: 0x0000706C File Offset: 0x0000526C
	public void SetStatus(CharacterAfflictions.STATUSTYPE statusType, float amount, bool pushStatus = true)
	{
		if (this.character.isZombie && !this.StatusAffectsZombie(statusType))
		{
			return;
		}
		if (!this.character.photonView.IsMine)
		{
			return;
		}
		this.currentStatuses[(int)statusType] = amount;
		this.currentStatuses[(int)statusType] = Mathf.Clamp(this.currentStatuses[(int)statusType], 0f, this.GetStatusCap(statusType));
		this.currentStatuses[(int)statusType] = this.RoundStatus(this.currentStatuses[(int)statusType]);
		this.currentIncrementalStatuses[(int)statusType] = 0f;
		this.currentDecrementalStatuses[(int)statusType] = 0f;
		this.character.ClampStamina();
		GUIManager.instance.bar.ChangeBar();
		if (statusType == CharacterAfflictions.STATUSTYPE.Web && this.currentStatuses[(int)statusType] == 0f)
		{
			this.ReleaseFromWebs();
		}
		if (pushStatus)
		{
			this.PushStatuses(null);
		}
	}

	// Token: 0x060000EE RID: 238 RVA: 0x0000713D File Offset: 0x0000533D
	public void AdjustStatus(CharacterAfflictions.STATUSTYPE statusType, float amount, bool fromRPC = false)
	{
		if (amount > 0f)
		{
			this.AddStatus(statusType, amount, fromRPC, true, true);
			return;
		}
		if (amount < 0f)
		{
			this.SubtractStatus(statusType, Mathf.Abs(amount), fromRPC, false);
		}
	}

	// Token: 0x060000EF RID: 239 RVA: 0x0000716C File Offset: 0x0000536C
	public void AddSunHeat(float amount)
	{
		Parasol parasol;
		if (this.character.data.currentItem && this.character.data.currentItem.TryGetComponent<Parasol>(ref parasol) && parasol.isOpen)
		{
			return;
		}
		if (this.character.data.wearingSunscreen)
		{
			return;
		}
		this.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, amount, false, true, true);
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x000071D1 File Offset: 0x000053D1
	private bool StatusAffectsSkeleton(CharacterAfflictions.STATUSTYPE type)
	{
		return type == CharacterAfflictions.STATUSTYPE.Injury || type == CharacterAfflictions.STATUSTYPE.Curse;
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x000071DD File Offset: 0x000053DD
	private bool StatusAffectsZombie(CharacterAfflictions.STATUSTYPE type)
	{
		return type == CharacterAfflictions.STATUSTYPE.Drowsy || type == CharacterAfflictions.STATUSTYPE.Web;
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x000071EC File Offset: 0x000053EC
	public bool AddStatus(CharacterAfflictions.STATUSTYPE statusType, float amount, bool fromRPC = false, bool playEffects = true, bool notify = true)
	{
		if (this.character.isZombie && !this.StatusAffectsZombie(statusType))
		{
			return false;
		}
		if (this.character.statusesLocked)
		{
			return false;
		}
		if (this.character.data.isSkeleton)
		{
			if (!this.StatusAffectsSkeleton(statusType))
			{
				return false;
			}
			if (statusType == CharacterAfflictions.STATUSTYPE.Injury)
			{
				amount *= 8f;
			}
		}
		if (this.character.isScoutmaster)
		{
			return false;
		}
		if (amount == 0f)
		{
			return false;
		}
		if (this.m_inAirport)
		{
			return false;
		}
		if (this.character.data.isInvincible && statusType != CharacterAfflictions.STATUSTYPE.Curse)
		{
			if (base.photonView.IsMine && this.character.data.isInvincibleMilk)
			{
				Singleton<AchievementManager>.Instance.AddStatusBlockedByMilk(amount);
			}
			return false;
		}
		float num = 2f - this.statusSum;
		if (!base.photonView.IsMine && !fromRPC)
		{
			return false;
		}
		float currentStatus = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot);
		float currentStatus2 = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold);
		if (statusType == CharacterAfflictions.STATUSTYPE.Cold && currentStatus > 0f)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, amount, fromRPC, false);
			amount -= currentStatus;
			if (amount <= 0f)
			{
				return false;
			}
		}
		else if (statusType == CharacterAfflictions.STATUSTYPE.Hot && currentStatus2 > 0f)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, amount, fromRPC, false);
			amount -= currentStatus2;
			if (amount <= 0f)
			{
				return false;
			}
		}
		this.currentIncrementalStatuses[(int)statusType] += amount;
		this.lastAddedIncrementalStatus[(int)statusType] = Time.time;
		Action<CharacterAfflictions.STATUSTYPE, float> onAddedIncrementalStatus = this.OnAddedIncrementalStatus;
		if (onAddedIncrementalStatus != null)
		{
			onAddedIncrementalStatus.Invoke(statusType, amount);
		}
		if (statusType == CharacterAfflictions.STATUSTYPE.Web && this.currentStatuses[(int)statusType] == 0f)
		{
			if (this.character.IsLocal && this.character.data.fullyConscious)
			{
				GUIManager.instance.strugglePrompt.gameObject.SetActive(true);
				GUIManager.instance.strugglePrompt.alpha = 0f;
				DOTweenModuleUI.DOFade(GUIManager.instance.strugglePrompt, 1f, 1f);
			}
			foreach (MeshRenderer meshRenderer in this.webWraps)
			{
				float num2 = Random.value + 1f;
				meshRenderer.gameObject.SetActive(true);
				meshRenderer.material.SetFloat("_Clip", 1f);
				TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(ShortcutExtensions.DOFloat(meshRenderer.material, 0.25f, "_Clip", num2), 3);
				ShortcutExtensions.DOLocalRotate(meshRenderer.transform, new Vector3(0f, 720f, 0f), num2, 0);
				meshRenderer.transform.parent.localScale = Vector3.one * 2f;
				TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(meshRenderer.transform.parent, 1f, num2), 3);
			}
		}
		if (this.currentIncrementalStatuses[(int)statusType] >= 0.025f)
		{
			float num3 = (float)Mathf.FloorToInt(this.currentIncrementalStatuses[(int)statusType] / 0.025f) * 0.025f;
			num3 = Mathf.Min(num3, num);
			this.currentStatuses[(int)statusType] += num3;
			this.currentStatuses[(int)statusType] = Mathf.Clamp(this.currentStatuses[(int)statusType], 0f, this.GetStatusCap(statusType));
			if (notify)
			{
				Action<CharacterAfflictions.STATUSTYPE, float> onAddedStatus = this.OnAddedStatus;
				if (onAddedStatus != null)
				{
					onAddedStatus.Invoke(statusType, num3);
				}
			}
			this.currentIncrementalStatuses[(int)statusType] = 0f;
			this.currentStatuses[(int)statusType] = this.RoundStatus(this.currentStatuses[(int)statusType]);
			this.character.ClampStamina();
			GUIManager.instance.bar.ChangeBar();
			if (playEffects)
			{
				this.StatusSFX(statusType, amount);
				if (this.character.IsLocal && this.character == Character.observedCharacter)
				{
					GUIManager.instance.AddStatusFX(statusType, amount);
				}
				this.PlayParticle(statusType);
			}
			this.lastAddedStatus[(int)statusType] = Time.time;
			if (!fromRPC)
			{
				this.PushStatuses(null);
			}
			if (this.character.IsLocal)
			{
				GlobalEvents.TriggerLocalCharacterStatusIncremented(this.character, statusType, true);
			}
		}
		if (statusType == CharacterAfflictions.STATUSTYPE.Hot && this.character.IsLocal && Singleton<MapHandler>.Instance && Singleton<MapHandler>.Instance.GetCurrentBiome() == Biome.BiomeType.Mesa)
		{
			float currentStatus3 = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot);
			if (currentStatus3 > Singleton<AchievementManager>.Instance.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxHeatTakenInMesa))
			{
				Singleton<AchievementManager>.Instance.SetRunBasedFloat(RUNBASEDVALUETYPE.MaxHeatTakenInMesa, currentStatus3);
			}
		}
		if (statusType == CharacterAfflictions.STATUSTYPE.Cold && this.character.IsLocal && Singleton<MapHandler>.Instance && Singleton<MapHandler>.Instance.GetCurrentBiome() == Biome.BiomeType.Alpine)
		{
			float currentStatus4 = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold);
			if (currentStatus4 > Singleton<AchievementManager>.Instance.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxColdTakenInAlpine))
			{
				Singleton<AchievementManager>.Instance.SetRunBasedFloat(RUNBASEDVALUETYPE.MaxColdTakenInAlpine, currentStatus4);
			}
		}
		if (statusType == CharacterAfflictions.STATUSTYPE.Spores && this.character.IsLocal && Singleton<MapHandler>.Instance && Singleton<MapHandler>.Instance.GetCurrentBiome() == Biome.BiomeType.Roots)
		{
			float currentStatus5 = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Spores);
			if (currentStatus5 > Singleton<AchievementManager>.Instance.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxSporesTakenInRoots))
			{
				Singleton<AchievementManager>.Instance.SetRunBasedFloat(RUNBASEDVALUETYPE.MaxSporesTakenInRoots, currentStatus5);
			}
		}
		return true;
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x000076EC File Offset: 0x000058EC
	public void SubtractStatus(CharacterAfflictions.STATUSTYPE statusType, float amount, bool fromRPC = false, bool decreasedNaturally = false)
	{
		if (this.character.isZombie && !this.StatusAffectsZombie(statusType))
		{
			return;
		}
		if (this.character.statusesLocked)
		{
			return;
		}
		if (!this.character.photonView.IsMine && !fromRPC)
		{
			return;
		}
		if (statusType == CharacterAfflictions.STATUSTYPE.Poison && !decreasedNaturally && this.character.IsLocal)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Spores, amount, false, false);
		}
		if ((statusType == CharacterAfflictions.STATUSTYPE.Poison || statusType == CharacterAfflictions.STATUSTYPE.Spores) && amount <= 0.1f)
		{
			this.ClearPoisonAfflictions();
		}
		if (this.currentStatuses[(int)statusType] == 0f)
		{
			this.currentDecrementalStatuses[(int)statusType] = 0f;
			return;
		}
		this.currentDecrementalStatuses[(int)statusType] += amount;
		if (this.currentDecrementalStatuses[(int)statusType] >= 0.025f)
		{
			float num = (float)Mathf.FloorToInt(this.currentDecrementalStatuses[(int)statusType] / 0.025f) * 0.025f;
			Debug.Log(string.Format("Removing status chunk: {0}", statusType));
			this.currentStatuses[(int)statusType] -= num;
			this.currentStatuses[(int)statusType] = Mathf.Clamp(this.currentStatuses[(int)statusType], 0f, this.GetStatusCap(statusType));
			if (statusType == CharacterAfflictions.STATUSTYPE.Hunger)
			{
				this.currentIncrementalStatuses[(int)statusType] = 0f;
			}
			this.currentDecrementalStatuses[(int)statusType] = 0f;
			this.currentStatuses[(int)statusType] = this.RoundStatus(this.currentStatuses[(int)statusType]);
			this.character.ClampStamina();
			GUIManager.instance.bar.ChangeBar();
			if (!fromRPC)
			{
				this.PushStatuses(null);
			}
			if (this.character.IsLocal)
			{
				GlobalEvents.TriggerLocalCharacterStatusIncremented(this.character, statusType, false);
			}
		}
		if (statusType == CharacterAfflictions.STATUSTYPE.Web && this.currentStatuses[(int)statusType] == 0f)
		{
			this.ReleaseFromWebs();
		}
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x0000789C File Offset: 0x00005A9C
	private void ReleaseFromWebs()
	{
		if (this.character.IsLocal)
		{
			GUIManager.instance.strugglePrompt.gameObject.SetActive(false);
		}
		using (List<MeshRenderer>.Enumerator enumerator = this.webWraps.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MeshRenderer wrap = enumerator.Current;
				float num = 0.25f;
				ShortcutExtensions.DOFloat(wrap.material, 1f, "_Clip", num);
				ShortcutExtensions.DOLocalRotate(wrap.transform, new Vector3(0f, 0f, 0f), num, 0);
				wrap.transform.parent.localScale = Vector3.one;
				TweenSettingsExtensions.OnComplete<TweenerCore<Vector3, Vector3, VectorOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(wrap.transform.parent, 4f, num), 3), delegate
				{
					wrap.gameObject.SetActive(false);
				});
			}
		}
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x000079B4 File Offset: 0x00005BB4
	private void StatusSFX(CharacterAfflictions.STATUSTYPE sT, float ammount)
	{
		if (sT == CharacterAfflictions.STATUSTYPE.Injury)
		{
			if (ammount > 0f && this.injurySmall)
			{
				this.injurySmall.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
			}
			if (ammount > 0.4f && this.injuryMid)
			{
				this.injuryMid.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
			}
			if (ammount > 0.75f && this.injuryHeavy)
			{
				this.injuryHeavy.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Hunger)
		{
			if (this.injuryHunger)
			{
				this.injuryHunger.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Cold)
		{
			if (this.injuryIce)
			{
				this.injuryIce.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Hot)
		{
			if (this.injuryFire)
			{
				this.injuryFire.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Poison)
		{
			if (this.injuryPoison)
			{
				this.injuryPoison.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Thorns)
		{
			if (this.injuryThorns)
			{
				this.injuryThorns.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Spores && this.injurySpore)
		{
			this.injurySpore.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
		}
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00007BAF File Offset: 0x00005DAF
	public void PlayDebugParticle()
	{
		this.PlayParticle(this.debugStatusType);
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00007BC0 File Offset: 0x00005DC0
	public void PlayParticle(CharacterAfflictions.STATUSTYPE statusType)
	{
		switch (statusType)
		{
		case CharacterAfflictions.STATUSTYPE.Injury:
			this.character.refs.customization.PulseStatus(this.colorInjury, 1f);
			return;
		case CharacterAfflictions.STATUSTYPE.Hunger:
		case CharacterAfflictions.STATUSTYPE.Weight:
			break;
		case CharacterAfflictions.STATUSTYPE.Cold:
			this.character.refs.customization.PulseStatus(this.colorCold, 1f);
			return;
		case CharacterAfflictions.STATUSTYPE.Poison:
			this.character.refs.customization.PulseStatus(this.colorPoison, 1f);
			return;
		case CharacterAfflictions.STATUSTYPE.Crab:
			this.character.refs.customization.PulseStatus(this.colorCrab, 1f);
			return;
		case CharacterAfflictions.STATUSTYPE.Curse:
			this.character.refs.customization.PulseStatus(this.colorCurse, 1f);
			return;
		case CharacterAfflictions.STATUSTYPE.Drowsy:
			this.character.refs.customization.PulseStatus(this.colorDrowsy, 1f);
			return;
		case CharacterAfflictions.STATUSTYPE.Hot:
			this.character.refs.customization.PulseStatus(this.colorHot, 1f);
			break;
		case CharacterAfflictions.STATUSTYPE.Thorns:
			this.character.refs.customization.PulseStatus(this.colorThorns, 1f);
			return;
		case CharacterAfflictions.STATUSTYPE.Spores:
			this.character.refs.customization.PulseStatus(this.colorSpores, 1f);
			return;
		default:
			return;
		}
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x00007D28 File Offset: 0x00005F28
	public void PushStatuses(Player specificPlayer = null)
	{
		this._pushStatuses = true;
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x00007D34 File Offset: 0x00005F34
	private void ActuallyPushStatuses()
	{
		this._pushStatuses = false;
		if (!this.character.photonView.IsMine)
		{
			return;
		}
		byte[] array = IBinarySerializable.ToManagedArray<StatusSyncData>(new StatusSyncData
		{
			statusList = new List<float>(this.currentStatuses)
		});
		this.character.photonView.RPC("SyncStatusesRPC", 1, new object[] { array });
		Action<Character> statusesUpdated = this.StatusesUpdated;
		if (statusesUpdated == null)
		{
			return;
		}
		statusesUpdated.Invoke(this.character);
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00007DB2 File Offset: 0x00005FB2
	private float[] DeserializeStatusArray(in byte[] data)
	{
		return IBinarySerializable.GetFromManagedArray<StatusSyncData>(data).statusList.ToArray();
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00007DC8 File Offset: 0x00005FC8
	[PunRPC]
	private void SyncStatusesRPC(byte[] data)
	{
		if (this.character.photonView.IsMine)
		{
			return;
		}
		float[] array = this.DeserializeStatusArray(in data);
		this.SetAll(array);
		Action<Character> statusesUpdated = this.StatusesUpdated;
		if (statusesUpdated == null)
		{
			return;
		}
		statusesUpdated.Invoke(this.character);
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00007E10 File Offset: 0x00006010
	public void PushThorns(Player specificPlayer = null)
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		byte[] array = IBinarySerializable.ToManagedArray<ThornSyncData>(this.thornData);
		if (specificPlayer == null)
		{
			this.character.photonView.RPC("SyncThornsRPC_Remote", 1, new object[] { array });
			return;
		}
		this.character.photonView.RPC("SyncThornsRPC_Remote", specificPlayer, new object[] { array });
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00007E7C File Offset: 0x0000607C
	[PunRPC]
	private void SyncThornsRPC_Remote(byte[] data)
	{
		if (base.photonView.IsMine)
		{
			return;
		}
		List<ushort> stuckThornIndices = IBinarySerializable.GetFromManagedArray<ThornSyncData>(data).stuckThornIndices;
		ushort num = 0;
		while ((int)num < this.physicalThorns.Count)
		{
			if (stuckThornIndices.Contains(num))
			{
				this.physicalThorns[(int)num].EnableThorn();
			}
			else
			{
				this.physicalThorns[(int)num].DisableThorn();
			}
			num += 1;
		}
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00007EE7 File Offset: 0x000060E7
	public void ApplyReconnectData(ReconnectData data)
	{
		Debug.Log(this.character.characterName + " (self) applying data: " + Pretty.Print(data));
		this.SetAll(data.currentStatuses);
		this.PushStatuses(null);
	}

	// Token: 0x060000FF RID: 255 RVA: 0x00007F24 File Offset: 0x00006124
	private void SetAll(float[] statuses)
	{
		if (statuses.Length != this.currentStatuses.Length)
		{
			string text = "Deserialized data length for " + this.character.gameObject.name + " does not match current status length!!!\ndeserialized data:";
			for (int i = 0; i < statuses.Length; i++)
			{
				text = text + statuses[i].ToString() + ", ";
			}
			text += "\nlocal data:";
			for (int j = 0; j < this.currentStatuses.Length; j++)
			{
				text = text + this.currentStatuses[j].ToString() + ", ";
			}
			Debug.LogError(text);
			return;
		}
		for (int k = 0; k < statuses.Length; k++)
		{
			float num = statuses[k] - this.currentStatuses[k];
			if (num > 0f)
			{
				this.AddStatus((CharacterAfflictions.STATUSTYPE)k, num, true, true, true);
			}
			if (num < 0f)
			{
				this.SubtractStatus((CharacterAfflictions.STATUSTYPE)k, -num, true, false);
			}
		}
	}

	// Token: 0x06000100 RID: 256 RVA: 0x00008011 File Offset: 0x00006211
	[PunRPC]
	public void RPC_ApplyStatusesFromFloatArray(float[] data, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		this.ApplyStatusesFromFloatArray(data);
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00008028 File Offset: 0x00006228
	public void ApplyStatusesFromFloatArray(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (i != 7 && i != 9)
			{
				float num = data[i];
				if (num > 0f)
				{
					this.AddStatus((CharacterAfflictions.STATUSTYPE)i, num, true, true, true);
				}
				if (num < 0f)
				{
					this.SubtractStatus((CharacterAfflictions.STATUSTYPE)i, -num, true, false);
				}
			}
		}
	}

	// Token: 0x06000102 RID: 258 RVA: 0x00008078 File Offset: 0x00006278
	public void PushAfflictions(Player specificPlayer = null)
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		byte[] array = IBinarySerializable.ToManagedArray<AfflictionSyncData>(new AfflictionSyncData
		{
			afflictions = new List<Affliction>(this.afflictionList)
		});
		if (specificPlayer == null)
		{
			this.character.photonView.RPC("SyncAfflictionsRPC", 1, new object[] { array });
			return;
		}
		this.character.photonView.RPC("SyncAfflictionsRPC", specificPlayer, new object[] { array });
	}

	// Token: 0x06000103 RID: 259 RVA: 0x000080F8 File Offset: 0x000062F8
	[PunRPC]
	private void SyncAfflictionsRPC(byte[] data)
	{
		if (this.character.IsLocal)
		{
			return;
		}
		Affliction[] array = IBinarySerializable.GetFromManagedArray<AfflictionSyncData>(data).afflictions.ToArray();
		for (int i = this.afflictionList.Count - 1; i >= 0; i--)
		{
			Affliction affliction = this.afflictionList[i];
			Affliction affliction2;
			if (!this.HasAfflictionType(array, affliction.GetAfflictionType(), out affliction2))
			{
				Debug.Log(string.Format("{0} removed old affliction: {1}", base.gameObject.name, affliction.GetAfflictionType()));
				this.RemoveAffliction(affliction, true, true);
			}
		}
		foreach (Affliction affliction3 in array)
		{
			Affliction affliction4;
			if (this.HasAfflictionType(this.afflictionList, affliction3.GetAfflictionType(), out affliction4))
			{
				Debug.Log(string.Format("{0} stacked affliction: {1}", base.gameObject.name, affliction3.GetAfflictionType()));
				affliction4.Stack(affliction3);
			}
			else
			{
				Debug.Log(string.Format("{0} added new affliction: {1}", base.gameObject.name, affliction3.GetAfflictionType()));
				this.AddAffliction(affliction3, true);
			}
		}
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00008220 File Offset: 0x00006420
	public bool HasAfflictionType(Affliction.AfflictionType type, out Affliction affliction)
	{
		foreach (Affliction affliction2 in this.afflictionList)
		{
			if (affliction2.GetAfflictionType() == type)
			{
				affliction = affliction2;
				return true;
			}
		}
		affliction = null;
		return false;
	}

	// Token: 0x06000105 RID: 261 RVA: 0x00008284 File Offset: 0x00006484
	private bool HasAfflictionType(IEnumerable<Affliction> afflictionList, Affliction.AfflictionType type, out Affliction affliction)
	{
		foreach (Affliction affliction2 in afflictionList)
		{
			if (affliction2.GetAfflictionType() == type)
			{
				affliction = affliction2;
				return true;
			}
		}
		affliction = null;
		return false;
	}

	// Token: 0x06000106 RID: 262 RVA: 0x000082DC File Offset: 0x000064DC
	public float GetStatusCap(CharacterAfflictions.STATUSTYPE type)
	{
		if (this.statusCaps.ContainsKey(type))
		{
			return this.statusCaps[type];
		}
		return 2f;
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00008300 File Offset: 0x00006500
	[ConsoleCommand]
	public static void TestExactStatus()
	{
		float num = 1f - Character.localCharacter.refs.afflictions.statusSum;
		Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num, false, true, true);
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00008342 File Offset: 0x00006542
	[ConsoleCommand]
	public static void Starve()
	{
		Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false, true, true);
	}

	// Token: 0x06000109 RID: 265 RVA: 0x00008362 File Offset: 0x00006562
	[ContextMenu("Test Poison over Time")]
	public void AddPoisonOverTime()
	{
		this.AddAffliction(new Affliction_PoisonOverTime(10f, 0f, 0.05f), false);
	}

	// Token: 0x0600010A RID: 266 RVA: 0x0000837F File Offset: 0x0000657F
	[ConsoleCommand]
	public static void ClearAllAilments()
	{
		Character.localCharacter.refs.afflictions.ClearAllStatus(false);
	}

	// Token: 0x0600010B RID: 267 RVA: 0x00008396 File Offset: 0x00006596
	[ConsoleCommand]
	public static void Hungry()
	{
		Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.75f, false, true, true);
	}

	// Token: 0x0600010C RID: 268 RVA: 0x000083B8 File Offset: 0x000065B8
	public void ClearAllStatus(bool excludeCurse = true)
	{
		int num = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length;
		for (int i = 0; i < num; i++)
		{
			CharacterAfflictions.STATUSTYPE statustype = (CharacterAfflictions.STATUSTYPE)i;
			Debug.Log("Clearing status: " + statustype.ToString());
			if (statustype != CharacterAfflictions.STATUSTYPE.Weight && statustype != CharacterAfflictions.STATUSTYPE.Crab && statustype != CharacterAfflictions.STATUSTYPE.Thorns && (!excludeCurse || statustype != CharacterAfflictions.STATUSTYPE.Curse))
			{
				Debug.Log(string.Format("Current: {0}, amount {1}", statustype, this.character.refs.afflictions.GetCurrentStatus(statustype)));
				Debug.Log(string.Format("SetStatus status: {0}", statustype));
				this.character.refs.afflictions.SetStatus(statustype, 0f, false);
			}
		}
		this.PushStatuses(null);
	}

	// Token: 0x0600010D RID: 269 RVA: 0x00008483 File Offset: 0x00006683
	[ConsoleCommand]
	public static void ClearHunger()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0f, true);
	}

	// Token: 0x0600010E RID: 270 RVA: 0x000084A0 File Offset: 0x000066A0
	[ConsoleCommand]
	public static void ClearDrowsy()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 0f, true);
	}

	// Token: 0x0600010F RID: 271 RVA: 0x000084BD File Offset: 0x000066BD
	[ConsoleCommand]
	public static void ClearInjury()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Injury, 0f, true);
	}

	// Token: 0x06000110 RID: 272 RVA: 0x000084DA File Offset: 0x000066DA
	[ConsoleCommand]
	public static void ClearCurse()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Curse, 0f, true);
	}

	// Token: 0x06000111 RID: 273 RVA: 0x000084F7 File Offset: 0x000066F7
	[ConsoleCommand]
	public static void ClearCold()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Cold, 0f, true);
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00008514 File Offset: 0x00006714
	[ConsoleCommand]
	public static void ClearPoison()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Poison, 0f, true);
	}

	// Token: 0x06000113 RID: 275 RVA: 0x00008531 File Offset: 0x00006731
	[ConsoleCommand]
	public static void ClearHot()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Hot, 0f, true);
	}

	// Token: 0x06000114 RID: 276 RVA: 0x0000854E File Offset: 0x0000674E
	[ConsoleCommand]
	public static void ClearAll()
	{
		Character.localCharacter.refs.afflictions.ClearAllStatus(false);
	}

	// Token: 0x06000115 RID: 277 RVA: 0x00008568 File Offset: 0x00006768
	public void ClearAllAfflictions()
	{
		for (int i = this.afflictionList.Count - 1; i >= 0; i--)
		{
			this.RemoveAffliction(this.afflictionList[i], false, false);
		}
		this.PushAfflictions(null);
	}

	// Token: 0x06000116 RID: 278 RVA: 0x000085A8 File Offset: 0x000067A8
	public void ClearPoisonAfflictions()
	{
		List<Affliction> list = new List<Affliction>();
		foreach (Affliction affliction in this.afflictionList)
		{
			if (affliction is Affliction_PoisonOverTime)
			{
				list.Add(affliction);
			}
			if (affliction is Affliction_ZombieBite)
			{
				list.Add(affliction);
			}
		}
		foreach (Affliction affliction2 in list)
		{
			Debug.Log(string.Format("curing affliction: {0}", affliction2.GetType()));
			this.RemoveAffliction(affliction2, false, false);
		}
		this.PushAfflictions(null);
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00008674 File Offset: 0x00006874
	[ContextMenu("Test Full Drowsy")]
	[ConsoleCommand]
	public static void AddDrowsy()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 0.2f, false, true, true);
	}

	// Token: 0x06000118 RID: 280 RVA: 0x00008699 File Offset: 0x00006899
	[ContextMenu("Test Curse")]
	[ConsoleCommand]
	public static void AddCurse()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.2f, false, true, true);
	}

	// Token: 0x06000119 RID: 281 RVA: 0x000086BE File Offset: 0x000068BE
	[ContextMenu("Test Death")]
	[ConsoleCommand]
	public static void Die()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f, false, true, true);
	}

	// Token: 0x0600011A RID: 282 RVA: 0x000086E3 File Offset: 0x000068E3
	[ContextMenu("Add Poison")]
	[ConsoleCommand]
	public static void AddPoison()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.2f, false, true, true);
	}

	// Token: 0x0600011B RID: 283 RVA: 0x00008708 File Offset: 0x00006908
	[ContextMenu("Test Cold")]
	[ConsoleCommand]
	public static void AddCold()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, 0.2f, false, true, true);
	}

	// Token: 0x0600011C RID: 284 RVA: 0x0000872D File Offset: 0x0000692D
	[ContextMenu("Test Hot")]
	[ConsoleCommand]
	public static void AddHot()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 0.2f, false, true, true);
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00008752 File Offset: 0x00006952
	[ContextMenu("Test Injury")]
	[ConsoleCommand]
	public static void AddInjury()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.2f, false, true, true);
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00008777 File Offset: 0x00006977
	[ContextMenu("Test Spores")]
	[ConsoleCommand]
	public static void AddSpores()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Spores, 0.2f, false, true, true);
	}

	// Token: 0x0600011F RID: 287 RVA: 0x0000879D File Offset: 0x0000699D
	[ContextMenu("Test Hunger")]
	[ConsoleCommand]
	public static void AddHunger()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.2f, false, true, true);
	}

	// Token: 0x06000120 RID: 288 RVA: 0x000087C2 File Offset: 0x000069C2
	[ConsoleCommand]
	public static void AddTinyHunger()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.025f, false, true, true);
	}

	// Token: 0x06000121 RID: 289 RVA: 0x000087E7 File Offset: 0x000069E7
	[ContextMenu("Test Crab")]
	public static void TestCrab()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Crab, 0.2f, false, true, true);
	}

	// Token: 0x06000122 RID: 290 RVA: 0x0000880C File Offset: 0x00006A0C
	[ConsoleCommand]
	public static void GetThorned()
	{
		Character.localCharacter.refs.afflictions.AddThorn(999);
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00008828 File Offset: 0x00006A28
	[ConsoleCommand]
	public static void GetUnThorned()
	{
		foreach (ThornOnMe thornOnMe in Character.localCharacter.refs.afflictions.physicalThorns)
		{
			if (thornOnMe.gameObject.activeInHierarchy)
			{
				Character.localCharacter.refs.afflictions.RemoveThorn(thornOnMe);
				break;
			}
		}
	}

	// Token: 0x06000124 RID: 292 RVA: 0x000088A8 File Offset: 0x00006AA8
	public void AddThorn(Vector3 position)
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		float num = float.MaxValue;
		ThornOnMe thornOnMe = null;
		foreach (ushort num2 in this.availableThornIndices)
		{
			float num3 = Vector3.Distance(this.physicalThorns[(int)num2].transform.position, position);
			if (num3 < num)
			{
				num = num3;
				thornOnMe = this.physicalThorns[(int)num2];
			}
		}
		if (thornOnMe != null)
		{
			this.AddThorn((ushort)this.physicalThorns.IndexOf(thornOnMe));
		}
	}

	// Token: 0x06000125 RID: 293 RVA: 0x0000895C File Offset: 0x00006B5C
	public void AddThorn(ushort thornIndex = 999)
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (this.availableThornIndices.Count == 0)
		{
			return;
		}
		if (this.character.data.isInvincible)
		{
			return;
		}
		if (this.character.data.isSkeleton)
		{
			return;
		}
		if (thornIndex == 999 || !this.availableThornIndices.Contains(thornIndex))
		{
			thornIndex = this.availableThornIndices.RandomSelection((ushort u) => 1);
		}
		base.photonView.RPC("RPC_EnableThorn", 0, new object[] { (int)thornIndex });
		this.availableThornIndices.Remove(thornIndex);
		this.thornData.stuckThornIndices.Add(thornIndex);
		this.UpdateWeight();
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00008A31 File Offset: 0x00006C31
	[PunRPC]
	public void RPC_EnableThorn(int thornIndex)
	{
		this.physicalThorns[thornIndex].EnableThorn();
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00008A44 File Offset: 0x00006C44
	[PunRPC]
	public void RPC_DisableThorn(int thornIndex)
	{
		this.physicalThorns[thornIndex].DisableThorn();
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00008A58 File Offset: 0x00006C58
	public void RemoveAllThorns()
	{
		foreach (ThornOnMe thornOnMe in this.physicalThorns)
		{
			this.RemoveThorn(thornOnMe);
		}
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00008AAC File Offset: 0x00006CAC
	[PunRPC]
	public void RemoveThornRPC(int index)
	{
		if (base.photonView.IsMine)
		{
			ThornOnMe thornOnMe = this.physicalThorns[index];
			base.photonView.RPC("RPC_DisableThorn", 0, new object[] { index });
			this.availableThornIndices.Add((ushort)index);
			this.thornData.stuckThornIndices.Remove((ushort)index);
			this.UpdateWeight();
			return;
		}
		base.photonView.RPC("RemoveThornRPC", base.photonView.Owner, new object[] { index });
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00008B44 File Offset: 0x00006D44
	public void RemoveRandomThornLinq()
	{
		ThornOnMe thornOnMe = Enumerable.Where<ThornOnMe>(this.physicalThorns, (ThornOnMe t) => t.stuckIn).RandomSelection((ThornOnMe t1) => 1);
		this.RemoveThorn(thornOnMe);
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00008BA8 File Offset: 0x00006DA8
	public void RemoveThorn(ThornOnMe thornToRemove)
	{
		ushort num = 0;
		while ((int)num < this.physicalThorns.Count)
		{
			if (this.physicalThorns[(int)num] == thornToRemove)
			{
				this.RemoveThornRPC((int)num);
				return;
			}
			num += 1;
		}
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00008BE8 File Offset: 0x00006DE8
	public int GetTotalThornStatusIncrements()
	{
		int num = 0;
		foreach (ThornOnMe thornOnMe in this.physicalThorns)
		{
			if (thornOnMe.stuckIn)
			{
				num += thornOnMe.thornDamage;
			}
		}
		return num;
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00008C48 File Offset: 0x00006E48
	public void StartWhirlwind()
	{
		base.photonView.RPC("StartWhirlwindRPC", 0, Array.Empty<object>());
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00008C60 File Offset: 0x00006E60
	[PunRPC]
	public void StartWhirlwindRPC()
	{
		this.whirlwind.transform.GetChild(0).gameObject.SetActive(true);
		this.whirlwind.gameObject.SetActive(true);
		this.whirlwind.transform.localScale = Vector3.zero;
		TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(this.whirlwind.transform, 1f, 1f), 27);
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00008CD1 File Offset: 0x00006ED1
	public void WarnStopWhirlwind()
	{
		base.photonView.RPC("WarnStopWhirlwindRPC", 0, Array.Empty<object>());
	}

	// Token: 0x06000130 RID: 304 RVA: 0x00008CEC File Offset: 0x00006EEC
	[PunRPC]
	public void WarnStopWhirlwindRPC()
	{
		CharacterAfflictions.<>c__DisplayClass153_0 CS$<>8__locals1 = new CharacterAfflictions.<>c__DisplayClass153_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.childObject = this.whirlwind.transform.GetChild(0).gameObject;
		base.StartCoroutine(CS$<>8__locals1.<WarnStopWhirlwindRPC>g__WarnStopWhirlwindRoutine|0());
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00008D2F File Offset: 0x00006F2F
	public void StopWhirlwind()
	{
		base.photonView.RPC("StopWhirlwindRPC", 0, Array.Empty<object>());
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00008D48 File Offset: 0x00006F48
	[PunRPC]
	public void StopWhirlwindRPC()
	{
		this.whirlwind.transform.localScale = Vector3.one;
		TweenSettingsExtensions.OnComplete<TweenerCore<Vector3, Vector3, VectorOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(this.whirlwind.transform, new Vector3(3f, 0f, 3f), 0.33f), 26), delegate
		{
			this.whirlwind.gameObject.SetActive(false);
		});
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000133 RID: 307 RVA: 0x00008DAC File Offset: 0x00006FAC
	public bool willZombify
	{
		get
		{
			return this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Spores) >= 0.5f && Ascents.currentAscent != -1;
		}
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00008DCC File Offset: 0x00006FCC
	public CharacterAfflictions()
	{
		Dictionary<CharacterAfflictions.STATUSTYPE, float> dictionary = new Dictionary<CharacterAfflictions.STATUSTYPE, float>();
		dictionary.Add(CharacterAfflictions.STATUSTYPE.Injury, 1f);
		this.statusCaps = dictionary;
		this.hungerPerSecond = 0.0005f;
		this.nightColdPerSecond = 0.002f;
		this.afflictionList = new List<Affliction>();
		this.availableThornIndices = new List<ushort>();
		base..ctor();
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00008E3A File Offset: 0x0000703A
	[CompilerGenerated]
	internal static int <PrettyPrintStaminaBar>g__NumCharacters|59_0(float value, ref CharacterAfflictions.<>c__DisplayClass59_0 A_1)
	{
		return Mathf.RoundToInt(value * A_1.totalNumChars);
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00008E4C File Offset: 0x0000704C
	[CompilerGenerated]
	internal static void <PrettyPrintStaminaBar>g__AddCharacters|59_1(char character, float statusValue, Color color, ref CharacterAfflictions.<>c__DisplayClass59_0 A_3)
	{
		int num = CharacterAfflictions.<PrettyPrintStaminaBar>g__NumCharacters|59_0(statusValue, ref A_3);
		if (num < 1)
		{
			return;
		}
		if (A_3.numCharsAdded <= A_3.stamBarCharLength && num + A_3.numCharsAdded > A_3.stamBarCharLength)
		{
			int num2 = A_3.stamBarCharLength - A_3.numCharsAdded;
			A_3.sb.Append(CharacterAfflictions.<PrettyPrintStaminaBar>g__FormatCharacters|59_2(character, num2, color, ref A_3));
			A_3.sb.Append("|");
			num -= num2;
			A_3.numCharsAdded += num2;
		}
		A_3.sb.Append(CharacterAfflictions.<PrettyPrintStaminaBar>g__FormatCharacters|59_2(character, num, color, ref A_3));
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00008EE4 File Offset: 0x000070E4
	[CompilerGenerated]
	internal static string <PrettyPrintStaminaBar>g__FormatCharacters|59_2(char character, int numCharacters, Color color, ref CharacterAfflictions.<>c__DisplayClass59_0 A_3)
	{
		string text = new string(character, numCharacters);
		if (!A_3.applyColor)
		{
			return text;
		}
		return string.Concat(new string[]
		{
			"<color=#",
			ColorUtility.ToHtmlStringRGBA(color),
			">",
			text,
			"</color>"
		});
	}

	// Token: 0x04000055 RID: 85
	public const float ZOMBIFICATION_SPORES_THRESHOLD = 0.5f;

	// Token: 0x04000056 RID: 86
	private bool _pushStatuses;

	// Token: 0x04000057 RID: 87
	private Dictionary<CharacterAfflictions.STATUSTYPE, float> statusCaps;

	// Token: 0x04000058 RID: 88
	[SerializeField]
	public float[] currentStatuses;

	// Token: 0x04000059 RID: 89
	private float[] currentIncrementalStatuses;

	// Token: 0x0400005A RID: 90
	private float[] currentDecrementalStatuses;

	// Token: 0x0400005B RID: 91
	private float[] lastAddedStatus;

	// Token: 0x0400005C RID: 92
	private float[] lastAddedIncrementalStatus;

	// Token: 0x0400005D RID: 93
	public float poisonReductionPerSecond;

	// Token: 0x0400005E RID: 94
	public float poisonReductionCooldown;

	// Token: 0x0400005F RID: 95
	public float drowsyReductionPerSecond;

	// Token: 0x04000060 RID: 96
	public float drowsyReductionCooldown;

	// Token: 0x04000061 RID: 97
	public float hotReductionPerSecond;

	// Token: 0x04000062 RID: 98
	public float hotReductionCooldown;

	// Token: 0x04000063 RID: 99
	public float sporesReductionPerSecond;

	// Token: 0x04000064 RID: 100
	public float sporesReductionCooldown;

	// Token: 0x04000065 RID: 101
	public float sporesAddPerSecondSlow;

	// Token: 0x04000066 RID: 102
	public float sporesAddPerSecondFast;

	// Token: 0x04000067 RID: 103
	public float hungerPerSecond;

	// Token: 0x04000068 RID: 104
	public float thornsReductionPerSecond;

	// Token: 0x04000069 RID: 105
	public float thornsReductionCooldown;

	// Token: 0x0400006A RID: 106
	public float nightColdPerSecond;

	// Token: 0x0400006B RID: 107
	public Character character;

	// Token: 0x0400006C RID: 108
	[SerializeReference]
	public List<Affliction> afflictionList;

	// Token: 0x0400006D RID: 109
	[FormerlySerializedAs("headVFX")]
	public Transform headVfxTransform;

	// Token: 0x0400006E RID: 110
	[ColorUsage(false, true)]
	public Color colorInjury;

	// Token: 0x0400006F RID: 111
	[ColorUsage(false, true)]
	public Color colorCold;

	// Token: 0x04000070 RID: 112
	[ColorUsage(false, true)]
	public Color colorCrab;

	// Token: 0x04000071 RID: 113
	[ColorUsage(false, true)]
	public Color colorPoison;

	// Token: 0x04000072 RID: 114
	[ColorUsage(false, true)]
	public Color colorCurse;

	// Token: 0x04000073 RID: 115
	[ColorUsage(false, true)]
	public Color colorDrowsy;

	// Token: 0x04000074 RID: 116
	[ColorUsage(false, true)]
	public Color colorHot;

	// Token: 0x04000075 RID: 117
	[ColorUsage(false, true)]
	public Color colorThorns;

	// Token: 0x04000076 RID: 118
	[ColorUsage(false, true)]
	public Color colorSpores;

	// Token: 0x04000077 RID: 119
	public SFX_Instance injurySmall;

	// Token: 0x04000078 RID: 120
	public SFX_Instance injuryMid;

	// Token: 0x04000079 RID: 121
	public SFX_Instance injuryHeavy;

	// Token: 0x0400007A RID: 122
	public SFX_Instance injuryIce;

	// Token: 0x0400007B RID: 123
	public SFX_Instance injuryFire;

	// Token: 0x0400007C RID: 124
	public SFX_Instance injuryPoison;

	// Token: 0x0400007D RID: 125
	public SFX_Instance injuryHunger;

	// Token: 0x0400007E RID: 126
	public SFX_Instance injuryThorns;

	// Token: 0x0400007F RID: 127
	public SFX_Instance injurySpore;

	// Token: 0x04000080 RID: 128
	public Action<CharacterAfflictions.STATUSTYPE, float> OnAddedStatus;

	// Token: 0x04000081 RID: 129
	public Action<CharacterAfflictions.STATUSTYPE, float> OnAddedIncrementalStatus;

	// Token: 0x04000083 RID: 131
	internal bool m_inAirport;

	// Token: 0x04000084 RID: 132
	public List<ThornOnMe> physicalThorns;

	// Token: 0x04000085 RID: 133
	public List<MeshRenderer> webWraps;

	// Token: 0x04000086 RID: 134
	public GameObject zombieScriptsPrefab;

	// Token: 0x04000087 RID: 135
	private List<ushort> availableThornIndices;

	// Token: 0x04000088 RID: 136
	public ThornSyncData thornData;

	// Token: 0x04000089 RID: 137
	public bool isWebbed;

	// Token: 0x0400008A RID: 138
	public static readonly int NumStatusTypes = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length;

	// Token: 0x0400008B RID: 139
	private float lastAddedPoison;

	// Token: 0x0400008C RID: 140
	public const float STATUS_INCREMENT = 0.025f;

	// Token: 0x0400008D RID: 141
	public const float MAX_TOTAL_STATUS = 2f;

	// Token: 0x0400008E RID: 142
	public const float SKELETON_INJURY_MULTIPLIER = 8f;

	// Token: 0x0400008F RID: 143
	public CharacterAfflictions.STATUSTYPE debugStatusType;

	// Token: 0x04000090 RID: 144
	public GameObject whirlwind;

	// Token: 0x020003F7 RID: 1015
	public enum STATUSTYPE
	{
		// Token: 0x0400171F RID: 5919
		Injury,
		// Token: 0x04001720 RID: 5920
		Hunger,
		// Token: 0x04001721 RID: 5921
		Cold,
		// Token: 0x04001722 RID: 5922
		Poison,
		// Token: 0x04001723 RID: 5923
		Crab,
		// Token: 0x04001724 RID: 5924
		Curse,
		// Token: 0x04001725 RID: 5925
		Drowsy,
		// Token: 0x04001726 RID: 5926
		Weight,
		// Token: 0x04001727 RID: 5927
		Hot,
		// Token: 0x04001728 RID: 5928
		Thorns,
		// Token: 0x04001729 RID: 5929
		Spores,
		// Token: 0x0400172A RID: 5930
		Web
	}
}
