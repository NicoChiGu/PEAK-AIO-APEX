using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class BrokenRopeCheck : CustomSpawnCondition
{
	// Token: 0x06000B74 RID: 2932 RVA: 0x0003CFB4 File Offset: 0x0003B1B4
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		this.lastData = data;
		float num = this.estimatedMaxRopeLength / 40f;
		if (data == null)
		{
			data = this.lastData;
		}
		this.lastData = data;
		base.transform.rotation = ExtQuaternion.FromUpAndRightPrioUp(Vector3.down, data.hit.normal);
		Debug.Log(string.Format("Anchor {0}", this.anchor));
		if (this.anchor.anchorPoint == null)
		{
			this.anchor.anchorPoint = this.anchor.transform.Find("AnchorPoint");
		}
		Debug.Log(string.Format("anchorPoint {0}", this.anchor.anchorPoint));
		RaycastHit raycastHit;
		bool flag = PExt.Raycast(new Ray(this.anchor.anchorPoint.transform.position, Vector3.down), ref raycastHit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 10f);
		float num2 = Vector3.Distance(raycastHit.point, base.transform.position);
		if (num2 > num * 39f || !flag)
		{
			this.ropeAnchorWithRope.ropeSegmentLength = 39f;
		}
		else
		{
			this.ropeAnchorWithRope.ropeSegmentLength = num2 / num - 1f;
		}
		return true;
	}

	// Token: 0x04000AA2 RID: 2722
	[SerializeField]
	private RopeAnchor anchor;

	// Token: 0x04000AA3 RID: 2723
	[SerializeField]
	private RopeAnchorWithRope ropeAnchorWithRope;

	// Token: 0x04000AA4 RID: 2724
	private PropSpawner.SpawnData lastData;

	// Token: 0x04000AA5 RID: 2725
	public float estimatedMaxRopeLength = 17f;
}
