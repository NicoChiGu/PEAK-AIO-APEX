using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020002C9 RID: 713
public class PointPing : MonoBehaviour
{
	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06001344 RID: 4932 RVA: 0x00062188 File Offset: 0x00060388
	public Vector3 PingerForward
	{
		get
		{
			return (base.transform.position - this.pointPinger.character.Head).normalized;
		}
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x000621BD File Offset: 0x000603BD
	public void Init(Character character)
	{
		this.character = character;
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x000621C6 File Offset: 0x000603C6
	private void Awake()
	{
		this.material = this.renderer.material;
	}

	// Token: 0x06001347 RID: 4935 RVA: 0x000621D9 File Offset: 0x000603D9
	private void Start()
	{
		this.camera = Camera.main;
		this.Go();
		this.pingSound.Play(base.transform.position);
	}

	// Token: 0x06001348 RID: 4936 RVA: 0x00062202 File Offset: 0x00060402
	public void Update()
	{
		this.Go();
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x0006220C File Offset: 0x0006040C
	private void Go()
	{
		float num = ExtCamera.SizeOfFrustumAtDistance(this.camera, Vector3.Distance(Character.localCharacter.Center, base.transform.position));
		this.character.refs.animations.point = base.gameObject;
		num = ExtDataTypes.PClampFloat(this.minMaxScale, num);
		base.transform.localScale = ExtSwizzle.xxx(num * this.sizeOfFrustum);
		Vector3 vector = base.transform.position - this.camera.transform.position;
		float num2 = Vector3.Angle(this.PingerForward, vector);
		Vector3 vector2 = Vector3.Lerp(-this.hitNormal, this.PingerForward, ExtDataTypes.Remap(num2, 0f, this.angleThing, 0f, 1f));
		float num3 = Vector3.Angle(vector2, vector);
		Vector3 vector3 = Vector3.Lerp(-Vector3.up, vector2, ExtDataTypes.Remap(num3, 0f, this.angleThing, 0f, 1f));
		base.transform.rotation = Quaternion.LookRotation(vector3, Vector3.up);
	}

	// Token: 0x040011ED RID: 4589
	public float sizeOfFrustum = 0.1f;

	// Token: 0x040011EE RID: 4590
	public Vector2 minMaxScale = new Vector2(0.2f, 3f);

	// Token: 0x040011EF RID: 4591
	public Vector2 visibilityFullNoneNoLos = new Vector2(30f, 50f);

	// Token: 0x040011F0 RID: 4592
	public float NoLosVisibilityMul = 0.5f;

	// Token: 0x040011F1 RID: 4593
	public float angleThing = 90f;

	// Token: 0x040011F2 RID: 4594
	public MeshRenderer renderer;

	// Token: 0x040011F3 RID: 4595
	public SpriteRenderer ringRenderer;

	// Token: 0x040011F4 RID: 4596
	public SFX_Instance pingSound;

	// Token: 0x040011F5 RID: 4597
	public Material material;

	// Token: 0x040011F6 RID: 4598
	public Vector3 hitNormal;

	// Token: 0x040011F7 RID: 4599
	public PointPinger pointPinger;

	// Token: 0x040011F8 RID: 4600
	private Camera camera;

	// Token: 0x040011F9 RID: 4601
	private Character character;
}
