using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x020001AA RID: 426
public class SingleBufferFeature : ScriptableRendererFeature
{
	// Token: 0x06000D43 RID: 3395 RVA: 0x00042A50 File Offset: 0x00040C50
	public override void Create()
	{
		this.m_ScriptablePass = new SingleBufferFeature.CustomRenderPass(this.settings, base.name);
		this.m_ScriptablePass.renderPassEvent = this.settings._event;
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00042A80 File Offset: 0x00040C80
	public unsafe override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		CameraType cameraType = *renderingData.cameraData.cameraType;
		if (cameraType == 4)
		{
			return;
		}
		if (!this.settings.showInSceneView && cameraType == 2)
		{
			return;
		}
		renderer.EnqueuePass(this.m_ScriptablePass);
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x00042ABD File Offset: 0x00040CBD
	protected override void Dispose(bool disposing)
	{
		this.m_ScriptablePass.Dispose();
	}

	// Token: 0x04000B7B RID: 2939
	public SingleBufferFeature.Settings settings = new SingleBufferFeature.Settings();

	// Token: 0x04000B7C RID: 2940
	private SingleBufferFeature.CustomRenderPass m_ScriptablePass;

	// Token: 0x020004A0 RID: 1184
	public class CustomRenderPass : ScriptableRenderPass
	{
		// Token: 0x06001BC3 RID: 7107 RVA: 0x000831F4 File Offset: 0x000813F4
		public CustomRenderPass(SingleBufferFeature.Settings settings, string name)
		{
			this.settings = settings;
			this.filteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.transparent), settings.layerMask, uint.MaxValue, 0);
			this.shaderTagsList.Add(new ShaderTagId("SRPDefaultUnlit"));
			this.shaderTagsList.Add(new ShaderTagId("UniversalForward"));
			this.shaderTagsList.Add(new ShaderTagId("UniversalForwardOnly"));
			this._profilingSampler = new ProfilingSampler(name);
		}

		// Token: 0x06001BC4 RID: 7108 RVA: 0x00083288 File Offset: 0x00081488
		public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor renderTextureDescriptor = *renderingData.cameraData.cameraTargetDescriptor;
			renderTextureDescriptor.depthBufferBits = 0;
			RenderingUtils.ReAllocateIfNeeded(ref this.rtTempColor, ref renderTextureDescriptor, 0, 0, false, 1, 0f, "_TemporaryColorTexture");
			if (this.settings.colorTargetDestinationID != "")
			{
				RenderingUtils.ReAllocateIfNeeded(ref this.rtCustomColor, ref renderTextureDescriptor, 0, 0, false, 1, 0f, this.settings.colorTargetDestinationID);
			}
			else
			{
				this.rtCustomColor = renderingData.cameraData.renderer->cameraColorTargetHandle;
			}
			RTHandle cameraDepthTargetHandle = renderingData.cameraData.renderer->cameraDepthTargetHandle;
			base.ConfigureTarget(this.rtCustomColor, cameraDepthTargetHandle);
			base.ConfigureClear(1, new Color(0f, 0f, 0f, 0f));
		}

		// Token: 0x06001BC5 RID: 7109 RVA: 0x0008335C File Offset: 0x0008155C
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer commandBuffer = CommandBufferPool.Get();
			ProfilingScope profilingScope;
			profilingScope..ctor(commandBuffer, this._profilingSampler);
			try
			{
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
				SortingCriteria sortingCriteria = 23;
				DrawingSettings drawingSettings = base.CreateDrawingSettings(this.shaderTagsList, ref renderingData, sortingCriteria);
				if (this.settings.overrideMaterial != null)
				{
					drawingSettings.overrideMaterialPassIndex = this.settings.overrideMaterialPass;
					drawingSettings.overrideMaterial = this.settings.overrideMaterial;
				}
				context.DrawRenderers(*renderingData.cullResults, ref drawingSettings, ref this.filteringSettings);
				if (this.settings.colorTargetDestinationID != "")
				{
					commandBuffer.SetGlobalTexture(this.settings.colorTargetDestinationID, this.rtCustomColor);
				}
				if (this.settings.blitMaterial != null)
				{
					RTHandle cameraColorTargetHandle = renderingData.cameraData.renderer->cameraColorTargetHandle;
					if (cameraColorTargetHandle != null && this.rtTempColor != null)
					{
						Blitter.BlitCameraTexture(commandBuffer, cameraColorTargetHandle, this.rtTempColor, this.settings.blitMaterial, 0);
						Blitter.BlitCameraTexture(commandBuffer, this.rtTempColor, cameraColorTargetHandle, 0f, false);
					}
				}
			}
			finally
			{
				profilingScope.Dispose();
			}
			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
			CommandBufferPool.Release(commandBuffer);
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x000834C4 File Offset: 0x000816C4
		public override void OnCameraCleanup(CommandBuffer cmd)
		{
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x000834C6 File Offset: 0x000816C6
		public void Dispose()
		{
			if (this.settings.colorTargetDestinationID != "")
			{
				RTHandle rthandle = this.rtCustomColor;
				if (rthandle != null)
				{
					rthandle.Release();
				}
			}
			RTHandle rthandle2 = this.rtTempColor;
			if (rthandle2 == null)
			{
				return;
			}
			rthandle2.Release();
		}

		// Token: 0x040019D7 RID: 6615
		private SingleBufferFeature.Settings settings;

		// Token: 0x040019D8 RID: 6616
		private FilteringSettings filteringSettings;

		// Token: 0x040019D9 RID: 6617
		private ProfilingSampler _profilingSampler;

		// Token: 0x040019DA RID: 6618
		private List<ShaderTagId> shaderTagsList = new List<ShaderTagId>();

		// Token: 0x040019DB RID: 6619
		private RTHandle rtCustomColor;

		// Token: 0x040019DC RID: 6620
		private RTHandle rtTempColor;
	}

	// Token: 0x020004A1 RID: 1185
	[Serializable]
	public class Settings
	{
		// Token: 0x040019DD RID: 6621
		public bool showInSceneView = true;

		// Token: 0x040019DE RID: 6622
		public RenderPassEvent _event = 300;

		// Token: 0x040019DF RID: 6623
		[Header("Draw Renderers Settings")]
		public LayerMask layerMask = 1;

		// Token: 0x040019E0 RID: 6624
		public Material overrideMaterial;

		// Token: 0x040019E1 RID: 6625
		public int overrideMaterialPass;

		// Token: 0x040019E2 RID: 6626
		public string colorTargetDestinationID = "";

		// Token: 0x040019E3 RID: 6627
		[Header("Blit Settings")]
		public Material blitMaterial;
	}
}
