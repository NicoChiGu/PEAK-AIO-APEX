using System;
using UnityEngine;

// Token: 0x02000343 RID: 835
public class StickyCactus : MonoBehaviour
{
	// Token: 0x06001573 RID: 5491 RVA: 0x0006DF85 File Offset: 0x0006C185
	private void Start()
	{
		CollisionModifier component = base.GetComponent<CollisionModifier>();
		component.onCollide = (Action<Character, CollisionModifier, Collision, Bodypart>)Delegate.Combine(component.onCollide, new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide));
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x0006DFB0 File Offset: 0x0006C1B0
	private void OnCollide(Character character, CollisionModifier modifier, Collision collision, Bodypart bodypart)
	{
		if (!character.IsLocal)
		{
			return;
		}
		if (character.warping)
		{
			return;
		}
		if (character.data.isInvincible)
		{
			return;
		}
		if (character.data.isSkeleton)
		{
			return;
		}
		if (bodypart.partType == BodypartType.Head)
		{
			return;
		}
		if (bodypart.partType == BodypartType.Torso)
		{
			return;
		}
		if (bodypart.partType == BodypartType.Hip)
		{
			return;
		}
		if (character.TryStickBodypart(bodypart, collision.contacts[0].point, CharacterAfflictions.STATUSTYPE.Thorns, 0f) && this.applyThorn)
		{
			character.refs.afflictions.AddThorn(collision.contacts[0].point);
		}
	}

	// Token: 0x04001439 RID: 5177
	public bool applyThorn = true;
}
