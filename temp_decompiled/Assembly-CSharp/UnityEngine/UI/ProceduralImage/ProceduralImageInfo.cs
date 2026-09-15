using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x02000378 RID: 888
	public struct ProceduralImageInfo
	{
		// Token: 0x06001687 RID: 5767 RVA: 0x0007475C File Offset: 0x0007295C
		public ProceduralImageInfo(float width, float height, float fallOffDistance, float pixelSize, Vector4 radius, float borderWidth)
		{
			this.width = Mathf.Abs(width);
			this.height = Mathf.Abs(height);
			this.fallOffDistance = Mathf.Max(0f, fallOffDistance);
			this.radius = radius;
			this.borderWidth = Mathf.Max(borderWidth, 0f);
			this.pixelSize = Mathf.Max(0f, pixelSize);
		}

		// Token: 0x04001558 RID: 5464
		public float width;

		// Token: 0x04001559 RID: 5465
		public float height;

		// Token: 0x0400155A RID: 5466
		public float fallOffDistance;

		// Token: 0x0400155B RID: 5467
		public Vector4 radius;

		// Token: 0x0400155C RID: 5468
		public float borderWidth;

		// Token: 0x0400155D RID: 5469
		public float pixelSize;
	}
}
