using System;
using UnityEngine;

// Token: 0x0200034F RID: 847
public class TiedBalloonCollision : MonoBehaviour
{
	// Token: 0x060015C3 RID: 5571 RVA: 0x00070445 File Offset: 0x0006E645
	private void OnCollisionEnter(Collision collision)
	{
		if (this.tiedBalloon.photonView.IsMine && collision.collider.GetComponent<StickyCactus>())
		{
			this.tiedBalloon.Pop();
		}
	}

	// Token: 0x0400149D RID: 5277
	public TiedBalloon tiedBalloon;
}
