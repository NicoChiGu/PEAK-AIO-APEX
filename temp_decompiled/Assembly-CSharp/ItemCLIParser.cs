using System;
using System.Collections.Generic;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000076 RID: 118
[TypeParser(typeof(Item))]
public class ItemCLIParser : CLITypeParser
{
	// Token: 0x06000540 RID: 1344 RVA: 0x0001F120 File Offset: 0x0001D320
	public override object Parse(string str)
	{
		return ObjectDatabaseAsset<ItemDatabase, Item>.GetObjectFromString(str);
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0001F128 File Offset: 0x0001D328
	public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
	{
		return SingletonAsset<ItemDatabase>.Instance.GetAutocompleteOptions(parameterText);
	}
}
