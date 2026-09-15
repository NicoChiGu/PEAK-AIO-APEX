using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class BananaPeel : MonoBehaviour
{
	// Token: 0x06000F85 RID: 3973 RVA: 0x0004D06D File Offset: 0x0004B26D
	private void Start()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0004D07C File Offset: 0x0004B27C
	private void Update()
	{
		if (this.item.itemState != ItemState.Ground)
		{
			return;
		}
		this.counter += Time.deltaTime;
		if (this.counter < 3f)
		{
			return;
		}
		if (Vector3.Distance(Character.localCharacter.Center, base.transform.position) > 1f || !Character.localCharacter.data.isGrounded)
		{
			return;
		}
		if (Character.localCharacter.data.avarageVelocity.magnitude < 1.5f)
		{
			return;
		}
		this.counter = 0f;
		base.GetComponent<PhotonView>().RPC("RPCA_TriggerBanana", 0, new object[] { Character.localCharacter.refs.view.ViewID });
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x0004D144 File Offset: 0x0004B344
	[PunRPC]
	public void RPCA_TriggerBanana(int viewID)
	{
		Character component = PhotonView.Find(viewID).GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		base.GetComponent<Rigidbody>().AddForce((component.data.lookDirection_Flat * 0.5f + Vector3.up) * 40f, 1);
		Rigidbody bodypartRig = component.GetBodypartRig(BodypartType.Foot_R);
		Rigidbody bodypartRig2 = component.GetBodypartRig(BodypartType.Foot_L);
		Rigidbody bodypartRig3 = component.GetBodypartRig(BodypartType.Hip);
		Rigidbody bodypartRig4 = component.GetBodypartRig(BodypartType.Head);
		component.RPCA_Fall(2f);
		bodypartRig.AddForce((component.data.lookDirection_Flat + Vector3.up) * 200f, 1);
		bodypartRig2.AddForce((component.data.lookDirection_Flat + Vector3.up) * 200f, 1);
		bodypartRig3.AddForce(Vector3.up * 1500f, 1);
		bodypartRig4.AddForce(component.data.lookDirection_Flat * -300f, 1);
		for (int i = 0; i < this.slipSFX.Length; i++)
		{
			this.slipSFX[i].Play(base.transform.position);
		}
	}

	// Token: 0x04000DDB RID: 3547
	private float counter = 2.5f;

	// Token: 0x04000DDC RID: 3548
	private Item item;

	// Token: 0x04000DDD RID: 3549
	public SFX_Instance[] slipSFX;
}
