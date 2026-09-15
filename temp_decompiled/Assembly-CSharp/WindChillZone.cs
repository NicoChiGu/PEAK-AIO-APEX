using System;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x020000B4 RID: 180
public class WindChillZone : MonoBehaviour
{
	// Token: 0x06000697 RID: 1687 RVA: 0x00025990 File Offset: 0x00023B90
	private void Awake()
	{
		WindChillZone.instance = this;
		this.windZoneBounds.center = base.transform.position;
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x000259B0 File Offset: 0x00023BB0
	private void OnDrawGizmosSelected()
	{
		this.windZoneBounds.center = base.transform.position;
		Gizmos.color = this.gizmoColor;
		Gizmos.DrawCube(this.windZoneBounds.center, this.windZoneBounds.extents * 2f);
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00025A03 File Offset: 0x00023C03
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00025A14 File Offset: 0x00023C14
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.HandleTime();
		if (this.windActive)
		{
			this.hasBeenActiveFor += Time.deltaTime;
			this.StormProgress = Mathf.Clamp01(1f - this.untilSwitch / this.timeUntilNextWind);
			this.timeUntilStorm = 0f;
			this.windIntensity = this.windIntensityCurve.Evaluate(this.StormProgress);
		}
		else
		{
			this.timeUntilStorm = Mathf.Clamp01(1f - this.untilSwitch / this.timeUntilNextWind);
			this.StormProgress = 0f;
			this.hasBeenActiveFor = 0f;
		}
		this.localCharacterInsideBounds = this.windZoneBounds.Contains(Character.localCharacter.Center);
		this.observedCharacterInsideBounds = Character.observedCharacter != null && this.windZoneBounds.Contains(Character.observedCharacter.Center);
		if (this.localCharacterInsideBounds && this.windActive)
		{
			this.ApplyStatus(Character.localCharacter);
			return;
		}
		this.windPlayerFactor = 0f;
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00025B34 File Offset: 0x00023D34
	private void HandleTime()
	{
		this.untilSwitch -= Time.deltaTime;
		Vector3 zero = Vector3.zero;
		if (this.untilSwitch < 0f && PhotonNetwork.IsMasterClient)
		{
			this.untilSwitch = this.GetNextWindTime(!this.windActive);
			this.view.RPC("RPCA_ToggleWind", 0, new object[]
			{
				!this.windActive,
				this.RandomWindDirection(),
				this.untilSwitch
			});
		}
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00025BC8 File Offset: 0x00023DC8
	private void FixedUpdate()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.currentForceMult = Mathf.MoveTowards(this.currentForceMult, (float)(this.windActive ? 1 : 0), Time.fixedDeltaTime);
		if (this.timeSpentWaiting >= this.delayBeforeForce)
		{
			if (this.localCharacterInsideBounds && this.windActive)
			{
				this.AddWindForceToCharacter(Character.localCharacter, this.currentForceMult);
			}
			foreach (Character character in Character.AllBotCharacters)
			{
				if (this.windZoneBounds.Contains(character.Center))
				{
					this.AddWindForceToCharacter(character, 0.6f * this.currentForceMult);
				}
			}
		}
		if (this.windMovesItems)
		{
			for (int i = 0; i < Item.ALL_ACTIVE_ITEMS.Count; i++)
			{
				Item item = Item.ALL_ACTIVE_ITEMS[i];
				if (ClassExtensionsMethods.UnityObjectExists<Item>(item) && item.itemState == ItemState.Ground && this.windZoneBounds.Contains(item.Center()))
				{
					this.AddWindForceToItem(item);
				}
			}
		}
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00025CEC File Offset: 0x00023EEC
	private void ApplyStatus(Character character)
	{
		this.windPlayerFactor = WindChillZone.GetWindIntensityAtPoint(character.Center, this.lightVolumeSampleThreshold_lower, this.lightVolumeSampleThreshold_margin);
		float num = Mathf.Max(this.grabStaminaMultiplierDuringWind * this.windPlayerFactor * this.currentForceMult, 1f);
		character.refs.climbing.climbingStamMinimumMultiplier = num;
		if (this.statusApplicationPerSecond > 0f)
		{
			character.refs.afflictions.AddStatus(this.statusType, this.windPlayerFactor * this.statusApplicationPerSecond * Time.deltaTime * Mathf.Clamp01(this.hasBeenActiveFor * 0.2f), false, true, true);
		}
		if (this.setSlippy)
		{
			character.data.slippy = Mathf.Clamp01(Mathf.Max(character.data.slippy, this.windPlayerFactor * 10f));
		}
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00025DC8 File Offset: 0x00023FC8
	private void AddWindForceToCharacter(Character character, float mult = 1f)
	{
		if (!character.photonView.IsMine)
		{
			return;
		}
		if (character.data.currentClimbHandle != null)
		{
			return;
		}
		float num = (this.useIntensityCurve ? (Mathf.Clamp01(this.windIntensity - 0.5f) * 2f) : 1f);
		bool flag = false;
		Parasol parasol;
		if (character.data.currentItem && character.data.currentItem.TryGetComponent<Parasol>(ref parasol) && parasol.isOpen)
		{
			num *= 10f;
			flag = true;
		}
		if (character.refs.balloons.currentBalloonCount > 0)
		{
			int num2 = 2;
			if (flag)
			{
				num2 = 0;
			}
			num *= (float)(num2 + character.refs.balloons.currentBalloonCount);
		}
		Affliction affliction;
		if (character.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.LowGravity, out affliction))
		{
			num = 0f;
		}
		if (this.useRaycast && Physics.Raycast(character.Center, -this.currentWindDirection, ref this.hitInfo, this.maxRaycastDistance, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap)))
		{
			float num3 = Mathf.InverseLerp(this.minRaycastDistance, this.maxRaycastDistance, this.hitInfo.distance);
			float num4 = Mathf.Clamp01(Vector3.Dot(this.hitInfo.normal, this.currentWindDirection));
			num3 += 1f - num4;
			num *= Mathf.Clamp01(num3);
		}
		num *= ((character.data.fallSeconds >= 0.01f) ? this.ragdolledWindForceMult : 1f);
		num *= mult;
		character.AddForceAtPosition(this.currentWindDirection * this.windForce * this.windPlayerFactor * num, character.Center, this.forceRadius);
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00025F90 File Offset: 0x00024190
	private void AddWindForceToItem(Item item)
	{
		float num = (this.useIntensityCurve ? (Mathf.Clamp01(this.windIntensity - 0.5f) * 2f) : 1f);
		item.rig.AddForce(this.currentWindDirection * this.windForce * this.windItemFactor * num, 5);
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00025FF4 File Offset: 0x000241F4
	private Vector3 RandomWindDirection()
	{
		return Vector3.Lerp(Vector3.right * ((Random.value > 0.5f) ? 1f : (-1f)), Vector3.forward, 0.2f).normalized;
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x0002603C File Offset: 0x0002423C
	internal static float GetWindIntensityAtPoint(Vector3 point, float thresholdLower, float thresholdMargin)
	{
		float num = LightVolume.Instance().SamplePositionAlpha(point);
		float num2;
		if (num > thresholdLower + thresholdMargin)
		{
			num2 = 1f;
		}
		else if (num < thresholdLower)
		{
			num2 = 0f;
		}
		else
		{
			num2 = Util.RangeLerp(0f, 1f, thresholdLower, thresholdLower + thresholdMargin, num, true, null);
		}
		return num2;
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0002608D File Offset: 0x0002428D
	private float timeSpentWaiting
	{
		get
		{
			return this.timeUntilNextWind - this.untilSwitch;
		}
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x0002609C File Offset: 0x0002429C
	[PunRPC]
	private void RPCA_ToggleWind(bool set, Vector3 windDir, float untilSwitch)
	{
		this.windActive = set;
		this.untilSwitch = untilSwitch;
		this.timeUntilNextWind = untilSwitch;
		if (this.windActive)
		{
			this.currentWindDirection = windDir;
		}
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x000260C4 File Offset: 0x000242C4
	private float GetNextWindTime(bool windActive)
	{
		if (windActive)
		{
			return Random.Range(this.windTimeRangeOn.x, this.windTimeRangeOn.y);
		}
		if (this.debubEnable)
		{
			return 0f;
		}
		return Random.Range(this.windTimeRangeOff.x, this.windTimeRangeOff.y);
	}

	// Token: 0x0400069E RID: 1694
	public static WindChillZone instance;

	// Token: 0x0400069F RID: 1695
	[Range(0f, 1f)]
	public float StormProgress;

	// Token: 0x040006A0 RID: 1696
	[Range(0f, 1f)]
	public float timeUntilStorm;

	// Token: 0x040006A1 RID: 1697
	public float grabStaminaMultiplierDuringWind = 1f;

	// Token: 0x040006A2 RID: 1698
	public Vector2 windTimeRangeOn;

	// Token: 0x040006A3 RID: 1699
	public Vector2 windTimeRangeOff;

	// Token: 0x040006A4 RID: 1700
	[Range(0f, 1f)]
	public float lightVolumeSampleThreshold_lower;

	// Token: 0x040006A5 RID: 1701
	[Range(0f, 1f)]
	public float lightVolumeSampleThreshold_margin;

	// Token: 0x040006A6 RID: 1702
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x040006A7 RID: 1703
	public Bounds windZoneBounds;

	// Token: 0x040006A8 RID: 1704
	internal Vector3 currentWindDirection;

	// Token: 0x040006A9 RID: 1705
	private Color gizmoColor = new Color(0f, 0f, 1f, 0.5f);

	// Token: 0x040006AA RID: 1706
	private float timeUntilNextWind;

	// Token: 0x040006AB RID: 1707
	public float forceRadius = 2f;

	// Token: 0x040006AC RID: 1708
	public float delayBeforeForce = 2f;

	// Token: 0x040006AD RID: 1709
	public float ragdolledWindForceMult = 0.5f;

	// Token: 0x040006AE RID: 1710
	public bool windMovesItems;

	// Token: 0x040006AF RID: 1711
	private float untilSwitch;

	// Token: 0x040006B0 RID: 1712
	[FormerlySerializedAs("windChillPerSecond")]
	public float statusApplicationPerSecond = 0.01f;

	// Token: 0x040006B1 RID: 1713
	public float windForce = 15f;

	// Token: 0x040006B2 RID: 1714
	internal float hasBeenActiveFor;

	// Token: 0x040006B3 RID: 1715
	private PhotonView view;

	// Token: 0x040006B4 RID: 1716
	public bool setSlippy;

	// Token: 0x040006B5 RID: 1717
	[FormerlySerializedAs("characterInsideBounds")]
	public bool localCharacterInsideBounds;

	// Token: 0x040006B6 RID: 1718
	[FormerlySerializedAs("characterInsideBounds")]
	public bool observedCharacterInsideBounds;

	// Token: 0x040006B7 RID: 1719
	public bool windActive;

	// Token: 0x040006B8 RID: 1720
	public float windPlayerFactor;

	// Token: 0x040006B9 RID: 1721
	public bool debubEnable;

	// Token: 0x040006BA RID: 1722
	public float windItemFactor = 1f;

	// Token: 0x040006BB RID: 1723
	[Header("Curve")]
	public float windIntensity;

	// Token: 0x040006BC RID: 1724
	public AnimationCurve windIntensityCurve;

	// Token: 0x040006BD RID: 1725
	public bool useIntensityCurve;

	// Token: 0x040006BE RID: 1726
	public bool useRaycast;

	// Token: 0x040006BF RID: 1727
	public float maxRaycastDistance;

	// Token: 0x040006C0 RID: 1728
	public float minRaycastDistance;

	// Token: 0x040006C1 RID: 1729
	private float currentForceMult = 1f;

	// Token: 0x040006C2 RID: 1730
	private RaycastHit hitInfo;
}
