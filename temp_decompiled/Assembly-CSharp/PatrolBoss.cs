using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class PatrolBoss : MonoBehaviour
{
	// Token: 0x060004F7 RID: 1271 RVA: 0x0001D3E3 File Offset: 0x0001B5E3
	public void Awake()
	{
		PatrolBoss.me = this;
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0001D3EC File Offset: 0x0001B5EC
	public Vector3 GetPoint()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.point.transform.position + ExtSwizzle.xoy(ExtMath.RandInsideUnitCircle()) * 10f, Vector3.down, ref raycastHit, 1000f, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap)))
		{
			return raycastHit.point;
		}
		Debug.LogError("This wrong");
		return Vector3.positiveInfinity;
	}

	// Token: 0x0400055D RID: 1373
	public static PatrolBoss me;

	// Token: 0x0400055E RID: 1374
	public GameObject point;
}
