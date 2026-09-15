using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000158 RID: 344
public class PhysicsSyncer : PhotonBinaryStreamSerializer<ItemPhysicsSyncData>
{
	// Token: 0x06000B09 RID: 2825 RVA: 0x0003AC29 File Offset: 0x00038E29
	protected override void Awake()
	{
		base.Awake();
		this.rig = base.GetComponent<Rigidbody>();
		this.m_photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0003AC49 File Offset: 0x00038E49
	public void ForceSyncForFrames(int frames = 10)
	{
		this.forceSyncFrames = frames;
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0003AC54 File Offset: 0x00038E54
	private void FixedUpdate()
	{
		if (this.rig == null)
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
		Vector3 vector3 = vector2 - this.rig.position;
		this.lastRecievedPosition = vector2;
		if (this.debug)
		{
			Debug.Log(string.Format("Position before move: {0}", this.rig.position));
		}
		this.rig.MovePosition(this.rig.position + vector3 * 0.5f);
		this.rig.MoveRotation(Quaternion.RotateTowards(this.rig.rotation, value.rotation, Time.fixedDeltaTime * this.maxAngleChangePerSecond));
		if (this.debug)
		{
			string text = "MOVING TO POSITION ";
			Vector3 vector4 = vector2;
			Debug.Log(text + vector4.ToString());
		}
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0003ADFD File Offset: 0x00038FFD
	public void ResetRecievedData()
	{
		this.lastRecievedPosition = base.transform.position;
		this.RemoteValue = Optionable<ItemPhysicsSyncData>.None;
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x0003AE1C File Offset: 0x0003901C
	public override ItemPhysicsSyncData GetDataToWrite()
	{
		ItemPhysicsSyncData itemPhysicsSyncData = default(ItemPhysicsSyncData);
		if (this.rig != null)
		{
			itemPhysicsSyncData.linearVelocity = this.rig.linearVelocity;
			itemPhysicsSyncData.angularVelocity = this.rig.angularVelocity;
			itemPhysicsSyncData.position = this.rig.position;
			itemPhysicsSyncData.rotation = this.rig.rotation;
			if (this.debug)
			{
				string text = "SENDING POSITION ";
				float3 position = itemPhysicsSyncData.position;
				Debug.Log(text + position.ToString());
			}
		}
		return itemPhysicsSyncData;
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x0003AEC5 File Offset: 0x000390C5
	public override bool ShouldSendData()
	{
		return true;
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x0003AEC8 File Offset: 0x000390C8
	public override void OnDataReceived(ItemPhysicsSyncData data)
	{
		if (this.debug)
		{
			Debug.Log("RECIEVED DATA 1");
		}
		base.OnDataReceived(data);
		if (this.rig == null)
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
		if (this.rig.isKinematic)
		{
			return;
		}
		if (this.debug)
		{
			Debug.Log("RECIEVED DATA 2");
		}
		this.m_lastPos = Optionable<Vector3>.Some(this.rig.position);
		this.rig.linearVelocity = data.linearVelocity;
		this.rig.angularVelocity = data.angularVelocity;
		this.lastRecievedLinearVelocity = data.linearVelocity;
		this.lastRecievedAngularVelocity = data.angularVelocity;
	}

	// Token: 0x04000A43 RID: 2627
	private PhotonView m_photonView;

	// Token: 0x04000A44 RID: 2628
	private Optionable<Vector3> m_lastPos;

	// Token: 0x04000A45 RID: 2629
	private Rigidbody rig;

	// Token: 0x04000A46 RID: 2630
	public bool debug;

	// Token: 0x04000A47 RID: 2631
	[SerializeField]
	internal bool shouldSync = true;

	// Token: 0x04000A48 RID: 2632
	private int forceSyncFrames;

	// Token: 0x04000A49 RID: 2633
	public float maxAngleChangePerSecond = 180f;

	// Token: 0x04000A4A RID: 2634
	[SerializeField]
	private Vector3 lastRecievedLinearVelocity;

	// Token: 0x04000A4B RID: 2635
	[SerializeField]
	private Vector3 lastRecievedAngularVelocity;

	// Token: 0x04000A4C RID: 2636
	[SerializeField]
	private Vector3 lastRecievedPosition;
}
