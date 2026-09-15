using System;

// Token: 0x02000373 RID: 883
public class WindAffectedStatusEmitter : StatusEmitter
{
	// Token: 0x06001665 RID: 5733 RVA: 0x000740BD File Offset: 0x000722BD
	private void FixedUpdate()
	{
		if (RootsWind.instance)
		{
			this.emitterDisabledByWind = RootsWind.instance.windZone.windActive;
			return;
		}
		this.emitterDisabledByWind = false;
	}
}
