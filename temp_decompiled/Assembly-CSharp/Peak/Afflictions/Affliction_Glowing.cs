using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003DE RID: 990
	public class Affliction_Glowing : Affliction
	{
		// Token: 0x06001949 RID: 6473 RVA: 0x0007D86F File Offset: 0x0007BA6F
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Glowing;
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x0007D874 File Offset: 0x0007BA74
		public override void OnApplied()
		{
			base.OnApplied();
			Material material = this.character.refs.mainRenderer.materials[0];
			float @float = material.GetFloat("_Glow");
			Debug.Log(string.Format("Appling Glow to character {0}, amount {1}", this.character.gameObject.name, @float));
			material.SetFloat("_Glow", @float + 1f);
			this.pointLightInstance = Object.Instantiate<GameObject>(this.pointLightPref, this.character.GetBodypart(BodypartType.Head).transform);
			this.pointLightInstance.transform.localPosition = Vector3.zero;
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x0007D918 File Offset: 0x0007BB18
		public override void OnRemoved()
		{
			base.OnRemoved();
			Material material = this.character.refs.mainRenderer.materials[0];
			float @float = material.GetFloat("_Glow");
			Debug.Log(string.Format("Removing Glow from character {0}, amount {1}", this.character.gameObject.name, @float));
			material.SetFloat("_Glow", @float - 1f);
			Object.DestroyImmediate(this.pointLightInstance);
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x0007D98F File Offset: 0x0007BB8F
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x0007D9A8 File Offset: 0x0007BBA8
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x0007D9B6 File Offset: 0x0007BBB6
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x040016CB RID: 5835
		public GameObject pointLightPref;

		// Token: 0x040016CC RID: 5836
		private GameObject pointLightInstance;
	}
}
