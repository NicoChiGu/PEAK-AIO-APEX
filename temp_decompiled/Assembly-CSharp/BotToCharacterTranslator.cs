using System;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class BotToCharacterTranslator : MonoBehaviour
{
	// Token: 0x060004EE RID: 1262 RVA: 0x0001D240 File Offset: 0x0001B440
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
		this.bot = base.GetComponentInParent<Bot>();
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x0001D25C File Offset: 0x0001B45C
	private void Update()
	{
		this.character.input.movementInput = this.bot.MovementInput;
		this.character.input.sprintIsPressed = this.bot.IsSprinting;
		this.character.data.lookValues = HelperFunctions.DirectionToLook(this.bot.LookDirection);
	}

	// Token: 0x04000558 RID: 1368
	private Character character;

	// Token: 0x04000559 RID: 1369
	private Bot bot;
}
