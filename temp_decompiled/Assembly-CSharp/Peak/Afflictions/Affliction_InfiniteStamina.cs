using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003E2 RID: 994
	public class Affliction_InfiniteStamina : Affliction
	{
		// Token: 0x06001968 RID: 6504 RVA: 0x0007DF48 File Offset: 0x0007C148
		public Affliction_InfiniteStamina(float totalTime)
			: base(totalTime)
		{
		}

		// Token: 0x06001969 RID: 6505 RVA: 0x0007DF51 File Offset: 0x0007C151
		public Affliction_InfiniteStamina()
		{
		}

		// Token: 0x0600196A RID: 6506 RVA: 0x0007DF59 File Offset: 0x0007C159
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.InfiniteStamina;
		}

		// Token: 0x0600196B RID: 6507 RVA: 0x0007DF5C File Offset: 0x0007C15C
		public override void Stack(Affliction incomingAffliction)
		{
			Affliction_InfiniteStamina affliction_InfiniteStamina = incomingAffliction as Affliction_InfiniteStamina;
			if (affliction_InfiniteStamina != null)
			{
				this.totalTime = incomingAffliction.totalTime;
				this.timeElapsed = 0f;
				if (this.drowsyAffliction != null)
				{
					this.drowsyAffliction.totalTime += affliction_InfiniteStamina.drowsyAffliction.totalTime;
				}
			}
		}

		// Token: 0x0600196C RID: 6508 RVA: 0x0007DFAF File Offset: 0x0007C1AF
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.StartSugarRush();
			}
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x0007DFC8 File Offset: 0x0007C1C8
		public override void OnRemoved()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.EndSugarRush();
				if (this.drowsyAffliction != null)
				{
					this.character.refs.afflictions.AddAffliction(this.drowsyAffliction, false);
				}
			}
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x0007E008 File Offset: 0x0007C208
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.climbDelay);
			bool flag = this.drowsyAffliction != null;
			serializer.WriteBool(flag);
			if (flag)
			{
				this.drowsyAffliction.Serialize(serializer);
			}
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x0007E050 File Offset: 0x0007C250
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.climbDelay = serializer.ReadFloat();
			this.bonusTime = this.climbDelay;
			if (serializer.ReadBool())
			{
				this.drowsyAffliction = new Affliction_AdjustDrowsyOverTime();
				this.drowsyAffliction.Deserialize(serializer);
			}
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x0007E0A0 File Offset: 0x0007C2A0
		protected override void UpdateEffect()
		{
			this.character.AddStamina(1f);
			if (this.character.data.isClimbing)
			{
				this.climbDelay = 0f;
				this.bonusTime = 0f;
			}
		}

		// Token: 0x040016D6 RID: 5846
		[SerializeReference]
		public Affliction drowsyAffliction;

		// Token: 0x040016D7 RID: 5847
		public float climbDelay;
	}
}
