using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x02000161 RID: 353
public static class Rebinding
{
	// Token: 0x06000B41 RID: 2881 RVA: 0x0003BFA4 File Offset: 0x0003A1A4
	public static void LoadRebindingsFromFile(InputActionAsset actions = null)
	{
		if (actions == null)
		{
			actions = InputSystem.actions;
		}
		string @string = PlayerPrefs.GetString("rebinds");
		if (!string.IsNullOrEmpty(@string))
		{
			InputActionRebindingExtensions.LoadBindingOverridesFromJson(actions, @string, true);
		}
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x0003BFDC File Offset: 0x0003A1DC
	public static void SaveRebindingsToFile(InputActionAsset actions = null)
	{
		if (actions == null)
		{
			actions = InputSystem.actions;
		}
		string text = InputActionRebindingExtensions.SaveBindingOverridesAsJson(actions);
		PlayerPrefs.SetString("rebinds", text);
	}
}
