using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020002CE RID: 718
[ModifierID("Free")]
public class FreeModifier : ProceduralImageModifier
{
	// Token: 0x17000130 RID: 304
	// (get) Token: 0x0600135E RID: 4958 RVA: 0x00062746 File Offset: 0x00060946
	// (set) Token: 0x0600135F RID: 4959 RVA: 0x0006274E File Offset: 0x0006094E
	public Vector4 Radius
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

	// Token: 0x06001360 RID: 4960 RVA: 0x00062762 File Offset: 0x00060962
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		return this.radius;
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x0006276C File Offset: 0x0006096C
	protected void OnValidate()
	{
		this.radius.x = Mathf.Max(0f, this.radius.x);
		this.radius.y = Mathf.Max(0f, this.radius.y);
		this.radius.z = Mathf.Max(0f, this.radius.z);
		this.radius.w = Mathf.Max(0f, this.radius.w);
	}

	// Token: 0x04001206 RID: 4614
	[SerializeField]
	private Vector4 radius;
}
