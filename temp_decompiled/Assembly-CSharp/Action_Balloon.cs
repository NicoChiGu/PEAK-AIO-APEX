using System;

// Token: 0x020000DB RID: 219
public class Action_Balloon : ItemAction
{
	// Token: 0x06000810 RID: 2064 RVA: 0x0002D124 File Offset: 0x0002B324
	public override void RunAction()
	{
		if (base.character)
		{
			if (this.balloon.isBunch)
			{
				base.character.refs.balloons.TieNewBalloon(0);
				base.character.refs.balloons.TieNewBalloon(2);
				base.character.refs.balloons.TieNewBalloon(4);
				return;
			}
			base.character.refs.balloons.TieNewBalloon(this.balloon.colorIndex);
		}
	}

	// Token: 0x040007D5 RID: 2005
	public Balloon balloon;
}
