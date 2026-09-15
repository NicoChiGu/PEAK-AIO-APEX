using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000303 RID: 771
[Serializable]
public abstract class PostSpawnBehavior : IMayHaveDeferredStep
{
	// Token: 0x1700013C RID: 316
	// (get) Token: 0x060013FA RID: 5114 RVA: 0x00065223 File Offset: 0x00063423
	protected virtual DeferredStepTiming DefaultTiming
	{
		get
		{
			return DeferredStepTiming.None;
		}
	}

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x060013FB RID: 5115 RVA: 0x00065226 File Offset: 0x00063426
	public virtual DeferredStepTiming DeferredTiming
	{
		get
		{
			if (this._timing != DeferredStepTiming.None)
			{
				return this._timing;
			}
			return this.DefaultTiming;
		}
	}

	// Token: 0x060013FC RID: 5116
	public abstract void RunBehavior(IEnumerable<GameObject> spawned);

	// Token: 0x060013FD RID: 5117 RVA: 0x00065240 File Offset: 0x00063440
	public IDeferredStep ConstructDeferred(IMayHaveDeferredStep parent)
	{
		if (this.DeferredTiming == DeferredStepTiming.None)
		{
			Debug.LogError(string.Format("Can't construct a deferred execution if timing is {0}", DeferredStepTiming.None));
			return null;
		}
		if (this.mute)
		{
			Debug.LogError("Can't construct a deferred execution of a muted step");
			return null;
		}
		PropSpawner propSpawner = parent as PropSpawner;
		if (propSpawner == null)
		{
			Debug.LogError("Assumed we could only get here from a prop spawning parent. Did something change?");
			return null;
		}
		return new PostSpawnBehavior.PostSpawnBehaviorDeferredRunner(this, propSpawner.syncTransforms, propSpawner.SpawnedProps);
	}

	// Token: 0x0400128D RID: 4749
	[SerializeField]
	protected DeferredStepTiming _timing;

	// Token: 0x0400128E RID: 4750
	public bool mute;

	// Token: 0x02000500 RID: 1280
	public readonly struct PostSpawnBehaviorDeferredRunner : IDeferredStep
	{
		// Token: 0x06001D48 RID: 7496 RVA: 0x00087A0F File Offset: 0x00085C0F
		public PostSpawnBehaviorDeferredRunner(PostSpawnBehavior behavior, bool syncTransforms, IEnumerable<GameObject> spawnedObjects)
		{
			this._syncTransforms = syncTransforms;
			this._spawnedObjects = Enumerable.ToList<GameObject>(spawnedObjects);
			this._behavior = behavior;
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x00087A2B File Offset: 0x00085C2B
		public void DeferredGo()
		{
			if (this._syncTransforms)
			{
				Physics.SyncTransforms();
			}
			this._behavior.RunBehavior(this._spawnedObjects);
		}

		// Token: 0x04001B3E RID: 6974
		private readonly bool _syncTransforms;

		// Token: 0x04001B3F RID: 6975
		private readonly List<GameObject> _spawnedObjects;

		// Token: 0x04001B40 RID: 6976
		private readonly PostSpawnBehavior _behavior;
	}
}
