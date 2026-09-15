using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003EB RID: 1003
	public class Affliction_AddBonusStamina : Affliction
	{
		// Token: 0x060019AC RID: 6572 RVA: 0x0007E7DA File Offset: 0x0007C9DA
		public Affliction_AddBonusStamina()
		{
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x0007E7E2 File Offset: 0x0007C9E2
		public Affliction_AddBonusStamina(float staminaAmount, float totalTime)
			: base(totalTime)
		{
			this.staminaAmount = staminaAmount;
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x0007E7F2 File Offset: 0x0007C9F2
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.AddBonusStamina;
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x0007E7F6 File Offset: 0x0007C9F6
		public override void Stack(Affliction incomingAffliction)
		{
			this.OnApplied();
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x0007E7FE File Offset: 0x0007C9FE
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				Debug.Log("Adding extra stamina: " + this.staminaAmount.ToString());
				this.character.AddExtraStamina(this.staminaAmount);
			}
		}

		// Token: 0x060019B1 RID: 6577 RVA: 0x0007E838 File Offset: 0x0007CA38
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.staminaAmount);
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x060019B2 RID: 6578 RVA: 0x0007E852 File Offset: 0x0007CA52
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.staminaAmount = serializer.ReadFloat();
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x040016E2 RID: 5858
		public float staminaAmount;
	}
}
