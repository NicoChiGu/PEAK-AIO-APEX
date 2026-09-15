using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Peak.Math
{
	// Token: 0x020003CF RID: 975
	public class Clamp01Processor : InputProcessor<Vector2>
	{
		// Token: 0x06001925 RID: 6437 RVA: 0x0007D3A9 File Offset: 0x0007B5A9
		public override Vector2 Process(Vector2 value, InputControl control)
		{
			return Vector2.ClampMagnitude(value, 1f);
		}
	}
}
