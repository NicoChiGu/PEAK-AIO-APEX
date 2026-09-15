using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003E1 RID: 993
	public class Affliction_ZombieBite : Affliction
	{
		// Token: 0x06001960 RID: 6496 RVA: 0x0007DE00 File Offset: 0x0007C000
		public override void OnApplied()
		{
			Debug.Log(string.Format("Added spores to character {0} total time: {1} delay: {2} status per second: {3}", new object[]
			{
				this.character.gameObject.name,
				this.totalTime,
				this.delayBeforeEffect,
				this.statusPerSecond
			}));
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x0007DE5F File Offset: 0x0007C05F
		public Affliction_ZombieBite(float totalTime, float delay, float statusPerSecond)
			: base(totalTime)
		{
			this.totalTime = totalTime + delay;
			this.delayBeforeEffect = delay;
			this.statusPerSecond = statusPerSecond;
		}

		// Token: 0x06001962 RID: 6498 RVA: 0x0007DE7F File Offset: 0x0007C07F
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.delayBeforeEffect);
			serializer.WriteFloat(this.statusPerSecond);
		}

		// Token: 0x06001963 RID: 6499 RVA: 0x0007DEA5 File Offset: 0x0007C0A5
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.delayBeforeEffect = serializer.ReadFloat();
			this.statusPerSecond = serializer.ReadFloat();
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x0007DECB File Offset: 0x0007C0CB
		public override void Stack(Affliction incomingAffliction)
		{
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x0007DECD File Offset: 0x0007C0CD
		public Affliction_ZombieBite()
		{
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x0007DED5 File Offset: 0x0007C0D5
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.ZombieBite;
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x0007DEDC File Offset: 0x0007C0DC
		protected override void UpdateEffect()
		{
			if (this.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Spores) < 0.025f)
			{
				this.totalTime = 0f;
				return;
			}
			if (this.timeElapsed > this.delayBeforeEffect)
			{
				this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Spores, this.statusPerSecond * Time.deltaTime, false, true, true);
			}
		}

		// Token: 0x040016D4 RID: 5844
		public float delayBeforeEffect;

		// Token: 0x040016D5 RID: 5845
		public float statusPerSecond;
	}
}
