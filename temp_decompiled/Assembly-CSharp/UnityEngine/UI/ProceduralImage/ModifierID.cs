using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x02000376 RID: 886
	[AttributeUsage(4)]
	public class ModifierID : Attribute
	{
		// Token: 0x0600166C RID: 5740 RVA: 0x000741D6 File Offset: 0x000723D6
		public ModifierID(string name)
		{
			this.name = name;
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x0600166D RID: 5741 RVA: 0x000741E5 File Offset: 0x000723E5
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x04001553 RID: 5459
		private string name;
	}
}
