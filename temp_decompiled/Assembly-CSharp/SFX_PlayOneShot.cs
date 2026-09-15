using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class SFX_PlayOneShot : MonoBehaviour
{
	// Token: 0x06000D36 RID: 3382 RVA: 0x000427C6 File Offset: 0x000409C6
	public void Start()
	{
		if (this.playOnStart)
		{
			this.Play();
		}
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x000427D6 File Offset: 0x000409D6
	public void OnEnable()
	{
		if (this.playOnEnable)
		{
			base.StartCoroutine(this.<OnEnable>g__PlayAfterAnim|6_0());
		}
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x000427ED File Offset: 0x000409ED
	public void Play()
	{
		this.PlayOneShot();
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x000427F8 File Offset: 0x000409F8
	public void PlayOneShot()
	{
		Action action = this.beforePlayAction;
		if (action != null)
		{
			action.Invoke();
		}
		if (this.sfx != null)
		{
			SFX_Player.instance.PlaySFX(this.sfx, base.transform.position, this.followTransform ? base.transform : null, null, 1f, false);
		}
		for (int i = 0; i < this.sfxs.Length; i++)
		{
			SFX_Player.instance.PlaySFX(this.sfxs[i], base.transform.position, this.followTransform ? base.transform : null, null, 1f, false);
		}
		Action action2 = this.afterPlayAction;
		if (action2 == null)
		{
			return;
		}
		action2.Invoke();
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x000428C0 File Offset: 0x00040AC0
	[CompilerGenerated]
	private IEnumerator <OnEnable>g__PlayAfterAnim|6_0()
	{
		yield return new WaitForEndOfFrame();
		this.Play();
		yield break;
	}

	// Token: 0x04000B6B RID: 2923
	public Action beforePlayAction;

	// Token: 0x04000B6C RID: 2924
	public Action afterPlayAction;

	// Token: 0x04000B6D RID: 2925
	public bool playOnStart;

	// Token: 0x04000B6E RID: 2926
	public bool playOnEnable;

	// Token: 0x04000B6F RID: 2927
	public bool followTransform = true;

	// Token: 0x04000B70 RID: 2928
	public SFX_Instance sfx;

	// Token: 0x04000B71 RID: 2929
	public SFX_Instance[] sfxs;
}
