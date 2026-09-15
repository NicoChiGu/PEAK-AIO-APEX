using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200021A RID: 538
public class BreakableBridge : OnNetworkStart
{
	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000FDC RID: 4060 RVA: 0x0004ECE1 File Offset: 0x0004CEE1
	public bool LocalCharacterOnBridge
	{
		get
		{
			return Time.time - this.localTouchStamp < 0.2f;
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000FDD RID: 4061 RVA: 0x0004ECF6 File Offset: 0x0004CEF6
	private float DistanceToLocalPlayer
	{
		get
		{
			return Vector3.Distance(Character.localCharacter.Center, base.transform.position);
		}
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x0004ED14 File Offset: 0x0004CF14
	private void Awake()
	{
		this.jungleVine = base.GetComponent<JungleVine>();
		this.photonView = base.GetComponent<PhotonView>();
		this.source = base.GetComponent<AudioSource>();
		foreach (CollisionModifier collisionModifier in base.GetComponentsInChildren<CollisionModifier>())
		{
			collisionModifier.applyEffects = false;
			collisionModifier.onCollide = (Action<Character, CollisionModifier, Collision, Bodypart>)Delegate.Combine(collisionModifier.onCollide, new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnBridgeCollision));
		}
		this.rend = base.GetComponentInChildren<Renderer>();
		this.rend.material.SetFloat(BreakableBridge.JitterAmount, 0f);
		this.rend.material.SetFloat(BreakableBridge.AlphaClip, 0.01f);
		if (this.holdsPeople == 0)
		{
			this.holdsPeople = 5;
		}
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x0004EDD4 File Offset: 0x0004CFD4
	public override void NetworkStart()
	{
		this.holdsPeople = Random.Range(1, this.maxPeople);
		this.photonView.RPC("SyncHoldsPeopleRPC", 0, new object[] { this.holdsPeople });
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x0004EE10 File Offset: 0x0004D010
	private void Update()
	{
		if (this.isShaking)
		{
			this.source.pitch += 0.1f * Time.deltaTime;
			this.source.volume += 0.1f * Time.deltaTime;
			this.source.enabled = true;
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (this.isBreaking && !this.isShaking && !this.isFallen)
		{
			this.timeUntilBreak -= Time.deltaTime;
			if (this.timeUntilBreak < 0f)
			{
				this.photonView.RPC("ShakeBridge_Rpc", 0, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x0004EEC8 File Offset: 0x0004D0C8
	private void FixedUpdate()
	{
		this.peopleOnBridge = 0;
		if (this.debug)
		{
			Debug.Log(string.Format("FixedUpdate: {0}, peopleOnBridge: {1}", Time.frameCount, this.peopleOnBridge));
		}
		if (this.peopleOnBridgeDict.Keys.Count > 0)
		{
			this.cachedPeopleOnBridgeList = Enumerable.ToList<Character>(this.peopleOnBridgeDict.Keys);
			foreach (Character character in this.cachedPeopleOnBridgeList)
			{
				Dictionary<Character, float> dictionary = this.peopleOnBridgeDict;
				Character character2 = character;
				dictionary[character2] += Time.deltaTime;
				if (this.peopleOnBridgeDict[character] < 0.25f)
				{
					this.peopleOnBridge++;
				}
			}
		}
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x0004EFB4 File Offset: 0x0004D1B4
	private void OnDestroy()
	{
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x0004EFB6 File Offset: 0x0004D1B6
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient || newPlayer == PhotonNetwork.LocalPlayer)
		{
			return;
		}
		this.photonView.RPC("SyncHoldsPeopleRPC", newPlayer, new object[] { this.holdsPeople });
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x0004EFF4 File Offset: 0x0004D1F4
	[PunRPC]
	public void SyncHoldsPeopleRPC(int holdsPeople)
	{
		this.holdsPeople = holdsPeople;
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x0004F000 File Offset: 0x0004D200
	public void AddCollisionModifiers()
	{
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.AddComponent<CollisionModifier>();
		}
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x0004F030 File Offset: 0x0004D230
	private void OnBridgeCollision(Character character, CollisionModifier collider, Collision collision, Bodypart bodypart)
	{
		if (this.isBreaking)
		{
			return;
		}
		if (character == Character.localCharacter)
		{
			this.localTouchStamp = Time.time;
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (!this.peopleOnBridgeDict.TryAdd(character, 0f))
		{
			this.peopleOnBridgeDict[character] = 0f;
		}
		if (this.peopleOnBridge < this.holdsPeople)
		{
			return;
		}
		if (this.isShaking)
		{
			return;
		}
		if (this.holdsPeople >= this.peopleOnBridge)
		{
			return;
		}
		this.isBreaking = true;
		this.timeUntilBreak = Random.Range(2.5f, 7.5f);
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x0004F0D4 File Offset: 0x0004D2D4
	[PunRPC]
	private void ShakeBridge_Rpc()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		this.source.enabled = true;
		this.source.Play();
		if (!this.isShaking)
		{
			this.source.volume = 0.125f;
		}
		if (this.DistanceToLocalPlayer < this.startShakeDistance)
		{
			Debug.Log(string.Format("start shake {0}", this.startShakeAmount));
			GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount, 0.2f, 15f);
		}
		base.StartCoroutine(this.<ShakeBridge_Rpc>g__RockShake|44_0());
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x0004F170 File Offset: 0x0004D370
	[PunRPC]
	private void Fall_Rpc()
	{
		base.StartCoroutine(this.<Fall_Rpc>g__DestroyRoutine|45_0());
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x0004F25C File Offset: 0x0004D45C
	[CompilerGenerated]
	private IEnumerator <ShakeBridge_Rpc>g__RockShake|44_0()
	{
		Debug.Log("Start shaking");
		float duration = 0f;
		float timeUntilShake = 0f;
		this.rend.material.SetFloat(BreakableBridge.JitterAmount, 1f);
		while (duration < this.fallTime)
		{
			timeUntilShake -= Time.deltaTime;
			if (this.LocalCharacterOnBridge && timeUntilShake <= 0f)
			{
				GamefeelHandler.instance.AddPerlinShake(this.climbingScreenShake, 0.2f, 15f);
				Debug.Log("Clime shake");
				timeUntilShake = this.screenShakeTickTime;
			}
			Vector3 vector = Vector2.zero;
			vector.x += Mathf.PerlinNoise1D(100f + duration * this.shakeScale) * this.axisMul.x;
			vector.y += Mathf.PerlinNoise1D(10300f + duration * this.shakeScale) * this.axisMul.y;
			vector.z += Mathf.PerlinNoise1D(1340f + duration * this.shakeScale) * this.axisMul.z;
			vector *= this.amount;
			duration += Time.deltaTime;
			yield return null;
		}
		this.rend.material.SetFloat(BreakableBridge.JitterAmount, 0f);
		Debug.Log("Done shaking");
		if (this.isShaking)
		{
			for (int i = 0; i < this.breakSfx.Length; i++)
			{
				this.breakSfx[i].Play(base.transform.position);
			}
		}
		this.isShaking = false;
		this.fullMesh.localPosition = ExtDataTypes.ToVec(0);
		this.source.volume = 0f;
		this.source.Stop();
		if (this.photonView.IsMine)
		{
			this.photonView.RPC("Fall_Rpc", 0, Array.Empty<object>());
		}
		yield break;
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x0004F26B File Offset: 0x0004D46B
	[CompilerGenerated]
	private IEnumerator <Fall_Rpc>g__DestroyRoutine|45_0()
	{
		this.isFallen = true;
		Object.DestroyImmediate(this.jungleVine.colliderRoot.gameObject);
		if (this.breakParticles != null)
		{
			this.breakParticles.Play();
		}
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			normalizedTime += Time.deltaTime * 0.7f;
			this.rend.material.SetFloat(BreakableBridge.BreakAmount, normalizedTime);
			yield return null;
		}
		Debug.Log(string.Format("Destroy: {0}", base.gameObject), base.gameObject);
		yield return null;
		yield break;
	}

	// Token: 0x04000E36 RID: 3638
	private static readonly int JitterAmount = Shader.PropertyToID("_JitterAmount");

	// Token: 0x04000E37 RID: 3639
	private static readonly int BreakAmount = Shader.PropertyToID("_BreakAmount");

	// Token: 0x04000E38 RID: 3640
	private static readonly int AlphaClip = Shader.PropertyToID("_AlphaClip");

	// Token: 0x04000E39 RID: 3641
	public int maxPeople = 5;

	// Token: 0x04000E3A RID: 3642
	public SFX_Instance[] breakSfx;

	// Token: 0x04000E3B RID: 3643
	[Range(0f, 1f)]
	public float breakPoint = 0.4f;

	// Token: 0x04000E3C RID: 3644
	[Range(0f, 1f)]
	public float breakChance = 0.5f;

	// Token: 0x04000E3D RID: 3645
	public Vector3 axisMul = new Vector3(1f, 1f, 1f);

	// Token: 0x04000E3E RID: 3646
	public float shakeScale = 30f;

	// Token: 0x04000E3F RID: 3647
	public float fallTime = 5f;

	// Token: 0x04000E40 RID: 3648
	public float amount = 1f;

	// Token: 0x04000E41 RID: 3649
	public float startShakeDistance = 10f;

	// Token: 0x04000E42 RID: 3650
	public float startShakeAmount = 400f;

	// Token: 0x04000E43 RID: 3651
	public float climbingScreenShake = 240f;

	// Token: 0x04000E44 RID: 3652
	public float screenShakeTickTime = 0.2f;

	// Token: 0x04000E45 RID: 3653
	public bool debug;

	// Token: 0x04000E46 RID: 3654
	public bool isShaking;

	// Token: 0x04000E47 RID: 3655
	public float localTouchStamp;

	// Token: 0x04000E48 RID: 3656
	public int holdsPeople;

	// Token: 0x04000E49 RID: 3657
	public int peopleOnBridge;

	// Token: 0x04000E4A RID: 3658
	public Transform fullMesh;

	// Token: 0x04000E4B RID: 3659
	public ParticleSystem breakParticles;

	// Token: 0x04000E4C RID: 3660
	private readonly Dictionary<Character, float> peopleOnBridgeDict = new Dictionary<Character, float>();

	// Token: 0x04000E4D RID: 3661
	private PhotonView photonView;

	// Token: 0x04000E4E RID: 3662
	private Renderer rend;

	// Token: 0x04000E4F RID: 3663
	private AudioSource source;

	// Token: 0x04000E50 RID: 3664
	private JungleVine jungleVine;

	// Token: 0x04000E51 RID: 3665
	private List<Character> cachedPeopleOnBridgeList = new List<Character>();

	// Token: 0x04000E52 RID: 3666
	private float timeUntilBreak;

	// Token: 0x04000E53 RID: 3667
	private bool isBreaking;

	// Token: 0x04000E54 RID: 3668
	private bool isFallen;
}
