using System;
using UnityEngine;
using WebSocketSharp;

// Token: 0x02000277 RID: 631
public class ItemAudioManager : MonoBehaviour
{
	// Token: 0x060011A7 RID: 4519 RVA: 0x00058DA3 File Offset: 0x00056FA3
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x060011A8 RID: 4520 RVA: 0x00058DBC File Offset: 0x00056FBC
	private void Update()
	{
		this.character.refs.animator.SetBool("Eat", false);
		this.character.refs.animator.SetBool("Heal", false);
		this.character.refs.animator.SetBool("Drink", false);
		this.character.refs.animator.SetBool("Antidote", false);
		this.throwCharge.volume = Mathf.Lerp(this.throwCharge.volume, 0f, Time.deltaTime * 5f);
		this.throwCharge.pitch = Mathf.Lerp(this.throwCharge.pitch, 1f, Time.deltaTime * 5f);
		if (!string.IsNullOrEmpty(this.prevUse) && !Ext.IsNullOrEmpty(this.prevUse))
		{
			this.character.refs.animator.SetBool(this.prevUse, false);
		}
		if (!this.character.data.currentItem && !string.IsNullOrEmpty(this.prevUse))
		{
			this.character.refs.animator.SetBool(this.prevUse, false);
		}
		if (this.character.refs.animator.GetBool("Consumed Item"))
		{
			this.finishTimer -= Time.deltaTime;
		}
		else
		{
			this.finishTimer = 0.25f;
		}
		if (this.finishTimer <= 0f)
		{
			this.character.refs.animator.SetBool("Consumed Item", false);
		}
		if (this.character.data.currentItem)
		{
			if (this.character.refs.items.throwChargeLevel > 0f)
			{
				this.throwCharge.volume = Mathf.Lerp(this.throwCharge.volume, 0.3f, Time.deltaTime * 10f);
				this.throwCharge.pitch = Mathf.Lerp(this.throwCharge.pitch, 2f + this.character.refs.items.throwChargeLevel * 3f, Time.deltaTime * 10f);
			}
			if (this.prevItem != this.character.data.currentItem)
			{
				for (int i = 0; i < this.switchGeneric.Length; i++)
				{
					this.switchGeneric[i].Play(base.transform.position);
				}
			}
			ItemUseFeedback itemUseFeedback;
			if (this.character.data.currentItem.TryGetComponent<ItemUseFeedback>(ref itemUseFeedback))
			{
				if (this.prevItem != this.character.data.currentItem)
				{
					SFX_Instance[] equip = itemUseFeedback.equip;
					for (int j = 0; j < equip.Length; j++)
					{
						equip[j].Play(base.transform.position);
					}
				}
				string useAnimation = itemUseFeedback.useAnimation;
				if (!string.IsNullOrEmpty(useAnimation))
				{
					if (this.character.data.currentItem.isUsingPrimary && this.character.data.currentItem.castProgress < 1f)
					{
						this.character.refs.animator.SetBool(useAnimation, true);
					}
					else
					{
						this.character.refs.animator.SetBool(useAnimation, false);
					}
				}
				this.prevUse = useAnimation;
			}
		}
		if (this.prevItem && !this.character.data.currentItem)
		{
			for (int k = 0; k < this.switchGeneric.Length; k++)
			{
				this.switchGeneric[k].Play(base.transform.position);
			}
		}
		this.prevItem = this.character.data.currentItem;
	}

	// Token: 0x04001025 RID: 4133
	private string prevUse;

	// Token: 0x04001026 RID: 4134
	private Item prevItem;

	// Token: 0x04001027 RID: 4135
	public AudioSource throwCharge;

	// Token: 0x04001028 RID: 4136
	private Character character;

	// Token: 0x04001029 RID: 4137
	[HideInInspector]
	public float finishTimer;

	// Token: 0x0400102A RID: 4138
	private float increase;

	// Token: 0x0400102B RID: 4139
	public SFX_Instance[] switchGeneric;
}
