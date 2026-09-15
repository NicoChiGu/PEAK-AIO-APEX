using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003DD RID: 989
	public class Affliction_Exhaustion : Affliction
	{
		// Token: 0x06001943 RID: 6467 RVA: 0x0007D7CA File Offset: 0x0007B9CA
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Exhausted;
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x0007D7D0 File Offset: 0x0007B9D0
		protected override void UpdateEffect()
		{
			base.UpdateEffect();
			float num = this.drainAmount / this.totalTime * Time.deltaTime;
			this.character.UseStamina(num, true);
			Debug.Log(string.Format("Exhausterd: {0}", num));
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x0007D81A File Offset: 0x0007BA1A
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.timeElapsed, incomingAffliction.totalTime);
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x0007D833 File Offset: 0x0007BA33
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.drainAmount);
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x0007D84D File Offset: 0x0007BA4D
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.drainAmount = serializer.ReadFloat();
		}

		// Token: 0x040016CA RID: 5834
		public float drainAmount;
	}
}
