using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class BingBongSpawnTool : MonoBehaviour
{
	// Token: 0x06000FB6 RID: 4022 RVA: 0x0004E080 File Offset: 0x0004C280
	private void Update()
	{
		this.counter += Time.unscaledDeltaTime;
		if (this.counter < this.spawnRate)
		{
			return;
		}
		if (!this.auto && !Input.GetKeyDown(323))
		{
			return;
		}
		if (this.auto && !Input.GetKey(323))
		{
			return;
		}
		this.counter = 0f;
		this.Spawn();
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x0004E0EC File Offset: 0x0004C2EC
	private void Spawn()
	{
		Vector3 position = this.GetPosition();
		Quaternion rotation = this.GetRotation();
		GameObject gameObject = PhotonNetwork.Instantiate(this.folder + this.objectToSpawn.name, position, rotation, 0, null);
		if (this.bingbongInit)
		{
			gameObject.GetComponent<PhotonView>().RPC("RPCA_BingBongInitObj", 3, new object[] { base.GetComponentInParent<PhotonView>().ViewID });
		}
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0004E15C File Offset: 0x0004C35C
	public Vector3 GetPosition()
	{
		Vector3 vector = base.transform.position;
		if (this.pos == BingBongSpawnTool.SpawnPos.RaycastPos)
		{
			RaycastHit raycastHit = HelperFunctions.LineCheck(base.transform.position, base.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical, 0f, 1);
			if (raycastHit.transform)
			{
				vector = raycastHit.point;
				vector += raycastHit.normal * this.normalOffsetPos;
			}
		}
		else if (this.pos == BingBongSpawnTool.SpawnPos.BingBong)
		{
			vector = base.transform.TransformPoint(Vector3.forward * 2f);
		}
		return vector + Random.insideUnitSphere * this.randomPosRadius;
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x0004E21C File Offset: 0x0004C41C
	public Quaternion GetRotation()
	{
		if (this.rot == BingBongSpawnTool.SpawnRot.BingBongRotation)
		{
			return base.transform.rotation;
		}
		if (this.rot == BingBongSpawnTool.SpawnRot.Random)
		{
			return Random.rotation;
		}
		if (this.rot == BingBongSpawnTool.SpawnRot.RaycastNormal)
		{
			return Quaternion.LookRotation(HelperFunctions.LineCheck(base.transform.position, base.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical, 0f, 1).normal);
		}
		BingBongSpawnTool.SpawnRot spawnRot = this.rot;
		return Quaternion.identity;
	}

	// Token: 0x04000E0E RID: 3598
	public float spawnRate = 0.1f;

	// Token: 0x04000E0F RID: 3599
	public bool auto = true;

	// Token: 0x04000E10 RID: 3600
	public string folder = "0_Items/";

	// Token: 0x04000E11 RID: 3601
	public GameObject objectToSpawn;

	// Token: 0x04000E12 RID: 3602
	public bool bingbongInit;

	// Token: 0x04000E13 RID: 3603
	public BingBongSpawnTool.SpawnPos pos;

	// Token: 0x04000E14 RID: 3604
	public BingBongSpawnTool.SpawnRot rot;

	// Token: 0x04000E15 RID: 3605
	public float randomPosRadius;

	// Token: 0x04000E16 RID: 3606
	public float normalOffsetPos;

	// Token: 0x04000E17 RID: 3607
	private float counter;

	// Token: 0x020004CD RID: 1229
	public enum SpawnPos
	{
		// Token: 0x04001A8C RID: 6796
		BingBong,
		// Token: 0x04001A8D RID: 6797
		RaycastPos
	}

	// Token: 0x020004CE RID: 1230
	public enum SpawnRot
	{
		// Token: 0x04001A8F RID: 6799
		BingBongRotation,
		// Token: 0x04001A90 RID: 6800
		Random,
		// Token: 0x04001A91 RID: 6801
		RaycastNormal,
		// Token: 0x04001A92 RID: 6802
		Identity
	}
}
