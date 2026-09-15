using System;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x0200038B RID: 907
	public class ThirdPersonController : BaseController
	{
		// Token: 0x0600173E RID: 5950 RVA: 0x00076358 File Offset: 0x00074558
		protected override void Move(float h, float v)
		{
			this.rigidBody.linearVelocity = v * this.speed * base.transform.forward;
			base.transform.rotation *= Quaternion.AngleAxis(this.movingTurnSpeed * h * Time.deltaTime, Vector3.up);
		}

		// Token: 0x040015B7 RID: 5559
		[SerializeField]
		private float movingTurnSpeed = 360f;
	}
}
