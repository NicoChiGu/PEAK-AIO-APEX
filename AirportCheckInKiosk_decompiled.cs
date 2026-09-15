using System;
using System.Collections;
using Peak.Network;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000A2 RID: 162
public class AirportCheckInKiosk : MonoBehaviourPun, IInteractibleConstant, IInteractible
{
	// Token: 0x0600060D RID: 1549 RVA: 0x0002300A File Offset: 0x0002120A
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x0002300D File Offset: 0x0002120D
	public void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0002301C File Offset: 0x0002121C
	private void Start()
	{
		if (GameHandler.GetService<NextLevelService>().Data.IsSome)
		{
			Debug.Log(string.Format("seconds left until next map... {0}", GameHandler.GetService<NextLevelService>().Data.Value.SecondsLeft));
		}
		GameHandler.GetService<RichPresenceService>().SetState(RichPresenceState.Status_Airport);
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00023070 File Offset: 0x00021270
	public void Interact(Character interactor)
	{
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x06000611 RID: 1553 RVA: 0x00023072 File Offset: 0x00021272
	// (set) Token: 0x06000612 RID: 1554 RVA: 0x000230A0 File Offset: 0x000212A0
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = base.GetComponentsInChildren<MeshRenderer>();
				MonoBehaviour.print(this._mr.Length);
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x000230AC File Offset: 0x000212AC
	public void HoverEnter()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				if (this.meshRenderers[i] != null)
				{
					this.meshRenderers[i].SetPropertyBlock(this.mpb);
				}
			}
		}
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x0002310C File Offset: 0x0002130C
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].SetPropertyBlock(this.mpb);
			}
		}
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x0002315C File Offset: 0x0002135C
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x00023169 File Offset: 0x00021369
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x00023171 File Offset: 0x00021371
	public string GetInteractionText()
	{
		return LocalizedText.GetText("BOARDFLIGHT", true);
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x0002317E File Offset: 0x0002137E
	public string GetName()
	{
		return LocalizedText.GetText("GATEKIOSK", true);
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x0002318B File Offset: 0x0002138B
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.IsInteractible(interactor);
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x00023194 File Offset: 0x00021394
	public float GetInteractTime(Character interactor)
	{
		return this.interactTime;
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x0002319C File Offset: 0x0002139C
	public void Interact_CastFinished(Character interactor)
	{
		GUIManager.instance.boardingPass.Open();
		GUIManager.instance.boardingPass.kiosk = this;
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x000231BD File Offset: 0x000213BD
	public void StartGame(int ascent)
	{
		base.photonView.RPC("LoadIslandMaster", 2, new object[] { ascent });
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x000231DF File Offset: 0x000213DF
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x000231E1 File Offset: 0x000213E1
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x000231E4 File Offset: 0x000213E4
	[PunRPC]
	public void LoadIslandMaster(int ascent)
	{
		if (!NetCode.Session.IsHost)
		{
			return;
		}
		Debug.Log("Loading scene as master.");
		int nextLevelIndexOrFallback = GameHandler.GetService<NextLevelService>().NextLevelIndexOrFallback;
		string text = SingletonAsset<MapBaker>.Instance.GetLevel(nextLevelIndexOrFallback + NextLevelService.debugLevelIndexOffset);
		if (string.IsNullOrEmpty(text))
		{
			text = "WilIsland";
		}
		base.photonView.RPC("BeginIslandLoadRPC", 0, new object[] { text, ascent });
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x00023260 File Offset: 0x00021460
	[PunRPC]
	public void BeginIslandLoadRPC(string sceneName, int ascent)
	{
		MenuWindow.CloseAllWindows();
		GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
		Debug.Log("Begin scene load RPC: " + sceneName);
		Ascents.currentAscent = ascent;
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Plane, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(sceneName, true, true, 0f) });
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x06000621 RID: 1569 RVA: 0x000232B9 File Offset: 0x000214B9
	public bool holdOnFinish { get; }

	// Token: 0x0400062B RID: 1579
	public float interactTime;

	// Token: 0x0400062C RID: 1580
	private MaterialPropertyBlock mpb;

	// Token: 0x0400062D RID: 1581
	private MeshRenderer[] _mr;
}
