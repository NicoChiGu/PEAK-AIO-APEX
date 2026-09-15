using System;
using UnityEngine;

// Token: 0x02000374 RID: 884
public class WindHeightEffect : MonoBehaviour
{
	// Token: 0x06001667 RID: 5735 RVA: 0x000740F0 File Offset: 0x000722F0
	private void Start()
	{
		this.zone = base.GetComponent<WindChillZone>();
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x00074100 File Offset: 0x00072300
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.zone.lightVolumeSampleThreshold_lower = Mathf.Lerp(this.from, this.to, Mathf.InverseLerp(this.fromHeight, this.toHeight, Character.observedCharacter.Center.y));
	}

	// Token: 0x0400154E RID: 5454
	public float from;

	// Token: 0x0400154F RID: 5455
	public float to;

	// Token: 0x04001550 RID: 5456
	public float fromHeight;

	// Token: 0x04001551 RID: 5457
	public float toHeight;

	// Token: 0x04001552 RID: 5458
	private WindChillZone zone;
}
