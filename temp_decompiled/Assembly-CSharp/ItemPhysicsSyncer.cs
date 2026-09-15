using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000154 RID: 340
public class ItemPhysicsSyncer : PhotonBinaryStreamSerializer<ItemPhysicsSyncData>
{
	// Token: 0x06000AF3 RID: 2803 RVA: 0x0003A575 File Offset: 0x00038775
	protected override void Awake()
	{
		base.Awake();
		this.m_photonView = base.GetComponent<PhotonView>();
		this.m_item = base.GetComponent<Item>();
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0003A595 File Offset: 0x00038795
	public void ForceSyncForFrames(int frames = 10)
	{
		this.forceSyncFrames = frames;
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x0003A5A0 File Offset: 0x000387A0
	private void FixedUpdate()
	{
		Rigidbody rig = this.m_item.rig;
		if (rig == null)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (this.m_photonView.IsMine)
		{
			return;
		}
		if (this.RemoteValue.IsNone)
		{
			if (this.debug)
			{
				Debug.Log("NO REMOTE VALUE");
			}
			return;
		}
		if (!this.shouldSync)
		{
			if (this.debug)
			{
				Debug.Log("NOT SYNCING");
			}
			return;
		}
		if (this.m_item.itemState != ItemState.Ground)
		{
			return;
		}
		if (this.m_lastPos.IsNone)
		{
			if (this.debug)
			{
				Debug.Log("NO LAST POSITION");
			}
			return;
		}
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackage += Time.fixedDeltaTime * 0.6f;
		float num2 = (float)((double)this.sinceLastPackage / num);
		ItemPhysicsSyncData value = this.RemoteValue.Value;
		Vector3 vector = value.position;
		Vector3 vector2 = Vector3.Lerp(this.m_lastPos.Value, vector, num2);
		Vector3 vector3 = vector2 - rig.position;
		this.lastRecievedPosition = vector2;
		rig.MovePosition(rig.position + vector3 * 0.5f);
		if (this.debug)
		{
			Debug.Log(string.Format("Rotating from {0} to {1}", rig.rotation, value.rotation));
		}
		rig.MoveRotation(Quaternion.RotateTowards(rig.rotation, value.rotation, Time.fixedDeltaTime * this.maxAngleChangePerSecond));
		if (this.debug)
		{
			string text = "MOVING TO POSITION ";
			Vector3 vector4 = vector2;
			Debug.Log(text + vector4.ToString());
		}
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x06000AF6 RID: 2806 RVA: 0x0003A74D File Offset: 0x0003894D
	protected virtual float maxAngleChangePerSecond
	{
		get
		{
			return 90f;
		}
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x0003A754 File Offset: 0x00038954
	public void ResetRecievedData()
	{
		this.lastRecievedPosition = base.transform.position;
		this.RemoteValue = Optionable<ItemPhysicsSyncData>.None;
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0003A774 File Offset: 0x00038974
	public override ItemPhysicsSyncData GetDataToWrite()
	{
		ItemPhysicsSyncData itemPhysicsSyncData = default(ItemPhysicsSyncData);
		Rigidbody rig = this.m_item.rig;
		if (rig != null)
		{
			itemPhysicsSyncData.linearVelocity = rig.linearVelocity;
			itemPhysicsSyncData.angularVelocity = rig.angularVelocity;
			itemPhysicsSyncData.position = rig.position;
			itemPhysicsSyncData.rotation = rig.rotation;
			if (this.debug)
			{
				string text = "SENDING POSITION ";
				float3 position = itemPhysicsSyncData.position;
				Debug.Log(text + position.ToString());
			}
		}
		return itemPhysicsSyncData;
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0003A810 File Offset: 0x00038A10
	public override bool ShouldSendData()
	{
		if (this.forceSyncFrames > 0 && this.m_item.itemState == ItemState.Ground)
		{
			this.forceSyncFrames--;
			return true;
		}
		return this.shouldSync && (!this.m_item.rig.isKinematic && !this.m_item.rig.IsSleeping()) && this.m_item.itemState == ItemState.Ground;
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0003A884 File Offset: 0x00038A84
	public override void OnDataReceived(ItemPhysicsSyncData data)
	{
		if (this.debug)
		{
			Debug.Log("RECIEVED DATA 1");
		}
		base.OnDataReceived(data);
		Rigidbody rig = this.m_item.rig;
		if (rig == null)
		{
			return;
		}
		if (this.m_item.itemState != ItemState.Ground)
		{
			return;
		}
		if (!this.shouldSync)
		{
			if (this.debug)
			{
				Debug.Log("SHOULDSYNC OFF");
			}
			return;
		}
		if (rig.isKinematic)
		{
			return;
		}
		if (this.debug)
		{
			Debug.Log("RECIEVED DATA 2");
		}
		this.m_lastPos = Optionable<Vector3>.Some(rig.position);
		rig.linearVelocity = data.linearVelocity;
		rig.angularVelocity = data.angularVelocity;
		this.lastRecievedLinearVelocity = data.linearVelocity;
		this.lastRecievedAngularVelocity = data.angularVelocity;
	}

	// Token: 0x04000A31 RID: 2609
	private Item m_item;

	// Token: 0x04000A32 RID: 2610
	private PhotonView m_photonView;

	// Token: 0x04000A33 RID: 2611
	private Optionable<Vector3> m_lastPos;

	// Token: 0x04000A34 RID: 2612
	public bool debug;

	// Token: 0x04000A35 RID: 2613
	[SerializeField]
	internal bool shouldSync = true;

	// Token: 0x04000A36 RID: 2614
	private int forceSyncFrames;

	// Token: 0x04000A37 RID: 2615
	[SerializeField]
	private Vector3 lastRecievedLinearVelocity;

	// Token: 0x04000A38 RID: 2616
	[SerializeField]
	private Vector3 lastRecievedAngularVelocity;

	// Token: 0x04000A39 RID: 2617
	[SerializeField]
	private Vector3 lastRecievedPosition;
}
