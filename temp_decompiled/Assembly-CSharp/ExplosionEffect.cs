using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000255 RID: 597
public class ExplosionEffect : MonoBehaviour
{
	// Token: 0x06001127 RID: 4391 RVA: 0x00056488 File Offset: 0x00054688
	private void Start()
	{
		this.GetPoints();
		foreach (ExplosionOrb explosionOrb in this.explosionPoints)
		{
			base.StartCoroutine(this.<Start>g__IExplode|12_0(explosionOrb));
		}
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x000564C8 File Offset: 0x000546C8
	private void GetPoints()
	{
		int num = 1 + this.explosionPointCount + this.subExplosionPointCount * this.explosionPointCount;
		this.explosionPoints = new ExplosionOrb[num];
		this.explosionPoints[0] = new ExplosionOrb(base.transform.position, Vector3.up, 0f, 1f, 1f);
		int num2 = 0;
		float num3 = this.childSizeFactor * this.childSizeFactor;
		for (int i = 0; i < this.explosionPointCount; i++)
		{
			num2++;
			Vector3 vector = Random.onUnitSphere * this.explosionRadius * this.spawnRadiusFactor;
			vector.y = Mathf.Abs(vector.y);
			this.explosionPoints[num2] = new ExplosionOrb(base.transform.position + vector, vector, Random.Range(this.minDelay, this.maxDelay), this.childSizeFactor, Random.Range(this.minSpeed, this.maxSpeed));
			int num4 = num2;
			for (int j = 0; j < this.subExplosionPointCount; j++)
			{
				num2++;
				Vector3 position = this.explosionPoints[num4].position;
				vector = Random.onUnitSphere * this.explosionRadius * this.explosionPoints[num4].size * this.spawnRadiusFactor;
				vector.y = Mathf.Abs(vector.y);
				this.explosionPoints[num2] = new ExplosionOrb(position + vector, vector, this.explosionPoints[num4].delay + Random.Range(this.minDelay, this.maxDelay), num3, this.explosionPoints[num4].speed + Random.Range(this.minSpeed, this.maxSpeed));
			}
		}
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x000566B0 File Offset: 0x000548B0
	public void OnDrawGizmosSelected()
	{
		foreach (ExplosionOrb explosionOrb in this.explosionPoints)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(explosionOrb.position, this.explosionRadius * explosionOrb.size);
		}
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x00056791 File Offset: 0x00054991
	[CompilerGenerated]
	private IEnumerator <Start>g__IExplode|12_0(ExplosionOrb item)
	{
		yield return new WaitForSeconds(item.delay / (this.speed * item.speed));
		GameObject gameObject = Object.Instantiate<GameObject>(this.explosionOrb, item.position, HelperFunctions.GetRandomRotationWithUp(item.direction));
		gameObject.GetComponentInChildren<Animator>().speed = this.speed * item.speed;
		gameObject.transform.localScale = Vector3.one * (item.size * this.baseScale);
		MeshRenderer componentInChildren = gameObject.GetComponentInChildren<MeshRenderer>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		componentInChildren.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(ExplosionEffect.k_Random, Random.value);
		componentInChildren.SetPropertyBlock(materialPropertyBlock);
		yield break;
	}

	// Token: 0x04000F94 RID: 3988
	private static readonly int k_Random = Shader.PropertyToID("_Random");

	// Token: 0x04000F95 RID: 3989
	public float speed = 1f;

	// Token: 0x04000F96 RID: 3990
	public GameObject explosionOrb;

	// Token: 0x04000F97 RID: 3991
	public float baseScale = 1f;

	// Token: 0x04000F98 RID: 3992
	public float explosionRadius = 5f;

	// Token: 0x04000F99 RID: 3993
	public float spawnRadiusFactor = 0.8f;

	// Token: 0x04000F9A RID: 3994
	public float childSizeFactor = 0.9f;

	// Token: 0x04000F9B RID: 3995
	public float minDelay = 0.4f;

	// Token: 0x04000F9C RID: 3996
	public float maxDelay = 0.5f;

	// Token: 0x04000F9D RID: 3997
	public float minSpeed = 0.75f;

	// Token: 0x04000F9E RID: 3998
	public float maxSpeed = 1.25f;

	// Token: 0x04000F9F RID: 3999
	private ExplosionOrb[] explosionPoints;

	// Token: 0x04000FA0 RID: 4000
	public int explosionPointCount = 4;

	// Token: 0x04000FA1 RID: 4001
	public int subExplosionPointCount = 2;
}
