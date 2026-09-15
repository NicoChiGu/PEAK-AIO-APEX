using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001C3 RID: 451
public class EndScreen : MenuWindow
{
	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000DE2 RID: 3554 RVA: 0x00045358 File Offset: 0x00043558
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000DE3 RID: 3555 RVA: 0x0004535B File Offset: 0x0004355B
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return null;
		}
	}

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x0004535E File Offset: 0x0004355E
	public override bool selectOnOpen
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x00045361 File Offset: 0x00043561
	private void Awake()
	{
		EndScreen.instance = this;
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x00045369 File Offset: 0x00043569
	protected override void Start()
	{
		base.Start();
		base.StartCoroutine(this.EndSequenceRoutine());
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00045380 File Offset: 0x00043580
	protected override void Initialize()
	{
		this.nextButton.onClick.AddListener(new UnityAction(this.Next));
		this.cosmeticNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
		this.ascentsNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
		this.promotionNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x000453FD File Offset: 0x000435FD
	private void Next()
	{
		this.WaitingForPlayersUI.gameObject.SetActive(true);
		Singleton<GameOverHandler>.Instance.LocalPlayerHasClosedEndScreen();
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x0004541A File Offset: 0x0004361A
	private IEnumerator EndSequenceRoutine()
	{
		UIInputHandler.SetSelectedObject(null);
		this.canvasGroup.alpha = 0f;
		DOTweenModuleUI.DOFade(this.canvasGroup, 1f, 1f);
		List<Character> activeCharacters = Enumerable.ToList<Character>(Enumerable.Where<Character>(Character.AllCharacters, delegate(Character c)
		{
			Player player;
			return c.photonView.IsOwnerActive && PlayerHandler.TryGetPlayer(c.photonView.OwnerActorNr, out player);
		}));
		if (activeCharacters.Count != Character.AllCharacters.Count)
		{
			Debug.LogWarning(string.Format("There were {0} broken Characters ", Character.AllCharacters.Count - activeCharacters.Count) + "in our character list! Why didn't those get destroyed??");
		}
		for (int i = 0; i < this.scoutWindows.Length; i++)
		{
			if (i < activeCharacters.Count)
			{
				this.scoutWindows[i].gameObject.SetActive(true);
				this.scoutWindows[i].Init(activeCharacters[i]);
			}
			else
			{
				this.scoutWindows[i].gameObject.SetActive(false);
			}
		}
		this.endTime.gameObject.SetActive(false);
		this.buttons.SetActive(false);
		this.peakBanner.SetActive(Character.localCharacter.refs.stats.won);
		this.yourFriendsWonBanner.SetActive(!Character.localCharacter.refs.stats.won && Character.localCharacter.refs.stats.somebodyElseWon);
		this.deadBanner.SetActive(!Character.localCharacter.refs.stats.won && !Character.localCharacter.refs.stats.somebodyElseWon);
		this.cosmeticUnlockObject.SetActive(false);
		yield return new WaitForSeconds(2f);
		try
		{
			this.endTime.text = this.GetTimeString(RunManager.Instance.timeSinceRunStarted);
			this.endTime.gameObject.SetActive(true);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
		if (Character.localCharacter.refs.stats.won)
		{
			Singleton<AchievementManager>.Instance.TestTimeAchievements();
		}
		yield return new WaitForSeconds(1f);
		yield return base.StartCoroutine(this.TimelineRoutine(activeCharacters));
		yield return new WaitForSeconds(0.25f);
		List<int> completedAscentsThisRun = Singleton<AchievementManager>.Instance.runBasedValueData.completedAscentsThisRun;
		yield return base.StartCoroutine(this.AscentRoutine(completedAscentsThisRun));
		yield return new WaitForSeconds(0.25f);
		this.selectedBadge = false;
		yield return base.StartCoroutine(this.BadgeRoutine());
		this.buttons.SetActive(true);
		if (!this.selectedBadge)
		{
			UIInputHandler.SetSelectedObject(this.returnToAirportButton.gameObject);
		}
		yield break;
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x0004542C File Offset: 0x0004362C
	private string GetTimeString(float totalSeconds)
	{
		int num = Mathf.FloorToInt(totalSeconds);
		int num2 = num / 3600;
		int num3 = num % 3600 / 60;
		int num4 = num % 60;
		return string.Format("{0}:{1:00}:{2:00}", num2, num3, num4);
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x00045472 File Offset: 0x00043672
	private IEnumerator TimelineRoutine(List<Character> allCharacters)
	{
		for (int j = 0; j < this.scouts.Length; j++)
		{
			this.scouts[j].gameObject.SetActive(false);
			this.scoutsAtPeak[j].gameObject.SetActive(false);
		}
		if (this.debug)
		{
			for (int k = 0; k < this.scouts.Length; k++)
			{
				this.scouts[k].color = this.debugColors[k];
				this.scoutsAtPeak[k].color = this.debugColors[k];
			}
		}
		else
		{
			for (int l = 0; l < allCharacters.Count; l++)
			{
				if (l < this.scouts.Length)
				{
					Color playerColor = allCharacters[l].refs.customization.PlayerColor;
					playerColor.a = 1f;
					this.scouts[l].color = playerColor;
					this.scoutsAtPeak[l].color = this.scouts[l].color;
				}
			}
		}
		yield return new WaitForSeconds(0.1f);
		List<List<EndScreen.TimelineInfo>> timelineInfos = new List<List<EndScreen.TimelineInfo>>();
		if (this.debug)
		{
			timelineInfos.Add(new List<EndScreen.TimelineInfo>());
			timelineInfos.Add(new List<EndScreen.TimelineInfo>());
			timelineInfos.Add(new List<EndScreen.TimelineInfo>());
			timelineInfos.Add(new List<EndScreen.TimelineInfo>());
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = Random.Range(10, this.pipCount - 10);
			for (int m = 0; m < this.pipCount; m++)
			{
				float num6 = (float)m / ((float)this.pipCount - 1f);
				EndScreen.TimelineInfo timelineInfo = default(EndScreen.TimelineInfo);
				num += this.GetRandom(num6) * 0.15f * num6;
				timelineInfo.height = Mathf.Clamp01(num6 + num);
				timelineInfo.time = num6;
				timelineInfos[0].Add(timelineInfo);
				EndScreen.TimelineInfo timelineInfo2 = default(EndScreen.TimelineInfo);
				num2 += this.GetRandom(num6) * 0.15f * num6;
				timelineInfo2.height = Mathf.Clamp01(num6 + num2);
				timelineInfo2.time = num6;
				timelineInfos[1].Add(timelineInfo2);
				EndScreen.TimelineInfo timelineInfo3 = default(EndScreen.TimelineInfo);
				num3 += this.GetRandom(num6) * 0.15f * num6;
				timelineInfo3.height = Mathf.Clamp01(num6 + num3);
				timelineInfo3.time = num6;
				timelineInfos[2].Add(timelineInfo3);
				EndScreen.TimelineInfo timelineInfo4 = default(EndScreen.TimelineInfo);
				num4 += this.GetRandom(num6) * 0.15f * num6;
				timelineInfo4.height = Mathf.Clamp01(num6 + num4);
				timelineInfo4.time = num6;
				if (m == num5)
				{
					timelineInfo4.died = true;
				}
				if (m > num5)
				{
					timelineInfo4.dead = true;
				}
				timelineInfos[3].Add(timelineInfo4);
			}
		}
		else
		{
			for (int n = 0; n < allCharacters.Count; n++)
			{
				if (allCharacters[n] != null)
				{
					timelineInfos.Add(allCharacters[n].refs.stats.timelineInfo);
				}
			}
		}
		for (int num7 = 0; num7 < timelineInfos.Count; num7++)
		{
			if (num7 < this.scouts.Length)
			{
				this.scouts[num7].gameObject.SetActive(true);
			}
		}
		int longestCount = 1;
		for (int num8 = 0; num8 < timelineInfos.Count; num8++)
		{
			if (timelineInfos[num8].Count > longestCount)
			{
				longestCount = timelineInfos[num8].Count;
			}
		}
		float startTime = 100000f;
		float maxTime = 0f;
		maxTime = Character.localCharacter.refs.stats.GetFinalTimelineInfo().time;
		startTime = Character.localCharacter.refs.stats.GetFirstTimelineInfo().time;
		maxTime -= startTime;
		if (maxTime == 0f)
		{
			maxTime = 1f;
		}
		float yieldTime = Mathf.Min(this.waitTime * Time.deltaTime / (float)longestCount, 0.2f);
		int num10;
		for (int i = 0; i < longestCount; i = num10 + 1)
		{
			for (int num9 = 0; num9 < timelineInfos.Count; num9++)
			{
				if (i < timelineInfos[num9].Count)
				{
					this.DrawPip(num9, timelineInfos[num9][i], maxTime, startTime, this.scouts[num9].color);
					if (!timelineInfos[num9][i].dead && !timelineInfos[num9][i].died)
					{
						this.scoutWindows[num9].UpdateAltitude(CharacterStats.UnitsToMeters(timelineInfos[num9][i].height));
					}
				}
			}
			yield return new WaitForSeconds(yieldTime * 0.33f);
			num10 = i;
		}
		for (int i = 0; i < timelineInfos.Count; i = num10 + 1)
		{
			Debug.Log(string.Format("Checking timeline info {0}, has infos: {1}", i, timelineInfos[i].Count));
			if (timelineInfos[i].Count > 0)
			{
				this.CheckPeak(i, timelineInfos[i][timelineInfos[i].Count - 1]);
				yield return new WaitForSeconds(0.25f);
			}
			num10 = i;
		}
		yield break;
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x00045488 File Offset: 0x00043688
	private List<BadgeData> GetBadgeUnlocks()
	{
		List<BadgeData> list = new List<BadgeData>();
		foreach (ACHIEVEMENTTYPE achievementtype in Singleton<AchievementManager>.Instance.runBasedValueData.achievementsEarnedThisRun)
		{
			BadgeData badgeData = GUIManager.instance.mainBadgeManager.GetBadgeData(achievementtype);
			if (badgeData != null)
			{
				list.Add(badgeData);
			}
		}
		return list;
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00045508 File Offset: 0x00043708
	private IEnumerator AscentRoutine(List<int> completedAscentsThisRun)
	{
		if (completedAscentsThisRun.Count > 0 && completedAscentsThisRun[0] == 0)
		{
			yield return this.AscentsUnlockRoutine();
		}
		int num;
		for (int i = 0; i < completedAscentsThisRun.Count; i = num + 1)
		{
			yield return new WaitForSeconds(0.5f);
			yield return this.PromotionUnlockRoutine(completedAscentsThisRun[i]);
			num = i;
		}
		yield return null;
		yield break;
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x0004551E File Offset: 0x0004371E
	private IEnumerator BadgeRoutine()
	{
		BadgeManager bm = base.GetComponent<BadgeManager>();
		bm.InheritData(GUIManager.instance.mainBadgeManager);
		List<BadgeData> badgeUnlocks = this.GetBadgeUnlocks();
		List<ACHIEVEMENTTYPE> achievementsEarnedThisRun = Singleton<AchievementManager>.Instance.runBasedValueData.achievementsEarnedThisRun;
		bool flag = false;
		bool unlockedCrown = false;
		for (int j = 0; j < achievementsEarnedThisRun.Count; j++)
		{
			if (achievementsEarnedThisRun[j] >= ACHIEVEMENTTYPE.TriedYourBestBadge && achievementsEarnedThisRun[j] <= ACHIEVEMENTTYPE.EnduranceBadge)
			{
				flag = true;
			}
		}
		if (flag && Singleton<AchievementManager>.Instance.AllBaseAchievementsUnlocked())
		{
			unlockedCrown = true;
		}
		int num;
		for (int i = 0; i < badgeUnlocks.Count; i = num + 1)
		{
			BadgeUI newBadge = Object.Instantiate<BadgeUI>(this.badge, this.badgeParentTF);
			newBadge.manager = bm;
			newBadge.Init(badgeUnlocks[i]);
			DOTweenModuleUI.DOFade(newBadge.canvasGroup, 1f, 0.2f);
			newBadge.transform.localScale = Vector3.one * 1.5f;
			TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOScale(newBadge.transform, 1f, 0.25f), 27);
			List<CustomizationOption> list = Singleton<Customization>.Instance.TryGetUnlockedCosmetics(badgeUnlocks[i]);
			foreach (CustomizationOption cosmetic in list)
			{
				yield return new WaitForSeconds(0.2f);
				yield return this.CosmeticUnlockRoutine(cosmetic);
				cosmetic = null;
			}
			List<CustomizationOption>.Enumerator enumerator = default(List<CustomizationOption>.Enumerator);
			if (i == 0)
			{
				UIInputHandler.SetSelectedObject(newBadge.gameObject);
				this.selectedBadge = true;
			}
			yield return new WaitForSeconds(0.2f);
			newBadge = null;
			num = i;
		}
		if (unlockedCrown)
		{
			yield return this.CosmeticUnlockRoutine(Singleton<Customization>.Instance.crownHat);
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
		yield break;
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x0004552D File Offset: 0x0004372D
	public void PopupNext()
	{
		this.inPopupView = false;
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x00045536 File Offset: 0x00043736
	private IEnumerator CosmeticUnlockRoutine(CustomizationOption cosmetic)
	{
		this.cosmeticUnlockObject.SetActive(true);
		string text = LocalizedText.GetText("NEWHAT", true);
		if (cosmetic.type == Customization.Type.Accessory || cosmetic.type == Customization.Type.Eyes)
		{
			text = LocalizedText.GetText("NEWLOOK", true);
		}
		if (cosmetic.type == Customization.Type.Fit)
		{
			text = LocalizedText.GetText("NEWFIT", true);
		}
		this.cosmeticUnlockTitle.text = text;
		this.cosmeticUnlockIcon.texture = cosmetic.texture;
		Shadow component = this.cosmeticUnlockIcon.GetComponent<Shadow>();
		if (component)
		{
			component.enabled = cosmetic.type == Customization.Type.Eyes;
		}
		this.cosmeticUnlockIcon.material = ((cosmetic.type == Customization.Type.Eyes) ? this.eyesMaterial : null);
		this.inPopupView = true;
		while (this.inPopupView)
		{
			UIInputHandler.SetSelectedObject(this.cosmeticNextButton.gameObject);
			yield return null;
		}
		this.cosmeticUnlockAnimator.Play("Done", 0, 0f);
		yield return new WaitForSeconds(0.25f);
		this.cosmeticUnlockObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x0004554C File Offset: 0x0004374C
	private IEnumerator AscentsUnlockRoutine()
	{
		this.ascentsUnlockObject.SetActive(true);
		this.inPopupView = true;
		while (this.inPopupView)
		{
			UIInputHandler.SetSelectedObject(this.ascentsNextButton.gameObject);
			yield return null;
		}
		this.ascentsUnlockAnimator.Play("Done", 0, 0f);
		yield return new WaitForSeconds(0.25f);
		this.ascentsUnlockObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x0004555B File Offset: 0x0004375B
	private IEnumerator PromotionUnlockRoutine(int ascent)
	{
		this.promotionUnlockObject.SetActive(true);
		string localizedReward = this.ascentData.ascents[ascent + 1].localizedReward;
		this.promotionUnlockTitle.text = localizedReward;
		if (ascent < this.ascentData.ascents.Count - 2)
		{
			this.promotionNextAscentUnlockText.text = LocalizedText.GetText("UNLOCKED", true).Replace("#", this.ascentData.ascents[ascent + 2].localizedTitle);
		}
		else
		{
			this.promotionNextAscentUnlockText.text = "";
		}
		this.promotionUnlockIcon.sprite = this.ascentData.ascents[ascent + 1].sashSprite;
		this.inPopupView = true;
		while (this.inPopupView)
		{
			UIInputHandler.SetSelectedObject(this.promotionNextButton.gameObject);
			yield return null;
		}
		this.promotionUnlockAnimator.Play("Done", 0, 0f);
		yield return new WaitForSeconds(0.25f);
		this.promotionUnlockObject.SetActive(false);
		if (ascent + 1 == 8)
		{
			yield return this.CosmeticUnlockRoutine(Singleton<Customization>.Instance.goatHat);
		}
		yield break;
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x00045571 File Offset: 0x00043771
	private float GetRandom(float nudge)
	{
		return Random.Range(-1f + nudge, 0f + nudge);
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x00045588 File Offset: 0x00043788
	public void DrawPip(int playerIndex, EndScreen.TimelineInfo heightTime, float maxTime, float startTime, Color color)
	{
		if (heightTime.dead)
		{
			return;
		}
		Image image = Object.Instantiate<Image>(heightTime.revived ? this.revivedPip : (heightTime.justPassedOut ? this.passedOutPip : (heightTime.died ? this.deadPip : this.pip)), this.scoutLines[playerIndex]);
		image.color = color;
		image.transform.GetChild(0).GetComponent<Image>().color = image.color;
		float num = CharacterStats.peakHeightInUnits;
		if (this.debug)
		{
			num = 1f;
		}
		image.transform.localPosition = new Vector3(this.timelinePanel.sizeDelta.x * Mathf.Clamp01((heightTime.time - startTime) / maxTime), this.timelinePanel.sizeDelta.y * heightTime.height / num, 0f);
		image.transform.localPosition += Vector3.up * (float)playerIndex * 2f;
		this.scouts[playerIndex].transform.localPosition = image.transform.localPosition;
		if (this.oldPip[playerIndex])
		{
			image.transform.right = this.oldPip[playerIndex].transform.position - image.transform.position;
			image.rectTransform.sizeDelta = new Vector2(Vector3.Distance(image.transform.position, this.oldPip[playerIndex].transform.position) / this.timelinePanel.lossyScale.x, 1.5f);
		}
		if (heightTime.died)
		{
			this.scouts[playerIndex].gameObject.SetActive(false);
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
		}
		if (heightTime.justPassedOut)
		{
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
		}
		else if (heightTime.passedOut)
		{
			image.transform.GetChild(0).GetComponent<Image>().material = this.passedOutMaterial;
		}
		if (heightTime.revived)
		{
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
			image.transform.GetChild(0).gameObject.SetActive(false);
			this.scouts[playerIndex].gameObject.SetActive(true);
		}
		this.oldPip[playerIndex] = image;
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x00045868 File Offset: 0x00043A68
	public void CheckPeak(int playerIndex, EndScreen.TimelineInfo timelineInfo)
	{
		if (playerIndex < this.scouts.Length && timelineInfo.time >= 0.99f && timelineInfo.height >= 1f && !this.scoutsAtPeak[playerIndex].gameObject.activeSelf && !timelineInfo.dead && timelineInfo.won)
		{
			this.scouts[playerIndex].gameObject.SetActive(false);
			this.scoutsAtPeak[playerIndex].gameObject.SetActive(true);
			this.scoutsAtPeak[playerIndex].transform.SetSiblingIndex(1);
			this.scoutsAtPeak[playerIndex].rectTransform.sizeDelta = Vector3.zero;
			TweenSettingsExtensions.SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(DOTweenModuleUI.DOSizeDelta(this.scoutsAtPeak[playerIndex].rectTransform, Vector3.one * 15f, 0.25f, false), 27);
		}
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x0004595C File Offset: 0x00043B5C
	public void ReturnToAirport()
	{
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 3f) });
	}

	// Token: 0x04000BEE RID: 3054
	public static EndScreen instance;

	// Token: 0x04000BEF RID: 3055
	public CanvasGroup canvasGroup;

	// Token: 0x04000BF0 RID: 3056
	public AscentData ascentData;

	// Token: 0x04000BF1 RID: 3057
	public bool debug;

	// Token: 0x04000BF2 RID: 3058
	public TMP_Text endTime;

	// Token: 0x04000BF3 RID: 3059
	public EndScreenScoutWindow[] scoutWindows;

	// Token: 0x04000BF4 RID: 3060
	public Color[] debugColors;

	// Token: 0x04000BF5 RID: 3061
	public BadgeData[] debugBadgeUnlocks;

	// Token: 0x04000BF6 RID: 3062
	public BadgeUI badge;

	// Token: 0x04000BF7 RID: 3063
	public Transform badgeParentTF;

	// Token: 0x04000BF8 RID: 3064
	public Transform[] scoutLines;

	// Token: 0x04000BF9 RID: 3065
	public Image[] scouts;

	// Token: 0x04000BFA RID: 3066
	public Image[] scoutsAtPeak;

	// Token: 0x04000BFB RID: 3067
	public int pipCount = 100;

	// Token: 0x04000BFC RID: 3068
	public float waitTime = 5f;

	// Token: 0x04000BFD RID: 3069
	public RectTransform timelinePanel;

	// Token: 0x04000BFE RID: 3070
	public Image pip;

	// Token: 0x04000BFF RID: 3071
	public Image deadPip;

	// Token: 0x04000C00 RID: 3072
	public Image passedOutPip;

	// Token: 0x04000C01 RID: 3073
	public Image revivedPip;

	// Token: 0x04000C02 RID: 3074
	public Material passedOutMaterial;

	// Token: 0x04000C03 RID: 3075
	public GameObject peakBanner;

	// Token: 0x04000C04 RID: 3076
	public GameObject deadBanner;

	// Token: 0x04000C05 RID: 3077
	public GameObject yourFriendsWonBanner;

	// Token: 0x04000C06 RID: 3078
	public GameObject buttons;

	// Token: 0x04000C07 RID: 3079
	public WaitingForPlayersUI WaitingForPlayersUI;

	// Token: 0x04000C08 RID: 3080
	public Button nextButton;

	// Token: 0x04000C09 RID: 3081
	public Button returnToAirportButton;

	// Token: 0x04000C0A RID: 3082
	public Material eyesMaterial;

	// Token: 0x04000C0B RID: 3083
	private bool selectedBadge;

	// Token: 0x04000C0C RID: 3084
	public GameObject cosmeticUnlockObject;

	// Token: 0x04000C0D RID: 3085
	public Animator cosmeticUnlockAnimator;

	// Token: 0x04000C0E RID: 3086
	public TMP_Text cosmeticUnlockTitle;

	// Token: 0x04000C0F RID: 3087
	public Button cosmeticNextButton;

	// Token: 0x04000C10 RID: 3088
	public RawImage cosmeticUnlockIcon;

	// Token: 0x04000C11 RID: 3089
	public GameObject ascentsUnlockObject;

	// Token: 0x04000C12 RID: 3090
	public Animator ascentsUnlockAnimator;

	// Token: 0x04000C13 RID: 3091
	public Button ascentsNextButton;

	// Token: 0x04000C14 RID: 3092
	public GameObject promotionUnlockObject;

	// Token: 0x04000C15 RID: 3093
	public Animator promotionUnlockAnimator;

	// Token: 0x04000C16 RID: 3094
	public TMP_Text promotionUnlockTitle;

	// Token: 0x04000C17 RID: 3095
	public TMP_Text promotionNextAscentUnlockText;

	// Token: 0x04000C18 RID: 3096
	public Button promotionNextButton;

	// Token: 0x04000C19 RID: 3097
	public Image promotionUnlockIcon;

	// Token: 0x04000C1A RID: 3098
	private bool inPopupView;

	// Token: 0x04000C1B RID: 3099
	private Image[] oldPip = new Image[4];

	// Token: 0x020004AA RID: 1194
	private enum TimelineNote
	{
		// Token: 0x040019FE RID: 6654
		None,
		// Token: 0x040019FF RID: 6655
		PassedOut,
		// Token: 0x04001A00 RID: 6656
		Dead,
		// Token: 0x04001A01 RID: 6657
		JustPassedOut,
		// Token: 0x04001A02 RID: 6658
		Died,
		// Token: 0x04001A03 RID: 6659
		Revived,
		// Token: 0x04001A04 RID: 6660
		Won
	}

	// Token: 0x020004AB RID: 1195
	public struct TimelineInfo
	{
		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06001BEF RID: 7151 RVA: 0x00083912 File Offset: 0x00081B12
		// (set) Token: 0x06001BF0 RID: 7152 RVA: 0x0008391D File Offset: 0x00081B1D
		public bool died
		{
			get
			{
				return this._note == EndScreen.TimelineNote.Died;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.Died);
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06001BF1 RID: 7153 RVA: 0x00083927 File Offset: 0x00081B27
		// (set) Token: 0x06001BF2 RID: 7154 RVA: 0x00083932 File Offset: 0x00081B32
		public bool dead
		{
			get
			{
				return this._note == EndScreen.TimelineNote.Dead;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.Dead);
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06001BF3 RID: 7155 RVA: 0x0008393C File Offset: 0x00081B3C
		// (set) Token: 0x06001BF4 RID: 7156 RVA: 0x00083947 File Offset: 0x00081B47
		public bool revived
		{
			get
			{
				return this._note == EndScreen.TimelineNote.Revived;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.Revived);
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06001BF5 RID: 7157 RVA: 0x00083951 File Offset: 0x00081B51
		// (set) Token: 0x06001BF6 RID: 7158 RVA: 0x0008395C File Offset: 0x00081B5C
		public bool justPassedOut
		{
			get
			{
				return this._note == EndScreen.TimelineNote.JustPassedOut;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.JustPassedOut);
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06001BF7 RID: 7159 RVA: 0x00083966 File Offset: 0x00081B66
		// (set) Token: 0x06001BF8 RID: 7160 RVA: 0x00083971 File Offset: 0x00081B71
		public bool passedOut
		{
			get
			{
				return this._note == EndScreen.TimelineNote.PassedOut;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.PassedOut);
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06001BF9 RID: 7161 RVA: 0x0008397B File Offset: 0x00081B7B
		// (set) Token: 0x06001BFA RID: 7162 RVA: 0x00083986 File Offset: 0x00081B86
		public bool won
		{
			get
			{
				return this._note == EndScreen.TimelineNote.Won;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.Won);
			}
		}

		// Token: 0x06001BFB RID: 7163 RVA: 0x00083990 File Offset: 0x00081B90
		private void SetNote(bool value, EndScreen.TimelineNote noteType)
		{
			if (value)
			{
				if (noteType != EndScreen.TimelineNote.None)
				{
					Debug.LogWarning(string.Format("Setting note to {0} which will override previous type {1}", noteType, this._note));
				}
				this._note = noteType;
				return;
			}
			Debug.LogWarning(string.Format("WHOA! When do we ever set a timeline event to FALSE? Something funky going on with {0}", noteType));
			if (this._note == noteType)
			{
				this._note = EndScreen.TimelineNote.None;
				return;
			}
			Debug.LogError(string.Format("Can't clear note {0} because current note is different {1}", noteType, this._note));
		}

		// Token: 0x04001A05 RID: 6661
		private EndScreen.TimelineNote _note;

		// Token: 0x04001A06 RID: 6662
		public float height;

		// Token: 0x04001A07 RID: 6663
		public float time;
	}
}
