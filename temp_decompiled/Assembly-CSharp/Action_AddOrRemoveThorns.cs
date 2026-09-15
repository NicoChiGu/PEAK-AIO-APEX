using System;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class Action_AddOrRemoveThorns : ItemAction
{
	// Token: 0x06000801 RID: 2049 RVA: 0x0002CE74 File Offset: 0x0002B074
	public override void RunAction()
	{
		int i = this.thornCount;
		if (i > 0)
		{
			while (i > 0)
			{
				if (this.specificBodyPart)
				{
					Vector3 vector = Vector3.Lerp(this.minOffset, this.maxOffset, Random.Range(0f, 1f));
					Transform transform = base.character.GetBodypart(this.location).transform;
					Vector3 vector2 = transform.position + transform.TransformVector(vector);
					base.character.refs.afflictions.AddThorn(vector2);
				}
				else
				{
					base.character.refs.afflictions.AddThorn(999);
				}
				i--;
			}
			return;
		}
		while (i < 0)
		{
			base.character.refs.afflictions.RemoveRandomThornLinq();
			i++;
		}
	}

	// Token: 0x040007CA RID: 1994
	public int thornCount;

	// Token: 0x040007CB RID: 1995
	public bool specificBodyPart;

	// Token: 0x040007CC RID: 1996
	public BodypartType location;

	// Token: 0x040007CD RID: 1997
	public Vector3 minOffset;

	// Token: 0x040007CE RID: 1998
	public Vector3 maxOffset;
}
