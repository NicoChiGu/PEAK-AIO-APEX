using System;
using System.Linq;
using Peak.Network;
using Unity.Multiplayer.Playmode;

// Token: 0x02000168 RID: 360
public class RichPresenceService : GameService, IDisposable
{
	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x06000B66 RID: 2918 RVA: 0x0003C9E2 File Offset: 0x0003ABE2
	[Obsolete("Rich presence functionality is encapsulated in the IRichPresence interface now.")]
	public RichPresenceState m_currentState
	{
		get
		{
			return this._presence.State;
		}
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x0003C9F0 File Offset: 0x0003ABF0
	public RichPresenceService()
	{
		IRichPresence richPresence2;
		if (!Enumerable.Contains<string>(CurrentPlayer.ReadOnlyTags(), "NoSteam"))
		{
			IRichPresence richPresence = new SteamRichPresence();
			richPresence2 = richPresence;
		}
		else
		{
			IRichPresence richPresence = new NoRichPresence();
			richPresence2 = richPresence;
		}
		this._presence = richPresence2;
		NetCode.RoomEvents.PlayerEntered += new Action(this.Dirty);
		NetCode.RoomEvents.PlayerLeft += new Action(this.Dirty);
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x0003CA56 File Offset: 0x0003AC56
	public void Dispose()
	{
		NetCode.RoomEvents.PlayerEntered -= new Action(this.Dirty);
		NetCode.RoomEvents.PlayerLeft -= new Action(this.Dirty);
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x0003CA84 File Offset: 0x0003AC84
	public void SetState(RichPresenceState state)
	{
		this._presence.SetState(state);
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x0003CA92 File Offset: 0x0003AC92
	public void Dirty()
	{
		this._presence.SetState(this._presence.State);
	}

	// Token: 0x04000A86 RID: 2694
	private IRichPresence _presence;
}
