using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.PhotonUtility;

// Token: 0x02000285 RID: 645
public class LavaRising : Singleton<LavaRising>
{
	// Token: 0x1700011E RID: 286
	// (get) Token: 0x060011FC RID: 4604 RVA: 0x0005B32D File Offset: 0x0005952D
	// (set) Token: 0x060011FD RID: 4605 RVA: 0x0005B335 File Offset: 0x00059535
	public float timeTraveled { get; set; }

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x060011FE RID: 4606 RVA: 0x0005B33E File Offset: 0x0005953E
	// (set) Token: 0x060011FF RID: 4607 RVA: 0x0005B346 File Offset: 0x00059546
	public bool started { get; set; }

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06001200 RID: 4608 RVA: 0x0005B34F File Offset: 0x0005954F
	// (set) Token: 0x06001201 RID: 4609 RVA: 0x0005B357 File Offset: 0x00059557
	public bool ended { get; set; }

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06001202 RID: 4610 RVA: 0x0005B360 File Offset: 0x00059560
	// (set) Token: 0x06001203 RID: 4611 RVA: 0x0005B368 File Offset: 0x00059568
	public float secondsWaitedToStart { get; set; }

	// Token: 0x06001204 RID: 4612 RVA: 0x0005B371 File Offset: 0x00059571
	protected override void Awake()
	{
		base.Awake();
		if (this.debug)
		{
			this.initialWaitTime = this.debugInitialWaitTime;
			this.travelTime = this.debugTravelTime;
		}
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x0005B399 File Offset: 0x00059599
	private void Start()
	{
		this.startHeight = this.lava.transform.position.y;
		Debug.Log("Initialized lava height: " + this.startHeight.ToString());
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x0005B3D0 File Offset: 0x000595D0
	public override void OnDestroy()
	{
		base.OnDestroy();
		CustomCommands<CustomCommandType>.UnregisterListener(this.debugCommandHandle);
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x0005B3E4 File Offset: 0x000595E4
	private void Update()
	{
		if ((PhotonNetwork.IsMasterClient && Singleton<MapHandler>.Instance.GetCurrentSegment() == Segment.TheKiln) || Singleton<MapHandler>.Instance.GetCurrentSegment() == Segment.Peak)
		{
			bool flag = false;
			this.syncTime += Time.deltaTime;
			if (!this.started && this.secondsWaitedToStart <= 0f && Ascents.fogEnabled)
			{
				this.StartWaiting();
			}
			if (!this.started && this.secondsWaitedToStart > 0f)
			{
				this.secondsWaitedToStart += Time.deltaTime;
				if (this.secondsWaitedToStart > this.initialWaitTime && Ascents.fogEnabled)
				{
					flag = true;
					this.started = true;
				}
			}
			if (this.syncTime > 15f)
			{
				this.syncTime = 0f;
				flag = true;
			}
			if (flag)
			{
				Debug.Log("Syncing Lava Rising to others...");
				GameUtils.instance.SyncLava(this.started, this.ended, this.timeTraveled, this.secondsWaitedToStart);
			}
		}
		if (this.started && !this.ended)
		{
			if (!this.shownLavaRisingMessage)
			{
				GUIManager.instance.TheLavaRises();
				GamefeelHandler.instance.AddPerlinShake(5f, 3f, 15f);
				this.shownLavaRisingMessage = true;
				Debug.Log("Lava rising started.");
			}
			this.timeTraveled += Time.deltaTime;
			float num = Mathf.Lerp(this.startHeight, this.topTransform.position.y, this.timeTraveled / this.travelTime);
			this.lava.MovePosition(new Vector3(this.lava.transform.position.x, num, this.lava.transform.position.z));
			if (this.timeTraveled > this.travelTime)
			{
				this.EndRising();
			}
		}
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x0005B5B8 File Offset: 0x000597B8
	public void RecieveLavaData(bool started, bool ended, float time, float timeWaited)
	{
		this.started = started;
		this.ended = ended;
		this.timeTraveled = time;
		this.secondsWaitedToStart = timeWaited;
		Debug.Log(string.Format("Handle Lava Rising package: started: {0}, ended: {1}, seconds waited: {2}, time traveled: {3} starting position: {4} total time: {5}", new object[] { started, ended, this.secondsWaitedToStart, this.timeTraveled, this.startHeight, this.travelTime }));
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x0005B641 File Offset: 0x00059841
	public void StartWaiting()
	{
		if (this.secondsWaitedToStart > 0f)
		{
			Debug.LogError("Tried to start waiting for lava rising but already rising!");
			return;
		}
		Debug.Log("Starting wait for lava rising");
		this.secondsWaitedToStart = Time.deltaTime;
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x0005B670 File Offset: 0x00059870
	private void EndRising()
	{
		Debug.Log("Ending lava rising.");
		this.ended = true;
	}

	// Token: 0x04001085 RID: 4229
	public Rigidbody lava;

	// Token: 0x04001086 RID: 4230
	public Transform topTransform;

	// Token: 0x04001087 RID: 4231
	public float initialWaitTime = 1f;

	// Token: 0x04001088 RID: 4232
	public float travelTime = 60f;

	// Token: 0x0400108D RID: 4237
	public bool debug;

	// Token: 0x0400108E RID: 4238
	public float debugInitialWaitTime = 1f;

	// Token: 0x0400108F RID: 4239
	public float debugTravelTime = 60f;

	// Token: 0x04001090 RID: 4240
	private bool shownLavaRisingMessage;

	// Token: 0x04001091 RID: 4241
	private ListenerHandle debugCommandHandle;

	// Token: 0x04001092 RID: 4242
	private float syncTime;

	// Token: 0x04001093 RID: 4243
	[SerializeField]
	private float startHeight;
}
