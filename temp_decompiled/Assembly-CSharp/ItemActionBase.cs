using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class ItemActionBase : MonoBehaviourPun
{
	// Token: 0x17000092 RID: 146
	// (get) Token: 0x060008F6 RID: 2294 RVA: 0x000303C3 File Offset: 0x0002E5C3
	[SerializeField]
	protected Character character
	{
		get
		{
			return this.item.holderCharacter;
		}
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x000303D0 File Offset: 0x0002E5D0
	public virtual void RunAction()
	{
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x000303D2 File Offset: 0x0002E5D2
	protected virtual void OnEnable()
	{
		this.Init();
		this.Subscribe();
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x000303E0 File Offset: 0x0002E5E0
	protected virtual void Start()
	{
		this.Unsubscribe();
		this.Subscribe();
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x000303EE File Offset: 0x0002E5EE
	public void OnDisable()
	{
		this.Unsubscribe();
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x000303F6 File Offset: 0x0002E5F6
	protected virtual void Subscribe()
	{
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x000303F8 File Offset: 0x0002E5F8
	protected virtual void Unsubscribe()
	{
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x000303FA File Offset: 0x0002E5FA
	private void Init()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x0400087A RID: 2170
	protected Item item;
}
