using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000029 RID: 41
[DefaultExecutionOrder(500)]
public class MainCameraMovement : Singleton<MainCameraMovement>
{
	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060002E1 RID: 737 RVA: 0x0001405E File Offset: 0x0001225E
	// (set) Token: 0x060002E2 RID: 738 RVA: 0x00014065 File Offset: 0x00012265
	public static Character specCharacter { get; protected set; }

	// Token: 0x060002E3 RID: 739 RVA: 0x00014070 File Offset: 0x00012270
	private void Start()
	{
		this.cam = base.GetComponent<MainCamera>();
		this.currentFov = this.cam.cam.fieldOfView;
		this.fovSetting = GameHandler.Instance.SettingsHandler.GetSetting<FovSetting>();
		this.extraFovSetting = GameHandler.Instance.SettingsHandler.GetSetting<ExtraFovSetting>();
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060002E4 RID: 740 RVA: 0x000140C9 File Offset: 0x000122C9
	public static bool IsSpectating
	{
		get
		{
			return Singleton<MainCameraMovement>.Instance.isSpectating;
		}
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x000140D8 File Offset: 0x000122D8
	private void LateUpdate()
	{
		if (this.isGodCam)
		{
			this.godcam.Update(base.transform, this.cam);
			return;
		}
		this.UpdateVariables();
		if (this.cam.camOverride)
		{
			this.OverrideCam();
			return;
		}
		if (Character.localCharacter && Character.localCharacter.data.fullyPassedOut)
		{
			this.Spectate();
			if (!this.isSpectating)
			{
				this.StartSpectate();
			}
			return;
		}
		if (this.isSpectating)
		{
			this.StopSpectating();
		}
		MainCameraMovement.specCharacter = null;
		this.CharacterCam();
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x00014170 File Offset: 0x00012370
	private void StartSpectate()
	{
		this.isSpectating = true;
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00014179 File Offset: 0x00012379
	private void StopSpectating()
	{
		this.isSpectating = false;
		MainCameraMovement.specCharacter = null;
		if (Character.localCharacter.Ghost != null)
		{
			PhotonNetwork.Destroy(Character.localCharacter.Ghost.gameObject);
		}
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x000141AE File Offset: 0x000123AE
	private void UpdateVariables()
	{
		this.sinceSwitch += Time.deltaTime;
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x000141C4 File Offset: 0x000123C4
	private void Spectate()
	{
		if (!this.HandleSpecSelection())
		{
			return;
		}
		PlayerGhost playerGhost = Character.localCharacter.Ghost;
		if (playerGhost == null && Character.localCharacter.data.dead)
		{
			playerGhost = PhotonNetwork.Instantiate("PlayerGhost", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PlayerGhost>();
			playerGhost.m_view.RPC("RPCA_InitGhost", 3, new object[]
			{
				Character.localCharacter.refs.view,
				MainCameraMovement.specCharacter.refs.view
			});
		}
		if (playerGhost && playerGhost.m_target != MainCameraMovement.specCharacter)
		{
			playerGhost.m_view.RPC("RPCA_SetTarget", 3, new object[] { MainCameraMovement.specCharacter.refs.view });
		}
		base.transform.position = MainCameraMovement.specCharacter.GetSpectatePosition();
		Vector3 vector = MainCameraMovement.specCharacter.data.lookDirection;
		if (Character.localCharacter != null)
		{
			vector = Character.localCharacter.data.lookDirection;
		}
		base.transform.rotation = Quaternion.LookRotation(vector);
		this.spectateZoom += Character.localCharacter.input.scrollInput * -0.5f;
		if (Character.localCharacter.input.scrollForwardIsPressed)
		{
			this.spectateZoom -= this.spectateZoomButtonSpeed * Time.deltaTime;
		}
		else if (Character.localCharacter.input.scrollBackwardIsPressed)
		{
			this.spectateZoom += this.spectateZoomButtonSpeed * Time.deltaTime;
		}
		this.spectateZoom = Mathf.Clamp(this.spectateZoom, this.spectateZoomMin, this.spectateZoomMax);
		Character.localCharacter.data.spectateZoom = Mathf.Lerp(Character.localCharacter.data.spectateZoom, this.spectateZoom, Time.deltaTime * 5f);
		base.transform.position += base.transform.TransformDirection(new Vector3(0f, 0.5f, -1f * Character.localCharacter.data.spectateZoom));
	}

	// Token: 0x060002EA RID: 746 RVA: 0x000143FC File Offset: 0x000125FC
	private bool HandleSpecSelection()
	{
		if (MainCameraMovement.specCharacter && !MainCameraMovement.specCharacter.data.canBeSpectated)
		{
			MainCameraMovement.specCharacter = null;
		}
		if (MainCameraMovement.specCharacter == null)
		{
			this.GetSpecPlayer();
		}
		if (MainCameraMovement.specCharacter == null)
		{
			return false;
		}
		if (MainCameraMovement.specCharacter == Character.localCharacter)
		{
			return true;
		}
		if (Character.localCharacter.input.spectateLeftWasPressed && this.sinceSwitch > 0.2f)
		{
			Transitions.instance.PlayTransition(TransitionType.SpectateSwitch, new Action(this.SwapSpecPlayerLeft), 5f, 5f);
			this.sinceSwitch = 0f;
		}
		if (Character.localCharacter.input.spectateRightWasPressed && this.sinceSwitch > 0.2f)
		{
			Transitions.instance.PlayTransition(TransitionType.SpectateSwitch, new Action(this.SwapSpecPlayerRight), 5f, 5f);
			this.sinceSwitch = 0f;
		}
		return !(MainCameraMovement.specCharacter == null);
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00014500 File Offset: 0x00012700
	public void SwapSpecPlayerLeft()
	{
		this.SwapSpecPlayer(-1);
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00014509 File Offset: 0x00012709
	public void SwapSpecPlayerRight()
	{
		this.SwapSpecPlayer(1);
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00014514 File Offset: 0x00012714
	private void SwapSpecPlayer(int add)
	{
		List<Character> list = new List<Character>();
		foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
		{
			if (character.data.canBeSpectated && !character.isBot)
			{
				list.Add(character);
			}
		}
		if (list.Count == 0)
		{
			MainCameraMovement.specCharacter = null;
			return;
		}
		if (MainCameraMovement.specCharacter == null)
		{
			Debug.LogError("WE FOUND IT");
			return;
		}
		int num = MainCameraMovement.specCharacter.GetPlayerListID(list);
		num += add;
		if (num < 0)
		{
			num = list.Count - 1;
		}
		if (num >= list.Count)
		{
			num = 0;
		}
		MainCameraMovement.specCharacter = list[num];
	}

	// Token: 0x060002EE RID: 750 RVA: 0x000145DC File Offset: 0x000127DC
	private void GetSpecPlayer()
	{
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		if (allPlayerCharacters.Count == 0)
		{
			return;
		}
		if (Character.localCharacter.data.canBeSpectated)
		{
			MainCameraMovement.specCharacter = Character.localCharacter;
			return;
		}
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			if (allPlayerCharacters[i].data.canBeSpectated && !allPlayerCharacters[i].isBot)
			{
				MainCameraMovement.specCharacter = allPlayerCharacters[i];
				return;
			}
		}
	}

	// Token: 0x060002EF RID: 751 RVA: 0x00014654 File Offset: 0x00012854
	private void CharacterCam()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.cam.cam.fieldOfView = this.GetFov();
		if (Character.localCharacter == null)
		{
			return;
		}
		if (Character.localCharacter == null)
		{
			return;
		}
		if (Character.localCharacter.data.lookDirection != Vector3.zero)
		{
			base.transform.rotation = Quaternion.LookRotation(Character.localCharacter.data.lookDirection);
			float num = 1f - Character.localCharacter.data.currentRagdollControll;
			if (num > this.ragdollCam)
			{
				this.ragdollCam = Mathf.Lerp(this.ragdollCam, num, Time.deltaTime * 5f);
			}
			else
			{
				this.ragdollCam = Mathf.Lerp(this.ragdollCam, num, Time.deltaTime * 0.5f);
			}
			this.physicsRot = Quaternion.Lerp(this.physicsRot, Character.localCharacter.GetBodypartRig(BodypartType.Head).transform.rotation, Time.deltaTime * 10f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.physicsRot, this.ragdollCam);
			base.transform.Rotate(GamefeelHandler.instance.GetRotation(), 0);
		}
		Vector3 cameraPos = Character.localCharacter.GetCameraPos(this.GetHeadOffset());
		Vector3 position = Character.localCharacter.GetBodypart(BodypartType.Torso).transform.position;
		this.targetPlayerPovPosition = Vector3.Lerp(cameraPos, position, this.ragdollCam);
		if (Vector3.Distance(base.transform.position, this.targetPlayerPovPosition) > this.characterPovMaxDistance)
		{
			base.transform.position = this.targetPlayerPovPosition + (base.transform.position - this.targetPlayerPovPosition).normalized * this.characterPovMaxDistance;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPlayerPovPosition, Time.deltaTime * this.characterPovLerpRate);
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x0001486C File Offset: 0x00012A6C
	private void OverrideCam()
	{
		this.cam.cam.fieldOfView = this.cam.camOverride.fov;
		this.cam.transform.position = this.cam.camOverride.transform.position;
		this.cam.transform.rotation = this.cam.camOverride.transform.rotation;
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x000148E4 File Offset: 0x00012AE4
	private float GetHeadOffset()
	{
		if (Character.localCharacter.data.isClimbing)
		{
			this.currentForwardOffset = Mathf.Lerp(this.currentForwardOffset, -0.5f, Time.deltaTime * 5f);
		}
		else
		{
			this.currentForwardOffset = Mathf.Lerp(this.currentForwardOffset, -0.5f, Time.deltaTime * 5f);
		}
		return this.currentForwardOffset;
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x0001494C File Offset: 0x00012B4C
	private float GetFov()
	{
		float num = this.fovSetting.Value;
		if (num < 60f)
		{
			num = 70f;
		}
		if (Character.localCharacter == null)
		{
			return num;
		}
		this.currentFov = Mathf.Lerp(this.currentFov, num + (Character.localCharacter.data.isClimbing ? this.extraFovSetting.Value : 0f), Time.deltaTime * 5f);
		return this.currentFov;
	}

	// Token: 0x040002AF RID: 687
	private float currentFov;

	// Token: 0x040002B0 RID: 688
	private float currentForwardOffset = 0.5f;

	// Token: 0x040002B1 RID: 689
	private MainCamera cam;

	// Token: 0x040002B2 RID: 690
	private FovSetting fovSetting;

	// Token: 0x040002B3 RID: 691
	private ExtraFovSetting extraFovSetting;

	// Token: 0x040002B5 RID: 693
	public float characterPovLerpRate = 5f;

	// Token: 0x040002B6 RID: 694
	public float characterPovMaxDistance = 0.1f;

	// Token: 0x040002B7 RID: 695
	private bool isSpectating;

	// Token: 0x040002B8 RID: 696
	internal bool isGodCam;

	// Token: 0x040002B9 RID: 697
	public GodCam godcam;

	// Token: 0x040002BA RID: 698
	private float spectateZoom = 2f;

	// Token: 0x040002BB RID: 699
	public float spectateZoomMin = 1f;

	// Token: 0x040002BC RID: 700
	public float spectateZoomMax = 5f;

	// Token: 0x040002BD RID: 701
	public float spectateZoomButtonSpeed = 30f;

	// Token: 0x040002BE RID: 702
	private float sinceSwitch;

	// Token: 0x040002BF RID: 703
	private float ragdollCam;

	// Token: 0x040002C0 RID: 704
	private Quaternion physicsRot;

	// Token: 0x040002C1 RID: 705
	private Vector3 targetPlayerPovPosition;
}
