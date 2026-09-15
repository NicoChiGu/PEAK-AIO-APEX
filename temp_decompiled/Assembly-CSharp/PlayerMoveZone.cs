using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000B0 RID: 176
public class PlayerMoveZone : MonoBehaviour
{
	// Token: 0x06000674 RID: 1652 RVA: 0x00024F10 File Offset: 0x00023110
	private void Awake()
	{
		this.zoneBounds.center = base.transform.position;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00024F28 File Offset: 0x00023128
	private void Start()
	{
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00024F2A File Offset: 0x0002312A
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.characterInsideBounds = this.zoneBounds.Contains(Character.observedCharacter.Center);
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00024F55 File Offset: 0x00023155
	private void FixedUpdate()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.characterInsideBounds && Character.observedCharacter == Character.localCharacter)
		{
			this.AddForceToCharacter();
		}
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00024F84 File Offset: 0x00023184
	private void AddForceToCharacter()
	{
		Character.localCharacter.AddForce(this.forceDirection * this.Force, 0.5f, 1f);
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00024FAC File Offset: 0x000231AC
	private void OnDrawGizmosSelected()
	{
		this.zoneBounds.center = base.transform.position;
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		Gizmos.DrawCube(this.zoneBounds.center, this.zoneBounds.extents * 2f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, base.transform.position + this.forceDirection * this.Force);
	}

	// Token: 0x04000680 RID: 1664
	[FormerlySerializedAs("windZoneBounds")]
	public Bounds zoneBounds;

	// Token: 0x04000681 RID: 1665
	public Vector3 forceDirection;

	// Token: 0x04000682 RID: 1666
	[FormerlySerializedAs("windForce")]
	[SerializeField]
	private float Force;

	// Token: 0x04000683 RID: 1667
	public bool characterInsideBounds;
}
