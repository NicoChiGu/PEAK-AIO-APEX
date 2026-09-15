using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020002CF RID: 719
[ModifierID("Only One Edge")]
public class OnlyOneEdgeModifier : ProceduralImageModifier
{
	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06001363 RID: 4963 RVA: 0x00062801 File Offset: 0x00060A01
	// (set) Token: 0x06001364 RID: 4964 RVA: 0x00062809 File Offset: 0x00060A09
	public float Radius
	{
		get
		{
			return this.radius;
		}
		set
		{
			this.radius = value;
			base._Graphic.SetVerticesDirty();
		}
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06001365 RID: 4965 RVA: 0x0006281D File Offset: 0x00060A1D
	// (set) Token: 0x06001366 RID: 4966 RVA: 0x00062825 File Offset: 0x00060A25
	public OnlyOneEdgeModifier.ProceduralImageEdge Side
	{
		get
		{
			return this.side;
		}
		set
		{
			this.side = value;
		}
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x00062830 File Offset: 0x00060A30
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		switch (this.side)
		{
		case OnlyOneEdgeModifier.ProceduralImageEdge.Top:
			return new Vector4(this.radius, this.radius, 0f, 0f);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Bottom:
			return new Vector4(0f, 0f, this.radius, this.radius);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Left:
			return new Vector4(this.radius, 0f, 0f, this.radius);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Right:
			return new Vector4(0f, this.radius, this.radius, 0f);
		default:
			return new Vector4(0f, 0f, 0f, 0f);
		}
	}

	// Token: 0x04001207 RID: 4615
	[SerializeField]
	private float radius;

	// Token: 0x04001208 RID: 4616
	[SerializeField]
	private OnlyOneEdgeModifier.ProceduralImageEdge side;

	// Token: 0x020004F6 RID: 1270
	public enum ProceduralImageEdge
	{
		// Token: 0x04001B21 RID: 6945
		Top,
		// Token: 0x04001B22 RID: 6946
		Bottom,
		// Token: 0x04001B23 RID: 6947
		Left,
		// Token: 0x04001B24 RID: 6948
		Right
	}
}
