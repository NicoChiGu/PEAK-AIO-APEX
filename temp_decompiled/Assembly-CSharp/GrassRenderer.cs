using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Compute;

// Token: 0x020000C2 RID: 194
[ExecuteInEditMode]
public class GrassRenderer : MonoBehaviour
{
	// Token: 0x0600071E RID: 1822 RVA: 0x0002816C File Offset: 0x0002636C
	private void OnEnable()
	{
		ComputeBuffer geometryBuffer = this.GeometryBuffer;
		if (geometryBuffer != null)
		{
			geometryBuffer.Dispose();
		}
		ComputeBuffer argumentsBuffer = this.ArgumentsBuffer;
		if (argumentsBuffer != null)
		{
			argumentsBuffer.Dispose();
		}
		ComputeBuffer grassPointsBuffer = this.GrassPointsBuffer;
		if (grassPointsBuffer != null)
		{
			grassPointsBuffer.Dispose();
		}
		this.DataProvider = base.GetComponent<GrassDataProvider>();
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x000281B8 File Offset: 0x000263B8
	private void OnDisable()
	{
		ComputeBuffer geometryBuffer = this.GeometryBuffer;
		if (geometryBuffer != null)
		{
			geometryBuffer.Dispose();
		}
		ComputeBuffer argumentsBuffer = this.ArgumentsBuffer;
		if (argumentsBuffer != null)
		{
			argumentsBuffer.Dispose();
		}
		ComputeBuffer grassPointsBuffer = this.GrassPointsBuffer;
		if (grassPointsBuffer != null)
		{
			grassPointsBuffer.Dispose();
		}
		this.GeometryBuffer = null;
		this.ArgumentsBuffer = null;
		this.GrassPointsBuffer = null;
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x0002820D File Offset: 0x0002640D
	private void Update()
	{
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00028210 File Offset: 0x00026410
	private void Render()
	{
		if (!this.DataProvider)
		{
			this.DataProvider = base.GetComponent<GrassDataProvider>();
		}
		if (this.GrassPointsBuffer == null || this.DataProvider.IsDirty())
		{
			ComputeBuffer grassPointsBuffer = this.GrassPointsBuffer;
			if (grassPointsBuffer != null)
			{
				grassPointsBuffer.Dispose();
			}
			this.GrassPointsBuffer = this.DataProvider.GetData();
		}
		Camera camera = null;
		if (Application.isPlaying)
		{
			camera = MainCamera.instance.cam;
		}
		if (!GrassChunking.ShouldDrawChunk(GrassChunking.GetChunkFromPosition(camera.transform.position), this.CurrentChunk))
		{
			return;
		}
		if (this.GeometryBuffer == null)
		{
			this.GeometryBuffer = new ComputeBuffer(10000, 148, 2);
			this.ArgumentsBuffer = new ComputeBuffer(1, 16, 256);
		}
		this.GeometryBuffer.SetCounterValue(0U);
		this.ArgumentsBuffer.SetData(this.argsBufferReset);
		this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "GeometryBuffer", this.GeometryBuffer);
		this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "IndirectArgsBuffer", this.ArgumentsBuffer);
		this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "GrassPoints", this.GrassPointsBuffer);
		this.grassComputeShader.SetFloat("_Time", Time.realtimeSinceStartup);
		this.grassComputeShader.SetVector("_CameraWSPos", camera.transform.position);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetBuffer("GeometryBuffer", this.GeometryBuffer);
		uint num;
		uint num2;
		uint num3;
		this.grassComputeShader.GetKernelThreadGroupSizes(this.grassGeometryKernel.kernelID, ref num, ref num2, ref num3);
		this.grassGeometryKernel.Dispatch(new int3(this.GrassPointsBuffer.count, 1, 1));
		Graphics.DrawProceduralIndirect(this.m_grassRenderMaterial, new Bounds(base.transform.position, Vector3.one * 500f), 0, this.ArgumentsBuffer, 0, null, materialPropertyBlock, 0, true, 0);
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00028411 File Offset: 0x00026611
	public GrassRenderer()
	{
		int[] array = new int[4];
		array[1] = 1;
		this.argsBufferReset = array;
		base..ctor();
	}

	// Token: 0x04000701 RID: 1793
	public int3 CurrentChunk = int3.zero;

	// Token: 0x04000702 RID: 1794
	public ComputeShader grassComputeShader;

	// Token: 0x04000703 RID: 1795
	private ComputeKernel grassGeometryKernel;

	// Token: 0x04000704 RID: 1796
	private ComputeBuffer GeometryBuffer;

	// Token: 0x04000705 RID: 1797
	private ComputeBuffer ArgumentsBuffer;

	// Token: 0x04000706 RID: 1798
	private ComputeBuffer GrassPointsBuffer;

	// Token: 0x04000707 RID: 1799
	private const int MAX_GRASS = 10000;

	// Token: 0x04000708 RID: 1800
	private const int DRAW_STRIDE = 148;

	// Token: 0x04000709 RID: 1801
	private const int INDIRECT_DRAW_ARGS_STIDE = 16;

	// Token: 0x0400070A RID: 1802
	private int[] argsBufferReset;

	// Token: 0x0400070B RID: 1803
	public Material m_grassRenderMaterial;

	// Token: 0x0400070C RID: 1804
	private GrassDataProvider DataProvider;
}
