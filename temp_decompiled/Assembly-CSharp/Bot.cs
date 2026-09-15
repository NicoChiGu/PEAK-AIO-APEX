using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000067 RID: 103
public class Bot : MonoBehaviour
{
	// Token: 0x1700005C RID: 92
	// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0001C6A8 File Offset: 0x0001A8A8
	// (set) Token: 0x060004C7 RID: 1223 RVA: 0x0001C6B0 File Offset: 0x0001A8B0
	public Vector3 LookDirection
	{
		get
		{
			return this.lookDirection;
		}
		set
		{
			this.lookDirection = value;
		}
	}

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x060004C8 RID: 1224 RVA: 0x0001C6B9 File Offset: 0x0001A8B9
	// (set) Token: 0x060004C9 RID: 1225 RVA: 0x0001C6C1 File Offset: 0x0001A8C1
	public Vector2 MovementInput
	{
		get
		{
			return this.movementInput;
		}
		set
		{
			this.movementInput = value;
		}
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x060004CA RID: 1226 RVA: 0x0001C6CA File Offset: 0x0001A8CA
	// (set) Token: 0x060004CB RID: 1227 RVA: 0x0001C6D2 File Offset: 0x0001A8D2
	public bool IsSprinting
	{
		get
		{
			return this.isSprinting;
		}
		set
		{
			this.isSprinting = value;
		}
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x060004CC RID: 1228 RVA: 0x0001C6DB File Offset: 0x0001A8DB
	public Vector3 Center
	{
		get
		{
			return this.centerTransform.position;
		}
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x060004CD RID: 1229 RVA: 0x0001C6E8 File Offset: 0x0001A8E8
	// (set) Token: 0x060004CE RID: 1230 RVA: 0x0001C6F0 File Offset: 0x0001A8F0
	[CanBeNull]
	public Character TargetCharacter
	{
		get
		{
			return this.targetCharacter;
		}
		set
		{
			this.targetCharacter = value;
		}
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x060004CF RID: 1231 RVA: 0x0001C6FC File Offset: 0x0001A8FC
	public Vector3? DistanceToTargetCharacter
	{
		get
		{
			if (!(this.TargetCharacter == null))
			{
				return default(Vector3?);
			}
			return new Vector3?(this.TargetCharacter.Center - this.Center);
		}
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x060004D0 RID: 1232 RVA: 0x0001C73C File Offset: 0x0001A93C
	public Vector3 HeadPosition
	{
		get
		{
			return this.Center + Vector3.up;
		}
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0001C74E File Offset: 0x0001A94E
	private void Awake()
	{
		this.navigator = base.GetComponentInChildren<Navigator>();
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0001C75C File Offset: 0x0001A95C
	private void Update()
	{
		this.timeSprinting = (this.IsSprinting ? (this.timeSprinting + Time.deltaTime) : 0f);
		this.timeSincePatrolEnded += Time.deltaTime;
		if (this.targetCharacter != null)
		{
			this.timeWithTarget += Time.deltaTime;
			this.timeWithoutTarget = 0f;
		}
		else
		{
			this.timeWithoutTarget += 0f;
			this.timeWithTarget = 0f;
		}
		if (this.timeSincePatrolEnded > 0.2f)
		{
			this.patrolHit = default(NavMeshHit?);
		}
		Debug.DrawLine(this.Center, this.targetPos_Set, Color.cyan);
		Debug.DrawLine(this.Center, this.Center + this.navigationDirection_read, Color.blue);
		Debug.DrawLine(this.Center + Vector3.up, this.Center + Vector3.up + this.lookDirection, Color.yellow);
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x0001C86B File Offset: 0x0001AA6B
	private void Start()
	{
		this.LookDirection = base.transform.forward;
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0001C87E File Offset: 0x0001AA7E
	public void ClearTarget()
	{
		this.targetCharacter = null;
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x0001C888 File Offset: 0x0001AA88
	public bool CanSee(Vector3 from, Vector3 to, float maxDistance = 70f, float maxAngle = 110f)
	{
		return Vector3.Distance(from, to) <= maxDistance && Vector3.Angle(this.lookDirection, to - from) <= maxAngle && !HelperFunctions.LineCheck(from, to, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform;
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0001C8DC File Offset: 0x0001AADC
	public Rigidbody LookForPlayerHead(Vector3 searcherHeadPos, float maxRange = 70f, float maxAngle = 110f)
	{
		using (IEnumerator<Character> enumerator = Enumerable.Where<Character>(Object.FindObjectsByType<Character>(0), (Character character) => !character.isBot).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Character character2 = enumerator.Current;
				if (character2 == null)
				{
					Debug.Log("No player found");
					return null;
				}
				if (Vector3.Distance(this.Center, character2.TorsoPos()) > maxRange)
				{
					return null;
				}
				if (Vector3.Angle(character2.TorsoPos() - this.Center, this.lookDirection) > maxAngle)
				{
					return null;
				}
				Bodypart bodypart = character2.GetBodypart(BodypartType.Head);
				Debug.DrawLine(searcherHeadPos, bodypart.Rig.position, Color.red);
				if (HelperFunctions.LineCheck(searcherHeadPos, bodypart.Rig.position, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform)
				{
					return null;
				}
				Debug.Log("Found player head", bodypart.Rig);
				return bodypart.Rig;
			}
		}
		return null;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0001CA0C File Offset: 0x0001AC0C
	public void Patrol()
	{
		this.timeSincePatrolEnded = 0f;
		NavMeshHit navMeshHit;
		if ((this.patrolHit == null || this.remainingNavDistance < 1f) && this.navigator.TryGetPointOnNavMeshCloseTo(PatrolBoss.me.GetPoint(), out navMeshHit))
		{
			this.patrolHit = new NavMeshHit?(navMeshHit);
			this.targetPos_Set = this.patrolHit.Value.position;
		}
		this.MoveForward();
		this.LookInDirection(this.navigationDirection_read, 3f);
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0001CA93 File Offset: 0x0001AC93
	public void RotateThenMove(Vector3 dir, float rotationSpeed = 3f)
	{
		if (HelperFunctions.FlatAngle(dir, this.lookDirection) < 5f)
		{
			this.MoveForward();
		}
		else
		{
			this.StandStill();
		}
		this.LookInDirection(dir, rotationSpeed);
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0001CABE File Offset: 0x0001ACBE
	public void StandStill()
	{
		this.MovementInput = new Vector2(0f, 0f);
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0001CAD5 File Offset: 0x0001ACD5
	public void MoveForward()
	{
		this.MovementInput = new Vector2(0f, 1f);
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x0001CAEC File Offset: 0x0001ACEC
	public void Chase()
	{
		if (this.TargetCharacter == null)
		{
			this.StandStill();
			Debug.Log("No target character");
			return;
		}
		this.targetPos_Set = this.TargetCharacter.Center;
		this.MoveForward();
		this.LookInDirection(this.navigationDirection_read, 3f);
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0001CB40 File Offset: 0x0001AD40
	public void LookAtPoint(Vector3 point, float rotationSpeed = 3f)
	{
		this.LookInDirection((point - this.Center).normalized, rotationSpeed);
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0001CB68 File Offset: 0x0001AD68
	public void LookInDirection(Vector3 direction, float rotationSpeed = 3f)
	{
		this.LookDirection = Vector3.RotateTowards(this.LookDirection, direction, Time.deltaTime * rotationSpeed, 0f);
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0001CB88 File Offset: 0x0001AD88
	public bool CanSeeTarget(float maxDistance = 20f, float maxAngle = 120f)
	{
		if (this.TargetCharacter != null && this.CanSee(this.HeadPosition, this.TargetCharacter.Center, maxDistance, maxAngle))
		{
			this.timeSinceSawTarget = 0f;
			return true;
		}
		this.timeSinceSawTarget += Time.deltaTime;
		return false;
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x0001CBD8 File Offset: 0x0001ADD8
	public void FleeFromPoint(Vector3 point)
	{
		if (this.fleePoint == null || this.remainingNavDistance < 2f)
		{
			Vector3 normalized = (this.Center - point).normalized;
			NavMeshHit navMeshHit;
			if (this.navigator.TryGetPointOnNavMeshCloseTo(this.Center + normalized * 6f, out navMeshHit))
			{
				this.fleePoint = new NavMeshHit?(navMeshHit);
				this.targetPos_Set = this.fleePoint.Value.position;
			}
		}
		this.MoveForward();
		this.LookInDirection(this.navigationDirection_read, 3f);
	}

	// Token: 0x04000536 RID: 1334
	public Vector3 targetPos_Set;

	// Token: 0x04000537 RID: 1335
	public Vector3 navigationDirection_read;

	// Token: 0x04000538 RID: 1336
	public bool targetIsReachable;

	// Token: 0x04000539 RID: 1337
	public float remainingNavDistance;

	// Token: 0x0400053A RID: 1338
	public float timeSincePatrolEnded;

	// Token: 0x0400053B RID: 1339
	public float timeWithTarget;

	// Token: 0x0400053C RID: 1340
	public float timeWithoutTarget;

	// Token: 0x0400053D RID: 1341
	public Navigator navigator;

	// Token: 0x0400053E RID: 1342
	public Transform centerTransform;

	// Token: 0x0400053F RID: 1343
	public float timeSinceSawTarget;

	// Token: 0x04000540 RID: 1344
	public float timeSprinting;

	// Token: 0x04000541 RID: 1345
	private bool isSprinting;

	// Token: 0x04000542 RID: 1346
	private Vector3 lookDirection;

	// Token: 0x04000543 RID: 1347
	private Vector2 movementInput;

	// Token: 0x04000544 RID: 1348
	private Character targetCharacter;

	// Token: 0x04000545 RID: 1349
	private NavMeshHit? fleePoint;

	// Token: 0x04000546 RID: 1350
	private NavMeshHit? patrolHit = new NavMeshHit?(default(NavMeshHit));
}
