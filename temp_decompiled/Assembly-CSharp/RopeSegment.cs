using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000174 RID: 372
public class RopeSegment : MonoBehaviour, IInteractible
{
	// Token: 0x06000BBE RID: 3006 RVA: 0x0003EDB0 File Offset: 0x0003CFB0
	private void Awake()
	{
		this.rope = base.GetComponentInParent<Rope>();
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x0003EDBE File Offset: 0x0003CFBE
	private void Update()
	{
		this.angle = this.GetAngle();
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x0003EDCC File Offset: 0x0003CFCC
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x0003EDD9 File Offset: 0x0003CFD9
	public string GetInteractionText()
	{
		return LocalizedText.GetText("GRAB", true);
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x0003EDE6 File Offset: 0x0003CFE6
	public string GetName()
	{
		return LocalizedText.GetText(this.displayName, true);
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x0003EDF4 File Offset: 0x0003CFF4
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x0003EDFC File Offset: 0x0003CFFC
	public void Interact(Character interactor)
	{
		interactor.refs.items.EquipSlot(Optionable<byte>.None);
		int num = base.transform.GetSiblingIndex() - 2;
		Debug.Log(string.Format("Grabbing Rope: {0} with index {1}", base.gameObject.name, num));
		interactor.GetComponent<PhotonView>().RPC("GrabRopeRpc", 0, new object[]
		{
			this.rope.GetComponentInParent<PhotonView>(),
			num
		});
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x0003EE7C File Offset: 0x0003D07C
	public bool IsInteractible(Character interactor)
	{
		float num = this.GetAngle();
		bool flag = num < interactor.refs.ropeHandling.maxRopeAngle * 0.6f || 180f - num < interactor.refs.ropeHandling.maxRopeAngle * 0.6f;
		flag = flag && this.rope.isClimbable;
		if (interactor.data.isRopeClimbing)
		{
			flag = flag && interactor.data.heldRope != this.rope;
		}
		return flag;
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x0003EF09 File Offset: 0x0003D109
	public float GetAngle()
	{
		return Vector3.Angle(Vector3.up, base.transform.up);
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x0003EF20 File Offset: 0x0003D120
	internal Vector3 GetClimbNormal(Vector3 charPos)
	{
		Vector3 vector = charPos - base.transform.position;
		vector = Vector3.ProjectOnPlane(vector, base.transform.up);
		return base.transform.InverseTransformDirection(vector);
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x0003EF60 File Offset: 0x0003D160
	internal void Tie(Vector3 tiePos)
	{
		RopeSegment.<>c__DisplayClass13_0 CS$<>8__locals1 = new RopeSegment.<>c__DisplayClass13_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.joint = base.gameObject.AddComponent<ConfigurableJoint>();
		CS$<>8__locals1.joint.xMotion = 0;
		CS$<>8__locals1.joint.yMotion = 0;
		CS$<>8__locals1.joint.zMotion = 0;
		CS$<>8__locals1.joint.anchor = Vector3.zero;
		base.StartCoroutine(CS$<>8__locals1.<Tie>g__ITighten|0(tiePos));
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x0003EFCD File Offset: 0x0003D1CD
	public void HoverEnter()
	{
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x0003EFCF File Offset: 0x0003D1CF
	public void HoverExit()
	{
	}

	// Token: 0x04000AE7 RID: 2791
	public Rope rope;

	// Token: 0x04000AE8 RID: 2792
	public float angle;

	// Token: 0x04000AE9 RID: 2793
	public string displayName;
}
