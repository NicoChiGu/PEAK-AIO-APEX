using System;
using Photon.Pun;
using Sirenix.Utilities;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002C3 RID: 707
public class PlayerGhost : MonoBehaviour
{
	// Token: 0x06001332 RID: 4914 RVA: 0x00061BDF File Offset: 0x0005FDDF
	private void Awake()
	{
		this.m_view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x00061BF0 File Offset: 0x0005FDF0
	[PunRPC]
	public void RPCA_InitGhost(PhotonView character, PhotonView t)
	{
		this.m_owner = character.GetComponent<Character>();
		this.m_owner.Ghost = this;
		this.RPCA_SetTarget(t);
		PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(this.m_owner.photonView.Owner);
		this.animatedMouth.audioSource = character.GetComponent<AnimatedMouth>().audioSource;
		this.CustomizeGhost(playerData);
		if (character.IsMine)
		{
			LinqExtensions.ForEach<Renderer>(this.PlayerRenderers, delegate(Renderer r)
			{
				r.enabled = false;
			});
			LinqExtensions.ForEach<Renderer>(this.EyeRenderers, delegate(Renderer r)
			{
				r.enabled = false;
			});
			this.mouthRenderer.enabled = false;
			this.accessoryRenderer.enabled = false;
			this.thirdEye.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x00061CDC File Offset: 0x0005FEDC
	private void CustomizeGhost(PersistentPlayerData data)
	{
		int skinIndex = CharacterCustomization.GetSkinIndex(data);
		for (int i = 0; i < this.PlayerRenderers.Length; i++)
		{
			this.PlayerRenderers[i].material.SetColor("_PlayerColor", Singleton<Customization>.Instance.skins[skinIndex].color);
		}
		for (int j = 0; j < this.EyeRenderers.Length; j++)
		{
			this.EyeRenderers[j].material.SetColor(PlayerGhost.SkinColor, Singleton<Customization>.Instance.skins[skinIndex].color);
		}
		int eyesIndex = CharacterCustomization.GetEyesIndex(data);
		for (int k = 0; k < this.EyeRenderers.Length; k++)
		{
			this.EyeRenderers[k].material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.eyes[eyesIndex].texture);
		}
		int accessoryIndex = CharacterCustomization.GetAccessoryIndex(data);
		this.accessoryRenderer.material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.accessories[accessoryIndex].texture);
		this.accessoryRenderer.gameObject.SetActive(accessoryIndex != 20);
		this.thirdEye.gameObject.SetActive(accessoryIndex == 20);
		int mouthIndex = CharacterCustomization.GetMouthIndex(data);
		this.mouthRenderer.material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.mouths[mouthIndex].texture);
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x00061E3E File Offset: 0x0006003E
	[PunRPC]
	public void RPCA_SetTarget(PhotonView t)
	{
		this.m_target = t.GetComponent<Character>();
	}

	// Token: 0x06001336 RID: 4918 RVA: 0x00061E4C File Offset: 0x0006004C
	private void Update()
	{
		if (!this.m_target)
		{
			return;
		}
		Vector3 vector = this.m_target.Center;
		base.transform.rotation = Quaternion.LookRotation(this.m_owner.data.lookDirection);
		vector += base.transform.forward * -1f * this.m_owner.data.spectateZoom;
		vector += base.transform.up * 0.5f;
		base.transform.position = Vector3.Lerp(base.transform.position, vector, Time.deltaTime * 3f);
		base.transform.rotation = Quaternion.LookRotation(MainCamera.instance.cam.transform.position - base.transform.position);
	}

	// Token: 0x040011D4 RID: 4564
	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	// Token: 0x040011D5 RID: 4565
	private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");

	// Token: 0x040011D6 RID: 4566
	public Character m_target;

	// Token: 0x040011D7 RID: 4567
	public Character m_owner;

	// Token: 0x040011D8 RID: 4568
	public PhotonView m_view;

	// Token: 0x040011D9 RID: 4569
	[Header("Customization Refrences")]
	public Renderer[] PlayerRenderers;

	// Token: 0x040011DA RID: 4570
	public Renderer[] EyeRenderers;

	// Token: 0x040011DB RID: 4571
	public Renderer mouthRenderer;

	// Token: 0x040011DC RID: 4572
	public Renderer accessoryRenderer;

	// Token: 0x040011DD RID: 4573
	public AnimatedMouth animatedMouth;

	// Token: 0x040011DE RID: 4574
	public GameObject thirdEye;
}
