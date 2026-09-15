using System;

// Token: 0x020000E0 RID: 224
public class Action_Consume : ItemAction
{
	// Token: 0x0600081E RID: 2078 RVA: 0x0002D456 File Offset: 0x0002B656
	public override void RunAction()
	{
		if (base.character)
		{
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
		}
	}
}
