using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200035B RID: 859
public class TriggerEvent : MonoBehaviour
{
	// Token: 0x060015F9 RID: 5625 RVA: 0x000715EF File Offset: 0x0006F7EF
	private void Start()
	{
		this.view = base.GetComponentInParent<PhotonView>();
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x00071600 File Offset: 0x0006F800
	private void OnTriggerEnter(Collider other)
	{
		TriggerEvent.<>c__DisplayClass9_0 CS$<>8__locals1 = new TriggerEvent.<>c__DisplayClass9_0();
		CS$<>8__locals1.<>4__this = this;
		if (this.onlyOnce && (this.hasActivated || this.hasTriggered))
		{
			return;
		}
		if (other.isTrigger)
		{
			return;
		}
		CS$<>8__locals1.player = other.GetComponentInParent<Character>();
		if (!CS$<>8__locals1.player)
		{
			return;
		}
		if (this.hits.Contains(CS$<>8__locals1.player))
		{
			return;
		}
		base.StartCoroutine(CS$<>8__locals1.<OnTriggerEnter>g__IHoldHit|0());
		this.TriggerEntered();
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x00071680 File Offset: 0x0006F880
	public void TriggerEntered()
	{
		if (this.onlyOnce && (this.hasActivated || this.hasTriggered))
		{
			return;
		}
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.triggerChance < Random.value)
		{
			return;
		}
		this.hasActivated = true;
		this.view.RPC("RPCA_Trigger", 0, new object[] { base.transform.GetSiblingIndex() });
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x000716F3 File Offset: 0x0006F8F3
	public void Trigger()
	{
		GameUtils.instance.StartCoroutine(this.TriggerRoutine());
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x00071706 File Offset: 0x0006F906
	private IEnumerator TriggerRoutine()
	{
		if (this.onlyOnce && this.hasTriggered)
		{
			yield break;
		}
		if (this.waitForRenderFrame)
		{
			yield return new WaitForEndOfFrame();
		}
		this.triggerEvent.Invoke();
		this.hasActivated = true;
		this.hasTriggered = true;
		yield break;
	}

	// Token: 0x040014D8 RID: 5336
	[Range(0f, 1f)]
	public float triggerChance = 1f;

	// Token: 0x040014D9 RID: 5337
	public bool onlyOnce;

	// Token: 0x040014DA RID: 5338
	public bool waitForRenderFrame = true;

	// Token: 0x040014DB RID: 5339
	public UnityEvent triggerEvent;

	// Token: 0x040014DC RID: 5340
	private PhotonView view;

	// Token: 0x040014DD RID: 5341
	private bool hasActivated;

	// Token: 0x040014DE RID: 5342
	private bool hasTriggered;

	// Token: 0x040014DF RID: 5343
	private List<Character> hits = new List<Character>();
}
