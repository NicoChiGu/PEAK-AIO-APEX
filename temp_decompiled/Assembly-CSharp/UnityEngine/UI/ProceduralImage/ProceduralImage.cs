using System;
using UnityEngine.Events;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x02000377 RID: 887
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Procedural Image")]
	public class ProceduralImage : Image
	{
		// Token: 0x1700014D RID: 333
		// (get) Token: 0x0600166E RID: 5742 RVA: 0x000741ED File Offset: 0x000723ED
		// (set) Token: 0x0600166F RID: 5743 RVA: 0x00074215 File Offset: 0x00072415
		private static Material DefaultProceduralImageMaterial
		{
			get
			{
				if (ProceduralImage.materialInstance == null)
				{
					ProceduralImage.materialInstance = new Material(Shader.Find("UI/Procedural UI Image"));
				}
				return ProceduralImage.materialInstance;
			}
			set
			{
				ProceduralImage.materialInstance = value;
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06001670 RID: 5744 RVA: 0x0007421D File Offset: 0x0007241D
		// (set) Token: 0x06001671 RID: 5745 RVA: 0x00074225 File Offset: 0x00072425
		public float BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				this.borderWidth = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06001672 RID: 5746 RVA: 0x00074234 File Offset: 0x00072434
		// (set) Token: 0x06001673 RID: 5747 RVA: 0x0007423C File Offset: 0x0007243C
		public float FalloffDistance
		{
			get
			{
				return this.falloffDistance;
			}
			set
			{
				this.falloffDistance = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06001674 RID: 5748 RVA: 0x0007424B File Offset: 0x0007244B
		// (set) Token: 0x06001675 RID: 5749 RVA: 0x0007428B File Offset: 0x0007248B
		protected ProceduralImageModifier Modifier
		{
			get
			{
				if (this.modifier == null)
				{
					this.modifier = base.GetComponent<ProceduralImageModifier>();
					if (this.modifier == null)
					{
						this.ModifierType = typeof(FreeModifier);
					}
				}
				return this.modifier;
			}
			set
			{
				this.modifier = value;
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06001676 RID: 5750 RVA: 0x00074294 File Offset: 0x00072494
		// (set) Token: 0x06001677 RID: 5751 RVA: 0x000742A4 File Offset: 0x000724A4
		public Type ModifierType
		{
			get
			{
				return this.Modifier.GetType();
			}
			set
			{
				if (this.modifier != null && this.modifier.GetType() != value)
				{
					if (base.GetComponent<ProceduralImageModifier>() != null)
					{
						Object.DestroyImmediate(base.GetComponent<ProceduralImageModifier>());
					}
					base.gameObject.AddComponent(value);
					this.Modifier = base.GetComponent<ProceduralImageModifier>();
					this.SetAllDirty();
					return;
				}
				if (this.modifier == null)
				{
					base.gameObject.AddComponent(value);
					this.Modifier = base.GetComponent<ProceduralImageModifier>();
					this.SetAllDirty();
				}
			}
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x00074338 File Offset: 0x00072538
		protected override void OnEnable()
		{
			base.OnEnable();
			this.Init();
		}

		// Token: 0x06001679 RID: 5753 RVA: 0x00074346 File Offset: 0x00072546
		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Remove(this.m_OnDirtyVertsCallback, new UnityAction(this.OnVerticesDirty));
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x00074370 File Offset: 0x00072570
		private void Init()
		{
			this.FixTexCoordsInCanvas();
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Combine(this.m_OnDirtyVertsCallback, new UnityAction(this.OnVerticesDirty));
			base.preserveAspect = false;
			this.material = null;
			if (base.sprite == null)
			{
				base.sprite = EmptySprite.Get();
			}
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x000743CC File Offset: 0x000725CC
		protected void OnVerticesDirty()
		{
			if (base.sprite == null)
			{
				base.sprite = EmptySprite.Get();
			}
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x000743E8 File Offset: 0x000725E8
		protected void FixTexCoordsInCanvas()
		{
			Canvas componentInParent = base.GetComponentInParent<Canvas>();
			if (componentInParent != null)
			{
				this.FixTexCoordsInCanvas(componentInParent);
			}
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x0007440C File Offset: 0x0007260C
		protected void FixTexCoordsInCanvas(Canvas c)
		{
			c.additionalShaderChannels |= 7;
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x0007441C File Offset: 0x0007261C
		private Vector4 FixRadius(Vector4 vec)
		{
			Rect rect = base.rectTransform.rect;
			vec..ctor(Mathf.Max(vec.x, 0f), Mathf.Max(vec.y, 0f), Mathf.Max(vec.z, 0f), Mathf.Max(vec.w, 0f));
			float num = Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(rect.width / (vec.x + vec.y), rect.width / (vec.z + vec.w)), rect.height / (vec.x + vec.w)), rect.height / (vec.z + vec.y)), 1f);
			return vec * num;
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x000744F1 File Offset: 0x000726F1
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			base.OnPopulateMesh(toFill);
			this.EncodeAllInfoIntoVertices(toFill, this.CalculateInfo());
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x00074507 File Offset: 0x00072707
		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.FixTexCoordsInCanvas();
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00074518 File Offset: 0x00072718
		private ProceduralImageInfo CalculateInfo()
		{
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			float num = 1f / Mathf.Max(0f, this.falloffDistance);
			Vector4 vector = this.FixRadius(this.Modifier.CalculateRadius(pixelAdjustedRect));
			float num2 = Mathf.Min(pixelAdjustedRect.width, pixelAdjustedRect.height);
			return new ProceduralImageInfo(pixelAdjustedRect.width + this.falloffDistance, pixelAdjustedRect.height + this.falloffDistance, this.falloffDistance, num, vector / num2, this.borderWidth / num2 * 2f);
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x000745A8 File Offset: 0x000727A8
		private void EncodeAllInfoIntoVertices(VertexHelper vh, ProceduralImageInfo info)
		{
			UIVertex uivertex = default(UIVertex);
			Vector2 vector;
			vector..ctor(info.width, info.height);
			Vector2 vector2;
			vector2..ctor(this.EncodeFloats_0_1_16_16(info.radius.x, info.radius.y), this.EncodeFloats_0_1_16_16(info.radius.z, info.radius.w));
			Vector2 vector3;
			vector3..ctor((info.borderWidth == 0f) ? 1f : Mathf.Clamp01(info.borderWidth), info.pixelSize);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref uivertex, i);
				uivertex.position += (uivertex.uv0 - new Vector3(0.5f, 0.5f)) * info.fallOffDistance;
				uivertex.uv1 = vector;
				uivertex.uv2 = vector2;
				uivertex.uv3 = vector3;
				vh.SetUIVertex(uivertex, i);
			}
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x000746D4 File Offset: 0x000728D4
		private float EncodeFloats_0_1_16_16(float a, float b)
		{
			Vector2 vector;
			vector..ctor(1f, 1.5259022E-05f);
			return Vector2.Dot(new Vector2(Mathf.Floor(a * 65534f) / 65535f, Mathf.Floor(b * 65534f) / 65535f), vector);
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06001684 RID: 5764 RVA: 0x00074721 File Offset: 0x00072921
		// (set) Token: 0x06001685 RID: 5765 RVA: 0x0007473D File Offset: 0x0007293D
		public override Material material
		{
			get
			{
				if (this.m_Material == null)
				{
					return ProceduralImage.DefaultProceduralImageMaterial;
				}
				return base.material;
			}
			set
			{
				base.material = value;
			}
		}

		// Token: 0x04001554 RID: 5460
		[SerializeField]
		private float borderWidth;

		// Token: 0x04001555 RID: 5461
		private ProceduralImageModifier modifier;

		// Token: 0x04001556 RID: 5462
		private static Material materialInstance;

		// Token: 0x04001557 RID: 5463
		[SerializeField]
		private float falloffDistance = 1f;
	}
}
