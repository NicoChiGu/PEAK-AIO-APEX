using System;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class GoToURL : MonoBehaviour
{
	// Token: 0x06001181 RID: 4481 RVA: 0x0005805B File Offset: 0x0005625B
	public void Click()
	{
		Application.OpenURL(this.URL);
	}

	// Token: 0x04000FFD RID: 4093
	public string URL;
}
