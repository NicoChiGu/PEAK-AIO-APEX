using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003EE RID: 1006
	public class Affliction_Blind : Affliction
	{
		// Token: 0x060019C3 RID: 6595 RVA: 0x0007EA0E File Offset: 0x0007CC0E
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Blind;
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x0007EA12 File Offset: 0x0007CC12
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
			this.timeElapsed = 0f;
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x0007EA2B File Offset: 0x0007CC2B
		public override void OnApplied()
		{
			this.character.AddIllegalStatus("BLIND", 60f);
			this.character.refs.customization.refs.blindRenderer.gameObject.SetActive(true);
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x0007EA67 File Offset: 0x0007CC67
		public override void OnRemoved()
		{
			this.character.refs.customization.refs.blindRenderer.gameObject.SetActive(false);
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x0007EA8E File Offset: 0x0007CC8E
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x0007EA9C File Offset: 0x0007CC9C
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}
	}
}
