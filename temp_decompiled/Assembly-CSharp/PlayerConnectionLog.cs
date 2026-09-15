using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class PlayerConnectionLog : MonoBehaviourPunCallbacks
{
	// Token: 0x06000EB7 RID: 3767 RVA: 0x00048184 File Offset: 0x00046384
	private void Awake()
	{
		GlobalEvents.OnAchievementThrown = (Action<ACHIEVEMENTTYPE>)Delegate.Combine(GlobalEvents.OnAchievementThrown, new Action<ACHIEVEMENTTYPE>(this.TestAchievementThrown));
		GlobalEvents.OnGemActivated = (Action<bool>)Delegate.Combine(GlobalEvents.OnGemActivated, new Action<bool>(this.TestGemActivated));
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x000481D4 File Offset: 0x000463D4
	private void OnDestroy()
	{
		GlobalEvents.OnAchievementThrown = (Action<ACHIEVEMENTTYPE>)Delegate.Remove(GlobalEvents.OnAchievementThrown, new Action<ACHIEVEMENTTYPE>(this.TestAchievementThrown));
		GlobalEvents.OnGemActivated = (Action<bool>)Delegate.Remove(GlobalEvents.OnGemActivated, new Action<bool>(this.TestGemActivated));
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x00048224 File Offset: 0x00046424
	private void RebuildString()
	{
		this.sb.Clear();
		foreach (string text in this.currentLog)
		{
			this.sb.Append(text);
			this.sb.Append("\n");
		}
		this.text.text = this.sb.ToString();
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x000482B0 File Offset: 0x000464B0
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (newPlayer.IsLocal)
		{
			return;
		}
		string text = NetworkingUtilities.Sanitize(newPlayer.NickName);
		if (text.Length > 32)
		{
			text = text.Substring(0, 32);
		}
		newPlayer.NickName = text;
		string text2 = this.GetColorTag(this.userColor) + " " + newPlayer.NickName + "</color>";
		string text3 = this.GetColorTag(this.joinedColor) + LocalizedText.GetText("JOINEDTHEEXPEDITION", true).Replace("#", text2) + "</color>";
		this.AddMessage(text3);
		if (this.sfxJoin)
		{
			this.sfxJoin.Play(default(Vector3));
		}
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x00048364 File Offset: 0x00046564
	public override void OnPlayerLeftRoom(Player newPlayer)
	{
		if (!newPlayer.IsLocal)
		{
			if (newPlayer.NickName == "Bing Bong")
			{
				return;
			}
			string text = this.GetColorTag(this.userColor) + " " + newPlayer.NickName + "</color>";
			string text2 = this.GetColorTag(this.leftColor) + LocalizedText.GetText("LEFTTHEEXPEDITION", true).Replace("#", text) + "</color>";
			this.AddMessage(text2);
			if (this.sfxLeave)
			{
				this.sfxLeave.Play(default(Vector3));
			}
		}
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x00048408 File Offset: 0x00046608
	public void TestAddJoin()
	{
		string text = this.GetColorTag(this.userColor) + " TESTPLAYER</color>";
		string text2 = this.GetColorTag(this.joinedColor) + LocalizedText.GetText("JOINEDTHEEXPEDITION", true).Replace("#", text) + "</color>";
		this.AddMessage(text2);
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x00048460 File Offset: 0x00046660
	public void TestAddLeft()
	{
		string text = this.GetColorTag(this.userColor) + " TESTPLAYER</color>";
		string text2 = this.GetColorTag(this.leftColor) + LocalizedText.GetText("LEFTTHEEXPEDITION", true).Replace("#", text) + "</color>";
		this.AddMessage(text2);
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x000484B8 File Offset: 0x000466B8
	private string GetColorTag(Color c)
	{
		return "<color=#" + ColorUtility.ToHtmlStringRGB(c) + ">";
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x000484CF File Offset: 0x000466CF
	private void AddMessage(string s)
	{
		this.currentLog.Add(s);
		Debug.Log("Message sent to player log: " + s);
		this.RebuildString();
		base.StartCoroutine(this.TimeoutMessageRoutine());
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x00048500 File Offset: 0x00046700
	private IEnumerator TimeoutMessageRoutine()
	{
		yield return new WaitForSeconds(8f);
		this.currentLog.RemoveAt(0);
		this.RebuildString();
		yield break;
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x00048510 File Offset: 0x00046710
	private void TestAchievementThrown(ACHIEVEMENTTYPE type)
	{
		if (Application.isEditor || Debug.isDebugBuild)
		{
			string text = this.GetColorTag(this.userColor) + " " + type.ToString() + "</color>";
			string text2 = this.GetColorTag(this.joinedColor) + "Got Badge: </color>" + text;
			this.AddMessage(text2);
		}
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x00048574 File Offset: 0x00046774
	private void TestGemActivated(bool activated)
	{
		string text = LocalizedText.GetText(activated ? "GEM_ACTIVATED" : "GEM_DEACTIVATED", true);
		this.AddMessage(this.GetColorTag(this.gemColor) + " " + text + "</color>");
	}

	// Token: 0x04000CA0 RID: 3232
	public TextMeshProUGUI text;

	// Token: 0x04000CA1 RID: 3233
	private List<string> currentLog = new List<string>();

	// Token: 0x04000CA2 RID: 3234
	private StringBuilder sb = new StringBuilder();

	// Token: 0x04000CA3 RID: 3235
	public Color joinedColor;

	// Token: 0x04000CA4 RID: 3236
	public Color leftColor;

	// Token: 0x04000CA5 RID: 3237
	public Color userColor;

	// Token: 0x04000CA6 RID: 3238
	public Color gemColor;

	// Token: 0x04000CA7 RID: 3239
	public SFX_Instance sfxJoin;

	// Token: 0x04000CA8 RID: 3240
	public SFX_Instance sfxLeave;
}
