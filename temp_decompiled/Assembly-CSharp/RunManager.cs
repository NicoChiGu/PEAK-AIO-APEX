using System;
using System.Collections;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200017B RID: 379
public class RunManager : MonoBehaviourPunCallbacks
{
	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000BFC RID: 3068 RVA: 0x000402B4 File Offset: 0x0003E4B4
	private bool shouldCheckGameEnd
	{
		get
		{
			return this.runStarted && this.timerActive && this._timeWhenEndGameCheckNeeded < Time.time && this._timeWhenEndGameCheckNeeded > this._timeEndGameLastChecked;
		}
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x000402E3 File Offset: 0x0003E4E3
	private void Awake()
	{
		RunManager.Instance = this;
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x000402EB File Offset: 0x0003E4EB
	private IEnumerator Start()
	{
		this.runStarted = false;
		this.timeSinceRunStarted = 0f;
		while (!NetCode.Session.InRoom || !Character.localCharacter || LoadingScreenHandler.loading)
		{
			yield return null;
		}
		Debug.Log("RUN STARTED");
		this.StartRun();
		yield return new WaitForSeconds(2f);
		if (NetCode.Session.IsHost)
		{
			base.photonView.RPC("RPC_SyncTime", 0, new object[] { 0f, true });
		}
		yield break;
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x000402FA File Offset: 0x0003E4FA
	private void Update()
	{
		if (this.timerActive)
		{
			this.timeSinceRunStarted += Time.deltaTime;
		}
		if (this.shouldCheckGameEnd)
		{
			this.CheckForGameEnd();
		}
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x00040324 File Offset: 0x0003E524
	public void StartRun()
	{
		this.runStarted = true;
		Singleton<AchievementManager>.Instance.InitRunBasedValues();
		NetCode.RoomEvents.PlayerLeft += new Action(this.OnPlayersChanged);
		NetCode.RoomEvents.RoomOwnerChanged += new Action(this.OnPlayersChanged);
		this._timeEndGameLastChecked = 0f;
		this._timeWhenEndGameCheckNeeded = Time.time + 2f;
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x0004038A File Offset: 0x0003E58A
	private void DebugCurrentTime()
	{
		Debug.Log(this.timeSinceRunStarted);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x0004039C File Offset: 0x0003E59C
	private void ScheduleNextEndGameCheck()
	{
		if (this._timeWhenEndGameCheckNeeded < Time.time && !this.shouldCheckGameEnd)
		{
			this._timeWhenEndGameCheckNeeded = Time.time + 2f;
		}
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x000403C4 File Offset: 0x0003E5C4
	private void OnPlayersChanged()
	{
		this.ScheduleNextEndGameCheck();
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x000403CC File Offset: 0x0003E5CC
	private void CheckForGameEnd()
	{
		this._timeEndGameLastChecked = Time.time;
		if (Character.localCharacter != null)
		{
			Character.localCharacter.CheckEndGame();
		}
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x000403F0 File Offset: 0x0003E5F0
	internal void SyncTimeMaster()
	{
		if (NetCode.Session.IsHost)
		{
			base.photonView.RPC("RPC_SyncTime", 0, new object[] { this.timeSinceRunStarted, this.timerActive });
		}
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x0004043C File Offset: 0x0003E63C
	internal void EndGame()
	{
		NetCode.RoomEvents.PlayerLeft -= new Action(this.OnPlayersChanged);
		NetCode.RoomEvents.RoomOwnerChanged -= new Action(this.OnPlayersChanged);
		if (NetCode.Session.IsHost)
		{
			base.photonView.RPC("RPC_SyncTime", 0, new object[] { this.timeSinceRunStarted, false });
		}
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x000404B0 File Offset: 0x0003E6B0
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (NetCode.Session.IsHost)
		{
			base.photonView.RPC("RPC_SyncTime", newPlayer, new object[] { this.timeSinceRunStarted, this.timerActive });
		}
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x000404FC File Offset: 0x0003E6FC
	[PunRPC]
	private void RPC_SyncTime(float time, bool timerActive)
	{
		Debug.Log(string.Format("Time synced: {0} timer active: {1}", time, timerActive));
		this.timeSinceRunStarted = time;
		this.timerActive = timerActive;
	}

	// Token: 0x04000B25 RID: 2853
	public float timeSinceRunStarted;

	// Token: 0x04000B26 RID: 2854
	private float _timeWhenEndGameCheckNeeded;

	// Token: 0x04000B27 RID: 2855
	private float _timeEndGameLastChecked = float.NegativeInfinity;

	// Token: 0x04000B28 RID: 2856
	private const float EndGameCheckDelay = 2f;

	// Token: 0x04000B29 RID: 2857
	private bool runStarted;

	// Token: 0x04000B2A RID: 2858
	private bool timerActive;

	// Token: 0x04000B2B RID: 2859
	public static RunManager Instance;
}
