using System;
using System.Collections;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000153 RID: 339
public class PassportManager : MenuWindow
{
	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06000ADB RID: 2779 RVA: 0x00039F55 File Offset: 0x00038155
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x06000ADC RID: 2780 RVA: 0x00039F58 File Offset: 0x00038158
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06000ADD RID: 2781 RVA: 0x00039F5B File Offset: 0x0003815B
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return this.buttons[0].button;
		}
	}

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x06000ADE RID: 2782 RVA: 0x00039F6A File Offset: 0x0003816A
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x06000ADF RID: 2783 RVA: 0x00039F6D File Offset: 0x0003816D
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x06000AE0 RID: 2784 RVA: 0x00039F70 File Offset: 0x00038170
	public override bool autoHideOnClose
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x00039F73 File Offset: 0x00038173
	public void Awake()
	{
		PassportManager.instance = this;
		this.uiObject.SetActive(false);
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x00039F87 File Offset: 0x00038187
	[ConsoleCommand]
	public static void TestAllCosmetics()
	{
		if (PassportManager.instance != null)
		{
			PassportManager.instance.testUnlockAll = true;
		}
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x00039FA4 File Offset: 0x000381A4
	public static string GeneratePassportNumber(string name)
	{
		string text = PassportManager.GenerateCountryCode(name);
		int num = PassportManager.GenerateNumericCode(name, 9);
		return string.Format("{0}{1:D7}", text, num);
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x00039FD4 File Offset: 0x000381D4
	private static string GenerateCountryCode(string name)
	{
		name = name.ToUpper().Replace(" ", "");
		if (name.Length < 2)
		{
			name += "XX";
		}
		return string.Format("{0}", name.get_Chars(0));
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0003A024 File Offset: 0x00038224
	private static int GenerateNumericCode(string input, int length)
	{
		return Mathf.Abs(input.GetHashCode()) % (int)Mathf.Pow(10f, (float)length);
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0003A03F File Offset: 0x0003823F
	public void ToggleOpen()
	{
		if (!this.closing)
		{
			if (!base.isOpen)
			{
				this.Open();
				this.uiObject.SetActive(true);
				this.OpenTab(this.activeType);
				return;
			}
			base.Close();
		}
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0003A078 File Offset: 0x00038278
	protected override void Initialize()
	{
		string characterName = Character.localCharacter.characterName;
		PassportManager.passportNumberString = PassportManager.GeneratePassportNumber(characterName);
		this.nameText.text = characterName;
		this.passportNumberText.text = PassportManager.passportNumberString;
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0003A0B7 File Offset: 0x000382B7
	protected override void OnClose()
	{
		base.StartCoroutine(this.CloseRoutine());
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0003A0C6 File Offset: 0x000382C6
	private IEnumerator CloseRoutine()
	{
		this.closing = true;
		this.anim.Play("Close");
		this.CameraIn();
		yield return new WaitForSeconds(0.5f);
		this.uiObject.SetActive(false);
		this.closing = false;
		yield break;
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0003A0D8 File Offset: 0x000382D8
	public void OpenTab(Customization.Type type)
	{
		this.activeType = type;
		int num = 0;
		for (int i = 0; i < this.tabs.Length; i++)
		{
			if (this.tabs[i].type == type)
			{
				num = i;
			}
			else
			{
				this.tabs[i].Close();
			}
		}
		this.tabs[num].Open();
		if (num == 4 || num == 6)
		{
			this.CameraOut();
		}
		else
		{
			this.CameraIn();
		}
		this.SetButtons(true);
		this.dummy.UpdateDummy();
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0003A158 File Offset: 0x00038358
	private void CameraIn()
	{
		ShortcutExtensions.DOOrthoSize(this.dummyCamera, 0.6f, 0.2f);
		ShortcutExtensions.DOLocalMove(this.dummyCamera.transform, new Vector3(0f, 1.65f, 1f), 0.2f, false);
		if (this.camIn)
		{
			this.camIn.Play(default(Vector3));
		}
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0003A1C8 File Offset: 0x000383C8
	private void CameraOut()
	{
		ShortcutExtensions.DOOrthoSize(this.dummyCamera, 1.3f, 0.2f);
		ShortcutExtensions.DOLocalMove(this.dummyCamera.transform, new Vector3(0f, 1.05f, 1f), 0.2f, false);
		if (this.camOut)
		{
			this.camOut.Play(default(Vector3));
		}
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0003A238 File Offset: 0x00038438
	public void SetButtons(bool fromTabChange = false)
	{
		int num = this.SetActiveButton(fromTabChange);
		if (fromTabChange)
		{
			this.page = num / this.buttonsPerPage;
		}
		CustomizationOption[] list = Singleton<Customization>.Instance.GetList(this.activeType);
		this.activeTabOptionCount = list.Length;
		this.pageNumNext.text = (this.page + 2).ToString();
		this.pageNumPrev.text = this.page.ToString();
		this.buttonNext.SetActive(this.activeTabOptionCount > (this.page + 1) * this.buttonsPerPage);
		this.buttonPrev.SetActive(this.activeTabOptionCount > this.buttonsPerPage && this.page > 0);
		for (int i = 0; i < this.buttonsPerPage; i++)
		{
			int num2 = i;
			int num3 = i + this.page * this.buttonsPerPage;
			if (num3 < list.Length)
			{
				CustomizationOption customizationOption = list[num3];
				this.buttons[num2].SetButton(customizationOption, num3);
			}
			else
			{
				this.buttons[num2].SetButton(null, -1);
			}
		}
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0003A348 File Offset: 0x00038548
	private int SetActiveButton(bool fromTabChange = false)
	{
		PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer);
		int num = playerData.customizationData.currentSkin;
		if (this.activeType == Customization.Type.Accessory)
		{
			num = playerData.customizationData.currentAccessory;
		}
		else if (this.activeType == Customization.Type.Eyes)
		{
			num = playerData.customizationData.currentEyes;
		}
		else if (this.activeType == Customization.Type.Mouth)
		{
			num = playerData.customizationData.currentMouth;
		}
		else if (this.activeType == Customization.Type.Fit)
		{
			num = playerData.customizationData.currentOutfit;
		}
		else if (this.activeType == Customization.Type.Hat)
		{
			num = playerData.customizationData.currentHat;
		}
		else if (this.activeType == Customization.Type.Sash)
		{
			num = playerData.customizationData.currentSash;
		}
		int num2 = this.page;
		if (fromTabChange)
		{
			num2 = num / this.buttonsPerPage;
		}
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].border.color = ((num == i + num2 * this.buttonsPerPage) ? this.activeBorderColor : this.inactiveBorderColor);
		}
		return num;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0003A454 File Offset: 0x00038654
	public void SetOption(CustomizationOption option, int index)
	{
		if (option.type == Customization.Type.Skin)
		{
			CharacterCustomization.SetCharacterSkinColor(index);
		}
		else if (option.type == Customization.Type.Eyes)
		{
			CharacterCustomization.SetCharacterEyes(index);
		}
		else if (option.type == Customization.Type.Mouth)
		{
			CharacterCustomization.SetCharacterMouth(index);
		}
		else if (option.type == Customization.Type.Accessory)
		{
			CharacterCustomization.SetCharacterAccessory(index);
		}
		else if (option.type == Customization.Type.Fit)
		{
			CharacterCustomization.SetCharacterOutfit(index);
		}
		else if (option.type == Customization.Type.Hat)
		{
			CharacterCustomization.SetCharacterHat(index);
		}
		else if (option.type == Customization.Type.Sash)
		{
			CharacterCustomization.SetCharacterSash(index);
		}
		this.SetActiveButton(false);
		this.dummy.UpdateDummy();
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0003A4F0 File Offset: 0x000386F0
	public void PageNext()
	{
		this.page++;
		if (this.page > (this.activeTabOptionCount - 1) / this.buttonsPerPage)
		{
			this.page = (this.activeTabOptionCount - 1) / this.buttonsPerPage;
		}
		this.SetButtons(false);
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0003A53E File Offset: 0x0003873E
	public void PagePrev()
	{
		this.page--;
		if (this.page < 0)
		{
			this.page = 0;
		}
		this.SetButtons(false);
	}

	// Token: 0x04000A19 RID: 2585
	public static PassportManager instance;

	// Token: 0x04000A1A RID: 2586
	public Animator anim;

	// Token: 0x04000A1B RID: 2587
	public GameObject uiObject;

	// Token: 0x04000A1C RID: 2588
	public PassportTab[] tabs;

	// Token: 0x04000A1D RID: 2589
	public Customization.Type activeType;

	// Token: 0x04000A1E RID: 2590
	public PassportButton[] buttons;

	// Token: 0x04000A1F RID: 2591
	public PlayerCustomizationDummy dummy;

	// Token: 0x04000A20 RID: 2592
	public Camera dummyCamera;

	// Token: 0x04000A21 RID: 2593
	public TextMeshProUGUI nameText;

	// Token: 0x04000A22 RID: 2594
	public TextMeshProUGUI passportNumberText;

	// Token: 0x04000A23 RID: 2595
	private static string passportNumberString;

	// Token: 0x04000A24 RID: 2596
	public Color inactiveBorderColor;

	// Token: 0x04000A25 RID: 2597
	public Color activeBorderColor;

	// Token: 0x04000A26 RID: 2598
	public bool testUnlockAll;

	// Token: 0x04000A27 RID: 2599
	public SFX_Instance camIn;

	// Token: 0x04000A28 RID: 2600
	public SFX_Instance camOut;

	// Token: 0x04000A29 RID: 2601
	public GameObject buttonNext;

	// Token: 0x04000A2A RID: 2602
	public GameObject buttonPrev;

	// Token: 0x04000A2B RID: 2603
	public TextMeshProUGUI pageNumNext;

	// Token: 0x04000A2C RID: 2604
	public TextMeshProUGUI pageNumPrev;

	// Token: 0x04000A2D RID: 2605
	private bool closing;

	// Token: 0x04000A2E RID: 2606
	private int activeTabOptionCount;

	// Token: 0x04000A2F RID: 2607
	private int page;

	// Token: 0x04000A30 RID: 2608
	private int buttonsPerPage = 28;
}
