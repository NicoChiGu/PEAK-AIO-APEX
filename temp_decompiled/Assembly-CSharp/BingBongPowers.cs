using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000213 RID: 531
[DefaultExecutionOrder(1000000)]
public class BingBongPowers : MonoBehaviour
{
	// Token: 0x06000FAE RID: 4014 RVA: 0x0004DE9A File Offset: 0x0004C09A
	private void Start()
	{
		this.SetGodCamStyle();
		base.GetComponentInChildren<Canvas>().enabled = base.GetComponent<PhotonView>().IsMine;
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0004DEB8 File Offset: 0x0004C0B8
	private void SetGodCamStyle()
	{
		MainCameraMovement component = MainCamera.instance.GetComponent<MainCameraMovement>();
		component.godcam.lookSens = 20f;
		component.godcam.lookDrag = 5f;
		component.godcam.force = 15f;
		component.godcam.drag = 3f;
		component.godcam.canOrbit = false;
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x0004DF1A File Offset: 0x0004C11A
	private void LateUpdate()
	{
		this.TogglePowers();
		base.transform.position = MainCamera.instance.transform.position;
		base.transform.rotation = MainCamera.instance.transform.rotation;
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x0004DF56 File Offset: 0x0004C156
	private void TogglePowers()
	{
		if (Input.GetKeyDown(49))
		{
			this.ToggleID(0);
		}
		if (Input.GetKeyDown(50))
		{
			this.ToggleID(1);
		}
		if (Input.GetKeyDown(51))
		{
			this.ToggleID(2);
		}
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0004DF88 File Offset: 0x0004C188
	private void ToggleID(int id)
	{
		base.GetComponent<BingBongPhysics>().enabled = false;
		base.GetComponent<BingBongTimeControl>().enabled = false;
		base.GetComponent<BingBongStatus>().enabled = false;
		if (id == 0)
		{
			base.GetComponent<BingBongPhysics>().enabled = true;
		}
		if (id == 1)
		{
			base.GetComponent<BingBongTimeControl>().enabled = true;
		}
		if (id == 2)
		{
			base.GetComponent<BingBongStatus>().enabled = true;
		}
		for (int i = 0; i < this.tooltipBar.childCount; i++)
		{
			if (i == id)
			{
				this.tooltipBar.GetChild(i).GetComponent<CanvasGroup>().alpha = 1f;
			}
			else
			{
				this.tooltipBar.GetChild(i).GetComponent<CanvasGroup>().alpha = 0.5f;
			}
		}
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x0004E03A File Offset: 0x0004C23A
	public void SetTexts(string titleDescr, string description)
	{
		this.titleText.text = titleDescr;
		this.descriptionText.text = description;
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x0004E054 File Offset: 0x0004C254
	public void SetTip(string tip, int toolID)
	{
		this.tooltipBar.GetChild(toolID).Find("Tip").GetComponent<TextMeshProUGUI>()
			.text = tip;
	}

	// Token: 0x04000E0B RID: 3595
	public TextMeshProUGUI titleText;

	// Token: 0x04000E0C RID: 3596
	public TextMeshProUGUI descriptionText;

	// Token: 0x04000E0D RID: 3597
	public Transform tooltipBar;
}
