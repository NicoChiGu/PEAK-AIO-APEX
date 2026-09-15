using System;
using UnityEngine;

// Token: 0x02000352 RID: 850
public class TombCheck : MonoBehaviour
{
	// Token: 0x060015CC RID: 5580 RVA: 0x000705DE File Offset: 0x0006E7DE
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x000705EC File Offset: 0x0006E7EC
	private void Update()
	{
		if (!this.character)
		{
			this.character = Character.localCharacter;
		}
		if (this.character && this.character.refs.animations && this.character.refs.animations.ambienceAudio)
		{
			if (this.character.refs.animations.ambienceAudio.inTomb)
			{
				this.anim.SetBool("Tomb", true);
				return;
			}
			this.anim.SetBool("Tomb", false);
		}
	}

	// Token: 0x040014A7 RID: 5287
	private Character character;

	// Token: 0x040014A8 RID: 5288
	private Animator anim;
}
