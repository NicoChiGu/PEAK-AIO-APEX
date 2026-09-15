using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002AE RID: 686
public class NavPoints : MonoBehaviour
{
	// Token: 0x060012B7 RID: 4791 RVA: 0x0005F5FF File Offset: 0x0005D7FF
	private void Awake()
	{
		NavPoints.instance = this;
		this.points = new List<NavPoint>();
		this.points.AddRange(base.GetComponentsInChildren<NavPoint>());
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x0005F624 File Offset: 0x0005D824
	private void OnDrawGizmos()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		Gizmos.color = Color.blue;
		foreach (NavPoint navPoint in this.points)
		{
			foreach (NavPoint navPoint2 in navPoint.connections)
			{
				Gizmos.DrawLine(navPoint.transform.position, navPoint2.transform.position);
			}
		}
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x0005F6DC File Offset: 0x0005D8DC
	public void ConnectPoints()
	{
		this.points = new List<NavPoint>();
		this.points.AddRange(base.GetComponentsInChildren<NavPoint>());
		foreach (NavPoint navPoint in this.points)
		{
			this.CheckPoint(navPoint);
		}
		foreach (NavPoint navPoint2 in this.points)
		{
			navPoint2.MirrorConnections();
		}
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x0005F78C File Offset: 0x0005D98C
	private void CheckPoint(NavPoint point)
	{
		point.connections = new List<NavPoint>();
		float num = float.PositiveInfinity;
		List<NavPoint> list = new List<NavPoint>();
		foreach (NavPoint navPoint in this.points)
		{
			if (!(navPoint == point) && !HelperFunctions.LineCheck(point.transform.position + Vector3.up, navPoint.transform.position + Vector3.up, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform)
			{
				list.Add(navPoint);
				float num2 = Vector3.Distance(point.transform.position, navPoint.transform.position);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		float num3 = num * 1.5f;
		foreach (NavPoint navPoint2 in list)
		{
			if (Vector3.Distance(point.transform.position, navPoint2.transform.position) < num3)
			{
				point.connections.Add(navPoint2);
			}
		}
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x0005F8E0 File Offset: 0x0005DAE0
	internal NavPoint GetNavPoint(Vector3 destination, Vector3 currentPos)
	{
		NavPoint navPoint = null;
		float num = float.PositiveInfinity;
		foreach (NavPoint navPoint2 in this.points)
		{
			float num2 = Vector3.Distance(currentPos, navPoint2.transform.position);
			if (num2 <= num && Vector3.Angle(destination - currentPos, navPoint2.transform.position - currentPos) <= 90f)
			{
				num = num2;
				navPoint = navPoint2;
			}
		}
		return navPoint;
	}

	// Token: 0x0400116C RID: 4460
	public static NavPoints instance;

	// Token: 0x0400116D RID: 4461
	public bool drawGizmos;

	// Token: 0x0400116E RID: 4462
	private List<NavPoint> points = new List<NavPoint>();
}
