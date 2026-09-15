using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

// Token: 0x0200011D RID: 285
public class ItemRenderFeatureManager : MonoBehaviour
{
	// Token: 0x06000917 RID: 2327 RVA: 0x000309CA File Offset: 0x0002EBCA
	private void Start()
	{
		this.getRendererFeature();
		if (this.disabledOnStart)
		{
			this.setFeatureActive(false);
		}
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x000309E4 File Offset: 0x0002EBE4
	private void getRendererFeature()
	{
		foreach (ScriptableRendererFeature scriptableRendererFeature in this.rend.rendererFeatures)
		{
			if (scriptableRendererFeature.name == this.featureName)
			{
				this.rendererFeature = scriptableRendererFeature;
			}
		}
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00030A50 File Offset: 0x0002EC50
	public void setFeatureActive(bool active)
	{
		this.rendererFeature.SetActive(active);
		MonoBehaviour.print(active);
		this.rendererFeature != null;
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00030A76 File Offset: 0x0002EC76
	private void OnDisable()
	{
		this.setFeatureActive(false);
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00030A7F File Offset: 0x0002EC7F
	private void Update()
	{
	}

	// Token: 0x04000888 RID: 2184
	private ScriptableRendererFeature rendererFeature;

	// Token: 0x04000889 RID: 2185
	[FormerlySerializedAs("falseOnStart")]
	public bool disabledOnStart;

	// Token: 0x0400088A RID: 2186
	public UniversalRendererData rend;

	// Token: 0x0400088B RID: 2187
	public string featureName;
}
