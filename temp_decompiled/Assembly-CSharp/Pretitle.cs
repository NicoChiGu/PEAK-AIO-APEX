using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Token: 0x020002CC RID: 716
public class Pretitle : MonoBehaviour
{
	// Token: 0x06001357 RID: 4951 RVA: 0x0006266E File Offset: 0x0006086E
	private void Start()
	{
		base.StartCoroutine(this.PreloadScene());
		base.StartCoroutine(this.LoadTitle());
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x0006268A File Offset: 0x0006088A
	private IEnumerator PreloadScene()
	{
		AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Title", 0);
		loadSceneAsync.allowSceneActivation = false;
		while (!this.allowedToSwitch)
		{
			yield return null;
		}
		loadSceneAsync.allowSceneActivation = true;
		yield break;
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x00062699 File Offset: 0x00060899
	private IEnumerator LoadTitle()
	{
		yield return new WaitForSecondsRealtime(this.loadWait);
		this.allowedToSwitch = true;
		yield break;
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x000626A8 File Offset: 0x000608A8
	private void Update()
	{
		bool flag = Input.GetKeyDown(32) || Input.GetKeyDown(27);
		if (!flag)
		{
			InputActionReference[] array = this.skipKeys;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].action.WasPressedThisFrame())
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			this.allowedToSwitch = true;
		}
	}

	// Token: 0x04001202 RID: 4610
	public InputActionReference[] skipKeys;

	// Token: 0x04001203 RID: 4611
	public float loadWait = 11f;

	// Token: 0x04001204 RID: 4612
	private bool allowedToSwitch;
}
