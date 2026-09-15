using System;
using UnityEngine;

// Token: 0x02000132 RID: 306
public class BakedVolumeLight : MonoBehaviour
{
	// Token: 0x06000994 RID: 2452 RVA: 0x00032D54 File Offset: 0x00030F54
	public float GetRadius()
	{
		if (this.scaleWithLossyScale > 0f)
		{
			float num = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			return Mathf.Lerp(this.radius, this.radius * num, this.scaleWithLossyScale);
		}
		return this.radius;
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x00032DD4 File Offset: 0x00030FD4
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = this.color;
		float num = this.GetRadius();
		BakedVolumeLight.LightModes lightModes = this.mode;
		if (lightModes == BakedVolumeLight.LightModes.Point)
		{
			Gizmos.DrawWireSphere(base.transform.position, num);
			return;
		}
		if (lightModes != BakedVolumeLight.LightModes.Spot)
		{
			return;
		}
		Vector3 vector = base.transform.position + base.transform.forward * num;
		Gizmos.DrawLine(base.transform.position, vector);
		float num2 = this.coneSize * 0.034906585f;
		Vector3[] array = new Vector3[]
		{
			vector + base.transform.up * num2 * num,
			vector + base.transform.right * num2 * num,
			vector + -base.transform.up * num2 * num,
			vector + -base.transform.right * num2 * num
		};
		foreach (Vector3 vector2 in array)
		{
			Gizmos.DrawLine(base.transform.position, vector2);
		}
		Gizmos.DrawLineStrip(array, true);
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x00032F37 File Offset: 0x00031137
	public void Rebake()
	{
		Object.FindAnyObjectByType<LightVolume>().Bake(null);
	}

	// Token: 0x0400090B RID: 2315
	public BakedVolumeLight.LightModes mode;

	// Token: 0x0400090C RID: 2316
	public Color color = Color.white;

	// Token: 0x0400090D RID: 2317
	public float intensity = 1f;

	// Token: 0x0400090E RID: 2318
	public float radius = 10f;

	// Token: 0x0400090F RID: 2319
	[Range(0f, 1f)]
	public float falloff = 0.5f;

	// Token: 0x04000910 RID: 2320
	[Range(0f, 1f)]
	[Tooltip("Percentage width at which the light should be full brightness. 1.0 means the entire cone is full bright, 0.0 means that the fade lerp starts immediately in the center")]
	public float coneFalloff = 0.9f;

	// Token: 0x04000911 RID: 2321
	[Range(0f, 90f)]
	public float coneSize = 30f;

	// Token: 0x04000912 RID: 2322
	[Range(0f, 1f)]
	public float scaleWithLossyScale;

	// Token: 0x02000457 RID: 1111
	public enum LightModes
	{
		// Token: 0x040018AA RID: 6314
		Point,
		// Token: 0x040018AB RID: 6315
		Spot
	}
}
