using System;
using UnityEngine;

// Token: 0x02000039 RID: 57
[ExecuteInEditMode]
public class RigCreatorRigidbody : MonoBehaviour
{
	// Token: 0x0600039F RID: 927 RVA: 0x00018393 File Offset: 0x00016593
	private void Awake()
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			Object.Destroy(this);
			return;
		}
		this.SetValues();
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x000183B0 File Offset: 0x000165B0
	private Rigidbody Rig()
	{
		if (!this.rig)
		{
			this.rig = base.GetComponentInParent<Rigidbody>();
		}
		return this.rig;
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x000183D1 File Offset: 0x000165D1
	private RigCreator RigCreator()
	{
		if (!this.rigCreator)
		{
			this.rigCreator = base.GetComponentInParent<RigCreator>();
		}
		return this.rigCreator;
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x000183F2 File Offset: 0x000165F2
	private void Update()
	{
		if (this.mass != this.Rig().mass)
		{
			this.RigCreator().RigidbodyChanged(this, this.Rig().mass);
			this.SetValues();
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00018424 File Offset: 0x00016624
	private void SetValues()
	{
		this.mass = this.Rig().mass;
	}

	// Token: 0x040003F9 RID: 1017
	internal float mass;

	// Token: 0x040003FA RID: 1018
	internal Rigidbody rig;

	// Token: 0x040003FB RID: 1019
	internal RigCreator rigCreator;
}
