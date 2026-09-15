using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

// Token: 0x020001BF RID: 447
public class DialogueEffect : MonoBehaviour
{
	// Token: 0x06000DC3 RID: 3523 RVA: 0x00044F1A File Offset: 0x0004311A
	private void Awake()
	{
		this.m_TextComponent = base.GetComponent<TMP_Text>();
		this.DTanimator = new DOTweenTMPAnimator(this.m_TextComponent);
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x00044F39 File Offset: 0x00043139
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00044F41 File Offset: 0x00043141
	private void OnEnable()
	{
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x00044F43 File Offset: 0x00043143
	private void OnDisable()
	{
		this.TryDestroy();
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x00044F4B File Offset: 0x0004314B
	public virtual void Init()
	{
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x00044F4D File Offset: 0x0004314D
	private void TryDestroy()
	{
		this.destroyed = true;
		Object.Destroy(this);
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00044F5C File Offset: 0x0004315C
	private void LateUpdate()
	{
		if (!this.destroyed)
		{
			this.EffectRoutine();
		}
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x00044F6C File Offset: 0x0004316C
	protected virtual void EffectRoutine()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
		if (characterCount == 0)
		{
			return;
		}
		for (int i = 0; i < characterCount; i++)
		{
			this.UpdateCharacter(i);
		}
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00044FAD File Offset: 0x000431AD
	public virtual void UpdateCharacter(int index)
	{
	}

	// Token: 0x04000BDB RID: 3035
	protected TMP_Text m_TextComponent;

	// Token: 0x04000BDC RID: 3036
	protected TMP_TextInfo textInfo;

	// Token: 0x04000BDD RID: 3037
	public DOTweenTMPAnimator DTanimator;

	// Token: 0x04000BDE RID: 3038
	private bool destroyed;
}
