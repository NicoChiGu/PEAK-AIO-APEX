using System;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class CompassPointer : MonoBehaviour
{
	// Token: 0x0600089C RID: 2204 RVA: 0x0002EEA4 File Offset: 0x0002D0A4
	private void Awake()
	{
		this.item = base.GetComponentInParent<Item>();
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0002EEB2 File Offset: 0x0002D0B2
	private void Update()
	{
		this.UpdateHeading();
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0002EEBC File Offset: 0x0002D0BC
	protected void UpdateHeading()
	{
		bool flag = true;
		switch (this.compassType)
		{
		case CompassPointer.CompassType.Normal:
			this.heading = Vector3.forward;
			break;
		case CompassPointer.CompassType.Warp:
			flag = false;
			this.needle.RotateAround(this.needle.transform.position, this.needle.right, this.warpSpeed * Time.deltaTime * this.speedMultiplier);
			break;
		case CompassPointer.CompassType.Pirate:
			this.UpdateHeadingPirate();
			break;
		}
		if (flag)
		{
			this.heading = Vector3.ProjectOnPlane(this.heading, base.transform.forward);
			this.needle.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(this.needle.transform.forward, this.heading, 10f * Time.deltaTime), base.transform.up);
		}
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x0002EF9C File Offset: 0x0002D19C
	protected void UpdateHeadingPirate()
	{
		if (Luggage.ALL_LUGGAGE.Count == 0)
		{
			this.heading = Quaternion.Euler(0f, Time.time * this.warpSpeed, 0f) * Vector3.forward;
		}
		if (this.item.inActiveList)
		{
			float num = float.MaxValue;
			foreach (Luggage luggage in Luggage.ALL_LUGGAGE)
			{
				if (Vector3.Distance(luggage.Center(), base.transform.position) < num)
				{
					num = Vector3.Distance(luggage.Center(), base.transform.position);
					this.currentLuggageVector = luggage.Center() - base.transform.position;
				}
			}
			this.heading = this.currentLuggageVector;
		}
	}

	// Token: 0x04000843 RID: 2115
	public CompassPointer.CompassType compassType;

	// Token: 0x04000844 RID: 2116
	public Transform needle;

	// Token: 0x04000845 RID: 2117
	public float warpSpeed = 2f;

	// Token: 0x04000846 RID: 2118
	public float speedMultiplier = 1f;

	// Token: 0x04000847 RID: 2119
	private Item item;

	// Token: 0x04000848 RID: 2120
	protected Vector3 heading;

	// Token: 0x04000849 RID: 2121
	private Vector3 currentLuggageVector = Vector3.zero;

	// Token: 0x0200044E RID: 1102
	public enum CompassType
	{
		// Token: 0x0400188C RID: 6284
		Normal,
		// Token: 0x0400188D RID: 6285
		Warp,
		// Token: 0x0400188E RID: 6286
		Pirate
	}
}
