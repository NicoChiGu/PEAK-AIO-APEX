using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000336 RID: 822
public class SlipperyJellyfish : MonoBehaviour
{
	// Token: 0x06001537 RID: 5431 RVA: 0x0006C25D File Offset: 0x0006A45D
	private void Start()
	{
		this.relay = base.GetComponentInParent<TriggerRelay>();
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x0006C26B File Offset: 0x0006A46B
	private void Update()
	{
		this.counter += Time.deltaTime;
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x0006C280 File Offset: 0x0006A480
	public void OnTriggerEnter(Collider other)
	{
		if (this.counter < 3f)
		{
			return;
		}
		Character componentInParent = other.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		this.counter = 0f;
		this.relay.view.RPC("RPCA_TriggerWithTarget", 0, new object[]
		{
			base.transform.GetSiblingIndex(),
			Character.localCharacter.refs.view.ViewID
		});
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x0006C30C File Offset: 0x0006A50C
	public void Trigger(int targetID)
	{
		Character component = PhotonView.Find(targetID).GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		Rigidbody bodypartRig = component.GetBodypartRig(BodypartType.Foot_R);
		Rigidbody bodypartRig2 = component.GetBodypartRig(BodypartType.Foot_L);
		Rigidbody bodypartRig3 = component.GetBodypartRig(BodypartType.Hip);
		Rigidbody bodypartRig4 = component.GetBodypartRig(BodypartType.Head);
		component.RPCA_Fall(2f);
		bodypartRig.AddForce((component.data.lookDirection_Flat + Vector3.up) * 200f, 1);
		bodypartRig2.AddForce((component.data.lookDirection_Flat + Vector3.up) * 200f, 1);
		bodypartRig3.AddForce(Vector3.up * 1500f, 1);
		bodypartRig4.AddForce(component.data.lookDirection_Flat * -300f, 1);
		component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.05f, true, true, true);
		for (int i = 0; i < this.slipSFX.Length; i++)
		{
			this.slipSFX[i].Play(base.transform.position);
		}
	}

	// Token: 0x040013BB RID: 5051
	private float counter = 2.5f;

	// Token: 0x040013BC RID: 5052
	private TriggerRelay relay;

	// Token: 0x040013BD RID: 5053
	public SFX_Instance[] slipSFX;
}
