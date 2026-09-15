using System;
using Peak.Network;
using UnityEngine;

// Token: 0x0200035E RID: 862
public class UIPlayerNames : MonoBehaviour
{
	// Token: 0x06001609 RID: 5641 RVA: 0x00071AC8 File Offset: 0x0006FCC8
	public int Init(CharacterInteractible characterInteractable)
	{
		this.indexCounter++;
		this.playerNameText[this.indexCounter - 1].characterInteractable = characterInteractable;
		this.playerNameText[this.indexCounter - 1].text.text = characterInteractable.GetName();
		for (int i = 0; i < this.playerNameText.Length; i++)
		{
			this.playerNameText[i].gameObject.SetActive(false);
		}
		this.localCannibalismSetting = GameHandler.Instance.SettingsHandler.GetSetting<CannibalismSetting>();
		return this.indexCounter - 1;
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x00071B5C File Offset: 0x0006FD5C
	public void UpdateName(int index, Vector3 position, bool visible, int speakingAmplitude)
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (index >= this.playerNameText.Length)
		{
			return;
		}
		this.playerNameText[index].transform.position = MainCamera.instance.cam.WorldToScreenPoint(position);
		if (visible)
		{
			if (this.CanCannibalize(this.playerNameText[index].characterInteractable.character))
			{
				this.playerNameText[index].characterInteractable.character.refs.customization.BecomeChicken();
			}
			this.playerNameText[index].gameObject.SetActive(true);
			this.playerNameText[index].group.alpha = Mathf.MoveTowards(this.playerNameText[index].group.alpha, 1f, Time.deltaTime * 5f);
			if (this.playerNameText[index].characterInteractable && AudioLevels.GetPlayerLevel(this.playerNameText[index].characterInteractable.character.player.GetUserId()) == 0f)
			{
				this.playerNameText[index].audioImage.sprite = this.mutedAudioSprite;
				return;
			}
			if (speakingAmplitude <= 0)
			{
				this.playerNameText[index].audioImageTimeout -= Time.deltaTime;
				if (this.playerNameText[index].audioImageTimeout <= 0f)
				{
					this.playerNameText[index].audioImage.sprite = this.audioSprites[0];
				}
			}
			else
			{
				this.playerNameText[index].audioImage.sprite = this.audioSprites[Mathf.Clamp(speakingAmplitude, 0, this.audioSprites.Length - 1)];
				this.playerNameText[index].audioImageTimeout = this.audioImageTimeoutMax;
			}
		}
		else
		{
			this.playerNameText[index].group.alpha = Mathf.MoveTowards(this.playerNameText[index].group.alpha, 0f, Time.deltaTime * 5f);
			if (this.playerNameText[index].group.alpha < 0.01f && this.playerNameText[index].gameObject.activeSelf)
			{
				this.playerNameText[index].characterInteractable.character.refs.customization.BecomeHuman();
				this.playerNameText[index].gameObject.SetActive(false);
			}
		}
		if ((Character.localCharacter.data.fullyPassedOut || Character.localCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hunger) < UIPlayerNames.CANNIBAL_HUNGER_THRESHOLD) && this.playerNameText[index].gameObject.activeSelf)
		{
			this.playerNameText[index].characterInteractable.character.refs.customization.BecomeHuman();
		}
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x00071E1C File Offset: 0x0007001C
	private bool CanCannibalize(Character otherCharacter)
	{
		return !otherCharacter.isBot && (!otherCharacter.refs.customization.isCannibalizable && Character.localCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hunger) >= UIPlayerNames.CANNIBAL_HUNGER_THRESHOLD && Character.localCharacter.data.fullyConscious && this.localCannibalismSetting.Value == 1 && otherCharacter.data.cannibalismPermitted);
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x00071E90 File Offset: 0x00070090
	public void DisableName(int index)
	{
		if (this.playerNameText[index])
		{
			this.playerNameText[index].gameObject.SetActive(false);
		}
	}

	// Token: 0x040014EA RID: 5354
	private int indexCounter;

	// Token: 0x040014EB RID: 5355
	public PlayerName[] playerNameText;

	// Token: 0x040014EC RID: 5356
	public Sprite[] audioSprites;

	// Token: 0x040014ED RID: 5357
	public Sprite mutedAudioSprite;

	// Token: 0x040014EE RID: 5358
	public CannibalismSetting localCannibalismSetting;

	// Token: 0x040014EF RID: 5359
	public float audioImageTimeoutMax = 1f;

	// Token: 0x040014F0 RID: 5360
	public static float CANNIBAL_HUNGER_THRESHOLD = 0.7f;
}
