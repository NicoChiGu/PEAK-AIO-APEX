using System;
using System.Collections.Generic;
using Peak.Dev;
using UnityEngine;
using UnityEngine.UIElements;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000162 RID: 354
public class ReconnectDataDebugPage : DebugPage
{
	// Token: 0x06000B43 RID: 2883 RVA: 0x0003C00B File Offset: 0x0003A20B
	public ReconnectDataDebugPage()
	{
		this.label = new Label();
		this.label.AddToClassList("info");
		base.Add(this.label);
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0003C03C File Offset: 0x0003A23C
	public override void Update()
	{
		base.Update();
		Object instance = Singleton<ReconnectHandler>.Instance;
		string text = "";
		if (instance != null)
		{
			text = string.Format("Reconnect Data found: {0}", Singleton<ReconnectHandler>.Instance.Records.Count);
			using (IEnumerator<KeyValuePair<string, ReconnectHandler.ReconnectDataRecord>> enumerator = Singleton<ReconnectHandler>.Instance.Records.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, ReconnectHandler.ReconnectDataRecord> keyValuePair = enumerator.Current;
					string text2;
					ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
					keyValuePair.Deconstruct(ref text2, ref reconnectDataRecord);
					string text3 = text2;
					ReconnectHandler.ReconnectDataRecord reconnectDataRecord2 = reconnectDataRecord;
					text = string.Concat(new string[]
					{
						text,
						Environment.NewLine,
						text3,
						" : ",
						Pretty.Print(reconnectDataRecord2.Data.currentStatuses)
					});
				}
				goto IL_00B7;
			}
		}
		text = "No reconnect handler found";
		IL_00B7:
		this.label.text = text;
	}

	// Token: 0x04000A7C RID: 2684
	private Label label;
}
