using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003E5 RID: 997
	public class Affliction_AdjustColdOverTime : Affliction
	{
		// Token: 0x06001980 RID: 6528 RVA: 0x0007E248 File Offset: 0x0007C448
		public Affliction_AdjustColdOverTime()
		{
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x0007E250 File Offset: 0x0007C450
		public Affliction_AdjustColdOverTime(float statusPerSecond, float totalTime)
			: base(totalTime)
		{
			this.statusPerSecond = statusPerSecond;
			this.totalTime = totalTime;
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x0007E267 File Offset: 0x0007C467
		protected override void UpdateEffect()
		{
			this.character.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Cold, this.statusPerSecond * Time.deltaTime, false);
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x0007E28C File Offset: 0x0007C48C
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.statusPerSecond);
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x0007E2A6 File Offset: 0x0007C4A6
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.statusPerSecond = serializer.ReadFloat();
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x0007E2C0 File Offset: 0x0007C4C0
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.ColdOverTime;
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x0007E2C3 File Offset: 0x0007C4C3
		public override void OnApplied()
		{
			if (this.character.IsLocal && this.statusPerSecond < 0f)
			{
				GUIManager.instance.StartHeat();
			}
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x0007E2E9 File Offset: 0x0007C4E9
		public override void OnRemoved()
		{
			if (this.character.IsLocal && this.statusPerSecond < 0f)
			{
				GUIManager.instance.EndHeat();
			}
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x0007E310 File Offset: 0x0007C510
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
			Affliction_AdjustColdOverTime affliction_AdjustColdOverTime = incomingAffliction as Affliction_AdjustColdOverTime;
			if (affliction_AdjustColdOverTime != null)
			{
				this.statusPerSecond = Mathf.Max(affliction_AdjustColdOverTime.statusPerSecond, this.statusPerSecond);
			}
		}

		// Token: 0x040016DB RID: 5851
		public float statusPerSecond;
	}
}
