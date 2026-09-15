using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x020000A5 RID: 165
public class Campfire : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000629 RID: 1577 RVA: 0x000236F4 File Offset: 0x000218F4
	public bool Lit
	{
		get
		{
			return this.state == Campfire.FireState.Lit;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x0600062A RID: 1578 RVA: 0x000236FF File Offset: 0x000218FF
	public float LitProgress
	{
		get
		{
			return ExtDataTypes.Clamp01(this.beenBurningFor / this.burnsFor);
		}
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x00023714 File Offset: 0x00021914
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.mainRenderer = base.GetComponentInChildren<Renderer>();
		this.startRot = this.fireParticles.emission.rateOverTime.constant;
		this.startSize = new Vector2(this.fireParticles.main.startSize.constantMin, this.fireParticles.main.startSize.constantMax);
		this.SetFireWoodCount(3);
		this.UpdateLit();
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x000237A8 File Offset: 0x000219A8
	private void Update()
	{
		if (this.Lit)
		{
			this.beenBurningFor += Time.deltaTime;
			ParticleSystem.MainModule main = this.fireParticles.main;
			ParticleSystem.MinMaxCurve minMaxCurve = main.startSize;
			minMaxCurve.constantMin = Mathf.Lerp(this.startSize.x, this.endSize.x, this.LitProgress);
			minMaxCurve.constantMax = Mathf.Lerp(this.startSize.y, this.endSize.y, this.LitProgress);
			main.startSize = minMaxCurve;
			ParticleSystem.EmissionModule emission = this.fireParticles.emission;
			ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
			rateOverTime.constant = Mathf.Lerp(this.startRot, this.endRot, this.LitProgress);
			emission.rateOverTime = rateOverTime;
			if (!this.t)
			{
				if (!this.isPyre && MoraleBoost.SpawnMoraleBoost(base.transform.position, this.moraleBoostRadius, this.moraleBoostBaseline, this.moraleBoostPerAdditionalScout, false, 2))
				{
					for (int i = 0; i < this.moraleBoost.Length; i++)
					{
						this.moraleBoost[i].Play(base.transform.position);
					}
					Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.MoraleBoosts, 1);
				}
				if (Character.localCharacter != null && Vector3.Distance(base.transform.position, Character.localCharacter.Center) <= this.moraleBoostRadius)
				{
					Character.localCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Injury, -0.2f, false);
				}
				for (int j = 0; j < this.fireStart.Length; j++)
				{
					this.fireStart[j].Play(base.transform.position);
				}
				this.t = true;
			}
			if (this.view.IsMine && this.beenBurningFor > this.burnsFor && !this.isPyre)
			{
				this.view.RPC("Extinguish_Rpc", 3, Array.Empty<object>());
			}
		}
		else if (this.t)
		{
			for (int k = 0; k < this.extinguish.Length; k++)
			{
				this.extinguish[k].Play(base.transform.position);
			}
			this.t = false;
		}
		this.StupidTextUpdate();
		this.UpdateAudioLoop();
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x000239F5 File Offset: 0x00021BF5
	private void StupidTextUpdate()
	{
		if (GUIManager.instance.currentInteractable == this)
		{
			GUIManager.instance.RefreshInteractablePrompt();
		}
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00023A0E File Offset: 0x00021C0E
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.moraleBoostRadius);
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00023A30 File Offset: 0x00021C30
	public Vector3 Center()
	{
		return this.mainRenderer.bounds.center;
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x00023A50 File Offset: 0x00021C50
	public string GetInteractionText()
	{
		if (!this.Lit)
		{
			string text;
			if (!this.EveryoneInRange(out text, 15f))
			{
				return text;
			}
			return LocalizedText.GetText("LIGHT", true);
		}
		else
		{
			if (this.Lit)
			{
				return LocalizedText.GetText("COOK", true);
			}
			return "";
		}
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x00023A9B File Offset: 0x00021C9B
	public string GetName()
	{
		if (!string.IsNullOrEmpty(this.nameOverride))
		{
			return LocalizedText.GetText(this.nameOverride, true);
		}
		return LocalizedText.GetText("CAMPFIRE", true);
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x00023AC2 File Offset: 0x00021CC2
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00023ACA File Offset: 0x00021CCA
	public void HoverEnter()
	{
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00023ACC File Offset: 0x00021CCC
	public void HoverExit()
	{
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00023AD0 File Offset: 0x00021CD0
	public void Interact(Character interactor)
	{
		if (this.Lit && interactor.data.currentItem != null && interactor.data.currentItem.cooking.canBeCooked)
		{
			this.currentlyCookingItem = interactor.data.currentItem;
			interactor.data.currentItem.GetComponent<ItemCooking>().StartCookingVisuals();
		}
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00023B38 File Offset: 0x00021D38
	public void Interact_CastFinished(Character interactor)
	{
		string text;
		if (this.Lit)
		{
			if (this.currentlyCookingItem)
			{
				if (this.currentlyCookingItem.GetData<IntItemData>(DataEntryKey.CookedAmount).Value == 0)
				{
					Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.MealsCooked, 1);
				}
				this.currentlyCookingItem.GetComponent<ItemCooking>().FinishCooking();
				return;
			}
		}
		else if (this.EveryoneInRange(out text, 15f))
		{
			this.view.RPC("Light_Rpc", 0, Array.Empty<object>());
		}
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00023BB0 File Offset: 0x00021DB0
	public void CancelCast(Character interactor)
	{
		if (this.currentlyCookingItem)
		{
			this.currentlyCookingItem.GetComponent<ItemCooking>().CancelCookingVisuals();
		}
		this.currentlyCookingItem = null;
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00023BD6 File Offset: 0x00021DD6
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000639 RID: 1593 RVA: 0x00023BD8 File Offset: 0x00021DD8
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00023BDC File Offset: 0x00021DDC
	public bool IsInteractible(Character interactor)
	{
		return this.state == Campfire.FireState.Off || (this.state != Campfire.FireState.Spent && interactor.data.currentItem != null && interactor.data.currentItem.cooking.canBeCooked);
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00023C28 File Offset: 0x00021E28
	public bool EveryoneInRange(out string printout, float range = 15f)
	{
		bool flag = true;
		printout = "";
		foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
		{
			if (!(character == null) && !character.photonView.Owner.IsInactive)
			{
				float num = Vector3.Distance(base.transform.position, character.Center);
				if (num > range && !character.data.dead)
				{
					flag = false;
					printout += string.Format("\n{0} {1}m", character.photonView.Owner.NickName, Mathf.RoundToInt(num * CharacterStats.unitsToMeters));
				}
			}
		}
		if (!flag)
		{
			printout = LocalizedText.GetText("CANTLIGHT", true) + "\n" + printout;
		}
		return flag;
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x00023D18 File Offset: 0x00021F18
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.state == Campfire.FireState.Off || (this.state != Campfire.FireState.Spent && interactor.data.currentItem != null && interactor.data.currentItem.cooking.canBeCooked);
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x00023D64 File Offset: 0x00021F64
	public float GetInteractTime(Character interactor)
	{
		return this.cookTime;
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x00023D6C File Offset: 0x00021F6C
	public void DebugLight()
	{
		this.view.RPC("Light_Rpc", 0, Array.Empty<object>());
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x00023D84 File Offset: 0x00021F84
	[PunRPC]
	private void SetFireWoodCount(int count)
	{
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x00023D88 File Offset: 0x00021F88
	private void UpdateAudioLoop()
	{
		if (this.loop)
		{
			float num = (this.Lit ? 0.5f : 0f);
			this.loop.volume = Mathf.Lerp(this.loop.volume, num, Time.deltaTime * 5f);
		}
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00023DE0 File Offset: 0x00021FE0
	private void HideLogs()
	{
		foreach (object obj in this.logRoot)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00023E3C File Offset: 0x0002203C
	[PunRPC]
	private void Light_Rpc()
	{
		this.state = Campfire.FireState.Lit;
		Shader.SetGlobalFloat("FakeMountainEnabled", (float)(this.disableFogFakeMountain ? 0 : 1));
		this.UpdateLit();
		this.smokeParticlesOff.Stop();
		this.smokeParticlesLit.Play();
		GUIManager.instance.RefreshInteractablePrompt();
		if (Singleton<MapHandler>.Instance)
		{
			Singleton<MapHandler>.Instance.GoToSegment(this.advanceToSegment);
		}
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x00023EAC File Offset: 0x000220AC
	[PunRPC]
	private void Extinguish_Rpc()
	{
		this.beenBurningFor = 0f;
		this.state = Campfire.FireState.Spent;
		this.HideLogs();
		this.UpdateLit();
		this.smokeParticlesOff.Stop();
		this.smokeParticlesLit.Stop();
		this.fireParticles.Stop();
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x00023EF8 File Offset: 0x000220F8
	private void UpdateLit()
	{
		if (this.enableWhenLit)
		{
			this.enableWhenLit.SetActive(this.state == Campfire.FireState.Lit);
		}
		if (this.disableWhenLit)
		{
			this.disableWhenLit.SetActive(this.state == Campfire.FireState.Off || this.state == Campfire.FireState.Spent);
		}
	}

	// Token: 0x04000634 RID: 1588
	public Segment advanceToSegment;

	// Token: 0x04000635 RID: 1589
	public Campfire.FireState state;

	// Token: 0x04000636 RID: 1590
	public GameObject enableWhenLit;

	// Token: 0x04000637 RID: 1591
	public GameObject disableWhenLit;

	// Token: 0x04000638 RID: 1592
	[FormerlySerializedAs("litTime")]
	public float burnsFor = 180f;

	// Token: 0x04000639 RID: 1593
	public float cookTime = 5f;

	// Token: 0x0400063A RID: 1594
	public Transform logRoot;

	// Token: 0x0400063B RID: 1595
	public int requiredFireWoods = 3;

	// Token: 0x0400063C RID: 1596
	public Vector2 endSize = new Vector2(0.1f, 0.2f);

	// Token: 0x0400063D RID: 1597
	public float endRot = 3f;

	// Token: 0x0400063E RID: 1598
	[FormerlySerializedAs("litTimeElapsed")]
	public float beenBurningFor;

	// Token: 0x0400063F RID: 1599
	public ParticleSystem fireParticles;

	// Token: 0x04000640 RID: 1600
	public ParticleSystem smokeParticlesOff;

	// Token: 0x04000641 RID: 1601
	public ParticleSystem smokeParticlesLit;

	// Token: 0x04000642 RID: 1602
	public float moraleBoostRadius;

	// Token: 0x04000643 RID: 1603
	public float moraleBoostBaseline;

	// Token: 0x04000644 RID: 1604
	public float moraleBoostPerAdditionalScout;

	// Token: 0x04000645 RID: 1605
	public float injuryReduction = 0.2f;

	// Token: 0x04000646 RID: 1606
	public SFX_Instance[] fireStart;

	// Token: 0x04000647 RID: 1607
	public SFX_Instance[] extinguish;

	// Token: 0x04000648 RID: 1608
	public SFX_Instance[] moraleBoost;

	// Token: 0x04000649 RID: 1609
	public AudioSource loop;

	// Token: 0x0400064A RID: 1610
	public bool isPyre;

	// Token: 0x0400064B RID: 1611
	public string nameOverride;

	// Token: 0x0400064C RID: 1612
	private Item currentlyCookingItem;

	// Token: 0x0400064D RID: 1613
	private Renderer mainRenderer;

	// Token: 0x0400064E RID: 1614
	private float startRot;

	// Token: 0x0400064F RID: 1615
	private Vector2 startSize;

	// Token: 0x04000650 RID: 1616
	private bool t;

	// Token: 0x04000651 RID: 1617
	private PhotonView view;

	// Token: 0x04000652 RID: 1618
	public bool disableFogFakeMountain;

	// Token: 0x0200042B RID: 1067
	public enum FireState
	{
		// Token: 0x040017F6 RID: 6134
		Off,
		// Token: 0x040017F7 RID: 6135
		Lit,
		// Token: 0x040017F8 RID: 6136
		Spent
	}
}
