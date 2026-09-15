using System;

// Token: 0x020000EA RID: 234
public class Action_LightLantern : ItemAction
{
	// Token: 0x06000839 RID: 2105 RVA: 0x0002D7E4 File Offset: 0x0002B9E4
	private void Awake()
	{
		this.lantern = base.GetComponent<Lantern>();
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0002D7F2 File Offset: 0x0002B9F2
	public override void RunAction()
	{
		this.lantern.ToggleLantern();
	}

	// Token: 0x040007E9 RID: 2025
	private Lantern lantern;
}
