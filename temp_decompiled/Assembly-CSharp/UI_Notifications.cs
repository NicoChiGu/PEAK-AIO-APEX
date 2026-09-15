using System;
using TMPro;
using UnityEngine;

// Token: 0x02000360 RID: 864
public class UI_Notifications : MonoBehaviour
{
	// Token: 0x06001613 RID: 5651 RVA: 0x00071F40 File Offset: 0x00070140
	public void AddNotification(string text)
	{
		Transform child = base.transform.GetChild(0);
		Object.Instantiate<GameObject>(this.prefab, child.position, child.rotation, child).GetComponentInChildren<TextMeshProUGUI>().text = text;
	}

	// Token: 0x040014F3 RID: 5363
	public GameObject prefab;
}
