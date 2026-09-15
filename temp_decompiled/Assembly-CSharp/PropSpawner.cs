using System;
using System.Collections.Generic;
using System.Linq;
using Peak.ProcGen;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020002D5 RID: 725
public class PropSpawner : LevelGenStep, IValidatable
{
	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06001381 RID: 4993 RVA: 0x00062FAB File Offset: 0x000611AB
	public override DeferredStepTiming DeferredTiming
	{
		get
		{
			if (this._deferredSteps.Count <= 0)
			{
				return DeferredStepTiming.None;
			}
			return DeferredStepTiming.AfterCurrentGroupTiming;
		}
	}

	// Token: 0x06001382 RID: 4994 RVA: 0x00062FBE File Offset: 0x000611BE
	public override IDeferredStep ConstructDeferred(IMayHaveDeferredStep parent)
	{
		if (this != parent)
		{
			Debug.LogError("What da HECK!!!");
			return null;
		}
		return new ExecuteDeferredStepList(this._deferredSteps);
	}

	// Token: 0x06001383 RID: 4995 RVA: 0x00062FE0 File Offset: 0x000611E0
	public void Validate()
	{
		this.ValidationState = this.DoValidation();
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x00062FEE File Offset: 0x000611EE
	public Color GetValidationColor()
	{
		return this.GetValidationColorImpl();
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06001385 RID: 4997 RVA: 0x00062FF6 File Offset: 0x000611F6
	// (set) Token: 0x06001386 RID: 4998 RVA: 0x00062FFE File Offset: 0x000611FE
	public ValidationState ValidationState { get; private set; }

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06001387 RID: 4999 RVA: 0x00063007 File Offset: 0x00061207
	private Dictionary<GameObject, PropSpawner.SpawnData> AllSpawnData
	{
		get
		{
			return this._propSpawnData.Dict;
		}
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06001388 RID: 5000 RVA: 0x00063014 File Offset: 0x00061214
	public IEnumerable<GameObject> SpawnedProps
	{
		get
		{
			return this.AllSpawnData.Keys;
		}
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x00063024 File Offset: 0x00061224
	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.position + this.area.y * 0.5f * base.transform.up;
		Vector3 vector2 = base.transform.position - this.area.y * 0.5f * base.transform.up;
		Vector3 vector3 = base.transform.position - this.area.x * 0.5f * base.transform.right;
		Vector3 vector4 = base.transform.position + this.area.x * 0.5f * base.transform.right;
		Gizmos.DrawLine(vector2, vector);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(vector2, vector2 + base.transform.forward * this.rayLength + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector, vector + base.transform.forward * this.rayLength + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector3, vector3 + base.transform.forward * this.rayLength + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector4, vector4 + base.transform.forward * this.rayLength + base.transform.forward * this.rayNearCutoff);
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position + base.transform.forward * this.rayLength / 2f, base.transform.rotation, Vector3.one);
		Gizmos.DrawWireCube(Vector3.zero, ExtSwizzle.xyn(this.area, this.rayLength));
		Gizmos.matrix = matrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(vector2, vector2 + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector3, vector3 + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector, vector + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector4, vector4 + base.transform.forward * this.rayNearCutoff);
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x00063304 File Offset: 0x00061504
	public void Go()
	{
		this.Clear();
		this.SpawnNew(true);
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x00063313 File Offset: 0x00061513
	public override void Execute()
	{
		this.Clear();
		this.SpawnNew(false);
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x00063322 File Offset: 0x00061522
	public void Add()
	{
		this.SpawnNew(true);
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x0006332C File Offset: 0x0006152C
	private void OnValidate()
	{
		this.validationConstraints = Enumerable.ToList<IValidationConstraint>(Enumerable.Concat<IValidationConstraint>(Enumerable.Cast<IValidationConstraint>(Enumerable.Where<PropSpawnerConstraint>(this.constraints, (PropSpawnerConstraint c) => c is IValidationConstraint)), Enumerable.Cast<IValidationConstraint>(Enumerable.Where<PropSpawnerConstraintPost>(this.postConstraints, (PropSpawnerConstraintPost c) => c is IValidationConstraint))));
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x000633A8 File Offset: 0x000615A8
	public ValidationState DoValidation()
	{
		int num = 0;
		if (this.AllSpawnData.Count == 0 && base.transform.childCount != 0)
		{
			this._madeDummyData = true;
			Debug.Log("Attempted validation without cached prop data. Doing our best to create dummy data.");
			for (int i = 0; i < base.transform.childCount; i++)
			{
				GameObject gameObject = base.transform.GetChild(i).gameObject;
				Dictionary<GameObject, PropSpawner.SpawnData> allSpawnData = this.AllSpawnData;
				GameObject gameObject2 = gameObject;
				PropSpawner.SpawnData spawnData = new PropSpawner.SpawnData();
				spawnData.spawnerTransform = base.transform;
				spawnData.pos = gameObject.transform.position;
				spawnData.normal = gameObject.transform.up;
				spawnData.spawnCount = 1;
				RaycastHit raycastHit = default(RaycastHit);
				raycastHit.normal = gameObject.transform.up;
				raycastHit.point = gameObject.transform.position;
				spawnData.hit = raycastHit;
				allSpawnData.Add(gameObject2, spawnData);
			}
		}
		foreach (KeyValuePair<GameObject, PropSpawner.SpawnData> keyValuePair in this.AllSpawnData)
		{
			GameObject gameObject3;
			PropSpawner.SpawnData spawnData2;
			keyValuePair.Deconstruct(ref gameObject3, ref spawnData2);
			GameObject gameObject4 = gameObject3;
			PropSpawner.SpawnData spawnData3 = spawnData2;
			bool flag = true;
			foreach (IValidationConstraint validationConstraint in this.validationConstraints)
			{
				if (!validationConstraint.Muted && !validationConstraint.Validate(gameObject4, spawnData3))
				{
					Debug.LogWarning(string.Format("Failed validation constraint {0}: {1}/{2}", validationConstraint.GetType(), base.name, gameObject4.name), gameObject4);
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				num++;
			}
		}
		if (num > 0)
		{
			Debug.LogWarning(string.Format("{0} failed validation on {1} out of {2} props.", base.name, num, this.AllSpawnData.Count), this);
			return ValidationState.Failed;
		}
		if (this.AllSpawnData.Count > 0)
		{
			Debug.Log(string.Format("{0} passed validation on all {1} props!", base.name, this.AllSpawnData.Count), this);
			return ValidationState.Passed;
		}
		Debug.LogWarning(base.name + " didn't have any spawned props to validate", this);
		return ValidationState.Unknown;
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x000635F0 File Offset: 0x000617F0
	public void SpawnNew(bool executeDeferredImmediately = false)
	{
		if (this.chanceToUseSpawner < 0.999f && Random.value > this.chanceToUseSpawner)
		{
			return;
		}
		int num = this.nrOfSpawns;
		if (this.randomSpawns)
		{
			num = Random.Range(this.minSpawnCount, this.nrOfSpawns);
		}
		int num2 = 25000;
		int num3 = 5000;
		int num4 = 0;
		while (num4 < num && num2 > 0 && num3 > 0)
		{
			num2--;
			num3--;
			if (this.TryToSpawn(num4))
			{
				num4++;
				num3 = 5000;
				if (this.syncTransforms)
				{
					Physics.SyncTransforms();
				}
			}
		}
		if (num2 == 0)
		{
			Debug.LogWarning("Max attempts reached in PropSpawner, could not spawn all props!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", base.gameObject);
		}
		if (num3 == 0)
		{
			Debug.LogWarning("Max attempts IN A ROW reached in PropSpawner, could not spawn all props!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", base.gameObject);
		}
		this.currentSpawns = base.transform.childCount;
		this.SpawnDecor();
		foreach (PostSpawnBehavior postSpawnBehavior in this.postSpawnBehaviors)
		{
			if (!postSpawnBehavior.mute)
			{
				if (executeDeferredImmediately || postSpawnBehavior.DeferredTiming != DeferredStepTiming.AfterCurrentGroupTiming)
				{
					postSpawnBehavior.RunBehavior(this.SpawnedProps);
				}
				else
				{
					this._deferredSteps.Add(postSpawnBehavior.ConstructDeferred(this));
				}
			}
		}
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x00063734 File Offset: 0x00061934
	private void SpawnDecor()
	{
		DecorSpawner[] componentsInChildren = base.GetComponentsInChildren<DecorSpawner>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Execute();
		}
	}

	// Token: 0x06001391 RID: 5009 RVA: 0x00063760 File Offset: 0x00061960
	public override void Clear()
	{
		this.ValidationState = ValidationState.Unknown;
		this.AllSpawnData.Clear();
		this._madeDummyData = false;
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		this._deferredSteps.Clear();
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x000637BF File Offset: 0x000619BF
	public void GoAll()
	{
		base.GetComponentInParent<PropGrouper>().RunAll(true);
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x000637CD File Offset: 0x000619CD
	public void ClearAll()
	{
		base.GetComponentInParent<PropGrouper>().ClearAll();
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x000637DC File Offset: 0x000619DC
	private static bool AllConstraintsPass(IEnumerable<PropSpawnerConstraint> currentConstraints, PropSpawner.SpawnData spawnData)
	{
		foreach (PropSpawnerConstraint propSpawnerConstraint in currentConstraints)
		{
			if (!propSpawnerConstraint.mute && !propSpawnerConstraint.CheckConstraint(spawnData))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x00063838 File Offset: 0x00061A38
	private bool AllValidationConstraintsPass(IEnumerable<IValidationConstraint> currentConstraints, GameObject prop, PropSpawner.SpawnData spawnData)
	{
		foreach (IValidationConstraint validationConstraint in currentConstraints)
		{
			if (!validationConstraint.Muted && !validationConstraint.Validate(prop, spawnData))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001396 RID: 5014 RVA: 0x00063894 File Offset: 0x00061A94
	private bool TryToSpawn(int currentSpawnCount)
	{
		PropSpawner.SpawnData randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return false;
		}
		if (!PropSpawner.AllConstraintsPass(this.constraints, randomPoint))
		{
			return false;
		}
		randomPoint.spawnCount = currentSpawnCount;
		GameObject gameObject = this.Spawn(randomPoint);
		if (gameObject != null)
		{
			this.AllSpawnData[gameObject] = randomPoint;
		}
		return gameObject != null;
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x000638EC File Offset: 0x00061AEC
	private GameObject Spawn(PropSpawner.SpawnData spawnData)
	{
		GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], spawnData.pos, HelperFunctions.GetRandomRotationWithUp(Vector3.up), base.transform);
		for (int i = 0; i < this.modifiers.Count; i++)
		{
			if (!this.modifiers[i].mute)
			{
				this.modifiers[i].ModifyObject(gameObject, spawnData);
			}
		}
		for (int j = 0; j < this.postConstraints.Count; j++)
		{
			if (!this.postConstraints[j].mute && !this.postConstraints[j].CheckConstraint(gameObject, spawnData))
			{
				Object.DestroyImmediate(gameObject);
				return null;
			}
		}
		return gameObject;
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x000639B0 File Offset: 0x00061BB0
	private PropSpawner.SpawnData GetRandomPoint()
	{
		Vector3 vector = base.transform.position;
		Vector2 vector2;
		vector2..ctor(Random.value, Random.value);
		vector += base.transform.right * Mathf.Lerp(-this.area.x * 0.5f, this.area.x * 0.5f, vector2.x);
		vector += base.transform.up * Mathf.Lerp(-this.area.y * 0.5f, this.area.y * 0.5f, vector2.y);
		if (!this.raycastPosition)
		{
			return new PropSpawner.SpawnData
			{
				pos = vector,
				normal = -base.transform.forward,
				rayDir = base.transform.forward,
				hit = default(RaycastHit),
				spawnerTransform = base.transform,
				placement = vector2
			};
		}
		Vector3 vector3 = base.transform.forward * this.rayNearCutoff;
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + (base.transform.forward + vector3 + this.rayDirectionOffset).normalized * (this.rayLength - this.rayNearCutoff), this.layerType, 0f, 1);
		if (raycastHit.transform)
		{
			return new PropSpawner.SpawnData
			{
				pos = raycastHit.point,
				normal = raycastHit.normal,
				rayDir = base.transform.forward,
				hit = raycastHit,
				spawnerTransform = base.transform,
				placement = vector2
			};
		}
		return null;
	}

	// Token: 0x04001211 RID: 4625
	private List<IDeferredStep> _deferredSteps = new List<IDeferredStep>();

	// Token: 0x04001212 RID: 4626
	public Vector2 area;

	// Token: 0x04001213 RID: 4627
	public Vector3 rayDirectionOffset;

	// Token: 0x04001214 RID: 4628
	public float rayLength = 5000f;

	// Token: 0x04001215 RID: 4629
	public float rayNearCutoff;

	// Token: 0x04001216 RID: 4630
	public bool raycastPosition = true;

	// Token: 0x04001217 RID: 4631
	public int nrOfSpawns = 500;

	// Token: 0x04001218 RID: 4632
	public bool randomSpawns;

	// Token: 0x04001219 RID: 4633
	public int minSpawnCount;

	// Token: 0x0400121A RID: 4634
	[Range(0f, 1f)]
	public float chanceToUseSpawner = 1f;

	// Token: 0x0400121B RID: 4635
	public int currentSpawns;

	// Token: 0x0400121C RID: 4636
	public GameObject[] props;

	// Token: 0x0400121D RID: 4637
	public bool syncTransforms = true;

	// Token: 0x0400121E RID: 4638
	public HelperFunctions.LayerType layerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x0400121F RID: 4639
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x04001220 RID: 4640
	[SerializeReference]
	public List<PropSpawnerConstraint> constraints = new List<PropSpawnerConstraint>();

	// Token: 0x04001221 RID: 4641
	[SerializeReference]
	public List<PropSpawnerConstraintPost> postConstraints = new List<PropSpawnerConstraintPost>();

	// Token: 0x04001222 RID: 4642
	[SerializeReference]
	public List<PostSpawnBehavior> postSpawnBehaviors = new List<PostSpawnBehavior>();

	// Token: 0x04001223 RID: 4643
	public const string k_ValidationGroupColor = "#91DDF2";

	// Token: 0x04001224 RID: 4644
	[SerializeReference]
	[Tooltip("Constraints that do NOT apply when spawning props, but may be useful for debugging in Editor.")]
	private List<IValidationConstraint> validationConstraints = new List<IValidationConstraint>();

	// Token: 0x04001226 RID: 4646
	[SerializeField]
	[HideInInspector]
	private bool _madeDummyData;

	// Token: 0x04001227 RID: 4647
	[SerializeReference]
	private PropSpawnData _propSpawnData = new PropSpawnData();

	// Token: 0x020004FB RID: 1275
	[Serializable]
	public class SpawnData
	{
		// Token: 0x04001B30 RID: 6960
		public Transform spawnerTransform;

		// Token: 0x04001B31 RID: 6961
		public Vector3 pos;

		// Token: 0x04001B32 RID: 6962
		public Vector3 normal;

		// Token: 0x04001B33 RID: 6963
		public Vector3 rayDir;

		// Token: 0x04001B34 RID: 6964
		public RaycastHit hit;

		// Token: 0x04001B35 RID: 6965
		public Vector2 placement;

		// Token: 0x04001B36 RID: 6966
		public int spawnCount;
	}
}
