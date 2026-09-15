using System;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class ClimbSFX : MonoBehaviour
{
	// Token: 0x060010B9 RID: 4281 RVA: 0x000541A0 File Offset: 0x000523A0
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x060010BA RID: 4282 RVA: 0x000541B8 File Offset: 0x000523B8
	private void Update()
	{
		if (this.character)
		{
			if (!this.character.data.isClimbing && this.sToggle)
			{
				this.sToggle = false;
				this.surfaceOff.SetActive(true);
				this.surfaceOnHeavy.SetActive(false);
				this.surfaceOn.SetActive(false);
			}
			if (this.character.data.isClimbing && !this.sToggle)
			{
				this.sToggle = true;
				this.surfaceOn.SetActive(true);
				if (this.character.data.avarageVelocity.y <= -6f)
				{
					this.surfaceOnHeavy.SetActive(true);
				}
				this.surfaceOff.SetActive(false);
			}
			if (!this.character.data.isRopeClimbing && this.rToggle)
			{
				this.rToggle = false;
				this.ropeOff.SetActive(true);
				this.ropeOn.SetActive(false);
			}
			if (this.character.data.isRopeClimbing && !this.rToggle)
			{
				this.rToggle = true;
				this.ropeOn.SetActive(true);
				this.ropeOff.SetActive(false);
			}
		}
	}

	// Token: 0x04000EEA RID: 3818
	private Character character;

	// Token: 0x04000EEB RID: 3819
	public GameObject ropeOn;

	// Token: 0x04000EEC RID: 3820
	public GameObject ropeOff;

	// Token: 0x04000EED RID: 3821
	private bool rToggle;

	// Token: 0x04000EEE RID: 3822
	public GameObject surfaceOn;

	// Token: 0x04000EEF RID: 3823
	public GameObject surfaceOff;

	// Token: 0x04000EF0 RID: 3824
	public GameObject surfaceOnHeavy;

	// Token: 0x04000EF1 RID: 3825
	private bool sToggle;
}
