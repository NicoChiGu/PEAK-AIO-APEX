using System;
using Photon.Pun;

// Token: 0x020000F4 RID: 244
public class Action_ReduceUses : ItemAction
{
	// Token: 0x06000850 RID: 2128 RVA: 0x0002DBA1 File Offset: 0x0002BDA1
	public override void RunAction()
	{
		this.item.photonView.RPC("ReduceUsesRPC", 0, Array.Empty<object>());
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0002DBC0 File Offset: 0x0002BDC0
	[PunRPC]
	public void ReduceUsesRPC()
	{
		OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (data.HasData && data.Value > 0)
		{
			data.Value--;
			if (this.item.totalUses > 0)
			{
				this.item.SetUseRemainingPercentage((float)data.Value / (float)this.item.totalUses);
			}
			if (data.Value == 0 && this.consumeOnFullyUsed && base.character && base.character.IsLocal && base.character.data.currentItem == this.item)
			{
				this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			}
		}
	}

	// Token: 0x040007FE RID: 2046
	public bool consumeOnFullyUsed;
}
