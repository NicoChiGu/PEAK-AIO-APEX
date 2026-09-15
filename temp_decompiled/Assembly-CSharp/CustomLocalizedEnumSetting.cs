using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zorro.Settings;

// Token: 0x02000189 RID: 393
public abstract class CustomLocalizedEnumSetting<[IsUnmanaged] T> : EnumSetting<T>, ICustomLocalizedEnumSetting where T : struct, ValueType, Enum
{
	// Token: 0x06000C6B RID: 3179 RVA: 0x00041559 File Offset: 0x0003F759
	public virtual List<string> GetCustomLocalizedChoices()
	{
		return Enumerable.ToList<string>(Enumerable.Select<string, string>(this.GetUnlocalizedChoices(), (string s) => LocalizedText.GetText(s, true)));
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x0004158A File Offset: 0x0003F78A
	public void DeregisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Remove(LocalizedText.OnLangugageChanged, action);
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x000415A1 File Offset: 0x0003F7A1
	public void RegisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Combine(LocalizedText.OnLangugageChanged, action);
	}
}
