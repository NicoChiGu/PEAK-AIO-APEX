using System;
using Photon.Voice.Unity.Demos.DemoVoiceUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	// Token: 0x0200038F RID: 911
	public static class UiExtensions
	{
		// Token: 0x0600175B RID: 5979 RVA: 0x0007707E File Offset: 0x0007527E
		public static void SetPosX(this RectTransform rectTransform, float x)
		{
			rectTransform.anchoredPosition3D = new Vector3(x, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z);
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x000770A2 File Offset: 0x000752A2
		public static void SetHeight(this RectTransform rectTransform, float h)
		{
			rectTransform.SetSizeWithCurrentAnchors(1, h);
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x000770AC File Offset: 0x000752AC
		public static void SetValue(this Toggle toggle, bool isOn)
		{
			toggle.SetIsOnWithoutNotify(isOn);
		}

		// Token: 0x0600175E RID: 5982 RVA: 0x000770B5 File Offset: 0x000752B5
		public static void SetValue(this Slider slider, float v)
		{
			slider.SetValueWithoutNotify(v);
		}

		// Token: 0x0600175F RID: 5983 RVA: 0x000770BE File Offset: 0x000752BE
		public static void SetValue(this InputField inputField, string v)
		{
			inputField.SetTextWithoutNotify(v);
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x000770C8 File Offset: 0x000752C8
		public static void DestroyChildren(this Transform transform)
		{
			if (null != transform && transform)
			{
				for (int i = transform.childCount - 1; i >= 0; i--)
				{
					Transform child = transform.GetChild(i);
					if (child && child.gameObject)
					{
						Object.Destroy(child.gameObject);
					}
				}
				transform.DetachChildren();
			}
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x00077127 File Offset: 0x00075327
		public static void Hide(this CanvasGroup canvasGroup, bool blockRaycasts = false, bool interactable = false)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = blockRaycasts;
			canvasGroup.interactable = interactable;
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x00077142 File Offset: 0x00075342
		public static void Show(this CanvasGroup canvasGroup, bool blockRaycasts = true, bool interactable = true)
		{
			canvasGroup.alpha = 1f;
			canvasGroup.blocksRaycasts = blockRaycasts;
			canvasGroup.interactable = interactable;
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x0007715D File Offset: 0x0007535D
		public static bool IsHidden(this CanvasGroup canvasGroup)
		{
			return canvasGroup.alpha <= 0f;
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x0007716F File Offset: 0x0007536F
		public static bool IsShown(this CanvasGroup canvasGroup)
		{
			return canvasGroup.alpha > 0f;
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x0007717E File Offset: 0x0007537E
		public static void SetSingleOnClickCallback(this Button button, UnityAction action)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(action);
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x00077197 File Offset: 0x00075397
		public static void SetSingleOnValueChangedCallback(this Toggle toggle, UnityAction<bool> action)
		{
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(action);
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x000771B0 File Offset: 0x000753B0
		public static void SetSingleOnValueChangedCallback(this InputField inputField, UnityAction<string> action)
		{
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(action);
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x000771C9 File Offset: 0x000753C9
		public static void SetSingleOnEndEditCallback(this InputField inputField, UnityAction<string> action)
		{
			inputField.onEndEdit.RemoveAllListeners();
			inputField.onEndEdit.AddListener(action);
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x000771E2 File Offset: 0x000753E2
		public static void SetSingleOnValueChangedCallback(this Dropdown inputField, UnityAction<int> action)
		{
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(action);
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x000771FB File Offset: 0x000753FB
		public static void SetSingleOnValueChangedCallback(this Slider slider, UnityAction<float> action)
		{
			slider.onValueChanged.RemoveAllListeners();
			slider.onValueChanged.AddListener(action);
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x00077214 File Offset: 0x00075414
		public static void SetSingleOnValueChangedCallback(this MicrophoneSelector selector, UnityAction<MicType, DeviceInfo> action)
		{
			selector.onValueChanged.RemoveAllListeners();
			selector.onValueChanged.AddListener(action);
		}
	}
}
