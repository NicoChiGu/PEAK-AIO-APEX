using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003E0 RID: 992
	public class Affliction_PoisonOverTime : Affliction
	{
		// Token: 0x06001958 RID: 6488 RVA: 0x0007DCDC File Offset: 0x0007BEDC
		public override void OnApplied()
		{
			Debug.Log(string.Format("Added poison to character {0} total time: {1} delay: {2} status per second: {3}", new object[]
			{
				this.character.gameObject.name,
				this.totalTime,
				this.delayBeforeEffect,
				this.statusPerSecond
			}));
		}

		// Token: 0x06001959 RID: 6489 RVA: 0x0007DD3B File Offset: 0x0007BF3B
		public Affliction_PoisonOverTime(float totalTime, float delay, float statusPerSecond)
			: base(totalTime)
		{
			this.totalTime = totalTime + delay;
			this.delayBeforeEffect = delay;
			this.statusPerSecond = statusPerSecond;
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x0007DD5B File Offset: 0x0007BF5B
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.delayBeforeEffect);
			serializer.WriteFloat(this.statusPerSecond);
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x0007DD81 File Offset: 0x0007BF81
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.delayBeforeEffect = serializer.ReadFloat();
			this.statusPerSecond = serializer.ReadFloat();
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x0007DDA7 File Offset: 0x0007BFA7
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x0007DDBC File Offset: 0x0007BFBC
		public Affliction_PoisonOverTime()
		{
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x0007DDC4 File Offset: 0x0007BFC4
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.PoisonOverTime;
		}

		// Token: 0x0600195F RID: 6495 RVA: 0x0007DDC7 File Offset: 0x0007BFC7
		protected override void UpdateEffect()
		{
			if (this.timeElapsed > this.delayBeforeEffect)
			{
				this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, this.statusPerSecond * Time.deltaTime, false, true, true);
			}
		}

		// Token: 0x040016D2 RID: 5842
		public float delayBeforeEffect;

		// Token: 0x040016D3 RID: 5843
		public float statusPerSecond;
	}
}
