using System;
using TMPro;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class GuidebookSpread : MonoBehaviour
{
	// Token: 0x060008F2 RID: 2290 RVA: 0x00030290 File Offset: 0x0002E490
	internal void SetPageLeft(RectTransform prefab)
	{
		if (this.pageLeftTransform != null)
		{
			Object.DestroyImmediate(this.pageLeftTransform.gameObject);
		}
		this.pageLeftTransform = Object.Instantiate<RectTransform>(prefab, base.transform);
		this.pageLeftTransform.offsetMax = new Vector2(-this.page1AlignmentRight, -this.page1AlignmentTop);
		this.pageLeftTransform.offsetMin = new Vector2(this.page1AlignmentLeft, this.page1AlignmentBottom);
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00030308 File Offset: 0x0002E508
	internal void SetPageRight(RectTransform prefab)
	{
		if (this.pageRightTransform != null)
		{
			Object.DestroyImmediate(this.pageRightTransform.gameObject);
		}
		this.pageRightTransform = Object.Instantiate<RectTransform>(prefab, base.transform);
		this.pageRightTransform.offsetMax = new Vector2(-this.page1AlignmentLeft, -this.page1AlignmentTop);
		this.pageRightTransform.offsetMin = new Vector2(this.page1AlignmentRight, this.page1AlignmentTop);
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00030380 File Offset: 0x0002E580
	internal void ClearContents()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x04000872 RID: 2162
	public TextMeshProUGUI pageNumberLeft;

	// Token: 0x04000873 RID: 2163
	public TextMeshProUGUI pageNumberRight;

	// Token: 0x04000874 RID: 2164
	public RectTransform pageLeftTransform;

	// Token: 0x04000875 RID: 2165
	public RectTransform pageRightTransform;

	// Token: 0x04000876 RID: 2166
	public float page1AlignmentLeft;

	// Token: 0x04000877 RID: 2167
	public float page1AlignmentRight;

	// Token: 0x04000878 RID: 2168
	public float page1AlignmentTop;

	// Token: 0x04000879 RID: 2169
	public float page1AlignmentBottom;
}
