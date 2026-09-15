using System;
using Photon.Pun;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000384 RID: 900
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	public abstract class BaseController : MonoBehaviour
	{
		// Token: 0x060016FE RID: 5886 RVA: 0x00075564 File Offset: 0x00073764
		protected virtual void OnEnable()
		{
			ChangePOV.CameraChanged += this.ChangePOV_CameraChanged;
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x00075578 File Offset: 0x00073778
		protected virtual void OnDisable()
		{
			ChangePOV.CameraChanged -= this.ChangePOV_CameraChanged;
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x0007558C File Offset: 0x0007378C
		protected virtual void ChangePOV_CameraChanged(Camera camera)
		{
			if (camera != this.ControllerCamera)
			{
				base.enabled = false;
				this.HideCamera(this.ControllerCamera);
				return;
			}
			this.ShowCamera(this.ControllerCamera);
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x000755BC File Offset: 0x000737BC
		protected virtual void Start()
		{
			if (base.GetComponent<PhotonView>().IsMine)
			{
				this.Init();
				this.SetCamera();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x000755DF File Offset: 0x000737DF
		protected virtual void Init()
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
			this.animator = base.GetComponent<Animator>();
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x000755F9 File Offset: 0x000737F9
		protected virtual void SetCamera()
		{
			this.camTrans = this.ControllerCamera.transform;
			this.camTrans.position += this.cameraDistance * base.transform.forward;
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x00075638 File Offset: 0x00073838
		protected virtual void UpdateAnimator(float h, float v)
		{
			bool flag = h != 0f || v != 0f;
			this.animator.SetBool("IsWalking", flag);
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x00075670 File Offset: 0x00073870
		protected virtual void FixedUpdate()
		{
			this.h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
			this.v = CrossPlatformInputManager.GetAxisRaw("Vertical");
			this.UpdateAnimator(this.h, this.v);
			this.Move(this.h, this.v);
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x000756C1 File Offset: 0x000738C1
		protected virtual void ShowCamera(Camera camera)
		{
			if (camera != null)
			{
				camera.gameObject.SetActive(true);
			}
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x000756D8 File Offset: 0x000738D8
		protected virtual void HideCamera(Camera camera)
		{
			if (camera != null)
			{
				camera.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001708 RID: 5896
		protected abstract void Move(float h, float v);

		// Token: 0x04001589 RID: 5513
		public Camera ControllerCamera;

		// Token: 0x0400158A RID: 5514
		protected Rigidbody rigidBody;

		// Token: 0x0400158B RID: 5515
		protected Animator animator;

		// Token: 0x0400158C RID: 5516
		protected Transform camTrans;

		// Token: 0x0400158D RID: 5517
		private float h;

		// Token: 0x0400158E RID: 5518
		private float v;

		// Token: 0x0400158F RID: 5519
		[SerializeField]
		protected float speed = 5f;

		// Token: 0x04001590 RID: 5520
		[SerializeField]
		private float cameraDistance;
	}
}
