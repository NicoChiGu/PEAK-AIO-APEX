using System;

// Token: 0x02000015 RID: 21
public class CharacterMovementZombie : CharacterMovement
{
	// Token: 0x060001F1 RID: 497 RVA: 0x0000F425 File Offset: 0x0000D625
	private void Awake()
	{
		this.zombie = base.GetComponent<MushroomZombie>();
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000F433 File Offset: 0x0000D633
	protected override void EvaluateGroundChecks()
	{
		base.EvaluateGroundChecks();
		if (this.zombie.currentState == MushroomZombie.State.Lunging)
		{
			this.zombie.character.data.isGrounded = false;
		}
	}

	// Token: 0x040001D4 RID: 468
	private MushroomZombie zombie;
}
