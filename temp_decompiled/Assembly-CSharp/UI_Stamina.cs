using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000362 RID: 866
public class UI_Stamina : MonoBehaviour
{
	// Token: 0x06001619 RID: 5657 RVA: 0x0007222F File Offset: 0x0007042F
	private void Update()
	{
		this.fill.fillAmount = Character.localCharacter.data.currentStamina;
	}

	// Token: 0x04001504 RID: 5380
	public ProceduralImage fill;
}
