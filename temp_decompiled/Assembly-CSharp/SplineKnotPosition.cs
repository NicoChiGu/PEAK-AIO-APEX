using System;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x02000281 RID: 641
public class SplineKnotPosition : MonoBehaviour
{
	// Token: 0x060011E7 RID: 4583 RVA: 0x0005A7FC File Offset: 0x000589FC
	private void Start()
	{
		if (this.splineContainer == null || this.splineContainer.Splines.Count == 0)
		{
			Debug.LogError("SplineContainer is missing or empty.");
			return;
		}
		Spline spline = this.splineContainer.Splines[0];
		float normalizedInterpolation = SplineUtility.GetNormalizedInterpolation<Spline>(this.splineContainer.Spline, this.f, 2);
		Debug.Log(string.Format("Knot {0} is at {1}% along the spline.", this.knotIndex, normalizedInterpolation * 100f));
	}

	// Token: 0x04001061 RID: 4193
	public SplineContainer splineContainer;

	// Token: 0x04001062 RID: 4194
	public int knotIndex;

	// Token: 0x04001063 RID: 4195
	public float f;
}
