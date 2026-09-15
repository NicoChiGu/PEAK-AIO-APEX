using System;
using UnityEngine;

// Token: 0x02000322 RID: 802
public class SegTest : MonoBehaviour
{
	// Token: 0x060014B5 RID: 5301 RVA: 0x00069904 File Offset: 0x00067B04
	private void Start()
	{
		ConfigurableJoint component = base.transform.GetChild(0).GetComponent<ConfigurableJoint>();
		this.joint2 = base.transform.GetChild(1).GetComponent<ConfigurableJoint>();
		this.joint2.connectedBody = component.GetComponent<Rigidbody>();
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x0006994B File Offset: 0x00067B4B
	private void Update()
	{
		this.joint2.connectedAnchor = new Vector3(0f, Mathf.Lerp(0.5f, -0.5f, this.val), 0f);
	}

	// Token: 0x04001331 RID: 4913
	[Range(0f, 1f)]
	public float val;

	// Token: 0x04001332 RID: 4914
	private ConfigurableJoint joint2;
}
