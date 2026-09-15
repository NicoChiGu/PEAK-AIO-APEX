using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000194 RID: 404
public class LobbyTypeSetting : CustomLocalizedEnumSetting<LobbyTypeSetting.LobbyType>, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000CB5 RID: 3253 RVA: 0x0004194E File Offset: 0x0003FB4E
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x00041950 File Offset: 0x0003FB50
	public override void Load(ISettingsSaveLoad loader)
	{
		base.Load(loader);
		if (!PlayerPrefs.HasKey("DEFAULT_CHANGE_LOBBY_TYPE"))
		{
			base.Value = this.GetDefaultValue();
		}
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x00041971 File Offset: 0x0003FB71
	public override void Save(ISettingsSaveLoad saver)
	{
		base.Save(saver);
		PlayerPrefs.SetInt("DEFAULT_CHANGE_LOBBY_TYPE", 1);
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x00041985 File Offset: 0x0003FB85
	protected override LobbyTypeSetting.LobbyType GetDefaultValue()
	{
		return LobbyTypeSetting.LobbyType.InviteOnly;
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x00041988 File Offset: 0x0003FB88
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x0004198B File Offset: 0x0003FB8B
	public override List<string> GetUnlocalizedChoices()
	{
		List<string> list = new List<string>();
		list.Add("Friends");
		list.Add("Invite Only");
		return list;
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x000419A8 File Offset: 0x0003FBA8
	public string GetDisplayName()
	{
		return "Lobby Mode";
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x000419AF File Offset: 0x0003FBAF
	public string GetCategory()
	{
		return "General";
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x000419B6 File Offset: 0x0003FBB6
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}

	// Token: 0x02000491 RID: 1169
	public enum LobbyType
	{
		// Token: 0x04001996 RID: 6550
		Friends,
		// Token: 0x04001997 RID: 6551
		InviteOnly
	}
}
