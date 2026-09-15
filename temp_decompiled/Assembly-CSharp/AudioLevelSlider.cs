using System;
using Peak.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200005E RID: 94
public class AudioLevelSlider : MonoBehaviour
{
	// Token: 0x17000059 RID: 89
	// (get) Token: 0x0600048F RID: 1167 RVA: 0x0001B929 File Offset: 0x00019B29
	public SelectableSlider ParentContainer
	{
		get
		{
			return this._container;
		}
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000490 RID: 1168 RVA: 0x0001B931 File Offset: 0x00019B31
	public bool isLocal
	{
		get
		{
			return this.player.IsLocal;
		}
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x0001B93E File Offset: 0x00019B3E
	private void Update()
	{
		Player player = this.player;
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0001B947 File Offset: 0x00019B47
	private void Awake()
	{
		this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0001B968 File Offset: 0x00019B68
	public void Init(Player newPlayer)
	{
		this.player = newPlayer;
		Player player = this.player;
		bool flag = this.player != null && !PhotonNetwork.OfflineMode;
		base.gameObject.SetActive(flag);
		if (flag)
		{
			this._container = base.GetComponent<SelectableSlider>();
			this._container.Init();
			bool isLocal = this.player.IsLocal;
			this.isLocalObject.SetActive(isLocal);
			this.isNotLocalObject.SetActive(!isLocal);
			this.crown.SetActive(this.player.IsMasterClient);
			this.playerName.text = this.player.NickName;
			this.playerName.color = (this.player.IsMasterClient ? this.playerColorGold : this.playerColorDefault);
			this.slider.SetValueWithoutNotify(AudioLevels.GetPlayerLevel(this.player.UserId));
		}
		this.bar.color = this.barGradient.Evaluate(this.slider.value);
		this.percent.text = Mathf.RoundToInt(this.slider.value * 200f).ToString() + "%";
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0001BAA8 File Offset: 0x00019CA8
	private void OnSliderChanged(float newValue)
	{
		if (this.player != null)
		{
			AudioLevels.SetPlayerLevel(this.player.UserId, newValue);
			this.icon.sprite = ((newValue == 0f) ? this.mutedAudioSprite : this.audioSprites[Mathf.FloorToInt(newValue * 2.99f)]);
			this.bar.color = this.barGradient.Evaluate(newValue);
			EventSystem.current.SetSelectedGameObject(null);
			this.percent.text = Mathf.RoundToInt(newValue * 200f).ToString() + "%";
		}
	}

	// Token: 0x0400050A RID: 1290
	private SelectableSlider _container;

	// Token: 0x0400050B RID: 1291
	public TextMeshProUGUI playerName;

	// Token: 0x0400050C RID: 1292
	public TextMeshProUGUI percent;

	// Token: 0x0400050D RID: 1293
	public Player player;

	// Token: 0x0400050E RID: 1294
	public Slider slider;

	// Token: 0x0400050F RID: 1295
	public Image bar;

	// Token: 0x04000510 RID: 1296
	public Gradient barGradient;

	// Token: 0x04000511 RID: 1297
	public Sprite[] audioSprites;

	// Token: 0x04000512 RID: 1298
	public Sprite mutedAudioSprite;

	// Token: 0x04000513 RID: 1299
	public Image icon;

	// Token: 0x04000514 RID: 1300
	public GameObject isLocalObject;

	// Token: 0x04000515 RID: 1301
	public GameObject isNotLocalObject;

	// Token: 0x04000516 RID: 1302
	public GameObject crown;

	// Token: 0x04000517 RID: 1303
	public Color playerColorGold;

	// Token: 0x04000518 RID: 1304
	public Color playerColorDefault;
}
