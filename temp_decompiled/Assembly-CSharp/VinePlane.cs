using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000367 RID: 871
public class VinePlane : MonoBehaviour
{
	// Token: 0x06001626 RID: 5670 RVA: 0x000726C4 File Offset: 0x000708C4
	private void Start()
	{
		this.UpdateCollider();
	}

	// Token: 0x06001627 RID: 5671 RVA: 0x000726CC File Offset: 0x000708CC
	private void OnValidate()
	{
		if (!Application.isPlaying && this.liveEdit)
		{
			this.Blast();
		}
	}

	// Token: 0x06001628 RID: 5672 RVA: 0x000726E4 File Offset: 0x000708E4
	public void Blast()
	{
		this.meshCollider.enabled = false;
		this.RestoreDefaults();
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			Transform child = this.bonesParent.GetChild(i);
			Vector3 vector = child.transform.position + child.transform.up * this.raycastStartLength;
			Vector3 vector2 = child.transform.position - child.transform.up * (this.raycastStartLength + this.raycastEndLength);
			RaycastHit raycastHit;
			if (Physics.Linecast(vector, vector2, ref raycastHit, this.mask.value, 1))
			{
				if (child.gameObject.activeSelf)
				{
					child.transform.position = raycastHit.point + base.transform.up * this.lift * this.GetDistanceFromCorner(i);
				}
				else
				{
					child.transform.position = raycastHit.point;
				}
				Plane plane;
				plane..ctor(base.transform.up, base.transform.position);
				if (child.gameObject.activeSelf)
				{
					float num = Mathf.Pow(Mathf.Clamp01(Mathf.Abs(plane.GetDistanceToPoint(child.transform.position) / this.raycastEndLength)), this.planeLiftPow);
					child.transform.position += base.transform.up * num * this.planeLiftAmount;
				}
			}
		}
		if (!this.liveEdit)
		{
			this.Bake();
			return;
		}
		this.skinnedMeshRenderer.material = this.editingMaterial;
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x000728A0 File Offset: 0x00070AA0
	public void Bake()
	{
		this.meshCollider.enabled = true;
		this.liveEdit = false;
		this.UpdateCollider();
		if (this.vineType == VinePlane.VineType.Normal)
		{
			this.skinnedMeshRenderer.material = this.vineMatNormal;
		}
		else if (this.vineType == VinePlane.VineType.Thorns)
		{
			this.skinnedMeshRenderer.material = this.vineMatThorns;
		}
		else if (this.vineType == VinePlane.VineType.Poison)
		{
			this.skinnedMeshRenderer.material = this.vineMatPoison;
		}
		this.skinnedMeshRendererLeaves.material = this.skinnedMeshRenderer.material;
	}

	// Token: 0x0600162A RID: 5674 RVA: 0x0007292D File Offset: 0x00070B2D
	private float GetDistanceFromCorner(int index)
	{
		return Mathf.InverseLerp(this.distanceToCorner, 0f, Vector3.Distance(this.bonesParent.GetChild(index).position, this.centerBone.position));
	}

	// Token: 0x0600162B RID: 5675 RVA: 0x00072960 File Offset: 0x00070B60
	private void RestoreDefaultsButton()
	{
		this.RestoreDefaults();
		this.Bake();
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x00072970 File Offset: 0x00070B70
	private void RestoreDefaults()
	{
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			this.bonesParent.GetChild(i).localPosition = this.defaultPositions[i];
			this.bonesParent.GetChild(i).localRotation = this.defaultRotations[i];
		}
		for (int j = 0; j < this.bonesParent.childCount; j++)
		{
			if (Mathf.Abs(this.bonesParent.GetChild(j).localPosition.y) > 3.9f)
			{
				this.bonesParent.GetChild(j).gameObject.SetActive(false);
			}
			else
			{
				this.bonesParent.GetChild(j).gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x00072A38 File Offset: 0x00070C38
	private void SetDefaultsBECAREFUL()
	{
		this.defaultPositions.Clear();
		this.defaultRotations.Clear();
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			this.defaultPositions.Add(this.bonesParent.GetChild(i).localPosition);
			this.defaultRotations.Add(this.bonesParent.GetChild(i).localRotation);
		}
		this.Bake();
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x00072AB0 File Offset: 0x00070CB0
	private void UpdateCollider()
	{
		this.skinnedMeshRenderer.ResetBounds();
		this.bakedMesh = new Mesh();
		this.skinnedMeshRenderer.BakeMesh(this.bakedMesh, true);
		this.meshCollider.sharedMesh = null;
		this.meshCollider.sharedMesh = this.bakedMesh;
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x00072B04 File Offset: 0x00070D04
	private void OnDrawGizmos()
	{
		Plane plane;
		plane..ctor(base.transform.up, base.transform.position);
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			if (this.bonesParent.GetChild(i).gameObject.activeSelf)
			{
				float num = Mathf.Abs(plane.GetDistanceToPoint(this.bonesParent.GetChild(i).transform.position));
				Gizmos.color = new Color(num, num, num);
				Gizmos.DrawSphere(this.bonesParent.GetChild(i).transform.position, 0.1f);
			}
		}
	}

	// Token: 0x0400150A RID: 5386
	public SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x0400150B RID: 5387
	public SkinnedMeshRenderer skinnedMeshRendererLeaves;

	// Token: 0x0400150C RID: 5388
	public MeshCollider meshCollider;

	// Token: 0x0400150D RID: 5389
	private Mesh bakedMesh;

	// Token: 0x0400150E RID: 5390
	public Transform bonesParent;

	// Token: 0x0400150F RID: 5391
	public float raycastStartLength = 1f;

	// Token: 0x04001510 RID: 5392
	public float raycastEndLength = 5f;

	// Token: 0x04001511 RID: 5393
	public LayerMask mask;

	// Token: 0x04001512 RID: 5394
	public float distanceToCorner = 5f;

	// Token: 0x04001513 RID: 5395
	public Transform centerBone;

	// Token: 0x04001514 RID: 5396
	public Material vineMatNormal;

	// Token: 0x04001515 RID: 5397
	public Material vineMatPoison;

	// Token: 0x04001516 RID: 5398
	public Material vineMatThorns;

	// Token: 0x04001517 RID: 5399
	public Material editingMaterial;

	// Token: 0x04001518 RID: 5400
	public VinePlane.VineType vineType;

	// Token: 0x04001519 RID: 5401
	public float lift = 0.1f;

	// Token: 0x0400151A RID: 5402
	public float planeLiftAmount = 0.5f;

	// Token: 0x0400151B RID: 5403
	public float planeLiftPow = 5f;

	// Token: 0x0400151C RID: 5404
	public bool liveEdit;

	// Token: 0x0400151D RID: 5405
	public List<Vector3> defaultPositions = new List<Vector3>();

	// Token: 0x0400151E RID: 5406
	public List<Quaternion> defaultRotations = new List<Quaternion>();

	// Token: 0x02000522 RID: 1314
	public enum VineType
	{
		// Token: 0x04001BC0 RID: 7104
		Normal,
		// Token: 0x04001BC1 RID: 7105
		Poison,
		// Token: 0x04001BC2 RID: 7106
		Thorns
	}
}
