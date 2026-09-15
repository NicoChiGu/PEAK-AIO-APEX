using System;

namespace Peak.Dev
{
	// Token: 0x020003DA RID: 986
	public static class Pretty
	{
		// Token: 0x06001933 RID: 6451 RVA: 0x0007D54C File Offset: 0x0007B74C
		public static string Print(object obj)
		{
			if (obj == null)
			{
				return "NULL";
			}
			IPrettyPrintable prettyPrintable = obj as IPrettyPrintable;
			if (prettyPrintable != null)
			{
				return prettyPrintable.ToPrettyString();
			}
			float[] array = obj as float[];
			if (array != null && array.Length == CharacterAfflictions.NumStatusTypes)
			{
				return CharacterAfflictions.PrettyPrintStaminaBar(array, 40, true);
			}
			return "[UNSUPPORTED PRETTY PRINT]";
		}
	}
}
