using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000B5 RID: 181
public class EventOnItemCollision : ItemComponent
{
	// Token: 0x060006A6 RID: 1702 RVA: 0x000261A6 File Offset: 0x000243A6
	private new void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000261B4 File Offset: 0x000243B4
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x000261B8 File Offset: 0x000243B8
	private void OnCollisionEnter(Collision collision)
	{
		if (this.onlyOnce && this.triggered)
		{
			return;
		}
		if (this.onlyWhenImKinematic && this.rb != null && !this.rb.isKinematic)
		{
			return;
		}
		Item componentInParent = collision.gameObject.GetComponentInParent<Item>();
		if (componentInParent == null || componentInParent.itemState != ItemState.Ground)
		{
			return;
		}
		Debug.Log(string.Format("{0} collided with {1} at velocity {2}", base.gameObject.name, componentInParent.gameObject.name, collision.relativeVelocity.magnitude));
		if (collision.relativeVelocity.magnitude > this.minCollisionVelocity)
		{
			this.TriggerEvent();
		}
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x0002626C File Offset: 0x0002446C
	internal void TriggerEvent()
	{
		if (this.onlyOnce && this.triggered)
		{
			return;
		}
		this.triggered = true;
		UnityEvent unityEvent = this.eventOnCollided;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		if (this.bonkSFX != null)
		{
			SFX_Player.instance.PlaySFX(this.bonkSFX, base.transform.position, null, null, 1f, false);
		}
	}

	// Token: 0x040006C3 RID: 1731
	public bool onlyWhenImKinematic;

	// Token: 0x040006C4 RID: 1732
	public UnityEvent eventOnCollided;

	// Token: 0x040006C5 RID: 1733
	private Rigidbody rb;

	// Token: 0x040006C6 RID: 1734
	public float minCollisionVelocity;

	// Token: 0x040006C7 RID: 1735
	public bool onlyOnce;

	// Token: 0x040006C8 RID: 1736
	public SFX_Instance bonkSFX;

	// Token: 0x040006C9 RID: 1737
	private bool triggered;
}
