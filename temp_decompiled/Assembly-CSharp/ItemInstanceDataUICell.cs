using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Token: 0x020000D2 RID: 210
public class ItemInstanceDataUICell : VisualElement
{
	// Token: 0x060007F2 RID: 2034 RVA: 0x0002CA38 File Offset: 0x0002AC38
	public ItemInstanceDataUICell(ItemInstanceData data)
	{
		this.data = data;
		this.label = new Label();
		this.label.AddToClassList("info");
		base.Add(this.label);
		this.label.text = data.guid.ToString();
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0002CA98 File Offset: 0x0002AC98
	public void Update()
	{
		string text = this.data.guid.ToString();
		text += string.Format(" - enteries: {0}", this.data.data.Count);
		foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in this.data.data)
		{
			text += string.Format("\n{0} : {1}", keyValuePair.Key, keyValuePair.Value.GetType().Name);
			text += "\n---";
			text = text + "\n" + keyValuePair.Value.ToString();
			text += "\n---";
		}
		this.label.text = text;
	}

	// Token: 0x040007C3 RID: 1987
	private ItemInstanceData data;

	// Token: 0x040007C4 RID: 1988
	private Label label;
}
