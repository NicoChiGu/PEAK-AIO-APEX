using System;
using System.Collections;
using ExitGames.Client.Photon;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002B1 RID: 689
public class OrbFogHandler : Singleton<OrbFogHandler>, IInRoomCallbacks
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x060012C4 RID: 4804 RVA: 0x0005FA66 File Offset: 0x0005DC66
	// (set) Token: 0x060012C5 RID: 4805 RVA: 0x0005FA6E File Offset: 0x0005DC6E
	public int currentID { get; private set; }

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x060012C6 RID: 4806 RVA: 0x0005FA77 File Offset: 0x0005DC77
	public static bool IsFoggingCurrentSegment
	{
		get
		{
			return Singleton<OrbFogHandler>.Instance != null && (Singleton<OrbFogHandler>.Instance.isMoving || Singleton<OrbFogHandler>.Instance.hasArrived);
		}
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x0005FAA0 File Offset: 0x0005DCA0
	protected override void Awake()
	{
		base.Awake();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x0005FAB4 File Offset: 0x0005DCB4
	private void Start()
	{
		this.sphere = base.GetComponentInChildren<FogSphere>();
		this.origins = base.transform.root.GetComponentsInChildren<FogSphereOrigin>();
		this.InitNewSphere(this.origins[this.currentID]);
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x0005FAEB File Offset: 0x0005DCEB
	private void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x0005FAF3 File Offset: 0x0005DCF3
	private void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
		Shader.SetGlobalFloat("FakeMountainEnabled", 1f);
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x0005FB0C File Offset: 0x0005DD0C
	private void Update()
	{
		this.sphere != null;
		if (!this.hasArrived)
		{
			bool flag = this.currentID >= this.origins.Length || this.origins[this.currentID].disableFog;
			if (Ascents.fogEnabled && !flag)
			{
				if (this.isMoving)
				{
					this.Move();
				}
				else
				{
					this.WaitToMove();
				}
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.Sync();
		}
		this.ApplyMeshEffects();
		float num = Mathf.Lerp(1f, 5f, this.dispelFogAmount);
		this.currentCloseFog = Mathf.Lerp(this.currentCloseFog, num, Time.deltaTime * 1f);
		Shader.SetGlobalFloat("CloseDistanceMod", this.currentCloseFog);
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x0005FBD0 File Offset: 0x0005DDD0
	private void Sync()
	{
		this.syncCounter += Time.deltaTime;
		if (this.syncCounter > 5f)
		{
			this.syncCounter = 0f;
			this.photonView.RPC("RPCA_SyncFog", 1, new object[] { this.currentSize, this.isMoving });
		}
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x0005FC3A File Offset: 0x0005DE3A
	[PunRPC]
	public void RPCA_SyncFog(float s, bool moving)
	{
		this.currentSize = s;
		this.isMoving = moving;
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x0005FC4A File Offset: 0x0005DE4A
	[PunRPC]
	public void RPC_InitFog(int originId, float s, bool arrived, bool moving, PhotonMessageInfo info)
	{
		if (info.Sender.ActorNumber != NetCode.Session.HostId)
		{
			return;
		}
		this.SetFogOrigin(originId);
		this.currentSize = s;
		this.hasArrived = arrived;
		this.isMoving = moving;
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x0005FC82 File Offset: 0x0005DE82
	public IEnumerator WaitForFogCatchUp()
	{
		this.isMoving = true;
		while (this.currentSize > 30f && this.isMoving && !this.hasArrived)
		{
			this.currentSize = Mathf.Lerp(this.currentSize, 29.5f, Time.deltaTime);
			this.currentSize = Mathf.MoveTowards(this.currentSize, 29.5f, Time.deltaTime);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x0005FC91 File Offset: 0x0005DE91
	public IEnumerator WaitForReveal()
	{
		float c = 0f;
		float t = 5f;
		this.sphere.ENABLE = 1f;
		while (c < t)
		{
			c += Time.deltaTime;
			this.sphere.REVEAL_AMOUNT = this.fogRevealCurve.Evaluate(c / t);
			this.sphere.ENABLE = this.fogFadeCurve.Evaluate(c / t);
			yield return null;
		}
		this.sphere.REVEAL_AMOUNT = 1f;
		this.sphere.ENABLE = 0f;
		this.currentSize = 800f;
		yield break;
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x0005FCA0 File Offset: 0x0005DEA0
	public IEnumerator DisableFog()
	{
		float c = 0f;
		float t = 1f;
		while (c < t)
		{
			c += Time.deltaTime;
			this.sphere.ENABLE = 1f - c / t;
			yield return null;
		}
		this.sphere.ENABLE = 0f;
		this.sphere.REVEAL_AMOUNT = 0f;
		this.currentSize = 800f;
		yield break;
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x0005FCB0 File Offset: 0x0005DEB0
	private void Move()
	{
		this.sphere.REVEAL_AMOUNT = 0f;
		this.sphere.ENABLE = Mathf.MoveTowards(this.sphere.ENABLE, 1f, Time.deltaTime * 0.1f);
		this.currentSize -= this.speed * Time.deltaTime;
		if (this.currentSize <= 30f)
		{
			this.Stop();
		}
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x0005FD24 File Offset: 0x0005DF24
	private void Stop()
	{
		this.hasArrived = true;
		this.isMoving = false;
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x0005FD34 File Offset: 0x0005DF34
	private void WaitToMove()
	{
		this.currentWaitTime += Time.deltaTime;
		if ((this.PlayersHaveMovedOn() || this.TimeToMove()) && PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("StartMovingRPC", 0, Array.Empty<object>());
		}
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x0005FD80 File Offset: 0x0005DF80
	private bool TimeToMove()
	{
		return Ascents.currentAscent >= 0 && this.currentWaitTime > this.maxWaitTime && this.currentID > 0;
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x0005FDA8 File Offset: 0x0005DFA8
	private bool PlayersHaveMovedOn()
	{
		if (Character.AllCharacters.Count == 0)
		{
			return false;
		}
		if (Ascents.currentAscent < 0)
		{
			return false;
		}
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (!Character.AllCharacters[i].data.dead && (Character.AllCharacters[i].Center.y < this.currentStartHeight || Character.AllCharacters[i].Center.z < this.currentStartForward))
			{
				return false;
			}
		}
		Debug.Log("Players have moved on");
		return true;
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x0005FE40 File Offset: 0x0005E040
	private void ApplyMeshEffects()
	{
		this.sphere.currentSize = this.currentSize;
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x0005FE53 File Offset: 0x0005E053
	public void InitNewSphere(FogSphereOrigin newOrigin)
	{
		this.sphere.fogPoint = newOrigin.transform.position;
		this.currentSize = newOrigin.size;
		this.currentStartHeight = newOrigin.moveOnHeight;
		this.currentStartForward = newOrigin.moveOnForward;
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x0005FE8F File Offset: 0x0005E08F
	[PunRPC]
	public void StartMovingRPC()
	{
		this.currentWaitTime = 0f;
		this.hasArrived = false;
		this.isMoving = true;
		GUIManager.instance.TheFogRises();
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x0005FEB4 File Offset: 0x0005E0B4
	public void SetFogOrigin(int id)
	{
		this.currentID = id;
		if (this.currentID < this.origins.Length)
		{
			this.hasArrived = false;
			this.sphere.gameObject.SetActive(true);
			this.InitNewSphere(this.origins[this.currentID]);
			return;
		}
		this.hasArrived = true;
		Debug.Log("Last section, disabling fog sphere");
		this.sphere.gameObject.SetActive(false);
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x0005FF28 File Offset: 0x0005E128
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.photonView.RPC("RPC_InitFog", newPlayer, new object[] { this.currentID, this.currentSize, this.hasArrived, this.isMoving });
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x0005FF84 File Offset: 0x0005E184
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x0005FF86 File Offset: 0x0005E186
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x0005FF88 File Offset: 0x0005E188
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x0005FF8A File Offset: 0x0005E18A
	public void OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x04001175 RID: 4469
	public float speed = 0.3f;

	// Token: 0x04001176 RID: 4470
	public float maxWaitTime = 500f;

	// Token: 0x04001177 RID: 4471
	public float currentWaitTime;

	// Token: 0x04001178 RID: 4472
	public bool hasArrived;

	// Token: 0x04001179 RID: 4473
	public bool isMoving;

	// Token: 0x0400117A RID: 4474
	public float currentSize;

	// Token: 0x0400117B RID: 4475
	public float currentStartHeight;

	// Token: 0x0400117C RID: 4476
	public float currentStartForward;

	// Token: 0x0400117D RID: 4477
	public float dispelFogAmount;

	// Token: 0x0400117E RID: 4478
	private FogSphere sphere;

	// Token: 0x0400117F RID: 4479
	private FogSphereOrigin[] origins;

	// Token: 0x04001181 RID: 4481
	private float syncCounter;

	// Token: 0x04001182 RID: 4482
	private PhotonView photonView;

	// Token: 0x04001183 RID: 4483
	public AnimationCurve fogRevealCurve;

	// Token: 0x04001184 RID: 4484
	public AnimationCurve fogFadeCurve;

	// Token: 0x04001185 RID: 4485
	public float currentCloseFog = 1f;
}
