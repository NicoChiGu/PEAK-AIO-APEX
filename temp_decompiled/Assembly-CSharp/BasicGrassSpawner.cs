using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sirenix.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000BD RID: 189
[BurstCompile]
public class BasicGrassSpawner : MonoBehaviour
{
	// Token: 0x0600070E RID: 1806 RVA: 0x00027AAC File Offset: 0x00025CAC
	public void Generate()
	{
		this.<Generate>g__SpawnRoutine|3_0();
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00027ABC File Offset: 0x00025CBC
	[CompilerGenerated]
	private void <Generate>g__SpawnRoutine|3_0()
	{
		LinqExtensions.ForEach<GrassChunk>(base.GetComponentsInChildren<GrassChunk>(), delegate(GrassChunk chunk)
		{
			Object.DestroyImmediate(chunk.gameObject);
		});
		NativeList<GrassPoint> nativeList = new NativeList<GrassPoint>(100000, 4);
		int num = Mathf.RoundToInt(base.transform.localScale.x);
		int num2 = Mathf.RoundToInt(base.transform.localScale.y);
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int i = 0; i < num; i++)
		{
			float num3 = (float)i / (float)num;
			if (stopwatch.ElapsedMilliseconds > 10L)
			{
				stopwatch.Reset();
				stopwatch.Start();
			}
			for (int j = 0; j < num2; j++)
			{
				float num4 = (float)j / (float)num2;
				Vector3 vector = base.transform.TransformPoint(new Vector3(Mathf.Lerp(-0.5f, 0.5f, num3), Mathf.Lerp(-0.5f, 0.5f, num4), 0f));
				Optionable<RaycastHit> optionable = this.<Generate>g__Raycast|3_3(vector, base.transform.forward, 500f);
				float num5 = 0.2f;
				if (optionable.IsSome)
				{
					float num6 = BasicGrassSpawner.<Generate>g__GetPerlin|3_2(optionable.Value.point);
					RaycastHit value = optionable.Value;
					if (num6 > num5)
					{
						num6 = math.remap(num5, 1f, 0f, 1f, num6);
						GrassPoint grassPoint = default(GrassPoint);
						grassPoint.worldPos = value.point;
						grassPoint.normal = value.normal;
						grassPoint.scale = num6;
						nativeList.Add(ref grassPoint);
					}
					int num7 = Random.Range(10, 20);
					for (int k = 0; k < num7; k++)
					{
						Vector3 vector2 = Random.insideUnitSphere * 2f;
						optionable = this.<Generate>g__Raycast|3_3(value.point + value.normal * 4f + vector2, -value.normal, 500f);
						if (optionable.IsSome)
						{
							float num8 = BasicGrassSpawner.<Generate>g__GetPerlin|3_2(optionable.Value.point);
							if (num8 > num5)
							{
								num8 = math.remap(num5, 1f, 0f, 1f, num8);
								GrassPoint grassPoint = default(GrassPoint);
								grassPoint.worldPos = optionable.Value.point;
								grassPoint.normal = optionable.Value.normal;
								grassPoint.scale = num8;
								nativeList.Add(ref grassPoint);
							}
						}
					}
				}
			}
		}
		this.grassMaterial.EnableKeyword("PROCEDURAL_INSTANCING_ON");
		stopwatch.Reset();
		stopwatch.Start();
		Dictionary<int3, List<GrassPoint>> dictionary = new Dictionary<int3, List<GrassPoint>>();
		int num9 = 0;
		foreach (GrassPoint grassPoint2 in nativeList)
		{
			if (stopwatch.ElapsedMilliseconds > 10L)
			{
				float num10 = (float)num9 / (float)nativeList.Length;
				stopwatch.Reset();
				stopwatch.Start();
			}
			int3 chunkFromPosition = GrassChunking.GetChunkFromPosition(grassPoint2.worldPos);
			if (!dictionary.ContainsKey(chunkFromPosition))
			{
				dictionary.Add(chunkFromPosition, new List<GrassPoint>());
			}
			dictionary[chunkFromPosition].Add(grassPoint2);
			num9++;
		}
		stopwatch.Reset();
		stopwatch.Start();
		int count = dictionary.Count;
		int num11 = 0;
		foreach (KeyValuePair<int3, List<GrassPoint>> keyValuePair in dictionary)
		{
			int3 key = keyValuePair.Key;
			if (stopwatch.ElapsedMilliseconds > 10L)
			{
				float num12 = (float)num11 / (float)count;
				stopwatch.Reset();
				stopwatch.Start();
			}
			Debug.Log(string.Format("Chunk: {0}", key));
			GameObject gameObject = new GameObject(string.Format("Chunk {0}", key));
			gameObject.transform.position = key * GrassChunking.CHUNK_SIZE;
			gameObject.AddComponent<GrassChunk>().SetData(keyValuePair.Value);
			gameObject.transform.SetParent(base.transform);
			GrassRenderer grassRenderer = gameObject.AddComponent<GrassRenderer>();
			grassRenderer.grassComputeShader = this.grassComputeShader;
			grassRenderer.m_grassRenderMaterial = this.grassMaterial;
			grassRenderer.CurrentChunk = key;
			num11++;
		}
		nativeList.Dispose();
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00027F60 File Offset: 0x00026160
	[CompilerGenerated]
	internal static float <Generate>g__GetPerlin|3_2(Vector3 worldPos)
	{
		return Mathf.Clamp01(Mathf.PerlinNoise(worldPos.x * 0.1f + worldPos.y * 0.02f, worldPos.z * 0.1f));
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x00027F94 File Offset: 0x00026194
	[CompilerGenerated]
	private Optionable<RaycastHit> <Generate>g__Raycast|3_3(Vector3 pos, Vector3 direction, float distance)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(pos, direction), ref raycastHit, distance) && Vector3.Angle(raycastHit.normal, Vector3.up) < 45f && TransformExtensions.IsGrandChildOf(raycastHit.transform, this.placableParent) && (raycastHit.collider.gameObject.layer == 20 || raycastHit.collider.tag.Contains("Stone")))
		{
			return Optionable<RaycastHit>.Some(raycastHit);
		}
		return Optionable<RaycastHit>.None;
	}

	// Token: 0x040006F7 RID: 1783
	public Transform placableParent;

	// Token: 0x040006F8 RID: 1784
	public Material grassMaterial;

	// Token: 0x040006F9 RID: 1785
	public ComputeShader grassComputeShader;
}
