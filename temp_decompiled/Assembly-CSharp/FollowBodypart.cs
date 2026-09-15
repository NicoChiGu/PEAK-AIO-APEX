using System;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class FollowBodypart : MonoBehaviour
{
	// Token: 0x0600115D RID: 4445 RVA: 0x0005753A File Offset: 0x0005573A
	private void Start()
	{
		this.target = base.GetComponentInParent<Character>().GetBodypart(this.followPart).transform;
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x00057558 File Offset: 0x00055758
	private void LateUpdate()
	{
		base.transform.position = this.target.position;
	}

	// Token: 0x04000FDE RID: 4062
	public BodypartType followPart;

	// Token: 0x04000FDF RID: 4063
	private Transform target;
}
