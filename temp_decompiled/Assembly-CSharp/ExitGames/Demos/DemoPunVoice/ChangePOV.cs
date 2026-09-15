using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000386 RID: 902
	public class ChangePOV : MonoBehaviour, IMatchmakingCallbacks
	{
		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06001710 RID: 5904 RVA: 0x000757C0 File Offset: 0x000739C0
		// (remove) Token: 0x06001711 RID: 5905 RVA: 0x000757F4 File Offset: 0x000739F4
		public static event ChangePOV.OnCameraChanged CameraChanged;

		// Token: 0x06001712 RID: 5906 RVA: 0x00075827 File Offset: 0x00073A27
		private void OnEnable()
		{
			CharacterInstantiation.CharacterInstantiated += this.OnCharacterInstantiated;
			PhotonNetwork.AddCallbackTarget(this);
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x00075840 File Offset: 0x00073A40
		private void OnDisable()
		{
			CharacterInstantiation.CharacterInstantiated -= this.OnCharacterInstantiated;
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x0007585C File Offset: 0x00073A5C
		private void Start()
		{
			this.defaultCamera = Camera.main;
			this.initialCameraPosition = new Vector3(this.defaultCamera.transform.position.x, this.defaultCamera.transform.position.y, this.defaultCamera.transform.position.z);
			this.initialCameraRotation = new Quaternion(this.defaultCamera.transform.rotation.x, this.defaultCamera.transform.rotation.y, this.defaultCamera.transform.rotation.z, this.defaultCamera.transform.rotation.w);
			this.FirstPersonCamActivator.onClick.AddListener(new UnityAction(this.FirstPersonMode));
			this.ThirdPersonCamActivator.onClick.AddListener(new UnityAction(this.ThirdPersonMode));
			this.OrthographicCamActivator.onClick.AddListener(new UnityAction(this.OrthographicMode));
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x00075974 File Offset: 0x00073B74
		private void OnCharacterInstantiated(GameObject character)
		{
			this.firstPersonController = character.GetComponent<FirstPersonController>();
			this.firstPersonController.enabled = false;
			this.thirdPersonController = character.GetComponent<ThirdPersonController>();
			this.thirdPersonController.enabled = false;
			this.orthographicController = character.GetComponent<OrthographicController>();
			this.ButtonsHolder.SetActive(true);
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x000759C9 File Offset: 0x00073BC9
		private void FirstPersonMode()
		{
			this.ToggleMode(this.firstPersonController);
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x000759D7 File Offset: 0x00073BD7
		private void ThirdPersonMode()
		{
			this.ToggleMode(this.thirdPersonController);
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x000759E5 File Offset: 0x00073BE5
		private void OrthographicMode()
		{
			this.ToggleMode(this.orthographicController);
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x000759F4 File Offset: 0x00073BF4
		private void ToggleMode(BaseController controller)
		{
			if (controller == null)
			{
				return;
			}
			if (controller.ControllerCamera == null)
			{
				return;
			}
			controller.ControllerCamera.gameObject.SetActive(true);
			controller.enabled = true;
			this.FirstPersonCamActivator.interactable = !(controller == this.firstPersonController);
			this.ThirdPersonCamActivator.interactable = !(controller == this.thirdPersonController);
			this.OrthographicCamActivator.interactable = !(controller == this.orthographicController);
			this.BroadcastChange(controller.ControllerCamera);
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x00075A8C File Offset: 0x00073C8C
		private void BroadcastChange(Camera camera)
		{
			if (camera == null)
			{
				return;
			}
			if (ChangePOV.CameraChanged != null)
			{
				ChangePOV.CameraChanged(camera);
			}
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x00075AAA File Offset: 0x00073CAA
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x00075AAC File Offset: 0x00073CAC
		public void OnCreatedRoom()
		{
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x00075AAE File Offset: 0x00073CAE
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x00075AB0 File Offset: 0x00073CB0
		public void OnJoinedRoom()
		{
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x00075AB2 File Offset: 0x00073CB2
		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x00075AB4 File Offset: 0x00073CB4
		public void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x00075AB8 File Offset: 0x00073CB8
		public void OnLeftRoom()
		{
			if (this.defaultCamera)
			{
				this.defaultCamera.gameObject.SetActive(true);
			}
			this.FirstPersonCamActivator.interactable = true;
			this.ThirdPersonCamActivator.interactable = true;
			this.OrthographicCamActivator.interactable = false;
			this.defaultCamera.transform.position = this.initialCameraPosition;
			this.defaultCamera.transform.rotation = this.initialCameraRotation;
			this.ButtonsHolder.SetActive(false);
		}

		// Token: 0x04001593 RID: 5523
		private FirstPersonController firstPersonController;

		// Token: 0x04001594 RID: 5524
		private ThirdPersonController thirdPersonController;

		// Token: 0x04001595 RID: 5525
		private OrthographicController orthographicController;

		// Token: 0x04001596 RID: 5526
		private Vector3 initialCameraPosition;

		// Token: 0x04001597 RID: 5527
		private Quaternion initialCameraRotation;

		// Token: 0x04001598 RID: 5528
		private Camera defaultCamera;

		// Token: 0x04001599 RID: 5529
		[SerializeField]
		private GameObject ButtonsHolder;

		// Token: 0x0400159A RID: 5530
		[SerializeField]
		private Button FirstPersonCamActivator;

		// Token: 0x0400159B RID: 5531
		[SerializeField]
		private Button ThirdPersonCamActivator;

		// Token: 0x0400159C RID: 5532
		[SerializeField]
		private Button OrthographicCamActivator;

		// Token: 0x0200052B RID: 1323
		// (Invoke) Token: 0x06001DD2 RID: 7634
		public delegate void OnCameraChanged(Camera newCamera);
	}
}
