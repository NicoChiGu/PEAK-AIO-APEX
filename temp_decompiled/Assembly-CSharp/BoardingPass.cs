using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x02000219 RID: 537
public class BoardingPass : MenuWindow
{
	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000FCA RID: 4042 RVA: 0x0004E925 File Offset: 0x0004CB25
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000FCB RID: 4043 RVA: 0x0004E928 File Offset: 0x0004CB28
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000FCC RID: 4044 RVA: 0x0004E92B File Offset: 0x0004CB2B
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000FCD RID: 4045 RVA: 0x0004E92E File Offset: 0x0004CB2E
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000FCE RID: 4046 RVA: 0x0004E931 File Offset: 0x0004CB31
	public override bool autoHideOnClose
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000FCF RID: 4047 RVA: 0x0004E934 File Offset: 0x0004CB34
	// (set) Token: 0x06000FD0 RID: 4048 RVA: 0x0004E93C File Offset: 0x0004CB3C
	public int ascentIndex
	{
		get
		{
			return this._ascentIndex;
		}
		set
		{
			this._ascentIndex = value;
		}
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000FD1 RID: 4049 RVA: 0x0004E945 File Offset: 0x0004CB45
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			if (this.decrementAscentButton.gameObject.activeInHierarchy)
			{
				return this.decrementAscentButton;
			}
			if (this.incrementAscentButton.gameObject.activeInHierarchy)
			{
				return this.incrementAscentButton;
			}
			return this.startGameButton;
		}
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x0004E980 File Offset: 0x0004CB80
	protected override void Initialize()
	{
		this.incrementAscentButton.onClick.AddListener(new UnityAction(this.IncrementAscent));
		this.decrementAscentButton.onClick.AddListener(new UnityAction(this.DecrementAscent));
		this.startGameButton.onClick.AddListener(new UnityAction(this.StartGame));
		this.closeButton.onClick.AddListener(new UnityAction(base.Close));
		this.UpdateAscent();
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x0004EA03 File Offset: 0x0004CC03
	private void InitMaxAscent()
	{
		this.maxUnlockedAscent = 0;
		Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out this.maxUnlockedAscent);
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x0004EA20 File Offset: 0x0004CC20
	protected override void OnOpen()
	{
		this.playerName.text = Character.localCharacter.characterName;
		List<Character> allCharacters = Character.AllCharacters;
		for (int i = 0; i < this.players.Length; i++)
		{
			if (allCharacters.Count > i)
			{
				this.players[i].gameObject.SetActive(true);
				this.players[i].color = allCharacters[i].refs.customization.PlayerColor;
			}
			else
			{
				this.players[i].gameObject.SetActive(false);
			}
		}
		this.canvasGroup.alpha = 0f;
		DOTweenModuleUI.DOFade(this.canvasGroup, 1f, 0.5f);
		this.UpdateAscent();
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x0004EADB File Offset: 0x0004CCDB
	protected override void OnClose()
	{
		DOTweenModuleUI.DOFade(this.canvasGroup, 0f, 0.2f);
		base.Invoke("HideIt", 0.2f);
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x0004EB03 File Offset: 0x0004CD03
	private void HideIt()
	{
		base.Hide();
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x0004EB0C File Offset: 0x0004CD0C
	private void UpdateAscent()
	{
		this.maxUnlockedAscent = Singleton<AchievementManager>.Instance.GetMaxAscent();
		int num = Mathf.Min(this.maxAscent, this.maxUnlockedAscent);
		this.incrementAscentButton.interactable = this.ascentIndex < num;
		this.decrementAscentButton.interactable = this.ascentIndex > -1;
		this.ascentTitle.text = this.ascentData.ascents[this.ascentIndex + 1].localizedTitle;
		this.ascentDesc.text = this.ascentData.ascents[this.ascentIndex + 1].localizedDescription;
		if (this.ascentIndex >= 2)
		{
			TMP_Text tmp_Text = this.ascentDesc;
			tmp_Text.text = tmp_Text.text + "\n\n<alpha=#CC><size=70%>" + LocalizedText.GetText("ANDALLOTHER", true);
		}
		if (this.ascentIndex == this.maxUnlockedAscent && this.ascentIndex > -1 && this.ascentIndex < 8)
		{
			this.reward.gameObject.SetActive(true);
			this.rewardText.text = this.ascentData.ascents[this.ascentIndex + 1].localizedReward;
			this.rewardImage.color = this.ascentData.ascents[this.ascentIndex + 1].color;
			return;
		}
		this.reward.gameObject.SetActive(false);
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x0004EC78 File Offset: 0x0004CE78
	public void IncrementAscent()
	{
		int ascentIndex = this.ascentIndex;
		this.ascentIndex = ascentIndex + 1;
		this.UpdateAscent();
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x0004EC9C File Offset: 0x0004CE9C
	public void DecrementAscent()
	{
		int ascentIndex = this.ascentIndex;
		this.ascentIndex = ascentIndex - 1;
		this.UpdateAscent();
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x0004ECBF File Offset: 0x0004CEBF
	public void StartGame()
	{
		this.kiosk.StartGame(this.ascentIndex);
	}

	// Token: 0x04000E25 RID: 3621
	public TMP_Text playerName;

	// Token: 0x04000E26 RID: 3622
	public TMP_Text ascentTitle;

	// Token: 0x04000E27 RID: 3623
	public TMP_Text ascentDesc;

	// Token: 0x04000E28 RID: 3624
	public GameObject reward;

	// Token: 0x04000E29 RID: 3625
	public Image rewardImage;

	// Token: 0x04000E2A RID: 3626
	public TextMeshProUGUI rewardText;

	// Token: 0x04000E2B RID: 3627
	public Image[] players;

	// Token: 0x04000E2C RID: 3628
	private int _ascentIndex;

	// Token: 0x04000E2D RID: 3629
	private int maxAscent = 7;

	// Token: 0x04000E2E RID: 3630
	private int maxUnlockedAscent;

	// Token: 0x04000E2F RID: 3631
	public AirportCheckInKiosk kiosk;

	// Token: 0x04000E30 RID: 3632
	public Button incrementAscentButton;

	// Token: 0x04000E31 RID: 3633
	public Button decrementAscentButton;

	// Token: 0x04000E32 RID: 3634
	public Button startGameButton;

	// Token: 0x04000E33 RID: 3635
	public Button closeButton;

	// Token: 0x04000E34 RID: 3636
	public AscentData ascentData;

	// Token: 0x04000E35 RID: 3637
	public CanvasGroup canvasGroup;
}
