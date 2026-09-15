using System;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Peak
{
	// Token: 0x020003B5 RID: 949
	public static class IntegratedGraphicsHelper
	{
		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600186D RID: 6253 RVA: 0x0007C2A4 File Offset: 0x0007A4A4
		public static bool IsUsingIntegratedGraphics
		{
			get
			{
				string graphicsDeviceName = SystemInfo.graphicsDeviceName;
				return graphicsDeviceName.Contains("Integrated") || (graphicsDeviceName.Contains("Intel") && !graphicsDeviceName.Contains("Arc")) || (graphicsDeviceName.Contains("Radeon") && graphicsDeviceName.Contains("Vega")) || graphicsDeviceName.Contains("Look I know this is dumb but it's the best I can do right now ok? Don't @ me.");
			}
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x0007C308 File Offset: 0x0007A508
		public static void DisableGPUCullingIfNecessary()
		{
			bool flag2;
			bool flag = CLI.TryGetArg("gpuCulling", out flag2);
			if (!IntegratedGraphicsHelper.IsUsingIntegratedGraphics && (!flag || flag2))
			{
				return;
			}
			if (IntegratedGraphicsHelper.IsUsingIntegratedGraphics && flag && flag2)
			{
				Debug.Log("Using integrated graphics, but GPU culling forced to remain on with override.");
				return;
			}
			if (IntegratedGraphicsHelper.IsUsingIntegratedGraphics)
			{
				Debug.Log("Integrated graphics detected! (" + SystemInfo.graphicsDeviceName + "). Disabling GPU culling. ");
			}
			else
			{
				Debug.Log("GPU culling forced off by command-line override!");
			}
			UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (universalRenderPipelineAsset == null)
			{
				Debug.LogError("uhhh... where is our URP asset?");
				return;
			}
			universalRenderPipelineAsset.gpuResidentDrawerEnableOcclusionCullingInCameras = false;
		}
	}
}
