using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000201 RID: 513
public class Antlion : MonoBehaviour
{
	// Token: 0x06000F4F RID: 3919 RVA: 0x0004B8F4 File Offset: 0x00049AF4
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.anim = base.GetComponentInChildren<Animator>(true);
		this.collisionModifier = base.GetComponentInChildren<CollisionModifier>();
		this.climbModifierSurface = base.GetComponentInChildren<ClimbModifierSurface>();
		this.climbModifierSurface.alwaysClimbableRange = 16f;
		this.collisionModifier.standableRange = 16f;
		this.head = base.transform.Find("Head").gameObject;
		this.slideObj = base.transform.Find("Hill/Slide").gameObject;
		this.slideMat = this.slideObj.GetComponent<MeshRenderer>().material;
		this.slideMat.SetFloat("_Str", this.str);
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x0004B9B4 File Offset: 0x00049BB4
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.escapeRadiusForAchievement);
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x0004B9D8 File Offset: 0x00049BD8
	private void Update()
	{
		this.TestAchievement();
		if (!this.attacking)
		{
			this.attackCounter += Time.deltaTime;
		}
		this.GetClosestTarget();
		if (this.closestTarget != null)
		{
			this.DoActive();
			this.activeFor += Time.deltaTime;
			return;
		}
		this.DoInactive();
		this.activeFor = 0f;
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x0004BA44 File Offset: 0x00049C44
	private void DoActive()
	{
		if (this.str > 0.95f)
		{
			this.collisionModifier.hasStandableRange = true;
			this.climbModifierSurface.hasAlwaysClimbableRange = true;
		}
		if (Vector3.Distance(base.transform.position, this.closestTarget.Center) < 5f && this.attackCounter > 0.15f && this.view.IsMine)
		{
			this.attackCounter = 0f;
			this.view.RPC("RPCA_Attack", 0, new object[] { this.closestTarget.refs.view.ViewID });
		}
		this.ActiveVisuals();
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x0004BAF7 File Offset: 0x00049CF7
	[PunRPC]
	public void RPCA_Attack(int targetID)
	{
		this.Attack(PhotonView.Find(targetID).GetComponent<Character>());
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x0004BB0C File Offset: 0x00049D0C
	private void Attack(Character target)
	{
		Antlion.<>c__DisplayClass19_0 CS$<>8__locals1 = new Antlion.<>c__DisplayClass19_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.target = target;
		base.StartCoroutine(CS$<>8__locals1.<Attack>g__IAttack|0());
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0004BB3A File Offset: 0x00049D3A
	private void SinkLuggage()
	{
		ShortcutExtensions.DOLocalMoveY(this.luggage.transform, -2f, 1f, false);
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x0004BB58 File Offset: 0x00049D58
	private void GetClosestTarget()
	{
		float num = 12f;
		if (this.closestTarget != null)
		{
			num = 16f;
		}
		float num2 = num;
		Character character = null;
		foreach (Character character2 in Character.AllCharacters)
		{
			float num3 = Vector3.Distance(base.transform.position, character2.Center);
			if (num3 < num)
			{
				character2.ClampSinceGrounded(1f + Mathf.Clamp01(this.activeFor / 3f));
			}
			if (num3 < num2)
			{
				num2 = num3;
				character = character2;
			}
		}
		if (character != this.closestTarget && this.view.IsMine)
		{
			this.view.RPC("RPCA_SetClosestTarget", 0, new object[] { (character != null) ? character.refs.view.ViewID : (-1) });
		}
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x0004BC60 File Offset: 0x00049E60
	[PunRPC]
	private void RPCA_SetClosestTarget(int targetID)
	{
		if (targetID == -1)
		{
			this.closestTarget = null;
			return;
		}
		this.closestTarget = PhotonView.Find(targetID).GetComponent<Character>();
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x0004BC7F File Offset: 0x00049E7F
	private void DoInactive()
	{
		this.collisionModifier.hasStandableRange = false;
		this.climbModifierSurface.hasAlwaysClimbableRange = false;
		this.InactiveVisuals();
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x0004BCA0 File Offset: 0x00049EA0
	private void ActiveVisuals()
	{
		if (!this.firstActivation)
		{
			this.firstActivation = true;
			this.SinkLuggage();
		}
		if (this.anim.gameObject.activeInHierarchy)
		{
			this.anim.SetBool("Active", true);
		}
		if (!this.slideObj.activeSelf)
		{
			this.slideObj.SetActive(true);
			this.head.SetActive(true);
		}
		if (this.closestTarget)
		{
			this.head.transform.rotation = Quaternion.Lerp(this.head.transform.rotation, Quaternion.LookRotation(this.closestTarget.Center - this.head.transform.position), Time.deltaTime * 2f);
		}
		this.str = Mathf.MoveTowards(this.str, 1f, Time.deltaTime);
		this.slideMat.SetFloat("_Str", this.str);
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0004BDA0 File Offset: 0x00049FA0
	private void InactiveVisuals()
	{
		if (this.anim.gameObject.activeInHierarchy)
		{
			this.anim.SetBool("Active", false);
		}
		this.str = Mathf.MoveTowards(this.str, 0f, Time.deltaTime);
		if (this.slideObj.activeSelf)
		{
			if (this.str < 0.01f)
			{
				this.slideObj.SetActive(false);
				this.head.SetActive(false);
			}
			this.slideMat.SetFloat("_Str", this.str);
		}
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0004BE34 File Offset: 0x0004A034
	private void TestAchievement()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (this.bitLocalPlayer)
		{
			if (Character.localCharacter.data.dead)
			{
				this.bitLocalPlayer = false;
				return;
			}
			if (Character.localCharacter.data.fullyConscious && Vector3.Distance(base.transform.position, Character.localCharacter.Center) > this.escapeRadiusForAchievement)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.MegaentomologyBadge);
				this.bitLocalPlayer = false;
			}
		}
	}

	// Token: 0x04000D92 RID: 3474
	private GameObject head;

	// Token: 0x04000D93 RID: 3475
	private Material slideMat;

	// Token: 0x04000D94 RID: 3476
	private GameObject slideObj;

	// Token: 0x04000D95 RID: 3477
	private CollisionModifier collisionModifier;

	// Token: 0x04000D96 RID: 3478
	private ClimbModifierSurface climbModifierSurface;

	// Token: 0x04000D97 RID: 3479
	private float str;

	// Token: 0x04000D98 RID: 3480
	private float activeFor;

	// Token: 0x04000D99 RID: 3481
	private Animator anim;

	// Token: 0x04000D9A RID: 3482
	private PhotonView view;

	// Token: 0x04000D9B RID: 3483
	private bool bitLocalPlayer;

	// Token: 0x04000D9C RID: 3484
	public float escapeRadiusForAchievement;

	// Token: 0x04000D9D RID: 3485
	private Character closestTarget;

	// Token: 0x04000D9E RID: 3486
	private float attackCounter;

	// Token: 0x04000D9F RID: 3487
	private bool attacking;

	// Token: 0x04000DA0 RID: 3488
	public GameObject luggage;

	// Token: 0x04000DA1 RID: 3489
	private bool firstActivation;
}
