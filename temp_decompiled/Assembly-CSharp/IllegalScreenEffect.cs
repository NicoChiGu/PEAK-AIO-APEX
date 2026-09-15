using System;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class IllegalScreenEffect : MonoBehaviour
{
	// Token: 0x06001199 RID: 4505 RVA: 0x000589F0 File Offset: 0x00056BF0
	private void Start()
	{
		this.rend = base.GetComponent<MeshRenderer>();
		this.rend.enabled = false;
		this.mat = this.rend.material;
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x00058A1C File Offset: 0x00056C1C
	private void Update()
	{
		if (!this.character)
		{
			if (Character.localCharacter)
			{
				this.character = Character.localCharacter;
				Character character = this.character;
				character.illegalStatusAction = (Action<string, float>)Delegate.Combine(character.illegalStatusAction, new Action<string, float>(this.AddStatus));
			}
			return;
		}
		if (this.character.data.fullyPassedOut || this.character.data.dead)
		{
			this.activeForSeconds = 0f;
		}
		this.activeForSeconds -= Time.deltaTime;
		if (this.activeForSeconds > 0f)
		{
			this.rend.enabled = true;
			float num = Mathf.Clamp01(this.activeForSeconds / 3f);
			float @float = this.mat.GetFloat(this.shaderVarName);
			if (num > @float)
			{
				this.mat.SetFloat(this.shaderVarName, Mathf.Lerp(@float, num, Time.deltaTime));
			}
			else
			{
				this.mat.SetFloat(this.shaderVarName, num);
			}
			this.character.data.isBlind = true;
			return;
		}
		this.rend.enabled = false;
		this.character.data.isBlind = false;
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x00058B57 File Offset: 0x00056D57
	private void AddStatus(string status, float duration)
	{
		if (status.ToUpper() != this.statusName.ToUpper())
		{
			return;
		}
		this.activeForSeconds = duration;
	}

	// Token: 0x04001017 RID: 4119
	public string statusName = "BLIND";

	// Token: 0x04001018 RID: 4120
	public string shaderVarName = "_Alpha";

	// Token: 0x04001019 RID: 4121
	private float activeForSeconds;

	// Token: 0x0400101A RID: 4122
	private Character character;

	// Token: 0x0400101B RID: 4123
	private MeshRenderer rend;

	// Token: 0x0400101C RID: 4124
	private Material mat;
}
