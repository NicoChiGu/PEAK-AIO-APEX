using System;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class BadgeUnlocker : MonoBehaviour
{
	// Token: 0x060004B6 RID: 1206 RVA: 0x0001C190 File Offset: 0x0001A390
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0001C1A0 File Offset: 0x0001A3A0
	public void Update()
	{
		if (this.useTestBadge)
		{
			int num = GUIManager.instance.mainBadgeManager.badgeData.Length;
			Texture2D texture2D = new Texture2D(num, 1);
			texture2D.filterMode = 0;
			for (int i = 0; i < num; i++)
			{
				texture2D.SetPixel(i, 1, Color.black);
			}
			texture2D.SetPixel(this.testBadge, 1, Color.white);
			texture2D.Apply();
			this.badgeSashRenderer.materials[0].SetTexture("BadgeUnlockTexture", texture2D);
		}
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0001C220 File Offset: 0x0001A420
	public static void SetBadges(Character refCharacter, Renderer sashRenderer)
	{
		int num = refCharacter.data.badgeStatus.Length;
		Texture2D texture2D = new Texture2D(num, 1);
		texture2D.filterMode = 0;
		for (int i = 0; i < num; i++)
		{
			if (refCharacter.data.badgeStatus[i])
			{
				if (GUIManager.instance.mainBadgeManager.badgeData[i] != null)
				{
					texture2D.SetPixel(GUIManager.instance.mainBadgeManager.badgeData[i].visualID, 1, Color.white);
				}
				else
				{
					texture2D.SetPixel(i, 1, Color.white);
				}
			}
			else if (GUIManager.instance.mainBadgeManager.badgeData[i] != null)
			{
				texture2D.SetPixel(GUIManager.instance.mainBadgeManager.badgeData[i].visualID, 1, Color.black);
			}
			else
			{
				texture2D.SetPixel(i, 1, Color.black);
			}
		}
		texture2D.Apply();
		if (sashRenderer == null)
		{
			return;
		}
		sashRenderer.materials[0].SetTexture("BadgeUnlockTexture", texture2D);
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001C324 File Offset: 0x0001A524
	public void BadgeUnlockVisual()
	{
		if (!this.character)
		{
			this.character = base.GetComponent<Character>();
		}
		BadgeUnlocker.SetBadges(this.character, this.badgeSashRenderer);
	}

	// Token: 0x04000522 RID: 1314
	public int testBadge;

	// Token: 0x04000523 RID: 1315
	public bool useTestBadge;

	// Token: 0x04000524 RID: 1316
	private Character character;

	// Token: 0x04000525 RID: 1317
	public Renderer badgeSashRenderer;
}
