using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002A5 RID: 677
public class MobManager : MonoBehaviour
{
	// Token: 0x06001289 RID: 4745 RVA: 0x0005E7F9 File Offset: 0x0005C9F9
	private void Awake()
	{
		MobManager.instance = this;
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x0005E801 File Offset: 0x0005CA01
	private void Update()
	{
		if (this.mobs.Count == 0)
		{
			return;
		}
		this.mobs[this.currentIndex].TestSleepMode();
		this.currentIndex = (this.currentIndex + 1) % this.mobs.Count;
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x0005E841 File Offset: 0x0005CA41
	public void Register(Mob mob)
	{
		if (!this.mobs.Contains(mob))
		{
			this.mobs.Add(mob);
		}
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x0005E85D File Offset: 0x0005CA5D
	public void Unregister(Mob mob)
	{
		this.mobs.Remove(mob);
		if (this.currentIndex >= this.mobs.Count)
		{
			this.currentIndex = 0;
		}
	}

	// Token: 0x04001151 RID: 4433
	public static MobManager instance;

	// Token: 0x04001152 RID: 4434
	public List<Mob> mobs = new List<Mob>();

	// Token: 0x04001153 RID: 4435
	private int currentIndex;
}
