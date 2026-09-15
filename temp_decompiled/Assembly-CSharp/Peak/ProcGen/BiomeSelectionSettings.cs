using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peak.ProcGen
{
	// Token: 0x020003D3 RID: 979
	[CreateAssetMenu(menuName = "Peak/BiomeSelector")]
	public class BiomeSelectionSettings : ScriptableObject
	{
		// Token: 0x040016C0 RID: 5824
		public List<BiomeSelection> Settings = new List<BiomeSelection>();
	}
}
