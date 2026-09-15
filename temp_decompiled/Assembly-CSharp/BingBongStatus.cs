using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class BingBongStatus : MonoBehaviour
{
	// Token: 0x06000FBB RID: 4027 RVA: 0x0004E2C7 File Offset: 0x0004C4C7
	private void OnEnable()
	{
		this.bingBongPowers = base.GetComponent<BingBongPowers>();
		this.bingBongPowers.SetTexts("STATUS", this.descr);
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0004E2EB File Offset: 0x0004C4EB
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x0004E2FC File Offset: 0x0004C4FC
	private void Update()
	{
		if (Input.GetKeyDown(102))
		{
			this.allStatusSelected = true;
			this.bingBongPowers.SetTip("Status: All", 2);
		}
		string[] names = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE));
		int num = names.Length;
		int num2 = (int)this.currentStatusTarget;
		if (Input.GetKeyDown(118))
		{
			this.allStatusSelected = false;
			num2--;
		}
		if (Input.GetKeyDown(99))
		{
			this.allStatusSelected = false;
			num2++;
		}
		if (num2 < 0)
		{
			num2 = num - 1;
		}
		if (num2 >= num)
		{
			num2 = 0;
		}
		if (this.currentStatusTarget != (CharacterAfflictions.STATUSTYPE)num2)
		{
			this.currentStatusTarget = (CharacterAfflictions.STATUSTYPE)num2;
			this.bingBongPowers.SetTip(names[num2] ?? "", 2);
		}
		Character target = this.GetTarget();
		if (target)
		{
			if (Input.GetKeyDown(323))
			{
				this.view.RPC("RPCA_AddStatusBingBing", 0, new object[]
				{
					target.photonView.ViewID,
					(int)(this.allStatusSelected ? ((CharacterAfflictions.STATUSTYPE)(-1)) : this.currentStatusTarget),
					1
				});
			}
			if (Input.GetKeyDown(324))
			{
				this.view.RPC("RPCA_AddStatusBingBing", 0, new object[]
				{
					target.photonView.ViewID,
					(int)(this.allStatusSelected ? ((CharacterAfflictions.STATUSTYPE)(-1)) : this.currentStatusTarget),
					-1
				});
			}
		}
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x0004E468 File Offset: 0x0004C668
	[PunRPC]
	public void RPCA_AddStatusBingBing(int target, int statusID, int mult)
	{
		Character component = PhotonView.Find(target).GetComponent<Character>();
		if (component.IsLocal)
		{
			if (mult > 0)
			{
				if (statusID == -1)
				{
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Weight, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 1f, false, true, true);
					return;
				}
				component.refs.afflictions.AddStatus(this.currentStatusTarget, 0.2f, false, true, true);
				return;
			}
			else
			{
				if (statusID == -1)
				{
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Poison, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Curse, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Weight, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, 1f, false, false);
					return;
				}
				component.refs.afflictions.SubtractStatus(this.currentStatusTarget, 0.2f, false, false);
			}
		}
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x0004E670 File Offset: 0x0004C870
	private Character GetTarget()
	{
		Character character = null;
		float num = float.MaxValue;
		foreach (Character character2 in Character.AllCharacters)
		{
			float num2 = Vector3.Angle(MainCamera.instance.transform.forward, character2.Center - MainCamera.instance.transform.position);
			if (num2 < num)
			{
				num = num2;
				character = character2;
			}
		}
		return character;
	}

	// Token: 0x04000E18 RID: 3608
	private BingBongPowers bingBongPowers;

	// Token: 0x04000E19 RID: 3609
	private string descr = "Add status: [LMB]\n\nRemove status: [RMB]\n\nSelect all status: [F]\n\nPrev status: [V]\n\nNext status: [C]\n\n";

	// Token: 0x04000E1A RID: 3610
	private PhotonView view;

	// Token: 0x04000E1B RID: 3611
	private bool allStatusSelected;

	// Token: 0x04000E1C RID: 3612
	private CharacterAfflictions.STATUSTYPE currentStatusTarget;
}
