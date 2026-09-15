using System;
using TMPro;
using UnityEngine;

// Token: 0x02000230 RID: 560
[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class CircularTextMeshPro : MonoBehaviour
{
	// Token: 0x17000117 RID: 279
	// (get) Token: 0x0600109E RID: 4254 RVA: 0x00053BE4 File Offset: 0x00051DE4
	// (set) Token: 0x0600109F RID: 4255 RVA: 0x00053BEC File Offset: 0x00051DEC
	[Tooltip("The radius of the text circle arc")]
	public float Radius
	{
		get
		{
			return this.m_radius;
		}
		set
		{
			this.m_radius = value;
			this.OnCurvePropertyChanged();
		}
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x00053BFB File Offset: 0x00051DFB
	private void Awake()
	{
		this.m_TextComponent = base.gameObject.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x00053C0E File Offset: 0x00051E0E
	private void OnEnable()
	{
		this.m_TextComponent.OnPreRenderText += new Action<TMP_TextInfo>(this.UpdateTextCurve);
		this.OnCurvePropertyChanged();
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x00053C2D File Offset: 0x00051E2D
	private void OnDisable()
	{
		this.m_TextComponent.OnPreRenderText -= new Action<TMP_TextInfo>(this.UpdateTextCurve);
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x00053C46 File Offset: 0x00051E46
	protected void OnCurvePropertyChanged()
	{
		this.UpdateTextCurve(this.m_TextComponent.textInfo);
		this.m_TextComponent.ForceMeshUpdate(false, false);
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x00053C68 File Offset: 0x00051E68
	protected void UpdateTextCurve(TMP_TextInfo textInfo)
	{
		for (int i = 0; i < textInfo.characterInfo.Length; i++)
		{
			if (textInfo.characterInfo[i].isVisible)
			{
				int vertexIndex = textInfo.characterInfo[i].vertexIndex;
				int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
				Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
				Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine);
				vertices[vertexIndex] += -vector;
				vertices[vertexIndex + 1] += -vector;
				vertices[vertexIndex + 2] += -vector;
				vertices[vertexIndex + 3] += -vector;
				Matrix4x4 matrix4x = this.ComputeTransformationMatrix(vector, textInfo, i);
				vertices[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex]);
				vertices[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 1]);
				vertices[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 2]);
				vertices[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 3]);
			}
		}
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x00053E04 File Offset: 0x00052004
	protected Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, TMP_TextInfo textInfo, int charIdx)
	{
		float num = this.m_radius + textInfo.lineInfo[textInfo.characterInfo[charIdx].lineNumber].baseline;
		float num2 = 2f * num * 3.1415927f;
		float num3 = ((charMidBaselinePos.x / num2 - 0.5f) * 360f + 90f) * 0.017453292f;
		float num4 = Mathf.Cos(num3);
		float num5 = Mathf.Sin(num3);
		Vector2 vector;
		vector..ctor(num4 * num, -num5 * num);
		float num6 = -Mathf.Atan2(num5, num4) * 57.29578f - 90f;
		return Matrix4x4.TRS(new Vector3(vector.x, vector.y, 0f), Quaternion.AngleAxis(num6, Vector3.forward), Vector3.one);
	}

	// Token: 0x04000ED7 RID: 3799
	private TextMeshProUGUI m_TextComponent;

	// Token: 0x04000ED8 RID: 3800
	[SerializeField]
	[HideInInspector]
	private float m_radius = 10f;
}
