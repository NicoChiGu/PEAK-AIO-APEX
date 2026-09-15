using System;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000308 RID: 776
public class PutMeInWall : MonoBehaviour
{
	// Token: 0x06001421 RID: 5153 RVA: 0x00065CC7 File Offset: 0x00063EC7
	private void Go()
	{
		this.PutInTheWall();
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x00065CD0 File Offset: 0x00063ED0
	public bool PutInTheWall()
	{
		Vector3 vector = base.transform.position - Vector3.forward * 50f;
		RaycastHit[] array = Physics.RaycastAll(vector, Vector3.forward, 500f);
		Debug.DrawLine(vector, vector + Vector3.forward * 100f, Color.red, 10f);
		Debug.Log(string.Format("hits: {0}", array.Length));
		Debug.Log(string.Format("list{0}", array));
		array = Enumerable.ToArray<RaycastHit>(Enumerable.OrderBy<RaycastHit, float>(array, (RaycastHit h) => h.distance));
		RaycastHit raycastHit = Enumerable.First<RaycastHit>(array, (RaycastHit h) => h.collider.gameObject != this.gameObject);
		Vector3 vector2 = raycastHit.point + Vector3.forward * ExtDataTypes.PRndRange(this.penetrationRnage);
		Collider component = base.GetComponent<Collider>();
		if (this.angle > 0f && Vector2.Angle(raycastHit.normal, Vector2.up) <= this.angle)
		{
			return false;
		}
		if (this.checkBelow)
		{
			RaycastHit[] array2 = Physics.SphereCastAll(vector2, component.bounds.extents.magnitude, Vector3.down, component.bounds.extents.magnitude * this.belowMargin);
			Debug.Log(string.Format("belowHits: {0}", array2.Length));
			array2 = Enumerable.ToArray<RaycastHit>(Enumerable.Where<RaycastHit>(array2, (RaycastHit hit) => hit.collider.gameObject != this.gameObject && hit.collider.gameObject != raycastHit.collider.gameObject));
			Debug.Log(string.Format("belowHits2: {0}", array2.Length));
			if (array2.Length != 0)
			{
				foreach (RaycastHit raycastHit2 in array2)
				{
					Debug.Log(string.Format("hit: {0}", raycastHit2.collider.gameObject));
				}
				Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.red, 10f);
				return false;
			}
			Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.green, 10f);
		}
		Debug.Log(raycastHit.collider.gameObject, raycastHit.collider.gameObject);
		base.transform.position = vector2;
		return true;
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x00065FC8 File Offset: 0x000641C8
	public Vector3? GetWallPosition2(Vector3 startCast, float maxDistance = 100f)
	{
		Vector3 vector = startCast - Vector3.forward * 50f;
		maxDistance += 50f;
		RaycastHit[] array = Physics.RaycastAll(vector, Vector3.forward, maxDistance);
		Debug.DrawLine(vector, vector + Vector3.forward * maxDistance, Color.red, 10f);
		Debug.Log(string.Format("hits: {0}", array.Length));
		Debug.Log(string.Format("list{0}", array));
		array = Enumerable.ToArray<RaycastHit>(Enumerable.OrderBy<RaycastHit, float>(array, (RaycastHit h) => h.distance));
		RaycastHit raycastHit = Enumerable.First<RaycastHit>(array, (RaycastHit h) => h.collider.gameObject != this.gameObject);
		Vector3 vector2 = raycastHit.point + Vector3.forward * ExtDataTypes.PRndRange(this.penetrationRnage);
		Collider component = base.GetComponent<Collider>();
		if (this.angle > 0f && Vector2.Angle(raycastHit.normal, Vector2.up) <= this.angle)
		{
			return default(Vector3?);
		}
		if (this.checkBelow)
		{
			if (Enumerable.ToArray<RaycastHit>(Enumerable.Where<RaycastHit>(Physics.SphereCastAll(vector2, component.bounds.extents.magnitude, Vector3.down, component.bounds.extents.magnitude * this.belowMargin), (RaycastHit hit) => hit.collider.gameObject != this.gameObject && hit.collider.gameObject != raycastHit.collider.gameObject)).Length != 0)
			{
				Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.red, 10f);
				return default(Vector3?);
			}
			Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.green, 10f);
		}
		Debug.Log(raycastHit.collider.gameObject, raycastHit.collider.gameObject);
		return new Vector3?(vector2);
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0006624C File Offset: 0x0006444C
	public Vector3? GetWallPosition(Vector3 startCast, float maxDistance = 100f)
	{
		Vector3 vector = startCast - Vector3.forward * 50f;
		maxDistance += 50f;
		RaycastHit[] array = Physics.RaycastAll(vector, Vector3.forward, maxDistance, HelperFunctions.GetMask(HelperFunctions.LayerType.Terrain));
		if (this.angle > 0f)
		{
			array = Enumerable.ToArray<RaycastHit>(Enumerable.Where<RaycastHit>(array, (RaycastHit h) => Vector2.Angle(h.normal, Vector2.up) > this.angle));
		}
		array = Enumerable.ToArray<RaycastHit>(Enumerable.OrderBy<RaycastHit, float>(array, (RaycastHit h) => h.distance));
		Debug.DrawLine(vector, vector + Vector3.up * maxDistance, Color.green, 10f);
		Debug.DrawLine(vector, vector + Vector3.forward * maxDistance, Color.red, 10f);
		Debug.Log(string.Format("hits: {0}", array.Length));
		Debug.Log(string.Format("list{0}", array));
		RaycastHit[] array2 = array;
		int num = 0;
		if (num >= array2.Length)
		{
			return default(Vector3?);
		}
		RaycastHit raycastHit = array2[num];
		return new Vector3?(raycastHit.point + Vector3.forward * ExtDataTypes.PRndRange(this.penetrationRnage));
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0006638C File Offset: 0x0006458C
	public void RandomRotation()
	{
		base.transform.rotation = Quaternion.Euler((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360));
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x000663C2 File Offset: 0x000645C2
	public void RandomScale()
	{
		base.transform.localScale *= ExtDataTypes.PRndRange(this.scaleRange);
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x000663E5 File Offset: 0x000645E5
	private void Start()
	{
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x000663E7 File Offset: 0x000645E7
	private void Update()
	{
	}

	// Token: 0x040012A9 RID: 4777
	public Vector2 penetrationRnage;

	// Token: 0x040012AA RID: 4778
	public Vector2 scaleRange = new Vector2(1f, 1f);

	// Token: 0x040012AB RID: 4779
	public bool checkBelow;

	// Token: 0x040012AC RID: 4780
	public float belowMargin = 1f;

	// Token: 0x040012AD RID: 4781
	public float angle = -1f;
}
