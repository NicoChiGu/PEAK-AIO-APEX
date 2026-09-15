using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class BeeSwarm : MonoBehaviourPun
{
	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000029 RID: 41 RVA: 0x000026DC File Offset: 0x000008DC
	private bool canSeeHive
	{
		get
		{
			return this.currentHiveDistance <= this.maxHiveDistance;
		}
	}

	// Token: 0x0600002A RID: 42 RVA: 0x000026EF File Offset: 0x000008EF
	protected void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastSawBeehivePosition = base.transform.position;
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002710 File Offset: 0x00000910
	private void Start()
	{
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<BugPhobiaSetting>();
		if (this.setting != null && this.setting.Value == 1)
		{
			this.beeParticles.GetComponent<ParticleSystemRenderer>().material = this.bingBongMaterial;
		}
	}

	// Token: 0x0600002C RID: 44 RVA: 0x0000275E File Offset: 0x0000095E
	public void SetBeehive(Beehive hive)
	{
		this.beehiveID = hive.instanceID;
		this.beehive = hive;
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002774 File Offset: 0x00000974
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.defaultAggroDistance);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.hiveAggroDistance);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, this.deAggroDistance);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.transform.position, this.maxHiveDistance);
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002804 File Offset: 0x00000A04
	private void Update()
	{
		if (this.dispersing)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (base.photonView.IsMine)
		{
			bool flag = this.beehiveDangerTick > 0f;
			if (this.beesAngry != flag)
			{
				base.photonView.RPC("SetBeesAngryRPC", 3, new object[] { flag });
			}
		}
		this.stingerField.statusAmountPerSecond = (this.beesAngry ? this.poisonOverTimeAngry : this.poisonOverTime);
		if (this.beehive == null)
		{
			this.TryGetBeehive();
		}
		this.UpdateAggro();
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (this.beesAngry)
		{
			this.beehiveDangerTick = Mathf.Max(this.beehiveDangerTick - Time.deltaTime, 0f);
		}
	}

	// Token: 0x0600002F RID: 47 RVA: 0x000028D3 File Offset: 0x00000AD3
	[PunRPC]
	public void SetBeesAngryRPC(bool angry)
	{
		Debug.Log(string.Format("Setting bees angry: {0}", angry));
		this.beesAngry = angry;
	}

	// Token: 0x06000030 RID: 48 RVA: 0x000028F1 File Offset: 0x00000AF1
	private void FixedUpdate()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		this.UpdateBeehavior();
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002904 File Offset: 0x00000B04
	private void UpdateBeehavior()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		this.currentHiveDistance = ((this.beehive == null) ? float.MaxValue : Vector3.Distance(this.beehive.transform.position, base.transform.position));
		this.currentLastSawHiveDistance = Vector3.Distance(this.lastSawBeehivePosition, base.transform.position);
		if (this.currentAggroCharacter == null)
		{
			this.rb.AddForce((this.lastSawBeehivePosition - base.transform.position).normalized * (this.movementForce * Time.fixedDeltaTime), 5);
			this.UpdateDisperse();
			this.beeAngryLoop.volume = Mathf.Lerp(this.beeAngryLoop.volume, 0f, Time.deltaTime * 2f);
			this.beeIdleLoop.volume = Mathf.Lerp(this.beeIdleLoop.volume, 0.75f, Time.deltaTime * 2f);
			return;
		}
		float num = (this.beesAngry ? this.movementForceAngry : this.movementForce);
		this.rb.AddForce((this.currentAggroCharacter.Center - base.transform.position).normalized * (num * Time.fixedDeltaTime), 5);
		this.beeAngryLoop.volume = Mathf.Lerp(this.beeAngryLoop.volume, 0.75f, Time.deltaTime * 2f);
		this.beeIdleLoop.volume = Mathf.Lerp(this.beeIdleLoop.volume, 0f, Time.deltaTime * 2f);
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002AC8 File Offset: 0x00000CC8
	private void UpdateDisperse()
	{
		if (this.currentAggroCharacter == null && !this.canSeeHive)
		{
			this.beeDispersalTick += Time.fixedDeltaTime;
			if (this.beeDispersalTick >= this.beesDispersalTime)
			{
				this.Disperse();
			}
			return;
		}
		this.beeDispersalTick = 0f;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002B1D File Offset: 0x00000D1D
	private void GetAngry(float time)
	{
		this.beehiveDangerTick = time;
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002B26 File Offset: 0x00000D26
	public void HiveDestroyed(Vector3 atPosition)
	{
		if (Vector3.Distance(base.transform.position, atPosition) <= this.hiveAggroDistance)
		{
			this.hiveDestroyed = true;
			this.lastSawBeehivePosition = atPosition;
			this.GetAngry(this.beesAngerTimeHiveDestroyed);
		}
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00002B5B File Offset: 0x00000D5B
	private void Disperse()
	{
		base.photonView.RPC("DisperseRPC", 0, Array.Empty<object>());
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002B73 File Offset: 0x00000D73
	[PunRPC]
	public void DisperseRPC()
	{
		this.dispersing = true;
		base.StartCoroutine(this.DisperseRoutine());
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00002B89 File Offset: 0x00000D89
	private IEnumerator DisperseRoutine()
	{
		float tick = 0f;
		if (this.stingerField)
		{
			Object.Destroy(this.stingerField.gameObject);
		}
		while (tick < 1f)
		{
			ParticleSystem.EmissionModule emission = this.beeParticles.emission;
			emission.rateOverTimeMultiplier = Mathf.Max(emission.rateOverTimeMultiplier - Time.deltaTime, 0f);
			float num = Mathf.Max(this.beeForceField.gravity.constantMin - Time.deltaTime, 0f);
			float num2 = Mathf.Max(this.beeForceField.gravity.constantMax - Time.deltaTime, 0f);
			this.beeForceField.gravity = new ParticleSystem.MinMaxCurve(num, num2);
			tick += Time.deltaTime;
			yield return null;
		}
		while (tick < 4f)
		{
			tick += Time.deltaTime;
			yield return null;
		}
		if (base.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		yield break;
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002B98 File Offset: 0x00000D98
	private void UpdateAggro()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		this.TryDeAggro();
		if (this.beehive != null && this.canSeeHive)
		{
			this.lastSawBeehivePosition = this.beehive.transform.position;
			if (this.beehive.item.holderCharacter != null)
			{
				this.beehiveDangerTick = this.beesAngerTimeHiveStolen;
				this.currentAggroCharacter = this.beehive.item.holderCharacter;
				return;
			}
		}
		if (this.currentAggroCharacter == null)
		{
			float num = float.MaxValue;
			Character character = null;
			if (this.beehive != null && this.currentLastSawHiveDistance > this.maxHiveDistance - this.hiveAggroDistance)
			{
				return;
			}
			float num2 = (this.beesAngry ? this.hiveAggroDistance : this.defaultAggroDistance);
			for (int i = 0; i < Character.AllCharacters.Count; i++)
			{
				Character character2 = Character.AllCharacters[i];
				float num3 = Vector3.Distance(character2.Center, base.transform.position);
				if (character2.data.fullyConscious && num3 < num2 && num3 < num)
				{
					num = num3;
					character = character2;
				}
			}
			this.currentAggroCharacter = character;
		}
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002CD4 File Offset: 0x00000ED4
	private void TryDeAggro()
	{
		if (this.currentAggroCharacter)
		{
			if (!this.currentAggroCharacter.data.fullyConscious)
			{
				this.currentAggroCharacter = null;
				return;
			}
			if (!this.hiveDestroyed && this.currentLastSawHiveDistance > this.maxHiveDistance)
			{
				this.currentAggroCharacter = null;
				return;
			}
			float num = Vector3.Distance(this.currentAggroCharacter.Center, base.transform.position);
			float num2 = (this.beesAngry ? this.hiveAggroDistance : this.deAggroDistance);
			if (num > num2)
			{
				this.currentAggroCharacter = null;
			}
		}
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00002D64 File Offset: 0x00000F64
	private void TryGetBeehive()
	{
		Beehive beehive = Beehive.GetBeehive(this.beehiveID);
		if (beehive != null)
		{
			this.beehive = beehive;
			beehive.currentBees = this;
			Debug.Log(string.Format("Reattached to beehive #{0}", this.beehiveID));
		}
	}

	// Token: 0x0400000E RID: 14
	public int beehiveID;

	// Token: 0x0400000F RID: 15
	public Beehive beehive;

	// Token: 0x04000010 RID: 16
	[SerializeField]
	private float beehiveDangerTick;

	// Token: 0x04000011 RID: 17
	[SerializeField]
	private float beeDispersalTick;

	// Token: 0x04000012 RID: 18
	private float beehiveDangerTime;

	// Token: 0x04000013 RID: 19
	private Rigidbody rb;

	// Token: 0x04000014 RID: 20
	public Character currentAggroCharacter;

	// Token: 0x04000015 RID: 21
	public float defaultAggroDistance;

	// Token: 0x04000016 RID: 22
	public float hiveAggroDistance;

	// Token: 0x04000017 RID: 23
	public float deAggroDistance;

	// Token: 0x04000018 RID: 24
	public float maxHiveDistance;

	// Token: 0x04000019 RID: 25
	public float movementForce;

	// Token: 0x0400001A RID: 26
	public float movementForceAngry;

	// Token: 0x0400001B RID: 27
	public float beesAngerTimeHiveStolen = 8f;

	// Token: 0x0400001C RID: 28
	public float beesAngerTimeHiveDestroyed = 20f;

	// Token: 0x0400001D RID: 29
	public float beesDispersalTime = 6f;

	// Token: 0x0400001E RID: 30
	public float poisonOverTime;

	// Token: 0x0400001F RID: 31
	public float poisonOverTimeAngry;

	// Token: 0x04000020 RID: 32
	public StatusField stingerField;

	// Token: 0x04000021 RID: 33
	public ParticleSystem beeParticles;

	// Token: 0x04000022 RID: 34
	public Material bingBongMaterial;

	// Token: 0x04000023 RID: 35
	public ParticleSystemForceField beeForceField;

	// Token: 0x04000024 RID: 36
	private Vector3 lastSawBeehivePosition;

	// Token: 0x04000025 RID: 37
	private float currentHiveDistance;

	// Token: 0x04000026 RID: 38
	private float currentLastSawHiveDistance;

	// Token: 0x04000027 RID: 39
	public bool beesAngry;

	// Token: 0x04000028 RID: 40
	public AudioSource beeIdleLoop;

	// Token: 0x04000029 RID: 41
	public AudioSource beeAngryLoop;

	// Token: 0x0400002A RID: 42
	private BugPhobiaSetting setting;

	// Token: 0x0400002B RID: 43
	private bool hiveDestroyed;

	// Token: 0x0400002C RID: 44
	private bool dispersing;
}
