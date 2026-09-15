using System;
using UnityEngine;

// Token: 0x0200031C RID: 796
public class SandStoneClimb : MonoBehaviour
{
	// Token: 0x06001478 RID: 5240 RVA: 0x00068054 File Offset: 0x00066254
	private void Start()
	{
		ClimbModifierSurface component = base.GetComponent<ClimbModifierSurface>();
		component.onClimbAction = (Action<Character>)Delegate.Combine(component.onClimbAction, new Action<Character>(this.OnClimb));
		CollisionModifier component2 = base.GetComponent<CollisionModifier>();
		component2.onCollide = (Action<Character, CollisionModifier, Collision, Bodypart>)Delegate.Combine(component2.onCollide, new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide));
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x000680AF File Offset: 0x000662AF
	private void OnCollide(Character character, CollisionModifier modifier, Collision collision, Bodypart bodypart)
	{
		base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, Vector3.zero, Time.deltaTime * 0.05f);
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x000680DC File Offset: 0x000662DC
	private void OnClimb(Character character)
	{
		base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, Vector3.zero, Time.deltaTime * 0.1f);
	}
}
