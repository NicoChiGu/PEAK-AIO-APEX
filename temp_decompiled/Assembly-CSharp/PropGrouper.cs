using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Peak.ProcGen;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class PropGrouper : MonoBehaviour, IValidatable
{
	// Token: 0x06001373 RID: 4979 RVA: 0x00062A98 File Offset: 0x00060C98
	public void RunAll(bool updateLightmap = true)
	{
		PropGrouper.<>c__DisplayClass3_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (!this.Verify())
		{
			return;
		}
		this.ClearAll();
		LevelGenStep[] componentsInChildren = base.GetComponentsInChildren<LevelGenStep>();
		List<LevelGenStep> list = new List<LevelGenStep>();
		CS$<>8__locals1.late = new List<LevelGenStep>();
		CS$<>8__locals1.deferredSteps = new Dictionary<DeferredStepTiming, List<IDeferredStep>>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			PropGrouper.PropGrouperTiming propGrouperTiming = componentsInChildren[i].GetComponentInParent<PropGrouper>().timing;
			if (propGrouperTiming == PropGrouper.PropGrouperTiming.Early)
			{
				list.Add(componentsInChildren[i]);
			}
			else if (propGrouperTiming == PropGrouper.PropGrouperTiming.Late)
			{
				CS$<>8__locals1.late.Add(componentsInChildren[i]);
			}
		}
		foreach (LevelGenStep levelGenStep in list)
		{
			levelGenStep.Execute();
			if (levelGenStep.DeferredTiming != DeferredStepTiming.None)
			{
				this.<RunAll>g__AddToStepList|3_2(levelGenStep.DeferredTiming, levelGenStep.ConstructDeferred(levelGenStep), ref CS$<>8__locals1);
			}
		}
		this.<RunAll>g__ExecuteAndClearDeferredStepsFor|3_1(DeferredStepTiming.AfterCurrentGroupTiming, ref CS$<>8__locals1);
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x00062B90 File Offset: 0x00060D90
	public void Validate()
	{
		this.ValidationState = this.DoValidation();
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x00062B9E File Offset: 0x00060D9E
	public Color GetValidationColor()
	{
		return this.GetValidationColorImpl();
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x00062BA8 File Offset: 0x00060DA8
	public ValidationState DoValidation()
	{
		ValidationState validationState = ValidationState.Passed;
		foreach (IValidatable validatable in Enumerable.Where<IValidatable>(base.GetComponentsInChildren<IValidatable>(), (IValidatable v) => !(v is PropGrouper)))
		{
			ValidationState validationState2 = validatable.DoValidation();
			if ((validationState2 == ValidationState.Unknown && validationState == ValidationState.Passed) || validationState2 == ValidationState.Failed)
			{
				validationState = validationState2;
			}
		}
		return validationState;
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06001377 RID: 4983 RVA: 0x00062C28 File Offset: 0x00060E28
	// (set) Token: 0x06001378 RID: 4984 RVA: 0x00062C30 File Offset: 0x00060E30
	public ValidationState ValidationState { get; private set; }

	// Token: 0x06001379 RID: 4985 RVA: 0x00062C3C File Offset: 0x00060E3C
	private bool Verify()
	{
		foreach (PropSpawner propSpawner in base.GetComponentsInChildren<PropSpawner>())
		{
			if (propSpawner.props == null)
			{
				Debug.LogError("Missing spawns on " + propSpawner.name, propSpawner.gameObject);
				return false;
			}
			GameObject[] props = propSpawner.props;
			for (int j = 0; j < props.Length; j++)
			{
				if (props[j] == null)
				{
					Debug.LogError("Missing prefab on " + propSpawner.name, propSpawner.gameObject);
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x00062CCC File Offset: 0x00060ECC
	public void ClearAll()
	{
		LevelGenStep[] componentsInChildren = base.GetComponentsInChildren<LevelGenStep>(true);
		int num = 0;
		foreach (LevelGenStep levelGenStep in componentsInChildren)
		{
			if (!(levelGenStep == null))
			{
				levelGenStep.Clear();
				num++;
			}
		}
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x00062D10 File Offset: 0x00060F10
	[CompilerGenerated]
	private void <RunAll>g__Done|3_0(ref PropGrouper.<>c__DisplayClass3_0 A_1)
	{
		foreach (LevelGenStep levelGenStep in A_1.late)
		{
			levelGenStep.Execute();
			if (levelGenStep.DeferredTiming != DeferredStepTiming.None)
			{
				this.<RunAll>g__AddToStepList|3_2(levelGenStep.DeferredTiming, levelGenStep.ConstructDeferred(levelGenStep), ref A_1);
			}
		}
		this.<RunAll>g__ExecuteAndClearDeferredStepsFor|3_1(DeferredStepTiming.AfterCurrentGroupTiming, ref A_1);
		if (this.ValidateAfterwards)
		{
			this.Validate();
		}
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x00062D94 File Offset: 0x00060F94
	[CompilerGenerated]
	private void <RunAll>g__ExecuteAndClearDeferredStepsFor|3_1(DeferredStepTiming key, ref PropGrouper.<>c__DisplayClass3_0 A_2)
	{
		if (!A_2.deferredSteps.ContainsKey(key))
		{
			return;
		}
		Debug.Log(string.Format("Executing {0} steps now that group is finished.", A_2.deferredSteps[key].Count));
		foreach (IDeferredStep deferredStep in A_2.deferredSteps[key])
		{
			deferredStep.DeferredGo();
		}
		A_2.deferredSteps[key].Clear();
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x00062E30 File Offset: 0x00061030
	[CompilerGenerated]
	private void <RunAll>g__AddToStepList|3_2(DeferredStepTiming key, IDeferredStep stepToAdd, ref PropGrouper.<>c__DisplayClass3_0 A_3)
	{
		if (!A_3.deferredSteps.ContainsKey(key))
		{
			A_3.deferredSteps.Add(key, new List<IDeferredStep>());
		}
		A_3.deferredSteps[key].Add(stepToAdd);
	}

	// Token: 0x0400120D RID: 4621
	public PropGrouper.PropGrouperTiming timing;

	// Token: 0x0400120E RID: 4622
	[SerializeField]
	private bool ValidateAfterwards;

	// Token: 0x020004F7 RID: 1271
	public enum PropGrouperTiming
	{
		// Token: 0x04001B26 RID: 6950
		Early,
		// Token: 0x04001B27 RID: 6951
		Late
	}
}
