using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000178 RID: 376
public class RopeSyncer : PhotonBinaryStreamSerializer<RopeSyncData>
{
	// Token: 0x06000BE7 RID: 3047 RVA: 0x0003FA1A File Offset: 0x0003DC1A
	protected override void Awake()
	{
		if (!this.rope)
		{
			this.rope = base.GetComponent<Rope>();
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x0003FA38 File Offset: 0x0003DC38
	public override RopeSyncData GetDataToWrite()
	{
		RopeSyncData syncData = this.rope.GetSyncData();
		syncData.updateVisualizerManually = this.updateVisualizerManually;
		return syncData;
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x0003FA5F File Offset: 0x0003DC5F
	public override void OnDataReceived(RopeSyncData data)
	{
		base.OnDataReceived(data);
		this.rope.SetSyncData(data);
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x0003FA74 File Offset: 0x0003DC74
	public override bool ShouldSendData()
	{
		if (this.rope.isClimbable && this.startSyncTime.IsNone)
		{
			this.startSyncTime = Optionable<float>.Some(Time.realtimeSinceStartup);
		}
		List<Transform> ropeSegments = this.rope.GetRopeSegments();
		if (ropeSegments.Count == 0)
		{
			return false;
		}
		Vector3 position = ropeSegments[0].position;
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		if (allPlayerCharacters.Count == 0)
		{
			return false;
		}
		float num = float.MaxValue;
		foreach (Character character in allPlayerCharacters)
		{
			float num2 = Vector3.Distance(character.Center, position);
			num = Mathf.Min(num, num2);
		}
		if (num > 100f)
		{
			return false;
		}
		if (this.startSyncTime.IsSome && Time.realtimeSinceStartup - this.startSyncTime.Value > 60f)
		{
			this.updateVisualizerManually = true;
			this.syncIndex++;
			if (this.syncIndex < 600)
			{
				return false;
			}
			this.syncIndex = 0;
		}
		return !this.rope.creatorLeft;
	}

	// Token: 0x04000B08 RID: 2824
	public Rope rope;

	// Token: 0x04000B09 RID: 2825
	public Optionable<float> startSyncTime = Optionable<float>.None;

	// Token: 0x04000B0A RID: 2826
	private int syncIndex;

	// Token: 0x04000B0B RID: 2827
	private bool updateVisualizerManually;
}
