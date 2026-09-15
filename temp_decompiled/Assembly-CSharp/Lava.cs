using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000283 RID: 643
public class Lava : MonoBehaviour
{
	// Token: 0x060011EC RID: 4588 RVA: 0x0005AA71 File Offset: 0x00058C71
	private void Start()
	{
		this.bounds = base.GetComponentInChildren<MeshRenderer>().bounds;
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x0005AA84 File Offset: 0x00058C84
	private void FixedUpdate()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.Movement();
		if (Character.localCharacter)
		{
			this.DoEffects();
			this.Heat();
		}
		this.TryCookItems();
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x0005AAB8 File Offset: 0x00058CB8
	private void Heat()
	{
		Character localCharacter = Character.localCharacter;
		if (localCharacter == null)
		{
			return;
		}
		this.counter += Time.deltaTime;
		if (this.OutsideBounds(localCharacter.Center))
		{
			return;
		}
		float num = localCharacter.Center.y - base.transform.position.y;
		float num2 = 1f - Mathf.Clamp01(num / this.height);
		if (num2 < 0.01f)
		{
			return;
		}
		if (this.counter < this.heatRate)
		{
			return;
		}
		this.counter = 0f;
		localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, num2 * this.heat * 1.5f, false, true, true);
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x0005AB70 File Offset: 0x00058D70
	private bool OutsideBounds(Vector3 pos)
	{
		return pos.x > this.bounds.max.x || pos.x < this.bounds.min.x || pos.z > this.bounds.max.z || pos.z < this.bounds.min.z;
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0005ABE8 File Offset: 0x00058DE8
	private void DoEffects()
	{
		Character localCharacter = Character.localCharacter;
		if (this.OutsideBounds(localCharacter.Center))
		{
			return;
		}
		if (localCharacter.Center.y - 0.5f > base.transform.position.y)
		{
			return;
		}
		localCharacter.AddForce(Vector3.up * 80f, 0.5f, 1f);
		localCharacter.data.sinceGrounded = 0f;
		localCharacter.refs.movement.ApplyExtraDrag(0.8f, true);
		if (this.hitPlayers.Contains(localCharacter))
		{
			return;
		}
		if (localCharacter.data.dead)
		{
			return;
		}
		if (localCharacter.refs.afflictions.statusSum > 1.9f)
		{
			return;
		}
		this.HitPlayer(localCharacter);
		base.StartCoroutine(this.IHoldPlayer(localCharacter));
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x0005ACC0 File Offset: 0x00058EC0
	private void HitPlayer(Character item)
	{
		item.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f, false, true, true);
		item.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 0.25f, false, true, true);
		item.data.sinceGrounded = 0f;
	}

	// Token: 0x060011F2 RID: 4594 RVA: 0x0005AD11 File Offset: 0x00058F11
	private IEnumerator IHoldPlayer(Character item)
	{
		this.hitPlayers.Add(item);
		yield return new WaitForSeconds(1f);
		this.hitPlayers.Remove(item);
		yield break;
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x0005AD27 File Offset: 0x00058F27
	private void Movement()
	{
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x0005AD2C File Offset: 0x00058F2C
	private void TryCookItems()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		for (int i = 0; i < Item.ALL_ACTIVE_ITEMS.Count; i++)
		{
			Item item = Item.ALL_ACTIVE_ITEMS[i];
			if (ClassExtensionsMethods.UnityObjectExists<Item>(item) && item.itemState != ItemState.Held && !this.OutsideBounds(item.Center()))
			{
				if (item.itemState == ItemState.InBackpack && item.backpackReference.IsSome && item.backpackReference.Value.Item2.type == BackpackReference.BackpackType.Equipped)
				{
					return;
				}
				if (this.TestSacrificeIdol(item))
				{
					return;
				}
				if (item.cooking.canBeCooked && this.GetItemCookAmount(item) > 0f && this.itemToCookTime.TryAdd(item, 0f))
				{
					Debug.Log("Lava started cooking: " + item.GetItemName(null));
					item.GetComponent<ItemCooking>().StartCookingVisuals();
				}
			}
		}
		this.itemToRemoveList.Clear();
		this.itemToCookList.Clear();
		foreach (Item item2 in this.itemToCookTime.Keys)
		{
			if (item2 == null)
			{
				this.itemToRemoveList.Add(item2);
			}
			else if (this.OutsideBounds(item2.Center()))
			{
				this.itemToRemoveList.Add(item2);
				item2.GetComponent<ItemCooking>().CancelCookingVisuals();
			}
			else
			{
				this.itemToCookList.Add(item2);
			}
		}
		foreach (Item item3 in this.itemToCookList)
		{
			float num = this.GetItemCookAmount(item3) * Time.deltaTime;
			Dictionary<Item, float> dictionary = this.itemToCookTime;
			Item item4 = item3;
			dictionary[item4] += num;
			if (this.itemToCookTime[item3] >= 1f)
			{
				Debug.Log("Lava finished cooking: " + item3.GetItemName(null));
				item3.GetComponent<ItemCooking>().FinishCooking();
				this.itemToCookTime[item3] = 0f;
			}
		}
		foreach (Item item5 in this.itemToRemoveList)
		{
			this.itemToCookTime.Remove(item5);
		}
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x0005AFC4 File Offset: 0x000591C4
	private float GetItemCookAmount(Item item)
	{
		float num = item.Center().y - base.transform.position.y;
		float num2 = 1f - Mathf.Clamp01(num / this.height);
		if (num2 < 0.01f)
		{
			return 0f;
		}
		return num2 * 0.7f;
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x0005B018 File Offset: 0x00059218
	private bool TestSacrificeIdol(Item item)
	{
		if (!this.isKiln)
		{
			return false;
		}
		if (item.Center().y > base.transform.position.y)
		{
			return false;
		}
		if (item.photonView.IsMine && item.itemTags.HasFlag(Item.ItemTags.GoldenIdol))
		{
			if (Character.localCharacter.data.currentItem == item)
			{
				Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
				Character.localCharacter.refs.afflictions.UpdateWeight();
			}
			PhotonNetwork.Destroy(item.gameObject);
			GameUtils.instance.ThrowSacrificeAchievement();
			return true;
		}
		return false;
	}

	// Token: 0x0400106E RID: 4206
	private List<Character> hitPlayers = new List<Character>();

	// Token: 0x0400106F RID: 4207
	public float heatRate = 0.5f;

	// Token: 0x04001070 RID: 4208
	public float heat = 0.02f;

	// Token: 0x04001071 RID: 4209
	public float height = 10f;

	// Token: 0x04001072 RID: 4210
	public bool isKiln;

	// Token: 0x04001073 RID: 4211
	private Bounds bounds;

	// Token: 0x04001074 RID: 4212
	private float counter;

	// Token: 0x04001075 RID: 4213
	public Dictionary<Item, float> itemToCookTime = new Dictionary<Item, float>();

	// Token: 0x04001076 RID: 4214
	private List<Item> itemToRemoveList = new List<Item>();

	// Token: 0x04001077 RID: 4215
	private List<Item> itemToCookList = new List<Item>();
}
