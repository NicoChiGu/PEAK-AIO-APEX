using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class MushroomZombieBiteCollider : MonoBehaviour
{
	// Token: 0x06000338 RID: 824 RVA: 0x0001634C File Offset: 0x0001454C
	private void OnTriggerEnter(Collider other)
	{
		if (Time.time - this.lastBitLocalCharacter < 5f)
		{
			return;
		}
		Character character;
		if (CharacterRagdoll.TryGetCharacterFromCollider(other, out character) && character.IsLocal)
		{
			this.lastBitLocalCharacter = Time.time;
			if (character.data.isSkeleton)
			{
				character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, this.parentZombie.biteInitialInjury / 8f * 2f, false, true, true);
			}
			else
			{
				character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, this.parentZombie.biteInitialInjury, false, true, true);
			}
			character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Spores, this.parentZombie.biteInitialSpores, false, true, true);
			Affliction_ZombieBite affliction_ZombieBite = new Affliction_ZombieBite(this.parentZombie.totalBiteSporesTime, this.parentZombie.biteDelayBeforeSpores, this.parentZombie.biteSporesPerSecond);
			character.refs.afflictions.AddAffliction(affliction_ZombieBite, false);
			character.Fall(this.parentZombie.biteStunTime, 0f);
			this.parentZombie.OnBitCharacter(character);
		}
	}

	// Token: 0x040002FE RID: 766
	public MushroomZombie parentZombie;

	// Token: 0x040002FF RID: 767
	public float lastBitLocalCharacter;
}
