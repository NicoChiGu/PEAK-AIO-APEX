using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class BeachSpawner : MonoBehaviour
{
	// Token: 0x06000F91 RID: 3985 RVA: 0x0004D550 File Offset: 0x0004B750
	private void Spawn()
	{
		this.Clear();
		int num = Random.Range(this.treeSpawnRange.x, this.treeSpawnRange.y);
		int num2 = 20;
		for (int i = 0; i < num; i++)
		{
			if (!this.TrySpawn(this.palmTrees[Random.Range(0, this.palmTrees.Length)]) && num2 > 0)
			{
				num2--;
				i--;
			}
		}
	}

	// Token: 0x06000F92 RID: 3986 RVA: 0x0004D5BC File Offset: 0x0004B7BC
	private void Clear()
	{
		foreach (GameObject gameObject in this.spawned)
		{
			Object.DestroyImmediate(gameObject);
		}
		this.spawned.Clear();
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x0004D618 File Offset: 0x0004B818
	private bool TrySpawn(GameObject go)
	{
		float num = Random.Range(0f, 360f);
		float num2 = Random.Range(0f, this.radius);
		Vector3 vector = new Vector3(Mathf.Cos(num), 0f, Mathf.Sin(num)) * num2 + this.treeParent.position;
		RaycastHit raycastHit;
		if (Physics.Linecast(vector + Vector3.up * 100f, vector - Vector3.up * 100f, ref raycastHit, this.layerMask.value, 0))
		{
			Debug.Log(raycastHit.collider.gameObject.name, raycastHit.collider.gameObject);
			if (raycastHit.collider.gameObject.CompareTag("Sand"))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(go, raycastHit.point, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				gameObject.transform.SetParent(this.treeParent);
				this.spawned.Add(gameObject);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x0004D73E File Offset: 0x0004B93E
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.73f, 0.57f, 0f);
		Gizmos.DrawWireSphere(this.treeParent.position, this.radius);
	}

	// Token: 0x04000DEE RID: 3566
	public GameObject[] palmTrees;

	// Token: 0x04000DEF RID: 3567
	public float radius;

	// Token: 0x04000DF0 RID: 3568
	public Vector2Int treeSpawnRange;

	// Token: 0x04000DF1 RID: 3569
	public List<GameObject> spawned;

	// Token: 0x04000DF2 RID: 3570
	public Transform treeParent;

	// Token: 0x04000DF3 RID: 3571
	public LayerMask layerMask;
}
