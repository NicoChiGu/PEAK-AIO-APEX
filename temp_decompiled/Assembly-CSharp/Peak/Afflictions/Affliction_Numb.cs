using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003EF RID: 1007
	public class Affliction_Numb : Affliction
	{
		// Token: 0x060019CA RID: 6602 RVA: 0x0007EAB2 File Offset: 0x0007CCB2
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Numb;
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x0007EAB6 File Offset: 0x0007CCB6
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
			this.timeElapsed = 0f;
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x0007EACF File Offset: 0x0007CCCF
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.StartNumb();
			}
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x0007EAE8 File Offset: 0x0007CCE8
		public override void OnRemoved()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.StopNumb();
			}
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x0007EB01 File Offset: 0x0007CD01
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x0007EB0F File Offset: 0x0007CD0F
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}
	}
}
