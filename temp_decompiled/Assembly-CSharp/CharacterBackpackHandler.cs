using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000071 RID: 113
public class CharacterBackpackHandler : MonoBehaviour
{
	// Token: 0x06000502 RID: 1282 RVA: 0x0001D6B4 File Offset: 0x0001B8B4
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.characterItems = base.GetComponent<CharacterItems>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x0001D6DC File Offset: 0x0001B8DC
	private void LateUpdate()
	{
		bool flag = !this.character.player.backpackSlot.IsEmpty();
		bool flag2 = this.characterItems.currentSelectedSlot.IsSome && this.characterItems.currentSelectedSlot.Value == 3;
		bool flag3 = flag && !flag2;
		bool flag4 = flag3;
		if (this.character.photonView.IsMine && !MainCameraMovement.IsSpectating)
		{
			flag4 = false;
		}
		this.backpack.SetActive(flag4);
		if (flag3)
		{
			if (!this.t)
			{
				for (int i = 0; i < this.wearSFX.Length; i++)
				{
					this.wearSFX[i].Play(this.character.refs.hip.transform.position);
				}
			}
			this.t = true;
		}
		else
		{
			this.t = false;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			if (!this.lastShow && flag3)
			{
				base.StartCoroutine(this.RefreshVisualsDelayed());
			}
			else if (this.lastShow && !flag3)
			{
				this.backpackVisuals.RemoveVisuals();
			}
		}
		this.lastShow = flag3;
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x0001D7F2 File Offset: 0x0001B9F2
	private IEnumerator RefreshVisualsDelayed()
	{
		yield return null;
		this.backpackVisuals.RefreshVisuals();
		yield break;
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x0001D804 File Offset: 0x0001BA04
	public void StashInBackpack(Character interactor, byte backpackSlotID)
	{
		CharacterItems items = interactor.refs.items;
		if (items.currentSelectedSlot.IsNone)
		{
			Debug.LogError("Need item slot selected to stash item in backpack!");
			return;
		}
		ItemSlot itemSlot = interactor.player.GetItemSlot(items.currentSelectedSlot.Value);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Slot ID {0} is invalid!", items.currentSelectedSlot.Value));
		}
		if (itemSlot.IsEmpty())
		{
			Debug.LogError(string.Format("Item slot {0} is empty!", itemSlot.itemSlotID));
			return;
		}
		this.photonView.RPC("RPCAddItemToCharacterBackpack", 0, new object[]
		{
			interactor.player.GetComponent<PhotonView>(),
			items.currentSelectedSlot.Value,
			backpackSlotID
		});
		interactor.player.EmptySlot(items.currentSelectedSlot);
		items.EquipSlot(Optionable<byte>.None);
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x0001D8F0 File Offset: 0x0001BAF0
	[PunRPC]
	public void RPCAddItemToCharacterBackpack(PhotonView playerView, byte inventorySlotID, byte backpackSlotID)
	{
		BackpackData backpackData;
		if (!this.character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			backpackData = this.character.player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
		}
		ItemSlot itemSlot = playerView.GetComponent<Player>().GetItemSlot(inventorySlotID);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Slot ID {0} is invalid!", inventorySlotID));
			return;
		}
		backpackData.AddItem(itemSlot.prefab, itemSlot.data, backpackSlotID);
		if (PhotonNetwork.IsMasterClient)
		{
			this.backpackVisuals.RefreshVisuals();
		}
		if (this.character.IsLocal)
		{
			this.character.refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x04000565 RID: 1381
	private Character character;

	// Token: 0x04000566 RID: 1382
	private CharacterItems characterItems;

	// Token: 0x04000567 RID: 1383
	private PhotonView photonView;

	// Token: 0x04000568 RID: 1384
	public BackpackOnBackVisuals backpackVisuals;

	// Token: 0x04000569 RID: 1385
	public GameObject backpack;

	// Token: 0x0400056A RID: 1386
	private bool lastShow;

	// Token: 0x0400056B RID: 1387
	public SFX_Instance[] wearSFX;

	// Token: 0x0400056C RID: 1388
	private bool t;
}
