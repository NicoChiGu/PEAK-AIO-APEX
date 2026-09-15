using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class AscentDebug : MonoBehaviour
{
	// Token: 0x06000471 RID: 1137 RVA: 0x0001B37E File Offset: 0x0001957E
	private void Awake()
	{
		Ascents.currentAscent = this.testAscent;
	}

	// Token: 0x04000503 RID: 1283
	public int testAscent;
}
