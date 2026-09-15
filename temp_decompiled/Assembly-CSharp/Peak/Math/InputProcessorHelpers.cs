using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Peak.Math
{
	// Token: 0x020003CD RID: 973
	internal static class InputProcessorHelpers
	{
		// Token: 0x06001922 RID: 6434 RVA: 0x0007D35A File Offset: 0x0007B55A
		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			InputSystem.RegisterProcessor<FramerateIndependentProcessor>(null);
			InputSystem.RegisterProcessor<Clamp01Processor>(null);
		}
	}
}
