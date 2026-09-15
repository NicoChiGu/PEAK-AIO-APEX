using System;
using UnityEngine;

// Token: 0x02000325 RID: 805
public class SetGlobalVariables : MonoBehaviour
{
	// Token: 0x060014BE RID: 5310 RVA: 0x000699E4 File Offset: 0x00067BE4
	private void Start()
	{
		foreach (StringAndFloat stringAndFloat in this.globalVariables)
		{
			PlayerPrefs.SetFloat(stringAndFloat.name, stringAndFloat.value);
		}
	}

	// Token: 0x04001336 RID: 4918
	public StringAndFloat[] globalVariables;
}
