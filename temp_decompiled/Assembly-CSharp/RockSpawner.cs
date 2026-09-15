using System;
using UnityEngine;

// Token: 0x02000315 RID: 789
public class RockSpawner : MonoBehaviour
{
	// Token: 0x06001463 RID: 5219 RVA: 0x00067830 File Offset: 0x00065A30
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(base.transform.position - this.area.y * 0.5f * base.transform.forward, base.transform.position + this.area.y * 0.5f * base.transform.forward);
		Gizmos.DrawLine(base.transform.position - this.area.x * 0.5f * base.transform.right, base.transform.position + this.area.x * 0.5f * base.transform.right);
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x0006790C File Offset: 0x00065B0C
	public void Go()
	{
		this.Clear();
		for (int i = 0; i < this.nrOfSpawns; i++)
		{
			this.DoSpawn();
		}
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x00067938 File Offset: 0x00065B38
	private void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x00067974 File Offset: 0x00065B74
	private void DoSpawn()
	{
		RockSpawner.ReturnData? randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return;
		}
		GameObject gameObject = this.rocks[Random.Range(0, this.rocks.Length)];
		Quaternion quaternion = gameObject.transform.rotation;
		if (this.rotation == RockSpawner.OriginalRotation.RaycastNormal)
		{
			quaternion = HelperFunctions.GetRandomRotationWithUp(randomPoint.Value.normal);
		}
		quaternion = Quaternion.Lerp(quaternion, Random.rotation, Mathf.Pow(Random.value, this.rotationPow) * this.maxRotation);
		GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, randomPoint.Value.pos, quaternion, base.transform);
		gameObject2.transform.position += base.transform.up * -this.downMove;
		gameObject2.transform.Rotate(base.transform.eulerAngles, 0);
		gameObject2.transform.localScale *= Random.Range(this.minScale, this.maxScale);
		Physics.SyncTransforms();
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x00067A78 File Offset: 0x00065C78
	private RockSpawner.ReturnData? GetRandomPoint()
	{
		Vector3 vector = base.transform.position;
		vector += base.transform.right * Mathf.Lerp(-this.area.x * 0.5f, this.area.x * 0.5f, Random.value);
		vector += base.transform.forward * Mathf.Lerp(-this.area.y * 0.5f, this.area.y * 0.5f, Random.value);
		if (!this.raycast)
		{
			return new RockSpawner.ReturnData?(new RockSpawner.ReturnData
			{
				pos = vector,
				normal = Vector3.up
			});
		}
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + base.transform.up * -5000f, HelperFunctions.LayerType.Terrain, 0f, 1);
		if (raycastHit.transform)
		{
			return new RockSpawner.ReturnData?(new RockSpawner.ReturnData
			{
				pos = raycastHit.point,
				normal = raycastHit.normal
			});
		}
		return default(RockSpawner.ReturnData?);
	}

	// Token: 0x040012F3 RID: 4851
	public Vector2 area;

	// Token: 0x040012F4 RID: 4852
	public GameObject[] rocks;

	// Token: 0x040012F5 RID: 4853
	public int nrOfSpawns = 500;

	// Token: 0x040012F6 RID: 4854
	public float downMove;

	// Token: 0x040012F7 RID: 4855
	public RockSpawner.OriginalRotation rotation;

	// Token: 0x040012F8 RID: 4856
	public bool raycast = true;

	// Token: 0x040012F9 RID: 4857
	public float minScale = 1f;

	// Token: 0x040012FA RID: 4858
	public float maxScale = 2f;

	// Token: 0x040012FB RID: 4859
	public float maxRotation = 1f;

	// Token: 0x040012FC RID: 4860
	public float rotationPow;

	// Token: 0x02000505 RID: 1285
	public enum OriginalRotation
	{
		// Token: 0x04001B4B RID: 6987
		PrefabRotation,
		// Token: 0x04001B4C RID: 6988
		RaycastNormal
	}

	// Token: 0x02000506 RID: 1286
	private struct ReturnData
	{
		// Token: 0x04001B4D RID: 6989
		public Vector3 pos;

		// Token: 0x04001B4E RID: 6990
		public Vector3 normal;
	}
}
