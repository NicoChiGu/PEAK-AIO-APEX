using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class Basketball : MonoBehaviour
{
	// Token: 0x06000F89 RID: 3977 RVA: 0x0004D288 File Offset: 0x0004B488
	public void OnCollisionEnter(Collision collision)
	{
		if (!collision.rigidbody && Mathf.Abs(Vector3.Dot(collision.contacts[0].normal, Vector3.up)) < 0.2f)
		{
			this.item.rig.linearVelocity = new Vector3(this.item.rig.linearVelocity.x * this.xzBounceLoss, this.item.rig.linearVelocity.y - this.yFall, this.item.rig.linearVelocity.z * this.xzBounceLoss);
		}
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x0004D338 File Offset: 0x0004B538
	public void Update()
	{
		if (this.item.itemState == ItemState.Held && this.item.holderCharacter.input.movementInput.magnitude > 0.5f && this.item.holderCharacter.data.isGrounded && !this.item.holderCharacter.refs.items.isChargingThrow)
		{
			this.dribbling = true;
			return;
		}
		this.dribbling = false;
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x0004D3B6 File Offset: 0x0004B5B6
	private void Start()
	{
		base.StartCoroutine(this.DribbleRoutine());
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x0004D3C5 File Offset: 0x0004B5C5
	private IEnumerator DribbleRoutine()
	{
		for (;;)
		{
			if (this.dribbling)
			{
				Vector3 endPosWorldSpace = Vector3.zero;
				RaycastHit raycastHit = HelperFunctions.LineCheck(this.basketballMesh.position, this.basketballMesh.position - Vector3.up * 100f, HelperFunctions.LayerType.AllPhysicalExceptCharacter, 0.1f, 1);
				if (raycastHit.collider != null)
				{
					endPosWorldSpace = raycastHit.point;
					bool playedSFX = false;
					float t = 0f;
					Vector3 avarageVelocity = this.item.holderCharacter.data.avarageVelocity;
					avarageVelocity.y = 0f;
					float dribSpeed = Mathf.Clamp(avarageVelocity.magnitude, 1f, 3f);
					while (t < 1f)
					{
						t += Time.deltaTime * dribSpeed;
						if (t > 0.5f && !playedSFX)
						{
							this.impact.Play(this.basketballMesh.position);
							playedSFX = true;
						}
						endPosWorldSpace = new Vector3(this.basketballMesh.parent.position.x, endPosWorldSpace.y, this.basketballMesh.parent.position.z);
						this.basketballMesh.position = Vector3.Lerp(this.basketballMesh.parent.TransformPoint(Vector3.zero), endPosWorldSpace + Vector3.up * this.dribbleFloorOffset, this.dribbleCurve.Evaluate(t));
						this.item.defaultPos = new Vector3(this.item.defaultPos.x, this.handsPositionCurve.Evaluate(t), this.item.defaultPos.z);
						yield return null;
					}
				}
				Vector3 avarageVelocity2 = this.item.holderCharacter.data.avarageVelocity;
				avarageVelocity2.y = 0f;
				yield return new WaitForSeconds((avarageVelocity2.magnitude > 3f) ? this.dribbleWaitSprint : this.dribbleWait);
				endPosWorldSpace = default(Vector3);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000DDE RID: 3550
	public Item item;

	// Token: 0x04000DDF RID: 3551
	public float xzBounceLoss = 0.5f;

	// Token: 0x04000DE0 RID: 3552
	public float yFall = 5f;

	// Token: 0x04000DE1 RID: 3553
	public Transform basketballMesh;

	// Token: 0x04000DE2 RID: 3554
	public SFX_Instance impact;

	// Token: 0x04000DE3 RID: 3555
	public float dribbleWait = 0.25f;

	// Token: 0x04000DE4 RID: 3556
	public float dribbleWaitSprint = 0.05f;

	// Token: 0x04000DE5 RID: 3557
	public float dribbleFloorOffset = 0.3f;

	// Token: 0x04000DE6 RID: 3558
	public AnimationCurve dribbleCurve;

	// Token: 0x04000DE7 RID: 3559
	public AnimationCurve handsPositionCurve;

	// Token: 0x04000DE8 RID: 3560
	private bool dribbling;
}
