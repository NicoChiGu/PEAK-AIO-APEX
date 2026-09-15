using System;

// Token: 0x02000326 RID: 806
[Serializable]
public class StringAndFloat
{
	// Token: 0x060014C0 RID: 5312 RVA: 0x00069A23 File Offset: 0x00067C23
	public StringAndFloat(string name, float value)
	{
		this.name = name;
		this.value = value;
	}

	// Token: 0x04001337 RID: 4919
	public string name;

	// Token: 0x04001338 RID: 4920
	public float value;
}
