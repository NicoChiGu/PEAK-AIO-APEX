using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002AD RID: 685
[DefaultExecutionOrder(1000)]
public class NavPoint : MonoBehaviour
{
	// Token: 0x060012B3 RID: 4787 RVA: 0x0005F448 File Offset: 0x0005D648
	internal NavPoint GetNext(Vector3 targetDirection)
	{
		List<NavPoint> list = new List<NavPoint>();
		foreach (NavPoint navPoint in this.connections)
		{
			if (HelperFunctions.FlatAngle(targetDirection, navPoint.transform.position - base.transform.position) < 90f)
			{
				list.Add(navPoint);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x0005F4E8 File Offset: 0x0005D6E8
	internal void MirrorConnections()
	{
		foreach (NavPoint navPoint in this.connections)
		{
			if (!navPoint.connections.Contains(this))
			{
				navPoint.connections.Add(this);
			}
		}
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x0005F550 File Offset: 0x0005D750
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		foreach (NavPoint navPoint in this.connections)
		{
			Gizmos.DrawLine(base.transform.position + Vector3.up * 0.1f, navPoint.transform.position + Vector3.up * 0.1f);
		}
	}

	// Token: 0x0400116B RID: 4459
	public List<NavPoint> connections = new List<NavPoint>();
}
