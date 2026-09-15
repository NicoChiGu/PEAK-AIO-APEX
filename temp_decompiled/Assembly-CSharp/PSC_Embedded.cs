using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020002F3 RID: 755
public class PSC_Embedded : PropSpawnerConstraint, IValidationConstraint
{
	// Token: 0x060013D5 RID: 5077 RVA: 0x00064904 File Offset: 0x00062B04
	private bool IsEmbeddedInCollider(PropSpawner.SpawnData spawnData, Collider[] ignoredColliders)
	{
		bool queriesHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		Vector3 pos = spawnData.pos;
		RaycastHit[] array = new RaycastHit[100];
		bool flag = false;
		List<Collider> list = new List<Collider>();
		foreach (Vector3 vector in PSC_Embedded.LeftRight)
		{
			foreach (Vector3 vector2 in PSC_Embedded.UpDown)
			{
				foreach (Vector3 vector3 in PSC_Embedded.ForwardBack)
				{
					Vector3 vector4 = vector + vector2 + vector3;
					if (vector4.sqrMagnitude >= 0.1f)
					{
						list.Clear();
						Vector3 normalized = vector4.normalized;
						int num = Physics.RaycastNonAlloc(pos - this.Deadzone * normalized, normalized, array, this.RaycastDistance, HelperFunctions.GetMask(this.LayerType));
						for (int l = 0; l < num; l++)
						{
							if (!Enumerable.Contains<Collider>(ignoredColliders, array[l].collider) && !list.Contains(array[l].collider))
							{
								if (Mathf.Abs(Vector3.Angle(vector4, array[l].normal)) < 90f && array[l].distance > 2f * this.Deadzone)
								{
									flag = true;
									break;
								}
								list.Add(array[l].collider);
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
		}
		Physics.queriesHitBackfaces = queriesHitBackfaces;
		return flag;
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x00064AC2 File Offset: 0x00062CC2
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		return this.DesiredResult == this.IsEmbeddedInCollider(spawnData, new Collider[] { spawnData.hit.collider });
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x00064AE8 File Offset: 0x00062CE8
	public override bool Validate(GameObject spawnedProp, PropSpawner.SpawnData spawnData)
	{
		Collider[] array = Enumerable.ToArray<Collider>(Enumerable.Append<Collider>(spawnedProp.GetComponentsInChildren<Collider>(), spawnData.hit.collider));
		return this.DesiredResult == this.IsEmbeddedInCollider(spawnData, array);
	}

	// Token: 0x0400126B RID: 4715
	public HelperFunctions.LayerType LayerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x0400126C RID: 4716
	[SerializeField]
	[Tooltip("Radius around spawn transform where it's ok to be embedded")]
	private float Deadzone = 0.01f;

	// Token: 0x0400126D RID: 4717
	[Tooltip("Should be at least as big as the largest object you think this prop might be embedded in")]
	public float RaycastDistance = 50f;

	// Token: 0x0400126E RID: 4718
	public bool DesiredResult;

	// Token: 0x0400126F RID: 4719
	private static readonly Vector3[] UpDown = new Vector3[]
	{
		Vector3.up,
		Vector3.zero,
		Vector3.down
	};

	// Token: 0x04001270 RID: 4720
	private static readonly Vector3[] ForwardBack = new Vector3[]
	{
		Vector3.forward,
		Vector3.zero,
		Vector3.back
	};

	// Token: 0x04001271 RID: 4721
	private static readonly Vector3[] LeftRight = new Vector3[]
	{
		Vector3.left,
		Vector3.zero,
		Vector3.right
	};
}
