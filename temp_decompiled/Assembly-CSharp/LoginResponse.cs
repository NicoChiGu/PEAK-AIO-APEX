using System;

// Token: 0x02000075 RID: 117
[Serializable]
public class LoginResponse
{
	// Token: 0x04000589 RID: 1417
	public bool VersionOkay;

	// Token: 0x0400058A RID: 1418
	public int HoursUntilLevel;

	// Token: 0x0400058B RID: 1419
	public int MinutesUntilLevel;

	// Token: 0x0400058C RID: 1420
	public int SecondsUntilLevel;

	// Token: 0x0400058D RID: 1421
	public int LevelIndex;

	// Token: 0x0400058E RID: 1422
	public string Message;
}
