using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class MagicBean : ItemComponent
{
	// Token: 0x0600093C RID: 2364 RVA: 0x00030E58 File Offset: 0x0002F058
	public void Update()
	{
		if (this.photonView.IsMine)
		{
			if (this.item.itemState == ItemState.Held)
			{
				base.GetData<OptionableBoolItemData>(DataEntryKey.Used).HasData = true;
				return;
			}
			if (PhotonNetwork.IsMasterClient && this.isPlanted)
			{
				this.timeToPlant -= Time.deltaTime;
				if (this.timeToPlant <= 0f)
				{
					float vineDistance = this.GetVineDistance(base.transform.position, this.averageNormal);
					this.photonView.RPC("GrowVineRPC", 0, new object[]
					{
						base.transform.position,
						this.averageNormal,
						vineDistance
					});
					this.GrowVineRPC(base.transform.position, this.averageNormal, vineDistance);
					PhotonNetwork.Destroy(base.gameObject);
				}
			}
		}
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x00030F41 File Offset: 0x0002F141
	private void DebugValue()
	{
		if (base.HasData(DataEntryKey.Used))
		{
			Debug.Log(base.GetData<BoolItemData>(DataEntryKey.Used).Value);
			return;
		}
		Debug.Log("No data");
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x00030F6D File Offset: 0x0002F16D
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00030F70 File Offset: 0x0002F170
	private void OnCollisionEnter(Collision collision)
	{
		if (this.photonView.IsMine && this.item.itemState == ItemState.Ground && base.GetData<OptionableBoolItemData>(DataEntryKey.Used).HasData && HelperFunctions.IsLayerInLayerMask(HelperFunctions.LayerType.TerrainMap, collision.gameObject.layer))
		{
			this.item.SetKinematicNetworked(true, this.item.transform.position, this.item.transform.rotation);
			this.DoNormalRaycasts(collision.contacts[0].point, collision.contacts[0].normal);
			this.isPlanted = true;
		}
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x00031018 File Offset: 0x0002F218
	private float GetVineDistance(Vector3 startPos, Vector3 direction)
	{
		RaycastHit[] array = HelperFunctions.LineCheckAll(startPos, startPos + direction * this.plantPrefab.maxLength, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		float num = this.plantPrefab.maxLength;
		foreach (RaycastHit raycastHit in array)
		{
			if (raycastHit.distance > 0.7f && raycastHit.distance < num)
			{
				num = raycastHit.distance;
			}
		}
		return num;
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x0003108D File Offset: 0x0002F28D
	[PunRPC]
	protected void GrowVineRPC(Vector3 pos, Vector3 direction, float maxLength)
	{
		MagicBeanVine magicBeanVine = Object.Instantiate<MagicBeanVine>(this.plantPrefab, pos, Quaternion.identity);
		magicBeanVine.transform.up = direction;
		magicBeanVine.maxLength = maxLength;
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x000310B4 File Offset: 0x0002F2B4
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		foreach (Vector3 vector in this.raycastSpotsTest)
		{
			Gizmos.DrawSphere(vector, 0.1f);
			Gizmos.DrawLine(base.transform.position, base.transform.position + this.averageNormal);
		}
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x0003113C File Offset: 0x0002F33C
	private void TestRaycast()
	{
		this.DoNormalRaycasts(base.transform.position, Vector3.up);
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x00031154 File Offset: 0x0002F354
	private void DoNormalRaycasts(Vector3 centralHit, Vector3 centralNormal)
	{
		this.raycastSpotsTest.Clear();
		List<Vector3> list = new List<Vector3>();
		float num = 0.2f;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 || j != 0)
				{
					Vector3 vector = Vector3.ProjectOnPlane(new Vector3((float)i, 0f, (float)j), centralNormal).normalized * num;
					Vector3 vector2 = centralHit + vector + centralNormal;
					this.raycastSpotsTest.Add(vector2);
					this.raycastResult = HelperFunctions.LineCheck(vector2, vector2 - centralNormal * 2f, HelperFunctions.LayerType.TerrainMap, 0f, 1);
					if (this.raycastResult.collider != null)
					{
						list.Add(this.raycastResult.normal);
					}
				}
			}
			Vector3 vector3 = centralNormal;
			foreach (Vector3 vector4 in list)
			{
				vector3 += vector4;
			}
			this.averageNormal = vector3.normalized;
			if (Vector3.Angle(this.averageNormal, Vector3.up) < this.snapToVerticalAngle)
			{
				this.averageNormal = Vector3.up;
			}
		}
	}

	// Token: 0x0400089A RID: 2202
	private bool isPlanted;

	// Token: 0x0400089B RID: 2203
	public float timeToPlant;

	// Token: 0x0400089C RID: 2204
	public MagicBeanVine plantPrefab;

	// Token: 0x0400089D RID: 2205
	public float snapToVerticalAngle = 15f;

	// Token: 0x0400089E RID: 2206
	private List<Vector3> raycastSpotsTest = new List<Vector3>();

	// Token: 0x0400089F RID: 2207
	private RaycastHit raycastResult;

	// Token: 0x040008A0 RID: 2208
	private Vector3 averageNormal;
}
