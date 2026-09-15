using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200017E RID: 382
[ExecuteAlways]
public class ScoutCannonAchievementZone : MonoBehaviour
{
	// Token: 0x06000C1D RID: 3101 RVA: 0x00040DC5 File Offset: 0x0003EFC5
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x00040DEC File Offset: 0x0003EFEC
	private void Update()
	{
		this.bounds.center = base.transform.position;
		this.bounds.size = base.transform.localScale;
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x00040E1A File Offset: 0x0003F01A
	private void FixedUpdate()
	{
		if (Application.isPlaying)
		{
			this.DetectPlayerWasLaunched();
			this.TestAchievement();
		}
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x00040E30 File Offset: 0x0003F030
	private void DetectPlayerWasLaunched()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (!this.bounds.Contains(Character.localCharacter.Center))
		{
			return;
		}
		if (Character.localCharacter.data.launchedByCannon)
		{
			Debug.Log("Player launched by Cannon");
			this.playerWasLaunched = true;
		}
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x00040E84 File Offset: 0x0003F084
	private void TestAchievement()
	{
		if (this.playerWasLaunched && Character.localCharacter.data.fallSeconds <= 0f && Character.localCharacter.data.isGrounded)
		{
			this.playerWasLaunched = false;
			if (Character.localCharacter.Center.y >= this.bounds.min.y)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.DaredevilBadge);
			}
		}
	}

	// Token: 0x04000B40 RID: 2880
	public Bounds bounds;

	// Token: 0x04000B41 RID: 2881
	private bool playerWasLaunched;
}
