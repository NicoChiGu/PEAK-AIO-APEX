using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
[ExecuteInEditMode]
public class RigCreatorCollider : MonoBehaviour
{
	// Token: 0x0600038C RID: 908 RVA: 0x0001801D File Offset: 0x0001621D
	private void Start()
	{
		if (this.disableOnStart)
		{
			this.Col().enabled = false;
			return;
		}
		base.GetComponentInParent<CharacterRagdoll>().colliderList.Add(this.Col());
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0001804A File Offset: 0x0001624A
	private void Awake()
	{
		if (this.IsEditor())
		{
			this.SetValues();
			return;
		}
		this.RegisterCollider();
		this.Col();
	}

	// Token: 0x0600038E RID: 910 RVA: 0x00018068 File Offset: 0x00016268
	private void RegisterCollider()
	{
		base.transform.parent.GetComponent<Bodypart>().RegisterCollider(this);
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00018080 File Offset: 0x00016280
	private bool IsEditor()
	{
		return Application.isEditor && !Application.isPlaying;
	}

	// Token: 0x06000390 RID: 912 RVA: 0x00018093 File Offset: 0x00016293
	private void OnDestroy()
	{
		if (!this.IsEditor())
		{
			return;
		}
		if (!this.RigCreator())
		{
			return;
		}
		this.RigCreator().RemoveCollider(this);
	}

	// Token: 0x06000391 RID: 913 RVA: 0x000180B8 File Offset: 0x000162B8
	internal CapsuleCollider Col()
	{
		if (!this.col)
		{
			this.col = base.GetComponent<CapsuleCollider>();
		}
		return this.col;
	}

	// Token: 0x06000392 RID: 914 RVA: 0x000180D9 File Offset: 0x000162D9
	private RigCreator RigCreator()
	{
		if (!this.rigCreator)
		{
			this.rigCreator = base.GetComponentInParent<RigCreator>();
		}
		return this.rigCreator;
	}

	// Token: 0x06000393 RID: 915 RVA: 0x000180FA File Offset: 0x000162FA
	private void Update()
	{
		if (this.IsEditor())
		{
			this.CheckEditorDataChanged();
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0001810C File Offset: 0x0001630C
	private void CheckEditorDataChanged()
	{
		if (this.position != base.transform.localPosition || this.rotation != base.transform.localRotation || this.scale != base.transform.localScale || this.height != this.Col().height || this.radius != this.Col().radius)
		{
			this.RigCreator().ColliderChanged(this, base.transform.localPosition, base.transform.localRotation, base.transform.localScale, this.height, this.radius);
			this.SetValues();
		}
	}

	// Token: 0x06000395 RID: 917 RVA: 0x000181C8 File Offset: 0x000163C8
	private void SetValues()
	{
		this.position = base.transform.localPosition;
		this.rotation = base.transform.localRotation;
		this.scale = base.transform.localScale;
		this.height = this.Col().height;
		this.radius = this.Col().radius;
	}

	// Token: 0x040003ED RID: 1005
	internal CapsuleCollider col;

	// Token: 0x040003EE RID: 1006
	internal Vector3 position;

	// Token: 0x040003EF RID: 1007
	internal Quaternion rotation;

	// Token: 0x040003F0 RID: 1008
	internal Vector3 scale;

	// Token: 0x040003F1 RID: 1009
	internal float height;

	// Token: 0x040003F2 RID: 1010
	internal float radius;

	// Token: 0x040003F3 RID: 1011
	public bool disableOnStart;

	// Token: 0x040003F4 RID: 1012
	internal RigCreator rigCreator;
}
