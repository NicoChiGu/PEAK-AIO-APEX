using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003E7 RID: 999
	public class Affliction_AdjustDrowsyOverTime : Affliction
	{
		// Token: 0x06001993 RID: 6547 RVA: 0x0007E40D File Offset: 0x0007C60D
		public Affliction_AdjustDrowsyOverTime()
		{
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0007E415 File Offset: 0x0007C615
		public Affliction_AdjustDrowsyOverTime(float statusPerSecond, float totalTime)
			: base(totalTime)
		{
			this.statusPerSecond = statusPerSecond;
			this.totalTime = totalTime;
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x0007E42C File Offset: 0x0007C62C
		protected override void UpdateEffect()
		{
			this.character.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.statusPerSecond * Time.deltaTime, false);
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x0007E451 File Offset: 0x0007C651
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.statusPerSecond);
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x0007E46B File Offset: 0x0007C66B
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.statusPerSecond = serializer.ReadFloat();
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x06001998 RID: 6552 RVA: 0x0007E485 File Offset: 0x0007C685
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.DrowsyOverTime;
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x0007E48C File Offset: 0x0007C68C
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
			Affliction_AdjustDrowsyOverTime affliction_AdjustDrowsyOverTime = incomingAffliction as Affliction_AdjustDrowsyOverTime;
			if (affliction_AdjustDrowsyOverTime != null)
			{
				this.statusPerSecond = Mathf.Max(affliction_AdjustDrowsyOverTime.statusPerSecond, this.statusPerSecond);
			}
		}

		// Token: 0x040016DC RID: 5852
		public float statusPerSecond;
	}
}
