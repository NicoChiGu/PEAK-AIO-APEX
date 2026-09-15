using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020000C3 RID: 195
public class GUIManager : MonoBehaviour
{
	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000723 RID: 1827 RVA: 0x00028434 File Offset: 0x00026634
	public bool wheelActive
	{
		get
		{
			return this.emoteWheel.gameObject.activeSelf || this.backpackWheel.gameObject.activeSelf;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000724 RID: 1828 RVA: 0x0002845A File Offset: 0x0002665A
	// (set) Token: 0x06000725 RID: 1829 RVA: 0x00028462 File Offset: 0x00026662
	internal IInteractible currentInteractable { get; private set; }

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000726 RID: 1830 RVA: 0x0002846B File Offset: 0x0002666B
	// (set) Token: 0x06000727 RID: 1831 RVA: 0x00028473 File Offset: 0x00026673
	public ControllerManager controllerManager { get; private set; }

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000728 RID: 1832 RVA: 0x0002847C File Offset: 0x0002667C
	public static bool InPauseMenu
	{
		get
		{
			GUIManager guimanager = GUIManager.instance;
			return guimanager != null && guimanager.pauseMenu.activeSelf;
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000729 RID: 1833 RVA: 0x00028493 File Offset: 0x00026693
	// (set) Token: 0x0600072A RID: 1834 RVA: 0x0002849A File Offset: 0x0002669A
	public static float TimeLastPaused { get; private set; }

	// Token: 0x0600072B RID: 1835 RVA: 0x000284A2 File Offset: 0x000266A2
	private void Awake()
	{
		GUIManager.instance = this;
		this.controllerManager = new ControllerManager();
		this.controllerManager.Init();
		this.InitReticleList();
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x000284C8 File Offset: 0x000266C8
	private void OnDestroy()
	{
		this.controllerManager.Destroy();
		if (this.character != null)
		{
			CharacterItems characterItems = this.character.refs.items;
			characterItems.onSlotEquipped = (Action)Delegate.Remove(characterItems.onSlotEquipped, new Action(this.OnSlotEquipped));
			GameUtils gameUtils = GameUtils.instance;
			gameUtils.OnUpdatedFeedData = (Action)Delegate.Remove(gameUtils.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
		}
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00028548 File Offset: 0x00026748
	private void Start()
	{
		this.UpdateItemPrompts();
		this.OnInteractChange();
		this.throwGO.SetActive(false);
		this.spectatingObject.SetActive(false);
		this.heroObject.SetActive(false);
		PhotosensitiveSetting setting = GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>();
		ColorblindSetting setting2 = GameHandler.Instance.SettingsHandler.GetSetting<ColorblindSetting>();
		this.photosensitivity = setting.Value == 1;
		this.colorblindness = setting2.Value == 1;
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x000285C4 File Offset: 0x000267C4
	private void LateUpdate()
	{
		this.UpdateDebug();
		this.UpdateBinocularOverlay();
		this.UpdateWindowStatus();
		if (Character.localCharacter)
		{
			if (Interaction.instance.currentHovered != this.currentInteractable)
			{
				this.OnInteractChange();
			}
			if (this.wasPitonClimbing)
			{
				this.RefreshInteractablePrompt();
			}
			this.interactPromptLunge.SetActive(Character.localCharacter.data.isClimbing && Character.localCharacter.data.currentStamina < 0.05f && Character.localCharacter.data.currentStamina > 0.0001f);
			this.wasPitonClimbing = Character.localCharacter.data.climbingSpikeCount > 0 && Character.localCharacter.data.isClimbing;
			if (!this.character)
			{
				this.character = Character.localCharacter;
				CharacterItems characterItems = this.character.refs.items;
				characterItems.onSlotEquipped = (Action)Delegate.Combine(characterItems.onSlotEquipped, new Action(this.OnSlotEquipped));
				GameUtils gameUtils = GameUtils.instance;
				gameUtils.OnUpdatedFeedData = (Action)Delegate.Combine(gameUtils.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
			}
			this.UpdateReticle();
			this.UpdateThrow();
			this.UpdateRope();
			this.UpdateDyingBar();
			this.UpdateEmoteWheel();
			this.TestUpdateItemPrompts();
			this.UpdateSpectate();
			this.UpdatePaused();
		}
		if (Character.observedCharacter)
		{
			this.UpdateItems();
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x0600072F RID: 1839 RVA: 0x0002873E File Offset: 0x0002693E
	// (set) Token: 0x06000730 RID: 1840 RVA: 0x00028746 File Offset: 0x00026946
	public bool windowShowingCursor { get; private set; }

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000731 RID: 1841 RVA: 0x0002874F File Offset: 0x0002694F
	// (set) Token: 0x06000732 RID: 1842 RVA: 0x00028757 File Offset: 0x00026957
	public bool windowBlockingInput { get; private set; }

	// Token: 0x06000733 RID: 1843 RVA: 0x00028760 File Offset: 0x00026960
	public void UpdateWindowStatus()
	{
		this.windowShowingCursor = false;
		this.windowBlockingInput = false;
		foreach (MenuWindow menuWindow in MenuWindow.AllActiveWindows)
		{
			if (menuWindow.blocksPlayerInput)
			{
				this.lastBlockedInput = Time.frameCount;
			}
			if (menuWindow.showCursorWhileOpen)
			{
				this.windowShowingCursor = true;
			}
		}
		if (this.pauseMenu.activeSelf)
		{
			this.windowShowingCursor = true;
			this.windowBlockingInput = true;
		}
		if (Time.frameCount < this.lastBlockedInput + 2)
		{
			this.windowBlockingInput = true;
		}
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x0002880C File Offset: 0x00026A0C
	public void UpdatePaused()
	{
		if (Character.localCharacter.input.pauseWasPressed && !LoadingScreenHandler.loading && !this.pauseMenu.activeSelf)
		{
			if (this.wheelActive)
			{
				return;
			}
			if (this.endScreen.isOpen)
			{
				return;
			}
			this.pauseMenu.SetActive(true);
			Character.localCharacter.input.pauseWasPressed = false;
		}
		if (GUIManager.InPauseMenu)
		{
			GUIManager.TimeLastPaused = Time.unscaledTime;
		}
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x00028884 File Offset: 0x00026A84
	private void OnSlotEquipped()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (i < Character.localCharacter.player.itemSlots.Length)
			{
				this.items[i].SetSelected();
			}
		}
		this.backpack.SetSelected();
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x000288D0 File Offset: 0x00026AD0
	private void OnUpdatedFeedData()
	{
		GUIManager.<>c__DisplayClass137_0 CS$<>8__locals1 = new GUIManager.<>c__DisplayClass137_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.feedData = GameUtils.instance.GetFeedDataForReceiver(Character.localCharacter.photonView.ViewID);
		int j;
		int num;
		for (j = 0; j < CS$<>8__locals1.feedData.Count; j = num + 1)
		{
			if (!Enumerable.Any<UI_UseItemProgressFriend>(this.friendUseItemProgressList, (UI_UseItemProgressFriend f) => f.giverID == CS$<>8__locals1.feedData[j].giverID))
			{
				UI_UseItemProgressFriend ui_UseItemProgressFriend = Object.Instantiate<UI_UseItemProgressFriend>(this.friendUseItemProgressPrefab, this.friendProgressTF);
				this.friendUseItemProgressList.Add(ui_UseItemProgressFriend);
				ui_UseItemProgressFriend.Init(CS$<>8__locals1.feedData[j]);
			}
			num = j;
		}
		int i;
		for (i = 0; i < this.friendUseItemProgressList.Count; i = num + 1)
		{
			if (!Enumerable.Any<FeedData>(CS$<>8__locals1.feedData, (FeedData f) => f.giverID == CS$<>8__locals1.<>4__this.friendUseItemProgressList[i].giverID))
			{
				this.friendUseItemProgressList[i].Kill();
				this.friendUseItemProgressList.RemoveAt(i);
			}
			num = i;
		}
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x00028A24 File Offset: 0x00026C24
	public void SetHeroTitle(string text, AudioClip stinger)
	{
		Debug.Log("Set hero title: " + text);
		if (this._heroRoutine != null)
		{
			base.StopCoroutine(this._heroRoutine);
		}
		if (this.stingerSound && stinger != null)
		{
			this.stingerSound.clip = stinger;
			this.stingerSound.Play();
		}
		this._heroRoutine = base.StartCoroutine(this.<SetHeroTitle>g__HeroRoutine|138_0(text));
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x00028A95 File Offset: 0x00026C95
	public void OpenBackpackWheel(BackpackReference backpackReference)
	{
		if (!this.wheelActive && !this.windowBlockingInput)
		{
			Character.localCharacter.data.usingBackpackWheel = true;
			this.backpackWheel.InitWheel(backpackReference);
		}
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00028AC3 File Offset: 0x00026CC3
	public void CloseBackpackWheel()
	{
		Debug.Log("Close Input Wheel");
		Character.localCharacter.data.usingBackpackWheel = false;
		this.backpackWheel.gameObject.SetActive(false);
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x0600073A RID: 1850 RVA: 0x00028AF0 File Offset: 0x00026CF0
	private bool canEmote
	{
		get
		{
			return !Character.localCharacter.data.dead;
		}
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x00028B04 File Offset: 0x00026D04
	private void UpdateEmoteWheel()
	{
		if (this.canEmote && Character.localCharacter.input.emoteIsPressed)
		{
			if (!this.wheelActive && !this.windowBlockingInput)
			{
				this.emoteWheel.SetActive(true);
				Character.localCharacter.data.usingEmoteWheel = true;
				return;
			}
		}
		else if (Character.localCharacter.data.usingEmoteWheel)
		{
			this.emoteWheel.SetActive(false);
			Character.localCharacter.data.usingEmoteWheel = false;
		}
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x00028B84 File Offset: 0x00026D84
	private void UpdateDyingBar()
	{
		this.dyingBarObject.gameObject.SetActive(Character.localCharacter.data.fullyPassedOut || Character.localCharacter.data.dead);
		if (this.dyingBarObject.gameObject.activeSelf)
		{
			this.dyingBarImage.fillAmount = 1f - Character.localCharacter.data.deathTimer;
			this.dyingBarImage.color = this.dyingBarGradient.Evaluate(1f - Character.localCharacter.data.deathTimer);
			this.dyingBarMushrooms.SetActive(Character.localCharacter.refs.afflictions.willZombify);
			if (Character.localCharacter.data.deathTimer >= 1f && !this.dead)
			{
				this.dyingBarAnimator.Play("Dead", 0, 0f);
				this.dead = true;
				return;
			}
		}
		else
		{
			this.dead = false;
		}
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x00028C88 File Offset: 0x00026E88
	private void UpdateSpectate()
	{
		if (MainCameraMovement.specCharacter != this.currentSpecCharacter)
		{
			this.currentSpecCharacter = MainCameraMovement.specCharacter;
			if (this.currentSpecCharacter)
			{
				this.spectatingObject.SetActive(true);
				this.spectatingInputs.SetActive(Character.localCharacter.data.dead);
				if (this.currentSpecCharacter == Character.localCharacter)
				{
					this.spectatingNameText.text = LocalizedText.GetText("YOURSELF", true);
					this.spectatingNameText.color = this.spectatingYourselfColor;
					return;
				}
				this.spectatingNameText.text = MainCameraMovement.specCharacter.characterName;
				this.spectatingNameText.color = this.spectatingNameColor;
				return;
			}
			else
			{
				this.spectatingObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x00028D58 File Offset: 0x00026F58
	private void UpdateRope()
	{
		RopeSpool ropeSpool;
		if (Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.TryGetComponent<RopeSpool>(ref ropeSpool))
		{
			this.ui_rope.gameObject.SetActive(true);
			if (ropeSpool.rope)
			{
				this.ui_rope.UpdateRope(ropeSpool.rope.GetRopeSegments().Count);
			}
			Shader.SetGlobalFloat(this.ROPE_INVERT, (float)(ropeSpool.isAntiRope ? 1 : 0));
			return;
		}
		this.ui_rope.gameObject.SetActive(false);
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x00028DF6 File Offset: 0x00026FF6
	private void UpdateWebs()
	{
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x00028DF8 File Offset: 0x00026FF8
	private void UpdateThrow()
	{
		this.throwGO.SetActive(Character.localCharacter.refs.items.throwChargeLevel > 0f);
		if (Character.localCharacter.refs.items.throwChargeLevel > 0f)
		{
			float num = Mathf.Lerp(0.692f, 0.808f, Character.localCharacter.refs.items.throwChargeLevel);
			this.throwBar.fillAmount = num;
			this.throwBar.color = this.throwGradient.Evaluate(Character.localCharacter.refs.items.throwChargeLevel);
		}
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x00028EA0 File Offset: 0x000270A0
	private void UpdateReticle()
	{
		this.reticleDefaultImage.color = ((this.character.data.sinceCanClimb < 0.05f) ? this.reticleColorHighlight : this.reticleColorDefault);
		if (Character.localCharacter.data.fullyPassedOut || Character.localCharacter.data.dead)
		{
			this.SetReticle(null);
			return;
		}
		if (this.reticleLock > 0f)
		{
			this.reticleLock -= Time.deltaTime;
			return;
		}
		if (Character.localCharacter.data.currentClimbHandle != null)
		{
			this.SetReticle(this.reticleSpike);
			return;
		}
		if (Character.localCharacter.data.isRopeClimbing)
		{
			this.SetReticle(this.reticleRope);
			return;
		}
		if (Character.localCharacter.data.sincePalJump < 0.5f)
		{
			this.SetReticle(this.reticleBoost);
			return;
		}
		if (Character.localCharacter.refs.items.throwChargeLevel > 0f)
		{
			this.SetReticle(this.reticleThrow);
			return;
		}
		if (Character.localCharacter.data.sincePressClimb < 0.1f && Character.localCharacter.refs.climbing.CanClimb())
		{
			this.SetReticle(this.reticleClimbTry);
			return;
		}
		if (Character.localCharacter.data.isClimbing)
		{
			if (Character.localCharacter.OutOfStamina())
			{
				this.SetReticle(this.reticleX);
				return;
			}
			this.SetReticle(this.reticleClimb);
			return;
		}
		else
		{
			if (Character.localCharacter.data.isReaching)
			{
				this.SetReticle(this.reticleReach);
				return;
			}
			if (Character.localCharacter.data.isVineClimbing)
			{
				this.SetReticle(this.reticleVine);
				return;
			}
			if (Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.UIData.isShootable && Character.localCharacter.data.currentItem.CanUsePrimary())
			{
				this.SetReticle(this.reticleShoot);
				return;
			}
			if (!this.emoteWheel.gameObject.activeSelf)
			{
				this.SetReticle(this.reticleDefault);
				return;
			}
			this.SetReticle(null);
			return;
		}
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x000290DC File Offset: 0x000272DC
	public void ReticleLand()
	{
		RectTransform component = this.reticleDefault.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(40f, 10f);
		TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOSizeDelta(component, new Vector2(10f, 10f), 0.33f, false), 10);
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0002912B File Offset: 0x0002732B
	public void Grasp()
	{
		this.SetReticle(this.reticleGrasp);
		this.reticleGrasp.GetComponent<Animator>().Play("Play", 0, 0f);
		this.reticleLock = 1f;
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0002915F File Offset: 0x0002735F
	public void ClimbJump()
	{
		this.SetReticle(this.reticleClimbJump);
		this.reticleLock = 0.5f;
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x00029178 File Offset: 0x00027378
	private void SetReticle(GameObject activeReticle)
	{
		if (activeReticle == this.lastReticle && activeReticle != null)
		{
			return;
		}
		this.lastReticle = activeReticle;
		for (int i = 0; i < this.reticleList.Count; i++)
		{
			if (this.reticleList[i] != activeReticle)
			{
				this.reticleList[i].SetActive(false);
			}
		}
		if (activeReticle)
		{
			activeReticle.SetActive(true);
		}
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x000291F0 File Offset: 0x000273F0
	private void InitReticleList()
	{
		this.reticleList.Add(this.reticleDefault);
		this.reticleList.Add(this.reticleRope);
		this.reticleList.Add(this.reticleSpike);
		this.reticleList.Add(this.reticleThrow);
		this.reticleList.Add(this.reticleReach);
		this.reticleList.Add(this.reticleX);
		this.reticleList.Add(this.reticleClimb);
		this.reticleList.Add(this.reticleClimbJump);
		this.reticleList.Add(this.reticleClimbTry);
		this.reticleList.Add(this.reticleGrasp);
		this.reticleList.Add(this.reticleVine);
		this.reticleList.Add(this.reticleBoost);
		this.reticleList.Add(this.reticleShoot);
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x000292DA File Offset: 0x000274DA
	private void UpdateDebug()
	{
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x000292DC File Offset: 0x000274DC
	private IEnumerator ScreenshotRoutine(bool disableHud)
	{
		bool cacheEnabled = this.hudCanvas.enabled;
		if (disableHud)
		{
			this.hudCanvas.enabled = false;
		}
		yield return null;
		string text = "";
		if (Application.isEditor)
		{
			text = "Screenshots/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
		}
		string text2 = "Screenshot_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
		ScreenCapture.CaptureScreenshot(Path.Combine(text, text2), 2);
		yield return null;
		this.hudCanvas.enabled = cacheEnabled;
		yield break;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x000292F4 File Offset: 0x000274F4
	public void AddStatusFX(CharacterAfflictions.STATUSTYPE type, float amount)
	{
		switch (type)
		{
		case CharacterAfflictions.STATUSTYPE.Injury:
			this.InjuryFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Hunger:
			this.HungerFX();
			return;
		case CharacterAfflictions.STATUSTYPE.Cold:
			this.ColdFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Poison:
			this.PoisonFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Curse:
			this.CurseFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Drowsy:
			this.DrowsyFX();
			return;
		case CharacterAfflictions.STATUSTYPE.Hot:
			this.HotFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Thorns:
			this.ThornsFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Spores:
			this.SporesFX(1f);
			return;
		case CharacterAfflictions.STATUSTYPE.Web:
			return;
		}
		this.InjuryFX(amount);
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0002938A File Offset: 0x0002758A
	private void InjuryFX(float amount)
	{
		GamefeelHandler.instance.AddPerlinShake((amount + 1f) * 5f, 0.3f, 15f);
		this.injurySVFX.Play(amount);
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x000293B9 File Offset: 0x000275B9
	private void CurseFX(float amount)
	{
		GamefeelHandler.instance.AddPerlinShake((amount + 1f) * 30f, 0.3f, 15f);
		this.curseSVFX.Play(amount);
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x000293E8 File Offset: 0x000275E8
	private void HungerFX()
	{
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x000293EC File Offset: 0x000275EC
	private void DrowsyFX()
	{
		float num = 1f;
		GamefeelHandler.instance.AddPerlinShake(num * 5f, 0.3f, 15f);
		this.drowsyFX.Play(num);
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00029426 File Offset: 0x00027626
	private void PoisonFX(float amount)
	{
		amount = 0.5f;
		GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f, 15f);
		this.poisonSVFX.Play(amount);
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x00029456 File Offset: 0x00027656
	private void ThornsFX(float amount)
	{
		amount = 0.5f;
		GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f, 15f);
		this.thornsSVFX.Play(amount);
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00029486 File Offset: 0x00027686
	private void SporesFX(float amount)
	{
		GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f, 15f);
		this.sporesSVFX.Play(amount);
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x000294AF File Offset: 0x000276AF
	private void ColdFX(float amount)
	{
		amount = 1f;
		GamefeelHandler.instance.AddPerlinShake(amount * 2f, 1f, 30f);
		this.PlayFXSequence(ref this.coldSequence, this.coldVolume, amount);
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x000294E6 File Offset: 0x000276E6
	private void HotFX(float amount)
	{
		amount = 1f;
		GamefeelHandler.instance.AddPerlinShake(amount * 2f, 1f, 30f);
		this.hotSVFX.Play(amount);
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x00029518 File Offset: 0x00027718
	private void PlayFXSequence(ref Sequence sequence, Volume volume, float amount)
	{
		TweenExtensions.Kill(sequence, false);
		sequence = DOTween.Sequence();
		TweenSettingsExtensions.Append(sequence, DOTween.To(() => volume.weight, delegate(float x)
		{
			volume.weight = x;
		}, amount, 0.06f));
		TweenSettingsExtensions.AppendInterval(sequence, 0.25f * amount);
		TweenSettingsExtensions.Append(sequence, DOTween.To(() => volume.weight, delegate(float x)
		{
			volume.weight = x;
		}, 0f, 0.45f));
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x000295AC File Offset: 0x000277AC
	public void StartSugarRush()
	{
		float num = 1f;
		if (GUIManager.instance.photosensitivity)
		{
			num = 0.25f;
		}
		DOTween.To(() => this.sugarRushVolume.weight, delegate(float x)
		{
			this.sugarRushVolume.weight = x;
		}, num, 0.5f);
		GUIManager.instance.bar.AddRainbow();
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00029604 File Offset: 0x00027804
	public void EndSugarRush()
	{
		DOTween.To(() => this.sugarRushVolume.weight, delegate(float x)
		{
			this.sugarRushVolume.weight = x;
		}, 0f, 0.5f);
		GUIManager.instance.bar.RemoveRainbow();
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x0002963D File Offset: 0x0002783D
	public void StartEnergyDrink()
	{
		this.energySVFX.StartFX(0.15f);
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0002964F File Offset: 0x0002784F
	public void EndEnergyDrink()
	{
		this.energySVFX.EndFX();
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x0002965C File Offset: 0x0002785C
	private void HeatFX(float amount)
	{
		amount = 1f;
		this.heatSVFX.Play(amount);
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00029671 File Offset: 0x00027871
	public void StartHeat()
	{
		this.heatSVFX.StartFX(0.5f);
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x00029683 File Offset: 0x00027883
	public void EndHeat()
	{
		this.heatSVFX.EndFX();
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00029690 File Offset: 0x00027890
	public void StartSunscreen()
	{
		this.sunscreenSVFX.StartFX(0.5f);
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x000296A2 File Offset: 0x000278A2
	public void EndSunscreen()
	{
		this.sunscreenSVFX.EndFX();
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x000296B0 File Offset: 0x000278B0
	private void OnInteractChange()
	{
		if (ClassExtensionsMethods.UnityObjectExists<IInteractible>(this.currentInteractable))
		{
			this.currentInteractable.HoverExit();
		}
		this.currentInteractable = Interaction.instance.currentHovered;
		if (ClassExtensionsMethods.UnityObjectExists<IInteractible>(this.currentInteractable))
		{
			this.currentInteractable.HoverEnter();
		}
		this.RefreshInteractablePrompt();
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00029704 File Offset: 0x00027904
	public void RefreshInteractablePrompt()
	{
		if (ClassExtensionsMethods.UnityObjectExists<IInteractible>(this.currentInteractable))
		{
			this.interactPromptText.text = this.currentInteractable.GetInteractionText();
			this.interactName.SetActive(true);
			this.interactPromptPrimary.SetActive(true);
			this.interactPromptSecondary.SetActive(false);
			this.interactPromptHold.SetActive(false);
			if (this.currentInteractable is Item)
			{
				this.interactNameText.text = ((Item)this.currentInteractable).GetItemName(null);
			}
			else
			{
				CharacterInteractible characterInteractible = this.currentInteractable as CharacterInteractible;
				if (characterInteractible != null)
				{
					this.interactPromptPrimary.SetActive(characterInteractible.IsPrimaryInteractible(Character.localCharacter));
					this.interactName.SetActive(false);
					if (characterInteractible.IsSecondaryInteractible(Character.localCharacter))
					{
						this.interactPromptSecondary.SetActive(true);
						this.secondaryInteractPromptText.text = characterInteractible.GetSecondaryInteractionText();
					}
				}
				else
				{
					this.interactNameText.text = this.currentInteractable.GetName();
				}
			}
		}
		else
		{
			this.interactName.SetActive(false);
			this.interactPromptPrimary.SetActive(false);
			this.interactPromptSecondary.SetActive(false);
			this.interactPromptHold.SetActive(false);
		}
		if (Character.localCharacter && Character.localCharacter.data.climbingSpikeCount > 0 && Character.localCharacter.data.isClimbing)
		{
			this.interactPromptSecondary.SetActive(true);
			this.secondaryInteractPromptText.text = LocalizedText.GetText("SETPITON", true);
		}
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x0002988B File Offset: 0x00027A8B
	public void EnableBinocularOverlay()
	{
		this.sinceShowedBinocularOverlay = 0;
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x00029894 File Offset: 0x00027A94
	private void UpdateBinocularOverlay()
	{
		if (this.sinceShowedBinocularOverlay > 1)
		{
			this.binocularOverlay.enabled = false;
		}
		else
		{
			this.binocularOverlay.enabled = true;
		}
		this.sinceShowedBinocularOverlay++;
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x000298C7 File Offset: 0x00027AC7
	public void BlurBinoculars()
	{
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x000298CC File Offset: 0x00027ACC
	public void UpdateItems()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		if (Character.observedCharacter == null || Character.observedCharacter.player == null)
		{
			for (int i = 0; i < this.items.Length; i++)
			{
				this.items[i].SetItem(null);
			}
			this.backpack.SetItem(null);
			this.UpdateItemPrompts();
			this.temporaryItem.gameObject.SetActive(false);
			return;
		}
		for (int j = 0; j < this.items.Length; j++)
		{
			if (j < Character.observedCharacter.player.itemSlots.Length)
			{
				this.items[j].SetItem(Character.observedCharacter.player.itemSlots[j]);
			}
		}
		this.backpack.SetItem(Character.observedCharacter.player.backpackSlot);
		if (!Character.observedCharacter.player.GetItemSlot(250).IsEmpty())
		{
			this.temporaryItem.gameObject.SetActive(true);
			this.temporaryItem.SetItem(Character.observedCharacter.player.GetItemSlot(250));
		}
		else
		{
			this.temporaryItem.gameObject.SetActive(false);
			this.temporaryItem.Clear();
		}
		this.UpdateItemPrompts();
		this.bar.ChangeBar();
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x00029A24 File Offset: 0x00027C24
	public void PlayDayNightText(int x)
	{
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x00029A28 File Offset: 0x00027C28
	private void TestUpdateItemPrompts()
	{
		if (!Character.localCharacter || !Character.localCharacter.data.currentItem)
		{
			this.canUsePrimaryPrevious = false;
			this.canUseSecondaryPrevious = false;
			return;
		}
		bool flag = Character.localCharacter.data.currentItem.CanUsePrimary();
		bool flag2 = Character.localCharacter.data.currentItem.CanUseSecondary();
		if (flag != this.canUsePrimaryPrevious || flag2 != this.canUseSecondaryPrevious)
		{
			this.UpdateItemPrompts();
		}
		this.canUsePrimaryPrevious = flag;
		this.canUsePrimaryPrevious = flag2;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x00029AB8 File Offset: 0x00027CB8
	public void UpdateItemPrompts()
	{
		if (Character.localCharacter != null && Character.localCharacter.data.currentItem)
		{
			Item currentItem = Character.localCharacter.data.currentItem;
			Item.ItemUIData uidata = currentItem.UIData;
			this.itemPromptMain.text = this.GetMainInteractPrompt(currentItem);
			this.itemPromptSecondary.text = this.GetSecondaryInteractPrompt(currentItem);
			this.itemPromptScroll.text = LocalizedText.GetText(uidata.scrollInteractPrompt, true);
			this.itemPromptMain.gameObject.SetActive(uidata.hasMainInteract && Character.localCharacter.data.currentItem.CanUsePrimary());
			this.itemPromptSecondary.gameObject.SetActive(uidata.hasSecondInteract && Character.localCharacter.data.currentItem.CanUseSecondary());
			this.itemPromptScroll.gameObject.SetActive(uidata.hasScrollingInteract);
			this.itemPromptDrop.gameObject.SetActive(uidata.canDrop);
			this.itemPromptThrow.gameObject.SetActive(uidata.canThrow);
			return;
		}
		this.itemPromptMain.gameObject.SetActive(false);
		this.itemPromptSecondary.gameObject.SetActive(false);
		this.itemPromptScroll.gameObject.SetActive(false);
		this.itemPromptDrop.gameObject.SetActive(false);
		this.itemPromptThrow.gameObject.SetActive(false);
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x00029C36 File Offset: 0x00027E36
	public void TheFogRises()
	{
		this.fogRises.SetActive(true);
		base.StartCoroutine(this.<TheFogRises>g__FogRisesRoutine|199_0());
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00029C51 File Offset: 0x00027E51
	public void TheLavaRises()
	{
		this.lavaRises.SetActive(true);
		base.StartCoroutine(this.<TheLavaRises>g__FogRisesRoutine|200_0());
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00029C6C File Offset: 0x00027E6C
	private string GetMainInteractPrompt(Item item)
	{
		return LocalizedText.GetText(item.UIData.mainInteractPrompt, true);
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00029C7F File Offset: 0x00027E7F
	public string GetSecondaryInteractPrompt(Item item)
	{
		return LocalizedText.GetText(item.UIData.secondaryInteractPrompt, true);
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x00029C92 File Offset: 0x00027E92
	public void StartNumb()
	{
		DOTweenModuleUI.DOFade(this.staminaCanvasGroup, 0f, 1f);
		DOTweenModuleUI.DOFade(this.mushroomsCanvasGroup, 1f, 1f);
		this.mushroomsCanvasGroup.gameObject.SetActive(true);
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x00029CD1 File Offset: 0x00027ED1
	public void StopNumb()
	{
		DOTweenModuleUI.DOFade(this.staminaCanvasGroup, 1f, 1f);
		TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(DOTweenModuleUI.DOFade(this.mushroomsCanvasGroup, 0f, 1f), delegate
		{
			this.mushroomsCanvasGroup.gameObject.SetActive(false);
		});
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x00029D46 File Offset: 0x00027F46
	[CompilerGenerated]
	private IEnumerator <SetHeroTitle>g__HeroRoutine|138_0(string heroString)
	{
		this.heroCanvasObject.gameObject.SetActive(true);
		yield return null;
		string dayString = DayNightManager.instance.DayCountString();
		string timeOfDayString = DayNightManager.instance.TimeOfDayString();
		this.heroObject.gameObject.SetActive(true);
		this.heroImage.color = new Color(this.heroImage.color.r, this.heroImage.color.g, this.heroImage.color.b, 1f);
		this.heroShadowImage.color = new Color(this.heroShadowImage.color.r, this.heroShadowImage.color.g, this.heroShadowImage.color.b, 0.12f);
		this.heroDayText.text = "";
		this.heroTimeOfDayText.text = "";
		this.heroBG.color = new Color(0f, 0f, 0f, 0f);
		DOTweenModuleUI.DOFade(this.heroBG, 0.5f, 0.5f);
		int num;
		for (int i = 0; i < heroString.Length; i = num + 1)
		{
			this.heroText.text = heroString.Substring(0, i + 1);
			this.heroCamera.Render();
			yield return new WaitForSeconds(0.1f);
			num = i;
		}
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < dayString.Length; i = num + 1)
		{
			this.heroDayText.text = dayString.Substring(0, i + 1);
			this.heroCamera.Render();
			yield return new WaitForSeconds(0.066f);
			num = i;
		}
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < timeOfDayString.Length; i = num + 1)
		{
			this.heroTimeOfDayText.text = timeOfDayString.Substring(0, i + 1);
			this.heroCamera.Render();
			yield return new WaitForSeconds(0.066f);
			num = i;
		}
		yield return new WaitForSeconds(1.5f);
		DOTweenModuleUI.DOFade(this.heroImage, 0f, 2f);
		DOTweenModuleUI.DOFade(this.heroShadowImage, 0f, 1f);
		DOTweenModuleUI.DOFade(this.heroBG, 0f, 2f);
		yield return new WaitForSeconds(2f);
		this.heroObject.gameObject.SetActive(false);
		this.heroCanvasObject.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00029D92 File Offset: 0x00027F92
	[CompilerGenerated]
	private IEnumerator <TheFogRises>g__FogRisesRoutine|199_0()
	{
		yield return new WaitForSeconds(4f);
		this.fogRises.SetActive(false);
		yield break;
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00029DA1 File Offset: 0x00027FA1
	[CompilerGenerated]
	private IEnumerator <TheLavaRises>g__FogRisesRoutine|200_0()
	{
		yield return new WaitForSeconds(4f);
		this.lavaRises.SetActive(false);
		yield break;
	}

	// Token: 0x0400070D RID: 1805
	public static GUIManager instance;

	// Token: 0x0400070E RID: 1806
	public Canvas hudCanvas;

	// Token: 0x0400070F RID: 1807
	public Canvas binocularOverlay;

	// Token: 0x04000710 RID: 1808
	public Canvas letterboxCanvas;

	// Token: 0x04000711 RID: 1809
	public BoardingPass boardingPass;

	// Token: 0x04000712 RID: 1810
	public StaminaBar bar;

	// Token: 0x04000713 RID: 1811
	public InventoryItemUI[] items;

	// Token: 0x04000714 RID: 1812
	public InventoryItemUI backpack;

	// Token: 0x04000715 RID: 1813
	public InventoryItemUI temporaryItem;

	// Token: 0x04000716 RID: 1814
	public CanvasGroup hudCanvasGroup;

	// Token: 0x04000717 RID: 1815
	public CanvasGroup staminaCanvasGroup;

	// Token: 0x04000718 RID: 1816
	public CanvasGroup mushroomsCanvasGroup;

	// Token: 0x04000719 RID: 1817
	public Sprite emptySprite;

	// Token: 0x0400071A RID: 1818
	public UI_Rope ui_rope;

	// Token: 0x0400071B RID: 1819
	public GameObject emoteWheel;

	// Token: 0x0400071C RID: 1820
	public BackpackWheel backpackWheel;

	// Token: 0x0400071D RID: 1821
	public UIPlayerNames playerNames;

	// Token: 0x0400071E RID: 1822
	public UI_UseItemProgressFriend friendUseItemProgressPrefab;

	// Token: 0x0400071F RID: 1823
	public Transform friendProgressTF;

	// Token: 0x04000720 RID: 1824
	public GameObject fogRises;

	// Token: 0x04000721 RID: 1825
	public GameObject lavaRises;

	// Token: 0x04000722 RID: 1826
	public LoadingScreen loadingScreenPrefab;

	// Token: 0x04000723 RID: 1827
	[FormerlySerializedAs("endgameCounter")]
	public EndgameCounter endgame;

	// Token: 0x04000724 RID: 1828
	public EndScreen endScreen;

	// Token: 0x04000725 RID: 1829
	[SerializeField]
	private GameObject pauseMenu;

	// Token: 0x04000726 RID: 1830
	public PauseMenuMainPage pauseMenuMainPage;

	// Token: 0x04000727 RID: 1831
	public List<UI_UseItemProgressFriend> friendUseItemProgressList = new List<UI_UseItemProgressFriend>();

	// Token: 0x04000728 RID: 1832
	private TextMeshProUGUI text;

	// Token: 0x0400072A RID: 1834
	public GameObject interactName;

	// Token: 0x0400072B RID: 1835
	public TextMeshProUGUI interactNameText;

	// Token: 0x0400072C RID: 1836
	public GameObject interactPromptPrimary;

	// Token: 0x0400072D RID: 1837
	public GameObject interactPromptSecondary;

	// Token: 0x0400072E RID: 1838
	public GameObject interactPromptHold;

	// Token: 0x0400072F RID: 1839
	public GameObject interactPromptLunge;

	// Token: 0x04000730 RID: 1840
	public TextMeshProUGUI interactPromptText;

	// Token: 0x04000731 RID: 1841
	public TextMeshProUGUI secondaryInteractPromptText;

	// Token: 0x04000732 RID: 1842
	public CanvasGroup strugglePrompt;

	// Token: 0x04000733 RID: 1843
	public TextMeshProUGUI itemPromptMain;

	// Token: 0x04000734 RID: 1844
	public TextMeshProUGUI itemPromptScroll;

	// Token: 0x04000735 RID: 1845
	public TextMeshProUGUI itemPromptSecondary;

	// Token: 0x04000736 RID: 1846
	public TextMeshProUGUI itemPromptDrop;

	// Token: 0x04000737 RID: 1847
	public TextMeshProUGUI itemPromptThrow;

	// Token: 0x04000738 RID: 1848
	public GameObject throwGO;

	// Token: 0x04000739 RID: 1849
	public Image throwBar;

	// Token: 0x0400073A RID: 1850
	public Gradient throwGradient;

	// Token: 0x0400073B RID: 1851
	public GameObject dyingBarObject;

	// Token: 0x0400073C RID: 1852
	public RectTransform dyingBarRect;

	// Token: 0x0400073D RID: 1853
	public Image dyingBarImage;

	// Token: 0x0400073E RID: 1854
	public Gradient dyingBarGradient;

	// Token: 0x0400073F RID: 1855
	public Animator dyingBarAnimator;

	// Token: 0x04000740 RID: 1856
	public GameObject dyingBarMushrooms;

	// Token: 0x04000741 RID: 1857
	public GameObject spectatingObject;

	// Token: 0x04000742 RID: 1858
	public GameObject spectatingInputs;

	// Token: 0x04000743 RID: 1859
	public TextMeshProUGUI spectatingNameText;

	// Token: 0x04000744 RID: 1860
	public Color spectatingNameColor;

	// Token: 0x04000745 RID: 1861
	public Color spectatingYourselfColor;

	// Token: 0x04000746 RID: 1862
	public GameObject heroObject;

	// Token: 0x04000747 RID: 1863
	public GameObject heroCanvasObject;

	// Token: 0x04000748 RID: 1864
	public Camera heroCamera;

	// Token: 0x04000749 RID: 1865
	public Image heroBG;

	// Token: 0x0400074A RID: 1866
	public RawImage heroImage;

	// Token: 0x0400074B RID: 1867
	public RawImage heroShadowImage;

	// Token: 0x0400074C RID: 1868
	public TextMeshProUGUI heroText;

	// Token: 0x0400074D RID: 1869
	public TextMeshProUGUI heroDayText;

	// Token: 0x0400074E RID: 1870
	public TextMeshProUGUI heroTimeOfDayText;

	// Token: 0x0400074F RID: 1871
	public AudioSource stingerSound;

	// Token: 0x04000750 RID: 1872
	public Volume blurVolume;

	// Token: 0x04000751 RID: 1873
	public Volume coldVolume;

	// Token: 0x04000752 RID: 1874
	public Volume sugarRushVolume;

	// Token: 0x04000753 RID: 1875
	public ScreenVFX injurySVFX;

	// Token: 0x04000754 RID: 1876
	public ScreenVFX coldSVFX;

	// Token: 0x04000755 RID: 1877
	public ScreenVFX poisonSVFX;

	// Token: 0x04000756 RID: 1878
	public ScreenVFX sugarRushSVFX;

	// Token: 0x04000757 RID: 1879
	public ScreenVFX hotSVFX;

	// Token: 0x04000758 RID: 1880
	public ScreenVFX energySVFX;

	// Token: 0x04000759 RID: 1881
	public ScreenVFX drowsyFX;

	// Token: 0x0400075A RID: 1882
	public ScreenVFX heatSVFX;

	// Token: 0x0400075B RID: 1883
	public ScreenVFX curseSVFX;

	// Token: 0x0400075C RID: 1884
	public ScreenVFX sunscreenSVFX;

	// Token: 0x0400075D RID: 1885
	public ScreenVFX thornsSVFX;

	// Token: 0x0400075E RID: 1886
	public ScreenVFX sporesSVFX;

	// Token: 0x0400075F RID: 1887
	public ScreenVFX sporesWarning;

	// Token: 0x04000760 RID: 1888
	private Character character;

	// Token: 0x04000761 RID: 1889
	public GameObject reticleDefault;

	// Token: 0x04000762 RID: 1890
	public GameObject reticleX;

	// Token: 0x04000763 RID: 1891
	public GameObject reticleClimb;

	// Token: 0x04000764 RID: 1892
	public GameObject reticleClimbJump;

	// Token: 0x04000765 RID: 1893
	public GameObject reticleThrow;

	// Token: 0x04000766 RID: 1894
	public GameObject reticleReach;

	// Token: 0x04000767 RID: 1895
	public GameObject reticleGrasp;

	// Token: 0x04000768 RID: 1896
	public GameObject reticleSpike;

	// Token: 0x04000769 RID: 1897
	public GameObject reticleRope;

	// Token: 0x0400076A RID: 1898
	public GameObject reticleClimbTry;

	// Token: 0x0400076B RID: 1899
	public GameObject reticleVine;

	// Token: 0x0400076C RID: 1900
	public GameObject reticleBoost;

	// Token: 0x0400076D RID: 1901
	public GameObject reticleShoot;

	// Token: 0x0400076E RID: 1902
	public Image reticleDefaultImage;

	// Token: 0x0400076F RID: 1903
	public Color reticleColorDefault;

	// Token: 0x04000770 RID: 1904
	public Color reticleColorHighlight;

	// Token: 0x04000771 RID: 1905
	private Coroutine _heroRoutine;

	// Token: 0x04000773 RID: 1907
	public BadgeManager mainBadgeManager;

	// Token: 0x04000775 RID: 1909
	public bool photosensitivity;

	// Token: 0x04000776 RID: 1910
	public bool colorblindness;

	// Token: 0x04000777 RID: 1911
	private bool wasPitonClimbing;

	// Token: 0x0400077A RID: 1914
	private int lastBlockedInput;

	// Token: 0x0400077B RID: 1915
	private bool dead;

	// Token: 0x0400077C RID: 1916
	private Character currentSpecCharacter;

	// Token: 0x0400077D RID: 1917
	private int ROPE_INVERT = Shader.PropertyToID("Invert");

	// Token: 0x0400077E RID: 1918
	private float reticleLock;

	// Token: 0x0400077F RID: 1919
	private GameObject lastReticle;

	// Token: 0x04000780 RID: 1920
	private List<GameObject> reticleList = new List<GameObject>();

	// Token: 0x04000781 RID: 1921
	private Sequence injurySequence;

	// Token: 0x04000782 RID: 1922
	private Sequence hungerSequence;

	// Token: 0x04000783 RID: 1923
	private Sequence coldSequence;

	// Token: 0x04000784 RID: 1924
	private Sequence poisonSequence;

	// Token: 0x04000785 RID: 1925
	public int sinceShowedBinocularOverlay = 10;

	// Token: 0x04000786 RID: 1926
	private bool canUsePrimaryPrevious;

	// Token: 0x04000787 RID: 1927
	private bool canUseSecondaryPrevious;

	// Token: 0x0200043C RID: 1084
	// (Invoke) Token: 0x06001A91 RID: 6801
	public delegate void MenuWindowEvent(MenuWindow window);
}
