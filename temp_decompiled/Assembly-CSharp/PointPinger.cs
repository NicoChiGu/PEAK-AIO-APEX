using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020002CA RID: 714
public class PointPinger : MonoBehaviour
{
	// Token: 0x1700012D RID: 301
	// (get) Token: 0x0600134B RID: 4939 RVA: 0x0006238E File Offset: 0x0006058E
	private bool inCooldown
	{
		get
		{
			return Time.time - this._timeLastPinged < this.coolDown;
		}
	}

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x0600134C RID: 4940 RVA: 0x000623A4 File Offset: 0x000605A4
	private bool shouldPing
	{
		get
		{
			return this.photonView.IsMine && this.character.input.pingWasPressed;
		}
	}

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x0600134D RID: 4941 RVA: 0x000623C5 File Offset: 0x000605C5
	private bool canPing
	{
		get
		{
			return this.character.data.fullyConscious && !this.inCooldown;
		}
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x000623E4 File Offset: 0x000605E4
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x000623FE File Offset: 0x000605FE
	private static bool TryGetPingHit(out RaycastHit hit)
	{
		return PExt.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), ref hit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), -1f);
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x00062420 File Offset: 0x00060620
	private void DoPing()
	{
		RaycastHit raycastHit;
		if (!PointPinger.TryGetPingHit(out raycastHit))
		{
			return;
		}
		this._timeLastPinged = Time.time;
		this.photonView.RPC("ReceivePoint_Rpc", 0, new object[] { raycastHit.point, raycastHit.normal });
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x00062477 File Offset: 0x00060677
	private void Update()
	{
		if (this.shouldPing && this.canPing)
		{
			this.DoPing();
		}
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x00062490 File Offset: 0x00060690
	[PunRPC]
	private void ReceivePoint_Rpc(Vector3 point, Vector3 hitNormal)
	{
		RaycastHit raycastHit;
		bool flag = PExt.LineCast(this.character.Head, Character.localCharacter.Head, ref raycastHit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), true);
		float num = Vector3.Distance(this.character.Head, Character.localCharacter.Head);
		PointPing component = this.pointPrefab.GetComponent<PointPing>();
		Vector2 visibilityFullNoneNoLos = component.visibilityFullNoneNoLos;
		float num2 = 1f - Mathf.InverseLerp(visibilityFullNoneNoLos.x, visibilityFullNoneNoLos.x + (visibilityFullNoneNoLos.y - visibilityFullNoneNoLos.x) * (flag ? component.NoLosVisibilityMul : 1f), num);
		if (num2 <= 0f)
		{
			return;
		}
		if (this.pingInstance != null)
		{
			Object.DestroyImmediate(this.pingInstance);
		}
		this.pingInstance = Object.Instantiate<GameObject>(this.pointPrefab, point, Quaternion.LookRotation((point - this.character.Head).normalized, Vector3.up));
		PointPing component2 = this.pingInstance.GetComponent<PointPing>();
		component2.hitNormal = hitNormal;
		component2.Init(this.character);
		component2.pointPinger = this;
		component2.renderer.material = Object.Instantiate<Material>(this.character.refs.mainRenderer.sharedMaterial);
		component2.material.SetFloat("_Opacity", num2);
		Object.Destroy(this.pingInstance, 2f);
	}

	// Token: 0x040011FA RID: 4602
	public GameObject pointPrefab;

	// Token: 0x040011FB RID: 4603
	public float coolDown;

	// Token: 0x040011FC RID: 4604
	public Character character;

	// Token: 0x040011FD RID: 4605
	private GameObject pingInstance;

	// Token: 0x040011FE RID: 4606
	private PhotonView photonView;

	// Token: 0x040011FF RID: 4607
	private float _timeLastPinged;
}
