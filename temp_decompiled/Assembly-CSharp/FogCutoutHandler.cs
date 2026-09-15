using System;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class FogCutoutHandler : MonoBehaviour
{
	// Token: 0x060006C2 RID: 1730 RVA: 0x00026A3A File Offset: 0x00024C3A
	public void debugCurrentCutoutZone()
	{
		this.setFogCutoutZone(this.index);
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00026A48 File Offset: 0x00024C48
	private void OnEnable()
	{
		this.setFogCutoutZone(0);
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00026A54 File Offset: 0x00024C54
	private void Update()
	{
		if (Character.localCharacter && Character.localCharacter.Center.z > this.currentCutoutZone.transform.position.z + this.currentCutoutZone.transitionPoint && this.index < this.cutoutZones.Length)
		{
			this.setFogCutoutZone(this.index);
			this.index++;
		}
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x00026ACC File Offset: 0x00024CCC
	public void setFogCutoutZone(int zone)
	{
		FogCutoutHandler.<>c__DisplayClass7_0 CS$<>8__locals1 = new FogCutoutHandler.<>c__DisplayClass7_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.zone = zone;
		base.StartCoroutine(CS$<>8__locals1.<setFogCutoutZone>g__changeZoneRoutine|0());
		this.currentCutoutZone = this.cutoutZones[CS$<>8__locals1.zone];
	}

	// Token: 0x040006D4 RID: 1748
	public FogCutoutZone[] cutoutZones;

	// Token: 0x040006D5 RID: 1749
	private FogCutoutZone currentCutoutZone;

	// Token: 0x040006D6 RID: 1750
	public int index;

	// Token: 0x040006D7 RID: 1751
	public float fadeTime = 1f;
}
