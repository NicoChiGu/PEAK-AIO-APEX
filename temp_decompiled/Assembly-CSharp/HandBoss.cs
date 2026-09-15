using System;
using System.Collections.Generic;
using Knot;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200027F RID: 639
public class HandBoss : MonoBehaviour
{
	// Token: 0x060011DB RID: 4571 RVA: 0x0005A503 File Offset: 0x00058703
	private void Start()
	{
		Cursor.visible = false;
	}

	// Token: 0x060011DC RID: 4572 RVA: 0x0005A50B File Offset: 0x0005870B
	private void DisableAll()
	{
		this.grabMaking.SetActive(false);
		this.grabUnmaking.SetActive(false);
		this.idle.SetActive(false);
		this.lr.gameObject.SetActive(false);
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x0005A544 File Offset: 0x00058744
	private void Update()
	{
		this.DisableAll();
		base.transform.position = Input.mousePosition;
		if (this.knotMaker.grabbedRope)
		{
			this.grabMaking.SetActive(true);
			return;
		}
		if (this.knotUnmaker.grabbing)
		{
			this.lr.gameObject.SetActive(true);
			LineRenderer lineRenderer = this.lr;
			int num = 0;
			List<TiedKnotVisualizer.KnotPart> knot = this.knotUnmaker.visualizer.knot;
			Vector3 position = knot[knot.Count - 1].position;
			List<TiedKnotVisualizer.KnotPart> knot2 = this.knotUnmaker.visualizer.knot;
			lineRenderer.SetPosition(num, ExtSwizzle.xyn(position, knot2[knot2.Count - 1].position.z - 1f));
			this.lr.startColor = this.knotUnmaker.lineColor;
			this.lr.endColor = this.knotUnmaker.lineColor;
			this.grabUnmaking.SetActive(true);
			Camera main = Camera.main;
			List<TiedKnotVisualizer.KnotPart> knot3 = this.knotUnmaker.visualizer.knot;
			main.WorldToScreenPoint(knot3[knot3.Count - 1].position);
			LineRenderer lineRenderer2 = this.lr;
			int num2 = 1;
			Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			List<TiedKnotVisualizer.KnotPart> knot4 = this.knotUnmaker.visualizer.knot;
			lineRenderer2.SetPosition(num2, ExtSwizzle.xyn(vector, knot4[knot4.Count - 1].position.z - 1f));
			return;
		}
		this.idle.SetActive(true);
	}

	// Token: 0x04001055 RID: 4181
	public GameObject grabMaking;

	// Token: 0x04001056 RID: 4182
	public GameObject grabUnmaking;

	// Token: 0x04001057 RID: 4183
	public GameObject idle;

	// Token: 0x04001058 RID: 4184
	public Image handImage;

	// Token: 0x04001059 RID: 4185
	public KnotMaker knotMaker;

	// Token: 0x0400105A RID: 4186
	public KnotUnmaker knotUnmaker;

	// Token: 0x0400105B RID: 4187
	public LineRenderer lr;
}
