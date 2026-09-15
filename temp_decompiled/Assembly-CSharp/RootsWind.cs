using System;
using UnityEngine;

// Token: 0x02000316 RID: 790
public class RootsWind : MonoBehaviour
{
	// Token: 0x06001469 RID: 5225 RVA: 0x00067BEA File Offset: 0x00065DEA
	private void Awake()
	{
		RootsWind.instance = this;
	}

	// Token: 0x040012FD RID: 4861
	public static RootsWind instance;

	// Token: 0x040012FE RID: 4862
	public WindChillZone windZone;
}
