using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000339 RID: 825
public class SpecialDayZone : MonoBehaviour
{
	// Token: 0x0600153F RID: 5439 RVA: 0x0006C46C File Offset: 0x0006A66C
	private void Start()
	{
		this.bounds.center = base.transform.position;
		this.outerBounds.center = base.transform.position;
		this.outerBounds.size = this.bounds.size + Vector3.one * this.blendSize;
		if (this.specialLight)
		{
			this.specialLight.color = Color.black;
		}
		if (this.specialLight)
		{
			this.specialLight.enabled = false;
		}
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x0006C508 File Offset: 0x0006A708
	private void OnDrawGizmosSelected()
	{
		this.bounds.center = base.transform.position;
		this.outerBounds.center = base.transform.position;
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
		Gizmos.DrawWireCube(this.bounds.center, new Vector3(this.bounds.size.x / 2f, this.bounds.size.y, this.bounds.size.z));
		Gizmos.DrawWireCube(this.bounds.center, new Vector3(this.bounds.size.x, this.bounds.size.y / 2f, this.bounds.size.z));
		Gizmos.DrawWireCube(this.bounds.center, new Vector3(this.bounds.size.x, this.bounds.size.y, this.bounds.size.z / 2f));
		Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
		this.outerBounds.size = this.bounds.size + Vector3.one * this.blendSize;
		Gizmos.DrawWireCube(this.outerBounds.center, this.outerBounds.size);
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x0006C6A4 File Offset: 0x0006A8A4
	private void Update()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (this.outerBounds.Contains(Character.localCharacter.Center))
		{
			if (this.specialLight)
			{
				this.specialLight.enabled = true;
			}
			this.inBounds = true;
			return;
		}
		if (this.specialLight)
		{
			this.specialLight.enabled = false;
		}
		this.inBounds = false;
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x0006C716 File Offset: 0x0006A916
	private void OnDisable()
	{
		if (this.specialLight)
		{
			this.specialLight.color = Color.black;
		}
		this.specialLight.enabled = false;
	}

	// Token: 0x040013C0 RID: 5056
	public bool overrideSun;

	// Token: 0x040013C1 RID: 5057
	[FormerlySerializedAs("lightIntensity")]
	public float daylLightIntensity = 1.5f;

	// Token: 0x040013C2 RID: 5058
	public float nightLightIntensity = 1.5f;

	// Token: 0x040013C3 RID: 5059
	public Color specialSunColor;

	// Token: 0x040013C4 RID: 5060
	public bool useCustomSun;

	// Token: 0x040013C5 RID: 5061
	public Light specialLight;

	// Token: 0x040013C6 RID: 5062
	public bool useCustomColorVals = true;

	// Token: 0x040013C7 RID: 5063
	public Color specialTopColor;

	// Token: 0x040013C8 RID: 5064
	public Color specialMidColor;

	// Token: 0x040013C9 RID: 5065
	public Color specialBottomColor;

	// Token: 0x040013CA RID: 5066
	[FormerlySerializedAs("shaderValsToBlend")]
	public ShaderVal[] globalShaderVals;

	// Token: 0x040013CB RID: 5067
	private float baseFog;

	// Token: 0x040013CC RID: 5068
	public bool overrideFog = true;

	// Token: 0x040013CD RID: 5069
	public float fogDensity = 400f;

	// Token: 0x040013CE RID: 5070
	public Bounds bounds;

	// Token: 0x040013CF RID: 5071
	public Bounds outerBounds;

	// Token: 0x040013D0 RID: 5072
	public float blendSize = 50f;

	// Token: 0x040013D1 RID: 5073
	[Header("Debug")]
	public bool inBounds;
}
