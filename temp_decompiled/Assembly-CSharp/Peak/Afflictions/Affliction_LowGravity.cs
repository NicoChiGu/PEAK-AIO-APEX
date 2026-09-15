using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003F0 RID: 1008
	public class Affliction_LowGravity : Affliction
	{
		// Token: 0x060019D1 RID: 6609 RVA: 0x0007EB25 File Offset: 0x0007CD25
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.LowGravity;
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x0007EB29 File Offset: 0x0007CD29
		public Affliction_LowGravity()
		{
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x0007EB38 File Offset: 0x0007CD38
		public Affliction_LowGravity(int lowGravAmount, float totalTime)
			: base(totalTime)
		{
			this.lowGravAmount = lowGravAmount;
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x0007EB50 File Offset: 0x0007CD50
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
			Affliction_LowGravity affliction_LowGravity = incomingAffliction as Affliction_LowGravity;
			if (affliction_LowGravity != null)
			{
				this.lowGravAmount = Mathf.Max(affliction_LowGravity.lowGravAmount, this.lowGravAmount);
			}
			this.timeElapsed = 0f;
			this.character.data.RecalculateLowGrav();
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x0007EBA8 File Offset: 0x0007CDA8
		public override bool Tick()
		{
			if (this.timeElapsed + 2f > this.totalTime)
			{
				if (!this.warning)
				{
					this.character.refs.afflictions.WarnStopWhirlwind();
					this.warning = true;
				}
			}
			else
			{
				this.warning = false;
			}
			return base.Tick();
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x0007EBFC File Offset: 0x0007CDFC
		public override void OnApplied()
		{
			this.character.data.RecalculateLowGrav();
			this.character.refs.afflictions.StartWhirlwind();
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x0007EC23 File Offset: 0x0007CE23
		public override void OnRemoved()
		{
			this.character.data.RecalculateLowGrav();
			this.character.refs.afflictions.StopWhirlwind();
			this.character.data.sinceGrounded = 0f;
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x0007EC5F File Offset: 0x0007CE5F
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteInt(this.lowGravAmount);
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x0007EC79 File Offset: 0x0007CE79
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.lowGravAmount = serializer.ReadInt();
		}

		// Token: 0x040016E6 RID: 5862
		public int lowGravAmount = 1;

		// Token: 0x040016E7 RID: 5863
		private bool warning;
	}
}
