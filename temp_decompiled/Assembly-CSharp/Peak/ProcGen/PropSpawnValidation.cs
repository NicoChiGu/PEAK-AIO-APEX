using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peak.ProcGen
{
	// Token: 0x020003D6 RID: 982
	public static class PropSpawnValidation
	{
		// Token: 0x0600192E RID: 6446 RVA: 0x0007D44C File Offset: 0x0007B64C
		public static Color GetValidationColorImpl(this IValidatable self)
		{
			return PropSpawnValidation.ValidationColors[self.ValidationState];
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x0007D45E File Offset: 0x0007B65E
		// Note: this type is marked as 'beforefieldinit'.
		static PropSpawnValidation()
		{
			Dictionary<ValidationState, Color> dictionary = new Dictionary<ValidationState, Color>();
			dictionary.Add(ValidationState.Unknown, Color.yellow);
			dictionary.Add(ValidationState.Passed, Color.green);
			dictionary.Add(ValidationState.Failed, Color.red);
			PropSpawnValidation.ValidationColors = dictionary;
		}

		// Token: 0x040016C5 RID: 5829
		public static readonly Dictionary<ValidationState, Color> ValidationColors;
	}
}
