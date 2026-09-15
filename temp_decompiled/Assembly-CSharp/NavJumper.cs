using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020002AC RID: 684
public class NavJumper : MonoBehaviour
{
	// Token: 0x060012AC RID: 4780 RVA: 0x0005F1C9 File Offset: 0x0005D3C9
	private void Start()
	{
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x0005F1CC File Offset: 0x0005D3CC
	private void Jump()
	{
		List<RaycastHit> list = new List<RaycastHit>();
		for (int i = 0; i < this.castsPerJump; i++)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + ExtSwizzle.xny(ExtMath.RandInsideUnitCircle() * this.castRadius, this.castHeight), Vector3.down * this.castHeight, ref raycastHit))
			{
				list.Add(raycastHit);
			}
		}
		Debug.Log(string.Format("Total: {0}", list.Count));
		list = Enumerable.ToList<RaycastHit>(Enumerable.Where<RaycastHit>(list, (RaycastHit hit) => Vector3.Angle(hit.normal, Vector3.up) < 50f));
		Debug.Log(string.Format("After angle: {0}", list.Count));
		list = Enumerable.ToList<RaycastHit>(Enumerable.Where<RaycastHit>(list, (RaycastHit hit) => Vector3.Distance(hit.point, base.transform.position) < this.maxDistance));
		Debug.Log(string.Format("After distance: {0}", list.Count));
		list = Enumerable.ToList<RaycastHit>(Enumerable.Where<RaycastHit>(list, (RaycastHit hit) => hit.point.z > base.transform.position.z && hit.point.y > base.transform.position.y));
		list = Enumerable.ToList<RaycastHit>(Enumerable.Where<RaycastHit>(list, (RaycastHit hit) => hit.point.y > base.transform.position.y));
		Debug.Log(string.Format("After Z: {0}", list.Count));
		if (list.Count == 0)
		{
			return;
		}
		RaycastHit raycastHit2 = Enumerable.First<RaycastHit>(Enumerable.OrderByDescending<RaycastHit, float>(list, (RaycastHit hit) => hit.point.z));
		Debug.DrawLine(base.transform.position + Vector3.up, raycastHit2.point + Vector3.up, Color.green, 10f);
		base.transform.position = raycastHit2.point;
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x0005F391 File Offset: 0x0005D591
	private void Update()
	{
	}

	// Token: 0x04001166 RID: 4454
	public int castsPerJump = 100;

	// Token: 0x04001167 RID: 4455
	public float maxDistance = 3f;

	// Token: 0x04001168 RID: 4456
	public float castRadius = 1f;

	// Token: 0x04001169 RID: 4457
	public float castHeight = 100f;

	// Token: 0x0400116A RID: 4458
	private int fails;
}
