using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000231 RID: 561
public class ClimbHandle : MonoBehaviour, IInteractible
{
	// Token: 0x060010A7 RID: 4263 RVA: 0x00053EDA File Offset: 0x000520DA
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x00053EE8 File Offset: 0x000520E8
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x00053EF5 File Offset: 0x000520F5
	public string GetInteractionText()
	{
		return LocalizedText.GetText("GRAB", true);
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x00053F02 File Offset: 0x00052102
	public string GetName()
	{
		if (this.isPickaxe)
		{
			return LocalizedText.GetText("PICKAXE", true);
		}
		return LocalizedText.GetText("PITONPROMPT", true);
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x00053F23 File Offset: 0x00052123
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x00053F2B File Offset: 0x0005212B
	public void HoverEnter()
	{
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x00053F2D File Offset: 0x0005212D
	public void HoverExit()
	{
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x00053F30 File Offset: 0x00052130
	public void Interact(Character interactor)
	{
		if (this.hanger)
		{
			return;
		}
		if (interactor.IsLocal)
		{
			interactor.refs.climbing.StopAnyClimbing();
		}
		else
		{
			Debug.LogWarning("How come someone who is not local is Interacting???");
		}
		this.view.RPC("RPCA_Hang", 0, new object[] { interactor.photonView });
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x00053F90 File Offset: 0x00052190
	[PunRPC]
	public void RPCA_Hang(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		Character component = view.GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		this.hanger = component;
		component.refs.climbing.StartHang(this);
		Action<Character> action = this.onHangStart;
		if (action == null)
		{
			return;
		}
		action.Invoke(component);
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x00053FE4 File Offset: 0x000521E4
	[PunRPC]
	public void RPCA_UnHang(PhotonView view)
	{
		this.hanger = null;
		if (view == null)
		{
			return;
		}
		Character component = view.GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		component.data.currentClimbHandle = null;
		Action action = this.onHangStop;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x0005402F File Offset: 0x0005222F
	public bool IsInteractible(Character interactor)
	{
		return this.hanger == null;
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x00054040 File Offset: 0x00052240
	internal void Break()
	{
		if (this.hanger != null)
		{
			this.hanger.refs.climbing.CancelHandle(true);
		}
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
	}

	// Token: 0x04000ED9 RID: 3801
	public Character hanger;

	// Token: 0x04000EDA RID: 3802
	internal PhotonView view;

	// Token: 0x04000EDB RID: 3803
	public bool isPickaxe;

	// Token: 0x04000EDC RID: 3804
	public Action<Character> onHangStart;

	// Token: 0x04000EDD RID: 3805
	public Action onHangStop;
}
