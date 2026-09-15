using System;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003B6 RID: 950
	public class LimitFrameRate : MonoBehaviour
	{
		// Token: 0x0600186F RID: 6255 RVA: 0x0007C398 File Offset: 0x0007A598
		private void Start()
		{
			Application.targetFrameRate = this.FrameRate;
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x0007C3A5 File Offset: 0x0007A5A5
		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			Application.targetFrameRate = this.FrameRate;
		}

		// Token: 0x0400169A RID: 5786
		public int FrameRate = 144;
	}
}
