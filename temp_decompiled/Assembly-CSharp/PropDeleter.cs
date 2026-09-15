using System;
using UnityEngine;

// Token: 0x020002D2 RID: 722
public class PropDeleter : LevelGenStep
{
	// Token: 0x0600136F RID: 4975 RVA: 0x0006294C File Offset: 0x00060B4C
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x00062964 File Offset: 0x00060B64
	public override void Execute()
	{
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, this.radius, HelperFunctions.GetMask(this.layerType)))
		{
			if (!(collider == null) && !(collider.gameObject == null))
			{
				int j = 0;
				Transform transform = collider.transform;
				while (j < 5)
				{
					j++;
					Transform parent = transform.parent;
					if (parent == null)
					{
						break;
					}
					PropGrouper componentInParent = transform.GetComponentInParent<PropGrouper>();
					if (!(componentInParent == null))
					{
						Transform transform2 = componentInParent.transform;
						bool flag = false;
						for (int k = 0; k < this.requiredParents.Length; k++)
						{
							if (transform2 == this.requiredParents[k])
							{
								flag = true;
							}
						}
						if (!flag && this.requiredParents.Length != 0)
						{
							break;
						}
						if (parent.GetComponent<PropSpawner>() || parent.GetComponent<PropSpawner_Line>())
						{
							Object.DestroyImmediate(transform.gameObject);
							break;
						}
						transform = parent;
					}
				}
			}
		}
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x00062A83 File Offset: 0x00060C83
	public override void Clear()
	{
	}

	// Token: 0x0400120A RID: 4618
	public HelperFunctions.LayerType layerType;

	// Token: 0x0400120B RID: 4619
	public float radius = 10f;

	// Token: 0x0400120C RID: 4620
	public Transform[] requiredParents;
}
