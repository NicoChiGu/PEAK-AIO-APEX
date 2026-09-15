using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000127 RID: 295
public class OrbThatMakesYouSleepy : MonoBehaviour
{
	// Token: 0x06000961 RID: 2401 RVA: 0x000319A2 File Offset: 0x0002FBA2
	private void Start()
	{
		this.anim.speed = this.animSpeed;
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x000319B5 File Offset: 0x0002FBB5
	public void Tick()
	{
		this.UpdateHypnosis(false);
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x000319C0 File Offset: 0x0002FBC0
	private void LateUpdate()
	{
		if (this.fakeBerry != null && this.fakeBerry.gameObject.activeInHierarchy)
		{
			this.anim.speed = this.animSpeed;
			this.ambientParticles.gameObject.SetActive(true);
			this.fakeBerry.transform.localPosition = new Vector3(-0.013f, -0.22f, 0.008f);
			this.fakeBerry.transform.localEulerAngles = Vector3.zero;
			return;
		}
		this.anim.speed = 0f;
		this.ambientParticles.gameObject.SetActive(false);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00031A6A File Offset: 0x0002FC6A
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(this.orb.transform.position, this.orbRadius);
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00031A94 File Offset: 0x0002FC94
	private void UpdateCastPositions()
	{
		this.castPositions[0] = this.orb.transform.position;
		this.castPositions[1] = this.orb.transform.position + MainCamera.instance.cam.transform.right * this.orbRadius;
		this.castPositions[2] = this.orb.transform.position - MainCamera.instance.cam.transform.right * this.orbRadius;
		this.castPositions[3] = this.orb.transform.position + MainCamera.instance.cam.transform.up * this.orbRadius;
		this.castPositions[4] = this.orb.transform.position - MainCamera.instance.cam.transform.up * this.orbRadius;
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x00031BBD File Offset: 0x0002FDBD
	private void DebugHypnosis()
	{
		this.UpdateHypnosis(true);
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00031BC8 File Offset: 0x0002FDC8
	private void UpdateHypnosis(bool debug = false)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!this.fakeBerry || !this.fakeBerry.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!ClassExtensionsMethods.UnityObjectExists<Character>(Character.localCharacter) || !Character.localCharacter.data.fullyConscious)
		{
			return;
		}
		Vector3 vector = Character.localCharacter.Center - this.orb.transform.position;
		if (debug)
		{
			Debug.Log("distance to character: " + vector.magnitude.ToString());
		}
		if (vector.magnitude > this.maxDistance)
		{
			return;
		}
		if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(MainCamera.instance.cam), new Bounds(this.orb.transform.position, Vector3.one * 0.5f)))
		{
			if (debug)
			{
				Debug.Log("Not inside view frustum");
			}
			return;
		}
		int num = 0;
		this.UpdateCastPositions();
		for (int i = 0; i < this.castPositions.Length; i++)
		{
			if (debug)
			{
				Debug.Log(string.Format("testing cast {0}", i));
			}
			Collider collider = HelperFunctions.LineCheck(this.castPositions[i], MainCamera.instance.cam.transform.position, HelperFunctions.LayerType.AllPhysical, 0f, 1).collider;
			if (collider == null)
			{
				if (debug)
				{
					Debug.Log("Hit nothing");
				}
				num++;
			}
			else if (collider.gameObject.GetComponentInParent<Character>() == Character.localCharacter)
			{
				if (debug)
				{
					Debug.Log("Hit our own character");
				}
				num++;
			}
		}
		if (num == 0)
		{
			if (debug)
			{
				Debug.Log("Blocked");
			}
			return;
		}
		float num2 = Vector3.Angle(-MainCamera.instance.cam.transform.forward, vector);
		float num3 = Mathf.InverseLerp(this.maxDistance, 2f, vector.magnitude);
		if (debug)
		{
			Debug.Log(string.Format("factor 1: {0}", num3));
		}
		float num4 = Mathf.Lerp(10f, 110f, num3);
		if (debug)
		{
			Debug.Log(string.Format("max angle: {0}", num4));
		}
		float num5 = Mathf.InverseLerp(num4, num4 / 2f, num2);
		if (debug)
		{
			Debug.Log(string.Format("factor 2 {0}", num5));
		}
		float num6 = Mathf.Lerp(this.minDrowsyPerTick, this.maxDrowsyPerTick, Mathf.Min(num3, num5));
		if (num <= 2)
		{
			num6 *= 0.5f;
		}
		if (debug)
		{
			Debug.Log(string.Format("Adding Status: {0}", num6));
		}
		Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, num6, false, true, true);
		this.particle.Play();
	}

	// Token: 0x040008BD RID: 2237
	public Transform orb;

	// Token: 0x040008BE RID: 2238
	public float orbRadius;

	// Token: 0x040008BF RID: 2239
	public float maxDistance;

	// Token: 0x040008C0 RID: 2240
	public float minDrowsyPerSecond;

	// Token: 0x040008C1 RID: 2241
	public float maxDrowsyPerSecond;

	// Token: 0x040008C2 RID: 2242
	public float minDrowsyPerTick;

	// Token: 0x040008C3 RID: 2243
	public float maxDrowsyPerTick;

	// Token: 0x040008C4 RID: 2244
	private Vector3[] castPositions = new Vector3[5];

	// Token: 0x040008C5 RID: 2245
	public ParticleSystem particle;

	// Token: 0x040008C6 RID: 2246
	public Animator anim;

	// Token: 0x040008C7 RID: 2247
	public float animSpeed = 1f;

	// Token: 0x040008C8 RID: 2248
	public ParticleSystem ambientParticles;

	// Token: 0x040008C9 RID: 2249
	public FakeItem fakeBerry;

	// Token: 0x040008CA RID: 2250
	private Plane[] planes = new Plane[6];
}
