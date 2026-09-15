using System;
using UnityEngine;

// Token: 0x02000292 RID: 658
[ExecuteInEditMode]
public class LineTest : MonoBehaviour
{
	// Token: 0x06001235 RID: 4661 RVA: 0x0005C32F File Offset: 0x0005A52F
	private void Update()
	{
		this.lr.SetPosition(0, this.start.position);
		this.lr.SetPosition(1, this.end.position);
	}

	// Token: 0x040010B0 RID: 4272
	public LineRenderer lr;

	// Token: 0x040010B1 RID: 4273
	public Transform start;

	// Token: 0x040010B2 RID: 4274
	public Transform end;
}
