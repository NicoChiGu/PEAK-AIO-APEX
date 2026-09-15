using System;
using System.Collections.Generic;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class AOE : MonoBehaviour
{
	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000F60 RID: 3936 RVA: 0x0004BF5E File Offset: 0x0004A15E
	private bool hasStatus
	{
		get
		{
			return Mathf.Abs(this.statusAmount) > 0f;
		}
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x0004BF72 File Offset: 0x0004A172
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.range);
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x0004BF8A File Offset: 0x0004A18A
	private void Start()
	{
		if (this.auto)
		{
			this.Explode();
		}
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x0004BF9A File Offset: 0x0004A19A
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.Explode();
		}
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0004BFAC File Offset: 0x0004A1AC
	public void Explode()
	{
		if (this.range == 0f)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.range, HelperFunctions.GetMask(this.mask));
		List<Character> list = new List<Character>();
		for (int i = 0; i < array.Length; i++)
		{
			Character character;
			CharacterRagdoll.TryGetCharacterFromCollider(array[i], out character);
			if (character != null && !list.Contains(character))
			{
				float num = Vector3.Distance(base.transform.position, character.Center);
				if (num <= this.range)
				{
					float factor = this.GetFactor(num);
					if (factor >= this.minFactor && (!this.requireLineOfSigh || !HelperFunctions.LineCheck(base.transform.position, character.Center, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform))
					{
						list.Add(character);
						Vector3 vector = Vector3.zero;
						if (this.useSingleDirection)
						{
							vector = this.singleDirectionForwardTF.forward;
						}
						else
						{
							vector = (character.Center - base.transform.position).normalized;
						}
						if (Mathf.Abs(this.statusAmount) > 0f)
						{
							if (this.illegalStatus != "")
							{
								if (this.illegalStatus.ToUpperInvariant().Equals("BLIND"))
								{
									Affliction_Blind affliction_Blind = new Affliction_Blind
									{
										totalTime = this.statusAmount * factor
									};
									character.refs.afflictions.AddAffliction(affliction_Blind, false);
								}
								else
								{
									character.AddIllegalStatus(this.illegalStatus, this.statusAmount * factor);
								}
							}
							else
							{
								character.refs.afflictions.AdjustStatus(this.statusType, this.statusAmount * factor, false);
								if (this.addtlStatus.Length != 0)
								{
									for (int j = 0; j < this.addtlStatus.Length; j++)
									{
										character.refs.afflictions.AdjustStatus(this.addtlStatus[j], this.statusAmount * factor, false);
									}
								}
							}
						}
						if (this.hasAffliction)
						{
							character.refs.afflictions.AddAffliction(this.affliction, false);
						}
						character.AddForce(vector * factor * this.knockback, 0.7f, 1.3f);
						if (this.fallTime > 0f && character.photonView.IsMine)
						{
							character.Fall(factor * this.fallTime, 0f);
						}
					}
				}
			}
			else if (this.canLaunchItems)
			{
				Item componentInParent = array[i].GetComponentInParent<Item>();
				if (componentInParent != null && componentInParent.photonView.IsMine && componentInParent.itemState == ItemState.Ground)
				{
					float num2 = Vector3.Distance(base.transform.position, componentInParent.Center());
					if (num2 <= this.range)
					{
						float factor2 = this.GetFactor(num2);
						if (factor2 >= this.minFactor && (!this.requireLineOfSigh || !HelperFunctions.LineCheck(base.transform.position, componentInParent.Center(), HelperFunctions.LayerType.TerrainMap, 0f, 1).transform))
						{
							EventOnItemCollision eventOnItemCollision;
							if (this.procCollisionEvents && componentInParent.TryGetComponent<EventOnItemCollision>(ref eventOnItemCollision))
							{
								eventOnItemCollision.TriggerEvent();
							}
							if (this.cooksItems)
							{
								componentInParent.cooking.FinishCooking();
							}
							Vector3 normalized = (componentInParent.Center() - base.transform.position).normalized;
							componentInParent.rig.AddForce(normalized * factor2 * this.knockback * this.itemKnockbackMultiplier, 1);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0004C372 File Offset: 0x0004A572
	private float GetFactor(float dist)
	{
		return Mathf.Pow(1f - dist / this.range, this.factorPow);
	}

	// Token: 0x04000DA5 RID: 3493
	public HelperFunctions.LayerType mask;

	// Token: 0x04000DA6 RID: 3494
	public bool auto = true;

	// Token: 0x04000DA7 RID: 3495
	public bool onEnable;

	// Token: 0x04000DA8 RID: 3496
	public float range = 5f;

	// Token: 0x04000DA9 RID: 3497
	public float fallTime = 0.5f;

	// Token: 0x04000DAA RID: 3498
	public float knockback = 25f;

	// Token: 0x04000DAB RID: 3499
	public float minFactor = 0.2f;

	// Token: 0x04000DAC RID: 3500
	public float factorPow = 1f;

	// Token: 0x04000DAD RID: 3501
	public bool requireLineOfSigh;

	// Token: 0x04000DAE RID: 3502
	public bool canLaunchItems;

	// Token: 0x04000DAF RID: 3503
	public float itemKnockbackMultiplier = 1f;

	// Token: 0x04000DB0 RID: 3504
	public float statusAmount;

	// Token: 0x04000DB1 RID: 3505
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000DB2 RID: 3506
	public CharacterAfflictions.STATUSTYPE[] addtlStatus;

	// Token: 0x04000DB3 RID: 3507
	public string illegalStatus = "";

	// Token: 0x04000DB4 RID: 3508
	public bool useSingleDirection;

	// Token: 0x04000DB5 RID: 3509
	public Transform singleDirectionForwardTF;

	// Token: 0x04000DB6 RID: 3510
	public bool hasAffliction;

	// Token: 0x04000DB7 RID: 3511
	[SerializeReference]
	public Affliction affliction;

	// Token: 0x04000DB8 RID: 3512
	public bool procCollisionEvents;

	// Token: 0x04000DB9 RID: 3513
	public bool cooksItems;
}
