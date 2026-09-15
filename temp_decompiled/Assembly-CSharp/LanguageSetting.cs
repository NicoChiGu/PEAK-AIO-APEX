using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000193 RID: 403
public class LanguageSetting : CustomLocalizedEnumSetting<LanguageSetting.Language>, IExposedSetting
{
	// Token: 0x06000CAD RID: 3245 RVA: 0x00041860 File Offset: 0x0003FA60
	public LocalizedText.Language ValueToLanguage(int val)
	{
		if (val == 0)
		{
			return LocalizedText.Language.English;
		}
		if (val == 1)
		{
			return LocalizedText.Language.French;
		}
		if (val == 2)
		{
			return LocalizedText.Language.Italian;
		}
		if (val == 3)
		{
			return LocalizedText.Language.German;
		}
		if (val == 4)
		{
			return LocalizedText.Language.SpanishSpain;
		}
		if (val == 5)
		{
			return LocalizedText.Language.SpanishLatam;
		}
		if (val == 6)
		{
			return LocalizedText.Language.BRPortuguese;
		}
		if (val == 7)
		{
			return LocalizedText.Language.Russian;
		}
		if (val == 8)
		{
			return LocalizedText.Language.Ukrainian;
		}
		if (val == 9)
		{
			return LocalizedText.Language.SimplifiedChinese;
		}
		if (val == 10)
		{
			return LocalizedText.Language.Japanese;
		}
		if (val == 11)
		{
			return LocalizedText.Language.Korean;
		}
		if (val == 12)
		{
			return LocalizedText.Language.Polish;
		}
		if (val == 13)
		{
			return LocalizedText.Language.Turkish;
		}
		return LocalizedText.Language.English;
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x000418CB File Offset: 0x0003FACB
	public override void ApplyValue()
	{
		LocalizedText.SetLanguage((int)this.ValueToLanguage((int)base.Value));
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x000418DE File Offset: 0x0003FADE
	protected override LanguageSetting.Language GetDefaultValue()
	{
		return (LanguageSetting.Language)LocalizedText.GetSystemLanguage();
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x000418E5 File Offset: 0x0003FAE5
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x000418E8 File Offset: 0x0003FAE8
	public string GetDisplayName()
	{
		return "Language";
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x000418F0 File Offset: 0x0003FAF0
	public override List<string> GetCustomLocalizedChoices()
	{
		List<string> list = new List<string>();
		int num = base.Value.GetType().GetEnumNames().Length;
		for (int i = 0; i < num; i++)
		{
			list.Add(LocalizedText.GetText("CURRENT_LANGUAGE", this.ValueToLanguage(i)));
		}
		return list;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0004193F File Offset: 0x0003FB3F
	public string GetCategory()
	{
		return "General";
	}

	// Token: 0x02000490 RID: 1168
	public enum Language
	{
		// Token: 0x04001987 RID: 6535
		English,
		// Token: 0x04001988 RID: 6536
		French,
		// Token: 0x04001989 RID: 6537
		Italian,
		// Token: 0x0400198A RID: 6538
		German,
		// Token: 0x0400198B RID: 6539
		SpanishSpain,
		// Token: 0x0400198C RID: 6540
		SpanishLatam,
		// Token: 0x0400198D RID: 6541
		BRPortuguese,
		// Token: 0x0400198E RID: 6542
		Russian,
		// Token: 0x0400198F RID: 6543
		Ukrainian,
		// Token: 0x04001990 RID: 6544
		SimplifiedChinese,
		// Token: 0x04001991 RID: 6545
		Japanese = 11,
		// Token: 0x04001992 RID: 6546
		Korean,
		// Token: 0x04001993 RID: 6547
		Polish,
		// Token: 0x04001994 RID: 6548
		Turkish
	}
}
