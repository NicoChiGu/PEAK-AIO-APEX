using System;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class SimpleDrawMesh : MonoBehaviour
{
	// Token: 0x06000D3C RID: 3388 RVA: 0x000428CF File Offset: 0x00040ACF
	private void Start()
	{
		this.GatherPools();
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x000428D7 File Offset: 0x00040AD7
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.distanceCheckObject.position, this.cullDistance);
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x000428F9 File Offset: 0x00040AF9
	private void Update()
	{
		this.drawMeshes();
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x00042904 File Offset: 0x00040B04
	public void drawMeshes()
	{
		if (!this.poolsGathered)
		{
			return;
		}
		if (Character.localCharacter && this.distanceCheckObject && Vector3.Distance(Character.localCharacter.Center, this.distanceCheckObject.position) > this.cullDistance)
		{
			return;
		}
		for (int i = 0; i < this.drawPools.Length; i++)
		{
			Graphics.DrawMeshInstanced(this.drawPools[i].mesh, 0, this.drawPools[i].material, this.drawPools[i].matricies, this.drawPools[i].matricies.Length);
		}
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x000429A8 File Offset: 0x00040BA8
	public void GatherPools()
	{
		for (int i = 0; i < this.drawPools.Length; i++)
		{
			Transform[] componentsInChildren = this.drawPools[i].transformsParent.GetComponentsInChildren<Transform>();
			this.drawPools[i].matricies = new Matrix4x4[componentsInChildren.Length];
			for (int j = 1; j < componentsInChildren.Length; j++)
			{
				this.drawPools[i].matricies[j] = Matrix4x4.TRS(componentsInChildren[j].position, componentsInChildren[j].rotation, componentsInChildren[j].localScale);
			}
		}
		this.poolsGathered = true;
	}

	// Token: 0x04000B72 RID: 2930
	public DrawPool[] drawPools;

	// Token: 0x04000B73 RID: 2931
	private bool poolsGathered;

	// Token: 0x04000B74 RID: 2932
	private Matrix4x4[] matrices;

	// Token: 0x04000B75 RID: 2933
	public float cullDistance = 10f;

	// Token: 0x04000B76 RID: 2934
	public Transform distanceCheckObject;
}
