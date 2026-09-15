using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class Action_AskBingBong : ItemAction
{
	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06000F24 RID: 3876 RVA: 0x0004A1B0 File Offset: 0x000483B0
	// (remove) Token: 0x06000F25 RID: 3877 RVA: 0x0004A1E8 File Offset: 0x000483E8
	public event Action_AskBingBong.AskEvent OnAsk;

	// Token: 0x06000F26 RID: 3878 RVA: 0x0004A220 File Offset: 0x00048420
	public override void RunAction()
	{
		int num = Random.Range(0, this.responses.Length);
		if (this.debugCycle)
		{
			num = this.debug;
			this.debug++;
			if (this.debug >= this.responses.Length)
			{
				this.debug = 0;
			}
		}
		this.item.photonView.RPC("Ask", 0, new object[]
		{
			num,
			Time.time < this.lastAsked + 1f
		});
		if (Time.time > this.lastAsked + 1f)
		{
			this.lastAsked = Time.time;
		}
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x0004A2D0 File Offset: 0x000484D0
	[PunRPC]
	public void Ask(int index, bool spamming)
	{
		if (this.item.holderCharacter != null)
		{
			this.squishAnim.SetTrigger("Squish");
			SFX_Player.instance.PlaySFX(this.squeak, base.transform.position, base.transform, null, 1f, false);
			this.subtitles.gameObject.SetActive(false);
			if (this.askRoutine != null)
			{
				base.StopCoroutine(this.askRoutine);
			}
			base.StartCoroutine(this.AskRoutine(index, spamming));
		}
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0004A35D File Offset: 0x0004855D
	private IEnumerator SubtitleRoutine(string subtitleID)
	{
		yield return new WaitForSeconds(0.19f);
		string text = LocalizedText.GetText(subtitleID, true);
		this.subtitles.gameObject.SetActive(true);
		this.subtitles.text = text;
		float t = 0f;
		while (t < 1.8f / this.source.pitch)
		{
			t += Time.deltaTime;
			this.subtitles.alpha = Mathf.Clamp01(t * 12f);
			this.subtitles.transform.localScale = Vector3.one * this.scaleCurve.Evaluate(Vector3.Distance(this.subtitles.transform.position, MainCamera.instance.cam.transform.position));
			this.subtitles.transform.forward = MainCamera.instance.cam.transform.forward;
			yield return null;
		}
		this.subtitles.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x0004A373 File Offset: 0x00048573
	private IEnumerator AskRoutine(int index, bool spamming)
	{
		float t = 0f;
		while (t < 0.5f)
		{
			this.item.holderCharacter.refs.items.UpdateAttachedItem();
			t += Time.deltaTime;
			yield return null;
		}
		if (spamming)
		{
			yield break;
		}
		this.source.Stop();
		this.subtitles.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		if (this.subtitleRoutine != null)
		{
			base.StopCoroutine(this.subtitleRoutine);
		}
		this.subtitleRoutine = base.StartCoroutine(this.SubtitleRoutine(this.responses[index].subtitleID));
		if (this.responses[index].sfx != null)
		{
			this.source.PlayOneShot(this.responses[index].sfx.clips[0]);
			if (this.OnAsk != null)
			{
				this.OnAsk(this.responses[index].sfx.clips[0]);
			}
		}
		yield break;
	}

	// Token: 0x04000D25 RID: 3365
	public AnimationCurve animationCurve;

	// Token: 0x04000D26 RID: 3366
	public SFX_Instance shake;

	// Token: 0x04000D27 RID: 3367
	public SFX_Instance squeak;

	// Token: 0x04000D28 RID: 3368
	public Action_AskBingBong.BingBongResponse[] responses;

	// Token: 0x04000D29 RID: 3369
	public Animator squishAnim;

	// Token: 0x04000D2A RID: 3370
	public Animator anim;

	// Token: 0x04000D2B RID: 3371
	public bool debugCycle;

	// Token: 0x04000D2C RID: 3372
	private int debug;

	// Token: 0x04000D2D RID: 3373
	public TextMeshPro subtitles;

	// Token: 0x04000D2E RID: 3374
	private float lastAsked;

	// Token: 0x04000D2F RID: 3375
	public AnimationCurve scaleCurve;

	// Token: 0x04000D30 RID: 3376
	public AudioSource source;

	// Token: 0x04000D32 RID: 3378
	public static int MOUTHBLENDID = Animator.StringToHash("Mouth Blend");

	// Token: 0x04000D33 RID: 3379
	private Coroutine askRoutine;

	// Token: 0x04000D34 RID: 3380
	private Coroutine subtitleRoutine;

	// Token: 0x020004C4 RID: 1220
	// (Invoke) Token: 0x06001C76 RID: 7286
	public delegate void AskEvent(AudioClip clip);

	// Token: 0x020004C5 RID: 1221
	[Serializable]
	public class BingBongResponse
	{
		// Token: 0x04001A67 RID: 6759
		public string subtitleID;

		// Token: 0x04001A68 RID: 6760
		public SFX_Instance sfx;

		// Token: 0x04001A69 RID: 6761
		public AnimationCurve mouthCurve;

		// Token: 0x04001A6A RID: 6762
		public float mouthCurveTime = 1f;
	}
}
