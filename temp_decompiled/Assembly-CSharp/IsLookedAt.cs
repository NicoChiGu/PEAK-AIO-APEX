using System;
using UnityEngine;

// Token: 0x02000276 RID: 630
public class IsLookedAt : MonoBehaviour
{
	// Token: 0x060011A3 RID: 4515 RVA: 0x00058C44 File Offset: 0x00056E44
	private void Start()
	{
		if (this.characterInteractible.character == Character.localCharacter)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.index = GUIManager.instance.playerNames.Init(this.characterInteractible);
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x00058C90 File Offset: 0x00056E90
	private void Update()
	{
		bool flag = false;
		float num = Vector3.Distance(MainCamera.instance.transform.position, base.transform.position);
		float num2 = Vector3.Angle(MainCamera.instance.transform.forward, base.transform.position - MainCamera.instance.transform.position);
		if (num < this.visibleDistance && num2 < this.visibleAngle + (this.visibleDistance - num) / this.visibleDistance * this.angleDistRatio)
		{
			flag = true;
		}
		if (this.mouth.character.data.isBlind)
		{
			flag = false;
		}
		GUIManager.instance.playerNames.UpdateName(this.index, this.playerNamePos.position, flag, this.mouth.amplitudeIndex);
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x00058D63 File Offset: 0x00056F63
	private void OnDisable()
	{
		GUIManager.instance.playerNames.DisableName(this.index);
	}

	// Token: 0x0400101E RID: 4126
	public AnimatedMouth mouth;

	// Token: 0x0400101F RID: 4127
	public CharacterInteractible characterInteractible;

	// Token: 0x04001020 RID: 4128
	public float visibleDistance = 8f;

	// Token: 0x04001021 RID: 4129
	public float visibleAngle = 45f;

	// Token: 0x04001022 RID: 4130
	public float angleDistRatio = 45f;

	// Token: 0x04001023 RID: 4131
	public Transform playerNamePos;

	// Token: 0x04001024 RID: 4132
	private int index;
}
