using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class HelperFunctions : MonoBehaviour
{
	// Token: 0x06000776 RID: 1910 RVA: 0x00029DCC File Offset: 0x00027FCC
	internal static Terrain GetTerrain(Vector3 center)
	{
		RaycastHit raycastHit = HelperFunctions.LineCheck(center + Vector3.up * 1000f, center - Vector3.up * 1000f, HelperFunctions.LayerType.Terrain, 0f, 1);
		if (raycastHit.transform)
		{
			return raycastHit.transform.GetComponent<Terrain>();
		}
		return null;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x00029E2C File Offset: 0x0002802C
	public static LayerMask GetMask(HelperFunctions.LayerType layerType)
	{
		if (layerType == HelperFunctions.LayerType.AllPhysical)
		{
			return HelperFunctions.AllPhysical;
		}
		if (layerType == HelperFunctions.LayerType.TerrainMap)
		{
			return HelperFunctions.terrainMapMask;
		}
		if (layerType == HelperFunctions.LayerType.Terrain)
		{
			return HelperFunctions.terrainMask;
		}
		if (layerType == HelperFunctions.LayerType.Default)
		{
			return HelperFunctions.DefaultMask;
		}
		if (layerType == HelperFunctions.LayerType.AllPhysicalExceptCharacter)
		{
			return HelperFunctions.AllPhysicalExceptCharacter;
		}
		if (layerType == HelperFunctions.LayerType.Map)
		{
			return HelperFunctions.MapMask;
		}
		if (layerType == HelperFunctions.LayerType.CharacterAndDefault)
		{
			return HelperFunctions.CharacterAndDefaultMask;
		}
		if (layerType == HelperFunctions.LayerType.AllPhysicalExceptDefault)
		{
			return HelperFunctions.AllPhysicalExceptDefault;
		}
		return HelperFunctions.MapMask;
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00029E90 File Offset: 0x00028090
	public static Vector3 GetGroundPos(Vector3 from, HelperFunctions.LayerType layerType, float radius = 0f)
	{
		Vector3 vector = from;
		RaycastHit raycastHit = HelperFunctions.LineCheck(from, from + Vector3.down * 10000f, layerType, radius, 1);
		if (raycastHit.transform)
		{
			vector = raycastHit.point;
		}
		return vector;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00029ED5 File Offset: 0x000280D5
	public static RaycastHit GetGroundPosRaycast(Vector3 from, HelperFunctions.LayerType layerType, float radius = 0f)
	{
		return HelperFunctions.LineCheck(from, from + Vector3.down * 10000f, layerType, radius, 1);
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00029EF5 File Offset: 0x000280F5
	internal static GameObject InstantiatePrefab(GameObject sourceObj, Vector3 pos, Quaternion rot, Transform parent)
	{
		GameObject gameObject = HelperFunctions.InstantiatePrefab(sourceObj, parent);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rot;
		return gameObject;
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00029F18 File Offset: 0x00028118
	internal static GameObject InstantiatePrefab(GameObject sourceObj, Transform parent)
	{
		GameObject gameObject = null;
		if (!Application.isEditor)
		{
			gameObject = Object.Instantiate<GameObject>(sourceObj, parent);
		}
		return gameObject;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00029F37 File Offset: 0x00028137
	public static RaycastHit GetGroundPosRaycast(Vector3 from, HelperFunctions.LayerType layerType, Vector3 gravityDir, float radius = 0f)
	{
		return HelperFunctions.LineCheck(from, from + gravityDir * 10000f, layerType, radius, 1);
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00029F54 File Offset: 0x00028154
	public static RaycastHit LineCheck(Vector3 from, Vector3 to, HelperFunctions.LayerType layerType, float radius = 0f, QueryTriggerInteraction triggerInteraction = 1)
	{
		RaycastHit raycastHit = default(RaycastHit);
		Ray ray;
		ray..ctor(from, to - from);
		if (radius == 0f)
		{
			Physics.Raycast(ray, ref raycastHit, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType));
		}
		else
		{
			Physics.SphereCast(ray, radius, ref raycastHit, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType));
		}
		return raycastHit;
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00029FBC File Offset: 0x000281BC
	public static RaycastHit[] LineCheckAll(Vector3 from, Vector3 to, HelperFunctions.LayerType layerType, float radius = 0f, QueryTriggerInteraction triggerInteraction = 1)
	{
		RaycastHit[] array;
		if (radius == 0f)
		{
			array = Physics.RaycastAll(from, to - from, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType), triggerInteraction);
		}
		else
		{
			array = Physics.SphereCastAll(from, radius, to - from, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType), triggerInteraction);
		}
		return array;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x0002A01C File Offset: 0x0002821C
	public static RaycastHit LineCheckIgnoreItem(Vector3 from, Vector3 to, HelperFunctions.LayerType layerType, Item ignoreItem)
	{
		RaycastHit raycastHit = default(RaycastHit);
		foreach (RaycastHit raycastHit2 in Physics.RaycastAll(from, to - from, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType)))
		{
			Item componentInParent = raycastHit2.collider.GetComponentInParent<Item>();
			if ((!componentInParent || !(componentInParent == ignoreItem)) && (raycastHit.collider == null || raycastHit.distance > raycastHit2.distance))
			{
				raycastHit = raycastHit2;
			}
		}
		return raycastHit;
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x0002A0AC File Offset: 0x000282AC
	internal static ConfigurableJoint AttachPositionJoint(Rigidbody rig1, Rigidbody rig2, bool useCustomConnection = false, Vector3 customConnectionPoint = default(Vector3))
	{
		ConfigurableJoint configurableJoint = rig1.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.xMotion = 0;
		configurableJoint.yMotion = 0;
		configurableJoint.zMotion = 0;
		configurableJoint.projectionMode = 1;
		configurableJoint.anchor = ((!useCustomConnection) ? rig1.transform.InverseTransformPoint(rig2.position) : rig1.transform.InverseTransformPoint(customConnectionPoint));
		configurableJoint.enableCollision = false;
		configurableJoint.connectedBody = rig2;
		return configurableJoint;
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x0002A116 File Offset: 0x00028316
	internal static Joint AttachFixedJoint(Rigidbody rig1, Rigidbody rig2)
	{
		FixedJoint fixedJoint = rig1.gameObject.AddComponent<FixedJoint>();
		fixedJoint.enableCollision = false;
		fixedJoint.connectedBody = rig2;
		return fixedJoint;
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x0002A134 File Offset: 0x00028334
	internal static Vector3 RandomOnFlatCircle()
	{
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		return new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x0002A160 File Offset: 0x00028360
	internal static void DestroyAll(Object[] objects)
	{
		for (int i = objects.Length - 1; i >= 0; i--)
		{
			Object.Destroy(objects[i]);
		}
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x0002A185 File Offset: 0x00028385
	internal static Vector3 EulerToLook(Vector2 euler)
	{
		return new Vector3(euler.y, -euler.x, 0f);
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x0002A19E File Offset: 0x0002839E
	internal static Vector3 LookToEuler(Vector2 lookRotationValues)
	{
		return new Vector3(-lookRotationValues.y, lookRotationValues.x, 0f);
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x0002A1B7 File Offset: 0x000283B7
	internal static Vector3 LookToDirection(Vector3 look, Vector3 targetDir)
	{
		return HelperFunctions.EulerToDirection(HelperFunctions.LookToEuler(look), targetDir);
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x0002A1CA File Offset: 0x000283CA
	internal static Vector3 EulerToDirection(Vector3 euler, Vector3 targetDir)
	{
		return Quaternion.Euler(euler) * targetDir;
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x0002A1D8 File Offset: 0x000283D8
	internal static Vector3 DirectionToEuler(Vector3 dir)
	{
		return Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x0002A1F8 File Offset: 0x000283F8
	internal static Vector3 DirectionToLook(Vector3 dir)
	{
		Vector3 vector = HelperFunctions.DirectionToEuler(dir);
		while (vector.x > 180f)
		{
			vector.x -= 360f;
		}
		return HelperFunctions.EulerToLook(vector);
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x0002A236 File Offset: 0x00028436
	internal static Vector3 GroundDirection(Vector3 planeNormal, Vector3 sideDirection)
	{
		return -Vector3.Cross(sideDirection, planeNormal);
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x0002A244 File Offset: 0x00028444
	internal static Vector3 SeparateClamps(Vector3 rotationError, float clamp)
	{
		rotationError.x = Mathf.Clamp(rotationError.x, -clamp, clamp);
		rotationError.y = Mathf.Clamp(rotationError.y, -clamp, clamp);
		rotationError.z = Mathf.Clamp(rotationError.z, -clamp, clamp);
		return rotationError;
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x0002A291 File Offset: 0x00028491
	internal static float FlatDistance(Vector3 from, Vector3 to)
	{
		return Vector2.Distance(from.XZ(), to.XZ());
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x0002A2A4 File Offset: 0x000284A4
	internal static void IgnoreConnect(Rigidbody rig1, Rigidbody rig2)
	{
		rig1.gameObject.AddComponent<ConfigurableJoint>().connectedBody = rig2;
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x0002A2B7 File Offset: 0x000284B7
	internal static RaycastHit[] SortRaycastResults(RaycastHit[] hitsToSort)
	{
		LinqExtensions.Sort<RaycastHit>(hitsToSort, new Comparison<RaycastHit>(HelperFunctions.RaycastHitComparer));
		return hitsToSort;
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0002A2CC File Offset: 0x000284CC
	public static Vector3[] GetCircularDirections(int count)
	{
		Vector3[] array = new Vector3[count];
		float num = 360f / (float)count;
		for (int i = 0; i < count; i++)
		{
			float num2 = (float)i * num;
			float num3 = 0.017453292f * num2;
			array[i] = new Vector3(Mathf.Cos(num3), 0f, Mathf.Sin(num3)).normalized;
		}
		return array;
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x0002A32C File Offset: 0x0002852C
	private static int RaycastHitComparer(RaycastHit x, RaycastHit y)
	{
		if (x.distance < y.distance)
		{
			return -1;
		}
		return 1;
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x0002A344 File Offset: 0x00028544
	internal static Quaternion GetRandomRotationWithUp(Vector3 normal)
	{
		Vector3 vector = Random.onUnitSphere;
		vector.y = 0f;
		vector = Vector3.Cross(normal, Vector3.Cross(normal, vector));
		return Quaternion.LookRotation(vector, normal);
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x0002A378 File Offset: 0x00028578
	public static Bounds GetTotalBounds(GameObject gameObject)
	{
		return HelperFunctions.GetTotalBounds(gameObject.GetComponentsInChildren<MeshRenderer>());
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x0002A388 File Offset: 0x00028588
	internal static Vector3 GetCenterOfMass(Transform transform)
	{
		Vector3 vector = Vector3.zero;
		float num = 0f;
		for (int i = 0; i < transform.childCount; i++)
		{
			Collider component = transform.GetChild(i).GetComponent<Collider>();
			if (component)
			{
				vector += component.transform.position;
				num += 1f;
			}
		}
		vector /= num;
		return transform.InverseTransformPoint(vector);
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x0002A3F0 File Offset: 0x000285F0
	public static Bounds GetTotalBounds(IEnumerable<Renderer> rends)
	{
		Bounds bounds = default(Bounds);
		bool flag = true;
		foreach (Renderer renderer in rends)
		{
			if (flag)
			{
				bounds = renderer.bounds;
				flag = false;
			}
			else
			{
				bounds.Encapsulate(renderer.bounds);
			}
		}
		return bounds;
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0002A458 File Offset: 0x00028658
	public static List<Tout> GetComponentListFromComponentArray<Tin, Tout>(IEnumerable<Tin> inComponents) where Tin : Component where Tout : Component
	{
		List<Tout> list = new List<Tout>();
		foreach (Tin tin in inComponents)
		{
			Tout component = tin.GetComponent<Tout>();
			if (component)
			{
				list.Add(component);
			}
		}
		return list;
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0002A4C0 File Offset: 0x000286C0
	internal static IEnumerable<T> SortBySiblingIndex<T>(IEnumerable<T> componentsToSort) where T : Component
	{
		List<T> list = new List<T>();
		list.AddRange(componentsToSort);
		list.Sort((T p1, T p2) => p1.transform.GetSiblingIndex().CompareTo(p2.transform.GetSiblingIndex()));
		return list;
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0002A4F3 File Offset: 0x000286F3
	internal static float FlatAngle(Vector3 dir1, Vector3 dir2)
	{
		return Vector3.Angle(dir1.Flat(), dir2.Flat());
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x0002A508 File Offset: 0x00028708
	internal static void SetChildCollidersLayer(Transform root, int layerID)
	{
		Collider[] componentsInChildren = root.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layerID;
		}
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0002A538 File Offset: 0x00028738
	internal static void SetJointDrive(ConfigurableJoint joint, float spring, float damper, Rigidbody rig)
	{
		JointDrive angularXDrive = joint.angularXDrive;
		angularXDrive.positionSpring = spring * rig.mass;
		angularXDrive.positionDamper = damper * rig.mass;
		joint.angularXDrive = angularXDrive;
		joint.angularYZDrive = angularXDrive;
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0002A578 File Offset: 0x00028778
	internal static Transform FindChildRecursive(string targetName, Transform root)
	{
		if (root.gameObject.name.ToUpper() == targetName.ToUpper())
		{
			return root;
		}
		for (int i = 0; i < root.childCount; i++)
		{
			Transform transform = HelperFunctions.FindChildRecursive(targetName, root.GetChild(i));
			if (!(transform == null) && transform.gameObject.name.ToUpper() == targetName.ToUpper())
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0002A5EC File Offset: 0x000287EC
	internal static void PhysicsRotateTowards(Rigidbody rig, Vector3 from, Vector3 to, float force)
	{
		Vector3 vector = Vector3.Cross(from, to).normalized * Vector3.Angle(from, to);
		rig.AddTorque(vector * force, 5);
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x0002A623 File Offset: 0x00028823
	internal static Vector3 MultiplyVectors(Vector3 v1, Vector3 v2)
	{
		v1.x *= v2.x;
		v1.y *= v2.y;
		v1.z *= v2.z;
		return v1;
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x0002A659 File Offset: 0x00028859
	public static Vector3 CubicBezier(Vector3 Start, Vector3 _P1, Vector3 _P2, Vector3 end, float _t)
	{
		return (1f - _t) * HelperFunctions.QuadraticBezier(Start, _P1, _P2, _t) + _t * HelperFunctions.QuadraticBezier(_P1, _P2, end, _t);
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x0002A688 File Offset: 0x00028888
	public static Vector3 QuadraticBezier(Vector3 start, Vector3 _P1, Vector3 end, float _t)
	{
		return (1f - _t) * HelperFunctions.LinearBezier(start, _P1, _t) + _t * HelperFunctions.LinearBezier(_P1, end, _t);
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x0002A6B1 File Offset: 0x000288B1
	public static Vector3 LinearBezier(Vector3 start, Vector3 end, float _t)
	{
		return (1f - _t) * start + _t * end;
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0002A6CC File Offset: 0x000288CC
	internal static Vector3 GetRandomPositionInBounds(Bounds bounds)
	{
		return new Vector3(Mathf.Lerp(bounds.min.x, bounds.max.x, Random.value), Mathf.Lerp(bounds.min.y, bounds.max.y, Random.value), Mathf.Lerp(bounds.min.z, bounds.max.z, Random.value));
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x0002A744 File Offset: 0x00028944
	internal static GameObject SpawnPrefab(GameObject gameObject, Vector3 position, Quaternion rotation, Transform transform)
	{
		GameObject gameObject2 = null;
		if (!Application.isEditor)
		{
			gameObject2 = Object.Instantiate<GameObject>(gameObject);
		}
		if (gameObject2 == null)
		{
			Debug.LogError("Failed to spawn prefab: " + gameObject.name, gameObject);
			return null;
		}
		gameObject2.transform.SetParent(transform);
		gameObject2.transform.rotation = rotation;
		gameObject2.transform.position = position;
		return gameObject2;
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0002A7A7 File Offset: 0x000289A7
	internal static Quaternion GetRotationWithUp(Vector3 forward, Vector3 up)
	{
		return Quaternion.LookRotation(Vector3.ProjectOnPlane(forward, up), up);
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0002A7B8 File Offset: 0x000289B8
	internal static float BoxDistance(Vector3 pos1, Vector3 pos2)
	{
		return Mathf.Max(Mathf.Max(Mathf.Max(0f, Mathf.Abs(pos1.x - pos2.x)), Mathf.Abs(pos1.y - pos2.y)), Mathf.Abs(pos1.z - pos2.z));
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0002A810 File Offset: 0x00028A10
	internal static bool CanSee(Transform looker, Vector3 pos, float maxAngle = 70f)
	{
		return Vector3.Angle(looker.forward, pos - looker.position) <= maxAngle && !HelperFunctions.LineCheck(looker.transform.position, pos, HelperFunctions.LayerType.TerrainMap, 0f, 1).transform;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0002A864 File Offset: 0x00028A64
	internal static bool InBoxRange(Vector3 position1, Vector3 position2, int range)
	{
		return Mathf.Abs(position1.x - position2.x) <= (float)range && Mathf.Abs(position1.y - position2.y) <= (float)range && Mathf.Abs(position1.z - position2.z) <= (float)range;
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0002A8BC File Offset: 0x00028ABC
	internal static Random.State SetRandomSeedFromWorldPos(Vector3 position, int seed)
	{
		position.x = (float)Mathf.RoundToInt(position.x);
		position.y = (float)Mathf.RoundToInt(position.y);
		position.z = (float)Mathf.RoundToInt(position.z);
		Random.State state = Random.state;
		Debug.Log("Set Seed");
		Random.InitState(Mathf.RoundToInt((float)seed + position.x + position.y * 100f + position.z * 10000f));
		return state;
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002A940 File Offset: 0x00028B40
	public static List<Transform> FindAllChildrenWithTag(string targetTag, Transform target)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < target.childCount; i++)
		{
			Transform child = target.GetChild(i);
			if (child.name.Contains(targetTag))
			{
				list.Add(child);
			}
			list.AddRange(HelperFunctions.FindAllChildrenWithTag(targetTag, child));
		}
		return list;
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0002A990 File Offset: 0x00028B90
	internal static T[] GridToFlatArray<T>(T[,] grid)
	{
		T[] array = new T[grid.GetLength(0) * grid.GetLength(1)];
		int length = grid.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				int num = i * length + j;
				array[num] = grid[j, i];
			}
		}
		return array;
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0002A9EC File Offset: 0x00028BEC
	internal static NativeArray<float> FloatGridToNativeArray(float[,] floats)
	{
		NativeArray<float> nativeArray = new NativeArray<float>(floats.GetLength(0) * floats.GetLength(1), 3, 1);
		int length = floats.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				int num = i * length + j;
				nativeArray[num] = floats[i, j];
			}
		}
		return nativeArray;
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x0002AA4C File Offset: 0x00028C4C
	internal static float[,] NativeArrayToFloatGrid(NativeArray<float> array, int arrayLength)
	{
		float[,] array2 = new float[arrayLength, arrayLength];
		int length = array.Length;
		for (int i = 0; i < length; i++)
		{
			int num = Mathf.FloorToInt((float)(i / arrayLength));
			int num2 = i - num * arrayLength;
			array2[num, num2] = array[i];
		}
		return array2;
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0002AA98 File Offset: 0x00028C98
	public static Vector2Int GetIndex_FlatToGrid(int flatIndex, int arrayLength)
	{
		int num = Mathf.FloorToInt((float)(flatIndex / arrayLength));
		int num2 = flatIndex - num * arrayLength;
		return new Vector2Int(num, num2);
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x0002AABC File Offset: 0x00028CBC
	public static int GetIndex_GridToFlat(Vector2Int gridIndex, int arrayLength)
	{
		return gridIndex.x * arrayLength + gridIndex.y;
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0002AAD0 File Offset: 0x00028CD0
	internal static List<Vector2Int> GetIndexesInBounds(int xRess, int yRess, Bounds selectionBounds, Bounds totalBounds)
	{
		int num = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.x, totalBounds.max.x, selectionBounds.min.x) * (float)xRess);
		int num2 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.x, totalBounds.max.x, selectionBounds.max.x) * (float)xRess);
		int num3 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.z, totalBounds.max.z, selectionBounds.min.z) * (float)xRess);
		int num4 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.z, totalBounds.max.z, selectionBounds.max.z) * (float)yRess);
		List<Vector2Int> list = new List<Vector2Int>();
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				list.Add(new Vector2Int(i, j));
				HelperFunctions.IDToWorldPos(i, j, xRess, yRess, totalBounds);
			}
		}
		return list;
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0002ABE8 File Offset: 0x00028DE8
	public static Vector3 IDToWorldPos(int x, int y, int xRess, int yRess, Bounds totalBounds)
	{
		float num = (float)x / ((float)xRess - 1f);
		float num2 = (float)y / ((float)yRess - 1f);
		return new Vector3(Mathf.Lerp(totalBounds.min.x, totalBounds.max.x, num), 0f, Mathf.Lerp(totalBounds.min.z, totalBounds.max.z, num2));
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0002AC54 File Offset: 0x00028E54
	internal static Vector3 GetRadomPointInBounds(Bounds b)
	{
		Vector3 min = b.min;
		Vector3 max = b.max;
		return new Vector3(Mathf.Lerp(min.x, max.x, Random.value), Mathf.Lerp(min.y, max.y, Random.value), Mathf.Lerp(min.z, max.z, Random.value));
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0002ACB8 File Offset: 0x00028EB8
	internal static Camera GetMainCamera()
	{
		if (MainCamera.instance == null)
		{
			MainCamera.instance = Object.FindAnyObjectByType<MainCamera>();
			MainCamera.instance.cam = MainCamera.instance.GetComponent<Camera>();
		}
		return MainCamera.instance.cam;
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0002ACF0 File Offset: 0x00028EF0
	internal static Color GetVertexColorAtPoint(Vector3[] verts, Color[] colors, Transform transform, Vector3 point)
	{
		if (colors.Length == 0)
		{
			return Color.black;
		}
		Color color = Color.black;
		float num = 10000000f;
		for (int i = 0; i < verts.Length; i++)
		{
			Vector3 vector = transform.TransformPoint(verts[i]);
			float num2 = Vector3.Distance(point, vector);
			if (num2 < num)
			{
				num = num2;
				color = colors[i];
			}
		}
		return color;
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0002AD49 File Offset: 0x00028F49
	internal static float GetValue(Color color)
	{
		return Mathf.Max(new float[] { color.r, color.g, color.b });
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x0002AD74 File Offset: 0x00028F74
	public static T RandomSelection<T>(List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0002ADA8 File Offset: 0x00028FA8
	public static bool IsLayerInLayerMask(LayerMask layerMask, int layer)
	{
		return (layerMask.value & (1 << layer)) != 0;
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0002ADBB File Offset: 0x00028FBB
	public static bool IsLayerInLayerMask(HelperFunctions.LayerType layerType, int layer)
	{
		return HelperFunctions.IsLayerInLayerMask(HelperFunctions.GetMask(layerType), layer);
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x0002ADC9 File Offset: 0x00028FC9
	public static Vector3 ZeroY(Vector3 original)
	{
		return new Vector3(original.x, 0f, original.z);
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x0002ADE4 File Offset: 0x00028FE4
	internal static bool AnyPlayerInZRange(float min, float max)
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (!character.isBot && character.Center.z >= min && character.Center.z <= max)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000788 RID: 1928
	public static LayerMask AllPhysical = LayerMask.GetMask(new string[] { "Terrain", "Map", "Default", "Character", "Rope" });

	// Token: 0x04000789 RID: 1929
	public static LayerMask AllPhysicalExceptCharacter = LayerMask.GetMask(new string[] { "Terrain", "Map", "Default", "Rope" });

	// Token: 0x0400078A RID: 1930
	public static LayerMask AllPhysicalExceptDefault = LayerMask.GetMask(new string[] { "Terrain", "Map", "Character", "Rope" });

	// Token: 0x0400078B RID: 1931
	public static LayerMask terrainMapMask = LayerMask.GetMask(new string[] { "Terrain", "Map" });

	// Token: 0x0400078C RID: 1932
	public static LayerMask terrainMask = LayerMask.GetMask(new string[] { "Terrain" });

	// Token: 0x0400078D RID: 1933
	public static LayerMask MapMask = LayerMask.GetMask(new string[] { "Map" });

	// Token: 0x0400078E RID: 1934
	public static LayerMask DefaultMask = LayerMask.GetMask(new string[] { "Default" });

	// Token: 0x0400078F RID: 1935
	public static LayerMask CharacterAndDefaultMask = LayerMask.GetMask(new string[] { "Character", "Default" });

	// Token: 0x02000445 RID: 1093
	public enum LayerType
	{
		// Token: 0x04001844 RID: 6212
		AllPhysical,
		// Token: 0x04001845 RID: 6213
		TerrainMap,
		// Token: 0x04001846 RID: 6214
		Terrain,
		// Token: 0x04001847 RID: 6215
		Map,
		// Token: 0x04001848 RID: 6216
		Default,
		// Token: 0x04001849 RID: 6217
		AllPhysicalExceptCharacter,
		// Token: 0x0400184A RID: 6218
		CharacterAndDefault,
		// Token: 0x0400184B RID: 6219
		AllPhysicalExceptDefault
	}
}
