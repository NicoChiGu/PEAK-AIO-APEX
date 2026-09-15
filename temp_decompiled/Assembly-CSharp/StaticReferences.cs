using System;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

// Token: 0x020001B0 RID: 432
[CreateAssetMenu(fileName = "StaticReferences", menuName = "Peak/StaticReferences")]
public class StaticReferences : SingletonAsset<StaticReferences>
{
	// Token: 0x04000B8F RID: 2959
	public AudioMixerGroup masterMixerGroup;
}
