using System;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public class Action_RaycastDart : ItemAction
{
	// Token: 0x06000F31 RID: 3889 RVA: 0x0004A4B7 File Offset: 0x000486B7
	public override void RunAction()
	{
		this.FireDart();
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0004A4BF File Offset: 0x000486BF
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.spawnTransform.position, this.dartCollisionSize);
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x0004A4E4 File Offset: 0x000486E4
	private void FireDart()
	{
		if (this.shotSFX)
		{
			this.shotSFX.Play(base.transform.position);
		}
		Physics.Raycast(this.spawnTransform.position, MainCamera.instance.transform.forward, ref this.lineHit, this.maxDistance, HelperFunctions.terrainMapMask, 1);
		if (!this.lineHit.collider)
		{
			this.lineHit.distance = this.maxDistance;
			this.lineHit.point = this.spawnTransform.position + MainCamera.instance.transform.forward * this.maxDistance;
		}
		this.sphereHits = Physics.SphereCastAll(this.spawnTransform.position, this.dartCollisionSize, MainCamera.instance.transform.forward, this.lineHit.distance, LayerMask.GetMask(new string[] { "Character" }), 1);
		foreach (RaycastHit raycastHit in this.sphereHits)
		{
			if (raycastHit.collider)
			{
				Character componentInParent = raycastHit.collider.GetComponentInParent<Character>();
				if (componentInParent)
				{
					Debug.Log("HIT");
					if (componentInParent != base.character)
					{
						this.DartImpact(componentInParent, this.spawnTransform.position, raycastHit.point);
						return;
					}
				}
			}
		}
		this.DartImpact(null, this.spawnTransform.position, this.lineHit.point);
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x0004A67C File Offset: 0x0004887C
	private void DartImpact(Character hitCharacter, Vector3 origin, Vector3 endpoint)
	{
		if (hitCharacter)
		{
			base.photonView.RPC("RPC_DartImpact", 0, new object[]
			{
				hitCharacter.photonView.ViewID,
				origin,
				endpoint
			});
			return;
		}
		base.photonView.RPC("RPC_DartImpact", 0, new object[] { -1, origin, endpoint });
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0004A700 File Offset: 0x00048900
	[PunRPC]
	private void RPC_DartImpact(int characterID, Vector3 origin, Vector3 endpoint)
	{
		if (characterID != -1)
		{
			Character component = PhotonNetwork.GetPhotonView(characterID).gameObject.GetComponent<Character>();
			if (component != null && component.photonView.IsMine)
			{
				Debug.Log("I'M HIT");
				foreach (Affliction affliction in this.afflictionsOnHit)
				{
					component.refs.afflictions.AddAffliction(affliction, false);
				}
			}
		}
		Object.Instantiate<GameObject>(this.dartVFX, endpoint, Quaternion.identity);
		GamefeelHandler.instance.AddPerlinShakeProximity(endpoint, 5f, 0.2f, 15f, 10f);
	}

	// Token: 0x04000D39 RID: 3385
	public float maxDistance;

	// Token: 0x04000D3A RID: 3386
	public float dartCollisionSize;

	// Token: 0x04000D3B RID: 3387
	[SerializeReference]
	public Affliction[] afflictionsOnHit;

	// Token: 0x04000D3C RID: 3388
	public Transform spawnTransform;

	// Token: 0x04000D3D RID: 3389
	public GameObject dartVFX;

	// Token: 0x04000D3E RID: 3390
	private HelperFunctions.LayerType layerMaskType;

	// Token: 0x04000D3F RID: 3391
	private RaycastHit lineHit;

	// Token: 0x04000D40 RID: 3392
	private RaycastHit[] sphereHits;

	// Token: 0x04000D41 RID: 3393
	public SFX_Instance shotSFX;
}
