using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003E8 RID: 1000
	public class Affliction_AdjustStatusOverTime : Affliction
	{
		// Token: 0x0600199B RID: 6555 RVA: 0x0007E4D5 File Offset: 0x0007C6D5
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.AdjustStatusOverTime;
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x0007E4DC File Offset: 0x0007C6DC
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
			Affliction_AdjustStatusOverTime affliction_AdjustStatusOverTime = incomingAffliction as Affliction_AdjustStatusOverTime;
			if (affliction_AdjustStatusOverTime != null)
			{
				this.statusPerSecond = Mathf.Max(affliction_AdjustStatusOverTime.statusPerSecond, this.statusPerSecond);
			}
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x0007E51D File Offset: 0x0007C71D
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.statusPerSecond);
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x0007E537 File Offset: 0x0007C737
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.statusPerSecond = serializer.ReadFloat();
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x040016DD RID: 5853
		public float statusPerSecond;
	}
}
