using System;
using System.Collections;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

// Token: 0x020001CA RID: 458
public class LoadingScreen : MonoBehaviour
{
	// Token: 0x06000E17 RID: 3607 RVA: 0x000466E3 File Offset: 0x000448E3
	private void Awake()
	{
		this.canvas.enabled = false;
		this.anim = base.GetComponent<Animator>();
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x00046715 File Offset: 0x00044915
	public virtual IEnumerator LoadingRoutine(Action runAfter, IEnumerator[] processList)
	{
		this.canvas.enabled = true;
		this.group.blocksRaycasts = true;
		float num = 0f;
		if (this.FadeOutAudioCurve != null && this.FadeOutAudioCurve.keys.Length != 0)
		{
			num = AnimationCurveExtensions.GetEndTime(this.FadeOutAudioCurve);
		}
		float extraLoadTime = this.loadStartYieldTime - num;
		if (this.FadeOutAudioCurve != null && this.FadeOutAudioCurve.keys.Length != 0)
		{
			yield return AnimationCurveExtensions.YieldForCurve(this.FadeOutAudioCurve, delegate(float f)
			{
				if (this.Mixer != null)
				{
					this.Mixer.SetFloat("LoadingFade", math.remap(0f, 1f, -80f, 0f, f));
				}
			}, true, 1f);
		}
		if (extraLoadTime > 0f)
		{
			yield return new WaitForSecondsRealtime(extraLoadTime);
		}
		int num2;
		for (int processIndex = 0; processIndex < processList.Length; processIndex = num2 + 1)
		{
			this.currentProcess = processList[processIndex];
			base.StartCoroutine(this.RunProcess(this.currentProcess));
			while (this.runningProcess)
			{
				yield return null;
			}
			num2 = processIndex;
		}
		if (!PhotonNetwork.IsMessageQueueRunning)
		{
			Debug.Log("Restarting message queue");
			PhotonNetwork.IsMessageQueueRunning = true;
		}
		if (runAfter != null)
		{
			runAfter.Invoke();
		}
		this.anim.SetTrigger("Finish");
		this.group.blocksRaycasts = false;
		Debug.Log("Loading finished.");
		if (this.FadeInAudioCurve != null && this.FadeInAudioCurve.keys.Length != 0)
		{
			Debug.Log("FADING OUT");
			yield return AnimationCurveExtensions.YieldForCurve(this.FadeInAudioCurve, delegate(float f)
			{
				if (this.Mixer != null)
				{
					this.Mixer.SetFloat("LoadingFade", math.remap(0f, 1f, -80f, 0f, f));
				}
			}, true, 1f);
			Debug.Log("DONE FADING OUT");
		}
		Debug.Log("Destroying self in 6 seconds");
		Object.Destroy(base.gameObject, 6f);
		yield break;
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x00046732 File Offset: 0x00044932
	private IEnumerator RunProcess(IEnumerator process)
	{
		Debug.Log("Process Started: process");
		this.runningProcess = true;
		yield return base.StartCoroutine(process);
		this.runningProcess = false;
		Debug.Log("Process Finished: process");
		yield break;
	}

	// Token: 0x04000C43 RID: 3139
	public AnimationCurve FadeOutAudioCurve;

	// Token: 0x04000C44 RID: 3140
	public AnimationCurve FadeInAudioCurve;

	// Token: 0x04000C45 RID: 3141
	public AudioMixer Mixer;

	// Token: 0x04000C46 RID: 3142
	public CanvasGroup group;

	// Token: 0x04000C47 RID: 3143
	public Canvas canvas;

	// Token: 0x04000C48 RID: 3144
	private Animator anim;

	// Token: 0x04000C49 RID: 3145
	public float loadStartYieldTime = 1.5f;

	// Token: 0x04000C4A RID: 3146
	protected IEnumerator currentProcess;

	// Token: 0x04000C4B RID: 3147
	private bool runningProcess;

	// Token: 0x020004B4 RID: 1204
	public enum LoadingScreenType
	{
		// Token: 0x04001A33 RID: 6707
		Basic,
		// Token: 0x04001A34 RID: 6708
		Plane
	}
}
