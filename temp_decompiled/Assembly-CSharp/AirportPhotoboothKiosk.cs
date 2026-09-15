using System;
using System.Collections;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000055 RID: 85
public class AirportPhotoboothKiosk : MonoBehaviour, IInteractible
{
	// Token: 0x06000441 RID: 1089 RVA: 0x0001A693 File Offset: 0x00018893
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000442 RID: 1090 RVA: 0x0001A696 File Offset: 0x00018896
	// (set) Token: 0x06000443 RID: 1091 RVA: 0x0001A6C4 File Offset: 0x000188C4
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = base.GetComponentsInChildren<MeshRenderer>();
				MonoBehaviour.print(this._mr.Length);
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x0001A6CD File Offset: 0x000188CD
	public void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x0001A6DA File Offset: 0x000188DA
	private void Start()
	{
		this.flashImage.enabled = !GUIManager.instance.photosensitivity;
		this.photosensitiveFlashImage.enabled = GUIManager.instance.photosensitivity;
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x0001A70C File Offset: 0x0001890C
	private void Update()
	{
		this.inPhotobooth = Character.localCharacter != null && Character.localCharacter.Center.x < this.insidePlaneTf.position.x;
		this.displayCamera.enabled = this.inPhotobooth;
		this.screen.SetActive(this.inPhotobooth);
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x0001A772 File Offset: 0x00018972
	public void Interact(Character interactor)
	{
		if (!this.takingPhoto)
		{
			this.view.RPC("InteractRPC", 0, Array.Empty<object>());
		}
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x0001A792 File Offset: 0x00018992
	[PunRPC]
	private void InteractRPC()
	{
		this.takingPhoto = true;
		base.StartCoroutine(this.PhotoboothRoutine());
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x0001A7A8 File Offset: 0x000189A8
	private IEnumerator PhotoboothRoutine()
	{
		this.anim.SetTrigger("Start");
		yield return new WaitForSeconds(3f);
		this.actualCamera.targetTexture = this.photoTextures[0];
		this.actualCamera.Render();
		yield return new WaitForSeconds(1f);
		this.anim.SetTrigger("Start");
		yield return new WaitForSeconds(3f);
		this.actualCamera.targetTexture = this.photoTextures[1];
		this.actualCamera.Render();
		yield return new WaitForSeconds(1f);
		this.anim.SetTrigger("Start");
		yield return new WaitForSeconds(3f);
		this.actualCamera.targetTexture = this.photoTextures[2];
		this.actualCamera.Render();
		yield return new WaitForSeconds(1f);
		this.anim.SetTrigger("Start");
		yield return new WaitForSeconds(3f);
		this.actualCamera.targetTexture = this.photoTextures[3];
		this.actualCamera.Render();
		yield return new WaitForSeconds(1f);
		if (this.inPhotobooth)
		{
			this.photoCanvas.SetActive(true);
			yield return new WaitForSeconds(3f);
			SteamScreenshots.TriggerScreenshot();
			this.takingPhoto = false;
			yield return new WaitForSeconds(2f);
			this.photoCanvas.SetActive(false);
		}
		else
		{
			yield return new WaitForSeconds(5f);
			this.takingPhoto = false;
		}
		yield break;
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x0001A7B8 File Offset: 0x000189B8
	public void HoverEnter()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				if (this.meshRenderers[i] != null)
				{
					this.meshRenderers[i].SetPropertyBlock(this.mpb);
				}
			}
		}
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x0001A818 File Offset: 0x00018A18
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].SetPropertyBlock(this.mpb);
			}
		}
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x0001A868 File Offset: 0x00018A68
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x0001A875 File Offset: 0x00018A75
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0001A87D File Offset: 0x00018A7D
	public string GetInteractionText()
	{
		return LocalizedText.GetText("START2", true);
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x0001A88A File Offset: 0x00018A8A
	public string GetName()
	{
		return LocalizedText.GetText("PHOTOBOOTH", true);
	}

	// Token: 0x040004BD RID: 1213
	public PhotonView view;

	// Token: 0x040004BE RID: 1214
	public Camera displayCamera;

	// Token: 0x040004BF RID: 1215
	public Camera actualCamera;

	// Token: 0x040004C0 RID: 1216
	public Animator anim;

	// Token: 0x040004C1 RID: 1217
	private MaterialPropertyBlock mpb;

	// Token: 0x040004C2 RID: 1218
	public GameObject photoCanvas;

	// Token: 0x040004C3 RID: 1219
	public GameObject screen;

	// Token: 0x040004C4 RID: 1220
	public RenderTexture[] photoTextures;

	// Token: 0x040004C5 RID: 1221
	public Image flashImage;

	// Token: 0x040004C6 RID: 1222
	public Image photosensitiveFlashImage;

	// Token: 0x040004C7 RID: 1223
	public Transform insidePlaneTf;

	// Token: 0x040004C8 RID: 1224
	private bool inPhotobooth;

	// Token: 0x040004C9 RID: 1225
	private MeshRenderer[] _mr;

	// Token: 0x040004CA RID: 1226
	private bool takingPhoto;
}
