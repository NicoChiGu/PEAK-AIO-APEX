using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003EA RID: 1002
	public class Affliction_PreventPoisonHealing : Affliction
	{
		// Token: 0x060019A6 RID: 6566 RVA: 0x0007E79B File Offset: 0x0007C99B
		public Affliction_PreventPoisonHealing()
		{
		}

		// Token: 0x060019A7 RID: 6567 RVA: 0x0007E7A3 File Offset: 0x0007C9A3
		public Affliction_PreventPoisonHealing(float totalTime)
			: base(totalTime)
		{
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x0007E7AC File Offset: 0x0007C9AC
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.PreventPoisonHealing;
		}

		// Token: 0x060019A9 RID: 6569 RVA: 0x0007E7B0 File Offset: 0x0007C9B0
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x060019AA RID: 6570 RVA: 0x0007E7BE File Offset: 0x0007C9BE
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x0007E7CC File Offset: 0x0007C9CC
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
		}
	}
}
