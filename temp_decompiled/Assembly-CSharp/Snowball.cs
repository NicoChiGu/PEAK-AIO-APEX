using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200012B RID: 299
public class Snowball : ItemComponent
{
	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000970 RID: 2416 RVA: 0x0003215C File Offset: 0x0003035C
	private float ScaledAngularDrag
	{
		get
		{
			return this.defaultAngularDrag;
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000971 RID: 2417 RVA: 0x00032164 File Offset: 0x00030364
	private bool CanGrow
	{
		get
		{
			return this.item.rig.linearVelocity.magnitude > this.minimumSpeedToGrow;
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000972 RID: 2418 RVA: 0x00032191 File Offset: 0x00030391
	private float TimeSpentBraking
	{
		get
		{
			return Time.time - this._timeBrakingStarted;
		}
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0003219F File Offset: 0x0003039F
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x000321A7 File Offset: 0x000303A7
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x000321B0 File Offset: 0x000303B0
	private void Init()
	{
		if (this._isInitialized)
		{
			return;
		}
		this._isInitialized = true;
		this._defaultPhysicsMaterial = this.physicalCollider.sharedMaterial;
		this.defaultAngularDrag = this.item.rig.angularDamping;
		this._leftHandPosition = this.LeftHand.transform.localPosition;
		this._rightHandPosition = this.RightHand.transform.localPosition;
		this._startTime = Time.time;
		this._timeLastTouchedScout = Time.time + 0.2f;
		this._timeLastCollided = Time.time + 0.2f;
		this._timeBrakingStarted = Time.time + 0.2f;
		this._previousPosition = default(Vector3?);
		this.scaleSyncer = base.GetComponent<ItemScaleSyncer>();
		this.scaleSyncer.InitScale();
		this.terrainLayer = LayerMask.NameToLayer("Terrain");
		this.characterLayer = LayerMask.NameToLayer("Character");
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x000322A1 File Offset: 0x000304A1
	public override void OnEnable()
	{
		base.OnEnable();
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Combine(item.OnStateChange, new Action<ItemState>(this.HandleStateChange));
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x000322D0 File Offset: 0x000304D0
	public override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x000322D8 File Offset: 0x000304D8
	private void Update()
	{
		if (Time.time - this._startTime < 0.2f)
		{
			return;
		}
		if (this.item.itemState != ItemState.Ground)
		{
			this._timeLastTouchedScout = Time.time;
			this._timeBrakingStarted = Time.time;
			this._previousPosition = default(Vector3?);
			return;
		}
		this.ProcessLastCollision();
		bool flag = this.scaleSyncer.currentScale < this.maxScaleToPickUp;
		this.item.blockInteraction = !flag;
		if (this.item.itemState != ItemState.Ground || Time.time - this._timeLastCollided > 1f)
		{
			this.item.rig.angularDamping = this.ScaledAngularDrag;
			this.physicalCollider.material.dynamicFriction = this._defaultPhysicsMaterial.dynamicFriction;
			return;
		}
		if (this.item.rig.isKinematic)
		{
			return;
		}
		if (this.item.rig.linearVelocity.magnitude >= this.minimumSpeedToGrow)
		{
			this._timeBrakingStarted = Time.time;
		}
		float num = Time.time - this._timeBrakingStarted;
		float num2 = Mathf.Clamp01(num / this.timeBeforeFullStop);
		this.physicalCollider.material.dynamicFriction = Mathf.Lerp(this._defaultPhysicsMaterial.dynamicFriction, 1000f, num2);
		this.item.rig.angularDamping = Mathf.Lerp(this.ScaledAngularDrag, this.BrakingAngularDrag, num2);
		if (num > this.timeBeforeFullStop && this.item.rig.linearVelocity.sqrMagnitude < 0.5f)
		{
			this.item.rig.linearVelocity = Vector3.zero;
			this.item.rig.angularVelocity = Vector3.zero;
			this.item.rig.isKinematic = this.scaleSyncer.currentScale > 2f;
		}
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x000324C0 File Offset: 0x000306C0
	private void FixedUpdate()
	{
		if (Time.time - this._startTime < 0.2f)
		{
			return;
		}
		if (this.flatCollider.enabled)
		{
			this.flatCollider.transform.parent.rotation = Quaternion.identity;
		}
		bool isKinematic = this.item.rig.isKinematic;
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x00032519 File Offset: 0x00030719
	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer != this.characterLayer)
		{
			return;
		}
		this._timeLastTouchedScout = Time.time;
		this.item.rig.isKinematic = false;
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x0003254B File Offset: 0x0003074B
	private void OnCollisionStay(Collision other)
	{
		if (other.gameObject.layer != this.characterLayer)
		{
			this._unprocessedContact = other;
			return;
		}
		this._timeLastTouchedScout = Time.time;
		this.item.rig.isKinematic = false;
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00032584 File Offset: 0x00030784
	private void HandleStateChange(ItemState state)
	{
		this.Init();
		if (state == ItemState.Held)
		{
			this.UpdateForBeingHeld();
		}
		else
		{
			this.UpdateMass();
		}
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Remove(item.OnStateChange, new Action<ItemState>(this.HandleStateChange));
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x000325D0 File Offset: 0x000307D0
	private void UpdateForBeingHeld()
	{
		this.flatCollider.enabled = false;
		float currentScale = this.scaleSyncer.currentScale;
		float num = Mathf.Min(this.maxCarriedScale, currentScale);
		this.LeftHand.transform.localPosition = this._leftHandPosition * num;
		this.RightHand.transform.localPosition = this._rightHandPosition * num;
		this.physicalCollider.transform.parent.localScale = ((currentScale > this.maxCarriedScale) ? (this.maxCarriedScale / currentScale * Vector3.one) : Vector3.one);
		this.UpdateMass();
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00032678 File Offset: 0x00030878
	private void UpdateMass()
	{
		float num = this.scaleSyncer.currentScale * this.physicalCollider.radius;
		this.item.rig.mass = 4.1887903f * num * num * num * this.Density;
		this.item.rig.angularDamping = this.ScaledAngularDrag;
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x000326D8 File Offset: 0x000308D8
	private void ProcessLastCollision()
	{
		if (this._unprocessedContact == null)
		{
			return;
		}
		this._lastCollision = this._unprocessedContact.collider;
		Snowball.ContactType contactType = this.ClassifyCurrentContact();
		switch (contactType)
		{
		case Snowball.ContactType.Unknown:
		case Snowball.ContactType.RegularGround:
		case Snowball.ContactType.Snowball:
			break;
		case Snowball.ContactType.SnowyGround:
			if (this.CanGrow)
			{
				this.Scale(1f);
			}
			this._previousPosition = new Vector3?(base.transform.position);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this._timeLastCollided = Time.time;
		this._lastContactType = contactType;
		this._unprocessedContact = null;
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00032768 File Offset: 0x00030968
	private void Scale(float modifier = 1f)
	{
		if (this._previousPosition == null)
		{
			return;
		}
		Vector3 vector = base.transform.position - this._previousPosition.Value;
		vector = Vector3.ProjectOnPlane(vector, this._unprocessedContact.contacts[0].normal);
		float num = Mathf.Clamp(modifier * this.scaleRate * vector.magnitude, 0f, 0.1f);
		this.scaleSyncer.currentScale += num / this.scaleSyncer.currentScale;
		this.UpdateMass();
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00032804 File Offset: 0x00030A04
	private Snowball.ContactType ClassifyCurrentContact()
	{
		GameObject gameObject = this._unprocessedContact.gameObject;
		Renderer renderer;
		if ((gameObject.layer == this.terrainLayer || gameObject.CompareTag("Stone1")) && gameObject.TryGetComponent<Renderer>(ref renderer))
		{
			string text = renderer.sharedMaterial.name.ToLowerInvariant();
			if (!text.Contains("ice") && !text.Contains("snow"))
			{
				return Snowball.ContactType.RegularGround;
			}
			return Snowball.ContactType.SnowyGround;
		}
		else
		{
			if (!gameObject.CompareTag("Snowball"))
			{
				return Snowball.ContactType.Unknown;
			}
			return Snowball.ContactType.Snowball;
		}
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00032884 File Offset: 0x00030A84
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x040008CE RID: 2254
	private const float k_StartGracePeriod = 0.2f;

	// Token: 0x040008CF RID: 2255
	private const float k_HardBrakeFriction = 1000f;

	// Token: 0x040008D0 RID: 2256
	private const string k_SnowName = "snow";

	// Token: 0x040008D1 RID: 2257
	private const string k_IceName = "ice";

	// Token: 0x040008D2 RID: 2258
	private const string k_StoneTag = "Stone1";

	// Token: 0x040008D3 RID: 2259
	private const string k_SnowballTag = "Snowball";

	// Token: 0x040008D4 RID: 2260
	private bool _isInitialized;

	// Token: 0x040008D5 RID: 2261
	private PhysicsMaterial _defaultPhysicsMaterial;

	// Token: 0x040008D6 RID: 2262
	private float _originalScaleMagnitude;

	// Token: 0x040008D7 RID: 2263
	private Vector3? _previousPosition;

	// Token: 0x040008D8 RID: 2264
	private Vector3 _leftHandPosition;

	// Token: 0x040008D9 RID: 2265
	private Vector3 _rightHandPosition;

	// Token: 0x040008DA RID: 2266
	private float _startTime;

	// Token: 0x040008DB RID: 2267
	private float _timeLastCollided;

	// Token: 0x040008DC RID: 2268
	private float _timeBrakingStarted;

	// Token: 0x040008DD RID: 2269
	private float _timeLastTouchedScout;

	// Token: 0x040008DE RID: 2270
	public float scaleRate;

	// Token: 0x040008DF RID: 2271
	private ItemScaleSyncer scaleSyncer;

	// Token: 0x040008E0 RID: 2272
	[SerializeField]
	[Range(0f, 10f)]
	public float minimumSpeedToGrow = 0.1f;

	// Token: 0x040008E1 RID: 2273
	private const float k_MinTimeOnGroundBeforeGrow = 1f;

	// Token: 0x040008E2 RID: 2274
	private const string k_SettingsName = "Settings";

	// Token: 0x040008E3 RID: 2275
	[FormerlySerializedAs("_density")]
	[SerializeField]
	[Range(0.01f, 10f)]
	private float Density = 1f;

	// Token: 0x040008E4 RID: 2276
	[FormerlySerializedAs("timeBeforeHardBrake")]
	[SerializeField]
	[Range(0f, 10f)]
	private float timeBeforeFullStop = 2f;

	// Token: 0x040008E5 RID: 2277
	[SerializeField]
	private float maxScaleToPickUp = 3f;

	// Token: 0x040008E6 RID: 2278
	[SerializeField]
	private float maxCarriedScale = 3f;

	// Token: 0x040008E7 RID: 2279
	private float defaultAngularDrag;

	// Token: 0x040008E8 RID: 2280
	[FormerlySerializedAs("brakingDrag")]
	[SerializeField]
	private float BrakingAngularDrag = 100f;

	// Token: 0x040008E9 RID: 2281
	private const string k_ReferencesName = "References";

	// Token: 0x040008EA RID: 2282
	[SerializeField]
	private Transform LeftHand;

	// Token: 0x040008EB RID: 2283
	[SerializeField]
	private Transform RightHand;

	// Token: 0x040008EC RID: 2284
	[SerializeField]
	private SphereCollider physicalCollider;

	// Token: 0x040008ED RID: 2285
	[SerializeField]
	private PhysicsMaterial HardBrakeMaterial;

	// Token: 0x040008EE RID: 2286
	[SerializeField]
	private Collider flatCollider;

	// Token: 0x040008EF RID: 2287
	[SerializeField]
	private GameObject snowballParticles;

	// Token: 0x040008F0 RID: 2288
	private int terrainLayer;

	// Token: 0x040008F1 RID: 2289
	private int characterLayer;

	// Token: 0x040008F2 RID: 2290
	private Collision _unprocessedContact;

	// Token: 0x040008F3 RID: 2291
	private Snowball.ContactType _lastContactType;

	// Token: 0x040008F4 RID: 2292
	private Collider _lastCollision;

	// Token: 0x040008F5 RID: 2293
	private bool _couldGrowLastFrame;

	// Token: 0x02000455 RID: 1109
	private enum ContactType
	{
		// Token: 0x040018A1 RID: 6305
		Unknown,
		// Token: 0x040018A2 RID: 6306
		RegularGround,
		// Token: 0x040018A3 RID: 6307
		SnowyGround,
		// Token: 0x040018A4 RID: 6308
		Snowball
	}
}
