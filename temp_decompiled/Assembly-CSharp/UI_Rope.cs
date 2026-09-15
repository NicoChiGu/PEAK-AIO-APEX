using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000361 RID: 865
public class UI_Rope : MonoBehaviour
{
	// Token: 0x06001615 RID: 5653 RVA: 0x00071F85 File Offset: 0x00070185
	private void OnEnable()
	{
		this.segments = 1;
		this.ropeLength = 1f;
	}

	// Token: 0x06001616 RID: 5654 RVA: 0x00071F9C File Offset: 0x0007019C
	private void Update()
	{
		this.ropeLength = Mathf.Lerp(this.ropeLength, (float)this.segments, Time.deltaTime * 5f);
		float num = (Mathf.Max(this.ropeLength, 0f) + this.ropeLengthOffset) * this.ropeLengthMult;
		this.rope.sizeDelta = new Vector2(num, this.rope.sizeDelta.y);
		for (int i = 0; i < this.ropeImages.Length; i++)
		{
			this.ropeImages[i].color = new Color(this.ropeImages[i].color.r, this.ropeImages[i].color.g, this.ropeImages[i].color.b, num * this.ropeLengthAlphaMult - Mathf.Floor(num * this.ropeLengthAlphaMult) + 0.01f);
		}
		bool flag = false;
		for (int j = 0; j < 3; j++)
		{
			this.ropeImages[j].fillAmount = this.ropeSpinA - (this.ropeLength * this.ropeSpinB / this.maxRopeLength - (float)j);
			if (this.ropeImages[j].fillAmount > 0f && !flag)
			{
				flag = true;
				this.ropeEnd.position = this.ropeImages[j].transform.position;
				this.ropeEnd.eulerAngles = this.ropeImages[j].transform.eulerAngles + new Vector3(0f, 0f, this.ropeImages[j].fillAmount * 360f + this.ropeEndOffset);
				this.ropeEndImage.color = new Color(this.ropeImages[j].color.r, this.ropeImages[j].color.g, this.ropeImages[j].color.b, 1f);
			}
		}
		string text = "m";
		int num2 = Mathf.RoundToInt(this.ropeLength * 100f * 0.25f);
		this.ropeLengthText.text = (num2 / 100).ToString() + "." + (num2 % 100).ToString() + text;
	}

	// Token: 0x06001617 RID: 5655 RVA: 0x000721E7 File Offset: 0x000703E7
	public void UpdateRope(int newSegments)
	{
		this.segments = newSegments;
	}

	// Token: 0x040014F4 RID: 5364
	public RectTransform rope;

	// Token: 0x040014F5 RID: 5365
	public float maxRopeLength = 40f;

	// Token: 0x040014F6 RID: 5366
	public float ropeLength = 40f;

	// Token: 0x040014F7 RID: 5367
	public float ropeLengthOffset;

	// Token: 0x040014F8 RID: 5368
	public float ropeLengthMult = 20f;

	// Token: 0x040014F9 RID: 5369
	public float ropeLengthAlphaMult;

	// Token: 0x040014FA RID: 5370
	public Image[] ropeImages;

	// Token: 0x040014FB RID: 5371
	private const string M = "m";

	// Token: 0x040014FC RID: 5372
	private const string FT = "ft";

	// Token: 0x040014FD RID: 5373
	public TextMeshProUGUI ropeLengthText;

	// Token: 0x040014FE RID: 5374
	private int segments;

	// Token: 0x040014FF RID: 5375
	public Transform ropeEnd;

	// Token: 0x04001500 RID: 5376
	public Image ropeEndImage;

	// Token: 0x04001501 RID: 5377
	public float ropeSpinA = 2f;

	// Token: 0x04001502 RID: 5378
	public float ropeSpinB = 3f;

	// Token: 0x04001503 RID: 5379
	public float ropeEndOffset;
}
