using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x02000259 RID: 601
public class FakeCursor : MonoBehaviour
{
	// Token: 0x06001133 RID: 4403 RVA: 0x00056854 File Offset: 0x00054A54
	private void Update()
	{
		Vector2 vector = Mouse.current.position.ReadValue();
		Vector2 vector2;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.target.parent as RectTransform, vector, null, ref vector2);
		this.target.localPosition = vector2;
	}

	// Token: 0x04000FA9 RID: 4009
	public Transform target;
}
