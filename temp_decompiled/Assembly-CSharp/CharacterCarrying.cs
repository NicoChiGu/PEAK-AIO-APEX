using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000225 RID: 549
public class CharacterCarrying : MonoBehaviour
{
	// Token: 0x06001020 RID: 4128 RVA: 0x0004FFF0 File Offset: 0x0004E1F0
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x00050000 File Offset: 0x0004E200
	private void FixedUpdate()
	{
		if (this.character.data.isCarried && this.character.data.carrier == null)
		{
			this.CarrierGone();
		}
		if (this.character.data.carrier)
		{
			this.GetCarried();
		}
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x0005005C File Offset: 0x0004E25C
	private void Update()
	{
		if (this.character.data.carriedPlayer && (this.character.data.carriedPlayer.data.dead || !this.character.data.carriedPlayer.data.fullyPassedOut || this.character.data.fullyPassedOut || this.character.data.dead) && this.character.refs.view.IsMine)
		{
			this.Drop(this.character.data.carriedPlayer);
		}
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x0005010C File Offset: 0x0004E30C
	private void ToggleCarryPhysics(bool setCarried)
	{
		this.character.refs.ragdoll.ToggleCollision(!setCarried);
		this.character.refs.animations.SetBool("IsCarried", setCarried);
		Debug.Log("SetIsCarried: " + setCarried.ToString());
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x00050164 File Offset: 0x0004E364
	private void GetCarried()
	{
		Vector3 vector = Vector3.ClampMagnitude(this.character.data.carrier.refs.carryPosRef.position + this.character.data.carrier.data.avarageVelocity * 0.06f - this.character.Center, 1f);
		this.character.AddForce(vector * 500f, 1f, 1f);
		this.character.refs.movement.ApplyExtraDrag(0.5f, true);
		this.character.data.sinceGrounded = 0f;
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x00050224 File Offset: 0x0004E424
	internal void StartCarry(Character target)
	{
		this.character.refs.items.EquipSlot(Optionable<byte>.None);
		this.character.photonView.RPC("RPCA_StartCarry", 0, new object[] { target.photonView });
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x00050270 File Offset: 0x0004E470
	[PunRPC]
	public void RPCA_StartCarry(PhotonView targetView)
	{
		Character component = targetView.GetComponent<Character>();
		BackpackSlot backpackSlot = this.character.player.backpackSlot;
		if (!backpackSlot.IsEmpty())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				Debug.Log(string.Format("{0} is starting to carry {1} but has backpack, dropping backpack", this.character, component));
				PhotonNetwork.InstantiateItemRoom(backpackSlot.GetPrefabName(), component.GetBodypart(BodypartType.Torso).transform.position, Quaternion.identity).GetComponent<PhotonView>().RPC("SetItemInstanceDataRPC", 0, new object[] { backpackSlot.data });
			}
			backpackSlot.EmptyOut();
		}
		else if (this.character.data.carriedPlayer != null)
		{
			this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
			return;
		}
		component.refs.carriying.ToggleCarryPhysics(true);
		component.data.isCarried = true;
		this.character.data.carriedPlayer = component;
		component.data.carrier = this.character;
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			Debug.Log("Updating weight for " + allPlayerCharacters[i].gameObject.name + "...");
			allPlayerCharacters[i].refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x000503D1 File Offset: 0x0004E5D1
	internal void Drop(Character target)
	{
		this.character.photonView.RPC("RPCA_Drop", 0, new object[] { target.photonView });
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x000503F8 File Offset: 0x0004E5F8
	[PunRPC]
	public void RPCA_Drop(PhotonView targetView)
	{
		Character component = targetView.GetComponent<Character>();
		component.refs.carriying.ToggleCarryPhysics(false);
		component.data.isCarried = false;
		component.data.carrier = null;
		this.character.data.carriedPlayer = null;
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			Debug.Log("Updating weight for " + allPlayerCharacters[i].gameObject.name + "...");
			allPlayerCharacters[i].refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x00050496 File Offset: 0x0004E696
	private void CarrierGone()
	{
		this.character.refs.carriying.ToggleCarryPhysics(false);
	}

	// Token: 0x04000E8C RID: 3724
	private Character character;
}
