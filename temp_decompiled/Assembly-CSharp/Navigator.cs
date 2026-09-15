using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200006C RID: 108
public class Navigator : MonoBehaviour
{
	// Token: 0x060004F1 RID: 1265 RVA: 0x0001D2CC File Offset: 0x0001B4CC
	private void Awake()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.agent.updatePosition = false;
		this.agent.updateRotation = false;
		this.bot = base.GetComponentInParent<Bot>();
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001D2FE File Offset: 0x0001B4FE
	private void Start()
	{
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0001D300 File Offset: 0x0001B500
	public bool TryGetPointOnNavMeshCloseTo(Vector3 position, out NavMeshHit hit)
	{
		return NavMesh.SamplePosition(position, ref hit, 2f, 1 << NavMesh.GetAreaFromName("Walkable"));
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0001D320 File Offset: 0x0001B520
	private void Update()
	{
		this.agent.nextPosition = this.bot.Center;
		this.bot.navigationDirection_read = this.agent.desiredVelocity.normalized;
		if (this.agent.isOnNavMesh)
		{
			this.bot.remainingNavDistance = this.agent.remainingDistance;
		}
		if (this.lastReadTargetPosition == this.bot.targetPos_Set)
		{
			return;
		}
		if (this.agent.isOnNavMesh)
		{
			this.lastReadTargetPosition = this.bot.targetPos_Set;
			this.agent.SetDestination(this.lastReadTargetPosition);
		}
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0001D3CD File Offset: 0x0001B5CD
	public void SetAgentVelocity(Vector3 velocity)
	{
		this.agent.velocity = velocity;
	}

	// Token: 0x0400055A RID: 1370
	[HideInInspector]
	public NavMeshAgent agent;

	// Token: 0x0400055B RID: 1371
	private Bot bot;

	// Token: 0x0400055C RID: 1372
	private Vector3 lastReadTargetPosition;
}
