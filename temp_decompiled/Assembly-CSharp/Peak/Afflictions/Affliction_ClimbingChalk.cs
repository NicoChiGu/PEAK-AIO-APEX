using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003F1 RID: 1009
	public class Affliction_ClimbingChalk : Affliction
	{
		// Token: 0x060019DA RID: 6618 RVA: 0x0007EC93 File Offset: 0x0007CE93
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.ClimbingChalk;
		}

		// Token: 0x060019DB RID: 6619 RVA: 0x0007EC98 File Offset: 0x0007CE98
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
			Affliction_ClimbingChalk affliction_ClimbingChalk = incomingAffliction as Affliction_ClimbingChalk;
			if (affliction_ClimbingChalk != null)
			{
				this.climbStaminaMultiplier = Mathf.Min(affliction_ClimbingChalk.climbStaminaMultiplier, this.climbStaminaMultiplier);
			}
			this.timeElapsed = 0f;
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x0007ECE8 File Offset: 0x0007CEE8
		public override void OnApplied()
		{
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x0007ECEA File Offset: 0x0007CEEA
		public override void OnRemoved()
		{
		}

		// Token: 0x060019DE RID: 6622 RVA: 0x0007ECEC File Offset: 0x0007CEEC
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.climbStaminaMultiplier);
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x0007ED06 File Offset: 0x0007CF06
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.climbStaminaMultiplier = serializer.ReadFloat();
		}

		// Token: 0x040016E8 RID: 5864
		public float climbStaminaMultiplier;
	}
}
