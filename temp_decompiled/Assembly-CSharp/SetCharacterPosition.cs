using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000324 RID: 804
public class SetCharacterPosition : MonoBehaviour
{
	// Token: 0x060014BA RID: 5306 RVA: 0x000699B0 File Offset: 0x00067BB0
	private void Go()
	{
		this.characterPrefab.transform.position = base.transform.position;
		PExt.SaveObj(this.characterPrefab);
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x000699D8 File Offset: 0x00067BD8
	private void Start()
	{
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x000699DA File Offset: 0x00067BDA
	private void Update()
	{
	}

	// Token: 0x04001335 RID: 4917
	public GameObject characterPrefab;
}
