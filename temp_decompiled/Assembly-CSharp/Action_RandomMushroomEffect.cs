using System;
using System.Collections;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class Action_RandomMushroomEffect : ItemAction
{
	// Token: 0x0600084C RID: 2124 RVA: 0x0002DAC8 File Offset: 0x0002BCC8
	public override void RunAction()
	{
		int num2;
		if (!this.useDebugEffect)
		{
			if (MushroomManager.instance == null)
			{
				return;
			}
			int num = this.mushroomTypeIndex % MushroomManager.instance.mushroomEffects.Length;
			num2 = MushroomManager.instance.mushroomEffects[num];
			int num3 = MushroomManager.instance.mushroomStamAmt[num];
			base.character.AddExtraStamina((float)num3 * 0.05f);
		}
		else
		{
			num2 = this.debugEffect;
		}
		base.character.StartCoroutine(this.RunRandomEffect(num2));
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0002DB4A File Offset: 0x0002BD4A
	private IEnumerator RunRandomEffect(int effect)
	{
		Character cachedCharacter = base.character;
		Debug.Log("Triggered effect " + effect.ToString());
		switch (effect)
		{
		case 0:
		{
			yield return new WaitForSeconds(3f);
			Affliction_InfiniteStamina affliction_InfiniteStamina = new Affliction_InfiniteStamina(4f);
			cachedCharacter.refs.afflictions.AddAffliction(affliction_InfiniteStamina, false);
			yield break;
		}
		case 1:
		{
			yield return new WaitForSeconds(3f);
			Affliction_FasterBoi affliction_FasterBoi = new Affliction_FasterBoi
			{
				moveSpeedMod = 0.5f,
				climbSpeedMod = 1.5f,
				totalTime = 5f,
				climbDelay = 1f
			};
			cachedCharacter.refs.afflictions.AddAffliction(affliction_FasterBoi, false);
			yield break;
		}
		case 2:
		{
			yield return new WaitForSeconds(3f);
			Affliction_LowGravity affliction_LowGravity = new Affliction_LowGravity(3, 15f);
			cachedCharacter.refs.afflictions.AddAffliction(affliction_LowGravity, false);
			yield break;
		}
		case 3:
		{
			Affliction_Invincibility affliction_Invincibility = new Affliction_Invincibility
			{
				totalTime = 10f
			};
			cachedCharacter.refs.afflictions.AddAffliction(affliction_Invincibility, false);
			yield break;
		}
		case 4:
			cachedCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Hunger, -0.15f, false);
			cachedCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Injury, -0.15f, false);
			cachedCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Poison, -0.15f, false);
			base.character.refs.afflictions.ClearPoisonAfflictions();
			yield break;
		case 5:
			yield return new WaitForSeconds(3f);
			GameUtils.instance.SpawnResourceAtPositionNetworked("VFX_SporeExploExploEdibleSpawn", cachedCharacter.Center, 1);
			GameUtils.instance.RPC_SpawnResourceAtPosition("VFX_SporeExploExploEdibleSpawn_NoKnockback", cachedCharacter.Center);
			cachedCharacter.AddForceToBodyPart(cachedCharacter.GetBodypartRig(BodypartType.Hip), Vector3.zero, Vector3.up * this.fartForce);
			yield break;
		case 6:
		{
			yield return new WaitForSeconds(3f);
			Affliction_Blind affliction_Blind = new Affliction_Blind
			{
				totalTime = 60f
			};
			cachedCharacter.refs.afflictions.AddAffliction(affliction_Blind, false);
			yield break;
		}
		case 7:
			yield return new WaitForSeconds(3f);
			cachedCharacter.Fall(8f, 0f);
			yield break;
		case 8:
			cachedCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Spores, 0.25f, false);
			yield break;
		case 9:
		{
			yield return new WaitForSeconds(3f);
			Affliction_Numb affliction_Numb = new Affliction_Numb
			{
				totalTime = 60f
			};
			cachedCharacter.refs.afflictions.AddAffliction(affliction_Numb, false);
			yield break;
		}
		default:
			yield break;
		}
	}

	// Token: 0x040007F6 RID: 2038
	public int mushroomTypeIndex;

	// Token: 0x040007F7 RID: 2039
	public const string RESOURCE_PATH_EXPLOSION = "VFX_SporeExploExploEdibleSpawn";

	// Token: 0x040007F8 RID: 2040
	public const string RESOURCE_PATH_EXPLOSION_NO_KNOCKBACK = "VFX_SporeExploExploEdibleSpawn_NoKnockback";

	// Token: 0x040007F9 RID: 2041
	public bool useDebugEffect;

	// Token: 0x040007FA RID: 2042
	public int debugEffect;

	// Token: 0x040007FB RID: 2043
	public float fartForce = 100f;

	// Token: 0x040007FC RID: 2044
	public static int[] GoodEffects = new int[] { 0, 1, 2, 3, 4 };

	// Token: 0x040007FD RID: 2045
	public static int[] BadEffects = new int[] { 5, 6, 7, 8, 9 };
}
