using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000265 RID: 613
public class FootStepPlayer : MonoBehaviour
{
	// Token: 0x06001164 RID: 4452 RVA: 0x000575D6 File Offset: 0x000557D6
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x000575F0 File Offset: 0x000557F0
	private void Update()
	{
		this.doStep = 0;
		using (IEnumerator enumerator = base.transform.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (((Transform)enumerator.Current).gameObject.activeSelf)
				{
					this.doStep++;
				}
			}
		}
		if (this.doStep == 0)
		{
			this.t = false;
		}
		if (this.doStep > 0 && !this.t)
		{
			this.PlayStep();
		}
		if (this.character.data.sinceGrounded <= 0f && !this.onGround.active)
		{
			this.onGround.SetActive(true);
			this.offGround.SetActive(false);
			this.PlayStep();
		}
		if (this.character.data.sinceGrounded > 0.25f && !this.offGround.active)
		{
			this.offGround.SetActive(true);
			this.onGround.SetActive(false);
			this.PlayStep();
		}
		if (this.bingBongRoom)
		{
			this.timer += Time.deltaTime;
			if (this.timer > 4f)
			{
				this.ambience.bingBongStatue.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0005774C File Offset: 0x0005594C
	private void PlayStep()
	{
		if (Physics.Linecast(base.transform.position, base.transform.position + Vector3.down * 100f, ref this.hit, this.floorLayer))
		{
			MeshRenderer component = this.hit.collider.GetComponent<MeshRenderer>();
			if (this.hit.collider.name == "BigRoom")
			{
				this.bingBongRoom = true;
			}
			if (component)
			{
				if (component.material.name == this.beachSand.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 1, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.beachRock.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 2, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.desertSand.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 1, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.desertSand.name)
				{
					this.surfaceLookup.PlayStep(base.transform.position, 1, this.audioSourceOverride);
					this.t = true;
					return;
				}
				foreach (Material material in this.jungleGrass)
				{
					if (component.material.name == material.name + " (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 3, this.audioSourceOverride);
						}
						this.t = true;
					}
				}
				if (component.material.name == this.jungleRock.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 4, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.iceRock.name + " (Instance)")
				{
					if (this.ambience)
					{
						this.ambience.naturelessTerrain = 30f;
					}
					this.surfaceLookup.PlayStep(base.transform.position, 5, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.iceSnow.name + " (Instance)")
				{
					if (this.ambience)
					{
						this.ambience.naturelessTerrain = 30f;
					}
					this.surfaceLookup.PlayStep(base.transform.position, 6, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.volcanoRock.name + " (Instance)")
				{
					if (this.ambience)
					{
						this.ambience.naturelessTerrain = 30f;
						this.ambience.vulcanoT = 10f;
					}
					this.surfaceLookup.PlayStep(base.transform.position, 9, this.audioSourceOverride);
					this.t = true;
					return;
				}
				foreach (Material material2 in this.metal)
				{
					if (component.material.name == material2.name + " (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 7, this.audioSourceOverride);
						}
						this.t = true;
					}
				}
				foreach (Material material3 in this.wood)
				{
					if (component.material.name == material3.name)
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 8, this.audioSourceOverride);
						}
						this.t = true;
					}
					if (component.material.name == material3.name + " (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 8, this.audioSourceOverride);
						}
						this.t = true;
					}
					if (component.material.name == material3.name + " (Instance) (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 8, this.audioSourceOverride);
						}
						this.t = true;
					}
				}
				if (!this.t)
				{
					this.surfaceLookup.PlayStep(base.transform.position, 0, this.audioSourceOverride);
					this.t = true;
				}
			}
			else
			{
				this.surfaceLookup.PlayStep(base.transform.position, 0, this.audioSourceOverride);
				this.t = true;
			}
		}
		else
		{
			this.surfaceLookup.PlayStep(base.transform.position, 0, this.audioSourceOverride);
			this.t = true;
		}
		this.t = true;
	}

	// Token: 0x04000FE1 RID: 4065
	private Character character;

	// Token: 0x04000FE2 RID: 4066
	public LayerMask floorLayer;

	// Token: 0x04000FE3 RID: 4067
	public StepSoundCollection surfaceLookup;

	// Token: 0x04000FE4 RID: 4068
	private int doStep;

	// Token: 0x04000FE5 RID: 4069
	private bool t;

	// Token: 0x04000FE6 RID: 4070
	public Material beachSand;

	// Token: 0x04000FE7 RID: 4071
	public Material beachRock;

	// Token: 0x04000FE8 RID: 4072
	public Material desertSand;

	// Token: 0x04000FE9 RID: 4073
	public Material[] jungleGrass;

	// Token: 0x04000FEA RID: 4074
	public Material jungleRock;

	// Token: 0x04000FEB RID: 4075
	public Material iceSnow;

	// Token: 0x04000FEC RID: 4076
	public Material iceRock;

	// Token: 0x04000FED RID: 4077
	public Material volcanoRock;

	// Token: 0x04000FEE RID: 4078
	public Material[] metal;

	// Token: 0x04000FEF RID: 4079
	public Material[] wood;

	// Token: 0x04000FF0 RID: 4080
	private RaycastHit hit;

	// Token: 0x04000FF1 RID: 4081
	public GameObject onGround;

	// Token: 0x04000FF2 RID: 4082
	public GameObject offGround;

	// Token: 0x04000FF3 RID: 4083
	public AmbienceAudio ambience;

	// Token: 0x04000FF4 RID: 4084
	public AudioSource audioSourceOverride;

	// Token: 0x04000FF5 RID: 4085
	private bool bingBongRoom;

	// Token: 0x04000FF6 RID: 4086
	private float timer;
}
