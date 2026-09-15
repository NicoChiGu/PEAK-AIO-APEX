using System;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class BotBoar : MonoBehaviour
{
	// Token: 0x060004E1 RID: 1249 RVA: 0x0001CC9F File Offset: 0x0001AE9F
	private void Awake()
	{
		this.bot = base.GetComponentInChildren<Bot>();
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0001CCB9 File Offset: 0x0001AEB9
	private void Start()
	{
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0001CCBB File Offset: 0x0001AEBB
	public void ClearTarget()
	{
		this.bot.ClearTarget();
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0001CCC8 File Offset: 0x0001AEC8
	private void Update()
	{
		this.bot.navigator.SetAgentVelocity(this.character.GetBodypart(BodypartType.Torso).Rig.linearVelocity);
		if (this.bot.timeSprinting > 3f)
		{
			this.bot.IsSprinting = false;
		}
		if (this.flee)
		{
			Debug.Log("Fleeing");
			if (this.bot.TargetCharacter == null || this.outOfSightTime >= 4f)
			{
				this.flee = false;
				this.outOfSightTime = 0f;
				this.bot.ClearTarget();
				this.potentialTarget = null;
				return;
			}
			this.bot.FleeFromPoint(this.bot.TargetCharacter.Center);
			if (this.bot.CanSee(this.bot.TargetCharacter.Head, this.bot.Center, 20f, 360f))
			{
				Debug.DrawLine(this.bot.TargetCharacter.Head, this.bot.Center, Color.green);
				this.outOfSightTime = 0f;
				return;
			}
			Debug.DrawLine(this.bot.TargetCharacter.Head, this.bot.Center, Color.red);
			this.outOfSightTime += Time.deltaTime;
			return;
		}
		else
		{
			if (this.bot.TargetCharacter)
			{
				Debug.Log("Chasing");
				Vector3? vector = this.bot.DistanceToTargetCharacter;
				if (vector == null || vector.GetValueOrDefault().magnitude <= 4f)
				{
					if (this.bot.timeWithTarget <= 15f)
					{
						goto IL_01E6;
					}
					vector = this.bot.DistanceToTargetCharacter;
					if (vector == null || vector.GetValueOrDefault().magnitude <= 2f)
					{
						goto IL_01E6;
					}
				}
				this.bot.ClearTarget();
				IL_01E6:
				this.bot.Chase();
				this.bot.CanSeeTarget(20f, 120f);
				if (this.bot.timeSinceSawTarget > 5f)
				{
					this.bot.ClearTarget();
				}
				if (!this.bot.IsSprinting)
				{
					this.flee = true;
				}
				return;
			}
			if (this.potentialTarget != null)
			{
				Debug.Log("Looking at target");
				if (!this.bot.CanSee(this.bot.HeadPosition, this.potentialTarget.Center, 70f, 110f))
				{
					this.potentialTarget = null;
					this.timeLookingAtTarget = 0f;
					return;
				}
				this.bot.StandStill();
				this.bot.LookAtPoint(this.potentialTarget.Center, 3f);
				this.timeLookingAtTarget += Time.deltaTime;
				if (this.timeLookingAtTarget > 4f)
				{
					this.bot.TargetCharacter = this.potentialTarget;
					this.bot.IsSprinting = true;
					this.potentialTarget = null;
					this.timeLookingAtTarget = 0f;
				}
			}
			if (this.potentialTarget == null)
			{
				this.bot.Patrol();
				Rigidbody rigidbody = this.bot.LookForPlayerHead(this.bot.HeadPosition, 20f, 110f);
				this.potentialTarget = ((rigidbody != null) ? rigidbody.GetComponentInParent<Character>() : null);
			}
			return;
		}
	}

	// Token: 0x04000547 RID: 1351
	private Bot bot;

	// Token: 0x04000548 RID: 1352
	private Rigidbody rig_g;

	// Token: 0x04000549 RID: 1353
	private Character character;

	// Token: 0x0400054A RID: 1354
	private Vector3 startPosition;

	// Token: 0x0400054B RID: 1355
	public float timeSinceSawTarget;

	// Token: 0x0400054C RID: 1356
	public Character potentialTarget;

	// Token: 0x0400054D RID: 1357
	public float timeLookingAtTarget;

	// Token: 0x0400054E RID: 1358
	public float timeSprinting;

	// Token: 0x0400054F RID: 1359
	private bool flee;

	// Token: 0x04000550 RID: 1360
	private float outOfSightTime;
}
