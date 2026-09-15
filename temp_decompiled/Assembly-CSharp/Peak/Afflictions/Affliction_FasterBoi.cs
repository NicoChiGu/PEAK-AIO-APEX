using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003DF RID: 991
	public class Affliction_FasterBoi : Affliction
	{
		// Token: 0x06001950 RID: 6480 RVA: 0x0007D9CC File Offset: 0x0007BBCC
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.FasterBoi;
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x0007D9CF File Offset: 0x0007BBCF
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x0007D9E8 File Offset: 0x0007BBE8
		protected override void UpdateEffect()
		{
			if (this.character.data.isClimbing)
			{
				this.climbDelay = 0f;
				this.bonusTime = 0f;
			}
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x0007DA14 File Offset: 0x0007BC14
		public override void OnApplied()
		{
			base.OnApplied();
			this.character.refs.movement.movementModifier += this.moveSpeedMod;
			this.character.refs.climbing.climbSpeedMod += this.climbSpeedMod;
			this.character.refs.ropeHandling.climbSpeedMod += this.climbSpeedMod;
			this.character.refs.vineClimbing.climbSpeedMod += this.climbSpeedMod;
			if (this.character.IsLocal)
			{
				GUIManager.instance.StartEnergyDrink();
			}
			this.cachedDrowsy = this.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy);
			this.character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 2f, false, false);
			this.bonusTime = this.climbDelay;
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x0007DB0C File Offset: 0x0007BD0C
		public override void OnRemoved()
		{
			base.OnRemoved();
			this.character.refs.movement.movementModifier -= this.moveSpeedMod;
			this.character.refs.climbing.climbSpeedMod -= this.climbSpeedMod;
			this.character.refs.ropeHandling.climbSpeedMod -= this.climbSpeedMod;
			this.character.refs.vineClimbing.climbSpeedMod -= this.climbSpeedMod;
			this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.cachedDrowsy + this.drowsyOnEnd, false, true, true);
			if (this.character.IsLocal)
			{
				GUIManager.instance.EndEnergyDrink();
			}
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x0007DBE8 File Offset: 0x0007BDE8
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.moveSpeedMod);
			serializer.WriteFloat(this.climbSpeedMod);
			serializer.WriteFloat(this.drowsyOnEnd);
			serializer.WriteFloat(this.cachedDrowsy);
			serializer.WriteFloat(this.climbDelay);
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x0007DC40 File Offset: 0x0007BE40
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.moveSpeedMod = serializer.ReadFloat();
			this.climbSpeedMod = serializer.ReadFloat();
			this.drowsyOnEnd = serializer.ReadFloat();
			this.cachedDrowsy = serializer.ReadFloat();
			this.climbDelay = serializer.ReadFloat();
			this.bonusTime = this.climbDelay;
			Debug.Log("Bonus time set to " + this.bonusTime.ToString());
		}

		// Token: 0x040016CD RID: 5837
		public float moveSpeedMod = 1f;

		// Token: 0x040016CE RID: 5838
		public float climbSpeedMod = 1f;

		// Token: 0x040016CF RID: 5839
		public float drowsyOnEnd;

		// Token: 0x040016D0 RID: 5840
		private float cachedDrowsy;

		// Token: 0x040016D1 RID: 5841
		public float climbDelay;
	}
}
