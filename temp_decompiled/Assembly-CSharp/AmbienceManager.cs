using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
[ExecuteInEditMode]
public class AmbienceManager : MonoBehaviour
{
	// Token: 0x06000451 RID: 1105 RVA: 0x0001A89F File Offset: 0x00018A9F
	private void Awake()
	{
		AmbienceManager.instance = this;
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x0001A8A7 File Offset: 0x00018AA7
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.Start();
		}
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x0001A8B6 File Offset: 0x00018AB6
	private void UpdateFog()
	{
		if (Application.isPlaying)
		{
			this.useFog = true;
		}
		Shader.SetGlobalFloat(AmbienceManager.Usefog, (float)(this.useFog ? 1 : 0));
		Shader.SetGlobalFloat(AmbienceManager.Maxfog, this.maxFog);
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x0001A8ED File Offset: 0x00018AED
	private void Start()
	{
		this.UpdateFog();
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x0001A8F8 File Offset: 0x00018AF8
	private void Update()
	{
		this.UpdateFog();
		RenderSettings.skybox = this.skyboxMaterial;
		RenderSettings.fogColor = this.fogColor;
		if (!this.dayNight)
		{
			return;
		}
		Color color = this.ambienceGradient.Evaluate(this.dayNight.timeOfDayNormalized);
		RenderSettings.ambientLight = color * this.brightness * color.a;
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x0001A962 File Offset: 0x00018B62
	public void ToggleFog()
	{
		this.useFog = !this.useFog;
		Shader.SetGlobalFloat(AmbienceManager.Usefog, (float)(this.useFog ? 1 : 0));
	}

	// Token: 0x040004CB RID: 1227
	private static readonly int Maxfog = Shader.PropertyToID("MAXFOG");

	// Token: 0x040004CC RID: 1228
	private static readonly int Usefog = Shader.PropertyToID("USEFOG");

	// Token: 0x040004CD RID: 1229
	public Color ambienceColor;

	// Token: 0x040004CE RID: 1230
	public Gradient ambienceGradient;

	// Token: 0x040004CF RID: 1231
	public Color fogColor;

	// Token: 0x040004D0 RID: 1232
	public float brightness = 1f;

	// Token: 0x040004D1 RID: 1233
	public Material skyboxMaterial;

	// Token: 0x040004D2 RID: 1234
	public DayNightManager dayNight;

	// Token: 0x040004D3 RID: 1235
	public float maxFog = 500f;

	// Token: 0x040004D4 RID: 1236
	public bool useFog = true;

	// Token: 0x040004D5 RID: 1237
	public static AmbienceManager instance;
}
