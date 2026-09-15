using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class CharacterGrabbing : MonoBehaviour
{
	// Token: 0x0600104A RID: 4170 RVA: 0x00051C25 File Offset: 0x0004FE25
	private void Start()
	{
		this.character = base.GetComponent<Character>();
		Bodypart bodypart = this.character.GetBodypart(BodypartType.Hand_R);
		bodypart.collisionStayAction = (Action<Collision>)Delegate.Combine(bodypart.collisionStayAction, new Action<Collision>(this.GrabAction));
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x00051C64 File Offset: 0x0004FE64
	private void GrabAction(Collision collision)
	{
		if (!this.character.data.isScoutmaster)
		{
			return;
		}
		if (!this.character.photonView.IsMine)
		{
			return;
		}
		if (this.character.data.grabJoint)
		{
			return;
		}
		if (!this.character.data.isReaching)
		{
			return;
		}
		if (this.character.data.sinceLetGoOfFriend < 0.35f)
		{
			return;
		}
		if (!collision.rigidbody)
		{
			return;
		}
		Character componentInParent = collision.transform.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent == this.character)
		{
			return;
		}
		BodypartType partType = componentInParent.GetPartType(collision.rigidbody);
		if (partType == (BodypartType)(-1))
		{
			return;
		}
		this.character.photonView.RPC("RPCA_GrabAttach", 0, new object[]
		{
			componentInParent.photonView,
			(int)partType,
			collision.rigidbody.transform.InverseTransformPoint(this.character.GetBodypart(BodypartType.Hand_R).Rig.transform.position)
		});
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x00051D80 File Offset: 0x0004FF80
	[PunRPC]
	public void RPCA_GrabAttach(PhotonView view, int bodyPartID, Vector3 relativePos)
	{
		Character component = view.GetComponent<Character>();
		if (!component)
		{
			return;
		}
		Rigidbody rig = component.GetBodypart((BodypartType)bodyPartID).Rig;
		Rigidbody rig2 = this.character.GetBodypart(BodypartType.Hand_R).Rig;
		rig2.transform.position = rig.transform.TransformPoint(relativePos);
		this.character.data.grabJoint = rig2.gameObject.AddComponent<FixedJoint>();
		this.character.data.grabJoint.connectedBody = rig;
		component.BreakCharacterCarrying(false);
		this.character.data.grabbedPlayer = component;
		component.data.grabbingPlayer = this.character;
		Debug.Log(string.Format("Grab Attaching {0} to {1}", component, rig));
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x00051E44 File Offset: 0x00050044
	[PunRPC]
	public void RPCA_GrabUnattach()
	{
		if (this.character.data.grabbedPlayer)
		{
			this.character.data.grabbedPlayer.data.grabbingPlayer = null;
		}
		this.character.data.grabbedPlayer = null;
		Object.Destroy(this.character.data.grabJoint);
		this.character.data.sinceLetGoOfFriend = 0f;
		Debug.Log("Grab unattaching");
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x00051EC8 File Offset: 0x000500C8
	private void Update()
	{
		if (!this.character.refs.view.IsMine)
		{
			return;
		}
		if (this.character.data.grabbingPlayer && this.character.input.jumpWasPressed && !this.character.data.grabbingPlayer.isBot)
		{
			this.character.data.grabbingPlayer.refs.view.RPC("RPCA_GrabUnattach", 0, Array.Empty<object>());
		}
		if (!this.CanGrab())
		{
			if (this.character.data.grabJoint || this.character.data.isReaching)
			{
				this.character.refs.view.RPC("RPCA_StopReaching", 0, Array.Empty<object>());
			}
			return;
		}
		if (this.character.data.sincePressReach < 0.2f)
		{
			if (!this.character.data.isReaching)
			{
				this.character.refs.view.RPC("RPCA_StartReaching", 0, Array.Empty<object>());
			}
		}
		else if (this.character.data.isReaching)
		{
			this.character.refs.view.RPC("RPCA_StopReaching", 0, Array.Empty<object>());
		}
		if (this.character.data.grabJoint)
		{
			if (this.character.data.grabbedPlayer)
			{
				this.character.data.grabbedPlayer.LimitFalling();
			}
			if (!this.character.data.isReaching)
			{
				this.character.refs.view.RPC("RPCA_GrabUnattach", 0, Array.Empty<object>());
			}
		}
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x0005209B File Offset: 0x0005029B
	private void FixedUpdate()
	{
		this.character.data.grabFriendDistance = 1000f;
		if (this.character.data.isReaching)
		{
			this.Reach();
		}
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x000520CA File Offset: 0x000502CA
	[PunRPC]
	private void RPCA_StopReaching()
	{
		this.character.data.isReaching = false;
		if (this.character.data.grabJoint)
		{
			Object.Destroy(this.character.data.grabJoint);
		}
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x00052109 File Offset: 0x00050309
	[PunRPC]
	private void RPCA_StartGrabbing()
	{
		this.character.data.isReaching = false;
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x0005211C File Offset: 0x0005031C
	[PunRPC]
	private void RPCA_StartReaching()
	{
		this.character.data.isReaching = true;
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x00052130 File Offset: 0x00050330
	private void Reach()
	{
		foreach (Character character in Character.AllCharacters)
		{
			float num = Vector3.Distance(this.character.Center, character.Center);
			if (num <= 4f && Vector3.Angle(this.character.data.lookDirection, character.Center - this.character.Center) <= 60f && this.TargetCanBeHelped(character))
			{
				if (character.IsStuck() && character.IsLocal)
				{
					character.UnStick();
				}
				character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Web, 1f, false, false);
				if (num < this.character.data.grabFriendDistance)
				{
					this.character.data.grabFriendDistance = num;
					this.character.data.sinceGrabFriend = 0f;
				}
				if (this.character.refs.view.IsMine)
				{
					GUIManager.instance.Grasp();
				}
				if (character.refs.view.IsMine)
				{
					character.DragTowards(this.character.Center, 70f);
					character.LimitFalling();
					GUIManager.instance.Grasp();
				}
			}
		}
	}

	// Token: 0x06001054 RID: 4180 RVA: 0x000522B4 File Offset: 0x000504B4
	private bool TargetCanBeHelped(Character item)
	{
		if (item != this.character)
		{
			if (item.IsStuck() || item.data.sinceUnstuck < 1f)
			{
				return true;
			}
			if (item.refs.afflictions.isWebbed)
			{
				return true;
			}
			if (item.data.isClimbing && item.Center.y < this.character.Center.y + 1f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001055 RID: 4181 RVA: 0x00052334 File Offset: 0x00050534
	private bool CanGrab()
	{
		return !(this.character.data.currentItem != null) && Time.time - this.character.data.lastConsumedItem >= 0.5f && !this.character.data.isClimbing && !this.character.data.isRopeClimbing && !this.character.data.isVineClimbing;
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x000523B8 File Offset: 0x000505B8
	internal void Throw(Vector3 force, float fallSeconds)
	{
		this.character.data.grabbedPlayer.RPCA_Fall(1f);
		this.character.data.grabbedPlayer.AddForce(force, 0.7f, 1f);
		this.RPCA_GrabUnattach();
	}

	// Token: 0x04000E9D RID: 3741
	private Character character;
}
