using System;

// Token: 0x02000006 RID: 6
public class BackpackSlot : ItemSlot
{
	// Token: 0x0600001A RID: 26 RVA: 0x000024CD File Offset: 0x000006CD
	public BackpackSlot(byte slotID)
		: base(slotID)
	{
	}

	// Token: 0x0600001B RID: 27 RVA: 0x000024D6 File Offset: 0x000006D6
	public override void EmptyOut()
	{
		this.hasBackpack = false;
		base.EmptyOut();
	}

	// Token: 0x0600001C RID: 28 RVA: 0x000024E5 File Offset: 0x000006E5
	public override bool IsEmpty()
	{
		return !this.hasBackpack;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x000024F0 File Offset: 0x000006F0
	public override string GetPrefabName()
	{
		return "Backpack";
	}

	// Token: 0x04000006 RID: 6
	public bool hasBackpack;
}
