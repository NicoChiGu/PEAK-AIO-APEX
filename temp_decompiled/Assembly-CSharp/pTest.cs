using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000307 RID: 775
public class pTest : MonoBehaviour
{
	// Token: 0x0600141C RID: 5148 RVA: 0x00065B87 File Offset: 0x00063D87
	private void Awake()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.agent.updatePosition = false;
		this.agent.updateRotation = false;
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x00065BAD File Offset: 0x00063DAD
	private void Start()
	{
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x00065BAF File Offset: 0x00063DAF
	private void Update()
	{
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x00065BB4 File Offset: 0x00063DB4
	private void OnDrawGizmosSelected()
	{
		BoxCollider boxCollider = base.GetComponent<BoxCollider>();
		Vector3 center = boxCollider.bounds.center;
		Collider[] array = Enumerable.ToArray<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(center, boxCollider.bounds.extents, boxCollider.transform.rotation), (Collider c) => c != boxCollider));
		Debug.Log(string.Format("position: {0}, extents: {1}", center, boxCollider.bounds.extents));
		foreach (Collider collider in array)
		{
			Debug.Log("Collider: " + collider.name);
		}
		Gizmos.color = ((array.Length != 0) ? Color.red : Color.green);
		Gizmos.DrawWireCube(center, boxCollider.bounds.extents * 2f);
	}

	// Token: 0x040012A8 RID: 4776
	private NavMeshAgent agent;
}
