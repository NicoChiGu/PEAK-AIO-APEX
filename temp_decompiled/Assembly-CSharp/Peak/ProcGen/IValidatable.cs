using System;

namespace Peak.ProcGen
{
	// Token: 0x020003D5 RID: 981
	public interface IValidatable
	{
		// Token: 0x0600192C RID: 6444
		ValidationState DoValidation();

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x0600192D RID: 6445
		ValidationState ValidationState { get; }
	}
}
