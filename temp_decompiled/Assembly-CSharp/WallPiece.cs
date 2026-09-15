using System;
using UnityEngine;

// Token: 0x0200036C RID: 876
public class WallPiece : MonoBehaviour
{
	// Token: 0x0600164D RID: 5709 RVA: 0x000739B0 File Offset: 0x00071BB0
	public void SnapToGrid()
	{
		base.transform.position = base.GetComponentInParent<Wall>().SnapToPosition(base.transform.position);
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x000739D3 File Offset: 0x00071BD3
	private void Start()
	{
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x000739D5 File Offset: 0x00071BD5
	private void Update()
	{
	}

	// Token: 0x04001533 RID: 5427
	public Vector2Int dimention = Vector2Int.one;

	// Token: 0x04001534 RID: 5428
	internal Vector2Int wallPosition;
}
