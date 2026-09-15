using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003ED RID: 1005
	public class Affliction_Invincibility : Affliction
	{
		// Token: 0x060019BB RID: 6587 RVA: 0x0007E911 File Offset: 0x0007CB11
		internal override void UpdateEffectNetworked()
		{
			this.character.refs.customization.PulseStatus(this.gold, Mathf.Clamp01(this.totalTime - this.timeElapsed));
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x0007E940 File Offset: 0x0007CB40
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Invincibility;
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x0007E944 File Offset: 0x0007CB44
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
			this.timeElapsed = 0f;
			this.character.data.RecalculateInvincibility();
			this.isFromMilk = this.isFromMilk || (incomingAffliction as Affliction_Invincibility).isFromMilk;
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x0007E994 File Offset: 0x0007CB94
		public override void OnApplied()
		{
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x0007E9A6 File Offset: 0x0007CBA6
		public override void OnRemoved()
		{
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x0007E9B8 File Offset: 0x0007CBB8
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteBool(this.isFromMilk);
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x0007E9D2 File Offset: 0x0007CBD2
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.isFromMilk = serializer.ReadBool();
		}

		// Token: 0x040016E4 RID: 5860
		private Color gold = new Color(0.9f, 0.59f, 0.035f);

		// Token: 0x040016E5 RID: 5861
		public bool isFromMilk;
	}
}
