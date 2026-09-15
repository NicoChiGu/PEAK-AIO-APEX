using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x0200027E RID: 638
public class AddPointAtEndOfSpline : MonoBehaviour
{
	// Token: 0x060011D6 RID: 4566 RVA: 0x0005A404 File Offset: 0x00058604
	public void SetAllZ(float v)
	{
		SplineContainer component = base.GetComponent<SplineContainer>();
		List<BezierKnot> list = Enumerable.ToList<BezierKnot>(component.Spline.Knots);
		for (int i = 0; i < list.Count; i++)
		{
			BezierKnot bezierKnot = list[i];
			bezierKnot.Position = ExtSwizzle.xyn(bezierKnot.Position, v);
			list[i] = bezierKnot;
		}
		component.Spline.Knots = list;
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x0005A46C File Offset: 0x0005866C
	private void GO()
	{
		SplineContainer component = base.GetComponent<SplineContainer>();
		BezierKnot bezierKnot = Enumerable.Last<BezierKnot>(component.Spline.Knots);
		List<BezierKnot> list = Enumerable.ToList<BezierKnot>(component.Spline.Knots);
		BezierKnot bezierKnot2 = list[list.Count - 2];
		component.Spline.Add(ExtDataTypes.PToV3(bezierKnot.Position) + (ExtDataTypes.PToV3(bezierKnot.Position) - ExtDataTypes.PToV3(bezierKnot2.Position)).normalized, 0);
		PExt.SaveObj(component);
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x0005A4F7 File Offset: 0x000586F7
	private void Start()
	{
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x0005A4F9 File Offset: 0x000586F9
	private void Update()
	{
	}
}
