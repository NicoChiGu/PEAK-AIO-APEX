using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class ConstructablePreview : MonoBehaviour
{
	// Token: 0x060008AF RID: 2223 RVA: 0x0002F5C8 File Offset: 0x0002D7C8
	public void SetValid(bool valid)
	{
		this.enableIfValid.SetActive(valid);
		this.enableIfInvalid.SetActive(!valid);
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0002F5E8 File Offset: 0x0002D7E8
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		foreach (ConstructablePreview.ConstructablePreviewAvoidanceSphere constructablePreviewAvoidanceSphere in this.avoidanceSpheres)
		{
			Gizmos.DrawWireSphere(base.transform.TransformPoint(constructablePreviewAvoidanceSphere.position), constructablePreviewAvoidanceSphere.radius);
		}
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0002F65C File Offset: 0x0002D85C
	public bool CollisionValid()
	{
		foreach (ConstructablePreview.ConstructablePreviewAvoidanceSphere constructablePreviewAvoidanceSphere in this.avoidanceSpheres)
		{
			if (Physics.CheckSphere(base.transform.TransformPoint(constructablePreviewAvoidanceSphere.position), constructablePreviewAvoidanceSphere.radius, HelperFunctions.GetMask(constructablePreviewAvoidanceSphere.layerType), 1))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04000856 RID: 2134
	public GameObject enableIfValid;

	// Token: 0x04000857 RID: 2135
	public GameObject enableIfInvalid;

	// Token: 0x04000858 RID: 2136
	public List<ConstructablePreview.ConstructablePreviewAvoidanceSphere> avoidanceSpheres;

	// Token: 0x0200044F RID: 1103
	[Serializable]
	public class ConstructablePreviewAvoidanceSphere
	{
		// Token: 0x0400188F RID: 6287
		public Vector3 position;

		// Token: 0x04001890 RID: 6288
		public float radius;

		// Token: 0x04001891 RID: 6289
		public HelperFunctions.LayerType layerType;
	}
}
