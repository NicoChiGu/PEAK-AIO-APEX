using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class Action_WarpRandomly : ItemAction
{
	// Token: 0x0600086A RID: 2154 RVA: 0x0002E0D2 File Offset: 0x0002C2D2
	private void Awake()
	{
		this.raycastDirectionVector = new Vector3(0f, -1f, 1f);
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x0002E0F0 File Offset: 0x0002C2F0
	private void TryFindNewPotentialSpot()
	{
		float num = Random.Range(this.minRaycastStartX, this.maxRaycastStartX);
		float num2 = base.transform.position.y + Random.Range(this.minRaycastRelativeStartY, this.maxRaycastRelativeStartY);
		float num3 = base.transform.position.z + this.raycastRelativeStartZ;
		Vector3 vector;
		vector..ctor(num, num2, num3);
		if (Physics.Raycast(vector, this.raycastDirectionVector, ref this.cachedHit, 999f, HelperFunctions.terrainMapMask))
		{
			Debug.DrawLine(vector, this.cachedHit.point, Color.red);
			Debug.Break();
			this.unvalidatedPoints.Add(this.cachedHit.point);
		}
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x0002E1A8 File Offset: 0x0002C3A8
	private void ValidateRecentPoints(int numberToValidate = 1)
	{
		for (int i = 0; i < numberToValidate; i++)
		{
			if (this.unvalidatedPoints.Count == 0)
			{
				return;
			}
			Vector3 vector = this.unvalidatedPoints[0];
			if (this.ValidatePoint(vector))
			{
				this.validatedPoints.Add(this.unvalidatedPoints[0]);
			}
			this.unvalidatedPoints.RemoveAt(0);
		}
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x0002E208 File Offset: 0x0002C408
	private bool ValidatePoint(Vector3 point)
	{
		return Vector3.Distance(point, base.transform.position) >= this.minDistanceFromCharacter;
	}

	// Token: 0x0400080D RID: 2061
	public float minimumDistance = 12f;

	// Token: 0x0400080E RID: 2062
	public bool restoreUsesOnFailure = true;

	// Token: 0x0400080F RID: 2063
	public List<Vector3> unvalidatedPoints = new List<Vector3>();

	// Token: 0x04000810 RID: 2064
	public List<Vector3> validatedPoints = new List<Vector3>();

	// Token: 0x04000811 RID: 2065
	public float minRaycastRelativeStartY;

	// Token: 0x04000812 RID: 2066
	public float maxRaycastRelativeStartY;

	// Token: 0x04000813 RID: 2067
	public float minRaycastStartX;

	// Token: 0x04000814 RID: 2068
	public float maxRaycastStartX;

	// Token: 0x04000815 RID: 2069
	public float raycastRelativeStartZ;

	// Token: 0x04000816 RID: 2070
	public float minDistanceFromCharacter = 30f;

	// Token: 0x04000817 RID: 2071
	private Vector3 raycastDirectionVector;

	// Token: 0x04000818 RID: 2072
	private RaycastHit cachedHit;
}
