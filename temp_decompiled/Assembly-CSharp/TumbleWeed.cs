using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200035D RID: 861
public class TumbleWeed : MonoBehaviour
{
	// Token: 0x06001603 RID: 5635 RVA: 0x0007179C File Offset: 0x0006F99C
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.rig = base.GetComponent<Rigidbody>();
		this.maxAngle = Mathf.Lerp(50f, 180f, Mathf.Pow(Random.value, 5f));
		this.rollForce *= Mathf.Lerp(0.5f, 1f, Mathf.Pow(Random.value, 2f));
		this.originalScale = base.transform.localScale.x;
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x00071828 File Offset: 0x0006FA28
	private void FixedUpdate()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		Vector3 vector = -Vector3.right;
		Character target = this.GetTarget();
		if (target)
		{
			vector = (target.Center - base.transform.position).normalized;
		}
		this.rig.AddForce(vector * this.rollForce, 5);
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x00071894 File Offset: 0x0006FA94
	private Character GetTarget()
	{
		float num = 300f;
		Character character = null;
		foreach (Character character2 in Character.AllCharacters)
		{
			if (Vector3.Angle(-Vector3.right, character2.Center - base.transform.position) <= this.maxAngle)
			{
				float num2 = Vector3.Distance(character2.Center, base.transform.position);
				if (num2 < num)
				{
					num = num2;
					character = character2;
				}
			}
		}
		return character;
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x00071938 File Offset: 0x0006FB38
	public void OnCollisionEnter(Collision collision)
	{
		Character componentInParent = collision.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		if (this.ignored.Contains(componentInParent))
		{
			return;
		}
		base.StartCoroutine(this.IgnoreTarget(componentInParent));
		float num = base.transform.localScale.x / this.originalScale;
		if (this.originalScale == 0f)
		{
			num = 1f;
		}
		num = Mathf.Clamp01(num);
		float num2 = Mathf.Clamp01(this.rig.linearVelocity.magnitude * num * this.powerMultiplier);
		if (this.testFullPower)
		{
			num2 = 1f;
		}
		if (num2 < 0.2f)
		{
			return;
		}
		componentInParent.Fall(2f * num2, 0f);
		componentInParent.AddForceAtPosition(this.rig.linearVelocity.normalized * this.collisionForce * num2, collision.contacts[0].point, 2f);
		componentInParent.refs.afflictions.AddThorn(collision.contacts[0].point);
		if (num2 > 0.6f)
		{
			componentInParent.refs.afflictions.AddThorn(collision.contacts[0].point);
		}
	}

	// Token: 0x06001607 RID: 5639 RVA: 0x00071A87 File Offset: 0x0006FC87
	public IEnumerator IgnoreTarget(Character target)
	{
		this.ignored.Add(target);
		yield return new WaitForSeconds(1f);
		this.ignored.Remove(target);
		yield break;
	}

	// Token: 0x040014E1 RID: 5345
	private Rigidbody rig;

	// Token: 0x040014E2 RID: 5346
	public float rollForce;

	// Token: 0x040014E3 RID: 5347
	public float collisionForce;

	// Token: 0x040014E4 RID: 5348
	private float maxAngle;

	// Token: 0x040014E5 RID: 5349
	private PhotonView photonView;

	// Token: 0x040014E6 RID: 5350
	private float originalScale = 1f;

	// Token: 0x040014E7 RID: 5351
	public float powerMultiplier = 0.035f;

	// Token: 0x040014E8 RID: 5352
	public bool testFullPower;

	// Token: 0x040014E9 RID: 5353
	private List<Character> ignored = new List<Character>();
}
