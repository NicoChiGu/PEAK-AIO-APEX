using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200030F RID: 783
public class RescueHook : ItemComponent
{
	// Token: 0x17000141 RID: 321
	// (get) Token: 0x0600143A RID: 5178 RVA: 0x0006665B File Offset: 0x0006485B
	public Character playerHoldingItem
	{
		get
		{
			return this.item.holderCharacter;
		}
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x00066668 File Offset: 0x00064868
	public override void Awake()
	{
		this.actionReduceUses = base.GetComponent<Action_ReduceUses>();
		this.camera = Camera.main;
		base.Awake();
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Combine(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
		this.line.positionCount = 40;
		this.startingClawLocalPos = this.claw.transform.localPosition;
		this.StopDisplayRope();
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x000666E4 File Offset: 0x000648E4
	public void Update()
	{
		Vector3 vector;
		RaycastHit hit = this.GetHit(out vector);
		this.item.overrideUsability = Optionable<bool>.Some(hit.transform);
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x00066718 File Offset: 0x00064918
	public void LateUpdate()
	{
		if (this.isPulling)
		{
			if (this.targetPlayer)
			{
				this.claw.transform.position = this.targetPlayer.Center;
			}
			else
			{
				this.claw.transform.position = this.targetPos;
			}
			this.claw.transform.forward = (this.claw.transform.position - base.transform.position).normalized;
			this.ropeRender.DisplayRope(this.dragPoint.position, this.clawButt.transform.position, this.sinceFire, this.line);
		}
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x000667DC File Offset: 0x000649DC
	private void FixedUpdate()
	{
		this.sinceFire += Time.fixedDeltaTime;
		if (this.isPulling)
		{
			if (this.targetPlayer)
			{
				this.targetPos = this.targetRig.position;
			}
			if (this.sinceFire > 0.25f)
			{
				if (this.targetPlayer)
				{
					this.targetPlayer.refs.movement.ApplyExtraDrag(this.extraDragOther, true);
					this.targetPlayer.data.sinceGrounded = Mathf.Clamp(this.targetPlayer.data.sinceGrounded, 0f, 1f);
					if (this.photonView.IsMine)
					{
						Vector3 normalized = (base.transform.position - this.targetPlayer.Center).normalized;
						Vector3 vector = Mathf.Clamp((base.transform.position - this.targetPlayer.Center).y, 0f, this.maxLiftDistance) * Vector3.up * this.liftDragForce;
						Vector3 vector2 = this.dragForce * normalized;
						vector2 *= this.pulLStrengthCurve.Evaluate(Vector3.Distance(base.transform.position, this.targetPlayer.Center));
						this.targetPlayer.AddForceToBodyPart(this.targetRig, (vector2 + vector) * 0.2f, vector2 + vector);
					}
					if (!this.threwAchievement && this.photonView.IsMine)
					{
						float num = Vector3.Distance(this.rescuedCharacterStartingPos, this.targetPlayer.Center) * CharacterStats.unitsToMeters;
						if (!this.targetPlayer.data.fullyConscious && num > 30f)
						{
							this.threwAchievement = true;
							Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.DisasterResponseBadge);
						}
					}
					if ((Vector3.Distance(this.dragPoint.position, this.targetRig.position) < this.stopPullFriendDistance || this.sinceFire > this.maxScoutHookTime) && this.photonView.IsMine)
					{
						this.photonView.RPC("RPCA_LetGo", 0, Array.Empty<object>());
					}
				}
				else
				{
					this.playerHoldingItem.refs.movement.ApplyExtraDrag(this.extraDragSelf, true);
					if (!this.hitNothing)
					{
						float num2 = 1f;
						num2 *= this.pulLStrengthCurve.Evaluate(Vector3.Distance(this.targetPos, this.playerHoldingItem.Center));
						if (this.startingPos.y < this.targetPos.y && this.playerHoldingItem.Center.y > this.targetPos.y)
						{
							Vector3 vector3 = (this.targetPos - this.playerHoldingItem.Center).normalized * this.launchForce * num2;
							Vector3 vector4 = Vector3.up * this.liftForce;
							vector3.y = 0f;
							this.playerHoldingItem.AddForce(vector3 + vector4, 1f, 1f);
							this.fly = false;
						}
						else
						{
							Vector3 vector5 = (this.targetPos - this.playerHoldingItem.Center).normalized * this.launchForce * num2;
							Vector3 vector6 = Vector3.up * this.liftForce;
							this.fly = true;
							this.playerHoldingItem.AddForce(vector5 + vector6, 1f, 1f);
							this.playerHoldingItem.data.sinceGrounded = 0f;
						}
					}
					Vector3 center = this.playerHoldingItem.Center;
					if (this.playerHoldingItem.Center.y > this.targetPos.y)
					{
						center.y = this.targetPos.y;
					}
					if (Vector3.Distance(this.playerHoldingItem.Center, this.targetPos) < this.stopPullDistance || this.sinceFire > this.maxWallHookTime)
					{
						this.RPCA_LetGo();
					}
				}
			}
			this.currentDistance = Vector3.Distance(this.dragPoint.position, this.targetPos);
			return;
		}
		this.fly = false;
		this.StopDisplayRope();
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x00066C48 File Offset: 0x00064E48
	private void OnDestroy()
	{
		if (this.playerHoldingItem && this.isPulling)
		{
			this.playerHoldingItem.data.sinceGrounded = 0f;
		}
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Remove(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x00066CA6 File Offset: 0x00064EA6
	private void StopDisplayRope()
	{
		this.line.enabled = false;
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x00066CB4 File Offset: 0x00064EB4
	private void Fire()
	{
		Debug.Log("Fire");
		this.sinceFire = 0f;
		for (int i = 0; i < this.rescueShot.Length; i++)
		{
			this.rescueShot[i].Play(base.transform.position);
		}
		Vector3 vector;
		RaycastHit hit = this.GetHit(out vector);
		if (!hit.transform)
		{
			this.photonView.RPC("RPCA_RescueWall", 0, new object[] { true, vector });
			return;
		}
		Character componentInParent = hit.transform.GetComponentInParent<Character>();
		Debug.Log(string.Format("Hit: {0} Rig: {1}, !hit.rigidbody: {2}", hit.collider.name, hit.rigidbody, !hit.rigidbody), hit.collider.gameObject);
		if (componentInParent)
		{
			this.photonView.RPC("RPCA_RescueCharacter", 0, new object[] { componentInParent.photonView });
			return;
		}
		this.photonView.RPC("RPCA_RescueWall", 0, new object[] { false, hit.point });
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x00066DEC File Offset: 0x00064FEC
	private RaycastHit GetHit(out Vector3 endFire)
	{
		Ray middleScreenRay = PExt.GetMiddleScreenRay();
		this.curRange = this.range;
		if (Vector3.Dot(middleScreenRay.direction, Vector3.up) < 0f)
		{
			this.curRange = this.rangeDownward;
		}
		List<RaycastHit> list = Enumerable.ToList<RaycastHit>(Physics.RaycastAll(middleScreenRay, this.curRange, HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysicalExceptDefault)));
		endFire = middleScreenRay.origin + middleScreenRay.direction * this.curRange;
		RaycastHit raycastHit = default(RaycastHit);
		foreach (RaycastHit raycastHit2 in list)
		{
			if (raycastHit2.rigidbody != null)
			{
				Item component = raycastHit2.rigidbody.GetComponent<Item>();
				Character character;
				if ((component != null && component.holderCharacter == this.item.holderCharacter) || (CharacterRagdoll.TryGetCharacterFromCollider(raycastHit2.collider, out character) && character == this.item.holderCharacter))
				{
					continue;
				}
			}
			if (raycastHit.distance == 0f || raycastHit2.distance < raycastHit.distance)
			{
				raycastHit = raycastHit2;
			}
		}
		if (Vector3.Distance(middleScreenRay.origin, raycastHit.point) < this.minRaycastDistance)
		{
			raycastHit = default(RaycastHit);
		}
		return raycastHit;
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x00066F60 File Offset: 0x00065160
	[PunRPC]
	public void RPCA_RescueItem(PhotonView objectView)
	{
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x00066F64 File Offset: 0x00065164
	[PunRPC]
	public void RPCA_RescueCharacter(PhotonView characterView)
	{
		Character component = characterView.GetComponent<Character>();
		this.targetPlayer = component;
		if (this.photonView.IsMine && this.targetPlayer.data.currentClimbHandle != null)
		{
			this.targetPlayer.refs.climbing.CancelHandle(false);
		}
		this.targetRig = component.GetBodypart(BodypartType.Torso).Rig;
		this.sinceFire = 0f;
		if (this.targetPlayer.photonView.IsMine || this.photonView.IsMine)
		{
			GamefeelHandler.instance.perlin.AddShake(base.transform.position, 5f, 0.2f, 15f, 40f);
		}
		this.isPulling = true;
		this.rescuedCharacterStartingPos = this.targetPlayer.Center;
		this.targetPlayer.Fall(2f, 0f);
		for (int i = 0; i < this.rescueHit.Length; i++)
		{
			this.rescueHit[i].Play(this.targetRig.transform.position);
		}
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x00067084 File Offset: 0x00065284
	[PunRPC]
	private void RPCA_LetGo()
	{
		Character holderCharacter = this.targetPlayer;
		float num = this.extraDragOther;
		if (holderCharacter == null)
		{
			holderCharacter = this.item.holderCharacter;
			num = this.extraDragSelf;
		}
		holderCharacter.refs.movement.ApplyExtraDrag(num, true);
		this.playerHoldingItem.data.sinceGrounded = 0f;
		this.targetRig = null;
		this.targetPlayer = null;
		this.isPulling = false;
		this.afterLetGoDragTime = this.afterLetGoDragSeconds;
		this.claw.transform.localPosition = this.startingClawLocalPos;
		this.claw.transform.localRotation = Quaternion.identity;
		if (this.playerHoldingItem.IsLocal)
		{
			this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
			OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
			if (data.HasData && data.Value == 0)
			{
				this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			}
		}
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x00067180 File Offset: 0x00065380
	[PunRPC]
	public void RPCA_RescueWall(bool hitNothing, Vector3 targetPos)
	{
		Debug.Log(string.Format("RPCA_RescueWall, hitnothing:{0}", hitNothing));
		this.targetPos = targetPos;
		this.startingPos = this.item.holderCharacter.Center;
		this.hitNothing = hitNothing;
		this.sinceFire = 0f;
		GamefeelHandler.instance.perlin.AddShake(base.transform.position, 5f, 0.2f, 15f, 40f);
		this.isPulling = true;
		this.playerHoldingItem.Fall(this.selfFallSeconds, 0f);
		if (this.photonView.IsMine && this.item.holderCharacter.data.currentClimbHandle != null)
		{
			this.item.holderCharacter.refs.climbing.CancelHandle(false);
		}
		for (int i = 0; i < this.rescueHit.Length; i++)
		{
			this.rescueHit[i].Play(targetPos);
		}
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x00067284 File Offset: 0x00065484
	private void OnPrimaryFinishedCast()
	{
		this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (data.HasData && data.Value == 0)
		{
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			return;
		}
		this.Fire();
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x000672DA File Offset: 0x000654DA
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x040012B9 RID: 4793
	public Transform dragPoint;

	// Token: 0x040012BA RID: 4794
	public GameObject claw;

	// Token: 0x040012BB RID: 4795
	public Transform clawButt;

	// Token: 0x040012BC RID: 4796
	public float maxLength = 40f;

	// Token: 0x040012BD RID: 4797
	public float dragForce = 100f;

	// Token: 0x040012BE RID: 4798
	public float liftDragForce = 100f;

	// Token: 0x040012BF RID: 4799
	public float extraDragSelf = 0.95f;

	// Token: 0x040012C0 RID: 4800
	public float extraDragOther = 0.95f;

	// Token: 0x040012C1 RID: 4801
	public float launchForce = 10f;

	// Token: 0x040012C2 RID: 4802
	public float liftForce = 1000f;

	// Token: 0x040012C3 RID: 4803
	private Action_ReduceUses actionReduceUses;

	// Token: 0x040012C4 RID: 4804
	private Camera camera;

	// Token: 0x040012C5 RID: 4805
	private float currentDistance;

	// Token: 0x040012C6 RID: 4806
	private bool fly;

	// Token: 0x040012C7 RID: 4807
	private bool hitNothing;

	// Token: 0x040012C8 RID: 4808
	private bool isPulling;

	// Token: 0x040012C9 RID: 4809
	private float sinceFire;

	// Token: 0x040012CA RID: 4810
	private Character targetPlayer;

	// Token: 0x040012CB RID: 4811
	private Vector3 targetPos;

	// Token: 0x040012CC RID: 4812
	private Vector3 startingPos;

	// Token: 0x040012CD RID: 4813
	private Vector3 rescuedCharacterStartingPos;

	// Token: 0x040012CE RID: 4814
	private bool threwAchievement;

	// Token: 0x040012CF RID: 4815
	private Rigidbody targetRig;

	// Token: 0x040012D0 RID: 4816
	public RopeRender ropeRender;

	// Token: 0x040012D1 RID: 4817
	public LineRenderer line;

	// Token: 0x040012D2 RID: 4818
	private Vector3 startingClawLocalPos;

	// Token: 0x040012D3 RID: 4819
	public float maxWallHookTime = 1f;

	// Token: 0x040012D4 RID: 4820
	public float maxScoutHookTime = 2f;

	// Token: 0x040012D5 RID: 4821
	public float maxLiftDistance = 10f;

	// Token: 0x040012D6 RID: 4822
	public float stopPullDistance = 5f;

	// Token: 0x040012D7 RID: 4823
	public float stopPullFriendDistance = 5f;

	// Token: 0x040012D8 RID: 4824
	public float minRaycastDistance = 5f;

	// Token: 0x040012D9 RID: 4825
	public SFX_Instance[] rescueShot;

	// Token: 0x040012DA RID: 4826
	public SFX_Instance[] rescueHit;

	// Token: 0x040012DB RID: 4827
	public AnimationCurve pulLStrengthCurve;

	// Token: 0x040012DC RID: 4828
	public Transform firePoint;

	// Token: 0x040012DD RID: 4829
	public float range = 30f;

	// Token: 0x040012DE RID: 4830
	public float rangeDownward = 40f;

	// Token: 0x040012DF RID: 4831
	public float curRange;

	// Token: 0x040012E0 RID: 4832
	public float afterLetGoDragSeconds = 1f;

	// Token: 0x040012E1 RID: 4833
	private float afterLetGoDragTime;

	// Token: 0x040012E2 RID: 4834
	public float selfFallSeconds = 0.5f;
}
