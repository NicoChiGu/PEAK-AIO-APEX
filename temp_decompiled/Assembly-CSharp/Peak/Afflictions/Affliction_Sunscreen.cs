using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003E6 RID: 998
	public class Affliction_Sunscreen : Affliction
	{
		// Token: 0x06001989 RID: 6537 RVA: 0x0007E351 File Offset: 0x0007C551
		public Affliction_Sunscreen()
		{
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x0007E359 File Offset: 0x0007C559
		public Affliction_Sunscreen(float totalTime)
			: base(totalTime)
		{
			this.totalTime = totalTime;
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x0007E369 File Offset: 0x0007C569
		protected override void UpdateEffect()
		{
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x0007E36B File Offset: 0x0007C56B
		internal override void UpdateEffectNetworked()
		{
			this.character.refs.customization.PulseStatus(Color.white, 1f);
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x0007E38C File Offset: 0x0007C58C
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x0007E39A File Offset: 0x0007C59A
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x0007E3A8 File Offset: 0x0007C5A8
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Sunscreen;
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x0007E3AC File Offset: 0x0007C5AC
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				this.character.data.wearingSunscreen = true;
				GUIManager.instance.StartSunscreen();
			}
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x0007E3D6 File Offset: 0x0007C5D6
		public override void OnRemoved()
		{
			if (this.character.IsLocal)
			{
				this.character.data.wearingSunscreen = false;
				GUIManager.instance.EndSunscreen();
			}
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x0007E400 File Offset: 0x0007C600
		public override void Stack(Affliction incomingAffliction)
		{
			this.timeElapsed = 0f;
		}
	}
}
