using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000366 RID: 870
public class VariantObjectSelector : MonoBehaviour, IGenConfigStep
{
	// Token: 0x06001622 RID: 5666 RVA: 0x00072410 File Offset: 0x00070610
	public void RunStep()
	{
		this.SelectVariations();
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x00072418 File Offset: 0x00070618
	public void SelectVariations()
	{
		VariantObject[] componentsInChildren = base.GetComponentsInChildren<VariantObject>(true);
		List<VariantObject> list = new List<VariantObject>();
		List<VariantObject> list2 = new List<VariantObject>();
		List<VariantObject> list3 = new List<VariantObject>();
		List<VariantObject> list4 = new List<VariantObject>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			switch (componentsInChildren[i].group)
			{
			case VariantObject.Group.Default:
				if (componentsInChildren[i].spawnChance > 0f)
				{
					list.Add(componentsInChildren[i]);
				}
				break;
			case VariantObject.Group.One:
				if (componentsInChildren[i].spawnChance > 0f)
				{
					list2.Add(componentsInChildren[i]);
				}
				break;
			case VariantObject.Group.Two:
				if (componentsInChildren[i].spawnChance > 0f)
				{
					list3.Add(componentsInChildren[i]);
				}
				break;
			case VariantObject.Group.Three:
				if (componentsInChildren[i].spawnChance > 0f)
				{
					list4.Add(componentsInChildren[i]);
				}
				break;
			}
		}
		new List<VariantObject>().AddRange(componentsInChildren);
		List<VariantObject> list5 = new List<VariantObject>();
		if (list.Count > 0)
		{
			VariantObject randomVariant = this.GetRandomVariant(list);
			list5.Add(randomVariant);
			list.Remove(randomVariant);
		}
		if (list2.Count > 0)
		{
			VariantObject randomVariant2 = this.GetRandomVariant(list2);
			list5.Add(randomVariant2);
			list2.Remove(randomVariant2);
		}
		if (list3.Count > 0)
		{
			VariantObject randomVariant3 = this.GetRandomVariant(list3);
			list5.Add(randomVariant3);
			list3.Remove(randomVariant3);
		}
		if (list4.Count > 0)
		{
			VariantObject randomVariant4 = this.GetRandomVariant(list4);
			list5.Add(randomVariant4);
			list4.Remove(randomVariant4);
		}
		VariantObject[] array = componentsInChildren;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].gameObject.SetActive(false);
		}
		foreach (VariantObject variantObject in list5)
		{
			variantObject.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x00072604 File Offset: 0x00070804
	private VariantObject GetRandomVariant(List<VariantObject> unPicked)
	{
		float num = 0f;
		foreach (VariantObject variantObject in unPicked)
		{
			num += variantObject.spawnChance;
		}
		float num2 = Random.Range(0f, num);
		foreach (VariantObject variantObject2 in unPicked)
		{
			num2 -= variantObject2.spawnChance;
			if (num2 <= 0f)
			{
				return variantObject2;
			}
		}
		return null;
	}
}
