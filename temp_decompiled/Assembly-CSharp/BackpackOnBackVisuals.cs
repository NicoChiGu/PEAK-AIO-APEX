using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class BackpackOnBackVisuals : BackpackVisuals, IInteractibleConstant, IInteractible
{
	// Token: 0x06000496 RID: 1174 RVA: 0x0001BB52 File Offset: 0x00019D52
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
		this.InitRenderers();
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x0001BB66 File Offset: 0x00019D66
	private void OnEnable()
	{
		this.RefreshCooking();
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x0001BB70 File Offset: 0x00019D70
	private void InitRenderers()
	{
		this.renderers = base.GetComponentsInChildren<MeshRenderer>();
		this.defaultTints = new Color[this.renderers.Length];
		for (int i = 0; i < this.renderers.Length; i++)
		{
			this.defaultTints[i] = this.renderers[i].material.GetColor("_Tint");
		}
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0001BBD4 File Offset: 0x00019DD4
	private void RefreshCooking()
	{
		IntItemData intItemData;
		if (this.character.player.backpackSlot.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
		{
			this.CookVisually(intItemData.Value);
		}
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x0001BC0C File Offset: 0x00019E0C
	protected virtual void CookVisually(int cookedAmount)
	{
		Debug.Log("Cooking backpack visually");
		if (this.renderers == null)
		{
			this.InitRenderers();
		}
		for (int i = 0; i < this.renderers.Length; i++)
		{
			if (cookedAmount > 0)
			{
				Debug.Log(string.Format("Cooked amount is {0}", cookedAmount));
				this.renderers[i].material.SetColor("_Tint", this.defaultTints[i] * ItemCooking.GetCookColor(cookedAmount));
			}
		}
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x0001BC8C File Offset: 0x00019E8C
	public override BackpackData GetBackpackData()
	{
		BackpackData backpackData;
		if (!this.character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			this.character.player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
		}
		return backpackData;
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0001BCD5 File Offset: 0x00019ED5
	protected override void PutItemInBackpack(GameObject visual, byte slotID)
	{
		visual.GetComponent<PhotonView>().RPC("PutInBackpackRPC", 0, new object[]
		{
			slotID,
			BackpackReference.GetFromEquippedBackpack(this.character)
		});
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x0001BD0C File Offset: 0x00019F0C
	public bool IsInteractible(Character interactor)
	{
		Vector3 vector = HelperFunctions.ZeroY(interactor.data.lookDirection);
		Vector3 vector2 = HelperFunctions.ZeroY(base.transform.forward);
		return Vector3.Angle(vector, vector2) < 110f;
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x0001BD47 File Offset: 0x00019F47
	public void Interact(Character interactor)
	{
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0001BD4C File Offset: 0x00019F4C
	public void HoverEnter()
	{
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren)
		{
			componentInChildren.material.SetFloat(BackpackOnBackVisuals.Interactable, 1f);
		}
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0001BD80 File Offset: 0x00019F80
	public void HoverExit()
	{
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren)
		{
			componentInChildren.material.SetFloat(BackpackOnBackVisuals.Interactable, 0f);
		}
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0001BDB1 File Offset: 0x00019FB1
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0001BDBE File Offset: 0x00019FBE
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0001BDC6 File Offset: 0x00019FC6
	public string GetInteractionText()
	{
		return LocalizedText.GetText("open", true);
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0001BDD3 File Offset: 0x00019FD3
	public string GetName()
	{
		return LocalizedText.GetText("SOMEONESBACKPACK", true).Replace("#", this.character.characterName);
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0001BDF5 File Offset: 0x00019FF5
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.IsInteractible(interactor);
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001BDFE File Offset: 0x00019FFE
	public float GetInteractTime(Character interactor)
	{
		return this.openRadialMenuTime;
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001BE06 File Offset: 0x0001A006
	public void Interact_CastFinished(Character interactor)
	{
		GUIManager.instance.OpenBackpackWheel(BackpackReference.GetFromEquippedBackpack(this.character));
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001BE1D File Offset: 0x0001A01D
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0001BE1F File Offset: 0x0001A01F
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x060004AA RID: 1194 RVA: 0x0001BE21 File Offset: 0x0001A021
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04000519 RID: 1305
	private static readonly int Interactable = Shader.PropertyToID("_Interactable");

	// Token: 0x0400051A RID: 1306
	public Character character;

	// Token: 0x0400051B RID: 1307
	public float openRadialMenuTime = 0.25f;

	// Token: 0x0400051C RID: 1308
	private MeshRenderer[] renderers;

	// Token: 0x0400051D RID: 1309
	private Color[] defaultTints;
}
