using System;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class IcicleCheck : CustomSpawnCondition
{
	// Token: 0x06001193 RID: 4499 RVA: 0x000587F0 File Offset: 0x000569F0
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		PropSpawner comp = base.GetComponentInParent<PropSpawner>();
		base.transform.localScale = ExtSwizzle.xxx(ExtDataTypes.PRndRange(this.minMaxScale));
		Vector3 vector = this.boxCollider.transform.TransformPoint(this.boxCollider.center);
		Vector3 vector2 = Vector3.Scale(this.boxCollider.transform.lossyScale, this.boxCollider.size) / 2f;
		if (!this.LineCheck())
		{
			return false;
		}
		Collider[] array = Enumerable.ToArray<Collider>(Enumerable.Where<Collider>(Physics.OverlapBox(vector, vector2, this.boxCollider.transform.rotation), (Collider c) => c.GetComponentInParent<PropSpawner>() != comp));
		foreach (Collider collider in array)
		{
			Debug.DrawLine(vector, collider.transform.position, Color.red);
		}
		base.transform.position += ExtSwizzle.oxo(ExtDataTypes.PRndRange(Vector2.Scale(base.transform.lossyScale, this.minMax)));
		return array.Length == 0;
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x00058920 File Offset: 0x00056B20
	public bool LineCheck()
	{
		Vector3 vector = base.transform.TransformPoint(this.localStart);
		Vector3 vector2 = base.transform.TransformPoint(this.localEnd);
		bool flag = !HelperFunctions.LineCheck(vector, vector2, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform;
		Debug.DrawLine(vector, vector2, flag ? Color.green : Color.red, 10f);
		return flag;
	}

	// Token: 0x04001012 RID: 4114
	public BoxCollider boxCollider;

	// Token: 0x04001013 RID: 4115
	public Vector2 minMax;

	// Token: 0x04001014 RID: 4116
	public Vector2 minMaxScale = new Vector2(1f, 1f);

	// Token: 0x04001015 RID: 4117
	public Vector3 localStart = new Vector3(0f, 0f, 0f);

	// Token: 0x04001016 RID: 4118
	public Vector3 localEnd = new Vector3(0f, 5f, 0f);
}
