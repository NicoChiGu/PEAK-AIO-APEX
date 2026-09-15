using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Photon.Pun;
using Photon.Realtime;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000072 RID: 114
[ConsoleClassCustomizer("Customization")]
public class CharacterCustomization : MonoBehaviour
{
	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000508 RID: 1288 RVA: 0x0001D9B0 File Offset: 0x0001BBB0
	public Color PlayerColor
	{
		get
		{
			CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(this._character.photonView.Owner);
			return Singleton<Customization>.Instance.skins[customizationData.currentSkin].color;
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000509 RID: 1289 RVA: 0x0001D9EC File Offset: 0x0001BBEC
	public Vector3 PlayerColorAsVector
	{
		get
		{
			Color playerColor = this.PlayerColor;
			return new Vector3(playerColor.r, playerColor.g, playerColor.b);
		}
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x0001DA17 File Offset: 0x0001BC17
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this._character = base.GetComponent<Character>();
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x0001DA34 File Offset: 0x0001BC34
	public void Start()
	{
		if (this.view.IsMine && !this._character.isBot)
		{
			this.SetRandomIdle();
			this.HideChicken();
			InRoomState inRoomState = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState as InRoomState;
			if (inRoomState != null && !inRoomState.hasLoadedCustomization)
			{
				inRoomState.hasLoadedCustomization = true;
				base.StartCoroutine(this.GetCosmeticsFromSteamRoutine());
				if (this._character.IsLocal)
				{
					this.refs.mainRenderer.updateWhenOffscreen = true;
				}
			}
		}
		Character character = this._character;
		character.reviveAction = (Action)Delegate.Combine(character.reviveAction, new Action(this.OnRevive));
		Character character2 = this._character;
		character2.UnPassOutAction = (Action)Delegate.Combine(character2.UnPassOutAction, new Action(this.OnRevive));
		Player player = ((this.overridePhotonPlayer != null) ? this.overridePhotonPlayer : this._character.photonView.Owner);
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		service.SubscribeToPlayerDataChange(player, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
		this.OnPlayerDataChange(service.GetPlayerData(player));
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0001DB4E File Offset: 0x0001BD4E
	private IEnumerator GetCosmeticsFromSteamRoutine()
	{
		while (!Singleton<AchievementManager>.Instance.gotStats)
		{
			yield return null;
		}
		this.TryGetCosmeticsFromSteam();
		yield break;
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x0001DB60 File Offset: 0x0001BD60
	private void TryGetCosmeticsFromSteam()
	{
		if (this.ignorePlayerCosmetics)
		{
			return;
		}
		if (Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "RandomCosmetics"))
		{
			this.RandomizeCosmetics();
			this.SetRandomSkinColor();
			return;
		}
		int num;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.LoadedCosmeticsPreviously, out num))
		{
			if (num > 0)
			{
				int num2;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Skin, out num2) && num2 != -1)
				{
					CharacterCustomization.SetCharacterSkinColor(num2);
				}
				else
				{
					this.SetRandomSkinColor();
				}
				int num3;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Eyes, out num3) && num3 != -1)
				{
					CharacterCustomization.SetCharacterEyes(num3);
				}
				else
				{
					this.SetRandomEyes();
				}
				int num4;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Mouth, out num4) && num4 != -1)
				{
					CharacterCustomization.SetCharacterMouth(num4);
				}
				else
				{
					this.SetRandomMouth();
				}
				int num5;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Accessory, out num5) && num4 != -1)
				{
					CharacterCustomization.SetCharacterAccessory(num5);
				}
				else
				{
					this.SetRandomAccessory();
				}
				int num6;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Outfit, out num6) && num6 != -1)
				{
					CharacterCustomization.SetCharacterOutfit(num6);
				}
				else
				{
					this.SetRandomOutfit();
				}
				int num7;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Hat, out num7) && num7 != -1)
				{
					CharacterCustomization.SetCharacterHat(num7);
				}
				else
				{
					this.SetRandomHat();
				}
				int num8;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Sash, out num8) && num8 != -1)
				{
					CharacterCustomization.SetCharacterSash(num8);
				}
				else
				{
					CharacterCustomization.SetCharacterSash(0);
				}
			}
			else
			{
				this.RandomizeCosmetics();
			}
			Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.LoadedCosmeticsPreviously, 1);
			return;
		}
		this.SetRandomSkinColor();
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0001DCBB File Offset: 0x0001BEBB
	[ConsoleCommand]
	public static void Randomize()
	{
		Character.localCharacter.refs.customization.RandomizeCosmetics();
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001DCD1 File Offset: 0x0001BED1
	public void RandomizeCosmetics()
	{
		this.SetRandomSkinColor();
		this.SetRandomEyes();
		this.SetRandomMouth();
		this.SetRandomAccessory();
		this.SetRandomOutfit();
		this.SetRandomHat();
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001DCF8 File Offset: 0x0001BEF8
	private void OnDestroy()
	{
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		if (service != null)
		{
			service.UnsubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
		}
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0001DD30 File Offset: 0x0001BF30
	public void SetCustomizationForRef(CustomizationRefs refs)
	{
		CustomizationRefs customizationRefs = this.refs;
		this.refs = refs;
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		service.SubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
		this.OnPlayerDataChange(service.GetPlayerData(this.view.Owner));
		this.refs = customizationRefs;
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x0001DD94 File Offset: 0x0001BF94
	internal void OnPlayerDataChange(PersistentPlayerData playerData)
	{
		if (this.refs.PlayerRenderers[0] == null)
		{
			return;
		}
		if (this._character.data.isScoutmaster)
		{
			return;
		}
		if (this.ignorePlayerCosmetics)
		{
			return;
		}
		Debug.Log("On Player Data Change");
		int skinIndex = CharacterCustomization.GetSkinIndex(playerData);
		if (this.useDebugColor)
		{
			skinIndex = this.debugColorIndex;
		}
		Renderer[] array = this.refs.PlayerRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetColor(CharacterCustomization.SkinColor, Singleton<Customization>.Instance.skins[skinIndex].color);
		}
		array = this.refs.EyeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetColor(CharacterCustomization.SkinColor, Singleton<Customization>.Instance.skins[skinIndex].color);
		}
		int fitIndex = CharacterCustomization.GetFitIndex(playerData);
		this.refs.mainRenderer.sharedMesh = Singleton<Customization>.Instance.fits[fitIndex].fitMesh;
		this.refs.mainRendererShadow.sharedMesh = Singleton<Customization>.Instance.fits[fitIndex].fitMesh;
		List<Material> list = new List<Material>();
		list.Add(this.refs.mainRenderer.materials[0]);
		list.Add(Singleton<Customization>.Instance.fits[fitIndex].fitMaterial);
		list.Add(Singleton<Customization>.Instance.fits[fitIndex].fitMaterialShoes);
		this.refs.mainRenderer.SetSharedMaterials(list);
		if (Singleton<Customization>.Instance.fits[fitIndex].noPants)
		{
			this.refs.skirt.gameObject.SetActive(false);
			this.refs.shortsShadow.gameObject.SetActive(false);
			this.refs.skirtShadow.gameObject.SetActive(false);
			this.refs.shorts.gameObject.SetActive(false);
		}
		else if (Singleton<Customization>.Instance.fits[fitIndex].isSkirt)
		{
			this.refs.skirt.gameObject.SetActive(true);
			this.refs.shortsShadow.gameObject.SetActive(false);
			this.refs.skirtShadow.gameObject.SetActive(true);
			this.refs.shorts.gameObject.SetActive(false);
			this.refs.skirt.sharedMaterial = Singleton<Customization>.Instance.fits[fitIndex].fitPantsMaterial;
		}
		else
		{
			this.refs.skirt.gameObject.SetActive(false);
			this.refs.shortsShadow.gameObject.SetActive(true);
			this.refs.skirtShadow.gameObject.SetActive(false);
			this.refs.shorts.gameObject.SetActive(true);
			this.refs.shorts.sharedMaterial = Singleton<Customization>.Instance.fits[fitIndex].fitPantsMaterial;
		}
		this.refs.playerHats[0].material = Singleton<Customization>.Instance.fits[fitIndex].fitHatMaterial;
		this.refs.playerHats[1].material = Singleton<Customization>.Instance.fits[fitIndex].fitHatMaterial;
		int eyesIndex = CharacterCustomization.GetEyesIndex(playerData);
		this.CurrentEyeTexture = Singleton<Customization>.Instance.eyes[eyesIndex].texture;
		array = this.refs.EyeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetTexture(CharacterCustomization.MainTex, this.CurrentEyeTexture);
		}
		int mouthIndex = CharacterCustomization.GetMouthIndex(playerData);
		this.refs.mouthRenderer.material.SetTexture(CharacterCustomization.MainTex, Singleton<Customization>.Instance.mouths[mouthIndex].texture);
		int accessoryIndex = CharacterCustomization.GetAccessoryIndex(playerData);
		this.refs.accessoryRenderer.material.SetTexture(CharacterCustomization.MainTex, Singleton<Customization>.Instance.accessories[accessoryIndex].texture);
		this.refs.accessoryRenderer.material.renderQueue = (Singleton<Customization>.Instance.accessories[accessoryIndex].drawUnderEye ? 3007 : 3009);
		this.refs.accessoryRenderer.enabled = !Singleton<Customization>.Instance.accessories[accessoryIndex].isThirdEye;
		this.refs.thirdEye.gameObject.SetActive(Singleton<Customization>.Instance.accessories[accessoryIndex].isThirdEye);
		int num = playerData.customizationData.currentHat;
		if (Singleton<Customization>.Instance.fits[fitIndex].overrideHat)
		{
			num = Singleton<Customization>.Instance.fits[fitIndex].overrideHatIndex;
		}
		MeshFilter meshFilter = null;
		for (int j = 0; j < this.refs.playerHats.Length; j++)
		{
			this.refs.playerHats[j].gameObject.SetActive(num == j);
			if (num == j)
			{
				meshFilter = this.refs.playerHats[j].GetComponent<MeshFilter>();
			}
		}
		if (!meshFilter)
		{
			meshFilter = this.refs.playerHats[0].GetComponent<MeshFilter>();
		}
		this.refs.hatShadowMeshFilter.sharedMesh = meshFilter.sharedMesh;
		this.refs.hatShadowMeshFilter.transform.SetPositionAndRotation(meshFilter.transform.position, meshFilter.transform.rotation);
		this.refs.hatShadowMeshFilter.transform.localScale = meshFilter.transform.localScale;
		List<Material> list2 = new List<Material>();
		list2.Add(this.refs.sashRenderer.materials[0]);
		int sashIndex = CharacterCustomization.GetSashIndex(playerData);
		list2.Add(this.refs.sashAscentMaterials[sashIndex]);
		this.refs.sashRenderer.SetMaterials(list2);
		if (this._character && this._character.refs.hideTheBody)
		{
			this._character.refs.hideTheBody.Refresh();
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0001E3B8 File Offset: 0x0001C5B8
	public static int GetEyesIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentEyes;
		if (num >= Singleton<Customization>.Instance.eyes.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0001E3E4 File Offset: 0x0001C5E4
	public static int GetSkinIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentSkin;
		if (num >= Singleton<Customization>.Instance.skins.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x0001E410 File Offset: 0x0001C610
	public static int GetFitIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentOutfit;
		if (num >= Singleton<Customization>.Instance.fits.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x0001E43C File Offset: 0x0001C63C
	public static int GetMouthIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentMouth;
		if (num >= Singleton<Customization>.Instance.mouths.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0001E468 File Offset: 0x0001C668
	public static int GetAccessoryIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentAccessory;
		if (num >= Singleton<Customization>.Instance.accessories.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0001E494 File Offset: 0x0001C694
	public static int GetSashIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentSash;
		if (num >= Singleton<Customization>.Instance.sashes.Length)
		{
			num = Singleton<Customization>.Instance.sashes.Length - 1;
		}
		return num;
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0001E4CC File Offset: 0x0001C6CC
	public static void SetCharacterSkinColor(int index)
	{
		if (index >= Singleton<Customization>.Instance.skins.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentSkin = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Skin, index);
		Debug.Log(string.Format("Set character color: {0}", index));
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x0001E528 File Offset: 0x0001C728
	public static void SetCharacterEyes(int index)
	{
		if (index >= Singleton<Customization>.Instance.eyes.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentEyes = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Eyes, index);
		Debug.Log(string.Format("Set character eyes: {0}", index));
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x0001E584 File Offset: 0x0001C784
	public static void SetCharacterMouth(int index)
	{
		if (index >= Singleton<Customization>.Instance.mouths.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentMouth = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Mouth, index);
		Debug.Log(string.Format("Setting Character Mouth: {0}", index));
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x0001E5E0 File Offset: 0x0001C7E0
	public static void SetCharacterAccessory(int index)
	{
		if (index >= Singleton<Customization>.Instance.accessories.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentAccessory = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Accessory, index);
		Debug.Log(string.Format("Setting Character Accessory: {0}", index));
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0001E63C File Offset: 0x0001C83C
	public static void SetCharacterOutfit(int index)
	{
		if (index >= Singleton<Customization>.Instance.fits.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentOutfit = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Outfit, index);
		Debug.Log(string.Format("Setting Character outfit: {0}", index));
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x0001E698 File Offset: 0x0001C898
	public static void SetCharacterHat(int index)
	{
		if (index >= Singleton<Customization>.Instance.hats.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentHat = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Hat, index);
		Debug.Log(string.Format("Setting Character Hat: {0}", index));
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0001E6F4 File Offset: 0x0001C8F4
	public static void SetCharacterSash(int index)
	{
		if (index >= Singleton<Customization>.Instance.sashes.Length)
		{
			index = Singleton<Customization>.Instance.sashes.Length - 1;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentSash = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Sash, index);
		Debug.Log(string.Format("Setting Character Sash: {0}", index));
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x0001E75D File Offset: 0x0001C95D
	public void SetRandomSkinColor()
	{
		CharacterCustomization.SetCharacterSkinColor(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Skin));
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x0001E76F File Offset: 0x0001C96F
	public void SetRandomEyes()
	{
		CharacterCustomization.SetCharacterEyes(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Eyes));
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x0001E782 File Offset: 0x0001C982
	public void SetRandomMouth()
	{
		CharacterCustomization.SetCharacterMouth(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Mouth));
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x0001E795 File Offset: 0x0001C995
	public void SetRandomAccessory()
	{
		CharacterCustomization.SetCharacterAccessory(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Accessory));
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0001E7A8 File Offset: 0x0001C9A8
	public void SetRandomOutfit()
	{
		CharacterCustomization.SetCharacterOutfit(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Fit));
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x0001E7BB File Offset: 0x0001C9BB
	public void SetRandomHat()
	{
		CharacterCustomization.SetCharacterHat(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Hat));
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x0001E7CE File Offset: 0x0001C9CE
	public void SetRandomIdle()
	{
		if (this.view.IsMine)
		{
			this.view.RPC("SetCharacterIdle_RPC", 3, new object[] { Random.Range(0, this.maxIdles) });
		}
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x0001E808 File Offset: 0x0001CA08
	[PunRPC]
	public void CharacterDied()
	{
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.deadEyes);
			this.refs.EyeRenderers[i].material.SetInt(CharacterCustomization.Spin, 0);
		}
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0001E86C File Offset: 0x0001CA6C
	[PunRPC]
	public void CharacterPassedOut()
	{
		this.isPassedOut = true;
		if (this.forcePresetEyes)
		{
			return;
		}
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.passedOutEyes);
			this.refs.EyeRenderers[i].material.SetInt(CharacterCustomization.Spin, 1);
		}
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0001E8E0 File Offset: 0x0001CAE0
	public void BecomeChicken()
	{
		if (!this.isCannibalizable)
		{
			this.isCannibalizable = true;
			if (this.chickenTweener != null)
			{
				TweenExtensions.Kill(this.chickenTweener, false);
			}
			this.ShowChicken();
			this.chickenTweener = TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(ShortcutExtensions.DOFloat(this.refs.chickenRenderer.material, 1f, CharacterCustomization.Opacity, 1f), new TweenCallback(this.HideHuman));
			for (int i = 0; i < this.refs.AllRenderers.Length; i++)
			{
				for (int j = 0; j < this.refs.AllRenderers[i].materials.Length; j++)
				{
					ShortcutExtensions.DOFloat(this.refs.AllRenderers[i].materials[j], 0f, CharacterCustomization.Opacity, 1f);
				}
			}
			this.chickenParticle.Play();
			ShortcutExtensions.DOLocalMoveY(this.refs.hatTransform, -4.66f, 1f, false);
		}
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0001E9DC File Offset: 0x0001CBDC
	public void BecomeHuman()
	{
		if (this.isCannibalizable)
		{
			this.isCannibalizable = false;
			if (this.chickenTweener != null)
			{
				TweenExtensions.Kill(this.chickenTweener, false);
			}
			this.chickenTweener = TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(ShortcutExtensions.DOFloat(this.refs.chickenRenderer.material, 0f, CharacterCustomization.Opacity, 1f), new TweenCallback(this.HideChicken));
			this.ShowHuman();
			for (int i = 0; i < this.refs.AllRenderers.Length; i++)
			{
				for (int j = 0; j < this.refs.AllRenderers[i].materials.Length; j++)
				{
					ShortcutExtensions.DOFloat(this.refs.AllRenderers[i].materials[j], 1f, CharacterCustomization.Opacity, 1f);
				}
			}
			this.chickenParticle.Stop();
			ShortcutExtensions.DOLocalMoveY(this.refs.hatTransform, -3.98f, 1f, false);
		}
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001EAD7 File Offset: 0x0001CCD7
	private void ShowChicken()
	{
		this.refs.chickenRenderer.gameObject.SetActive(true);
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x0001EAEF File Offset: 0x0001CCEF
	private void HideChicken()
	{
		this.refs.chickenRenderer.gameObject.SetActive(false);
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0001EB08 File Offset: 0x0001CD08
	private void ShowHuman()
	{
		this.refs.mainRenderer.gameObject.SetActive(true);
		this.refs.sashRenderer.gameObject.SetActive(true);
		this.refs.shorts.gameObject.SetActive(true);
		this.refs.skirt.gameObject.SetActive(true);
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0001EB70 File Offset: 0x0001CD70
	private void HideHuman()
	{
		this.refs.mainRenderer.gameObject.SetActive(false);
		this.refs.sashRenderer.gameObject.SetActive(false);
		this.refs.shorts.gameObject.SetActive(false);
		this.refs.skirt.gameObject.SetActive(false);
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0001EBD8 File Offset: 0x0001CDD8
	public void HideAllRenderers()
	{
		for (int i = 0; i < this.refs.AllRenderers.Length; i++)
		{
			this.refs.AllRenderers[i].enabled = false;
		}
		this.refs.hatTransform.gameObject.SetActive(false);
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0001EC28 File Offset: 0x0001CE28
	public void ShowAllRenderers()
	{
		for (int i = 0; i < this.refs.AllRenderers.Length; i++)
		{
			this.refs.AllRenderers[i].enabled = true;
		}
		this.refs.hatTransform.gameObject.SetActive(true);
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001EC78 File Offset: 0x0001CE78
	public void PulseStatus(Color c, float intensity = 1f)
	{
		for (int i = 0; i < this.refs.PlayerRenderers.Length; i++)
		{
			for (int j = 0; j < this.refs.PlayerRenderers[i].materials.Length; j++)
			{
				this.refs.PlayerRenderers[i].materials[j].SetColor(CharacterCustomization.StatusColor, c);
				this.refs.PlayerRenderers[i].materials[j].SetFloat(CharacterCustomization.StatusGlow, intensity);
				ShortcutExtensions.DOFloat(this.refs.PlayerRenderers[i].materials[j], 0f, CharacterCustomization.StatusGlow, 0.5f);
			}
		}
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0001ED2C File Offset: 0x0001CF2C
	[PunRPC]
	public void SetCharacterIdle_RPC(int index)
	{
		this.PlayerAnimator.SetFloat(CharacterCustomization.Idle, (float)index);
		Debug.Log(string.Format("Setting Character Idle: {0}", index));
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0001ED55 File Offset: 0x0001CF55
	private static CharacterCustomizationData GetCustomizationData(Player player)
	{
		return GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(player).customizationData;
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0001ED68 File Offset: 0x0001CF68
	private static void SetCustomizationData(CharacterCustomizationData customizationData, Player player)
	{
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		PersistentPlayerData playerData = service.GetPlayerData(player);
		playerData.customizationData = customizationData;
		service.SetPlayerData(player, playerData);
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001ED90 File Offset: 0x0001CF90
	public void Update()
	{
		if (this._character.data.passedOut && !this.isPassedOut)
		{
			this.isPassedOut = true;
			if (this.view.IsMine)
			{
				this.view.RPC("CharacterPassedOut", 3, Array.Empty<object>());
			}
		}
		if (this._character.data.dead && !this.isDead)
		{
			this.isDead = true;
			if (this.view.IsMine)
			{
				this.view.RPC("CharacterDied", 3, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0001EE25 File Offset: 0x0001D025
	public void OnRevive()
	{
		if (this.view.IsMine)
		{
			this.view.RPC("OnRevive_RPC", 3, Array.Empty<object>());
		}
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0001EE4C File Offset: 0x0001D04C
	[PunRPC]
	public void OnRevive_RPC()
	{
		Debug.Log("test dead");
		this.isDead = false;
		this.isPassedOut = false;
		if (this.forcePresetEyes)
		{
			return;
		}
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.CurrentEyeTexture);
			this.refs.EyeRenderers[i].material.SetInt(CharacterCustomization.Spin, 0);
		}
	}

	// Token: 0x0400056D RID: 1389
	private Character _character;

	// Token: 0x0400056E RID: 1390
	public CustomizationRefs refs;

	// Token: 0x0400056F RID: 1391
	public static readonly int MainTex = Shader.PropertyToID("_MainTex");

	// Token: 0x04000570 RID: 1392
	private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");

	// Token: 0x04000571 RID: 1393
	private static readonly int Idle = Animator.StringToHash("Idle");

	// Token: 0x04000572 RID: 1394
	private static readonly int Spin = Shader.PropertyToID("_Spin");

	// Token: 0x04000573 RID: 1395
	private static readonly int VertexGhost = Shader.PropertyToID("_VertexGhost");

	// Token: 0x04000574 RID: 1396
	private static readonly int StatusColor = Shader.PropertyToID("_StatusColor");

	// Token: 0x04000575 RID: 1397
	private static readonly int StatusGlow = Shader.PropertyToID("_StatusGlow");

	// Token: 0x04000576 RID: 1398
	private static readonly int Opacity = Shader.PropertyToID("_Opacity");

	// Token: 0x04000577 RID: 1399
	public bool useDebugColor;

	// Token: 0x04000578 RID: 1400
	public int debugColorIndex;

	// Token: 0x04000579 RID: 1401
	public int maxIdles;

	// Token: 0x0400057A RID: 1402
	public Animator PlayerAnimator;

	// Token: 0x0400057B RID: 1403
	private PhotonView view;

	// Token: 0x0400057C RID: 1404
	public ParticleSystem chickenParticle;

	// Token: 0x0400057D RID: 1405
	public Texture passedOutEyes;

	// Token: 0x0400057E RID: 1406
	public bool ignorePlayerCosmetics;

	// Token: 0x0400057F RID: 1407
	private Texture CurrentEyeTexture;

	// Token: 0x04000580 RID: 1408
	[FormerlySerializedAs("diedTexture")]
	public Texture deadEyes;

	// Token: 0x04000581 RID: 1409
	public bool forcePresetEyes;

	// Token: 0x04000582 RID: 1410
	[FormerlySerializedAs("isDead")]
	public bool isPassedOut;

	// Token: 0x04000583 RID: 1411
	public bool isDead;

	// Token: 0x04000584 RID: 1412
	public Player overridePhotonPlayer;

	// Token: 0x04000585 RID: 1413
	public bool isCannibalizable;

	// Token: 0x04000586 RID: 1414
	private Tweener chickenTweener;
}
