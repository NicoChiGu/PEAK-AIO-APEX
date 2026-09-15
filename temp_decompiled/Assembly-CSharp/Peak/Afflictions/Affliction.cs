using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003DC RID: 988
	[Serializable]
	public abstract class Affliction
	{
		// Token: 0x06001935 RID: 6453 RVA: 0x0007D624 File Offset: 0x0007B824
		public Affliction()
		{
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x0007D62C File Offset: 0x0007B82C
		public Affliction(float totalTime)
		{
			this.totalTime = totalTime;
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x0007D63C File Offset: 0x0007B83C
		public static Affliction CreateBlankAffliction(Affliction.AfflictionType afflictionType)
		{
			switch (afflictionType)
			{
			case Affliction.AfflictionType.PoisonOverTime:
				return new Affliction_PoisonOverTime();
			case Affliction.AfflictionType.InfiniteStamina:
				return new Affliction_InfiniteStamina();
			case Affliction.AfflictionType.FasterBoi:
				return new Affliction_FasterBoi();
			case Affliction.AfflictionType.Exhausted:
				return new Affliction_Exhaustion();
			case Affliction.AfflictionType.Glowing:
				return new Affliction_Glowing();
			case Affliction.AfflictionType.ColdOverTime:
				return new Affliction_AdjustColdOverTime();
			case Affliction.AfflictionType.Chaos:
				return new Affliction_Chaos();
			case Affliction.AfflictionType.AdjustStatus:
				return new Affliction_AdjustStatus();
			case Affliction.AfflictionType.ClearAllStatus:
				return new Affliction_ClearAllStatus();
			case Affliction.AfflictionType.PreventPoisonHealing:
				return new Affliction_PreventPoisonHealing();
			case Affliction.AfflictionType.AddBonusStamina:
				return new Affliction_AddBonusStamina();
			case Affliction.AfflictionType.DrowsyOverTime:
				return new Affliction_AdjustDrowsyOverTime();
			case Affliction.AfflictionType.AdjustStatusOverTime:
				return new Affliction_AdjustStatusOverTime();
			case Affliction.AfflictionType.Sunscreen:
				return new Affliction_Sunscreen();
			case Affliction.AfflictionType.BingBongShield:
				return new Affliction_BingBongShield();
			case Affliction.AfflictionType.ZombieBite:
				return new Affliction_ZombieBite();
			case Affliction.AfflictionType.Invincibility:
				return new Affliction_Invincibility();
			case Affliction.AfflictionType.LowGravity:
				return new Affliction_LowGravity();
			case Affliction.AfflictionType.Blind:
				return new Affliction_Blind();
			case Affliction.AfflictionType.Numb:
				return new Affliction_Numb();
			case Affliction.AfflictionType.ClimbingChalk:
				return new Affliction_ClimbingChalk();
			default:
				return null;
			}
		}

		// Token: 0x06001938 RID: 6456
		public abstract Affliction.AfflictionType GetAfflictionType();

		// Token: 0x06001939 RID: 6457 RVA: 0x0007D724 File Offset: 0x0007B924
		public virtual void OnApplied()
		{
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x0007D726 File Offset: 0x0007B926
		public virtual void OnRemoved()
		{
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x0600193B RID: 6459 RVA: 0x0007D728 File Offset: 0x0007B928
		public virtual bool worksOnBot
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600193C RID: 6460
		public abstract void Stack(Affliction incomingAffliction);

		// Token: 0x0600193D RID: 6461 RVA: 0x0007D72C File Offset: 0x0007B92C
		public virtual bool Tick()
		{
			if (this.bonusTime > 0f)
			{
				this.bonusTime -= Time.deltaTime;
			}
			else
			{
				this.timeElapsed += Time.deltaTime;
			}
			if (this.timeElapsed >= this.totalTime)
			{
				return true;
			}
			this.UpdateEffect();
			return false;
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x0007D783 File Offset: 0x0007B983
		protected virtual void UpdateEffect()
		{
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x0007D785 File Offset: 0x0007B985
		internal virtual void UpdateEffectNetworked()
		{
		}

		// Token: 0x06001940 RID: 6464
		public abstract void Serialize(BinarySerializer serializer);

		// Token: 0x06001941 RID: 6465
		public abstract void Deserialize(BinaryDeserializer serializer);

		// Token: 0x06001942 RID: 6466 RVA: 0x0007D788 File Offset: 0x0007B988
		public Affliction Copy()
		{
			BinarySerializer binarySerializer = new BinarySerializer(100, 2);
			Affliction affliction = Affliction.CreateBlankAffliction(this.GetAfflictionType());
			this.Serialize(binarySerializer);
			BinaryDeserializer binaryDeserializer = new BinaryDeserializer(binarySerializer);
			affliction.Deserialize(binaryDeserializer);
			binarySerializer.Dispose();
			binaryDeserializer.Dispose();
			return affliction;
		}

		// Token: 0x040016C6 RID: 5830
		public float timeElapsed;

		// Token: 0x040016C7 RID: 5831
		public float totalTime;

		// Token: 0x040016C8 RID: 5832
		protected float bonusTime;

		// Token: 0x040016C9 RID: 5833
		[HideInInspector]
		public Character character;

		// Token: 0x0200053B RID: 1339
		public enum AfflictionType
		{
			// Token: 0x04001BFD RID: 7165
			PoisonOverTime,
			// Token: 0x04001BFE RID: 7166
			InfiniteStamina,
			// Token: 0x04001BFF RID: 7167
			FasterBoi,
			// Token: 0x04001C00 RID: 7168
			Exhausted,
			// Token: 0x04001C01 RID: 7169
			Glowing,
			// Token: 0x04001C02 RID: 7170
			ColdOverTime,
			// Token: 0x04001C03 RID: 7171
			Chaos,
			// Token: 0x04001C04 RID: 7172
			AdjustStatus,
			// Token: 0x04001C05 RID: 7173
			ClearAllStatus,
			// Token: 0x04001C06 RID: 7174
			PreventPoisonHealing,
			// Token: 0x04001C07 RID: 7175
			AddBonusStamina,
			// Token: 0x04001C08 RID: 7176
			DrowsyOverTime,
			// Token: 0x04001C09 RID: 7177
			AdjustStatusOverTime,
			// Token: 0x04001C0A RID: 7178
			Sunscreen,
			// Token: 0x04001C0B RID: 7179
			BingBongShield,
			// Token: 0x04001C0C RID: 7180
			ZombieBite,
			// Token: 0x04001C0D RID: 7181
			Invincibility,
			// Token: 0x04001C0E RID: 7182
			LowGravity,
			// Token: 0x04001C0F RID: 7183
			Blind,
			// Token: 0x04001C10 RID: 7184
			Numb,
			// Token: 0x04001C11 RID: 7185
			ClimbingChalk
		}
	}
}
