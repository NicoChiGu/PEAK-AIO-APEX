using System;
using System.Collections.Generic;
using Zorro.Core.CLI;

// Token: 0x02000046 RID: 70
[TypeParser(typeof(ACHIEVEMENTTYPE))]
public class AchievementCLIParser : CLITypeParser
{
	// Token: 0x06000415 RID: 1045 RVA: 0x0001A034 File Offset: 0x00018234
	public override object Parse(string str)
	{
		ACHIEVEMENTTYPE achievementtype;
		if (Enum.TryParse<ACHIEVEMENTTYPE>(str, ref achievementtype))
		{
			return achievementtype;
		}
		return ACHIEVEMENTTYPE.NONE;
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x0001A058 File Offset: 0x00018258
	public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
	{
		List<ParameterAutocomplete> list = new List<ParameterAutocomplete>();
		foreach (ACHIEVEMENTTYPE achievementtype in (ACHIEVEMENTTYPE[])Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			list.Add(new ParameterAutocomplete(achievementtype.ToString()));
		}
		return list;
	}
}
