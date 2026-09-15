using System;
using UnityEngine;

// Token: 0x0200023D RID: 573
public class CustomizationRefs : MonoBehaviour
{
	// Token: 0x060010D5 RID: 4309 RVA: 0x00054BD8 File Offset: 0x00052DD8
	public void SetSkeleton(bool active, bool isLocal)
	{
		if (active)
		{
			for (int i = 0; i < this.AllRenderers.Length; i++)
			{
				this.AllRenderers[i].enabled = false;
			}
			this.skeletonRenderer.gameObject.SetActive(true);
			if (isLocal)
			{
				((SkinnedMeshRenderer)this.skeletonRenderer).sharedMesh = this.skeletonFirstPerson;
				this.skeletonRenderer.material = this.skeletonFirstPersonMat;
			}
			else
			{
				((SkinnedMeshRenderer)this.skeletonRenderer).sharedMesh = this.skeletonThirdPerson;
				this.skeletonRenderer.material = this.skeletonThirdPersonMat;
			}
			this.mainRendererShadow.sharedMesh = this.skeletonThirdPerson;
			this.skirtShadow.enabled = false;
			this.shortsShadow.enabled = false;
			this.sashRenderer.enabled = true;
			this.thirdEye.GetComponent<Renderer>().enabled = false;
			return;
		}
		for (int j = 0; j < this.AllRenderers.Length; j++)
		{
			this.AllRenderers[j].enabled = true;
		}
		this.skeletonRenderer.gameObject.SetActive(false);
		this.mainRendererShadow.sharedMesh = this.mainRenderer.sharedMesh;
		this.skirtShadow.enabled = true;
		this.shortsShadow.enabled = true;
		this.thirdEye.GetComponent<Renderer>().enabled = true;
		if (this.thirdEye.activeSelf)
		{
			this.accessoryRenderer.enabled = false;
		}
	}

	// Token: 0x04000F22 RID: 3874
	public SkinnedMeshRenderer mainRenderer;

	// Token: 0x04000F23 RID: 3875
	public SkinnedMeshRenderer mainRendererShadow;

	// Token: 0x04000F24 RID: 3876
	public Renderer[] PlayerRenderers;

	// Token: 0x04000F25 RID: 3877
	public Renderer[] EyeRenderers;

	// Token: 0x04000F26 RID: 3878
	public Renderer mouthRenderer;

	// Token: 0x04000F27 RID: 3879
	public Renderer accessoryRenderer;

	// Token: 0x04000F28 RID: 3880
	public Renderer shorts;

	// Token: 0x04000F29 RID: 3881
	public Renderer skirt;

	// Token: 0x04000F2A RID: 3882
	public Renderer skirtShadow;

	// Token: 0x04000F2B RID: 3883
	public Renderer shortsShadow;

	// Token: 0x04000F2C RID: 3884
	public Renderer[] playerHats;

	// Token: 0x04000F2D RID: 3885
	public MeshFilter hatShadowMeshFilter;

	// Token: 0x04000F2E RID: 3886
	public Renderer sashRenderer;

	// Token: 0x04000F2F RID: 3887
	public Renderer blindRenderer;

	// Token: 0x04000F30 RID: 3888
	public Renderer chickenRenderer;

	// Token: 0x04000F31 RID: 3889
	public Renderer skeletonRenderer;

	// Token: 0x04000F32 RID: 3890
	public Mesh skeletonFirstPerson;

	// Token: 0x04000F33 RID: 3891
	public Mesh skeletonThirdPerson;

	// Token: 0x04000F34 RID: 3892
	public Material skeletonFirstPersonMat;

	// Token: 0x04000F35 RID: 3893
	public Material skeletonThirdPersonMat;

	// Token: 0x04000F36 RID: 3894
	public Material[] sashAscentMaterials;

	// Token: 0x04000F37 RID: 3895
	public Transform hatTransform;

	// Token: 0x04000F38 RID: 3896
	public Renderer[] AllRenderers;

	// Token: 0x04000F39 RID: 3897
	public GameObject thirdEye;
}
