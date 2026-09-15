using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001C0 RID: 448
public class EmoteWheel : UIWheel
{
	// Token: 0x06000DCD RID: 3533 RVA: 0x00044FB7 File Offset: 0x000431B7
	private void Start()
	{
		this.nextButton.onClick.AddListener(new UnityAction(this.TabNext));
		this.prevButton.onClick.AddListener(new UnityAction(this.TabPrev));
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x00044FF1 File Offset: 0x000431F1
	protected override void Update()
	{
		if (Character.localCharacter.input.selectSlotBackwardWasPressed)
		{
			this.Tab(-1);
		}
		else if (Character.localCharacter.input.selectSlotForwardWasPressed)
		{
			this.Tab(1);
		}
		base.Update();
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x0004502B File Offset: 0x0004322B
	public void OnEnable()
	{
		this.InitWheel();
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x00045033 File Offset: 0x00043233
	public void OnDisable()
	{
		this.Choose();
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x0004503C File Offset: 0x0004323C
	public void InitWheel()
	{
		this.chosenEmoteData = null;
		for (int i = 0; i < this.slices.Length; i++)
		{
			int num = i + 8 * this.page;
			this.slices[i].Init(this.data[num], this);
		}
		this.selectedEmoteName.text = "";
		this.nextButton.gameObject.SetActive(this.page + 1 < this.pages);
		this.prevButton.gameObject.SetActive(this.page > 0);
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x000450CD File Offset: 0x000432CD
	private void TabNext()
	{
		this.Tab(1);
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x000450D6 File Offset: 0x000432D6
	private void TabPrev()
	{
		this.Tab(-1);
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x000450DF File Offset: 0x000432DF
	private void Tab(int index)
	{
		this.page += index;
		this.page = Mathf.Clamp(this.page, 0, this.pages - 1);
		this.InitWheel();
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0004510F File Offset: 0x0004330F
	public void Choose()
	{
		if (this.chosenEmoteData != null)
		{
			Character.localCharacter.refs.animations.PlayEmote(this.chosenEmoteData.anim);
		}
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0004513E File Offset: 0x0004333E
	public void Hover(EmoteWheelData emoteWheelData)
	{
		this.selectedEmoteName.text = LocalizedText.GetText(emoteWheelData.emoteName, true);
		this.chosenEmoteData = emoteWheelData;
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x0004515E File Offset: 0x0004335E
	public void Dehover(EmoteWheelData emoteWheelData)
	{
		if (this.chosenEmoteData == emoteWheelData)
		{
			this.selectedEmoteName.text = "";
			this.chosenEmoteData = null;
		}
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00045188 File Offset: 0x00043388
	protected override void TestSelectSliceGamepad(Vector2 gamepadVector)
	{
		float num = 0f;
		EmoteWheelSlice emoteWheelSlice = null;
		if (gamepadVector.sqrMagnitude >= 0.5f)
		{
			for (int i = 0; i < this.slices.Length; i++)
			{
				float num2 = Vector3.Angle(gamepadVector, this.slices[i].GetUpVector());
				if (emoteWheelSlice == null || num2 < num)
				{
					emoteWheelSlice = this.slices[i];
					num = num2;
				}
			}
		}
		if (emoteWheelSlice != null)
		{
			EventSystem.current.SetSelectedGameObject(emoteWheelSlice.button.gameObject);
			emoteWheelSlice.Hover();
			return;
		}
		EventSystem.current.SetSelectedGameObject(null);
		this.Dehover(this.chosenEmoteData);
	}

	// Token: 0x04000BDF RID: 3039
	public EmoteWheelSlice[] slices;

	// Token: 0x04000BE0 RID: 3040
	public EmoteWheelData[] data;

	// Token: 0x04000BE1 RID: 3041
	public TextMeshProUGUI selectedEmoteName;

	// Token: 0x04000BE2 RID: 3042
	private EmoteWheelData chosenEmoteData;

	// Token: 0x04000BE3 RID: 3043
	public Button nextButton;

	// Token: 0x04000BE4 RID: 3044
	public Button prevButton;

	// Token: 0x04000BE5 RID: 3045
	private int page;

	// Token: 0x04000BE6 RID: 3046
	public int pages = 2;
}
