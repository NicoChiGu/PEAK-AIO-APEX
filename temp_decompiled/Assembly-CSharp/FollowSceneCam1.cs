using System;
using UnityEngine;

// Token: 0x02000263 RID: 611
[ExecuteInEditMode]
public class FollowSceneCam1 : MonoBehaviour
{
	// Token: 0x06001160 RID: 4448 RVA: 0x00057578 File Offset: 0x00055778
	private void OnDrawGizmosSelected()
	{
		if (Camera.current != null)
		{
			base.transform.position = Camera.current.transform.position;
		}
	}
}
