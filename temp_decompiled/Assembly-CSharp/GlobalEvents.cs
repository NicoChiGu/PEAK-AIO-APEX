using System;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000BC RID: 188
public static class GlobalEvents
{
	// Token: 0x060006FA RID: 1786 RVA: 0x00027600 File Offset: 0x00025800
	public static void TriggerItemRequested(Item interactor, Character character)
	{
		try
		{
			if (GlobalEvents.OnItemRequested != null)
			{
				GlobalEvents.OnItemRequested.Invoke(interactor, character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x0002763C File Offset: 0x0002583C
	public static void TriggerItemConsumed(Item item, Character character)
	{
		try
		{
			if (item != null && character != null)
			{
				Debug.Log(item.UIData.itemName + " consumed by " + character.gameObject.name);
			}
			if (GlobalEvents.OnItemConsumed != null)
			{
				GlobalEvents.OnItemConsumed.Invoke(item, character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x000276AC File Offset: 0x000258AC
	public static void TriggerRespawnChestOpened(RespawnChest chest, Character character)
	{
		try
		{
			if (GlobalEvents.OnRespawnChestOpened != null)
			{
				GlobalEvents.OnRespawnChestOpened.Invoke(chest, character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x000276E8 File Offset: 0x000258E8
	public static void TriggerLuggageOpened(Luggage luggage, Character character)
	{
		try
		{
			if (GlobalEvents.OnLuggageOpened != null)
			{
				GlobalEvents.OnLuggageOpened.Invoke(luggage, character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00027724 File Offset: 0x00025924
	public static void TriggerLocalCharacterWonRun()
	{
		try
		{
			if (GlobalEvents.OnLocalCharacterWonRun != null)
			{
				GlobalEvents.OnLocalCharacterWonRun.Invoke();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x0002775C File Offset: 0x0002595C
	public static void TriggerSomeoneWonRun()
	{
		try
		{
			if (GlobalEvents.OnSomeoneWonRun != null)
			{
				GlobalEvents.OnSomeoneWonRun.Invoke();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x00027794 File Offset: 0x00025994
	public static void TriggerCharacterPassedOut(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterPassedOut != null)
			{
				GlobalEvents.OnCharacterPassedOut.Invoke(character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x000277CC File Offset: 0x000259CC
	public static void TriggerCharacterDied(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterDied != null)
			{
				GlobalEvents.OnCharacterDied.Invoke(character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00027804 File Offset: 0x00025A04
	public static void TriggerLocalCharacterStatusIncremented(Character character, CharacterAfflictions.STATUSTYPE statusType, bool changeWasPositive)
	{
		try
		{
			if (GlobalEvents.OnLocalStatusIncremented != null)
			{
				GlobalEvents.OnLocalStatusIncremented.Invoke(character, statusType, changeWasPositive);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00027840 File Offset: 0x00025A40
	public static void TriggerCharacterFell(Character character, float time)
	{
		try
		{
			if (GlobalEvents.OnCharacterFell != null)
			{
				GlobalEvents.OnCharacterFell.Invoke(character, time);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x0002787C File Offset: 0x00025A7C
	public static void TriggerRunEnded()
	{
		try
		{
			if (GlobalEvents.OnRunEnded != null)
			{
				GlobalEvents.OnRunEnded.Invoke();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x000278B4 File Offset: 0x00025AB4
	public static void TriggerBugleTooted(Item bugle)
	{
		try
		{
			if (GlobalEvents.OnBugleTooted != null)
			{
				GlobalEvents.OnBugleTooted.Invoke(bugle);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x000278EC File Offset: 0x00025AEC
	public static void TriggerCharacterSpawned(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterSpawned != null)
			{
				GlobalEvents.OnCharacterSpawned.Invoke(character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00027924 File Offset: 0x00025B24
	public static void TriggerCharacterDestroyed(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterOwnerDisconnected != null)
			{
				GlobalEvents.OnCharacterOwnerDisconnected.Invoke(character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x0002795C File Offset: 0x00025B5C
	public static void TriggerCharacterAudioLevelsUpdated()
	{
		try
		{
			if (GlobalEvents.OnCharacterAudioLevelsUpdated != null)
			{
				GlobalEvents.OnCharacterAudioLevelsUpdated.Invoke();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00027994 File Offset: 0x00025B94
	public static void TriggerPlayerConnected(Player player)
	{
		try
		{
			if (GlobalEvents.OnPlayerConnected != null)
			{
				GlobalEvents.OnPlayerConnected.Invoke(player);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x000279CC File Offset: 0x00025BCC
	public static void TriggerPlayerDisconnected(Player player)
	{
		try
		{
			if (GlobalEvents.OnPlayerDisconnected != null)
			{
				GlobalEvents.OnPlayerDisconnected.Invoke(player);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00027A04 File Offset: 0x00025C04
	public static void TriggerItemThrown(Item item)
	{
		try
		{
			if (GlobalEvents.OnItemThrown != null)
			{
				GlobalEvents.OnItemThrown.Invoke(item);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x00027A3C File Offset: 0x00025C3C
	public static void TriggerAchievementThrown(ACHIEVEMENTTYPE cheevo)
	{
		try
		{
			if (GlobalEvents.OnAchievementThrown != null)
			{
				GlobalEvents.OnAchievementThrown.Invoke(cheevo);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x00027A74 File Offset: 0x00025C74
	public static void TriggerGemActivated(bool activated)
	{
		try
		{
			if (GlobalEvents.OnGemActivated != null)
			{
				GlobalEvents.OnGemActivated.Invoke(activated);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x040006E3 RID: 1763
	public static Action<Item, Character> OnItemRequested;

	// Token: 0x040006E4 RID: 1764
	public static Action<Item, Character> OnItemConsumed;

	// Token: 0x040006E5 RID: 1765
	public static Action<RespawnChest, Character> OnRespawnChestOpened;

	// Token: 0x040006E6 RID: 1766
	public static Action<Luggage, Character> OnLuggageOpened;

	// Token: 0x040006E7 RID: 1767
	public static Action OnLocalCharacterWonRun;

	// Token: 0x040006E8 RID: 1768
	public static Action OnSomeoneWonRun;

	// Token: 0x040006E9 RID: 1769
	public static Action<Character> OnCharacterPassedOut;

	// Token: 0x040006EA RID: 1770
	public static Action<Character> OnCharacterDied;

	// Token: 0x040006EB RID: 1771
	public static Action<Character, CharacterAfflictions.STATUSTYPE, bool> OnLocalStatusIncremented;

	// Token: 0x040006EC RID: 1772
	public static Action<Character, float> OnCharacterFell;

	// Token: 0x040006ED RID: 1773
	public static Action OnRunEnded;

	// Token: 0x040006EE RID: 1774
	public static Action<Item> OnBugleTooted;

	// Token: 0x040006EF RID: 1775
	public static Action<Character> OnCharacterSpawned;

	// Token: 0x040006F0 RID: 1776
	public static Action<Character> OnCharacterOwnerDisconnected;

	// Token: 0x040006F1 RID: 1777
	public static Action OnCharacterAudioLevelsUpdated;

	// Token: 0x040006F2 RID: 1778
	public static Action<Player> OnPlayerConnected;

	// Token: 0x040006F3 RID: 1779
	public static Action<Player> OnPlayerDisconnected;

	// Token: 0x040006F4 RID: 1780
	public static Action<Item> OnItemThrown;

	// Token: 0x040006F5 RID: 1781
	public static Action<ACHIEVEMENTTYPE> OnAchievementThrown;

	// Token: 0x040006F6 RID: 1782
	public static Action<bool> OnGemActivated;
}
