using System;
using TMPro;
using UnityEngine;

// Token: 0x0200035F RID: 863
public class UI_Interaction : MonoBehaviour
{
	// Token: 0x0600160F RID: 5647 RVA: 0x00071ED3 File Offset: 0x000700D3
	private void Start()
	{
		this.text = base.GetComponentInChildren<TextMeshProUGUI>();
	}

	// Token: 0x06001610 RID: 5648 RVA: 0x00071EE1 File Offset: 0x000700E1
	private void Update()
	{
		this.OnChange();
	}

	// Token: 0x06001611 RID: 5649 RVA: 0x00071EEC File Offset: 0x000700EC
	private void OnChange()
	{
		this.current = Interaction.instance.currentHovered;
		if (this.current != null)
		{
			this.text.text = this.current.GetInteractionText();
			return;
		}
		this.text.text = "";
	}

	// Token: 0x040014F1 RID: 5361
	private TextMeshProUGUI text;

	// Token: 0x040014F2 RID: 5362
	private IInteractible current;
}
