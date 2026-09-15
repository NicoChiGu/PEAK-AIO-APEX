using System;
using System.Collections.Generic;
using Peak.Network;
using Peak.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200005D RID: 93
public class AudioLevels : MonoBehaviour
{
	// Token: 0x06000483 RID: 1155 RVA: 0x0001B4F0 File Offset: 0x000196F0
	public static void ResetSliders()
	{
		AudioLevels.PlayerAudioLevels.Clear();
		GlobalEvents.TriggerCharacterAudioLevelsUpdated();
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x0001B501 File Offset: 0x00019701
	public static float GetPlayerLevel(string playerID)
	{
		if (!AudioLevels.PlayerAudioLevels.ContainsKey(playerID))
		{
			return 0.5f;
		}
		return AudioLevels.PlayerAudioLevels[playerID];
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0001B521 File Offset: 0x00019721
	public static void SetPlayerLevel(string playerID, float f)
	{
		if (!AudioLevels.PlayerAudioLevels.ContainsKey(playerID))
		{
			AudioLevels.PlayerAudioLevels.Add(playerID, 1f);
		}
		AudioLevels.PlayerAudioLevels[playerID] = f;
		GlobalEvents.TriggerCharacterAudioLevelsUpdated();
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001B551 File Offset: 0x00019751
	private void Update()
	{
		if (this._dirty)
		{
			this.UpdateSliders();
		}
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0001B564 File Offset: 0x00019764
	public void OnEnable()
	{
		if (PhotonNetwork.OfflineMode && Application.isPlaying)
		{
			base.gameObject.SetActive(false);
		}
		this._dirty = true;
		NetCode.RoomEvents.PlayerEntered += new Action(this.OnPlayerListChanged);
		NetCode.RoomEvents.PlayerLeft += new Action(this.OnPlayerListChanged);
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0001B5BE File Offset: 0x000197BE
	public void OnDisable()
	{
		NetCode.RoomEvents.PlayerEntered -= new Action(this.OnPlayerListChanged);
		NetCode.RoomEvents.PlayerLeft -= new Action(this.OnPlayerListChanged);
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x0001B5EC File Offset: 0x000197EC
	public void OnPlayerListChanged()
	{
		this._dirty = true;
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0001B5F8 File Offset: 0x000197F8
	public void UpdateSliders()
	{
		this._dirty = false;
		Player[] playerList = PhotonNetwork.PlayerList;
		int i = 0;
		for (int j = 0; j < playerList.Length; j++)
		{
			if (this.sliders.Count > j)
			{
				this.sliders[j].Init(playerList[j]);
			}
			i = j + 1;
		}
		while (i < this.sliders.Count)
		{
			this.sliders[i].Init(null);
			i++;
		}
		this.InitNavigation();
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0001B674 File Offset: 0x00019874
	private void InitNavigation()
	{
		if (!this.mainPage)
		{
			Debug.LogWarning("Couldn't init because no pause menu main page!");
		}
		Debug.Log("initializing audio sliders.");
		bool flag = false;
		for (int i = 0; i < this.sliders.Count; i++)
		{
			if (this.sliders[i].isActiveAndEnabled && !this.sliders[i].isLocal)
			{
				object obj = i == this.sliders.Count - 1 || !this.sliders[i + 1].gameObject.activeInHierarchy;
				SelectableSlider selectableSlider = (flag ? this.sliders[i - 1].ParentContainer : null);
				object obj2 = obj;
				SelectableSlider selectableSlider2 = ((obj2 != null) ? null : this.sliders[i + 1].ParentContainer);
				this.SetSliderSelection(this.sliders[i].ParentContainer, selectableSlider, selectableSlider2);
				flag = true;
				if (obj2 != null)
				{
					Debug.Log("Pointing resume button at " + this.sliders[i].name);
					Navigation navigation = this.mainPage.resumeButton.navigation;
					navigation.mode = 4;
					navigation.selectOnRight = this.sliders[i].ParentContainer.MySelectable;
					navigation.selectOnUp = this.sliders[i].ParentContainer.MySelectable;
					this.mainPage.resumeButton.navigation = navigation;
				}
			}
		}
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001B7F4 File Offset: 0x000199F4
	private void SetSliderSelection(SelectableSlider current, SelectableSlider prev, SelectableSlider next)
	{
		Selectable selectable = ((prev != null) ? prev.MySelectable : null);
		Selectable selectable2 = ((next != null) ? next.MySelectable : null) ?? this.mainPage.resumeButton;
		Debug.Log(string.Concat(new string[]
		{
			"Setting up nav for ",
			current.name,
			". Up: ",
			(selectable != null) ? selectable.name : null,
			" | Down: ",
			(selectable2 != null) ? selectable2.name : null
		}), this);
		Selectable mySelectable = current.MySelectable;
		Navigation navigation = default(Navigation);
		navigation.mode = 4;
		navigation.selectOnUp = selectable;
		navigation.selectOnDown = selectable2;
		navigation.selectOnLeft = this.mainPage.resumeButton;
		navigation.selectOnRight = (NetCode.Session.IsHost ? current.GetComponentInChildren<KickButton>().MyButton : null);
		mySelectable.navigation = navigation;
		Selectable childSlider = current.ChildSlider;
		navigation = default(Navigation);
		navigation.mode = 4;
		navigation.selectOnUp = selectable;
		navigation.selectOnDown = selectable2;
		navigation.selectOnLeft = null;
		navigation.selectOnRight = null;
		childSlider.navigation = navigation;
	}

	// Token: 0x04000506 RID: 1286
	private bool _dirty;

	// Token: 0x04000507 RID: 1287
	public static Dictionary<string, float> PlayerAudioLevels = new Dictionary<string, float>();

	// Token: 0x04000508 RID: 1288
	public List<AudioLevelSlider> sliders;

	// Token: 0x04000509 RID: 1289
	[SerializeField]
	private PauseMenuMainPage mainPage;
}
