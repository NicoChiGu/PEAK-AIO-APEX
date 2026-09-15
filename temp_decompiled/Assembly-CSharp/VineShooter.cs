using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000368 RID: 872
public class VineShooter : ItemComponent
{
	// Token: 0x06001631 RID: 5681 RVA: 0x00072C17 File Offset: 0x00070E17
	public override void Awake()
	{
		this.actionReduceUses = base.GetComponent<Action_ReduceUses>();
		base.Awake();
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Combine(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x00072C52 File Offset: 0x00070E52
	private void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Remove(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x00072C7C File Offset: 0x00070E7C
	public void Update()
	{
		RaycastHit raycastHit;
		this.item.overrideUsability = Optionable<bool>.Some(this.WillAttach(out raycastHit));
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x00072CA4 File Offset: 0x00070EA4
	private void OnPrimaryFinishedCast()
	{
		Debug.Log("VineShooter shoot");
		RaycastHit raycastHit;
		if (!this.WillAttach(out raycastHit))
		{
			return;
		}
		if (this.disableOnFire != null)
		{
			this.disableOnFire.SetActive(false);
		}
		JungleVine component = this.vinePrefab.GetComponent<JungleVine>();
		Vector2 vector;
		vector..ctor(component.minDown, component.maxDown);
		int num = 10;
		for (int i = 0; i < num; i++)
		{
			float num2 = -ExtDataTypes.PRndRange(Vector2.Lerp(vector, Vector2.zero, (float)i / ((float)num - 1f)));
			Transform transform = MainCamera.instance.transform;
			Vector3 vector2 = transform.position + transform.forward;
			Vector3 vector3 = transform.position - Vector3.up * 0.2f;
			RaycastHit raycastHit2;
			if (Physics.Raycast(vector2, Vector3.down, ref raycastHit2, 4f, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 0))
			{
				vector3 = raycastHit2.point + Vector3.up * 1.5f;
			}
			Debug.Log(string.Format("from: {0}, to: {1}, hang: {2}", vector3, raycastHit.point, num2));
			Vector3 vector4;
			if (JungleVine.CheckVinePath(vector3, raycastHit.point, num2, out vector4, 0f))
			{
				PhotonNetwork.Instantiate(this.vinePrefab.name, vector3, Quaternion.identity, 0, null).GetComponent<JungleVine>().photonView.RPC("ForceBuildVine_RPC", 3, new object[] { vector3, raycastHit.point, num2, vector4 });
				this.actionReduceUses.RunAction();
				Debug.DrawLine(vector3, raycastHit.point, Color.green, 5f);
				GameUtils.instance.IncrementPermanentItemsPlaced();
				return;
			}
			Debug.DrawLine(vector3, raycastHit.point, Color.red, 5f);
		}
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x00072E9C File Offset: 0x0007109C
	public bool WillAttach(out RaycastHit hit)
	{
		hit = default(RaycastHit);
		if (!Character.localCharacter.data.isGrounded)
		{
			return false;
		}
		if (!Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, ref hit, this.maxLength, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 0))
		{
			return false;
		}
		float num = Vector3.Dot(Vector3.up, (hit.point - MainCamera.instance.transform.position).normalized);
		return num <= 0.6f && num >= -0.7f && Vector3.Dot(Vector3.up, (hit.point - MainCamera.instance.transform.position).normalized) <= 0.5f;
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x00072F72 File Offset: 0x00071172
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x0400151F RID: 5407
	public GameObject vinePrefab;

	// Token: 0x04001520 RID: 5408
	public GameObject disableOnFire;

	// Token: 0x04001521 RID: 5409
	public float maxLength = 40f;

	// Token: 0x04001522 RID: 5410
	private Action_ReduceUses actionReduceUses;
}
