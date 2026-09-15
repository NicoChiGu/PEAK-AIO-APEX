using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002BD RID: 701
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(PhotonView))]
public class ChangeColor : MonoBehaviour
{
	// Token: 0x06001317 RID: 4887 RVA: 0x00060F74 File Offset: 0x0005F174
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (this.photonView.IsMine)
		{
			Color color = Random.ColorHSV();
			this.photonView.RPC("ChangeColour", 3, new object[]
			{
				new Vector3(color.r, color.g, color.b)
			});
		}
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x00060FD6 File Offset: 0x0005F1D6
	[PunRPC]
	private void ChangeColour(Vector3 randomColor)
	{
		base.GetComponent<Renderer>().material.SetColor("_Color", new Color(randomColor.x, randomColor.y, randomColor.z));
	}

	// Token: 0x040011BF RID: 4543
	private PhotonView photonView;
}
