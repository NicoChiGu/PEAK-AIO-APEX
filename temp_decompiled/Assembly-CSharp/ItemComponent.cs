using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200011B RID: 283
public abstract class ItemComponent : MonoBehaviourPunCallbacks
{
	// Token: 0x06000905 RID: 2309 RVA: 0x0003071C File Offset: 0x0002E91C
	public virtual void Awake()
	{
		this.item = base.GetComponent<Item>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00030736 File Offset: 0x0002E936
	public T GetData<T>(DataEntryKey key) where T : DataEntryValue, new()
	{
		return this.item.GetData<T>(key);
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00030744 File Offset: 0x0002E944
	public T GetData<T>(DataEntryKey key, Func<T> getNew) where T : DataEntryValue, new()
	{
		return this.item.GetData<T>(key, getNew);
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00030753 File Offset: 0x0002E953
	public bool HasData(DataEntryKey key)
	{
		return this.item.data != null && this.item.data.HasData(key);
	}

	// Token: 0x06000909 RID: 2313
	public abstract void OnInstanceDataSet();

	// Token: 0x0600090A RID: 2314 RVA: 0x00030775 File Offset: 0x0002E975
	public void ForceSync()
	{
		if (!this.photonView.IsMine)
		{
			Debug.LogError("Not allowed to force sync an object you don't own..");
			return;
		}
		this.photonView.RPC("SetItemInstanceDataRPC", 1, new object[] { this.item.data });
	}

	// Token: 0x04000885 RID: 2181
	[NonSerialized]
	public Item item;

	// Token: 0x04000886 RID: 2182
	protected PhotonView photonView;
}
