using System;
using TMPro;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class BadgeManager : MonoBehaviour
{
	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06000DA9 RID: 3497 RVA: 0x00044972 File Offset: 0x00042B72
	// (set) Token: 0x06000DAA RID: 3498 RVA: 0x0004497C File Offset: 0x00042B7C
	public BadgeUI selectedBadge
	{
		get
		{
			return this._selectedBadge;
		}
		set
		{
			this._selectedBadge = value;
			if (this._selectedBadge != null && this._selectedBadge.data != null)
			{
				if (this._selectedBadge.data.IsLocked)
				{
					this.badgePopupName.text = "???";
					this.badgePopupDescription.text = LocalizedText.GetText(LocalizedText.GetDescriptionIndex(this._selectedBadge.data.displayName), true);
				}
				else
				{
					this.badgePopupName.text = LocalizedText.GetText(LocalizedText.GetNameIndex(this._selectedBadge.data.displayName), true);
					this.badgePopupDescription.text = LocalizedText.GetText(LocalizedText.GetDescriptionIndex(this._selectedBadge.data.displayName), true);
				}
			}
			else
			{
				this.badgePopupName.text = "???";
				this.badgePopupDescription.text = LocalizedText.GetText("BADGELOCKED", true);
			}
			this.badgePopupAnim.Play("Popup", 0, 0f);
		}
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x00044A8B File Offset: 0x00042C8B
	public void InheritData(BadgeManager other)
	{
		this.badgeData = new BadgeData[other.badgeData.Length];
		other.badgeData.CopyTo(this.badgeData, 0);
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00044AB2 File Offset: 0x00042CB2
	private void OnEnable()
	{
		this.selectedBadge = null;
		if (this.initBadgesOnEnable)
		{
			this.InitBadges();
		}
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x00044ACC File Offset: 0x00042CCC
	public BadgeData GetBadgeData(ACHIEVEMENTTYPE achievementType)
	{
		foreach (BadgeData badgeData in this.badgeData)
		{
			if (!(badgeData == null) && badgeData.linkedAchievement == achievementType)
			{
				return badgeData;
			}
		}
		return null;
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00044B08 File Offset: 0x00042D08
	private void InitBadges()
	{
		this.badges = base.GetComponentsInChildren<BadgeUI>();
		for (int i = 0; i < this.badges.Length; i++)
		{
			if (i < this.badgeData.Length)
			{
				this.badges[i].Init(this.badgeData[i]);
			}
			else
			{
				this.badges[i].Init(null);
			}
		}
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00044B64 File Offset: 0x00042D64
	private void Update()
	{
		this.badgePopup.SetActive(this.selectedBadge != null);
		if (this.selectedBadge)
		{
			this.badgePopup.transform.position = this.selectedBadge.transform.position;
		}
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x00044BB8 File Offset: 0x00042DB8
	public void AddAllToCSV()
	{
		for (int i = 0; i < this.badgeData.Length; i++)
		{
			this.badgeData[i].AddToCSV();
		}
	}

	// Token: 0x04000BC3 RID: 3011
	private BadgeUI _selectedBadge;

	// Token: 0x04000BC4 RID: 3012
	public GameObject badgePopup;

	// Token: 0x04000BC5 RID: 3013
	public Animator badgePopupAnim;

	// Token: 0x04000BC6 RID: 3014
	public TextMeshProUGUI badgePopupName;

	// Token: 0x04000BC7 RID: 3015
	public TextMeshProUGUI badgePopupDescription;

	// Token: 0x04000BC8 RID: 3016
	public BadgeData[] badgeData;

	// Token: 0x04000BC9 RID: 3017
	private BadgeUI[] badges;

	// Token: 0x04000BCA RID: 3018
	public bool initBadgesOnEnable;
}
