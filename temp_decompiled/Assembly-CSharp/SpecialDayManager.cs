using System;
using UnityEngine;

// Token: 0x020001AF RID: 431
public class SpecialDayManager : MonoBehaviour
{
	// Token: 0x06000D54 RID: 3412 RVA: 0x00042DFA File Offset: 0x00040FFA
	private void Start()
	{
		this.zones = Object.FindObjectsByType<SpecialDayZone>(0, 0);
		this.startFog = AmbienceManager.instance.maxFog;
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00042E1C File Offset: 0x0004101C
	private void Update()
	{
		float num = 0f;
		if (!Character.observedCharacter)
		{
			return;
		}
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].outerBounds.Contains(Character.observedCharacter.Center))
			{
				Color specialSunColor = DayNightManager.instance.specialSunColor;
				Color specialTopColor = DayNightManager.instance.specialTopColor;
				Color specialMidColor = DayNightManager.instance.specialMidColor;
				Color specialBottomColor = DayNightManager.instance.specialBottomColor;
				float maxFog = AmbienceManager.instance.maxFog;
				float num2 = Vector3.Distance(Character.observedCharacter.Center, this.zones[i].bounds.ClosestPoint(Character.observedCharacter.Center));
				num2 /= this.zones[i].blendSize;
				float num3 = 1f - num2 * 2f;
				this.debugblend = num3;
				num3 = Mathf.Clamp01(num3);
				if (this.zones[i].overrideSun)
				{
					float num4 = Mathf.Lerp(this.zones[i].nightLightIntensity, this.zones[i].daylLightIntensity, DayNightManager.instance.dayNightBlend);
					DayNightManager.instance.specialSunColor = Color.Lerp(specialSunColor, this.zones[i].specialSunColor * num4, num3);
				}
				if (this.zones[i].useCustomSun)
				{
					DayNightManager.instance.specialDaySunBlend = Mathf.Max(num, num3);
					if (this.zones[i].specialLight != null)
					{
						this.zones[i].specialLight.enabled = true;
						Color color = Color.Lerp(specialSunColor, this.zones[i].specialSunColor, num3);
						color *= num3;
						this.zones[i].specialLight.color = color;
						DayNightManager.instance.specialSunColor *= 1f - num3;
					}
				}
				Shader.SetGlobalFloat("SpecialDayBlend", Mathf.Lerp(0f, 1f, num3));
				if (this.zones[i].useCustomColorVals)
				{
					DayNightManager.instance.specialDaySkyBlend = Mathf.Max(num, num3);
					DayNightManager.instance.specialTopColor = Color.Lerp(specialTopColor, this.zones[i].specialTopColor, num3);
					DayNightManager.instance.specialMidColor = Color.Lerp(specialMidColor, this.zones[i].specialMidColor, num3);
					DayNightManager.instance.specialBottomColor = Color.Lerp(specialBottomColor, this.zones[i].specialBottomColor, num3);
				}
				if (this.zones[i].overrideFog)
				{
					float num5 = Mathf.Lerp(maxFog, this.zones[i].fogDensity, num3);
					num5 = Mathf.Lerp(this.startFog, num5, Mathf.Max(num, num3));
					AmbienceManager.instance.maxFog = num5;
				}
				if (this.zones[i].globalShaderVals.Length != 0)
				{
					float num6 = 0f;
					for (int j = 0; j < this.zones[i].globalShaderVals.Length; j++)
					{
						float num7 = Mathf.Lerp(num6, this.zones[i].globalShaderVals[j].value, num3);
						num7 *= Mathf.Max(num, num3);
						Shader.SetGlobalFloat(DayNightManager.instance.getShaderValue(this.zones[i].globalShaderVals[j].parameter), num7);
						num6 = num7;
					}
				}
				num = num3;
			}
			else if (this.zones[i].specialLight != null)
			{
				this.zones[i].specialLight.enabled = false;
			}
		}
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x000431BC File Offset: 0x000413BC
	private void OnDisable()
	{
		Shader.SetGlobalFloat("SpecialDayBlend", 0f);
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].globalShaderVals.Length != 0)
			{
				for (int j = 0; j < this.zones[i].globalShaderVals.Length; j++)
				{
					Shader.SetGlobalFloat(DayNightManager.instance.getShaderValue(this.zones[i].globalShaderVals[j].parameter), 0f);
				}
			}
		}
	}

	// Token: 0x04000B8B RID: 2955
	public SpecialDayZone[] zones;

	// Token: 0x04000B8C RID: 2956
	public float debug;

	// Token: 0x04000B8D RID: 2957
	private float startFog;

	// Token: 0x04000B8E RID: 2958
	public float debugblend;
}
