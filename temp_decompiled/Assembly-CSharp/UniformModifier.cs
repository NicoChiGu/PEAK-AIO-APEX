using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020002D1 RID: 721
[ModifierID("Uniform")]
public class UniformModifier : ProceduralImageModifier
{
	// Token: 0x17000133 RID: 307
	// (get) Token: 0x0600136B RID: 4971 RVA: 0x00062918 File Offset: 0x00060B18
	// (set) Token: 0x0600136C RID: 4972 RVA: 0x00062920 File Offset: 0x00060B20
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

	// Token: 0x0600136D RID: 4973 RVA: 0x00062934 File Offset: 0x00060B34
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		float num = this.radius;
		return new Vector4(num, num, num, num);
	}

	// Token: 0x04001209 RID: 4617
	[SerializeField]
	private float radius;
}
