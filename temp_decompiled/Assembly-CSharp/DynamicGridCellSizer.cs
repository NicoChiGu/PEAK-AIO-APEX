using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200024F RID: 591
[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGridCellSizer : MonoBehaviour
{
	// Token: 0x06001114 RID: 4372 RVA: 0x00056027 File Offset: 0x00054227
	private void Awake()
	{
		this.grid = base.GetComponent<GridLayoutGroup>();
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x00056035 File Offset: 0x00054235
	private void Update()
	{
		if (base.transform.childCount != this.childCount)
		{
			this.childCount = base.transform.childCount;
			this.ResizeCells();
		}
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x00056064 File Offset: 0x00054264
	public void ResizeCells()
	{
		this.iconCount = this.grid.transform.childCount;
		float width = this.gridRectTransform.rect.width;
		float height = this.gridRectTransform.rect.height;
		int num = Mathf.Max(1, Mathf.CeilToInt((float)this.iconCount / (float)this.maxIconsPerRow));
		Debug.Log("Rows!" + num.ToString());
		int num2 = Mathf.CeilToInt((float)this.iconCount / (float)num);
		float num3 = (width - (float)this.grid.padding.left - (float)this.grid.padding.right - this.grid.spacing.x * (float)(num2 - 1)) / (float)num2;
		float num4 = (height - (float)this.grid.padding.top - (float)this.grid.padding.bottom - this.grid.spacing.y * (float)(num - 1)) / (float)num;
		float num5 = Mathf.Min(num3, num4);
		this.grid.cellSize = new Vector2(num5, num5);
	}

	// Token: 0x04000F82 RID: 3970
	public RectTransform gridRectTransform;

	// Token: 0x04000F83 RID: 3971
	public int iconCount;

	// Token: 0x04000F84 RID: 3972
	public int maxIconsPerRow = 8;

	// Token: 0x04000F85 RID: 3973
	private GridLayoutGroup grid;

	// Token: 0x04000F86 RID: 3974
	private int childCount = -1;
}
