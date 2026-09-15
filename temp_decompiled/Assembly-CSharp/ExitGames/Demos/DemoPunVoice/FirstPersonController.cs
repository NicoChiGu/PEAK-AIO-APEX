using System;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000388 RID: 904
	public class FirstPersonController : BaseController
	{
		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600172B RID: 5931 RVA: 0x00075FB4 File Offset: 0x000741B4
		public Vector3 Velocity
		{
			get
			{
				return this.rigidBody.linearVelocity;
			}
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x00075FC1 File Offset: 0x000741C1
		protected override void SetCamera()
		{
			base.SetCamera();
			this.mouseLook.Init(base.transform, this.camTrans);
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x00075FE0 File Offset: 0x000741E0
		protected override void Move(float h, float v)
		{
			Vector3 vector = this.camTrans.forward * v + this.camTrans.right * h;
			vector.x *= this.speed;
			vector.z *= this.speed;
			vector.y = 0f;
			this.rigidBody.linearVelocity = vector;
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x00076055 File Offset: 0x00074255
		private void Update()
		{
			this.RotateView();
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x00076060 File Offset: 0x00074260
		private void RotateView()
		{
			this.oldYRotation = base.transform.eulerAngles.y;
			this.mouseLook.LookRotation(base.transform, this.camTrans);
			this.velRotation = Quaternion.AngleAxis(base.transform.eulerAngles.y - this.oldYRotation, Vector3.up);
			this.rigidBody.linearVelocity = this.velRotation * this.rigidBody.linearVelocity;
		}

		// Token: 0x040015AC RID: 5548
		[SerializeField]
		private MouseLookHelper mouseLook = new MouseLookHelper();

		// Token: 0x040015AD RID: 5549
		private float oldYRotation;

		// Token: 0x040015AE RID: 5550
		private Quaternion velRotation;
	}
}
