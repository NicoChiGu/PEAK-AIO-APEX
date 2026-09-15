using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class MapGenerationStage : MonoBehaviour
{
	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000660 RID: 1632 RVA: 0x000244D1 File Offset: 0x000226D1
	private bool singleObject
	{
		get
		{
			return this.spawnMode == MapGenerationStage.SpawnMode.SingleObject;
		}
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x000244DC File Offset: 0x000226DC
	private void OnDrawGizmosSelected()
	{
		if (this.useMinimumHeightLimit)
		{
			Gizmos.color = new Color(1f, 0.21f, 0f, 0.49f);
			Gizmos.DrawCube(base.transform.position + new Vector3(0f, this.minimumHeightLimit, 0f), new Vector3(1000f, 0.01f, 1000f));
		}
		if (this.useMaximumHeightLimit)
		{
			Gizmos.color = new Color(0f, 1f, 0.96f, 0.49f);
			Gizmos.DrawCube(base.transform.position + new Vector3(0f, this.maximumHeightLimit, 0f), new Vector3(1000f, 0.01f, 1000f));
		}
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x000245B1 File Offset: 0x000227B1
	public void Generate(int seed = 0)
	{
		this.ClearSpawnedObjects();
		this.GenerateNodeMap();
		this.RunProximityPasses();
		this.SpawnObjectsFromNodeMap();
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x000245CC File Offset: 0x000227CC
	public void ClearSpawnedObjects()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		this.spawnedObjects.Clear();
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00024614 File Offset: 0x00022814
	private void GenerateNodeMap()
	{
		if (this.nodeSpacing == 0f)
		{
			Debug.LogError("NODE SPACING IS ZERO! THIS WOULD RESULT IN INFINITE SPAWNING!");
			return;
		}
		Vector2 vector;
		vector..ctor(this.spawnRange.bounds.min.x, this.spawnRange.bounds.min.z);
		Vector2 vector2;
		vector2..ctor(this.spawnRange.bounds.max.x, this.spawnRange.bounds.max.z);
		Vector2 vector3;
		vector3..ctor(vector.x, vector.y);
		this.nodeMap.Clear();
		while (vector3.y <= vector2.y)
		{
			List<MapGenerationStage.GenerationNode> list = new List<MapGenerationStage.GenerationNode>();
			this.nodeMap.Add(list);
			while (vector3.x <= vector2.x)
			{
				Vector2 vector4;
				vector4..ctor(vector3.x, vector3.y);
				if (this.randomizedNodeOffset > 0f)
				{
					vector4 += new Vector2(Random.Range(-this.randomizedNodeOffset, this.randomizedNodeOffset), Random.Range(-this.randomizedNodeOffset, this.randomizedNodeOffset));
				}
				list.Add(new MapGenerationStage.GenerationNode(new Vector2(vector4.x, vector4.y), this.defaultDensity));
				vector3.x += this.nodeSpacing;
			}
			vector3.x = vector.x;
			vector3.y += this.nodeSpacing;
		}
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x000247AB File Offset: 0x000229AB
	private void SpawnObjectsFromNodeMap()
	{
		this.RunActionOnAllNodes(new Action<MapGenerationStage.GenerationNode>(this.TrySpawnObject));
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x000247C0 File Offset: 0x000229C0
	private void SpawnObject(Vector3 spot, Vector3 normal)
	{
		GameObject gameObject;
		if (this.singleObject)
		{
			if (this.objectPrefab)
			{
				gameObject = Object.Instantiate<GameObject>(this.objectPrefab);
			}
			else
			{
				gameObject = new GameObject();
			}
		}
		else if (!this.singleObject && this.spawnList)
		{
			gameObject = Object.Instantiate<GameObject>(this.spawnList.GetSingleSpawn());
		}
		else
		{
			gameObject = new GameObject();
		}
		if (this.randomizeRotation)
		{
			if (this.randomizeRotationOnNormalPlane)
			{
				gameObject.transform.rotation = HelperFunctions.GetRandomRotationWithUp(normal);
			}
			else
			{
				gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, (float)Random.Range(0, 360), gameObject.transform.eulerAngles.z);
			}
		}
		if (this.heightVariation != Vector2.zero)
		{
			spot += Vector3.up * Random.Range(this.heightVariation.x, this.heightVariation.y);
		}
		if (this.scaleVariation != Vector2.zero)
		{
			float num = Random.Range(this.scaleVariation.x, this.scaleVariation.y);
			gameObject.transform.localScale += new Vector3(num, num, num);
		}
		gameObject.transform.position = spot;
		gameObject.transform.SetParent(base.transform, true);
		LazyGizmo lazyGizmo = gameObject.AddComponent<LazyGizmo>();
		lazyGizmo.onSelected = false;
		lazyGizmo.color = this.testGizmoColor;
		lazyGizmo.radius = this.testGizmoSize;
		this.spawnedObjects.Add(gameObject);
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00024960 File Offset: 0x00022B60
	private void TrySpawnObject(MapGenerationStage.GenerationNode node)
	{
		Vector3 point;
		point..ctor(node.position.x, base.transform.position.y, node.position.y);
		Vector3 vector = Vector3.up;
		if ((this.raycastDownward || this.allowedTags.Count > 0) && Physics.Raycast(point + Vector3.up * 50f, Vector3.down, ref this.hit, 100f))
		{
			if (this.useMinimumHeightLimit && this.hit.point.y < base.transform.position.y + this.minimumHeightLimit)
			{
				node.valid = false;
				return;
			}
			if (this.useMaximumHeightLimit && this.hit.point.y > base.transform.position.y + this.maximumHeightLimit)
			{
				node.valid = false;
				return;
			}
			if (this.allowedTags.Count > 0 && !this.allowedTags.Contains(this.hit.collider.gameObject.tag))
			{
				node.valid = false;
				return;
			}
			if (this.raycastDownward)
			{
				point = this.hit.point;
				vector = this.hit.normal;
				Debug.DrawLine(point, point + vector * 10f, Color.red, 10f);
			}
		}
		if (!node.valid)
		{
			return;
		}
		if (Random.Range(0f, 1f) < node.probability)
		{
			this.SpawnObject(point, vector);
		}
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x00024AFB File Offset: 0x00022CFB
	private void RunProximityPasses()
	{
		this.RunActionOnAllNodes(new Action<MapGenerationStage.GenerationNode>(this.RunProximityPassesOnNode));
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00024B10 File Offset: 0x00022D10
	private void RunProximityPassesOnNode(MapGenerationStage.GenerationNode node)
	{
		this.RunPositionGradientPass(node);
		for (int i = 0; i < this.proximityPassData.Count; i++)
		{
			MapGenerationStage.GenerationProximityPassData generationProximityPassData = this.proximityPassData[i];
			List<GameObject> list = generationProximityPassData.previousStage.spawnedObjects;
			for (int j = 0; j < list.Count; j++)
			{
				float num = Vector3.Distance(node.position, Util.FlattenVector3(list[j].transform.position));
				if (num < generationProximityPassData.hardAvoidanceRadius * list[j].transform.localScale.x)
				{
					node.valid = false;
				}
				else if (num <= generationProximityPassData.minMaxProximity.y)
				{
					float num2 = Util.RangeLerp(generationProximityPassData.correlation, 0f, generationProximityPassData.minMaxProximity.x, generationProximityPassData.minMaxProximity.y, num, true, null);
					node.probability = Mathf.Clamp(node.probability + num2, this.minMaxDensity.x, this.minMaxDensity.y);
				}
			}
		}
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x00024C2C File Offset: 0x00022E2C
	private void RunPositionGradientPass(MapGenerationStage.GenerationNode node)
	{
		float num = (node.position.x - this.spawnRange.bounds.min.x) / (this.spawnRange.bounds.max.x - this.spawnRange.bounds.min.x);
		float num2 = (node.position.y - this.spawnRange.bounds.min.z) / (this.spawnRange.bounds.max.z - this.spawnRange.bounds.min.z);
		float num3 = 0f;
		float num4 = 0f;
		if (this.useCurveX)
		{
			num3 = this.curveX.Evaluate(num);
		}
		if (this.useCurveZ)
		{
			num4 = this.curveZ.Evaluate(num2);
		}
		node.probability = Mathf.Clamp(node.probability + num3 + num4, this.minMaxDensity.x, this.minMaxDensity.y);
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00024D50 File Offset: 0x00022F50
	private void RunActionOnAllNodes(Action<MapGenerationStage.GenerationNode> Action)
	{
		for (int i = 0; i < this.nodeMap.Count; i++)
		{
			List<MapGenerationStage.GenerationNode> list = this.nodeMap[i];
			for (int j = 0; j < list.Count; j++)
			{
				MapGenerationStage.GenerationNode generationNode = list[j];
				Action.Invoke(generationNode);
			}
		}
	}

	// Token: 0x04000662 RID: 1634
	public BoxCollider spawnRange;

	// Token: 0x04000663 RID: 1635
	public float nodeSpacing = 1f;

	// Token: 0x04000664 RID: 1636
	[Range(0f, 1f)]
	public float defaultDensity;

	// Token: 0x04000665 RID: 1637
	public Vector2 minMaxDensity = new Vector2(0f, 1f);

	// Token: 0x04000666 RID: 1638
	public float randomizedNodeOffset;

	// Token: 0x04000667 RID: 1639
	public bool useCurveX;

	// Token: 0x04000668 RID: 1640
	public AnimationCurve curveX;

	// Token: 0x04000669 RID: 1641
	public bool useCurveZ;

	// Token: 0x0400066A RID: 1642
	public AnimationCurve curveZ;

	// Token: 0x0400066B RID: 1643
	public List<MapGenerationStage.GenerationProximityPassData> proximityPassData;

	// Token: 0x0400066C RID: 1644
	public bool useMinimumHeightLimit;

	// Token: 0x0400066D RID: 1645
	public float minimumHeightLimit;

	// Token: 0x0400066E RID: 1646
	public bool useMaximumHeightLimit;

	// Token: 0x0400066F RID: 1647
	public float maximumHeightLimit;

	// Token: 0x04000670 RID: 1648
	public MapGenerationStage.SpawnMode spawnMode;

	// Token: 0x04000671 RID: 1649
	public GameObject objectPrefab;

	// Token: 0x04000672 RID: 1650
	public SpawnList spawnList;

	// Token: 0x04000673 RID: 1651
	public bool randomizeRotation = true;

	// Token: 0x04000674 RID: 1652
	public bool randomizeRotationOnNormalPlane = true;

	// Token: 0x04000675 RID: 1653
	public bool raycastDownward = true;

	// Token: 0x04000676 RID: 1654
	public List<string> allowedTags;

	// Token: 0x04000677 RID: 1655
	public Vector2 heightVariation;

	// Token: 0x04000678 RID: 1656
	public Vector2 scaleVariation;

	// Token: 0x04000679 RID: 1657
	public Color testGizmoColor = Color.red;

	// Token: 0x0400067A RID: 1658
	public float testGizmoSize = 0.5f;

	// Token: 0x0400067B RID: 1659
	public List<List<MapGenerationStage.GenerationNode>> nodeMap = new List<List<MapGenerationStage.GenerationNode>>();

	// Token: 0x0400067C RID: 1660
	public List<GameObject> spawnedObjects;

	// Token: 0x0400067D RID: 1661
	private RaycastHit hit;

	// Token: 0x0200042D RID: 1069
	public enum SpawnMode
	{
		// Token: 0x040017FC RID: 6140
		SingleObject,
		// Token: 0x040017FD RID: 6141
		SpawnList
	}

	// Token: 0x0200042E RID: 1070
	public class GenerationNode
	{
		// Token: 0x06001A78 RID: 6776 RVA: 0x00080370 File Offset: 0x0007E570
		public GenerationNode(Vector2 pos, float defaultProbability)
		{
			this.position = pos;
			this.probability = defaultProbability;
			this.valid = true;
		}

		// Token: 0x040017FE RID: 6142
		public Vector2 position;

		// Token: 0x040017FF RID: 6143
		public float probability;

		// Token: 0x04001800 RID: 6144
		public bool valid;
	}

	// Token: 0x0200042F RID: 1071
	[Serializable]
	public class GenerationProximityPassData
	{
		// Token: 0x04001801 RID: 6145
		public MapGenerationStage previousStage;

		// Token: 0x04001802 RID: 6146
		public float hardAvoidanceRadius;

		// Token: 0x04001803 RID: 6147
		public Vector2 minMaxProximity;

		// Token: 0x04001804 RID: 6148
		public float correlation;
	}
}
