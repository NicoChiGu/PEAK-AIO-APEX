using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x02000294 RID: 660
public class LoadingScreenAnimationSimple : MonoBehaviour
{
	// Token: 0x0600123A RID: 4666 RVA: 0x0005C544 File Offset: 0x0005A744
	private void Start()
	{
		base.StartCoroutine(this.AnimateRoutine());
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x0005C553 File Offset: 0x0005A753
	private IEnumerator AnimateRoutine()
	{
		float dots = 0f;
		for (;;)
		{
			yield return new WaitForSeconds(this.yieldTime);
			if (dots == 0f)
			{
				this.loading.text = LocalizedText.GetText("LOADING", true);
			}
			else if (dots == 1f)
			{
				this.loading.text = LocalizedText.GetText("LOADING", true) + ".";
			}
			else if (dots == 2f)
			{
				this.loading.text = LocalizedText.GetText("LOADING", true) + "..";
			}
			else if (dots == 3f)
			{
				this.loading.text = LocalizedText.GetText("LOADING", true) + "...";
			}
			float num = dots;
			dots = num + 1f;
			if (dots > 3f)
			{
				dots = 0f;
			}
		}
		yield break;
	}

	// Token: 0x040010BC RID: 4284
	public float yieldTime = 1f;

	// Token: 0x040010BD RID: 4285
	public TMP_Text loading;
}
