using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000060 RID: 96
public abstract class BackpackVisuals : MonoBehaviour
{
	// Token: 0x060004AD RID: 1197
	public abstract BackpackData GetBackpackData();

	// Token: 0x060004AE RID: 1198 RVA: 0x0001BE48 File Offset: 0x0001A048
	private void OnDestroy()
	{
		foreach (ValueTuple<GameObject, ushort> valueTuple in this.visualItems.Values)
		{
			PhotonNetwork.Destroy(valueTuple.Item1);
		}
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0001BEA4 File Offset: 0x0001A0A4
	public void RefreshVisuals()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BackpackData backpackData = this.GetBackpackData();
		if (backpackData == null)
		{
			return;
		}
		for (byte b = 0; b < 4; b += 1)
		{
			ItemSlot itemSlot = backpackData.itemSlots[(int)b];
			Optionable<ushort> optionable = (itemSlot.IsEmpty() ? Optionable<ushort>.None : Optionable<ushort>.Some(itemSlot.prefab.itemID));
			ValueTuple<GameObject, ushort> valueTuple;
			Optionable<ushort> optionable2 = (this.visualItems.TryGetValue(b, ref valueTuple) ? Optionable<ushort>.Some(valueTuple.Item2) : Optionable<ushort>.None);
			if (optionable != optionable2)
			{
				if (optionable.IsSome && optionable2.IsSome)
				{
					Debug.LogError("Item Visuals Missmatch!");
				}
				else if (optionable.IsSome && optionable2.IsNone)
				{
					Debug.Log(string.Format("Spawning Backpack Visual for {0}", optionable.Value));
					GameObject gameObject = PhotonNetwork.Instantiate("0_Items/" + itemSlot.GetPrefabName(), new Vector3(0f, -500f, 0f), Quaternion.identity, 0, null);
					this.PutItemInBackpack(gameObject, b);
					gameObject.GetComponent<PhotonView>().RPC("SetItemInstanceDataRPC", 0, new object[] { itemSlot.data });
					this.visualItems.Add(b, new ValueTuple<GameObject, ushort>(gameObject, optionable.Value));
				}
				else if (optionable.IsNone || optionable2.IsSome)
				{
					Debug.Log(string.Format("Removing backpack visual for {0}", optionable2.Value));
					ValueTuple<GameObject, ushort> valueTuple2;
					if (!this.visualItems.TryGetValue(b, ref valueTuple2))
					{
						Debug.LogError(string.Format("Failed to get spawned object from slotID {0}", b));
					}
					PhotonView component = valueTuple2.Item1.GetComponent<PhotonView>();
					Debug.Log(string.Format("Destroying photon view: {0}", component));
					PhotonNetwork.Destroy(component);
					this.visualItems.Remove(b);
				}
				else
				{
					Debug.LogError("Should be unreachable");
				}
			}
			else if (optionable.IsNone)
			{
				Debug.Log(string.Format("Not Spawning backpack visual for slot id: {0} because it's empty...", b));
			}
		}
	}

	// Token: 0x060004B0 RID: 1200
	protected abstract void PutItemInBackpack(GameObject visual, byte slotID);

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001C0B2 File Offset: 0x0001A2B2
	private void OnApplicationQuit()
	{
		this.m_shuttingDown = true;
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001C0BC File Offset: 0x0001A2BC
	public void RemoveVisuals()
	{
		if (this.m_shuttingDown)
		{
			return;
		}
		foreach (ValueTuple<GameObject, ushort> valueTuple in this.visualItems.Values)
		{
			GameObject item = valueTuple.Item1;
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(item);
			}
			else
			{
				item.gameObject.SetActive(false);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.visualItems.Clear();
		}
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001C148 File Offset: 0x0001A348
	public bool TryGetSpawnedItem(byte slotID, out Item item)
	{
		return this.spawnedVisualItems.TryGetValue(slotID, ref item) && item != null;
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001C163 File Offset: 0x0001A363
	public void SetSpawnedBackpackItem(byte slotID, Item item)
	{
		this.spawnedVisualItems[slotID] = item;
	}

	// Token: 0x0400051E RID: 1310
	public Transform[] backpackSlots;

	// Token: 0x0400051F RID: 1311
	private Dictionary<byte, ValueTuple<GameObject, ushort>> visualItems = new Dictionary<byte, ValueTuple<GameObject, ushort>>();

	// Token: 0x04000520 RID: 1312
	private Dictionary<byte, Item> spawnedVisualItems = new Dictionary<byte, Item>();

	// Token: 0x04000521 RID: 1313
	protected bool m_shuttingDown;
}
