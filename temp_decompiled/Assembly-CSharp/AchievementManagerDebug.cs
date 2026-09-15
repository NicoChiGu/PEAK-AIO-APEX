using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class AchievementManagerDebug : SerializedMonoBehaviour
{
	// Token: 0x06000418 RID: 1048 RVA: 0x0001A0B3 File Offset: 0x000182B3
	private void Awake()
	{
		this.achievementManager = base.GetComponent<AchievementManager>();
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x0001A0C1 File Offset: 0x000182C1
	private void Update()
	{
		this.runBasedInts = this.achievementManager.runBasedValueData.runBasedInts;
	}

	// Token: 0x0400049B RID: 1179
	private AchievementManager achievementManager;

	// Token: 0x0400049C RID: 1180
	[SerializeField]
	public Dictionary<RUNBASEDVALUETYPE, int> runBasedInts = new Dictionary<RUNBASEDVALUETYPE, int>();
}
