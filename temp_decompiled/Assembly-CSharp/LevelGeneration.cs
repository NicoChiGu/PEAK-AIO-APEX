using System;
using UnityEngine;

// Token: 0x02000289 RID: 649
public class LevelGeneration : MonoBehaviour
{
	// Token: 0x0600121F RID: 4639 RVA: 0x0005C198 File Offset: 0x0005A398
	public void Generate()
	{
		Debug.LogWarning("UH OHHHH!!! Doing nothing because we're not in Editor code!!", this);
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x0005C1A8 File Offset: 0x0005A3A8
	private void RandomizeBiomeVariants()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			BiomeVariant[] componentsInChildren = base.transform.GetChild(i).GetComponentsInChildren<BiomeVariant>(true);
			BiomeVariant[] array = componentsInChildren;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].gameObject.SetActive(false);
			}
			if (componentsInChildren.Length != 0)
			{
				componentsInChildren[Random.Range(0, componentsInChildren.Length)].gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x0005C216 File Offset: 0x0005A416
	private void Clear()
	{
		Object.FindFirstObjectByType<LightVolume>().SetSize();
		base.GetComponent<PropGrouper>().ClearAll();
	}

	// Token: 0x040010A5 RID: 4261
	public int seed;

	// Token: 0x040010A6 RID: 4262
	public bool updateLightmap = true;
}
