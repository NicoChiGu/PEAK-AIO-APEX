using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class StickyItemComponent : ItemComponent
{
	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060003AE RID: 942 RVA: 0x00018590 File Offset: 0x00016790
	// (set) Token: 0x060003AF RID: 943 RVA: 0x00018598 File Offset: 0x00016798
	public Character stuckToCharacter { get; protected set; }

	// Token: 0x060003B0 RID: 944 RVA: 0x000185A1 File Offset: 0x000167A1
	public override void Awake()
	{
		base.Awake();
		this.physicsSyncer = base.GetComponent<ItemPhysicsSyncer>();
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x000185B8 File Offset: 0x000167B8
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (this.photonView.IsMine && this.stuckToCharacter)
		{
			this.photonView.RPC("RPC_StickToCharacterRemote", newPlayer, new object[]
			{
				this.stuckToCharacter.photonView.ViewID,
				(int)this.stuckToBodypart,
				this.stuckLocalOffset
			});
		}
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0001862C File Offset: 0x0001682C
	[PunRPC]
	public void RPC_StickToCharacterRemote(int characterViewID, int bodyPartType, Vector3 offset)
	{
		PhotonView photonView = PhotonNetwork.GetPhotonView(characterViewID);
		if (!photonView)
		{
			return;
		}
		Character component = photonView.GetComponent<Character>();
		if (component.IsLocal)
		{
			return;
		}
		Bodypart bodypart = component.GetBodypart((BodypartType)bodyPartType);
		if (bodypart == null)
		{
			return;
		}
		this.StickToCharacterLocal(component, bodypart, offset);
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00018674 File Offset: 0x00016874
	internal virtual void StickToCharacterLocal(Character character, Bodypart bodypart, Vector3 worldOffset)
	{
		if (this.item.itemState != ItemState.Ground)
		{
			return;
		}
		if (character == null)
		{
			return;
		}
		this.stuck = true;
		this.stuckToCharacter = character;
		this.stuckToTransform = bodypart.transform;
		this.item.rig.isKinematic = true;
		this.physicalCollider.isTrigger = true;
		this.item.rig.angularVelocity = Vector3.zero;
		this.item.rig.linearVelocity = Vector3.zero;
		Debug.Log(string.Format("Stuck to {0} with offset distance {1}", character.gameObject.name, worldOffset.magnitude));
		if (bodypart.partType == BodypartType.Foot_R || bodypart.partType == BodypartType.Foot_L)
		{
			worldOffset.y = Mathf.Max(worldOffset.y, 0f);
		}
		this.stuckToBodypart = bodypart.partType;
		this.stuckLocalOffset = this.stuckToTransform.InverseTransformVector(worldOffset);
		this.stuckLocalRotationOffset = this.stuckToTransform.rotation * Quaternion.Inverse(base.transform.rotation);
		this.physicsSyncer.shouldSync = false;
		if (!StickyItemComponent.ALL_STUCK_ITEMS.Contains(this))
		{
			StickyItemComponent.ALL_STUCK_ITEMS.Add(this);
		}
		character.refs.afflictions.UpdateWeight();
		if (character.IsLocal)
		{
			this.photonView.RPC("RPC_StickToCharacterRemote", 1, new object[]
			{
				character.photonView.ViewID,
				(int)bodypart.partType,
				worldOffset
			});
		}
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x0001880C File Offset: 0x00016A0C
	private void Unstick()
	{
		Character stuckToCharacter = this.stuckToCharacter;
		this.stuck = false;
		this.item.rig.isKinematic = false;
		this.physicalCollider.isTrigger = false;
		this.stuckToCharacter = null;
		this.stuckToTransform = null;
		StickyItemComponent.ALL_STUCK_ITEMS.Remove(this);
		this.physicsSyncer.shouldSync = true;
		if (stuckToCharacter)
		{
			stuckToCharacter.refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x00018882 File Offset: 0x00016A82
	private void OnDestroy()
	{
		StickyItemComponent.ALL_STUCK_ITEMS.Remove(this);
		if (this.stuckToCharacter)
		{
			this.stuckToCharacter.refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x000188B2 File Offset: 0x00016AB2
	private void Update()
	{
		if (this.stuck && this.item.itemState != ItemState.Ground)
		{
			this.Unstick();
		}
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x000188D0 File Offset: 0x00016AD0
	protected virtual void FixedUpdate()
	{
		if (this.stuck)
		{
			this.item.rig.MovePosition(this.stuckToTransform.TransformPoint(this.stuckLocalOffset));
			this.item.rig.MoveRotation(this.stuckToTransform.rotation * this.stuckLocalRotationOffset);
		}
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x0001892C File Offset: 0x00016B2C
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x04000402 RID: 1026
	public static List<StickyItemComponent> ALL_STUCK_ITEMS = new List<StickyItemComponent>();

	// Token: 0x04000404 RID: 1028
	public Vector3 stuckLocalOffset;

	// Token: 0x04000405 RID: 1029
	public BodypartType stuckToBodypart;

	// Token: 0x04000406 RID: 1030
	public Quaternion stuckLocalRotationOffset;

	// Token: 0x04000407 RID: 1031
	protected Transform stuckToTransform;

	// Token: 0x04000408 RID: 1032
	protected bool stuck;

	// Token: 0x04000409 RID: 1033
	public float throwChargeRequirement;

	// Token: 0x0400040A RID: 1034
	public int addWeightToStuckPlayer;

	// Token: 0x0400040B RID: 1035
	public int addThornsToStuckPlayer;

	// Token: 0x0400040C RID: 1036
	public Collider physicalCollider;

	// Token: 0x0400040D RID: 1037
	public float spherecastRadius;

	// Token: 0x0400040E RID: 1038
	protected ItemPhysicsSyncer physicsSyncer;

	// Token: 0x0400040F RID: 1039
	private RaycastHit sphereCastHit;
}
