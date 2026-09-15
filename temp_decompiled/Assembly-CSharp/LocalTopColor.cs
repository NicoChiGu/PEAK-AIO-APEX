using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class LocalTopColor : MonoBehaviour
{
	// Token: 0x0600065D RID: 1629 RVA: 0x0002447E File Offset: 0x0002267E
	private void Start()
	{
		this.setTopVector();
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00024488 File Offset: 0x00022688
	private void setTopVector()
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		Vector3 vector = base.transform.InverseTransformDirection(Vector3.up);
		materialPropertyBlock.SetVector("_LocalTopDirection", vector);
		this.renderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x04000661 RID: 1633
	public MeshRenderer renderer;
}
