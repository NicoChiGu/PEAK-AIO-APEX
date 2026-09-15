using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class LightCuller : MonoBehaviour
{
	// Token: 0x0600122F RID: 4655 RVA: 0x0005C2B5 File Offset: 0x0005A4B5
	private void Start()
	{
		this.lightToCull = base.GetComponent<Light>();
		this.defaultRange = this.lightToCull.range;
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x0005C2D4 File Offset: 0x0005A4D4
	private void OnEnable()
	{
		base.StartCoroutine(this.LightCullRoutine());
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x0005C2E3 File Offset: 0x0005A4E3
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.cullDistance);
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x0005C305 File Offset: 0x0005A505
	private IEnumerator LightCullRoutine()
	{
		yield return new WaitForSeconds(Random.value);
		for (;;)
		{
			if (Character.localCharacter)
			{
				bool shouldEnable = Vector3.Distance(MainCamera.instance.transform.position, base.transform.position) < this.cullDistance;
				if (!this.lightToCull.enabled && shouldEnable)
				{
					this.lightToCull.enabled = true;
					float t = 0f;
					while (t < 1f)
					{
						t += Time.deltaTime;
						this.lightToCull.range = this.defaultRange * t;
						yield return null;
					}
				}
				if (this.lightToCull.enabled && !shouldEnable)
				{
					float t = 0f;
					while (t < 1f)
					{
						t += Time.deltaTime;
						this.lightToCull.range = this.defaultRange * (1f - t);
						yield return null;
					}
					this.lightToCull.enabled = false;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x040010AD RID: 4269
	private Light lightToCull;

	// Token: 0x040010AE RID: 4270
	public float cullDistance = 50f;

	// Token: 0x040010AF RID: 4271
	public float defaultRange;
}
