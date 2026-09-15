using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000353 RID: 851
public class TombTrigger : MonoBehaviour
{
	// Token: 0x060015CF RID: 5583 RVA: 0x00070698 File Offset: 0x0006E898
	private void OnTriggerEnter(Collider other)
	{
		Debug.LogError("Attempting tomb trigger");
		Character componentInParent = other.GetComponentInParent<Character>();
		if (componentInParent != null && componentInParent == Character.localCharacter && !this.triggered)
		{
			this.TriggerTomb();
		}
		if (componentInParent != null && componentInParent == Character.localCharacter && Character.localCharacter.GetComponent<CharacterAnimations>() && Character.localCharacter.GetComponent<CharacterAnimations>().ambienceAudio)
		{
			Character.localCharacter.GetComponent<CharacterAnimations>().ambienceAudio.inTomb = true;
		}
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x0007072C File Offset: 0x0006E92C
	private void TriggerTomb()
	{
		this.triggered = true;
		GUIManager.instance.SetHeroTitle(Singleton<MountainProgressHandler>.Instance.tombProgressPoint.localizedTitle, Singleton<MountainProgressHandler>.Instance.tombProgressPoint.clip);
	}

	// Token: 0x040014A9 RID: 5289
	private bool triggered;
}
