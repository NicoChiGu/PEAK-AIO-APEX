using System;
using UnityEngine;

// Token: 0x02000335 RID: 821
public class SlideTrail : MonoBehaviour
{
	// Token: 0x06001532 RID: 5426 RVA: 0x0006C0D8 File Offset: 0x0006A2D8
	private void Start()
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		this.l = componentsInChildren[0];
		this.r = componentsInChildren[1];
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x0006C10C File Offset: 0x0006A30C
	private void Update()
	{
		this.l.transform.position = this.character.GetBodypartRig(BodypartType.Hand_L).position;
		this.r.transform.position = this.character.GetBodypartRig(BodypartType.Hand_R).position;
		if (this.character.IsSliding() && this.character.data.outOfStaminaFor > 2f)
		{
			this.HandlePart(this.l, this.l.transform.position);
			this.HandlePart(this.r, this.r.transform.position);
			return;
		}
		this.SetPartOn(this.l, false);
		this.SetPartOn(this.r, false);
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x0006C1D4 File Offset: 0x0006A3D4
	private void HandlePart(ParticleSystem part, Vector3 position)
	{
		if (HelperFunctions.LineCheck(position, position - this.character.data.groundNormal * 0.3f, HelperFunctions.LayerType.Terrain, 0f, 1).transform)
		{
			this.SetPartOn(part, true);
			return;
		}
		this.SetPartOn(part, false);
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x0006C22E File Offset: 0x0006A42E
	private void SetPartOn(ParticleSystem part, bool on)
	{
		if (on && !part.isPlaying)
		{
			part.Play(true);
			return;
		}
		if (!on && part.isPlaying)
		{
			part.Stop(true);
		}
	}

	// Token: 0x040013B8 RID: 5048
	private ParticleSystem l;

	// Token: 0x040013B9 RID: 5049
	private ParticleSystem r;

	// Token: 0x040013BA RID: 5050
	private Character character;
}
