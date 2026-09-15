using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000240 RID: 576
public class DayNightManager : MonoBehaviour
{
	// Token: 0x060010DB RID: 4315 RVA: 0x00054E30 File Offset: 0x00053030
	public string getShaderValue(DayNightManager.ShaderParams parameter)
	{
		switch (parameter)
		{
		case DayNightManager.ShaderParams.AirDistortion:
			return "_GlobalHazeAmount";
		case DayNightManager.ShaderParams.RimFresnel:
			return "RimFresnelIntensity";
		case DayNightManager.ShaderParams.LavaAlpha:
			return "LavaAlpha";
		case DayNightManager.ShaderParams.BandFog:
			return "BandFogAmount";
		case DayNightManager.ShaderParams.SunSize:
			return "SunSizeMult";
		default:
			return "";
		}
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x00054E7C File Offset: 0x0005307C
	public void clearAllShaderParams()
	{
		foreach (object obj in Enum.GetValues(typeof(DayNightManager.ShaderParams)))
		{
			DayNightManager.ShaderParams shaderParams = (DayNightManager.ShaderParams)obj;
			Shader.SetGlobalFloat(this.getShaderValue(shaderParams), 0f);
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x060010DD RID: 4317 RVA: 0x00054EE8 File Offset: 0x000530E8
	public float timeOfDayNormalized
	{
		get
		{
			return this.timeOfDay % 24f / 24f;
		}
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x060010DE RID: 4318 RVA: 0x00054EFC File Offset: 0x000530FC
	public float isDay
	{
		get
		{
			return (float)((this.timeOfDay >= this.dayStart && this.timeOfDay < this.dayEnd) ? 1 : 0);
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x060010DF RID: 4319 RVA: 0x00054F20 File Offset: 0x00053120
	public float dayNightBlend
	{
		get
		{
			return Mathf.Lerp(this.currentProfile.sunGradient.Evaluate(this.timeOfDayNormalized).a, this.newProfile.sunGradient.Evaluate(this.timeOfDayNormalized).a, this.profileBlend);
		}
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x00054F6E File Offset: 0x0005316E
	private void Awake()
	{
		DayNightManager.instance = this;
		this.newProfile = this.currentProfile;
		this.clearAllShaderParams();
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x00054F88 File Offset: 0x00053188
	public void BlendProfiles(DayNightProfile profile)
	{
		DayNightManager.<>c__DisplayClass47_0 CS$<>8__locals1 = new DayNightManager.<>c__DisplayClass47_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.profile = profile;
		base.StartCoroutine(CS$<>8__locals1.<BlendProfiles>g__blendRoutine|0());
		List<string> list = new List<string>();
		if (this.newProfile.globalShaderFloats != null)
		{
			for (int i = 0; i < this.newProfile.globalShaderFloats.Length; i++)
			{
				this.allShaderFloats.Add(this.getShaderValue(this.newProfile.globalShaderFloats[i].parameter));
				list.Add(this.getShaderValue(this.newProfile.globalShaderFloats[i].parameter));
			}
		}
		if (this.newProfile.animatedGlobalShaderFloats != null)
		{
			for (int j = 0; j < this.newProfile.animatedGlobalShaderFloats.Length; j++)
			{
				this.allShaderFloats.Add(this.getShaderValue(this.newProfile.animatedGlobalShaderFloats[j].parameter));
				list.Add(this.getShaderValue(this.newProfile.animatedGlobalShaderFloats[j].parameter));
			}
		}
		List<string> list2 = new List<string>();
		for (int k = 0; k < this.lastShaderFloats.Count; k++)
		{
			if (!list.Contains(this.lastShaderFloats[k]))
			{
				list2.Add(this.lastShaderFloats[k]);
			}
		}
		this.lastShaderFloats = list2;
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x000550E0 File Offset: 0x000532E0
	private void Start()
	{
		this.timeOfDay = this.startingTimeOfDay;
		this.UpdateCycle();
		this.photonView = base.GetComponent<PhotonView>();
		float num = (this.dayEnd - this.dayStart) / 24f;
		float num2 = 1f - num;
		this.dayNightRatio = num / num2;
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x00055130 File Offset: 0x00053330
	public void setTimeOfDay(float timeToSet)
	{
		if (timeToSet > 48f)
		{
			this.timeOfDay = 48f;
		}
		this.timeOfDay = timeToSet;
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x0005514C File Offset: 0x0005334C
	private void Update()
	{
		this.HazeDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.AirDistortion));
		this.RimFresnelDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.RimFresnel));
		this.LavaAlphaDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.LavaAlpha));
		this.BandFogDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.BandFog));
		this.SunSizeDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.SunSize));
		this.timeOfDay += 1f / (this.dayLengthInMinutes * 60f) * Time.deltaTime * 24f;
		if (this.timeOfDay > 24f)
		{
			this.timeOfDay -= 24f;
			this.passedMidnight = true;
		}
		if (this.passedMidnight && this.timeOfDay >= 5.5f)
		{
			this.dayCount++;
			this.passedMidnight = false;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.syncTimer += Time.deltaTime;
			if (this.syncTimer > 5f)
			{
				this.photonView.RPC("RPCA_SyncTime", 0, new object[] { this.timeOfDay });
				this.syncTimer = 0f;
			}
		}
		this.UpdateCycle();
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x0005528A File Offset: 0x0005348A
	public string DayCountString()
	{
		return LocalizedText.GetText("DAY", true).Replace("#", DayNightManager.IntToNumberWord(DayNightManager.instance.dayCount) ?? "");
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x000552BC File Offset: 0x000534BC
	public string TimeOfDayString()
	{
		if (this.timeOfDay >= 23.5f)
		{
			return "night";
		}
		if (this.timeOfDay >= 17.5f)
		{
			return "evening";
		}
		if (this.timeOfDay >= 11.5f)
		{
			return "afternoon";
		}
		if (this.timeOfDay >= 5.5f)
		{
			return "morning";
		}
		return "night";
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x0005531C File Offset: 0x0005351C
	private static string IntToNumberWord(int x)
	{
		if (x == 1)
		{
			return LocalizedText.GetText("One", true);
		}
		if (x == 2)
		{
			return LocalizedText.GetText("Two", true);
		}
		if (x == 3)
		{
			return LocalizedText.GetText("Three", true);
		}
		if (x == 4)
		{
			return LocalizedText.GetText("Four", true);
		}
		if (x == 5)
		{
			return LocalizedText.GetText("Five", true);
		}
		if (x == 6)
		{
			return LocalizedText.GetText("Six", true);
		}
		if (x == 7)
		{
			return LocalizedText.GetText("Seven", true);
		}
		if (x == 8)
		{
			return LocalizedText.GetText("Eight", true);
		}
		if (x == 9)
		{
			return LocalizedText.GetText("Nine", true);
		}
		if (x == 10)
		{
			return LocalizedText.GetText("Ten", true);
		}
		return x.ToString() ?? "";
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x000553DC File Offset: 0x000535DC
	public string FloatToTimeString(float time)
	{
		time = Mathf.Clamp(time, 0f, 24f);
		int num = Mathf.FloorToInt(time);
		int num2 = Mathf.FloorToInt((time - (float)num) * 60f);
		string text = ((num < 12) ? "AM" : "PM");
		int num3 = ((num % 12 == 0) ? 12 : (num % 12));
		return string.Format("{0:D2}:{1:D2} {2}", num3, num2, text);
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x0005544A File Offset: 0x0005364A
	[PunRPC]
	public void RPCA_SyncTime(float time)
	{
		this.timeOfDay = time;
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x00055453 File Offset: 0x00053653
	private void OnValidate()
	{
		this.UpdateCycle();
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x0005545C File Offset: 0x0005365C
	public void UpdateCycle()
	{
		this.timeString = this.FloatToTimeString(this.timeOfDay);
		float timeOfDayNormalized = this.timeOfDayNormalized;
		Vector3 vector = this.highNoonRotation + new Vector3(0f, 0f, this.angleOffsetZ.Evaluate(timeOfDayNormalized));
		float num = timeOfDayNormalized;
		if (this.isDay < 0.5f)
		{
			if (num > this.dayEnd / 24f)
			{
				num = this.dayEnd / 24f - (num - this.dayEnd / 24f) * this.dayNightRatio;
			}
			else if (num < this.dayStart / 24f)
			{
				num = this.dayStart / 24f + (this.dayStart / 24f - num) * this.dayNightRatio;
			}
		}
		Vector3 vector2;
		vector2..ctor((num * this.rotDir - 0.5f) * 360f, 0f, 0f);
		this.earth.transform.rotation = Quaternion.Euler(vector) * Quaternion.Euler(vector2);
		Color color = Color.Lerp(Color.Lerp(this.currentProfile.sunGradient.Evaluate(timeOfDayNormalized), this.newProfile.sunGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialSunColor, this.specialDaySunBlend);
		Color color2 = Color.Lerp(Color.Lerp(this.currentProfile.skyTopGradient.Evaluate(timeOfDayNormalized), this.newProfile.skyTopGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialTopColor, this.specialDaySkyBlend);
		Color color3 = Color.Lerp(Color.Lerp(this.currentProfile.skyMidGradient.Evaluate(timeOfDayNormalized), this.newProfile.skyMidGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialMidColor, this.specialDaySkyBlend);
		Color color4 = Color.Lerp(Color.Lerp(this.currentProfile.skyBottomGradient.Evaluate(timeOfDayNormalized), this.newProfile.skyBottomGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialBottomColor, this.specialDaySkyBlend);
		Shader.SetGlobalColor(DayNightManager.SkyTopColor, color2);
		Shader.SetGlobalColor(DayNightManager.SkyMidColor, color3);
		Shader.SetGlobalColor(DayNightManager.SkyBottomColor, color4);
		Shader.SetGlobalFloat(DayNightManager.TIMEOFDAY, timeOfDayNormalized);
		Shader.SetGlobalFloat(DayNightManager.Name, this.isDay);
		Shader.SetGlobalFloat(DayNightManager.FOG, Mathf.Lerp(this.currentProfile.fogGradient.Evaluate(timeOfDayNormalized).r, this.newProfile.fogGradient.Evaluate(timeOfDayNormalized).r, this.profileBlend));
		this.sun.color = color;
		this.moon.color = color;
		if (!this.isBlending && this.currentProfile.animatedGlobalShaderFloats != null)
		{
			for (int i = 0; i < this.currentProfile.animatedGlobalShaderFloats.Length; i++)
			{
				Shader.SetGlobalFloat(this.getShaderValue(this.currentProfile.animatedGlobalShaderFloats[i].parameter), this.currentProfile.animatedGlobalShaderFloats[i].paramValue.Evaluate(timeOfDayNormalized));
			}
		}
		float num2 = -(this.snowstormWindFactor * 1.75f + this.rainstormWindFactor * 1.25f);
		float num3 = Mathf.Lerp(this.currentProfile.sunIntensity, this.newProfile.sunIntensity, this.profileBlend);
		this.sun.intensity = Mathf.Max(0.015f, (color.a * 2f - 1f) * 0.5f * num3 + num2);
		float num4 = Mathf.Lerp(this.currentProfile.sunIntensity, this.newProfile.sunIntensity, this.profileBlend);
		this.moon.intensity = Mathf.Max(0.015f, (1f - color.a * 2f) * 0.5f * num4 + num2);
		this.lensFlare.intensity = (color.a - 0.5f) * 2f + num2;
		if (this.specialDaySunBlend < 0.5f)
		{
			if (color.a < 0.5f)
			{
				this.sun.enabled = false;
				this.moon.enabled = true;
				Shader.SetGlobalInt(DayNightManager.IsDayReal, 0);
			}
			else
			{
				this.moon.enabled = false;
				this.sun.enabled = true;
				Shader.SetGlobalInt(DayNightManager.IsDayReal, 1);
			}
		}
		else
		{
			this.moon.enabled = false;
			this.sun.enabled = false;
		}
		this.sun.shadowStrength = math.saturate(num3);
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x000558E0 File Offset: 0x00053AE0
	private void OnDisable()
	{
		for (int i = 0; i < this.allShaderFloats.Count; i++)
		{
			Shader.SetGlobalFloat(this.allShaderFloats[i], 0f);
		}
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x00055919 File Offset: 0x00053B19
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(this.earth.transform.position, this.sun.transform.position);
	}

	// Token: 0x04000F3D RID: 3901
	public static DayNightManager instance;

	// Token: 0x04000F3E RID: 3902
	private static readonly int TIMEOFDAY = Shader.PropertyToID("_TimeOfDay");

	// Token: 0x04000F3F RID: 3903
	private static readonly int FOG = Shader.PropertyToID("EXTRAFOG");

	// Token: 0x04000F40 RID: 3904
	private static readonly int Name = Shader.PropertyToID("IsDay");

	// Token: 0x04000F41 RID: 3905
	private static readonly int IsDayReal = Shader.PropertyToID("IsDayReal");

	// Token: 0x04000F42 RID: 3906
	private static readonly int SkyTopColor = Shader.PropertyToID("SkyTopColor");

	// Token: 0x04000F43 RID: 3907
	private static readonly int SkyMidColor = Shader.PropertyToID("SkyMidColor");

	// Token: 0x04000F44 RID: 3908
	private static readonly int SkyBottomColor = Shader.PropertyToID("SkyBottomColor");

	// Token: 0x04000F45 RID: 3909
	[Range(0f, 48f)]
	public float timeOfDay;

	// Token: 0x04000F46 RID: 3910
	public float dayLengthInMinutes = 10f;

	// Token: 0x04000F47 RID: 3911
	public float startingTimeOfDay = 9f;

	// Token: 0x04000F48 RID: 3912
	public float dayStart = 5f;

	// Token: 0x04000F49 RID: 3913
	public float dayEnd = 21f;

	// Token: 0x04000F4A RID: 3914
	public int dayCount = 1;

	// Token: 0x04000F4B RID: 3915
	public LensFlareComponentSRP lensFlare;

	// Token: 0x04000F4C RID: 3916
	public AnimationCurve angleOffsetZ;

	// Token: 0x04000F4D RID: 3917
	public Vector3 highNoonRotation;

	// Token: 0x04000F4E RID: 3918
	public Transform earth;

	// Token: 0x04000F4F RID: 3919
	public Light sun;

	// Token: 0x04000F50 RID: 3920
	public Light moon;

	// Token: 0x04000F51 RID: 3921
	public DayNightProfile currentProfile;

	// Token: 0x04000F52 RID: 3922
	public DayNightProfile newProfile;

	// Token: 0x04000F53 RID: 3923
	private float profileBlend;

	// Token: 0x04000F54 RID: 3924
	[Header("Special Day")]
	[Range(0f, 1f)]
	public float specialDaySunBlend;

	// Token: 0x04000F55 RID: 3925
	public Color specialSunColor;

	// Token: 0x04000F56 RID: 3926
	public float specialDaySkyBlend;

	// Token: 0x04000F57 RID: 3927
	public Color specialTopColor;

	// Token: 0x04000F58 RID: 3928
	public Color specialMidColor;

	// Token: 0x04000F59 RID: 3929
	public Color specialBottomColor;

	// Token: 0x04000F5A RID: 3930
	private List<string> lastShaderFloats = new List<string>();

	// Token: 0x04000F5B RID: 3931
	private List<string> allShaderFloats = new List<string>();

	// Token: 0x04000F5C RID: 3932
	private bool isBlending;

	// Token: 0x04000F5D RID: 3933
	public string timeString;

	// Token: 0x04000F5E RID: 3934
	public float rotDir = 1f;

	// Token: 0x04000F5F RID: 3935
	public float snowstormWindFactor;

	// Token: 0x04000F60 RID: 3936
	public float rainstormWindFactor;

	// Token: 0x04000F61 RID: 3937
	private PhotonView photonView;

	// Token: 0x04000F62 RID: 3938
	public float dayNightRatio = 2f;

	// Token: 0x04000F63 RID: 3939
	public float syncTimer;

	// Token: 0x04000F64 RID: 3940
	private bool passedMidnight;

	// Token: 0x04000F65 RID: 3941
	public float HazeDebug;

	// Token: 0x04000F66 RID: 3942
	public float RimFresnelDebug;

	// Token: 0x04000F67 RID: 3943
	public float LavaAlphaDebug;

	// Token: 0x04000F68 RID: 3944
	public float BandFogDebug;

	// Token: 0x04000F69 RID: 3945
	public float SunSizeDebug;

	// Token: 0x020004D4 RID: 1236
	public enum ShaderParams
	{
		// Token: 0x04001AA7 RID: 6823
		AirDistortion,
		// Token: 0x04001AA8 RID: 6824
		RimFresnel,
		// Token: 0x04001AA9 RID: 6825
		LavaAlpha,
		// Token: 0x04001AAA RID: 6826
		BandFog,
		// Token: 0x04001AAB RID: 6827
		SunSize
	}

	// Token: 0x020004D5 RID: 1237
	[Serializable]
	public class ShaderParameters
	{
		// Token: 0x04001AAC RID: 6828
		public string paramName;

		// Token: 0x04001AAD RID: 6829
		public string paramValue;
	}
}
