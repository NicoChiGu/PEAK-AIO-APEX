using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003E3 RID: 995
	public class Affliction_AdjustStatus : Affliction
	{
		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06001971 RID: 6513 RVA: 0x0007E0DA File Offset: 0x0007C2DA
		public override bool worksOnBot
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x0007E0DD File Offset: 0x0007C2DD
		public Affliction_AdjustStatus()
		{
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x0007E0E5 File Offset: 0x0007C2E5
		public Affliction_AdjustStatus(CharacterAfflictions.STATUSTYPE statusType, float statusAmount, float totalTime)
			: base(totalTime)
		{
			this.statusType = statusType;
			this.statusAmount = statusAmount;
		}

		// Token: 0x06001974 RID: 6516 RVA: 0x0007E0FC File Offset: 0x0007C2FC
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.AdjustStatus;
		}

		// Token: 0x06001975 RID: 6517 RVA: 0x0007E0FF File Offset: 0x0007C2FF
		public override void Stack(Affliction incomingAffliction)
		{
			this.OnApplied();
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x0007E107 File Offset: 0x0007C307
		public override void OnApplied()
		{
			if (this.character.photonView.IsMine)
			{
				this.character.refs.afflictions.AdjustStatus(this.statusType, this.statusAmount, false);
			}
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x0007E140 File Offset: 0x0007C340
		public override void Serialize(BinarySerializer serializer)
		{
			Debug.Log("Serializing int");
			serializer.WriteInt((int)this.statusType);
			Debug.Log("Serializing float");
			serializer.WriteFloat(this.statusAmount);
			Debug.Log("Serializing float");
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x0007E190 File Offset: 0x0007C390
		public override void Deserialize(BinaryDeserializer serializer)
		{
			Debug.Log("Deserializing int");
			this.statusType = (CharacterAfflictions.STATUSTYPE)serializer.ReadInt();
			Debug.Log("Deserializing float");
			this.statusAmount = serializer.ReadFloat();
			Debug.Log("Deserializing float");
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x040016D8 RID: 5848
		public CharacterAfflictions.STATUSTYPE statusType;

		// Token: 0x040016D9 RID: 5849
		public float statusAmount;
	}
}
