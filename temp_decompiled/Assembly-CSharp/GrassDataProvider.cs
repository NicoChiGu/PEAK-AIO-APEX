using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public abstract class GrassDataProvider : MonoBehaviour
{
	// Token: 0x0600071B RID: 1819
	public abstract bool IsDirty();

	// Token: 0x0600071C RID: 1820
	public abstract ComputeBuffer GetData();
}
