using System;
using System.Collections.Generic;
using System.Linq;
using Zorro.Settings;

// Token: 0x0200018A RID: 394
public abstract class CustomLocalizedOffOnSetting : OffOnSetting, ICustomLocalizedEnumSetting
{
	// Token: 0x06000C6F RID: 3183 RVA: 0x000415C0 File Offset: 0x0003F7C0
	public List<string> GetCustomLocalizedChoices()
	{
		return Enumerable.ToList<string>(Enumerable.Select<string, string>(this.GetUnlocalizedChoices(), (string s) => LocalizedText.GetText(s, true)));
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x000415F1 File Offset: 0x0003F7F1
	public void DeregisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Remove(LocalizedText.OnLangugageChanged, action);
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x00041608 File Offset: 0x0003F808
	public void RegisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Combine(LocalizedText.OnLangugageChanged, action);
	}
}
