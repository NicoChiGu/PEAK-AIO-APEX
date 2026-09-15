using System;
using System.Collections.Generic;
using Knot;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class KnotTemplateBoss : MonoBehaviour
{
	// Token: 0x060011DF RID: 4575 RVA: 0x0005A6C9 File Offset: 0x000588C9
	private void Awake()
	{
		KnotTemplateBoss.me = this;
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x060011E0 RID: 4576 RVA: 0x0005A6D1 File Offset: 0x000588D1
	// (set) Token: 0x060011E1 RID: 4577 RVA: 0x0005A6D9 File Offset: 0x000588D9
	public LinkedListNode<KnotTemplate> Current
	{
		get
		{
			return this.current;
		}
		set
		{
			ExtTransform.KillAllChildren(this.displayRoot, true, false, false);
			this.current = value;
			Object.Instantiate<KnotTemplate>(this.current.Value, this.displayRoot);
		}
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x0005A707 File Offset: 0x00058907
	private void Start()
	{
		this.templates = new LinkedList<KnotTemplate>(this.startTemplates);
		this.Current = this.templates.First;
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x0005A72C File Offset: 0x0005892C
	public void Next()
	{
		this.Current = ((this.current.Next != null) ? this.Current.Next : this.templates.First);
		Object.FindFirstObjectByType<KnotMaker>().Clear();
		Object.FindFirstObjectByType<KnotUnmaker>().grabbing = false;
		Object.FindFirstObjectByType<TiedKnotVisualizer>().Clear();
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x0005A784 File Offset: 0x00058984
	public void Previous()
	{
		this.Current = ((this.Current.Previous != null) ? this.current.Previous : this.templates.Last);
		Object.FindFirstObjectByType<KnotMaker>().Clear();
		Object.FindFirstObjectByType<KnotUnmaker>().grabbing = false;
		Object.FindFirstObjectByType<TiedKnotVisualizer>().Clear();
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x0005A7DB File Offset: 0x000589DB
	private void Update()
	{
	}

	// Token: 0x0400105C RID: 4188
	public Transform displayRoot;

	// Token: 0x0400105D RID: 4189
	public List<KnotTemplate> startTemplates = new List<KnotTemplate>();

	// Token: 0x0400105E RID: 4190
	public LinkedList<KnotTemplate> templates = new LinkedList<KnotTemplate>();

	// Token: 0x0400105F RID: 4191
	private LinkedListNode<KnotTemplate> current;

	// Token: 0x04001060 RID: 4192
	public static KnotTemplateBoss me;
}
