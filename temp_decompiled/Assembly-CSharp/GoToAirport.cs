using System;
using System.Collections;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001C5 RID: 453
public class GoToAirport : MonoBehaviour
{
	// Token: 0x06000DFB RID: 3579 RVA: 0x00045A70 File Offset: 0x00043C70
	public void GoFromMainMenu()
	{
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Plane, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f) });
	}
}
