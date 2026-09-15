using System;
using UnityEngine;

// Token: 0x0200026D RID: 621
[ExecuteAlways]
public class HotSun : MonoBehaviour
{
	// Token: 0x0600118E RID: 4494 RVA: 0x00058655 File Offset: 0x00056855
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x0005867C File Offset: 0x0005687C
	private void Start()
	{
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x00058680 File Offset: 0x00056880
	private void Update()
	{
		this.bounds.center = base.transform.position;
		this.bounds.size = base.transform.localScale;
		if (!Application.isPlaying)
		{
			return;
		}
		if (Character.localCharacter == null)
		{
			return;
		}
		if (!this.bounds.Contains(Character.localCharacter.Center))
		{
			return;
		}
		if (DayNightManager.instance.sun.intensity < 5f)
		{
			return;
		}
		Transform transform = DayNightManager.instance.sun.transform;
		RaycastHit raycastHit = HelperFunctions.LineCheck(Character.localCharacter.Center + transform.forward * -1000f, Character.localCharacter.Center, HelperFunctions.LayerType.AllPhysical, 0f, 1);
		bool flag = false;
		if (raycastHit.transform == null || raycastHit.transform.root == Character.localCharacter.transform.root)
		{
			flag = true;
		}
		if (flag)
		{
			this.counter += Time.deltaTime;
			if (this.counter > this.rate)
			{
				this.counter = 0f;
				Character.localCharacter.refs.afflictions.AddSunHeat(this.amount);
			}
		}
	}

	// Token: 0x0400100D RID: 4109
	public Bounds bounds;

	// Token: 0x0400100E RID: 4110
	public float rate = 0.5f;

	// Token: 0x0400100F RID: 4111
	public float amount = 0.05f;

	// Token: 0x04001010 RID: 4112
	private float counter;
}
