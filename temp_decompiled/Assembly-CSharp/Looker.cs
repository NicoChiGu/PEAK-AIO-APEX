using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000297 RID: 663
public class Looker : MonoBehaviour
{
	// Token: 0x06001241 RID: 4673 RVA: 0x0005C5D8 File Offset: 0x0005A7D8
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.pivot = base.transform.Find("Pivot");
		this.SetRandomSwitch();
		this.view = base.GetComponent<PhotonView>();
		if (GameHandler.Instance.SettingsHandler.GetSetting<LookerSetting>().Value == null)
		{
			this.guy.SetActive(false);
		}
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x0005C63C File Offset: 0x0005A83C
	private void ToggleLookers()
	{
		Looker[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Looker>();
		int num = Random.Range(0, componentsInChildren.Length);
		if (Random.value < 0.95f)
		{
			num = -1;
		}
		foreach (Looker looker in componentsInChildren)
		{
			if (looker.transform.GetSiblingIndex() != num)
			{
				looker.view.RPC("RPCA_DisableLooker", 3, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x0005C6AD File Offset: 0x0005A8AD
	[PunRPC]
	public void RPCA_DisableLooker()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x0005C6BC File Offset: 0x0005A8BC
	private void SetRandomSwitch()
	{
		if (Random.Range(0f, 1f) < 0.1f)
		{
			this.untilSwitch = Random.Range(5f, 20f);
			return;
		}
		this.untilSwitch = Random.Range(1f, 5f);
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x0005C70C File Offset: 0x0005A90C
	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.hasChecked)
		{
			if (PhotonNetwork.IsMasterClient && base.transform.GetSiblingIndex() == 0)
			{
				this.ToggleLookers();
			}
			this.hasChecked = true;
		}
		Transform transform = MainCamera.instance.transform;
		this.pivot.LookAt(transform.position);
		if (this.neverReturn)
		{
			this.selfDestructCounter -= Time.deltaTime;
			if (this.selfDestructCounter <= 0f)
			{
				Object.Destroy(this.pivot.gameObject);
				Object.Destroy(this);
			}
			return;
		}
		this.untilSwitch -= Time.deltaTime;
		float num = Vector3.Distance(transform.position, this.pivot.position);
		bool flag = Vector3.Dot(transform.forward, (this.pivot.position - transform.position).normalized) > 0.8f && num < 40f;
		if (this.isActive && flag)
		{
			this.untilSwitch -= Time.deltaTime * 5f;
			this.hasLookedAtMeFor += Time.deltaTime;
		}
		bool flag2 = this.hasLookedAtMeFor > 3f;
		bool flag3 = num < 5f;
		if (this.view.IsMine && this.untilSwitch <= 0f)
		{
			this.isActive = !this.isActive;
			this.view.RPC("RPCA_Switch", 0, new object[] { this.isActive });
		}
		if (flag2 || flag3)
		{
			this.view.RPC("RPCA_CodeRed", 3, Array.Empty<object>());
		}
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x0005C8C0 File Offset: 0x0005AAC0
	[PunRPC]
	private void RPCA_Switch(bool switchTo)
	{
		if (this.neverReturn)
		{
			return;
		}
		this.anim.SetBool("IsActive", switchTo);
		this.SetRandomSwitch();
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x0005C8E2 File Offset: 0x0005AAE2
	[PunRPC]
	private void RPCA_CodeRed()
	{
		this.anim.SetBool("IsActive", false);
		this.neverReturn = true;
	}

	// Token: 0x040010C0 RID: 4288
	public GameObject guy;

	// Token: 0x040010C1 RID: 4289
	private Animator anim;

	// Token: 0x040010C2 RID: 4290
	private Transform pivot;

	// Token: 0x040010C3 RID: 4291
	private float untilSwitch;

	// Token: 0x040010C4 RID: 4292
	private bool neverReturn;

	// Token: 0x040010C5 RID: 4293
	private float selfDestructCounter = 3f;

	// Token: 0x040010C6 RID: 4294
	private bool isActive;

	// Token: 0x040010C7 RID: 4295
	private PhotonView view;

	// Token: 0x040010C8 RID: 4296
	private bool hasChecked;

	// Token: 0x040010C9 RID: 4297
	private float hasLookedAtMeFor;
}
