using System;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class ChunkingVizBox : MonoBehaviour
{
	// Token: 0x0600053A RID: 1338 RVA: 0x0001EF61 File Offset: 0x0001D161
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0001EF88 File Offset: 0x0001D188
	private void LateUpdate()
	{
		Vector3 position = MainCamera.instance.transform.position;
		Bounds bounds;
		bounds..ctor(base.transform.position, base.transform.localScale);
		bool flag = bounds.Contains(position);
		if (this.m_lastState != flag)
		{
			GameObject[] array = this.objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
		}
		this.m_lastState = flag;
	}

	// Token: 0x04000587 RID: 1415
	public GameObject[] objects;

	// Token: 0x04000588 RID: 1416
	private bool m_lastState = true;
}
