using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000211 RID: 529
[DefaultExecutionOrder(1000002)]
public class BingBongForceAbilities : MonoBehaviour
{
	// Token: 0x06000F9A RID: 3994 RVA: 0x0004D925 File Offset: 0x0004BB25
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle || this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			this.DoEffect();
		}
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x0004D94B File Offset: 0x0004BB4B
	[PunRPC]
	public void RPCA_BingBongInitObj(int bingbongID)
	{
		this.bingbong = PhotonView.Find(bingbongID).transform;
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x0004D95E File Offset: 0x0004BB5E
	private void LateUpdate()
	{
		base.transform.position = this.bingbong.position;
		base.transform.rotation = this.bingbong.rotation;
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x0004D98C File Offset: 0x0004BB8C
	private void Update()
	{
		this.removeAfterSeconds -= Time.deltaTime;
		this.effectTime -= Time.deltaTime;
		if (this.view.IsMine && this.removeAfterSeconds <= 0f)
		{
			PhotonNetwork.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x0004D9E3 File Offset: 0x0004BBE3
	private void FixedUpdate()
	{
		if (this.effectTime > 0f && this.physicsType != BingBongPhysics.PhysicsType.ForcePush_Gentle && this.physicsType != BingBongPhysics.PhysicsType.ForcePush)
		{
			this.DoEffect();
		}
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x0004DA0C File Offset: 0x0004BC0C
	private void DoEffect()
	{
		foreach (Character character in this.GetTargets())
		{
			character.refs.movement.ApplyExtraDrag(this.drag, true);
			character.AddForce(this.GetForceDirection(character.Center) * this.force, 1f, 1f);
			character.data.sinceGrounded = Mathf.Clamp(character.data.sinceGrounded, 0f, 0.25f);
			if (this.fallAmount > 0.01f && character.IsLocal)
			{
				character.Fall(this.fallAmount, 0f);
			}
		}
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x0004DAE8 File Offset: 0x0004BCE8
	private Vector3 GetForceDirection(Vector3 playerPos)
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return -this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForceGrab)
		{
			return this.TargetPos() - playerPos;
		}
		return Vector3.zero;
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x0004DB68 File Offset: 0x0004BD68
	private List<Character> GetTargets()
	{
		Vector3 vector = this.TargetPos();
		float num = 5f;
		List<Character> list = new List<Character>();
		foreach (Character character in Character.AllCharacters)
		{
			if (Vector3.Distance(vector, character.Center) < num)
			{
				list.Add(character);
			}
		}
		return list;
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0004DBE0 File Offset: 0x0004BDE0
	private Vector3 TargetPos()
	{
		return base.transform.TransformPoint(Vector3.forward * 5f);
	}

	// Token: 0x04000DF9 RID: 3577
	private PhotonView view;

	// Token: 0x04000DFA RID: 3578
	private Transform bingbong;

	// Token: 0x04000DFB RID: 3579
	public BingBongPhysics.PhysicsType physicsType;

	// Token: 0x04000DFC RID: 3580
	public float force;

	// Token: 0x04000DFD RID: 3581
	public float drag;

	// Token: 0x04000DFE RID: 3582
	public float fallAmount;

	// Token: 0x04000DFF RID: 3583
	public float removeAfterSeconds = 2f;

	// Token: 0x04000E00 RID: 3584
	public float effectTime = 2f;
}
