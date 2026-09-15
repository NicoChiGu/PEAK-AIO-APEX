using System;
using UnityEngine;

// Token: 0x02000245 RID: 581
public class DesertRockSpawner : LevelGenStep
{
	// Token: 0x060010FB RID: 4347 RVA: 0x00055C0C File Offset: 0x00053E0C
	public override void Clear()
	{
		this.GetRefs();
		for (int i = 0; i < this.enterences.childCount; i++)
		{
			Transform child = this.enterences.GetChild(i);
			for (int j = child.childCount - 1; j >= 0; j--)
			{
				Object.DestroyImmediate(child.GetChild(j).gameObject);
			}
		}
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x00055C68 File Offset: 0x00053E68
	public override void Execute()
	{
		bool flag = Random.value < 0.5f;
		this.Clear();
		int num = Random.Range(0, this.enterences.childCount);
		for (int i = 0; i < this.enterences.childCount; i++)
		{
			Transform child = this.enterences.GetChild(i);
			if (i == num && flag)
			{
				HelperFunctions.InstantiatePrefab(this.enterenceObjects[Random.Range(0, this.enterenceObjects.Length)], child.position, child.rotation, child).transform.localScale = Vector3.one * 2f;
				this.inside.position = new Vector3(child.position.x, this.inside.position.y, child.position.z);
			}
			else
			{
				HelperFunctions.InstantiatePrefab(this.blockerObjects[Random.Range(0, this.blockerObjects.Length)], child.position, child.rotation, child).transform.localScale = Vector3.one * 2f;
			}
		}
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x00055D83 File Offset: 0x00053F83
	private void GetRefs()
	{
		this.enterences = base.transform.Find("Enterences");
		this.inside = base.transform.Find("Inside");
	}

	// Token: 0x04000F70 RID: 3952
	public GameObject[] blockerObjects;

	// Token: 0x04000F71 RID: 3953
	public GameObject[] enterenceObjects;

	// Token: 0x04000F72 RID: 3954
	private Transform enterences;

	// Token: 0x04000F73 RID: 3955
	private Transform inside;
}
