using System;

// Token: 0x020000E9 RID: 233
public class Action_LaunchPlayer : ItemAction
{
	// Token: 0x06000837 RID: 2103 RVA: 0x0002D7AB File Offset: 0x0002B9AB
	public override void RunAction()
	{
		base.character.AddForce(MainCamera.instance.transform.forward * this.force, 1f, 1f);
	}

	// Token: 0x040007E8 RID: 2024
	public float force;
}
