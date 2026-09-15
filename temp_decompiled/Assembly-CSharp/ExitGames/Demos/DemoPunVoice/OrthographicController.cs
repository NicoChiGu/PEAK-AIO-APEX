using System;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x0200038A RID: 906
	public class OrthographicController : ThirdPersonController
	{
		// Token: 0x06001739 RID: 5945 RVA: 0x000762AB File Offset: 0x000744AB
		protected override void Init()
		{
			base.Init();
			this.ControllerCamera = Camera.main;
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x000762BE File Offset: 0x000744BE
		protected override void SetCamera()
		{
			base.SetCamera();
			this.offset = this.camTrans.position - base.transform.position;
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x000762E7 File Offset: 0x000744E7
		protected override void Move(float h, float v)
		{
			base.Move(h, v);
			this.CameraFollow();
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x000762F8 File Offset: 0x000744F8
		private void CameraFollow()
		{
			Vector3 vector = base.transform.position + this.offset;
			this.camTrans.position = Vector3.Lerp(this.camTrans.position, vector, this.smoothing * Time.deltaTime);
		}

		// Token: 0x040015B5 RID: 5557
		public float smoothing = 5f;

		// Token: 0x040015B6 RID: 5558
		private Vector3 offset;
	}
}
