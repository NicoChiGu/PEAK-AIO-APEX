using System;
using UnityEngine;

// Token: 0x02000318 RID: 792
public class Rotate : MonoBehaviour
{
	// Token: 0x0600146E RID: 5230 RVA: 0x00067E6D File Offset: 0x0006606D
	private void Update()
	{
		this.tf.transform.Rotate(this.rotation * Time.deltaTime);
	}

	// Token: 0x04001307 RID: 4871
	public Transform tf;

	// Token: 0x04001308 RID: 4872
	public Vector3 rotation;
}
