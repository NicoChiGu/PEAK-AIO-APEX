using System;
using UnityEngine;

// Token: 0x0200025E RID: 606
[ExecuteInEditMode]
public class FogConfig : MonoBehaviour
{
	// Token: 0x0600114E RID: 4430 RVA: 0x00056E29 File Offset: 0x00055029
	private void Start()
	{
		Shader.SetGlobalFloat("_WeatherBlend", 0f);
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x00056E3C File Offset: 0x0005503C
	private void Update()
	{
		this.sinceSet += Time.deltaTime;
		if (FogConfig.currentFog == this && this.sinceSet > 0.1f && this.sinceSet < 10f)
		{
			float num = Shader.GetGlobalFloat("_WeatherBlend");
			if (num > 0f)
			{
				num = Mathf.MoveTowards(num, 0f, Time.deltaTime * 0.3f);
				Shader.SetGlobalFloat("_WeatherBlend", num);
			}
		}
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x00056EB8 File Offset: 0x000550B8
	public void SetFog()
	{
		FogConfig.currentFog = this;
		this.sinceSet = 0f;
		Shader.SetGlobalTexture("_WindTexture", this.windTexture);
		float num = Shader.GetGlobalFloat("_WeatherBlend");
		if (this.useWindChillZoneIntensity)
		{
			num = this.zone.windIntensity;
		}
		else
		{
			num = Mathf.MoveTowards(num, this.maxVal, Time.deltaTime * 0.3f);
		}
		Shader.SetGlobalFloat("_WeatherBlend", num);
		Shader.SetGlobalColor("WindTint", this.windTint);
		Shader.SetGlobalFloat("WindSkyBrightnessValue", this.windSkyBrightnessValue);
		Shader.SetGlobalFloat("WindTextureInfluence", this.windTextureInfluence);
		Shader.SetGlobalFloat("WindFogDensity", this.windFogDensity);
		Shader.SetGlobalFloat("WindFogTextureDensity", this.WindFogTextureDensity);
		Shader.SetGlobalFloat("WindMixInfluence", this.windMixInfluence);
		Shader.SetGlobalVector("WindSpeed", new Vector4(this.windSpeed.x, this.windSpeed.y, 0f, 0f));
		Vector3 forward = base.transform.forward;
		Vector3 vector = -Vector3.Cross(Vector3.up, forward);
		float num2 = Vector3.Angle(Vector3.up, forward);
		if (this.straightDown)
		{
			vector = Vector3.forward;
			num2 = 180f;
		}
		Shader.SetGlobalVector("WindRotationAxis", new Vector4(vector.x, vector.y, vector.z, 0f));
		Shader.SetGlobalFloat("WindRotationAngle", num2);
		Shader.SetGlobalFloat("WindSphereScale", this.windSphereScale);
	}

	// Token: 0x04000FBB RID: 4027
	public static FogConfig currentFog;

	// Token: 0x04000FBC RID: 4028
	public float windSkyBrightnessValue = 0.2f;

	// Token: 0x04000FBD RID: 4029
	public float windTextureInfluence = 0.2f;

	// Token: 0x04000FBE RID: 4030
	public Color windTint = Color.white;

	// Token: 0x04000FBF RID: 4031
	public Texture windTexture;

	// Token: 0x04000FC0 RID: 4032
	public float windSphereScale = 5f;

	// Token: 0x04000FC1 RID: 4033
	public Vector2 windSpeed;

	// Token: 0x04000FC2 RID: 4034
	public float windFogDensity = 50f;

	// Token: 0x04000FC3 RID: 4035
	public float WindFogTextureDensity = 15f;

	// Token: 0x04000FC4 RID: 4036
	public float windMixInfluence;

	// Token: 0x04000FC5 RID: 4037
	public float maxVal = 1f;

	// Token: 0x04000FC6 RID: 4038
	public bool straightDown;

	// Token: 0x04000FC7 RID: 4039
	private float sinceSet = 10f;

	// Token: 0x04000FC8 RID: 4040
	public WindChillZone zone;

	// Token: 0x04000FC9 RID: 4041
	public bool useWindChillZoneIntensity;
}
