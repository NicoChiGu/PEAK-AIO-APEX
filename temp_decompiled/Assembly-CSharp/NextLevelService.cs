using System;
using UnityEngine;
using Zorro.Core;
using Zorro.UI.Modal;

// Token: 0x02000150 RID: 336
public class NextLevelService : GameService
{
	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x06000AC9 RID: 2761 RVA: 0x00039CDC File Offset: 0x00037EDC
	private NextLevelService.NextLevelData FallbackData
	{
		get
		{
			NextLevelService.NextLevelData nextLevelData = this._offlineDataFallback.GetValueOrDefault();
			if (this._offlineDataFallback == null)
			{
				nextLevelData = NextLevelService.CreateFallbackData();
				this._offlineDataFallback = new NextLevelService.NextLevelData?(nextLevelData);
			}
			return this._offlineDataFallback.Value;
		}
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000ACA RID: 2762 RVA: 0x00039D20 File Offset: 0x00037F20
	public int SecondsLeftFallback
	{
		get
		{
			return this.FallbackData.SecondsLeft;
		}
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06000ACB RID: 2763 RVA: 0x00039D3B File Offset: 0x00037F3B
	public bool HasReceivedLevelIndex
	{
		get
		{
			return this.Data.IsSome;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06000ACC RID: 2764 RVA: 0x00039D48 File Offset: 0x00037F48
	public int NextLevelIndexOrFallback
	{
		get
		{
			if (!this.HasReceivedLevelIndex)
			{
				return this.OfflineLevelIndex;
			}
			return this.Data.Value.CurrentLevelIndex;
		}
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x00039D6C File Offset: 0x00037F6C
	public void NewData(LoginResponse response)
	{
		this.Data = Optionable<NextLevelService.NextLevelData>.Some(new NextLevelService.NextLevelData(response));
		Debug.Log("Setting new NextLevelData: " + this.Data.Value.ToString());
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x00039DB4 File Offset: 0x00037FB4
	private static NextLevelService.NextLevelData CreateFallbackData()
	{
		DateTime dateTime;
		dateTime..ctor(2025, 6, 14, 17, 0, 0);
		double num = (double)((int)Math.Floor((DateTime.UtcNow - dateTime).TotalHours));
		int num2 = 24;
		int num3 = (int)Math.Floor(num / (double)num2);
		TimeSpan timeSpan = dateTime.AddHours((double)(num3 * num2)).AddHours((double)num2) - DateTime.UtcNow;
		int num4 = (int)Math.Floor(timeSpan.TotalHours);
		int minutes = timeSpan.Minutes;
		int seconds = timeSpan.Seconds;
		return new NextLevelService.NextLevelData(new LoginResponse
		{
			VersionOkay = true,
			HoursUntilLevel = num4,
			MinutesUntilLevel = minutes,
			SecondsUntilLevel = seconds,
			LevelIndex = num3,
			Message = string.Empty
		});
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000ACF RID: 2767 RVA: 0x00039E79 File Offset: 0x00038079
	public int OfflineLevelIndex
	{
		get
		{
			return this.FallbackData.CurrentLevelIndex;
		}
	}

	// Token: 0x04000A11 RID: 2577
	public static int debugLevelIndexOffset;

	// Token: 0x04000A12 RID: 2578
	public Optionable<NextLevelService.NextLevelData> Data;

	// Token: 0x04000A13 RID: 2579
	private NextLevelService.NextLevelData? _offlineDataFallback;

	// Token: 0x02000475 RID: 1141
	public struct NextLevelData
	{
		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06001B52 RID: 6994 RVA: 0x00082364 File Offset: 0x00080564
		public int SecondsLeft
		{
			get
			{
				float num = Time.realtimeSinceStartup - this.StartupTimeWhenQueried;
				float num2 = this.SecondsLeftFromQueryTime - num;
				QueryingGameTimeStatus queryingGameTimeStatus;
				if (num2 < 0f && !GameHandler.TryGetStatus<QueryingGameTimeStatus>(out queryingGameTimeStatus))
				{
					CloudAPI.CheckVersion(delegate(LoginResponse response)
					{
						GameHandler.GetService<NextLevelService>().NewData(response);
						if (!response.VersionOkay)
						{
							Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_OUTOFDATE_TITLE", true), LocalizedText.GetText("MODAL_OUTOFDATE_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
							{
								new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
							}), new Action(Application.Quit));
						}
					});
				}
				return Mathf.RoundToInt(num2);
			}
		}

		// Token: 0x06001B53 RID: 6995 RVA: 0x000823C0 File Offset: 0x000805C0
		public NextLevelData(LoginResponse login)
		{
			this.CurrentLevelIndex = login.LevelIndex;
			this.StartupTimeWhenQueried = Time.realtimeSinceStartup;
			float num = (float)(login.HoursUntilLevel * 60 * 60 + login.MinutesUntilLevel * 60 + login.SecondsUntilLevel);
			this.SecondsLeftFromQueryTime = num;
			this.DevMessage = login.Message;
		}

		// Token: 0x06001B54 RID: 6996 RVA: 0x00082416 File Offset: 0x00080616
		public override string ToString()
		{
			return string.Format("CurrentIndex: {0}, seconds left {1}", this.CurrentLevelIndex, this.SecondsLeft);
		}

		// Token: 0x04001938 RID: 6456
		public int CurrentLevelIndex;

		// Token: 0x04001939 RID: 6457
		public float StartupTimeWhenQueried;

		// Token: 0x0400193A RID: 6458
		public float SecondsLeftFromQueryTime;

		// Token: 0x0400193B RID: 6459
		public string DevMessage;
	}
}
