using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000293 RID: 659
[ExecuteAlways]
public class LoadingScreenAnimation : MonoBehaviour
{
	// Token: 0x06001237 RID: 4663 RVA: 0x0005C368 File Offset: 0x0005A568
	private void Start()
	{
		string text = LocalizedText.GetText("LOADING", true);
		if (LocalizedText.CURRENT_LANGUAGE == LocalizedText.Language.SimplifiedChinese)
		{
			this.loadingString = string.Concat(new string[] { text, "...", text, "...", text, "...", text, "..." });
			this.defaultLoadingStringLength = (float)this.loadingString.Length;
			return;
		}
		if (LocalizedText.CURRENT_LANGUAGE == LocalizedText.Language.Korean || LocalizedText.CURRENT_LANGUAGE == LocalizedText.Language.Japanese)
		{
			this.loadingString = string.Concat(new string[] { text, "...", text, "...", text, "..." });
			this.defaultLoadingStringLength = (float)this.loadingString.Length;
			return;
		}
		this.loadingString = string.Concat(new string[] { text, "...", text, "...", text, "...", text, "...", text, "..." });
		this.defaultLoadingStringLength = 50f;
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x0005C494 File Offset: 0x0005A694
	private void Update()
	{
		this.barFill.fillAmount = Mathf.Lerp(this.barFillMinMax.x, this.barFillMinMax.y, this.fillAmount);
		this.planeRotation.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(this.planeRotationMinMax.x, this.planeRotationMinMax.y, this.fillAmount));
		this.loadingText.text = this.loadingString.Substring(0, Mathf.RoundToInt(this.defaultLoadingStringLength * this.fillAmount));
	}

	// Token: 0x040010B3 RID: 4275
	public Image barFill;

	// Token: 0x040010B4 RID: 4276
	public Transform planeRotation;

	// Token: 0x040010B5 RID: 4277
	public TMP_Text loadingText;

	// Token: 0x040010B6 RID: 4278
	[Range(0f, 1f)]
	public float fillAmount;

	// Token: 0x040010B7 RID: 4279
	public Vector2 barFillMinMax;

	// Token: 0x040010B8 RID: 4280
	public Vector2 planeRotationMinMax;

	// Token: 0x040010B9 RID: 4281
	private string loadingString;

	// Token: 0x040010BA RID: 4282
	private float defaultLoadingStringLength = 50f;

	// Token: 0x040010BB RID: 4283
	public float maxFill;
}
