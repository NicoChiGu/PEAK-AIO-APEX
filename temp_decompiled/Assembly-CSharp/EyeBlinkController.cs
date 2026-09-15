using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Token: 0x020000B6 RID: 182
public class EyeBlinkController : MonoBehaviour
{
	// Token: 0x060006AB RID: 1707 RVA: 0x000262DC File Offset: 0x000244DC
	private void Start()
	{
		this.character = base.GetComponentInParent<Character>();
		if (!this.character.IsLocal)
		{
			base.enabled = false;
			return;
		}
		foreach (ScriptableRendererFeature scriptableRendererFeature in this.rend.rendererFeatures)
		{
			if (scriptableRendererFeature.name == "Eye Blink")
			{
				this.rendererFeature = scriptableRendererFeature;
			}
		}
		this.rendererFeature.SetActive(true);
		this.setEyeBlinkActive();
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x0002637C File Offset: 0x0002457C
	private void setEyeBlinkActive()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		this.eyeBlinkMaterial.SetFloat("_EyeOpen", (float)(this.enableEyeBlink ? 1 : 0));
		if (!this.enableEyeBlink)
		{
			this.rendererFeature.SetActive(false);
			this.eyeBlinkMaterial.SetFloat("_EyeOpen", 1f);
		}
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x000263E0 File Offset: 0x000245E0
	private void Update()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		if (this.character.data.passedOutOnTheBeach > 0f)
		{
			this.eyeOpenValue = 0f;
			this.enableEyeBlink = true;
		}
		else
		{
			this.eyeOpenValue = Mathf.MoveTowards(this.eyeOpenValue, 1f, Time.deltaTime * 0.15f);
			if (this.eyeOpenValue >= 0.999f)
			{
				this.enableEyeBlink = false;
				this.rendererFeature.SetActive(false);
			}
			else
			{
				this.enableEyeBlink = true;
			}
		}
		if (this.enableEyeBlink)
		{
			this.rendererFeature.SetActive(true);
			this.eyeBlinkMaterial.SetFloat("_EyeOpen", Mathf.Clamp01(this.openCurve.Evaluate(this.eyeOpenValue)));
		}
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x000264AA File Offset: 0x000246AA
	private void OnDisable()
	{
		if (this.rendererFeature != null)
		{
			this.rendererFeature.SetActive(false);
		}
		this.eyeBlinkMaterial.SetFloat("_EyeOpen", 1f);
	}

	// Token: 0x040006CA RID: 1738
	private Character character;

	// Token: 0x040006CB RID: 1739
	public UniversalRendererData rend;

	// Token: 0x040006CC RID: 1740
	public Material eyeBlinkMaterial;

	// Token: 0x040006CD RID: 1741
	public bool enableEyeBlink;

	// Token: 0x040006CE RID: 1742
	public AnimationCurve openCurve;

	// Token: 0x040006CF RID: 1743
	[Range(0f, 1f)]
	public float eyeOpenValue;

	// Token: 0x040006D0 RID: 1744
	private ScriptableRendererFeature rendererFeature;
}
