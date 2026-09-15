using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003E4 RID: 996
	public class Affliction_ClearAllStatus : Affliction
	{
		// Token: 0x06001979 RID: 6521 RVA: 0x0007E1DF File Offset: 0x0007C3DF
		public Affliction_ClearAllStatus()
		{
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x0007E1E7 File Offset: 0x0007C3E7
		public Affliction_ClearAllStatus(bool excludeCurse, float totalTime)
			: base(totalTime)
		{
			this.excludeCurse = excludeCurse;
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x0007E1F7 File Offset: 0x0007C3F7
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.ClearAllStatus;
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x0007E1FA File Offset: 0x0007C3FA
		public override void Stack(Affliction incomingAffliction)
		{
			this.OnApplied();
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x0007E202 File Offset: 0x0007C402
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				this.character.refs.afflictions.ClearAllStatus(this.excludeCurse);
			}
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x0007E22C File Offset: 0x0007C42C
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteBool(this.excludeCurse);
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x0007E23A File Offset: 0x0007C43A
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.excludeCurse = serializer.ReadBool();
		}

		// Token: 0x040016DA RID: 5850
		public bool excludeCurse;
	}
}
