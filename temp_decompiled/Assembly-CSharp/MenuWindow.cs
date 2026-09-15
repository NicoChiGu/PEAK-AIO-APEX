using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001D3 RID: 467
public class MenuWindow : MonoBehaviour, INavigationContainer
{
	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000E43 RID: 3651 RVA: 0x00046DE7 File Offset: 0x00044FE7
	public virtual bool openOnStart
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000E44 RID: 3652 RVA: 0x00046DEA File Offset: 0x00044FEA
	public virtual bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000E45 RID: 3653 RVA: 0x00046DED File Offset: 0x00044FED
	public virtual Selectable objectToSelectOnOpen
	{
		get
		{
			return null;
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000E46 RID: 3654 RVA: 0x00046DF0 File Offset: 0x00044FF0
	public virtual bool closeOnPause
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000E47 RID: 3655 RVA: 0x00046DF3 File Offset: 0x00044FF3
	public virtual bool closeOnUICancel
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000E48 RID: 3656 RVA: 0x00046DF6 File Offset: 0x00044FF6
	public virtual bool blocksPlayerInput
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000E49 RID: 3657 RVA: 0x00046DF9 File Offset: 0x00044FF9
	public virtual bool showCursorWhileOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000E4A RID: 3658 RVA: 0x00046DFC File Offset: 0x00044FFC
	public virtual bool autoHideOnClose
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000E4B RID: 3659 RVA: 0x00046DFF File Offset: 0x00044FFF
	// (set) Token: 0x06000E4C RID: 3660 RVA: 0x00046E07 File Offset: 0x00045007
	public bool isOpen { get; private set; }

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000E4D RID: 3661 RVA: 0x00046E10 File Offset: 0x00045010
	// (set) Token: 0x06000E4E RID: 3662 RVA: 0x00046E18 File Offset: 0x00045018
	public bool inputActive { get; private set; }

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000E4F RID: 3663 RVA: 0x00046E21 File Offset: 0x00045021
	// (set) Token: 0x06000E50 RID: 3664 RVA: 0x00046E29 File Offset: 0x00045029
	public bool initialized { get; private set; }

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000E51 RID: 3665 RVA: 0x00046E32 File Offset: 0x00045032
	public virtual GameObject panel
	{
		get
		{
			return base.gameObject;
		}
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x00046E3A File Offset: 0x0004503A
	protected virtual void Start()
	{
		if (!this.isOpen)
		{
			if (this.openOnStart)
			{
				this.Open();
				return;
			}
			this.StartClosed();
		}
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x00046E59 File Offset: 0x00045059
	protected virtual void Update()
	{
		if (this.isOpen)
		{
			INavigationContainer.PushActive(this);
		}
		this.TestCloseViaInput();
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x00046E70 File Offset: 0x00045070
	private void TestCloseViaInput()
	{
		if (this.inputActive)
		{
			if (this.closeOnPause && Character.localCharacter && Character.localCharacter.input.pauseWasPressed)
			{
				this.Close();
				Character.localCharacter.input.pauseWasPressed = false;
				return;
			}
			if (this.closeOnUICancel && Singleton<UIInputHandler>.Instance.cancelWasPressed)
			{
				this.Close();
				Singleton<UIInputHandler>.Instance.cancelWasPressed = false;
				return;
			}
		}
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x00046EE7 File Offset: 0x000450E7
	protected virtual void Initialize()
	{
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x00046EEC File Offset: 0x000450EC
	internal virtual void Open()
	{
		Debug.Log("opening window", base.gameObject);
		this.isOpen = true;
		if (!MenuWindow.AllActiveWindows.Contains(this))
		{
			MenuWindow.AllActiveWindows.Add(this);
		}
		this.Show();
		if (!this.initialized)
		{
			this.Initialize();
			this.initialized = true;
		}
		this.OnOpen();
		if (this.selectOnOpen)
		{
			this.SelectStartingElement();
		}
		this.SetInputActive(true);
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00046F5E File Offset: 0x0004515E
	protected virtual void OnOpen()
	{
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00046F60 File Offset: 0x00045160
	private void OnDestroy()
	{
		if (MenuWindow.AllActiveWindows.Contains(this))
		{
			MenuWindow.AllActiveWindows.Remove(this);
		}
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00046F7C File Offset: 0x0004517C
	public static void CloseAllWindows()
	{
		for (int i = MenuWindow.AllActiveWindows.Count - 1; i >= 0; i--)
		{
			if (MenuWindow.AllActiveWindows[i] != null)
			{
				MenuWindow.AllActiveWindows[i].ForceClose();
			}
		}
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x00046FC3 File Offset: 0x000451C3
	internal void StartClosed()
	{
		this.isOpen = false;
		this.SetInputActive(false);
		this.panel.SetActive(false);
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x00046FE0 File Offset: 0x000451E0
	internal void Close()
	{
		Debug.Log(base.gameObject.name + " closing.");
		this.isOpen = false;
		if (MenuWindow.AllActiveWindows.Contains(this))
		{
			MenuWindow.AllActiveWindows.Remove(this);
		}
		this.OnClose();
		this.SetInputActive(false);
		if (this.autoHideOnClose)
		{
			this.Hide();
		}
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x00047042 File Offset: 0x00045242
	internal void ForceClose()
	{
		this.Close();
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x0004704A File Offset: 0x0004524A
	protected virtual void OnClose()
	{
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x0004704C File Offset: 0x0004524C
	public void Show()
	{
		this.panel.SetActive(true);
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x0004705A File Offset: 0x0004525A
	public void Hide()
	{
		this.panel.SetActive(false);
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x00047068 File Offset: 0x00045268
	public void SetInputActive(bool active)
	{
		this.inputActive = active;
	}

	// Token: 0x06000E61 RID: 3681 RVA: 0x00047071 File Offset: 0x00045271
	private void SelectStartingElement()
	{
		UIInputHandler.SetSelectedObject((this.objectToSelectOnOpen == null) ? null : this.objectToSelectOnOpen.gameObject);
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x00047094 File Offset: 0x00045294
	public int GetContainerPriority()
	{
		return 1;
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x00047097 File Offset: 0x00045297
	public GameObject GetDefaultSelection()
	{
		if (this.objectToSelectOnOpen == null)
		{
			return null;
		}
		return this.objectToSelectOnOpen.gameObject;
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x000470B4 File Offset: 0x000452B4
	public bool IsValidSelection(GameObject selection)
	{
		return selection.activeInHierarchy;
	}

	// Token: 0x04000C62 RID: 3170
	public static List<MenuWindow> AllActiveWindows = new List<MenuWindow>();
}
