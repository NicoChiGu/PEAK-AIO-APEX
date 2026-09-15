using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x02000379 RID: 889
	[DisallowMultipleComponent]
	public abstract class ProceduralImageModifier : MonoBehaviour
	{
		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06001688 RID: 5768 RVA: 0x000747BE File Offset: 0x000729BE
		protected Graphic _Graphic
		{
			get
			{
				if (this.graphic == null)
				{
					this.graphic = base.GetComponent<Graphic>();
				}
				return this.graphic;
			}
		}

		// Token: 0x06001689 RID: 5769
		public abstract Vector4 CalculateRadius(Rect imageRect);

		// Token: 0x0400155E RID: 5470
		protected Graphic graphic;
	}
}
