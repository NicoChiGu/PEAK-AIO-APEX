using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;

namespace Peak.Dev
{
	// Token: 0x020003D8 RID: 984
	public static class PlayerLoopHelper
	{
		// Token: 0x06001931 RID: 6449 RVA: 0x0007D490 File Offset: 0x0007B690
		public static PlayerLoopSystem InsertSystemAfter<T>(this PlayerLoopSystem self, in PlayerLoopSystem insertedSystem) where T : struct
		{
			PlayerLoopSystem playerLoopSystem = default(PlayerLoopSystem);
			playerLoopSystem.loopConditionFunction = self.loopConditionFunction;
			playerLoopSystem.type = self.type;
			playerLoopSystem.updateDelegate = self.updateDelegate;
			playerLoopSystem.updateFunction = self.updateFunction;
			PlayerLoopSystem playerLoopSystem2 = playerLoopSystem;
			List<PlayerLoopSystem> list = new List<PlayerLoopSystem>();
			if (self.subSystemList != null)
			{
				for (int i = 0; i < self.subSystemList.Length; i++)
				{
					list.Add(self.subSystemList[i]);
					if (self.subSystemList[i].type == typeof(T))
					{
						list.Add(insertedSystem);
					}
				}
			}
			playerLoopSystem2.subSystemList = list.ToArray();
			return playerLoopSystem2;
		}
	}
}
