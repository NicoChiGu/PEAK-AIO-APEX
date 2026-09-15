using System;
using System.Collections.Generic;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class Bugfix : MonoBehaviour, IInteractible
{
	// Token: 0x06000FF2 RID: 4082 RVA: 0x0004F3AE File Offset: 0x0004D5AE
	private void Start()
	{
		base.transform.localScale = Vector3.zero;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x0004F3CC File Offset: 0x0004D5CC
	private void LateUpdate()
	{
		this.counter += Time.deltaTime;
		this.lifeTime += Time.deltaTime;
		if (this.targetCharacter && !this.targetCharacter.data.dead)
		{
			if (this.targetCharacter.IsLocal && this.counter > 29f)
			{
				this.targetCharacter.refs.afflictions.AddAffliction(new Affliction_PreventPoisonHealing(30f), false);
				if (this.totalStatusApplied < this.maxStatus || this.targetCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) < 0.5f)
				{
					this.targetCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.05f, false, true, true);
					this.totalStatusApplied += 0.05f;
				}
				this.counter = 0f;
			}
			Vector3 vector = this.leg.TransformPoint(this.localPos);
			base.transform.position = vector;
			Quaternion quaternion = Quaternion.LookRotation(this.leg.TransformDirection(this.forward), this.leg.TransformDirection(this.up));
			base.transform.rotation = quaternion;
			base.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, this.lifeTime / 300f);
			return;
		}
		if (this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x0004F55C File Offset: 0x0004D75C
	[PunRPC]
	public void AttachBug(int targetID)
	{
		PhotonView photonView = PhotonView.Find(targetID);
		this.targetCharacter = photonView.GetComponent<Character>();
		Rigidbody bodypartRig = this.targetCharacter.GetBodypartRig(BodypartType.Knee_R);
		this.leg = bodypartRig.transform;
		this.localPos = new Vector3(-0.27054f, 0f, -0.17134f);
		Vector3 vector = bodypartRig.transform.TransformPoint(this.localPos);
		Vector3 vector2;
		vector2..ctor(0f, 55f, 0f);
		Quaternion quaternion = bodypartRig.transform.rotation * Quaternion.Euler(vector2);
		base.transform.position = vector;
		base.transform.rotation = quaternion;
		this.forward = this.leg.InverseTransformDirection(base.transform.forward);
		this.up = this.leg.InverseTransformDirection(base.transform.up);
		Bugfix.AllAttachedBugs.Add(this, this.targetCharacter);
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x0004F652 File Offset: 0x0004D852
	private void OnDestroy()
	{
		Bugfix.AllAttachedBugs.Remove(this);
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x0004F660 File Offset: 0x0004D860
	public bool IsInteractible(Character interactor)
	{
		float num = Vector3.Angle(base.transform.position - MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
		float num2 = ((Character.AllCharacters.Count == 1) ? 15f : 0f);
		return num <= 2f + num2 + this.lifeTime / 60f;
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x0004F6D3 File Offset: 0x0004D8D3
	public void Interact(Character interactor)
	{
		GameUtils.instance.InstantiateAndGrab(this.bugItem, interactor, 0);
		this.photonView.RPC("RPCA_Remove", 0, Array.Empty<object>());
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x0004F6FD File Offset: 0x0004D8FD
	[PunRPC]
	public void RPCA_Remove()
	{
		if (this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x0004F717 File Offset: 0x0004D917
	public void HoverEnter()
	{
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x0004F719 File Offset: 0x0004D919
	public void HoverExit()
	{
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x0004F71B File Offset: 0x0004D91B
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x0004F728 File Offset: 0x0004D928
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x0004F730 File Offset: 0x0004D930
	public string GetInteractionText()
	{
		return LocalizedText.GetText("TICKINTERACT", true);
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x0004F73D File Offset: 0x0004D93D
	public string GetName()
	{
		return LocalizedText.GetText("TICK", true);
	}

	// Token: 0x04000E5C RID: 3676
	public Item bugItem;

	// Token: 0x04000E5D RID: 3677
	private Transform leg;

	// Token: 0x04000E5E RID: 3678
	private Vector3 localPos;

	// Token: 0x04000E5F RID: 3679
	private Vector3 forward;

	// Token: 0x04000E60 RID: 3680
	private Vector3 up;

	// Token: 0x04000E61 RID: 3681
	public float maxStatus = 0.5f;

	// Token: 0x04000E62 RID: 3682
	private float totalStatusApplied;

	// Token: 0x04000E63 RID: 3683
	private float lifeTime;

	// Token: 0x04000E64 RID: 3684
	private PhotonView photonView;

	// Token: 0x04000E65 RID: 3685
	private Character targetCharacter;

	// Token: 0x04000E66 RID: 3686
	public static Dictionary<Bugfix, Character> AllAttachedBugs = new Dictionary<Bugfix, Character>();

	// Token: 0x04000E67 RID: 3687
	private float counter;
}
