using System;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class PlayerShaderParams : MonoBehaviour
{
	// Token: 0x06000B32 RID: 2866 RVA: 0x0003BC8E File Offset: 0x00039E8E
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		Shader.SetGlobalVector("PlayerPos", Character.localCharacter.Center + this.playerCenterOffset);
	}

	// Token: 0x04000A6F RID: 2671
	public Vector3 playerCenterOffset;
}
