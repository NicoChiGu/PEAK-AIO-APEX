using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.Core;

// Token: 0x020002B9 RID: 697
public class PeakHandler : Singleton<PeakHandler>
{
	// Token: 0x06001301 RID: 4865 RVA: 0x0006062A File Offset: 0x0005E82A
	public void SummonHelicopter()
	{
		this.peakSequence.SetActive(true);
		this.summonedHelicopter = true;
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x00060640 File Offset: 0x0005E840
	public void EndCutscene()
	{
		this.isPlayingCinematic = true;
		List<Character> allCharacters = Character.AllCharacters;
		foreach (Character character in allCharacters)
		{
			character.refs.animator.gameObject.SetActive(false);
		}
		MainCamera.instance.gameObject.SetActive(false);
		MenuWindow.CloseAllWindows();
		this.peakSequence.SetActive(false);
		GUIManager.instance.letterboxCanvas.gameObject.SetActive(true);
		GUIManager.instance.hudCanvas.enabled = false;
		this.endCutscene.SetActive(true);
		this.SetCosmetics(allCharacters);
		base.StartCoroutine(this.<EndCutscene>g__OpenEndscreen|12_0());
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x00060710 File Offset: 0x0005E910
	private void SetCosmetics(List<Character> characters)
	{
		Singleton<MicrophoneRelay>.Instance.RegisterMicListener(new Action<float[]>(this.OnGetLocalMic));
		characters = Enumerable.ToList<Character>(Enumerable.Where<Character>(characters, (Character character) => character.refs.stats.won));
		characters.Sort((Character c1, Character c2) => c1.photonView.ViewID.CompareTo(c2.photonView.ViewID));
		characters[0].refs.customization.SetCustomizationForRef(this.firstCutsceneScout);
		if (characters[0].data.isSkeleton)
		{
			this.firstCutsceneScout.SetSkeleton(true, false);
		}
		this.firstCutsceneScout.GetComponent<AnimatedMouth>().audioSource = characters[0].GetComponent<AnimatedMouth>().audioSource;
		this.localMouths.Add(this.firstCutsceneScout.GetComponent<AnimatedMouth>());
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if (i >= characters.Count)
			{
				this.cutsceneScoutRefs[i].gameObject.SetActive(false);
			}
			else
			{
				characters[i].refs.customization.SetCustomizationForRef(this.cutsceneScoutRefs[num]);
				if (characters[i].data.isSkeleton)
				{
					this.cutsceneScoutRefs[i].SetSkeleton(true, false);
				}
				BadgeUnlocker.SetBadges(characters[i], this.cutsceneScoutRefs[num].sashRenderer);
				this.cutsceneScoutRefs[num].GetComponent<AnimatedMouth>().audioSource = characters[i].GetComponent<AnimatedMouth>().audioSource;
				if (characters[i].IsLocal)
				{
					this.localMouths.Add(this.cutsceneScoutRefs[num].GetComponent<AnimatedMouth>());
				}
				num++;
			}
		}
		if (characters.Count <= 1)
		{
			this.cutsceneScoutAnims[0].alone = true;
		}
		if (characters.Count <= 2)
		{
			this.cutsceneScoutAnims[1].alone = true;
		}
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x00060900 File Offset: 0x0005EB00
	private void OnGetLocalMic(float[] buffer)
	{
		foreach (AnimatedMouth animatedMouth in this.localMouths)
		{
			animatedMouth.OnGetMic(buffer);
		}
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x00060954 File Offset: 0x0005EB54
	public void EndScreenComplete()
	{
		Singleton<GameOverHandler>.Instance.ForceEveryPlayerDoneWithEndScreen();
		this.endScreenComplete = true;
		base.StartCoroutine(PeakHandler.<EndScreenComplete>g__CreditsLogic|15_0());
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x00060973 File Offset: 0x0005EB73
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (this.isPlayingCinematic && Singleton<MicrophoneRelay>.Instance)
		{
			Singleton<MicrophoneRelay>.Instance.UnregisterMicListener(new Action<float[]>(this.OnGetLocalMic));
		}
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x000609C3 File Offset: 0x0005EBC3
	[CompilerGenerated]
	private IEnumerator <EndCutscene>g__OpenEndscreen|12_0()
	{
		yield return new WaitForSeconds(this.secondsUntilEndscreen);
		GUIManager.instance.endScreen.Open();
		while (!this.endScreenComplete)
		{
			yield return null;
		}
		this.endCutsceneAnimator.SetBool("Next", true);
		GUIManager.instance.endScreen.Close();
		yield break;
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x000609D2 File Offset: 0x0005EBD2
	[CompilerGenerated]
	internal static IEnumerator <EndScreenComplete>g__CreditsLogic|15_0()
	{
		yield return new WaitForSecondsRealtime(20f);
		InputAction anyKeyAction = InputSystem.actions.FindAction("AnyKey", false);
		float timeCreditsStarted = Time.unscaledTime;
		float t = 0f;
		float timeAnyKeyLastPressed = float.NegativeInfinity;
		bool anyKeyPressed = false;
		while (Time.unscaledTime - timeCreditsStarted < 60f)
		{
			if (anyKeyAction != null && anyKeyAction.WasPerformedThisFrame() && !anyKeyPressed)
			{
				anyKeyPressed = true;
				timeAnyKeyLastPressed = Time.unscaledTime;
			}
			if (anyKeyPressed)
			{
				if (GUIManager.TimeLastPaused + 0.5f > timeAnyKeyLastPressed)
				{
					Debug.Log("'Any key' consumed by pause menu");
					anyKeyPressed = false;
				}
				else if (Time.unscaledTime - timeAnyKeyLastPressed > 0.5f)
				{
					break;
				}
			}
			t += Time.unscaledDeltaTime;
			yield return null;
		}
		Debug.Log("Local player is done with credits!");
		Singleton<GameOverHandler>.Instance.LoadAirport();
		yield break;
	}

	// Token: 0x040011A1 RID: 4513
	public bool summonedHelicopter;

	// Token: 0x040011A2 RID: 4514
	public GameObject peakSequence;

	// Token: 0x040011A3 RID: 4515
	public GameObject endCutscene;

	// Token: 0x040011A4 RID: 4516
	public Animator endCutsceneAnimator;

	// Token: 0x040011A5 RID: 4517
	public float secondsUntilEndscreen = 13f;

	// Token: 0x040011A6 RID: 4518
	public CustomizationRefs firstCutsceneScout;

	// Token: 0x040011A7 RID: 4519
	public CustomizationRefs[] cutsceneScoutRefs;

	// Token: 0x040011A8 RID: 4520
	public EndCutsceneScoutHelper[] cutsceneScoutAnims;

	// Token: 0x040011A9 RID: 4521
	private List<AnimatedMouth> localMouths = new List<AnimatedMouth>();

	// Token: 0x040011AA RID: 4522
	public bool isPlayingCinematic;

	// Token: 0x040011AB RID: 4523
	private bool endScreenComplete;
}
