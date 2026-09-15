using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200030C RID: 780
public class RemoveAfterSeconds : MonoBehaviour
{
	// Token: 0x06001431 RID: 5169 RVA: 0x000664F7 File Offset: 0x000646F7
	private void Start()
	{
		if (this.photonRemove)
		{
			this.view = base.GetComponent<PhotonView>();
		}
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x0006650D File Offset: 0x0006470D
	public void Config(bool setShrink, float setSeconds)
	{
		this.seconds = setSeconds;
		this.shrink = setShrink;
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x00066520 File Offset: 0x00064720
	private void Update()
	{
		if (this.seconds < 0f)
		{
			if (this.shrink && base.transform.localScale.x > 0.01f)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.zero, Time.deltaTime);
				return;
			}
			if (!this.photonRemove || !this.view)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			if (this.view.IsMine)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
		}
		else
		{
			this.seconds -= Time.deltaTime;
		}
	}

	// Token: 0x040012B4 RID: 4788
	public float seconds = 5f;

	// Token: 0x040012B5 RID: 4789
	public bool shrink;

	// Token: 0x040012B6 RID: 4790
	public bool photonRemove;

	// Token: 0x040012B7 RID: 4791
	private PhotonView view;
}
