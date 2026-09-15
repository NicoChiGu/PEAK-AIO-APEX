using System;
using UnityEngine;

// Token: 0x02000261 RID: 609
public class FogSphereOrigin : MonoBehaviour
{
	// Token: 0x0600115B RID: 4443 RVA: 0x000574A8 File Offset: 0x000556A8
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(new Vector3(0f, this.moveOnHeight, this.moveOnForward), new Vector3(200f, 200f, 2f));
		Gizmos.color = Color.green;
		Gizmos.DrawCube(new Vector3(0f, this.moveOnHeight, this.moveOnForward), new Vector3(200f, 2f, 1000f));
	}

	// Token: 0x04000FDA RID: 4058
	public float size = 650f;

	// Token: 0x04000FDB RID: 4059
	public float moveOnHeight;

	// Token: 0x04000FDC RID: 4060
	public float moveOnForward;

	// Token: 0x04000FDD RID: 4061
	public bool disableFog;
}
