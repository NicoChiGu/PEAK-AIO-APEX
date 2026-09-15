using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002C4 RID: 708
public class PlayerGizmos : MonoBehaviour
{
	// Token: 0x06001339 RID: 4921 RVA: 0x00061F63 File Offset: 0x00060163
	private void Start()
	{
		PlayerGizmos.instance = this;
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x00061F6C File Offset: 0x0006016C
	private void Update()
	{
		for (int i = this.gizmos.Count - 1; i >= 0; i--)
		{
			GizmoInstance gizmoInstance = this.gizmos[i];
			if (gizmoInstance == null)
			{
				this.gizmos.RemoveAt(i);
			}
			else
			{
				gizmoInstance.framesSinceActivated++;
				if (gizmoInstance.framesSinceActivated > 5)
				{
					gizmoInstance.giz.SetActive(false);
					this.gizmos.Remove(gizmoInstance);
				}
			}
		}
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x00061FE0 File Offset: 0x000601E0
	public void DisplayGizmo(PlayerGizmos.GizmoType gizmoType, Vector3 pos, Vector3 direction)
	{
		GameObject gizmo = this.GetGizmo(gizmoType);
		GizmoInstance gizmoInstance = this.Contains(gizmo);
		if (gizmoInstance != null)
		{
			gizmoInstance.framesSinceActivated = 0;
		}
		else
		{
			this.gizmos.Add(new GizmoInstance
			{
				giz = gizmo,
				framesSinceActivated = 0
			});
		}
		gizmo.SetActive(true);
		gizmo.transform.position = pos;
		gizmo.transform.rotation = Quaternion.LookRotation(direction);
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x0006204B File Offset: 0x0006024B
	private GameObject GetGizmo(PlayerGizmos.GizmoType gizmoType)
	{
		if (gizmoType == PlayerGizmos.GizmoType.Pointer)
		{
			return this.pointer;
		}
		return null;
	}

	// Token: 0x0600133D RID: 4925 RVA: 0x00062058 File Offset: 0x00060258
	private GizmoInstance Contains(GameObject gizmo)
	{
		foreach (GizmoInstance gizmoInstance in this.gizmos)
		{
			if (gizmoInstance.giz == gizmo)
			{
				return gizmoInstance;
			}
		}
		return null;
	}

	// Token: 0x040011DF RID: 4575
	public List<GizmoInstance> gizmos = new List<GizmoInstance>();

	// Token: 0x040011E0 RID: 4576
	public static PlayerGizmos instance;

	// Token: 0x040011E1 RID: 4577
	public GameObject pointer;

	// Token: 0x020004F3 RID: 1267
	public enum GizmoType
	{
		// Token: 0x04001B18 RID: 6936
		Pointer
	}
}
