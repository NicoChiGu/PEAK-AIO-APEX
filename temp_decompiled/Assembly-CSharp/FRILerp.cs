using System;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class FRILerp : MonoBehaviour
{
	// Token: 0x06001168 RID: 4456 RVA: 0x00057CF5 File Offset: 0x00055EF5
	private void Start()
	{
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x00057CF7 File Offset: 0x00055EF7
	public static Vector3 Lerp(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * (useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime)));
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x00057D1D File Offset: 0x00055F1D
	public static Vector3 PLerp(Vector3 from, Vector3 target, float speed, float dt)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x00057D35 File Offset: 0x00055F35
	public static Quaternion PLerp(Quaternion from, Quaternion target, float speed, float dt)
	{
		return Quaternion.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x00057D4D File Offset: 0x00055F4D
	public static float PLerp(float from, float target, float speed, float dt)
	{
		return Mathf.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x00057D65 File Offset: 0x00055F65
	public static Vector3 LerpFixed(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * (useTimeScale ? Time.fixedDeltaTime : Time.unscaledDeltaTime)));
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x00057D8B File Offset: 0x00055F8B
	public static Vector3 LerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x00057DA7 File Offset: 0x00055FA7
	public static float Lerp(float from, float target, float speed, bool useTimeScale = true)
	{
		return Mathf.Lerp(from, target, 1f - Mathf.Exp(-speed * (useTimeScale ? Time.fixedDeltaTime : Time.unscaledDeltaTime)));
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x00057DCD File Offset: 0x00055FCD
	public static float LerpUnclamped(float from, float target, float speed)
	{
		return Mathf.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x00057DE9 File Offset: 0x00055FE9
	public static Vector3 Slerp(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.Slerp(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x00057E05 File Offset: 0x00056005
	public static Vector3 SlerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.SlerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x00057E21 File Offset: 0x00056021
	public static Quaternion Lerp(Quaternion from, Quaternion target, float speed)
	{
		return Quaternion.Lerp(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x00057E3D File Offset: 0x0005603D
	public static Quaternion LerpUnclamped(Quaternion from, Quaternion target, float speed)
	{
		return Quaternion.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}
}
