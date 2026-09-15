using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000133 RID: 307
[ExecuteInEditMode]
public class LightVolume : MonoBehaviour
{
	// Token: 0x06000998 RID: 2456 RVA: 0x00032F9C File Offset: 0x0003119C
	private void SetShaderVars()
	{
		Shader.SetGlobalFloat("brightness", this.brightness);
		Shader.SetGlobalFloat("ambienceStrength", this.ambienceStrength);
		Shader.SetGlobalFloat("ambienceMin", this.ambienceMin);
		Shader.SetGlobalVector("gridRes", this.gridRes);
		Shader.SetGlobalFloat("raySpacing", this.raySpacing);
		Shader.SetGlobalVector("gridOffset", this.gridOffset);
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x00033018 File Offset: 0x00031218
	public void SetSize()
	{
		Shader.SetGlobalTexture("_LightMap", null);
		Bounds totalBounds = LightVolume.GetTotalBounds((this.sceneParent == null) ? base.gameObject : this.sceneParent);
		this.gridOffset = totalBounds.center;
		this.gridRes = new Vector3Int(Mathf.CeilToInt((totalBounds.size.x + 3f) / this.raySpacing), Mathf.CeilToInt((totalBounds.size.y + 3f) / this.raySpacing), Mathf.CeilToInt((totalBounds.size.z + 3f) / this.raySpacing));
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x000330C4 File Offset: 0x000312C4
	private static Bounds GetTotalBounds(GameObject gameObject)
	{
		Bounds bounds = default(Bounds);
		bool flag = true;
		foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
		{
			if (flag)
			{
				bounds = meshRenderer.bounds;
			}
			else
			{
				bounds.Encapsulate(meshRenderer.bounds);
			}
			flag = false;
		}
		return bounds;
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x00033114 File Offset: 0x00031314
	private void OnDrawGizmosSelected()
	{
		if (!this.showVolumeGizmos)
		{
			return;
		}
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(this.gridOffset - Vector3.one * 0.25f, this.gridRes * this.raySpacing);
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(this.gridOffset + Vector3.one * 0.25f, this.gridRes * this.raySpacing);
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x000331A8 File Offset: 0x000313A8
	private void Awake()
	{
		LightVolume.instance = this;
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x000331B0 File Offset: 0x000313B0
	private void Start()
	{
		this.SetShaderVars();
		Shader.SetGlobalTexture("_LightMap", this.lightMap);
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x0600099E RID: 2462 RVA: 0x000331C8 File Offset: 0x000313C8
	private bool RaytracingShaderNotSupported
	{
		get
		{
			return !SystemInfo.supportsRayTracingShaders;
		}
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x000331D4 File Offset: 0x000313D4
	public void Bake(Action onComplete = null)
	{
		if (!this.computeShader)
		{
			Debug.LogError("Cannot bake at runtime (serialize the ComputeShader if you want to do this)");
			return;
		}
		if (!this.rayTracingShader)
		{
			Debug.LogError("Cannot bake at runtime (serialize the RayTracingShader if you want to do this)");
			return;
		}
		this.SetSize();
		RenderTexture renderTexture = this.RunBake();
		RenderTexture renderTexture2 = this.RunBlur(renderTexture);
		renderTexture2.name = "LightVolumeRenderTexture";
		this.SetShaderVars();
		Shader.SetGlobalTexture("_LightMap", renderTexture2);
		this.SaveTex(renderTexture2, onComplete);
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x0003324C File Offset: 0x0003144C
	private RenderTexture RunBake()
	{
		this.rayTracingShader.SetVector("gridRadius", new Vector3((float)this.gridRes.x, (float)this.gridRes.y, (float)this.gridRes.z) * (this.raySpacing / 2f));
		this.rayTracingShader.SetVector("gridOffset", this.gridOffset);
		this.rayTracingShader.SetVector("skyColor", this.skyColor);
		this.rayTracingShader.SetInt("rayCount", this.rayCount);
		ComputeBuffer computeBuffer;
		int num = this.BuildLights(out computeBuffer);
		IDisposable disposable;
		this.BuildMeshes(out disposable, num);
		RenderTexture renderTexture = LightVolume.Create3DTexture(1, 2, this.gridRes);
		this.rayTracingShader.SetTexture("lightMap", renderTexture);
		for (int i = 0; i < num + 1; i++)
		{
			this.rayTracingShader.SetInt("doLightIndex", i);
			this.rayTracingShader.Dispatch("RaygenShader", this.gridRes.x, this.gridRes.y, this.gridRes.z, null);
		}
		computeBuffer.Dispose();
		disposable.Dispose();
		return renderTexture;
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00033388 File Offset: 0x00031588
	private static RenderTexture Create3DTexture(FilterMode filterMode, RenderTextureFormat format, Vector3Int resolution)
	{
		RenderTexture renderTexture = new RenderTexture(resolution.x, resolution.y, 0);
		renderTexture.enableRandomWrite = true;
		renderTexture.format = format;
		renderTexture.dimension = 3;
		renderTexture.volumeDepth = resolution.z;
		renderTexture.wrapMode = 1;
		renderTexture.filterMode = filterMode;
		renderTexture.hideFlags = 52;
		if (!renderTexture.Create())
		{
			throw new Exception("Failed to create texture");
		}
		return renderTexture;
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x000333F4 File Offset: 0x000315F4
	private int BuildLights(out ComputeBuffer toDispose)
	{
		List<LightVolume.GpuLight> list = new List<LightVolume.GpuLight>();
		GameObject gameObject = ((this.sceneParent == null) ? base.gameObject : this.sceneParent);
		if (this.allLightsFound == null)
		{
			this.allLightsFound = new List<BakedVolumeLight>();
		}
		this.allLightsFound.Clear();
		foreach (BakedVolumeLight bakedVolumeLight in gameObject.GetComponentsInChildren<BakedVolumeLight>())
		{
			this.allLightsFound.Add(bakedVolumeLight);
			Vector3 vector;
			vector..ctor(bakedVolumeLight.color.r, bakedVolumeLight.color.g, bakedVolumeLight.color.b);
			BakedVolumeLight.LightModes mode = bakedVolumeLight.mode;
			float num;
			if (mode != BakedVolumeLight.LightModes.Point)
			{
				if (mode != BakedVolumeLight.LightModes.Spot)
				{
					throw new Exception();
				}
				num = bakedVolumeLight.coneSize * 0.017453292f;
			}
			else
			{
				num = 0f;
			}
			float num2 = num;
			list.Add(new LightVolume.GpuLight
			{
				Position = bakedVolumeLight.transform.position,
				ConeSize = num2,
				Direction = bakedVolumeLight.transform.forward,
				Radius = bakedVolumeLight.GetRadius(),
				Color = vector * bakedVolumeLight.intensity,
				Falloff = bakedVolumeLight.falloff,
				ConeFalloff = bakedVolumeLight.coneFalloff
			});
		}
		int count = list.Count;
		if (count == 0)
		{
			list.Add(default(LightVolume.GpuLight));
		}
		ComputeBuffer computeBuffer = new ComputeBuffer(list.Count, 52);
		computeBuffer.SetData<LightVolume.GpuLight>(list);
		this.rayTracingShader.SetBuffer("lightBuffer", computeBuffer);
		this.rayTracingShader.SetInt("lightBufferLength", count);
		toDispose = computeBuffer;
		return count;
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x000335A8 File Offset: 0x000317A8
	private void BuildMeshes(out IDisposable toDispose, int lightCountForDebug)
	{
		int value = this.occluderMask.value;
		GameObject gameObject = ((this.sceneParent == null) ? base.gameObject : this.sceneParent);
		if (this.allMeshRenderersFound == null)
		{
			this.allMeshRenderersFound = new List<MeshRenderer>();
		}
		this.allMeshRenderersFound.Clear();
		RayTracingAccelerationStructure rayTracingAccelerationStructure = new RayTracingAccelerationStructure();
		uint num = 0U;
		int num2 = 0;
		foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
		{
			if (((1 << meshRenderer.gameObject.layer) & value) != 0 || meshRenderer.GetComponent<LightingCollider>())
			{
				this.allMeshRenderersFound.Add(meshRenderer);
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component == null)
				{
					Debug.LogError("Mesh renderer without filter: " + meshRenderer.gameObject.name, meshRenderer.gameObject);
				}
				Mesh sharedMesh = component.sharedMesh;
				if (!(sharedMesh == null))
				{
					int subMeshCount = sharedMesh.subMeshCount;
					for (int j = 0; j < subMeshCount; j++)
					{
						num += sharedMesh.GetIndexCount(j);
					}
					num2 += sharedMesh.vertexCount;
					RayTracingSubMeshFlags[] array = new RayTracingSubMeshFlags[subMeshCount];
					for (int k = 0; k < array.Length; k++)
					{
						array[k] = 3;
					}
					rayTracingAccelerationStructure.AddInstance(meshRenderer, array, true, false, 255U, uint.MaxValue);
				}
			}
		}
		rayTracingAccelerationStructure.Build();
		Debug.Log(string.Format("Light Volume Baker found: {0} lights, {1} meshes, {2} indices, {3} vertices", new object[]
		{
			lightCountForDebug,
			this.allMeshRenderersFound.Count,
			num,
			num2
		}));
		this.rayTracingShader.SetAccelerationStructure("g_SceneAccelStruct", rayTracingAccelerationStructure);
		toDispose = rayTracingAccelerationStructure;
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00033764 File Offset: 0x00031964
	private RenderTexture RunBlur(RenderTexture inputTex)
	{
		if (this.blurRadius <= 0)
		{
			return inputTex;
		}
		this.computeShader.SetInt("blurRadius", this.blurRadius);
		Vector3Int vector3Int;
		vector3Int..ctor(inputTex.width, inputTex.height, inputTex.volumeDepth);
		RenderTexture renderTexture = LightVolume.Create3DTexture(inputTex.filterMode, inputTex.format, vector3Int);
		for (int i = 0; i < 3; i++)
		{
			this.computeShader.SetTexture(1, "blurInputLightMap", inputTex);
			this.computeShader.SetTexture(1, "lightMap", renderTexture);
			this.computeShader.SetInt("blurAxis", i);
			uint num = 4U;
			uint num2 = 4U;
			uint num3 = 4U;
			long num4 = ((long)vector3Int.x + (long)((ulong)num) - 1L) / (long)((ulong)num);
			long num5 = ((long)vector3Int.y + (long)((ulong)num2) - 1L) / (long)((ulong)num2);
			long num6 = ((long)vector3Int.z + (long)((ulong)num3) - 1L) / (long)((ulong)num3);
			this.computeShader.Dispatch(1, (int)num4, (int)num5, (int)num6);
			RenderTexture renderTexture2 = inputTex;
			RenderTexture renderTexture3 = renderTexture;
			renderTexture = renderTexture2;
			inputTex = renderTexture3;
		}
		Object.DestroyImmediate(renderTexture);
		return inputTex;
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00033870 File Offset: 0x00031A70
	private void SaveTex(RenderTexture renderTexture, Action onComplete = null)
	{
		AsyncGPUReadbackRequest asyncGPUReadbackRequest = AsyncGPUReadback.Request(renderTexture, 0, null);
		asyncGPUReadbackRequest.WaitForCompletion();
		byte[] array = new byte[asyncGPUReadbackRequest.layerDataSize * asyncGPUReadbackRequest.layerCount];
		for (int i = 0; i < asyncGPUReadbackRequest.layerCount; i++)
		{
			NativeArray<byte>.Copy(asyncGPUReadbackRequest.GetData<byte>(i), 0, array, i * asyncGPUReadbackRequest.layerDataSize, asyncGPUReadbackRequest.layerDataSize);
		}
		if (!this.lightMap || this.lightMap.width != renderTexture.width || this.lightMap.height != renderTexture.height || this.lightMap.depth != renderTexture.volumeDepth || this.lightMap.graphicsFormat != renderTexture.graphicsFormat)
		{
			if (this.lightMap)
			{
				Object.DestroyImmediate(this.lightMap);
			}
			this.lightMap = new Texture3D(renderTexture.width, renderTexture.height, renderTexture.volumeDepth, renderTexture.graphicsFormat, 0);
		}
		this.lightMap.name = "LightVolumeBakeTexture";
		this.lightMap.wrapMode = renderTexture.wrapMode;
		this.lightMap.filterMode = renderTexture.filterMode;
		this.lightMap.SetPixelData<byte>(array, 0, 0);
		this.lightMap.Apply();
		Shader.SetGlobalTexture("_LightMap", this.lightMap);
		if (onComplete != null)
		{
			onComplete.Invoke();
		}
		Object.DestroyImmediate(renderTexture);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x000339D2 File Offset: 0x00031BD2
	public static LightVolume Instance()
	{
		if (LightVolume.instance == null)
		{
			LightVolume.instance = Object.FindAnyObjectByType<LightVolume>();
		}
		return LightVolume.instance;
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x000339F0 File Offset: 0x00031BF0
	internal Color SamplePosition(Vector3 worldPos)
	{
		worldPos -= this.gridOffset;
		worldPos += this.raySpacing * this.gridRes * 0.5f;
		worldPos.x /= this.raySpacing;
		worldPos.y /= this.raySpacing;
		worldPos.z /= this.raySpacing;
		return this.lightMap.GetPixel((int)worldPos.x, (int)worldPos.y, (int)worldPos.z);
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x00033A88 File Offset: 0x00031C88
	public float SamplePositionAlpha(Vector3 worldPos)
	{
		worldPos -= this.gridOffset;
		worldPos += this.raySpacing * this.gridRes * 0.5f;
		worldPos.x /= this.raySpacing;
		worldPos.y /= this.raySpacing;
		worldPos.z /= this.raySpacing;
		return this.lightMap.GetPixel((int)worldPos.x, (int)worldPos.y, (int)worldPos.z).a;
	}

	// Token: 0x04000913 RID: 2323
	public bool showVolumeGizmos = true;

	// Token: 0x04000914 RID: 2324
	public float brightness = 1f;

	// Token: 0x04000915 RID: 2325
	public float ambienceStrength = 1f;

	// Token: 0x04000916 RID: 2326
	public float ambienceMin = 0.05f;

	// Token: 0x04000917 RID: 2327
	public Color skyColor = Color.white;

	// Token: 0x04000918 RID: 2328
	public Vector3Int gridRes;

	// Token: 0x04000919 RID: 2329
	public Vector3 gridOffset;

	// Token: 0x0400091A RID: 2330
	public int rayCount = 128;

	// Token: 0x0400091B RID: 2331
	public float raySpacing = 1.5f;

	// Token: 0x0400091C RID: 2332
	[Tooltip("Colliders matching this mask will be used for light tracing, colliders not matching will be ignored")]
	public LayerMask occluderMask = -1;

	// Token: 0x0400091D RID: 2333
	[Tooltip("Radius (in texels) for how much to box blur the output texture")]
	public int blurRadius;

	// Token: 0x0400091E RID: 2334
	public GameObject sceneParent;

	// Token: 0x0400091F RID: 2335
	public ComputeShader computeShader;

	// Token: 0x04000920 RID: 2336
	public RayTracingShader rayTracingShader;

	// Token: 0x04000921 RID: 2337
	public Texture3D lightMap;

	// Token: 0x04000922 RID: 2338
	public List<BakedVolumeLight> allLightsFound;

	// Token: 0x04000923 RID: 2339
	public List<MeshRenderer> allMeshRenderersFound;

	// Token: 0x04000924 RID: 2340
	internal static LightVolume instance;

	// Token: 0x02000458 RID: 1112
	private struct GpuLight
	{
		// Token: 0x040018AC RID: 6316
		public Vector3 Position;

		// Token: 0x040018AD RID: 6317
		public float ConeSize;

		// Token: 0x040018AE RID: 6318
		public Vector3 Direction;

		// Token: 0x040018AF RID: 6319
		public float Radius;

		// Token: 0x040018B0 RID: 6320
		public Vector3 Color;

		// Token: 0x040018B1 RID: 6321
		public float Falloff;

		// Token: 0x040018B2 RID: 6322
		public float ConeFalloff;
	}
}
