using System;
using UnityEngine;

// Token: 0x02000333 RID: 819
public class SkeletonExplosion : MonoBehaviour
{
	// Token: 0x0600151D RID: 5405 RVA: 0x0006BD00 File Offset: 0x00069F00
	public void Boom(Character character)
	{
		base.transform.forward = character.GetBodypart(BodypartType.Hip).transform.forward;
		base.transform.position = character.Center;
		foreach (Rigidbody rigidbody in this.rb)
		{
			rigidbody.AddExplosionForce(this.force * Random.Range(this.randomForceRange.x, this.randomForceRange.y), this.explosionOrigin.position, this.radius, this.upwardsModifier, 1);
			Object.Destroy(rigidbody.gameObject, 10f + Random.value);
		}
	}

	// Token: 0x040013AE RID: 5038
	public Transform explosionOrigin;

	// Token: 0x040013AF RID: 5039
	public Rigidbody[] rb;

	// Token: 0x040013B0 RID: 5040
	public float force;

	// Token: 0x040013B1 RID: 5041
	public Vector2 randomForceRange;

	// Token: 0x040013B2 RID: 5042
	public float radius;

	// Token: 0x040013B3 RID: 5043
	public float upwardsModifier;
}
