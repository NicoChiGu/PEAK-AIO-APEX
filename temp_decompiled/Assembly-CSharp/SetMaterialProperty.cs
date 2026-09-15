using System;
using UnityEngine;

// Token: 0x02000327 RID: 807
public class SetMaterialProperty : MonoBehaviour
{
	// Token: 0x060014C1 RID: 5313 RVA: 0x00069A39 File Offset: 0x00067C39
	public void Go()
	{
		this.SetVal(this.propertyValue);
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x00069A48 File Offset: 0x00067C48
	public void SetVal(float val)
	{
		Renderer component = base.GetComponent<Renderer>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		component.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(this.propertyName, val);
		component.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x04001339 RID: 4921
	public string propertyName;

	// Token: 0x0400133A RID: 4922
	public float propertyValue;
}
