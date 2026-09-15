using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200026C RID: 620
public class HideTheBody : MonoBehaviour
{
	// Token: 0x06001188 RID: 4488 RVA: 0x00058390 File Offset: 0x00056590
	private void Start()
	{
		this.character = base.GetComponentInParent<Character>();
		this.Toggle(true);
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x000583A8 File Offset: 0x000565A8
	private void Update()
	{
		bool flag = !this.character.IsLocal || this.character.data.fullyPassedOut || this.character.data.dead || this.isDummy;
		if (!this.character.IsLocal && this.character.data.carrier != null && this.character.data.carrier.IsLocal)
		{
			flag = false;
		}
		if (flag != this.isShowing)
		{
			this.Toggle(flag);
		}
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x0005843E File Offset: 0x0005663E
	public void Refresh()
	{
		this.Toggle(this.isShowing);
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x0005844C File Offset: 0x0005664C
	private void Toggle(bool show)
	{
		this.isShowing = show;
		this.shadowCaster.SetActive(!show);
		this.shadowCasterHat.SetActive(!show);
		if (show)
		{
			this.SetShowing(this.body, 0f);
			this.SetShowing(this.headRend, 0f);
			this.SetShowing(this.sash, 0f);
			for (int i = 0; i < this.costumes.Length; i++)
			{
				this.SetShowing(this.costumes[i], 0f);
			}
			Renderer[] componentsInChildren = this.face.GetComponentsInChildren<Renderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				this.SetShowing(componentsInChildren[j], 0f);
			}
			for (int k = 0; k < this.refs.playerHats.Length; k++)
			{
				this.SetShowing(this.refs.playerHats[k], 0f);
			}
			return;
		}
		this.SetShowing(this.body, 1f);
		this.SetShowing(this.headRend, 1f);
		this.SetShowing(this.sash, 1f);
		for (int l = 0; l < this.costumes.Length; l++)
		{
			this.SetShowing(this.costumes[l], 1f);
		}
		Renderer[] componentsInChildren2 = this.face.GetComponentsInChildren<Renderer>();
		for (int m = 0; m < componentsInChildren2.Length; m++)
		{
			this.SetShowing(componentsInChildren2[m], 1f);
		}
		for (int n = 0; n < this.refs.playerHats.Length; n++)
		{
			this.SetShowing(this.refs.playerHats[n], 1f);
		}
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x000585FC File Offset: 0x000567FC
	public void SetShowing(Renderer r, float x)
	{
		Material[] materials = r.materials;
		Material[] array = materials;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(this.VERTEXGHOST, x);
		}
		r.materials = materials;
	}

	// Token: 0x04001001 RID: 4097
	public bool isDummy;

	// Token: 0x04001002 RID: 4098
	public SkinnedMeshRenderer body;

	// Token: 0x04001003 RID: 4099
	public Renderer headRend;

	// Token: 0x04001004 RID: 4100
	public CustomizationRefs refs;

	// Token: 0x04001005 RID: 4101
	public Transform face;

	// Token: 0x04001006 RID: 4102
	public GameObject shadowCaster;

	// Token: 0x04001007 RID: 4103
	public GameObject shadowCasterHat;

	// Token: 0x04001008 RID: 4104
	public SkinnedMeshRenderer[] costumes;

	// Token: 0x04001009 RID: 4105
	[FormerlySerializedAs("Sash")]
	public SkinnedMeshRenderer sash;

	// Token: 0x0400100A RID: 4106
	private bool isShowing = true;

	// Token: 0x0400100B RID: 4107
	private Character character;

	// Token: 0x0400100C RID: 4108
	private int VERTEXGHOST = Shader.PropertyToID("_VertexGhost");
}
