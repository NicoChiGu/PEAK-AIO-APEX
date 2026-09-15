using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003EC RID: 1004
	public class Affliction_BingBongShield : Affliction
	{
		// Token: 0x060019B3 RID: 6579 RVA: 0x0007E86C File Offset: 0x0007CA6C
		internal override void UpdateEffectNetworked()
		{
			this.character.refs.customization.PulseStatus(this.gold, 1f);
		}

		// Token: 0x060019B4 RID: 6580 RVA: 0x0007E88E File Offset: 0x0007CA8E
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.BingBongShield;
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x0007E892 File Offset: 0x0007CA92
		public override void Stack(Affliction incomingAffliction)
		{
			this.timeElapsed = 0f;
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x0007E8AF File Offset: 0x0007CAAF
		public override void OnApplied()
		{
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x0007E8C1 File Offset: 0x0007CAC1
		public override void OnRemoved()
		{
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x0007E8D3 File Offset: 0x0007CAD3
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x0007E8E1 File Offset: 0x0007CAE1
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x040016E3 RID: 5859
		private Color gold = new Color(0.9f, 0.59f, 0.035f);
	}
}
