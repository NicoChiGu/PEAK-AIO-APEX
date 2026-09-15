using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000F6 RID: 246
public class Action_ShowBinocularOverlay : ItemAction
{
	// Token: 0x06000855 RID: 2133 RVA: 0x0002DCBB File Offset: 0x0002BEBB
	private void Update()
	{
		if (this.binocularsActive && !this.isProp)
		{
			this.TestLookAtSun();
		}
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0002DCD4 File Offset: 0x0002BED4
	public override void RunAction()
	{
		this.binocularsActive = !this.binocularsActive;
		if (!this.isProp)
		{
			this.featureManager.setFeatureActive(this.binocularsActive);
			MainCamera.instance.SetCameraOverride(this.binocularsActive ? this.cameraOverride : null);
		}
		this.item.photonView.RPC("ToggleUseRPC", 0, new object[] { this.binocularsActive });
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0002DD4E File Offset: 0x0002BF4E
	[PunRPC]
	private void ToggleUseRPC(bool open)
	{
		this.item.defaultPos = new Vector3(this.item.defaultPos.x, (float)(open ? 1 : 0), this.item.defaultPos.z);
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x0002DD88 File Offset: 0x0002BF88
	private void TestLookAtSun()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (Singleton<MapHandler>.Instance.GetCurrentBiome() != Biome.BiomeType.Mesa)
		{
			return;
		}
		if (DayNightManager.instance.sun.intensity < 5f)
		{
			return;
		}
		Transform transform = DayNightManager.instance.sun.transform;
		if (Vector3.Angle(MainCamera.instance.transform.forward, -transform.forward) > 10f)
		{
			return;
		}
		RaycastHit raycastHit = HelperFunctions.LineCheck(Character.localCharacter.Center + transform.forward * -1000f, Character.localCharacter.Center, HelperFunctions.LayerType.AllPhysical, 0f, 1);
		if (raycastHit.transform == null || raycastHit.transform.root == Character.localCharacter.transform.root)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AstronomyBadge);
		}
	}

	// Token: 0x04000800 RID: 2048
	public bool binocularsActive;

	// Token: 0x04000801 RID: 2049
	public CameraOverride cameraOverride;

	// Token: 0x04000802 RID: 2050
	public ItemRenderFeatureManager featureManager;

	// Token: 0x04000803 RID: 2051
	public bool isProp;
}
