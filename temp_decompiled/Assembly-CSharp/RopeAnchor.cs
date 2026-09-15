using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class RopeAnchor : MonoBehaviour
{
	// Token: 0x06000B95 RID: 2965 RVA: 0x0003DD7D File Offset: 0x0003BF7D
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x06000B96 RID: 2966 RVA: 0x0003DD8B File Offset: 0x0003BF8B
	// (set) Token: 0x06000B97 RID: 2967 RVA: 0x0003DD93 File Offset: 0x0003BF93
	public bool Ghost
	{
		get
		{
			return this.isGhost;
		}
		set
		{
			this.isGhost = value;
			this.HideAll();
			if (this.isGhost)
			{
				this.ghostPart.SetActive(true);
				return;
			}
			this.normalPart.SetActive(true);
		}
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x0003DDC3 File Offset: 0x0003BFC3
	private void HideAll()
	{
		this.ghostPart.SetActive(false);
		this.normalPart.SetActive(false);
	}

	// Token: 0x04000ABC RID: 2748
	public GameObject ghostPart;

	// Token: 0x04000ABD RID: 2749
	public GameObject normalPart;

	// Token: 0x04000ABE RID: 2750
	public Transform anchorPoint;

	// Token: 0x04000ABF RID: 2751
	private bool isGhost;

	// Token: 0x04000AC0 RID: 2752
	public PhotonView photonView;
}
