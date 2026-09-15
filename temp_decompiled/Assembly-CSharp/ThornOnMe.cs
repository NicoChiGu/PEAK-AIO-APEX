using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class ThornOnMe : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x060003BB RID: 955 RVA: 0x00018942 File Offset: 0x00016B42
	private void OnEnable()
	{
		if (this.mainRenderer == null)
		{
			this.AddPropertyBlock();
		}
	}

	// Token: 0x060003BC RID: 956 RVA: 0x00018958 File Offset: 0x00016B58
	private float GetPopOutTime(bool solo)
	{
		if (!solo)
		{
			return 120f;
		}
		return 30f;
	}

	// Token: 0x060003BD RID: 957 RVA: 0x00018968 File Offset: 0x00016B68
	public bool ShouldPopOut()
	{
		return this.stuckIn && Time.time > this.popOutTime;
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00018984 File Offset: 0x00016B84
	public void EnableThorn()
	{
		if (!this.character.IsLocal || this.visibleLocally)
		{
			base.gameObject.SetActive(true);
		}
		this.stuckIn = true;
		this.popOutTime = Time.time + this.GetPopOutTime(Character.AllCharacters.Count == 1);
	}

	// Token: 0x060003BF RID: 959 RVA: 0x000189D8 File Offset: 0x00016BD8
	public void DisableThorn()
	{
		base.gameObject.SetActive(false);
		this.stuckIn = false;
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x000189F0 File Offset: 0x00016BF0
	public bool IsInteractible(Character interactor)
	{
		if (interactor.IsStuck())
		{
			return false;
		}
		float num = Vector3.Angle(base.transform.position - MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
		float num2 = ((Character.AllCharacters.Count == 1 || interactor != this.character) ? 15f : 0f);
		return num <= 5f + num2;
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00018A6E File Offset: 0x00016C6E
	public void Interact(Character interactor)
	{
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x00018A70 File Offset: 0x00016C70
	private void AddPropertyBlock()
	{
		this.mpb = new MaterialPropertyBlock();
		this.mainRenderer = base.GetComponentInChildren<MeshRenderer>();
		if (!this.mainRenderer)
		{
			this.mainRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
		}
		this.mainRenderer.GetPropertyBlock(this.mpb);
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x00018ABE File Offset: 0x00016CBE
	public void HoverEnter()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
		this.mainRenderer.SetPropertyBlock(this.mpb);
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x00018AE6 File Offset: 0x00016CE6
	public void HoverExit()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
		this.mainRenderer.SetPropertyBlock(this.mpb);
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x00018B0E File Offset: 0x00016D0E
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x00018B1B File Offset: 0x00016D1B
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x00018B23 File Offset: 0x00016D23
	public string GetInteractionText()
	{
		return LocalizedText.GetText("REMOVE", true);
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00018B30 File Offset: 0x00016D30
	public string GetName()
	{
		return LocalizedText.GetText("Name_Thorn", true);
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x00018B3D File Offset: 0x00016D3D
	public bool IsConstantlyInteractable(Character interactor)
	{
		return true;
	}

	// Token: 0x060003CA RID: 970 RVA: 0x00018B40 File Offset: 0x00016D40
	public float GetInteractTime(Character interactor)
	{
		if (Character.AllCharacters.Count != 1 && !(interactor != this.character))
		{
			return 3f;
		}
		return 1f;
	}

	// Token: 0x060003CB RID: 971 RVA: 0x00018B68 File Offset: 0x00016D68
	public void Interact_CastFinished(Character interactor)
	{
		this.character.refs.afflictions.RemoveThorn(this);
	}

	// Token: 0x060003CC RID: 972 RVA: 0x00018B80 File Offset: 0x00016D80
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x060003CD RID: 973 RVA: 0x00018B82 File Offset: 0x00016D82
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x060003CE RID: 974 RVA: 0x00018B84 File Offset: 0x00016D84
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04000410 RID: 1040
	[HideInInspector]
	public Character character;

	// Token: 0x04000411 RID: 1041
	public int thornDamage;

	// Token: 0x04000412 RID: 1042
	public bool stuckIn;

	// Token: 0x04000413 RID: 1043
	public bool visibleLocally;

	// Token: 0x04000414 RID: 1044
	private float popOutTime;

	// Token: 0x04000415 RID: 1045
	private MaterialPropertyBlock mpb;

	// Token: 0x04000416 RID: 1046
	public Renderer mainRenderer;
}
