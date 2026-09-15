using System;
using UnityEngine;

// Token: 0x02000260 RID: 608
[ExecuteAlways]
public class FogSphere : MonoBehaviour
{
	// Token: 0x06001154 RID: 4436 RVA: 0x0005717C File Offset: 0x0005537C
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0005718A File Offset: 0x0005538A
	private void OnDisable()
	{
		Shader.SetGlobalFloat("FogEnabled", 0f);
		Shader.SetGlobalFloat("_FogSphereSize", 9999999f);
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x000571AC File Offset: 0x000553AC
	private void Update()
	{
		this.SetSize();
		this.SetSharderVars();
		if (this.currentSize > 120f)
		{
			this.t = false;
		}
		if (!this.t && this.currentSize < 120f)
		{
			this.t = true;
			for (int i = 0; i < this.fogStart.Length; i++)
			{
				this.fogStart[i].Play(default(Vector3));
			}
		}
		if (!this.t2 && this.REVEAL_AMOUNT > 0.1f)
		{
			this.t2 = true;
			for (int j = 0; j < this.fogReveal.Length; j++)
			{
				this.fogReveal[j].Play(default(Vector3));
			}
		}
		if (this.REVEAL_AMOUNT < 0.1f)
		{
			this.t2 = false;
		}
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x00057278 File Offset: 0x00055478
	private void SetSharderVars()
	{
		if (this.mpb == null)
		{
			this.mpb = new MaterialPropertyBlock();
		}
		this.rend.GetPropertyBlock(this.mpb);
		this.mpb.SetFloat("_PADDING", this.PADDING);
		this.mpb.SetFloat("_FogDepth", this.currentSize);
		this.mpb.SetFloat("_RevealAmount", this.REVEAL_AMOUNT);
		this.mpb.SetVector("_FogCenter", this.fogPoint);
		Shader.SetGlobalFloat("_FogSphereSize", this.currentSize);
		Shader.SetGlobalVector("FogCenter", this.fogPoint);
		Shader.SetGlobalFloat("FogEnabled", this.ENABLE);
		if (Application.isPlaying && Character.localCharacter != null)
		{
			Character.localCharacter.data.isInFog = false;
			if (Mathf.Approximately(this.ENABLE, 1f) && Character.localCharacter != null && Vector3.Distance(this.fogPoint, Character.localCharacter.Center) > this.currentSize)
			{
				float num = 0.0105f;
				if (Character.localCharacter.data.isSkeleton)
				{
					Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num / 8f * Time.deltaTime, false, true, true);
				}
				else
				{
					Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, num * Time.deltaTime, false, true, true);
				}
				Character.localCharacter.data.isInFog = true;
			}
		}
		this.rend.SetPropertyBlock(this.mpb);
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x00057428 File Offset: 0x00055628
	private void SetSize()
	{
		float num = (this.currentSize + this.PADDING) * this.ratio;
		base.transform.localScale = Vector3.one * num;
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x00057460 File Offset: 0x00055660
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(this.fogPoint, this.currentSize);
	}

	// Token: 0x04000FCE RID: 4046
	public float currentSize = 50f;

	// Token: 0x04000FCF RID: 4047
	[Range(0f, 1f)]
	public float ENABLE = 1f;

	// Token: 0x04000FD0 RID: 4048
	[Range(0f, 1f)]
	public float REVEAL_AMOUNT;

	// Token: 0x04000FD1 RID: 4049
	public float PADDING = 300f;

	// Token: 0x04000FD2 RID: 4050
	public Vector3 fogPoint;

	// Token: 0x04000FD3 RID: 4051
	private float ratio = 2f;

	// Token: 0x04000FD4 RID: 4052
	private Renderer rend;

	// Token: 0x04000FD5 RID: 4053
	public SFX_Instance[] fogStart;

	// Token: 0x04000FD6 RID: 4054
	private bool t;

	// Token: 0x04000FD7 RID: 4055
	public SFX_Instance[] fogReveal;

	// Token: 0x04000FD8 RID: 4056
	private bool t2;

	// Token: 0x04000FD9 RID: 4057
	private MaterialPropertyBlock mpb;
}
