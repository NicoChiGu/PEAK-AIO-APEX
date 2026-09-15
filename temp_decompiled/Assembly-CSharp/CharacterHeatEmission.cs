using System;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class CharacterHeatEmission : MonoBehaviour
{
	// Token: 0x06000183 RID: 387 RVA: 0x0000AE0D File Offset: 0x0000900D
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x06000184 RID: 388 RVA: 0x0000AE1C File Offset: 0x0000901C
	public void Update()
	{
		base.transform.position = this.character.refs.hip.transform.position;
		if (this.character.data.sinceAddedCold < 3f)
		{
			return;
		}
		this.counter += Time.deltaTime;
		if (this.counter < this.rate)
		{
			return;
		}
		this.counter = 0f;
		foreach (Character character in Character.AllCharacters)
		{
			if (Vector3.Distance(base.transform.position, character.Center) < this.radius)
			{
				character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, this.heatAmount, false, false);
			}
		}
	}

	// Token: 0x06000185 RID: 389 RVA: 0x0000AF08 File Offset: 0x00009108
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x0400014D RID: 333
	public float radius = 1f;

	// Token: 0x0400014E RID: 334
	public float heatAmount = 0.05f;

	// Token: 0x0400014F RID: 335
	public float rate = 0.5f;

	// Token: 0x04000150 RID: 336
	private float counter;

	// Token: 0x04000151 RID: 337
	private Character character;
}
