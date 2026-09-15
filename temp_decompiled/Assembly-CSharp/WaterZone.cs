using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class WaterZone : SerializedMonoBehaviour
{
	// Token: 0x0600068D RID: 1677 RVA: 0x0002573F File Offset: 0x0002393F
	private void Awake()
	{
		this.zoneBounds.center = base.transform.position;
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x00025757 File Offset: 0x00023957
	private void OnEnable()
	{
		base.StartCoroutine(this.UpdateTrackedCharactersRoutine());
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x00025766 File Offset: 0x00023966
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0002576E File Offset: 0x0002396E
	private void Update()
	{
		this.UpdateCharacterVisuals();
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00025776 File Offset: 0x00023976
	private IEnumerator UpdateTrackedCharactersRoutine()
	{
		while (base.isActiveAndEnabled)
		{
			foreach (Character character in Character.AllCharacters)
			{
				if (!this.trackedCharacters.ContainsKey(character))
				{
					this.trackedCharacters.Add(character, WaterZone.InWaterState.NONE);
				}
			}
			foreach (Character character2 in Character.AllBotCharacters)
			{
				if (!this.trackedCharacters.ContainsKey(character2))
				{
					this.trackedCharacters.Add(character2, WaterZone.InWaterState.NONE);
				}
			}
			this.cachedCharacters = Enumerable.ToArray<Character>(this.trackedCharacters.Keys);
			foreach (Character character3 in this.cachedCharacters)
			{
				if (character3 == null)
				{
					yield return null;
				}
				else
				{
					if (this.zoneBounds.Contains(character3.Center))
					{
						this.trackedCharacters[character3] = WaterZone.InWaterState.Deep;
					}
					else if (this.zoneBounds.Contains(character3.GetBodypart(BodypartType.Foot_L).transform.position) || this.zoneBounds.Contains(character3.GetBodypart(BodypartType.Foot_R).transform.position))
					{
						this.trackedCharacters[character3] = WaterZone.InWaterState.Shallow;
					}
					else
					{
						this.trackedCharacters[character3] = WaterZone.InWaterState.NONE;
					}
					yield return null;
				}
			}
			Character[] array = null;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00025788 File Offset: 0x00023988
	private void UpdateCharacterVisuals()
	{
		foreach (KeyValuePair<Character, WaterZone.InWaterState> keyValuePair in this.trackedCharacters)
		{
			if (keyValuePair.Value == WaterZone.InWaterState.Deep)
			{
				keyValuePair.Key.data.inWater = this.animModDeep;
			}
			else if (keyValuePair.Value == WaterZone.InWaterState.Shallow)
			{
				keyValuePair.Key.data.inWater = this.animModShallow;
			}
		}
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00025818 File Offset: 0x00023A18
	private void FixedUpdate()
	{
		this.TryAddForceToCharacter();
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00025820 File Offset: 0x00023A20
	private void TryAddForceToCharacter()
	{
		foreach (KeyValuePair<Character, WaterZone.InWaterState> keyValuePair in this.trackedCharacters)
		{
			if (keyValuePair.Value == WaterZone.InWaterState.Deep)
			{
				keyValuePair.Key.refs.movement.SetWaterMovementModifier(this.movementModifierDeep);
			}
			else if (keyValuePair.Value == WaterZone.InWaterState.Shallow)
			{
				keyValuePair.Key.refs.movement.SetWaterMovementModifier(this.movementModifierShallow);
			}
		}
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x000258BC File Offset: 0x00023ABC
	private void OnDrawGizmosSelected()
	{
		this.zoneBounds.center = base.transform.position;
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		Gizmos.DrawCube(this.zoneBounds.center, this.zoneBounds.extents * 2f);
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(this.zoneBounds.center, this.zoneBounds.extents * 2f);
	}

	// Token: 0x04000697 RID: 1687
	public Bounds zoneBounds;

	// Token: 0x04000698 RID: 1688
	[SerializeField]
	public Dictionary<Character, WaterZone.InWaterState> trackedCharacters = new Dictionary<Character, WaterZone.InWaterState>();

	// Token: 0x04000699 RID: 1689
	public float movementModifierShallow = 0.85f;

	// Token: 0x0400069A RID: 1690
	public float movementModifierDeep = 0.7f;

	// Token: 0x0400069B RID: 1691
	public float animModDeep = 0.25f;

	// Token: 0x0400069C RID: 1692
	public float animModShallow = 0.3f;

	// Token: 0x0400069D RID: 1693
	private Character[] cachedCharacters;

	// Token: 0x02000435 RID: 1077
	public enum InWaterState
	{
		// Token: 0x04001819 RID: 6169
		NONE,
		// Token: 0x0400181A RID: 6170
		Shallow,
		// Token: 0x0400181B RID: 6171
		Deep
	}
}
