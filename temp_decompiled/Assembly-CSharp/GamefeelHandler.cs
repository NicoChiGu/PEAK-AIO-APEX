using System;
using UnityEngine;

// Token: 0x02000267 RID: 615
public class GamefeelHandler : MonoBehaviour
{
	// Token: 0x06001176 RID: 4470 RVA: 0x00057E61 File Offset: 0x00056061
	private void Awake()
	{
		GamefeelHandler.instance = this;
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x00057E69 File Offset: 0x00056069
	private void Start()
	{
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>();
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x00057E80 File Offset: 0x00056080
	public Vector3 GetRotation()
	{
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			vector += base.transform.GetChild(i).localEulerAngles;
		}
		return vector;
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x00057EC2 File Offset: 0x000560C2
	public void AddRotationShake_Local_Stiff(Vector3 force)
	{
		this.stiff.AddForce(force);
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x00057ED0 File Offset: 0x000560D0
	public void AddRotationShake_Local_Loose(Vector3 force)
	{
		this.loose.AddForce(force);
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x00057EDE File Offset: 0x000560DE
	public void AddPerlinShake(float amount = 1f, float duration = 0.2f, float scale = 15f)
	{
		if (this.setting.Value == 1)
		{
			amount *= 0.5f;
			amount = Mathf.Min(amount, 3f);
		}
		this.perlin.AddShake(amount, duration, scale);
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x00057F14 File Offset: 0x00056114
	public void AddPerlinShakeProximity(Vector3 position, float amount = 1f, float duration = 0.2f, float scale = 15f, float maxProximity = 10f)
	{
		if (this.setting.Value == 1)
		{
			amount *= 0.5f;
			amount = Mathf.Min(amount, 3f);
		}
		float num = 1f;
		if (Character.observedCharacter)
		{
			num = 1f - Mathf.Clamp01(Vector3.Distance(Character.observedCharacter.Center, position) / maxProximity);
		}
		this.perlin.AddShake(amount * num, duration, scale);
	}

	// Token: 0x04000FF7 RID: 4087
	public static GamefeelHandler instance;

	// Token: 0x04000FF8 RID: 4088
	public PhotosensitiveSetting setting;

	// Token: 0x04000FF9 RID: 4089
	public RotationSpring stiff;

	// Token: 0x04000FFA RID: 4090
	public RotationSpring loose;

	// Token: 0x04000FFB RID: 4091
	public PerlinShake perlin;
}
