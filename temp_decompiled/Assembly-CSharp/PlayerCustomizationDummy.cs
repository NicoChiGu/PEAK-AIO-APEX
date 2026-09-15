using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002C2 RID: 706
public class PlayerCustomizationDummy : MonoBehaviour
{
	// Token: 0x0600132C RID: 4908 RVA: 0x000616D8 File Offset: 0x0005F8D8
	public void UpdateDummy()
	{
		PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer);
		this.SetPlayerColor(playerData.customizationData.currentSkin);
		int fitIndex = CharacterCustomization.GetFitIndex(playerData);
		this.SetPlayerCostume(fitIndex);
		int num = playerData.customizationData.currentHat;
		if (Singleton<Customization>.Instance.fits[fitIndex].overrideHat)
		{
			num = Singleton<Customization>.Instance.fits[fitIndex].overrideHatIndex;
		}
		this.SetPlayerHat(num);
		int eyesIndex = CharacterCustomization.GetEyesIndex(playerData);
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(PlayerCustomizationDummy.MainTex, Singleton<Customization>.Instance.eyes[eyesIndex].texture);
		}
		int accessoryIndex = CharacterCustomization.GetAccessoryIndex(playerData);
		this.refs.accessoryRenderer.material.SetTexture(PlayerCustomizationDummy.MainTex, Singleton<Customization>.Instance.accessories[accessoryIndex].texture);
		this.refs.accessoryRenderer.material.renderQueue = (Singleton<Customization>.Instance.accessories[accessoryIndex].drawUnderEye ? 3007 : 3009);
		this.refs.accessoryRenderer.enabled = !Singleton<Customization>.Instance.accessories[accessoryIndex].isThirdEye;
		this.refs.thirdEye.gameObject.SetActive(Singleton<Customization>.Instance.accessories[accessoryIndex].isThirdEye);
		this.refs.mouthRenderer.material.SetTexture(PlayerCustomizationDummy.MainTex, Singleton<Customization>.Instance.mouths[playerData.customizationData.currentMouth].texture);
		List<Material> list = new List<Material>();
		list.Add(this.refs.sashRenderer.materials[0]);
		int num2 = CharacterCustomization.GetSashIndex(playerData);
		if (num2 >= this.refs.sashAscentMaterials.Length)
		{
			num2 = this.refs.sashAscentMaterials.Length - 1;
		}
		list.Add(this.refs.sashAscentMaterials[num2]);
		this.refs.sashRenderer.SetMaterials(list);
	}

	// Token: 0x0600132D RID: 4909 RVA: 0x000618F8 File Offset: 0x0005FAF8
	public void SetPlayerCostume(int index)
	{
		int num = index;
		if (num >= Singleton<Customization>.Instance.fits.Length)
		{
			num = 0;
		}
		this.refs.mainRenderer.sharedMesh = Singleton<Customization>.Instance.fits[num].fitMesh;
		List<Material> list = new List<Material>();
		list.Add(this.refs.mainRenderer.materials[0]);
		list.Add(Singleton<Customization>.Instance.fits[num].fitMaterial);
		list.Add(Singleton<Customization>.Instance.fits[num].fitMaterialShoes);
		this.refs.mainRenderer.SetSharedMaterials(list);
		if (Singleton<Customization>.Instance.fits[num].noPants)
		{
			this.refs.skirt.gameObject.SetActive(false);
			this.refs.shorts.gameObject.SetActive(false);
		}
		else if (Singleton<Customization>.Instance.fits[num].isSkirt)
		{
			this.refs.skirt.gameObject.SetActive(true);
			this.refs.shorts.gameObject.SetActive(false);
			this.refs.skirt.sharedMaterial = Singleton<Customization>.Instance.fits[num].fitPantsMaterial;
		}
		else
		{
			this.refs.skirt.gameObject.SetActive(false);
			this.refs.shorts.gameObject.SetActive(true);
			this.refs.shorts.sharedMaterial = Singleton<Customization>.Instance.fits[num].fitPantsMaterial;
		}
		this.refs.playerHats[0].material = Singleton<Customization>.Instance.fits[num].fitHatMaterial;
		this.refs.playerHats[1].material = Singleton<Customization>.Instance.fits[num].fitHatMaterial;
	}

	// Token: 0x0600132E RID: 4910 RVA: 0x00061AD0 File Offset: 0x0005FCD0
	public void SetPlayerHat(int index)
	{
		for (int i = 0; i < this.refs.playerHats.Length; i++)
		{
			this.refs.playerHats[i].gameObject.SetActive(index == i);
		}
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x00061B10 File Offset: 0x0005FD10
	public void SetPlayerColor(int index)
	{
		if (index >= Singleton<Customization>.Instance.skins.Length)
		{
			return;
		}
		for (int i = 0; i < this.refs.PlayerRenderers.Length; i++)
		{
			this.refs.PlayerRenderers[i].material.SetColor(PlayerCustomizationDummy.SkinColor, Singleton<Customization>.Instance.skins[index].color);
		}
		for (int j = 0; j < this.refs.EyeRenderers.Length; j++)
		{
			this.refs.EyeRenderers[j].material.SetColor(PlayerCustomizationDummy.SkinColor, Singleton<Customization>.Instance.skins[index].color);
		}
	}

	// Token: 0x040011D1 RID: 4561
	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	// Token: 0x040011D2 RID: 4562
	private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");

	// Token: 0x040011D3 RID: 4563
	public CustomizationRefs refs;
}
