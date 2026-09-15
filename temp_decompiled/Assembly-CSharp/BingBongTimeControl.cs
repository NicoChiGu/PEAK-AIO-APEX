using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000216 RID: 534
public class BingBongTimeControl : MonoBehaviour
{
	// Token: 0x06000FC1 RID: 4033 RVA: 0x0004E713 File Offset: 0x0004C913
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x0004E724 File Offset: 0x0004C924
	private void Update()
	{
		this.syncCounter += Time.unscaledDeltaTime;
		if (Input.GetKeyDown(114))
		{
			this.currentTimeScale = 1f;
		}
		if (Input.GetKeyDown(102))
		{
			this.currentTimeScale = 0f;
		}
		if (Input.GetKeyDown(323))
		{
			this.currentTimeScale += Mathf.Clamp(0.1f, this.currentTimeScale * 0.3f, 0.5f);
		}
		if (Input.GetKeyDown(324))
		{
			this.currentTimeScale -= Mathf.Clamp(0.1f, this.currentTimeScale * 0.3f, 0.5f);
		}
		this.currentTimeScale = Mathf.Clamp(this.currentTimeScale, 0.02f, 10f);
		if (Time.timeScale != this.currentTimeScale)
		{
			this.bingBongPowers.SetTip(string.Format("Time Scale: {0:P0}", this.currentTimeScale), 1);
			if (this.syncCounter > 0.1f)
			{
				this.view.RPC("RPCA_SyncTime", 0, new object[] { this.currentTimeScale });
			}
		}
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x0004E84E File Offset: 0x0004CA4E
	[PunRPC]
	public void RPCA_SyncTime(float newTime)
	{
		Time.timeScale = newTime;
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x0004E856 File Offset: 0x0004CA56
	private void OnDestroy()
	{
		Time.timeScale = 1f;
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x0004E862 File Offset: 0x0004CA62
	private void OnEnable()
	{
		this.bingBongPowers = base.GetComponent<BingBongPowers>();
		this.bingBongPowers.SetTexts("TIME", this.descr);
	}

	// Token: 0x04000E1D RID: 3613
	private PhotonView view;

	// Token: 0x04000E1E RID: 3614
	public float currentTimeScale = 1f;

	// Token: 0x04000E1F RID: 3615
	private float syncCounter;

	// Token: 0x04000E20 RID: 3616
	private BingBongPowers bingBongPowers;

	// Token: 0x04000E21 RID: 3617
	private string descr = "Reset time: [R]\n\nFreeze: [F]\n\nFaster: [LMB]\n\nSlower: [RMB]";
}
