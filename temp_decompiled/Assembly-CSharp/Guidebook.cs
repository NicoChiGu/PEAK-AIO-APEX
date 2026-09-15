using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200001E RID: 30
public class Guidebook : Item
{
	// Token: 0x06000242 RID: 578 RVA: 0x00011160 File Offset: 0x0000F360
	public override void OnEnable()
	{
		base.OnEnable();
		if (SettingsHandler.Instance != null && SettingsHandler.Instance.GetSetting<RenderScaleSetting>().Value == RenderScaleSetting.RenderScaleQuality.Low)
		{
			this.canvasScaler.scaleFactor = 2f;
			this.currentRenderTexture.width = 3600;
			this.currentRenderTexture.height = 1800;
			this.lastRenderTexture.width = 3600;
			this.lastRenderTexture.height = 1800;
		}
		else
		{
			this.canvasScaler.scaleFactor = 1f;
			this.currentRenderTexture.width = 1800;
			this.currentRenderTexture.height = 900;
			this.lastRenderTexture.width = 1800;
			this.lastRenderTexture.height = 900;
		}
		if (this.isSinglePage)
		{
			base.Invoke("OpenSinglePage", 0.01f);
		}
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00011248 File Offset: 0x0000F448
	private void OpenSinglePage()
	{
		RenderTexture renderTexture = new RenderTexture(this.guidebookRenderTexture);
		renderTexture.Create();
		this.guidebookRenderTexture = renderTexture;
		this.currentRenderTexture = renderTexture;
		this.renderCamera.targetTexture = this.guidebookRenderTexture;
		if (base.itemState == ItemState.InBackpack)
		{
			this.canvasScaler.gameObject.SetActive(false);
		}
		this.currentlyVisibleLeftPageIndex = 2;
		this.currentlyVisibleRightPageIndex = 3;
		this.nextVisibleLeftPageIndex = 0;
		this.nextVisibleRightPageIndex = 1;
		this.UpdatePageDisplay();
		for (int i = 0; i < this.pageRenderers.Length; i++)
		{
			this.pageRenderers[i].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
		}
	}

	// Token: 0x06000244 RID: 580 RVA: 0x000112F3 File Offset: 0x0000F4F3
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.isSinglePage)
		{
			Object.Destroy(this.renderCamera.targetTexture);
		}
	}

	// Token: 0x06000245 RID: 581 RVA: 0x00011313 File Offset: 0x0000F513
	internal void ToggleGuidebook()
	{
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("ToggleGuidebook_RPC", 0, new object[] { !this.isOpen });
		}
	}

	// Token: 0x06000246 RID: 582 RVA: 0x0001134C File Offset: 0x0000F54C
	[PunRPC]
	public void ToggleGuidebook_RPC(bool open)
	{
		this.isOpen = open;
		if (this.isOpen)
		{
			if (!this.isSinglePage)
			{
				this.anim.Play("Open", 0, 0f);
			}
			this.coll.enabled = false;
			this.renderCamera.targetTexture = this.guidebookRenderTexture;
			this.currentlyVisibleLeftPageIndex = 2;
			this.currentlyVisibleRightPageIndex = 3;
			this.nextVisibleLeftPageIndex = 0;
			this.nextVisibleRightPageIndex = 1;
			this.UpdatePageDisplay();
			for (int i = 0; i < this.pageRenderers.Length; i++)
			{
				this.pageRenderers[i].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
			}
			return;
		}
		if (!this.isSinglePage)
		{
			this.anim.Play("Close", 0, 0f);
		}
		this.coll.enabled = true;
		ShortcutExtensions.DOLocalMove(this.bookTransform, Vector3.zero, 0.25f, false);
		ShortcutExtensions.DOLocalRotate(this.bookTransform, Vector3.zero, 0.25f, 0);
		for (int j = 0; j < this.pageRenderers.Length; j++)
		{
			this.pageRenderers[j].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00011484 File Offset: 0x0000F684
	private void LateUpdate()
	{
		if (this.isOpen && base.holderCharacter.IsLocal)
		{
			this.bookTransform.position = Vector3.Lerp(this.bookTransform.position, MainCamera.instance.cam.transform.position + MainCamera.instance.cam.transform.forward * this.readingDistance, Time.deltaTime * 10f);
			this.bookTransform.forward = MainCamera.instance.cam.transform.forward;
		}
	}

	// Token: 0x06000248 RID: 584 RVA: 0x00011526 File Offset: 0x0000F726
	private void PopulatePages()
	{
		this.pageSpreads = Enumerable.ToList<GuidebookSpread>(base.GetComponentsInChildren<GuidebookSpread>(true));
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0001153C File Offset: 0x0000F73C
	private void PopulatePageNumbers()
	{
		for (int i = 0; i < this.pageSpreads.Count; i++)
		{
		}
	}

	// Token: 0x0600024A RID: 586 RVA: 0x00011560 File Offset: 0x0000F760
	internal void FlipPageRight()
	{
		if (base.photonView.IsMine && this.currentPageSet < this.pageSpreads.Count - 1)
		{
			this.currentPageSet++;
			base.photonView.RPC("FlipPageRight_RPC", 0, new object[] { this.currentPageSet });
		}
	}

	// Token: 0x0600024B RID: 587 RVA: 0x000115C4 File Offset: 0x0000F7C4
	internal void FlipPageLeft()
	{
		if (base.photonView.IsMine && this.currentPageSet >= 1)
		{
			this.currentPageSet--;
			base.photonView.RPC("FlipPageLeft_RPC", 0, new object[] { this.currentPageSet });
		}
	}

	// Token: 0x0600024C RID: 588 RVA: 0x0001161C File Offset: 0x0000F81C
	[PunRPC]
	public void FlipPageRight_RPC(int currentPage)
	{
		this.currentlyVisibleLeftPageIndex = 2;
		this.currentlyVisibleRightPageIndex = 3;
		this.nextVisibleLeftPageIndex = 4;
		this.nextVisibleRightPageIndex = 5;
		this.anim.Play("Guidebook_FlipRight", 0, 0f);
		this.currentPageSet = currentPage;
		this.UpdatePageDisplay();
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00011668 File Offset: 0x0000F868
	[PunRPC]
	public void FlipPageLeft_RPC(int currentPage)
	{
		this.currentlyVisibleLeftPageIndex = 2;
		this.currentlyVisibleRightPageIndex = 3;
		this.nextVisibleLeftPageIndex = 0;
		this.nextVisibleRightPageIndex = 1;
		this.anim.Play("Guidebook_FlipLeft", 0, 0f);
		this.currentPageSet = currentPage;
		this.UpdatePageDisplay();
	}

	// Token: 0x0600024E RID: 590 RVA: 0x000116B4 File Offset: 0x0000F8B4
	private void UpdatePageDisplay()
	{
		Graphics.CopyTexture(this.currentRenderTexture, this.lastRenderTexture);
		for (int i = 0; i < this.pageSpreads.Count; i++)
		{
			this.pageSpreads[i].gameObject.SetActive(i == this.currentPageSet);
		}
		this.renderCamera.Render();
		this.pageRenderers[this.currentlyVisibleLeftPageIndex].material.SetTexture(Guidebook.BASETEX, this.lastRenderTexture);
		this.pageRenderers[this.currentlyVisibleRightPageIndex].material.SetTexture(Guidebook.BASETEX, this.lastRenderTexture);
		this.pageRenderers[this.nextVisibleLeftPageIndex].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
		this.pageRenderers[this.nextVisibleRightPageIndex].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
	}

	// Token: 0x04000225 RID: 549
	public static int BASETEX = Shader.PropertyToID("_BaseTexture");

	// Token: 0x04000226 RID: 550
	public bool isSinglePage;

	// Token: 0x04000227 RID: 551
	public Animator anim;

	// Token: 0x04000228 RID: 552
	public int currentPageSet;

	// Token: 0x04000229 RID: 553
	[FormerlySerializedAs("pages")]
	[PreviouslySerializedAs("pages")]
	public List<GuidebookSpread> pageSpreads;

	// Token: 0x0400022A RID: 554
	public List<RectTransform> pagePrefabs;

	// Token: 0x0400022B RID: 555
	public Camera renderCamera;

	// Token: 0x0400022C RID: 556
	public CanvasScaler canvasScaler;

	// Token: 0x0400022D RID: 557
	public Texture currentRenderTexture;

	// Token: 0x0400022E RID: 558
	public Texture lastRenderTexture;

	// Token: 0x0400022F RID: 559
	public Renderer[] pageRenderers;

	// Token: 0x04000230 RID: 560
	public Transform bookTransform;

	// Token: 0x04000231 RID: 561
	public float readingDistance = 0.4f;

	// Token: 0x04000232 RID: 562
	public Collider coll;

	// Token: 0x04000233 RID: 563
	[HideInInspector]
	public bool isOpen;

	// Token: 0x04000234 RID: 564
	public RenderTexture guidebookRenderTexture;

	// Token: 0x04000235 RID: 565
	private int currentlyVisibleLeftPageIndex;

	// Token: 0x04000236 RID: 566
	private int currentlyVisibleRightPageIndex;

	// Token: 0x04000237 RID: 567
	private int nextVisibleLeftPageIndex;

	// Token: 0x04000238 RID: 568
	private int nextVisibleRightPageIndex;
}
