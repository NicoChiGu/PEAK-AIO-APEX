using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001FB RID: 507
public class AmbienceAudio : MonoBehaviour
{
	// Token: 0x06000F3D RID: 3901 RVA: 0x0004A8D8 File Offset: 0x00048AD8
	private void Start()
	{
		this.ambienceVolumes = base.GetComponent<Animator>();
		this.dayNight = Object.FindAnyObjectByType<DayNightManager>();
		this.character = base.GetComponentInParent<Character>();
		this.stingerSource.clip = this.startStinger[Random.Range(0, this.startStinger.Length)];
		this.stingerSource.Play();
		this.volcanoObj = GameObject.Find("VolcanoModel");
		if (GameObject.Find("Airport"))
		{
			base.gameObject.SetActive(false);
			if (this.voice)
			{
				this.reverbFilter.enabled = false;
				this.echoFilter.enabled = false;
				this.lowPassFilter.enabled = false;
			}
		}
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x0004A994 File Offset: 0x00048B94
	private void Update()
	{
		if (this.character.inAirport)
		{
			return;
		}
		this.naturelessTerrain -= 6f * Time.deltaTime;
		if (this.naturelessTerrain > 0f)
		{
			this.ambienceVolumes.SetBool("Natureless", true);
		}
		if (this.naturelessTerrain < 0f)
		{
			this.ambienceVolumes.SetBool("Natureless", false);
		}
		this.ambienceVolumes.SetBool("Tomb", this.inTomb);
		try
		{
			float num = math.saturate(LightVolume.Instance().SamplePositionAlpha(base.transform.position));
			num = math.saturate(1f - math.remap(0f, 0.3f, 0f, 1f, num));
			this.reverb.room = (int)math.remap(0f, 1f, -4000f, -100f, num);
		}
		catch
		{
			Debug.LogError("You probably need to bake the lightmap");
		}
		if (this.volcanoObj)
		{
			this.vulcanoT -= Time.deltaTime;
			if (this.vulcanoT <= 0f)
			{
				this.volcano = false;
				this.vulcanoT = 0f;
				this.reverb.enabled = true;
			}
			if (this.vulcanoT > 0f)
			{
				this.volcano = true;
				this.reverb.enabled = false;
			}
			if (Vector3.Distance(base.transform.position, this.volcanoObj.transform.position) < 200f)
			{
				this.vulcanoT = 10f;
			}
			this.ambienceVolumes.SetBool("Volcano", this.volcano);
		}
		if (this.ambienceVolumes && this.dayNight)
		{
			this.ambienceVolumes.SetFloat("Height", base.transform.position.y);
			this.ambienceVolumes.SetFloat("Time", this.dayNight.timeOfDay);
			if (base.transform.position.z > this.alpineStingerZ - 500f && !this.alpineSunTime1 && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
			{
				this.ambienceVolumes.SetBool("Desert", true);
			}
			if (!this.inTomb)
			{
				if (this.dayNight.timeOfDay > 5.5f && this.dayNight.timeOfDay < 6.5f && this.t != 1)
				{
					this.t = 1;
					this.stingerSource.clip = this.sunRiseStinger[Random.Range(0, this.sunRiseStinger.Length)];
					if (base.transform.position.z > this.tropicsStingerZ - 500f && !this.tropicsSunTime2)
					{
						if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Tropics))
						{
							this.stingerSource.clip = this.tropicsSunrise;
						}
						else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Roots))
						{
							this.stingerSource.clip = this.rootsSunrise;
						}
						if (this.priorityMusicTimer <= 0f)
						{
							this.tropicsSunTime2 = true;
						}
					}
					if (base.transform.position.z > this.alpineStingerZ - 500f && !this.alpineSunTime2)
					{
						if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Alpine))
						{
							this.stingerSource.clip = this.alpineSunrise;
						}
						else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
						{
							this.stingerSource.clip = this.desertSunrise;
						}
						if (this.priorityMusicTimer <= 0f)
						{
							this.alpineSunTime2 = true;
						}
					}
					if (!this.volcano)
					{
						this.stingerSource.Play();
					}
				}
				if (this.dayNight.timeOfDay > 19.5f && this.dayNight.timeOfDay < 20f && this.t != 2)
				{
					this.t = 2;
					this.stingerSource.clip = this.sunSetStinger[Random.Range(0, this.sunSetStinger.Length)];
					if (base.transform.position.z > this.tropicsStingerZ - 500f && !this.tropicsSunTime1)
					{
						if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Tropics))
						{
							this.stingerSource.clip = this.tropicsSunset;
						}
						else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Roots))
						{
							this.stingerSource.clip = this.rootsSunset;
						}
						if (this.priorityMusicTimer <= 0f)
						{
							this.tropicsSunTime1 = true;
						}
					}
					if (base.transform.position.z > this.alpineStingerZ - 500f && !this.alpineSunTime1)
					{
						if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Alpine))
						{
							this.stingerSource.clip = this.alpineSunset;
						}
						else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
						{
							this.stingerSource.clip = this.desertSunset;
						}
						this.ambienceVolumes.SetBool("Desert", true);
						if (this.priorityMusicTimer <= 0f)
						{
							this.alpineSunTime1 = true;
						}
					}
					if (!this.volcano)
					{
						this.stingerSource.Play();
					}
				}
				if (this.dayNight.timeOfDay > 21.2f && this.dayNight.timeOfDay < 26f && this.t != 3)
				{
					this.t = 3;
					this.stingerSource.clip = this.nightStinger[Random.Range(0, this.nightStinger.Length)];
					if (!this.volcano)
					{
						this.stingerSource.Play();
					}
				}
			}
		}
		this.priorityMusicTimer -= Time.deltaTime;
		CharacterData data = this.character.data;
		if (data.sinceDead > 0.5f && !Character.localCharacter.warping && !data.passedOut && !data.dead && !data.fullyPassedOut && !this.inTomb)
		{
			if (base.transform.position.z > this.beachStingerZ && !this.playedBeach)
			{
				this.playedBeach = true;
				this.mainMusic.clip = this.climbStingerBeach;
				this.mainMusic.volume = 0.35f;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played beach stinger");
			}
			if (base.transform.position.z > this.tropicsStingerZ && !this.playedTropicsOrRoots && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Tropics))
			{
				this.playedTropicsOrRoots = true;
				this.mainMusic.clip = this.climbStingerTropics;
				this.mainMusic.volume = 0.5f;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played tropics stinger");
			}
			if (base.transform.position.z > this.tropicsStingerZ && !this.playedTropicsOrRoots && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Roots))
			{
				this.playedTropicsOrRoots = true;
				this.mainMusic.clip = this.climbStingerRoots;
				this.mainMusic.volume = 0.5f;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played tropics stinger");
			}
			if (base.transform.position.z > this.alpineStingerZ && !this.playedAlpineOrMesa && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Alpine))
			{
				this.mainMusic.volume = 0.4f;
				this.playedAlpineOrMesa = true;
				this.mainMusic.clip = this.climbStingerAlpine;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played alpine stinger");
			}
			if (base.transform.position.z > this.alpineStingerZ && !this.playedAlpineOrMesa && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
			{
				this.mainMusic.volume = 0.4f;
				this.playedAlpineOrMesa = true;
				this.mainMusic.clip = this.climbStingerMesa;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played mesa stinger");
			}
			if (base.transform.position.z > this.calderaStingerZ && !this.playedCaldera)
			{
				if (!this.volcanoObj)
				{
					this.volcanoObj = GameObject.Find("VolcanoModel");
				}
				this.mainMusic.volume = 0.75f;
				this.playedCaldera = true;
				this.mainMusic.clip = this.climbStingerCaldera;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played caldera stinger");
			}
			if (base.transform.position.y > this.kilnStingerY && !this.playedKiln)
			{
				this.inKiln -= Time.deltaTime;
				if (this.inKiln < -2f)
				{
					this.mainMusic.volume = 0.6f;
					this.playedKiln = true;
					this.mainMusic.clip = this.climbStingerKiln;
					this.mainMusic.Play();
					this.priorityMusicTimer = 120f;
					Debug.Log("Played kiln stinger");
				}
			}
			else
			{
				this.inKiln = 0f;
			}
			if (base.transform.position.z > this.peaksTingerZ && !this.playedPeak)
			{
				this.mainMusic.volume = 1f;
				this.playedPeak = true;
				this.mainMusic.clip = this.climbStingerPeak;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
			}
		}
		else
		{
			this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0f, 0.05f);
			this.mainMusic.volume = Mathf.Lerp(this.mainMusic.volume, 0f, 0.05f);
		}
		if (this.priorityMusicTimer > 0f)
		{
			this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0f, 0.05f);
		}
		if (this.priorityMusicTimer <= 0f)
		{
			this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0.35f, 0.05f);
			this.priorityMusicTimer = 0f;
		}
		if (this.inTomb)
		{
			if (base.transform.position.z > 700f && !this.playedTomb)
			{
				this.playedTomb = true;
				this.mainMusic.clip = this.tombClimb;
				this.mainMusic.Play();
			}
			this.mainMusic.volume = 0.5f;
		}
		if (base.transform.position.y > 450f)
		{
			this.inTomb = false;
		}
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x0004B48C File Offset: 0x0004968C
	private void Coverage()
	{
		float num = 8f;
		this.ceiling = false;
		if (Physics.Linecast(base.transform.position, base.transform.position + Vector3.up * 8f * num, ref this.hit, this.layer))
		{
			this.ceiling = true;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * num, ref this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * -num, ref this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.right * num, ref this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.right * -num, ref this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.up * num * 4f, ref this.hit, this.layer))
		{
			this.coverage += 2f;
		}
	}

	// Token: 0x04000D4A RID: 3402
	public float obstruction;

	// Token: 0x04000D4B RID: 3403
	private float coverage;

	// Token: 0x04000D4C RID: 3404
	public bool ceiling;

	// Token: 0x04000D4D RID: 3405
	public bool inTomb;

	// Token: 0x04000D4E RID: 3406
	public LayerMask layer;

	// Token: 0x04000D4F RID: 3407
	private RaycastHit hit;

	// Token: 0x04000D50 RID: 3408
	public AudioReverbZone reverb;

	// Token: 0x04000D51 RID: 3409
	private DayNightManager dayNight;

	// Token: 0x04000D52 RID: 3410
	private Animator ambienceVolumes;

	// Token: 0x04000D53 RID: 3411
	private int t;

	// Token: 0x04000D54 RID: 3412
	public AudioSource stingerSource;

	// Token: 0x04000D55 RID: 3413
	public AudioClip[] startStinger;

	// Token: 0x04000D56 RID: 3414
	public AudioClip[] sunRiseStinger;

	// Token: 0x04000D57 RID: 3415
	public AudioClip[] sunSetStinger;

	// Token: 0x04000D58 RID: 3416
	public AudioClip[] nightStinger;

	// Token: 0x04000D59 RID: 3417
	public bool volcano;

	// Token: 0x04000D5A RID: 3418
	public GameObject volcanoObj;

	// Token: 0x04000D5B RID: 3419
	public float vulcanoT;

	// Token: 0x04000D5C RID: 3420
	public float naturelessTerrain;

	// Token: 0x04000D5D RID: 3421
	public AudioSource mainMusic;

	// Token: 0x04000D5E RID: 3422
	public AudioClip climbStingerBeach;

	// Token: 0x04000D5F RID: 3423
	private bool playedBeach;

	// Token: 0x04000D60 RID: 3424
	public AudioClip climbStingerTropics;

	// Token: 0x04000D61 RID: 3425
	public AudioClip climbStingerRoots;

	// Token: 0x04000D62 RID: 3426
	private bool playedTropicsOrRoots;

	// Token: 0x04000D63 RID: 3427
	public AudioClip climbStingerAlpine;

	// Token: 0x04000D64 RID: 3428
	private bool playedAlpineOrMesa;

	// Token: 0x04000D65 RID: 3429
	public AudioClip climbStingerMesa;

	// Token: 0x04000D66 RID: 3430
	public AudioClip climbStingerCaldera;

	// Token: 0x04000D67 RID: 3431
	private bool playedCaldera;

	// Token: 0x04000D68 RID: 3432
	public AudioClip climbStingerKiln;

	// Token: 0x04000D69 RID: 3433
	private bool playedKiln;

	// Token: 0x04000D6A RID: 3434
	public AudioClip climbStingerPeak;

	// Token: 0x04000D6B RID: 3435
	private bool playedPeak;

	// Token: 0x04000D6C RID: 3436
	public AudioClip tombClimb;

	// Token: 0x04000D6D RID: 3437
	public AudioSource bingBongStatue;

	// Token: 0x04000D6E RID: 3438
	private float priorityMusicTimer;

	// Token: 0x04000D6F RID: 3439
	public float beachStingerZ;

	// Token: 0x04000D70 RID: 3440
	public float tropicsStingerZ;

	// Token: 0x04000D71 RID: 3441
	public float alpineStingerZ;

	// Token: 0x04000D72 RID: 3442
	public float calderaStingerZ;

	// Token: 0x04000D73 RID: 3443
	public float kilnStingerY;

	// Token: 0x04000D74 RID: 3444
	public float peaksTingerZ;

	// Token: 0x04000D75 RID: 3445
	public Transform voice;

	// Token: 0x04000D76 RID: 3446
	public AudioReverbFilter reverbFilter;

	// Token: 0x04000D77 RID: 3447
	public AudioEchoFilter echoFilter;

	// Token: 0x04000D78 RID: 3448
	public AudioLowPassFilter lowPassFilter;

	// Token: 0x04000D79 RID: 3449
	private float inKiln;

	// Token: 0x04000D7A RID: 3450
	private bool tropicsSunTime1;

	// Token: 0x04000D7B RID: 3451
	private bool tropicsSunTime2;

	// Token: 0x04000D7C RID: 3452
	public AudioClip tropicsSunrise;

	// Token: 0x04000D7D RID: 3453
	public AudioClip tropicsSunset;

	// Token: 0x04000D7E RID: 3454
	public AudioClip rootsSunrise;

	// Token: 0x04000D7F RID: 3455
	public AudioClip rootsSunset;

	// Token: 0x04000D80 RID: 3456
	private bool alpineSunTime1;

	// Token: 0x04000D81 RID: 3457
	private bool alpineSunTime2;

	// Token: 0x04000D82 RID: 3458
	public AudioClip alpineSunrise;

	// Token: 0x04000D83 RID: 3459
	public AudioClip alpineSunset;

	// Token: 0x04000D84 RID: 3460
	public AudioClip desertSunrise;

	// Token: 0x04000D85 RID: 3461
	public AudioClip desertSunset;

	// Token: 0x04000D86 RID: 3462
	private bool playedTomb;

	// Token: 0x04000D87 RID: 3463
	private Character character;
}
