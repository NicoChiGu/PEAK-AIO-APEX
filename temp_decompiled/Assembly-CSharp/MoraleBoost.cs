using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000142 RID: 322
public class MoraleBoost
{
	// Token: 0x06000A64 RID: 2660 RVA: 0x00037228 File Offset: 0x00035428
	public static bool SpawnMoraleBoost(Vector3 origin, float radius, float baselineStaminaBoost, float staminaBoostPerAdditionalScout, bool sendToAll = false, int minScouts = 1)
	{
		List<Character> list = new List<Character>();
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			Character character = Character.AllCharacters[i];
			if (radius == -1f || Vector3.Distance(character.Center, origin) <= radius)
			{
				list.Add(character);
			}
		}
		if (list.Count < minScouts)
		{
			return false;
		}
		Debug.Log(string.Format("Creating morale boost. Characters in radius: {0} total boost: {1}", list.Count, baselineStaminaBoost));
		foreach (Character character2 in list)
		{
			if (sendToAll)
			{
				character2.photonView.RPC("MoraleBoost", 0, new object[] { baselineStaminaBoost, list.Count });
			}
			else
			{
				character2.MoraleBoost(baselineStaminaBoost, list.Count);
			}
		}
		return true;
	}
}
