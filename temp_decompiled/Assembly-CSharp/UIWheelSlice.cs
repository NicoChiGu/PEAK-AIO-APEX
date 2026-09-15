using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001EC RID: 492
public class UIWheelSlice : MonoBehaviour
{
	// Token: 0x06000EF5 RID: 3829 RVA: 0x0004965D File Offset: 0x0004785D
	public Vector3 GetUpVector()
	{
		return Quaternion.Euler(0f, 0f, this.offsetRotation) * base.transform.up;
	}

	// Token: 0x04000CFE RID: 3326
	public Button button;

	// Token: 0x04000CFF RID: 3327
	private float offsetRotation = 22.5f;
}
