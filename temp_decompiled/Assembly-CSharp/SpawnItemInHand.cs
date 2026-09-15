using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class SpawnItemInHand : MonoBehaviour
{
	// Token: 0x06000D52 RID: 3410 RVA: 0x00042DE3 File Offset: 0x00040FE3
	private IEnumerator Start()
	{
		while (!Character.localCharacter)
		{
			yield return null;
		}
		if (this.blockItemSwapUntilSpawned)
		{
			Character.localCharacter.input.itemSwitchBlocked = true;
		}
		yield return null;
		yield return null;
		yield return null;
		yield return new WaitForSeconds(1.5f);
		Character.localCharacter.refs.items.SpawnItemInHand(this.item.gameObject.name);
		while (Character.localCharacter.data.currentItem == null)
		{
			yield return null;
		}
		Character.localCharacter.input.itemSwitchBlocked = false;
		yield break;
	}

	// Token: 0x04000B89 RID: 2953
	public Item item;

	// Token: 0x04000B8A RID: 2954
	public bool blockItemSwapUntilSpawned;
}
