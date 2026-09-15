using System;
using UnityEngine;

// Token: 0x020002A7 RID: 679
public class MoveTransform : MonoBehaviour
{
	// Token: 0x0600129D RID: 4765 RVA: 0x0005ED6D File Offset: 0x0005CF6D
	private void Update()
	{
		base.transform.position += this.move * Time.deltaTime;
	}

	// Token: 0x04001158 RID: 4440
	public Vector3 move;
}
