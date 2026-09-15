using System;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200036B RID: 875
public class WallNavigatorTest : MonoBehaviour, ISerializationCallbackReceiver
{
	// Token: 0x06001644 RID: 5700 RVA: 0x000738A6 File Offset: 0x00071AA6
	private void Start()
	{
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x000738A8 File Offset: 0x00071AA8
	private void Update()
	{
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x000738AC File Offset: 0x00071AAC
	private void TryFindValidPath()
	{
		this.color = Color.red;
		if (Enumerable.ToList<Vector3>(Enumerable.Where<Vector3>(this.triangulation.vertices, (Vector3 vert) => Vector3.Distance(base.transform.position, vert) < this.sphereSize)).Count > 0)
		{
			this.color = Color.green;
		}
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x000738F8 File Offset: 0x00071AF8
	private void Print()
	{
		Debug.Log(string.Format("Verts{0}, Indices{1}, Areas{2}", this.triangulation.vertices.Length, this.triangulation.indices.Length, this.triangulation.areas.Length));
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x0007394A File Offset: 0x00071B4A
	private void OnDrawGizmosSelected()
	{
		this.TryFindValidPath();
		Gizmos.color = this.color;
		Gizmos.DrawWireSphere(base.transform.position, this.sphereSize);
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x00073973 File Offset: 0x00071B73
	public void OnBeforeSerialize()
	{
		this.triangulation = NavMesh.CalculateTriangulation();
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x00073980 File Offset: 0x00071B80
	public void OnAfterDeserialize()
	{
	}

	// Token: 0x0400152E RID: 5422
	public float fDistance = 3f;

	// Token: 0x0400152F RID: 5423
	public NavMeshSurface surface;

	// Token: 0x04001530 RID: 5424
	private NavMeshTriangulation triangulation;

	// Token: 0x04001531 RID: 5425
	public float sphereSize;

	// Token: 0x04001532 RID: 5426
	private Color color;
}
