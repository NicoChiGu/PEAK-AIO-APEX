using System;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class Action_Torch : OnItemStateChangedAction
{
	// Token: 0x06000864 RID: 2148 RVA: 0x0002DFAC File Offset: 0x0002C1AC
	public override void RunAction(ItemState state)
	{
		if (state == ItemState.Held)
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				ParticleSystem.MainModule main = this.particles[i].main;
				Debug.Log("char is null? " + (base.character == null).ToString());
				main.customSimulationSpace = base.character.refs.animationPositionTransform;
			}
		}
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0002E018 File Offset: 0x0002C218
	private void Update()
	{
		this.torchLight.intensity = this.lightCurve.Evaluate(Time.time * this.lightSpeed) * this.lightIntensity;
	}

	// Token: 0x04000807 RID: 2055
	public ParticleSystem[] particles;

	// Token: 0x04000808 RID: 2056
	public Light torchLight;

	// Token: 0x04000809 RID: 2057
	public AnimationCurve lightCurve;

	// Token: 0x0400080A RID: 2058
	public float lightSpeed = 1f;

	// Token: 0x0400080B RID: 2059
	public float lightIntensity = 10f;
}
