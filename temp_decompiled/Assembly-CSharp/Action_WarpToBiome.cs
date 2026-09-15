using System;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class Action_WarpToBiome : ItemAction
{
	// Token: 0x0600086F RID: 2159 RVA: 0x0002E261 File Offset: 0x0002C461
	public override void RunAction()
	{
		Debug.Log("WARP TO " + this.segmentToWarpTo.ToString());
		MapHandler.JumpToSegment(this.segmentToWarpTo);
	}

	// Token: 0x04000819 RID: 2073
	public Segment segmentToWarpTo;
}
