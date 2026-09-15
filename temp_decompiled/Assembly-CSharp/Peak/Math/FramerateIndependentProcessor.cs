using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Peak.Math
{
	// Token: 0x020003CE RID: 974
	public class FramerateIndependentProcessor : InputProcessor<Vector2>
	{
		// Token: 0x06001923 RID: 6435 RVA: 0x0007D368 File Offset: 0x0007B568
		public override Vector2 Process(Vector2 value, InputControl control)
		{
			float num = (this.UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime);
			if (this.DivideInstead)
			{
				num = 1f / num;
			}
			return num * value;
		}

		// Token: 0x040016BA RID: 5818
		public bool UseScaledTime;

		// Token: 0x040016BB RID: 5819
		public bool DivideInstead;
	}
}
