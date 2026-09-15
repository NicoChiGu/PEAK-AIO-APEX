using System;
using System.Collections.Generic;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class RockSpawnerGD : MonoBehaviour
{
	// Token: 0x06000B6B RID: 2923 RVA: 0x0003CAAC File Offset: 0x0003ACAC
	public void createDeck()
	{
		this.deck.Clear();
		for (int i = 0; i < this.objectsToSpawn.Length; i++)
		{
			for (int j = 0; j < this.objectsToSpawn[i].maxCount; j++)
			{
				this.deck.Add(this.objectsToSpawn[i]);
			}
		}
		this.shuffleDeck();
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0003CB08 File Offset: 0x0003AD08
	public void shuffleDeck()
	{
		for (int i = 0; i < this.deck.Count; i++)
		{
			SpawnObject spawnObject = this.deck[i];
			int num = Random.Range(i, this.objectsToSpawn.Length);
			this.deck[i] = this.deck[num];
			this.deck[num] = spawnObject;
		}
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x0003CB6C File Offset: 0x0003AD6C
	public SpawnObject DrawFromDeck()
	{
		SpawnObject spawnObject = this.deck[0];
		this.deck.RemoveAt(0);
		return spawnObject;
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x0003CB88 File Offset: 0x0003AD88
	public void spawnObjects()
	{
		this.clearList();
		this.createDeck();
		int count = this.deck.Count;
		int num = count / this.layerCount;
		if (this.layerCount > count)
		{
			num = count;
		}
		for (int i = 0; i < count; i++)
		{
			float num2 = (float)i * this.yBias + 1f;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position - base.transform.up + (base.transform.right * Random.Range(-1f, 1f) * this.shape.size.x / 2f + base.transform.forward * (Mathf.Pow(Random.Range(-1f, 1f), num2) * this.shape.size.z / 2f)), -base.transform.up, ref raycastHit))
			{
				SpawnObject spawnObject = this.DrawFromDeck();
				GameObject gameObject = Object.Instantiate<GameObject>(spawnObject.prefab);
				gameObject.transform.position = raycastHit.point + new Vector3(Random.Range(-spawnObject.posJitter.x, spawnObject.posJitter.x), Random.Range(-spawnObject.posJitter.y, spawnObject.posJitter.y), Random.Range(-spawnObject.posJitter.z, spawnObject.posJitter.z));
				gameObject.transform.eulerAngles += new Vector3(Random.Range(-spawnObject.randomRot.x, spawnObject.randomRot.x), Random.Range(-spawnObject.randomRot.y, spawnObject.randomRot.y), Random.Range(-spawnObject.randomRot.z, spawnObject.randomRot.z));
				gameObject.transform.localScale += new Vector3(Random.Range(-spawnObject.randomScale.x, spawnObject.randomScale.x), Random.Range(-spawnObject.randomScale.y, spawnObject.randomScale.y), Random.Range(-spawnObject.randomScale.z, spawnObject.randomScale.z));
				gameObject.transform.localScale += Vector3.one * Random.Range(-spawnObject.uniformScale, spawnObject.uniformScale);
				gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, Vector3.one - new Vector3((float)ExtDataTypes.PCeilToInt(Random.Range(0f, spawnObject.inversion.x)), (float)ExtDataTypes.PCeilToInt(Random.Range(0f, spawnObject.inversion.y)), (float)ExtDataTypes.PCeilToInt(Random.Range(0f, spawnObject.inversion.z))).normalized * 2f);
				gameObject.transform.localScale *= spawnObject.scaleMultiplier;
				this.spawnedObjects.Add(gameObject);
				gameObject.transform.parent = base.transform;
				if (i % num == 0)
				{
					Physics.SyncTransforms();
				}
			}
		}
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0003CF28 File Offset: 0x0003B128
	public void clearList()
	{
		for (int i = 0; i < this.spawnedObjects.Count; i++)
		{
			Object.DestroyImmediate(this.spawnedObjects[i]);
		}
		this.spawnedObjects.Clear();
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x0003CF67 File Offset: 0x0003B167
	public void OnValidate()
	{
		this.shape.size = new Vector3(this.colliderScale.x, 0f, this.colliderScale.y);
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0003CF94 File Offset: 0x0003B194
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x04000A92 RID: 2706
	public Vector2 colliderScale;

	// Token: 0x04000A93 RID: 2707
	public SpawnObject[] objectsToSpawn;

	// Token: 0x04000A94 RID: 2708
	public List<GameObject> spawnedObjects;

	// Token: 0x04000A95 RID: 2709
	public List<SpawnObject> deck;

	// Token: 0x04000A96 RID: 2710
	public Vector2 castShape;

	// Token: 0x04000A97 RID: 2711
	public BoxCollider shape;

	// Token: 0x04000A98 RID: 2712
	public float yBias;

	// Token: 0x04000A99 RID: 2713
	[Range(1f, 99f)]
	public int layerCount;
}
