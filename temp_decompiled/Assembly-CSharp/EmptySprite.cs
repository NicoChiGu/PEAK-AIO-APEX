using System;
using UnityEngine;

// Token: 0x020002CD RID: 717
public static class EmptySprite
{
	// Token: 0x0600135C RID: 4956 RVA: 0x00062711 File Offset: 0x00060911
	public static Sprite Get()
	{
		if (EmptySprite.instance == null)
		{
			EmptySprite.instance = Resources.Load<Sprite>("procedural_ui_image_default_sprite");
		}
		return EmptySprite.instance;
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x00062734 File Offset: 0x00060934
	public static bool IsEmptySprite(Sprite s)
	{
		return EmptySprite.Get() == s;
	}

	// Token: 0x04001205 RID: 4613
	private static Sprite instance;
}
