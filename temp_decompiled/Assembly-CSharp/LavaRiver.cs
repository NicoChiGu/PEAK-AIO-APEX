using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x02000286 RID: 646
public class LavaRiver : CustomSpawnCondition
{
	// Token: 0x0600120C RID: 4620 RVA: 0x0005B6B7 File Offset: 0x000598B7
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		this.Spawn();
		return true;
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x0005B6C0 File Offset: 0x000598C0
	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < this.frames.Count; i++)
		{
			Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)i / (float)this.frames.Count);
			Gizmos.DrawSphere(this.frames[i].position, 0.1f);
			Gizmos.DrawLine(this.frames[i].position, this.frames[i].position + this.frames[i].up * 0.5f);
		}
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x0005B76E File Offset: 0x0005996E
	public void Spawn()
	{
		this.GenerateData();
		this.Apply();
		this.AddLights();
		this.addAlongSpline();
		if (this.spawnAlongSpline != null)
		{
			this.SpawnItemsAlongSpline();
		}
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x0005B79C File Offset: 0x0005999C
	private void AddLights()
	{
		Transform transform = base.transform.Find("BakedLight");
		if (transform == null)
		{
			return;
		}
		GameObject gameObject = transform.gameObject;
		Transform transform2 = base.transform.Find("BakedLights");
		for (int i = transform2.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(transform2.GetChild(i).gameObject);
		}
		if (this.spawnLights)
		{
			for (int j = 0; j < this.frames.Count; j += 3)
			{
				Object.Instantiate<GameObject>(gameObject, this.frames[j].position + this.frames[j].up * 4f, Quaternion.identity, transform2).SetActive(true);
			}
		}
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x0005B868 File Offset: 0x00059A68
	private void addAlongSpline()
	{
		if (this.detailsAlongSpline.Length == 0)
		{
			return;
		}
		Transform transform = base.transform.Find("DetailObjects");
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.frames.Count; j += this.stepsAlongSpline)
		{
			for (int k = 0; k < this.detailsAlongSpline.Length; k++)
			{
				Object.Instantiate<GameObject>(this.detailsAlongSpline[k], this.frames[j].position, quaternion.identity, transform).SetActive(true);
			}
		}
		for (int l = 0; l < this.detailsAlongSpline.Length; l++)
		{
			this.detailsAlongSpline[l].SetActive(false);
		}
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x0005B938 File Offset: 0x00059B38
	private void SpawnItemsAlongSpline()
	{
		this.splineObjectParent = base.transform.Find("SpawnedObjects");
		if (this.splineObjectParent == null)
		{
			this.splineObjectParent = new GameObject("SpawnedObjects").transform;
			this.splineObjectParent.SetParent(base.transform);
		}
		for (int i = this.splineObjectParent.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(this.splineObjectParent.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.frames.Count; j++)
		{
			Object.Instantiate<GameObject>(this.spawnAlongSpline, this.frames[j].position, Quaternion.LookRotation(this.frames[j].forward), this.splineObjectParent).SetActive(true);
		}
		this.spawnAlongSpline.SetActive(false);
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x0005BA1D File Offset: 0x00059C1D
	private void GenerateData()
	{
		this.Simulate();
		this.Simplify();
		this.SmoothUps();
		this.SmoothUps();
		this.SmoothUps();
		this.SmoothUps();
		this.SmoothUps();
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x0005BA4C File Offset: 0x00059C4C
	public void Apply()
	{
		SplineContainer componentInChildren = base.GetComponentInChildren<SplineContainer>();
		componentInChildren.transform.position = Vector3.zero;
		componentInChildren.transform.rotation = Quaternion.identity;
		for (int i = componentInChildren.Splines.Count - 1; i >= 0; i--)
		{
			SplineUtility.RemoveSplineAt<SplineContainer>(componentInChildren, i);
		}
		Spline spline = new Spline();
		foreach (LavaRiver.LavaRiverFrame lavaRiverFrame in this.frames)
		{
			Quaternion rotationWithUp = HelperFunctions.GetRotationWithUp(lavaRiverFrame.forward, lavaRiverFrame.up);
			spline.Add(new BezierKnot(lavaRiverFrame.position, Vector3.zero, Vector3.zero, rotationWithUp), 0);
		}
		for (int j = 0; j < spline.Count; j++)
		{
			spline.SetKnot(j, new BezierKnot(spline[j].Position, spline[j].TangentIn, spline[j].TangentOut, Quaternion.Euler(Vector3.left)), 1);
		}
		SplineUtility.AddSpline<SplineContainer>(componentInChildren, spline);
		SplineExtrude component = componentInChildren.GetComponent<SplineExtrude>();
		component.GetComponent<MeshFilter>().mesh = new Mesh();
		component.Capped = true;
		component.Rebuild();
		if (this.splash != null)
		{
			this.splash.transform.position = this.frames[this.frames.Count - 1].position;
		}
		if (this.endRock != null)
		{
			this.endRock.transform.position = this.frames[this.frames.Count - 1].position;
			this.endRock.transform.rotation = Random.rotation;
		}
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x0005BC3C File Offset: 0x00059E3C
	private void Simulate()
	{
		LavaRiver.<>c__DisplayClass25_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.frames = new List<LavaRiver.LavaRiverFrame>();
		this.steps = this.maxSteps;
		CS$<>8__locals1.vel = base.transform.forward * this.spawnVel;
		CS$<>8__locals1.pos = base.transform.position + base.transform.up * 0.1f + base.transform.forward * 0.1f;
		CS$<>8__locals1.up = base.transform.up;
		CS$<>8__locals1.lastPos = CS$<>8__locals1.pos;
		while (this.<Simulate>g__SimulationStep|25_0(ref CS$<>8__locals1))
		{
		}
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x0005BCF4 File Offset: 0x00059EF4
	public void SmoothUps()
	{
		for (int i = 1; i < this.frames.Count - 1; i++)
		{
			LavaRiver.LavaRiverFrame lavaRiverFrame = this.frames[i - 1];
			LavaRiver.LavaRiverFrame lavaRiverFrame2 = this.frames[i];
			LavaRiver.LavaRiverFrame lavaRiverFrame3 = this.frames[i + 1];
			Vector3 normalized = (lavaRiverFrame.up + lavaRiverFrame2.up + lavaRiverFrame3.up).normalized;
			lavaRiverFrame2.up = normalized;
		}
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x0005BD70 File Offset: 0x00059F70
	public void Simplify()
	{
		for (int i = 1; i < this.frames.Count; i++)
		{
			LavaRiver.LavaRiverFrame lavaRiverFrame = this.frames[i - 1];
			LavaRiver.LavaRiverFrame lavaRiverFrame2 = this.frames[i];
			if (Vector3.Distance(lavaRiverFrame.position, lavaRiverFrame2.position) < this.prefDistancePerFrame)
			{
				this.frames.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x0005BDD8 File Offset: 0x00059FD8
	public void Clear()
	{
		this.frames.Clear();
		SplineContainer componentInChildren = base.GetComponentInChildren<SplineContainer>();
		componentInChildren.transform.position = Vector3.zero;
		componentInChildren.transform.rotation = Quaternion.identity;
		for (int i = componentInChildren.Splines.Count - 1; i >= 0; i--)
		{
			SplineUtility.RemoveSplineAt<SplineContainer>(componentInChildren, i);
		}
		componentInChildren.GetComponent<SplineExtrude>().Rebuild();
		this.endRock.transform.position = base.transform.position;
		if (this.splash != null)
		{
			this.splash.transform.position = base.transform.position;
		}
		if (this.splineObjectParent != null)
		{
			for (int j = this.splineObjectParent.childCount - 1; j >= 0; j--)
			{
				Object.DestroyImmediate(this.splineObjectParent.GetChild(j).gameObject);
			}
		}
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x0005BF38 File Offset: 0x0005A138
	[CompilerGenerated]
	private bool <Simulate>g__SimulationStep|25_0(ref LavaRiver.<>c__DisplayClass25_0 A_1)
	{
		this.steps--;
		if (this.steps < 0)
		{
			return false;
		}
		if (A_1.vel.magnitude < 0.01f)
		{
			return false;
		}
		if (Vector3.Distance(base.transform.position, A_1.pos) > this.maxLength)
		{
			return false;
		}
		A_1.vel += Vector3.down * this.gravity * this.timeStep;
		A_1.vel += A_1.up * -this.wallStick * this.timeStep;
		A_1.vel *= this.drag;
		Vector3 vector = A_1.pos + A_1.vel * this.timeStep;
		RaycastHit raycastHit = HelperFunctions.LineCheck(A_1.lastPos, vector, HelperFunctions.LayerType.TerrainMap, 0f, 1);
		if (raycastHit.transform)
		{
			A_1.up = raycastHit.normal;
			vector = raycastHit.point + raycastHit.normal * 0.05f;
			A_1.vel = Vector3.ProjectOnPlane(A_1.vel, raycastHit.normal);
		}
		A_1.pos = vector;
		this.frames.Add(new LavaRiver.LavaRiverFrame
		{
			position = A_1.pos,
			up = A_1.up,
			forward = A_1.vel.normalized
		});
		A_1.lastPos = A_1.pos;
		return true;
	}

	// Token: 0x04001094 RID: 4244
	public float spawnVel = 5f;

	// Token: 0x04001095 RID: 4245
	public float gravity = 10f;

	// Token: 0x04001096 RID: 4246
	public float wallStick;

	// Token: 0x04001097 RID: 4247
	public float drag = 0.8f;

	// Token: 0x04001098 RID: 4248
	public float timeStep = 0.02f;

	// Token: 0x04001099 RID: 4249
	public int maxSteps = 1000;

	// Token: 0x0400109A RID: 4250
	public float maxLength = 500f;

	// Token: 0x0400109B RID: 4251
	public bool spawnLights = true;

	// Token: 0x0400109C RID: 4252
	private int steps;

	// Token: 0x0400109D RID: 4253
	public float prefDistancePerFrame = 0.3f;

	// Token: 0x0400109E RID: 4254
	public GameObject endRock;

	// Token: 0x0400109F RID: 4255
	public GameObject splash;

	// Token: 0x040010A0 RID: 4256
	public GameObject spawnAlongSpline;

	// Token: 0x040010A1 RID: 4257
	public GameObject[] detailsAlongSpline;

	// Token: 0x040010A2 RID: 4258
	public int stepsAlongSpline;

	// Token: 0x040010A3 RID: 4259
	private Transform splineObjectParent;

	// Token: 0x040010A4 RID: 4260
	public List<LavaRiver.LavaRiverFrame> frames = new List<LavaRiver.LavaRiverFrame>();

	// Token: 0x020004DF RID: 1247
	[Serializable]
	public class LavaRiverFrame
	{
		// Token: 0x04001ACA RID: 6858
		public Vector3 position;

		// Token: 0x04001ACB RID: 6859
		public Vector3 up;

		// Token: 0x04001ACC RID: 6860
		public Vector3 forward;
	}
}
